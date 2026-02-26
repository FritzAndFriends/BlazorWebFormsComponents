/**
 * Shared capture utilities for extracting rendered HTML and screenshots
 * from sample pages that use data-audit-control markers.
 */

import { chromium } from 'playwright';
import fs from 'node:fs';
import path from 'node:path';

/**
 * Parse common CLI arguments for capture scripts.
 * @param {string[]} argv - process.argv
 * @param {object} defaults - default values for url and output
 * @returns {{ url: string, output: string, pages: string[] }}
 */
export function parseArgs(argv, defaults) {
  const args = argv.slice(2);
  let url = defaults.url;
  let output = defaults.output;
  const pages = [];

  for (let i = 0; i < args.length; i++) {
    switch (args[i]) {
      case '--url':
        url = args[++i];
        break;
      case '--output':
        output = args[++i];
        break;
      case '--pages':
        // Comma-separated list of page names
        pages.push(...args[++i].split(',').map(p => p.trim()));
        break;
    }
  }

  return { url: url.replace(/\/+$/, ''), output, pages };
}

/**
 * Discover control sample directories from the filesystem.
 * @param {string} samplesDir - Absolute path to the ControlSamples directory
 * @returns {string[]} - Array of control directory names
 */
export function discoverPages(samplesDir) {
  if (!fs.existsSync(samplesDir)) {
    console.warn(`Samples directory not found: ${samplesDir}`);
    return [];
  }

  return fs.readdirSync(samplesDir, { withFileTypes: true })
    .filter(d => d.isDirectory())
    .map(d => d.name)
    .sort();
}

/**
 * Capture all audit-marked controls from a single page.
 *
 * @param {import('playwright').Page} page - Playwright page instance
 * @param {string} pageUrl - Full URL to navigate to
 * @param {string} controlName - Name of the control (used for output paths)
 * @param {string} outputDir - Root output directory
 * @returns {Promise<{ controls: number, errors: string[] }>}
 */
export async function capturePage(page, pageUrl, controlName, outputDir) {
  const errors = [];
  let controlCount = 0;

  const controlDir = path.join(outputDir, controlName);
  fs.mkdirSync(controlDir, { recursive: true });

  try {
    const response = await page.goto(pageUrl, {
      waitUntil: 'networkidle',
      timeout: 30_000,
    });

    if (!response || response.status() !== 200) {
      const status = response ? response.status() : 'no response';
      const msg = `[SKIP] ${controlName}: HTTP ${status} at ${pageUrl}`;
      console.warn(msg);
      errors.push(msg);
      return { controls: 0, errors };
    }

    // Full-page screenshot
    await page.screenshot({
      path: path.join(controlDir, 'page.png'),
      fullPage: true,
    });

    // Find all audit-marked elements
    const markers = await page.$$('[data-audit-control]');

    for (const marker of markers) {
      try {
        const auditId = await marker.getAttribute('data-audit-control');
        if (!auditId) continue;

        // Extract innerHTML
        const innerHTML = await marker.innerHTML();

        // Save HTML
        const htmlPath = path.join(controlDir, `${auditId}.html`);
        fs.writeFileSync(htmlPath, innerHTML, 'utf-8');

        // Take element screenshot
        try {
          const pngPath = path.join(controlDir, `${auditId}.png`);
          await marker.screenshot({ path: pngPath });
        } catch (screenshotErr) {
          const msg = `[WARN] ${controlName}/${auditId}: screenshot failed — ${screenshotErr.message}`;
          console.warn(msg);
          errors.push(msg);
        }

        controlCount++;
        console.log(`  ✓ ${controlName}/${auditId}`);
      } catch (elErr) {
        const msg = `[ERROR] ${controlName}: element extraction failed — ${elErr.message}`;
        console.error(msg);
        errors.push(msg);
      }
    }

    if (markers.length === 0) {
      console.log(`  (no audit markers found on ${controlName})`);
    }
  } catch (navErr) {
    const msg = `[ERROR] ${controlName}: navigation failed — ${navErr.message}`;
    console.error(msg);
    errors.push(msg);
  }

  return { controls: controlCount, errors };
}

/**
 * Run the full capture pipeline.
 *
 * @param {object} options
 * @param {string} options.baseUrl - Base URL of the running app
 * @param {string} options.outputDir - Root output directory
 * @param {string[]} options.pageNames - Control names to capture (empty = all discovered)
 * @param {string} options.samplesDir - Filesystem path to ControlSamples directory
 * @param {(controlName: string) => string} options.buildUrl - Function to build a page URL from control name
 * @param {boolean} [options.ignoreHttpsErrors] - Whether to ignore HTTPS errors (for Blazor dev certs)
 */
export async function runCapture(options) {
  const { baseUrl, outputDir, pageNames, samplesDir, buildUrl, ignoreHttpsErrors = false } = options;

  // Discover or filter pages
  let pages = discoverPages(samplesDir);
  if (pageNames.length > 0) {
    const requested = new Set(pageNames.map(p => p.toLowerCase()));
    pages = pages.filter(p => requested.has(p.toLowerCase()));
  }

  if (pages.length === 0) {
    console.error('No pages to capture. Check --pages filter or samples directory.');
    process.exit(1);
  }

  console.log(`\nCapture target: ${baseUrl}`);
  console.log(`Output directory: ${outputDir}`);
  console.log(`Pages to capture: ${pages.length}\n`);

  fs.mkdirSync(outputDir, { recursive: true });

  const browser = await chromium.launch();
  const context = await browser.newContext({ ignoreHTTPSErrors: ignoreHttpsErrors });
  const page = await context.newPage();

  let totalPages = 0;
  let totalControls = 0;
  const allErrors = [];

  for (const controlName of pages) {
    const pageUrl = buildUrl(controlName);
    console.log(`→ ${controlName}: ${pageUrl}`);

    const result = await capturePage(page, pageUrl, controlName, outputDir);
    totalPages++;
    totalControls += result.controls;
    allErrors.push(...result.errors);
  }

  await browser.close();

  // Summary
  console.log('\n' + '='.repeat(50));
  console.log('CAPTURE SUMMARY');
  console.log('='.repeat(50));
  console.log(`  Pages captured:   ${totalPages}`);
  console.log(`  Controls found:   ${totalControls}`);
  console.log(`  Errors:           ${allErrors.length}`);

  if (allErrors.length > 0) {
    console.log('\nErrors:');
    for (const err of allErrors) {
      console.log(`  ${err}`);
    }
  }

  console.log(`\nOutput: ${path.resolve(outputDir)}`);
}
