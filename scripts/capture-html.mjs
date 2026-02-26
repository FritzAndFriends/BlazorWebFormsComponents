#!/usr/bin/env node
/**
 * Capture rendered HTML from BeforeWebForms sample pages running on IIS Express.
 *
 * Usage:
 *   node scripts/capture-html.mjs [--url http://localhost:55501] [--output audit-output/webforms] [--pages Button,Calendar]
 */

import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { parseArgs, runCapture } from './capture-utils.mjs';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const repoRoot = path.resolve(__dirname, '..');

const defaults = {
  url: 'http://localhost:55501',
  output: 'audit-output/webforms',
};

const { url, output, pages } = parseArgs(process.argv, defaults);

const samplesDir = path.join(repoRoot, 'samples', 'BeforeWebForms', 'ControlSamples');

await runCapture({
  baseUrl: url,
  outputDir: output,
  pageNames: pages,
  samplesDir,
  buildUrl: (controlName) => `${url}/ControlSamples/${controlName}/Default`,
  ignoreHttpsErrors: false,
});
