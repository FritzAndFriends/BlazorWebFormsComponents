#!/usr/bin/env node

/**
 * HTML Normalization Pipeline for Web Forms ↔ Blazor Fidelity Audit
 *
 * Normalizes captured HTML files so that known, intentional divergences
 * (see planning-docs/DIVERGENCE-REGISTRY.md) are stripped before comparison.
 *
 * M12-02 Enhancement (Tier 2 Data Controls):
 *   - Pager/sort link normalization: postback hrefs fully stripped (not replaced)
 *   - Auto-generated ID normalization: handles MainContent_ prefixes and _ctl01_ middle segments
 *   - Event handler stripping: all on* attributes removed
 *   - Blazor enhanced navigation attribute stripping (data-enhance-*)
 *   - Form action attribute stripping
 *   - Table legacy attribute stripping (cellpadding, cellspacing, rules, border)
 *
 * Usage:
 *   node scripts/normalize-html.mjs --input <file-or-dir> --output <file-or-dir>
 *   node scripts/normalize-html.mjs --compare <dir1> <dir2> --report <report.md>
 */

import { readFileSync, writeFileSync, mkdirSync, readdirSync, statSync, existsSync } from 'node:fs';
import { resolve, join, dirname, relative, extname, basename, sep } from 'node:path';
import { fileURLToPath } from 'node:url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

// ── Load configuration ──────────────────────────────────────────────────────

const RULES_PATH = join(__dirname, 'normalize-rules.json');

function loadRules() {
  const raw = readFileSync(RULES_PATH, 'utf-8');
  const config = JSON.parse(raw);
  return config.rules.filter(r => r.enabled);
}

// ── Named CSS colors → hex (lowercase) ──────────────────────────────────────

const CSS_COLORS = {
  black: '#000000', silver: '#c0c0c0', gray: '#808080', white: '#ffffff',
  maroon: '#800000', red: '#ff0000', purple: '#800080', fuchsia: '#ff00ff',
  green: '#008000', lime: '#00ff00', olive: '#808000', yellow: '#ffff00',
  navy: '#000080', blue: '#0000ff', teal: '#008080', aqua: '#00ffff',
  orange: '#ffa500', aliceblue: '#f0f8ff', antiquewhite: '#faebd7',
  aquamarine: '#7fffd4', azure: '#f0ffff', beige: '#f5f5dc',
  bisque: '#ffe4c4', blanchedalmond: '#ffebcd', blueviolet: '#8a2be2',
  brown: '#a52a2a', burlywood: '#deb887', cadetblue: '#5f9ea0',
  chartreuse: '#7fff00', chocolate: '#d2691e', coral: '#ff7f50',
  cornflowerblue: '#6495ed', cornsilk: '#fff8dc', crimson: '#dc143c',
  cyan: '#00ffff', darkblue: '#00008b', darkcyan: '#008b8b',
  darkgoldenrod: '#b8860b', darkgray: '#a9a9a9', darkgreen: '#006400',
  darkkhaki: '#bdb76b', darkmagenta: '#8b008b', darkolivegreen: '#556b2f',
  darkorange: '#ff8c00', darkorchid: '#9932cc', darkred: '#8b0000',
  darksalmon: '#e9967a', darkseagreen: '#8fbc8f', darkslateblue: '#483d8b',
  darkslategray: '#2f4f4f', darkturquoise: '#00ced1', darkviolet: '#9400d3',
  deeppink: '#ff1493', deepskyblue: '#00bfff', dimgray: '#696969',
  dodgerblue: '#1e90ff', firebrick: '#b22222', floralwhite: '#fffaf0',
  forestgreen: '#228b22', gainsboro: '#dcdcdc', ghostwhite: '#f8f8ff',
  gold: '#ffd700', goldenrod: '#daa520', greenyellow: '#adff2f',
  honeydew: '#f0fff0', hotpink: '#ff69b4', indianred: '#cd5c5c',
  indigo: '#4b0082', ivory: '#fffff0', khaki: '#f0e68c',
  lavender: '#e6e6fa', lavenderblush: '#fff0f5', lawngreen: '#7cfc00',
  lemonchiffon: '#fffacd', lightblue: '#add8e6', lightcoral: '#f08080',
  lightcyan: '#e0ffff', lightgoldenrodyellow: '#fafad2', lightgray: '#d3d3d3',
  lightgreen: '#90ee90', lightpink: '#ffb6c1', lightsalmon: '#ffa07a',
  lightseagreen: '#20b2aa', lightskyblue: '#87cefa', lightslategray: '#778899',
  lightsteelblue: '#b0c4de', lightyellow: '#ffffe0', limegreen: '#32cd32',
  linen: '#faf0e6', magenta: '#ff00ff', mediumaquamarine: '#66cdaa',
  mediumblue: '#0000cd', mediumorchid: '#ba55d3', mediumpurple: '#9370db',
  mediumseagreen: '#3cb371', mediumslateblue: '#7b68ee',
  mediumspringgreen: '#00fa9a', mediumturquoise: '#48d1cc',
  mediumvioletred: '#c71585', midnightblue: '#191970', mintcream: '#f5fffa',
  mistyrose: '#ffe4e1', moccasin: '#ffe4b5', navajowhite: '#ffdead',
  oldlace: '#fdf5e6', olivedrab: '#6b8e23', orangered: '#ff4500',
  orchid: '#da70d6', palegoldenrod: '#eee8aa', palegreen: '#98fb98',
  paleturquoise: '#afeeee', palevioletred: '#db7093', papayawhip: '#ffefd5',
  peachpuff: '#ffdab9', peru: '#cd853f', pink: '#ffc0cb', plum: '#dda0dd',
  powderblue: '#b0e0e6', rosybrown: '#bc8f8f', royalblue: '#4169e1',
  saddlebrown: '#8b4513', salmon: '#fa8072', sandybrown: '#f4a460',
  seagreen: '#2e8b57', seashell: '#fff5ee', sienna: '#a0522d',
  skyblue: '#87ceeb', slateblue: '#6a5acd', slategray: '#708090',
  snow: '#fffafa', springgreen: '#00ff7f', steelblue: '#4682b4',
  tan: '#d2b48c', thistle: '#d8bfd8', tomato: '#ff6347',
  turquoise: '#40e0d0', violet: '#ee82ee', wheat: '#f5deb3',
  whitesmoke: '#f5f5f5', yellowgreen: '#9acd32',
  grey: '#808080', darkgrey: '#a9a9a9', dimgrey: '#696969',
  lightgrey: '#d3d3d3', lightslategrey: '#778899', slategrey: '#708090'
};

// ── Regex-based normalization transforms ────────────────────────────────────

/**
 * Apply all regex-based rules from normalize-rules.json
 */
function applyRegexRules(html, rules) {
  for (const rule of rules) {
    if (rule.pattern === 'CUSTOM_FUNCTION') continue; // handled separately
    const flags = rule.flags || 'g';
    const re = new RegExp(rule.pattern, flags);
    html = html.replace(re, rule.replacement);
  }
  return html;
}

/**
 * Normalize CSS style attribute values:
 * - Lowercase property names and values
 * - Normalize named colors to hex
 * - Normalize hex colors to lowercase 6-digit
 * - Ensure trailing semicolons, trim spacing
 */
function normalizeStyleAttributes(html) {
  return html.replace(/\bstyle="([^"]*)"/gi, (_match, styleVal) => {
    let normalized = styleVal
      .split(';')
      .map(decl => decl.trim())
      .filter(Boolean)
      .map(decl => {
        const colonIdx = decl.indexOf(':');
        if (colonIdx === -1) return decl.toLowerCase();
        const prop = decl.slice(0, colonIdx).trim().toLowerCase();
        let val = decl.slice(colonIdx + 1).trim().toLowerCase();
        // Named color → hex
        val = val.replace(/\b([a-z]+)\b/g, (m) => CSS_COLORS[m] || m);
        // 3-digit hex → 6-digit hex
        val = val.replace(/#([0-9a-f])([0-9a-f])([0-9a-f])(?![0-9a-f])/g,
          (_m, r, g, b) => `#${r}${r}${g}${g}${b}${b}`);
        return `${prop}:${val}`;
      })
      .join('; ');
    if (normalized && !normalized.endsWith(';')) normalized += ';';
    return `style="${normalized}"`;
  });
}

/**
 * Sort attributes alphabetically within each HTML element.
 * Handles both quoted and unquoted attribute values.
 */
function sortAttributes(html) {
  // Match opening tags (not closing, not comments, not doctype)
  return html.replace(/<([a-zA-Z][a-zA-Z0-9]*)((?:\s+[^>]*?)?)(\s*\/?\s*)>/g,
    (_match, tagName, attrStr, closing) => {
      if (!attrStr || !attrStr.trim()) return `<${tagName}${closing}>`;

      // Parse attributes: name="value", name='value', name=value, or bare name
      const attrRegex = /([a-zA-Z_:][\w:.-]*)(?:\s*=\s*(?:"([^"]*)"|'([^']*)'|(\S+)))?/g;
      const attrs = [];
      let m;
      while ((m = attrRegex.exec(attrStr)) !== null) {
        const name = m[1];
        const value = m[2] ?? m[3] ?? m[4] ?? null;
        attrs.push({ name, value });
      }

      attrs.sort((a, b) => a.name.localeCompare(b.name));

      const attrString = attrs
        .map(a => a.value !== null ? `${a.name}="${a.value}"` : a.name)
        .join(' ');

      return `<${tagName} ${attrString}${closing}>`;
    }
  );
}

/**
 * Normalize whitespace:
 * - Collapse runs of whitespace to single space within tags
 * - Trim each line
 * - Remove blank lines
 * - Consistent newline-per-element
 */
function normalizeWhitespace(html) {
  // Remove blank lines and trim each line
  let lines = html.split(/\r?\n/).map(l => l.trim()).filter(Boolean);
  let result = lines.join('\n');
  // Collapse multiple spaces within tags to single space
  result = result.replace(/<([^>]+)>/g, (_m, inner) => {
    return '<' + inner.replace(/\s{2,}/g, ' ').trim() + '>';
  });
  return result;
}

/**
 * Clean up residual empty attribute artifacts left by regex stripping.
 * e.g., double-spaces from removed attributes.
 */
function cleanupArtifacts(html) {
  // Collapse multiple spaces within tags
  html = html.replace(/<([^>]+)>/g, (_m, inner) => {
    return '<' + inner.replace(/\s{2,}/g, ' ').trim() + '>';
  });
  // Remove empty tags that had all content stripped (e.g., empty <script></script>)
  html = html.replace(/<(script|style)[^>]*>\s*<\/\1>/gi, '');
  return html;
}

// ── Main normalization pipeline ─────────────────────────────────────────────

function normalizeHtml(html, rules) {
  html = applyRegexRules(html, rules);
  html = normalizeStyleAttributes(html);
  html = sortAttributes(html);
  html = cleanupArtifacts(html);
  html = normalizeWhitespace(html);
  return html;
}

// ── File system helpers ─────────────────────────────────────────────────────

function collectHtmlFiles(dir) {
  const files = [];
  function walk(d) {
    for (const entry of readdirSync(d)) {
      const full = join(d, entry);
      if (statSync(full).isDirectory()) {
        walk(full);
      } else if (extname(full).toLowerCase() === '.html') {
        files.push(full);
      }
    }
  }
  walk(dir);
  return files;
}

function isDirectory(p) {
  return existsSync(p) && statSync(p).isDirectory();
}

function isFile(p) {
  return existsSync(p) && statSync(p).isFile();
}

function ensureDir(p) {
  mkdirSync(p, { recursive: true });
}

// ── CLI argument parsing ────────────────────────────────────────────────────

function parseArgs(argv) {
  const args = { mode: null, input: null, output: null, compareA: null, compareB: null, report: null };
  let i = 2; // skip node + script

  while (i < argv.length) {
    switch (argv[i]) {
      case '--input':
        args.mode = 'normalize';
        args.input = argv[++i];
        break;
      case '--output':
        args.output = argv[++i];
        break;
      case '--compare':
        args.mode = 'compare';
        args.compareA = argv[++i];
        args.compareB = argv[++i];
        break;
      case '--report':
        args.report = argv[++i];
        break;
      default:
        console.error(`Unknown argument: ${argv[i]}`);
        process.exit(1);
    }
    i++;
  }

  return args;
}

// ── Normalize mode ──────────────────────────────────────────────────────────

function runNormalize(inputPath, outputPath, rules) {
  inputPath = resolve(inputPath);
  outputPath = resolve(outputPath);

  if (isFile(inputPath)) {
    // Single file
    const html = readFileSync(inputPath, 'utf-8');
    const normalized = normalizeHtml(html, rules);
    ensureDir(dirname(outputPath));
    writeFileSync(outputPath, normalized, 'utf-8');
    console.log(`Normalized: ${inputPath} → ${outputPath}`);
    return;
  }

  if (isDirectory(inputPath)) {
    const files = collectHtmlFiles(inputPath);
    if (files.length === 0) {
      console.log(`No HTML files found in ${inputPath}`);
      return;
    }
    for (const file of files) {
      const rel = relative(inputPath, file);
      const outFile = join(outputPath, rel);
      const html = readFileSync(file, 'utf-8');
      const normalized = normalizeHtml(html, rules);
      ensureDir(dirname(outFile));
      writeFileSync(outFile, normalized, 'utf-8');
      console.log(`  ${rel}`);
    }
    console.log(`\nNormalized ${files.length} file(s) → ${outputPath}`);
    return;
  }

  console.error(`Input path not found: ${inputPath}`);
  process.exit(1);
}

// ── Compare mode ────────────────────────────────────────────────────────────

function simpleDiff(linesA, linesB) {
  const diff = [];
  const maxLen = Math.max(linesA.length, linesB.length);
  for (let i = 0; i < maxLen; i++) {
    const a = i < linesA.length ? linesA[i] : undefined;
    const b = i < linesB.length ? linesB[i] : undefined;
    if (a === b) continue;
    if (a !== undefined && b !== undefined) {
      diff.push(`- ${a}`);
      diff.push(`+ ${b}`);
    } else if (a !== undefined) {
      diff.push(`- ${a}`);
    } else {
      diff.push(`+ ${b}`);
    }
  }
  return diff;
}

function runCompare(dirA, dirB, reportPath) {
  dirA = resolve(dirA);
  dirB = resolve(dirB);

  if (!isDirectory(dirA)) {
    console.error(`Directory not found: ${dirA}`);
    process.exit(1);
  }
  if (!isDirectory(dirB)) {
    console.error(`Directory not found: ${dirB}`);
    process.exit(1);
  }

  const filesA = collectHtmlFiles(dirA).map(f => relative(dirA, f));
  const filesB = collectHtmlFiles(dirB).map(f => relative(dirB, f));
  const allFiles = [...new Set([...filesA, ...filesB])].sort();

  // Group files by control (first path segment)
  const controls = {};
  for (const f of allFiles) {
    const parts = f.split(sep);
    const control = parts.length > 1 ? parts[0] : '(root)';
    const variant = basename(f, extname(f));
    if (!controls[control]) controls[control] = [];
    controls[control].push({ rel: f, variant });
  }

  let totalCompared = 0;
  let exactMatches = 0;
  let divergences = 0;
  const reportLines = [];

  reportLines.push('# HTML Audit Comparison Report');
  reportLines.push('');
  reportLines.push(`Generated: ${new Date().toISOString()}`);
  reportLines.push('');

  // We'll build the summary after scanning
  const controlResults = [];

  for (const [control, variants] of Object.entries(controls).sort(([a], [b]) => a.localeCompare(b))) {
    const rows = [];
    const diffSections = [];

    for (const { rel, variant } of variants) {
      totalCompared++;
      const pathA = join(dirA, rel);
      const pathB = join(dirB, rel);

      if (!existsSync(pathA)) {
        divergences++;
        rows.push(`| ${variant} | ❌ Missing in source A | File only exists in second directory |`);
        continue;
      }
      if (!existsSync(pathB)) {
        divergences++;
        rows.push(`| ${variant} | ❌ Missing in source B | File only exists in first directory |`);
        continue;
      }

      const contentA = readFileSync(pathA, 'utf-8').trim();
      const contentB = readFileSync(pathB, 'utf-8').trim();

      if (contentA === contentB) {
        exactMatches++;
        rows.push(`| ${variant} | ✅ Match | - |`);
      } else {
        divergences++;
        const linesA = contentA.split('\n');
        const linesB = contentB.split('\n');
        const diff = simpleDiff(linesA, linesB);
        const summary = diff.length > 20
          ? `${diff.length} line differences`
          : 'Tag structure differs';
        rows.push(`| ${variant} | ⚠️ Divergent | ${summary} |`);

        // Limit diff output to keep report readable
        const displayDiff = diff.slice(0, 40);
        diffSections.push({ variant, diff: displayDiff, truncated: diff.length > 40 });
      }
    }

    controlResults.push({ control, rows, diffSections });
  }

  // Summary section
  reportLines.push('## Summary');
  reportLines.push(`- Controls compared: ${totalCompared}`);
  reportLines.push(`- Exact matches: ${exactMatches}`);
  reportLines.push(`- Divergences found: ${divergences}`);
  reportLines.push('');

  // Results by control
  reportLines.push('## Results by Control');
  reportLines.push('');

  for (const { control, rows, diffSections } of controlResults) {
    reportLines.push(`### ${control}`);
    reportLines.push('| Variant | Status | Diff Summary |');
    reportLines.push('|---------|--------|-------------|');
    for (const row of rows) reportLines.push(row);
    reportLines.push('');

    for (const { variant, diff, truncated } of diffSections) {
      reportLines.push(`#### ${variant} Diff`);
      reportLines.push('```diff');
      for (const line of diff) reportLines.push(line);
      if (truncated) reportLines.push('... (truncated)');
      reportLines.push('```');
      reportLines.push('');
    }
  }

  const report = reportLines.join('\n');

  if (reportPath) {
    reportPath = resolve(reportPath);
    ensureDir(dirname(reportPath));
    writeFileSync(reportPath, report, 'utf-8');
    console.log(`Report written: ${reportPath}`);
  } else {
    console.log(report);
  }

  console.log(`\nSummary: ${totalCompared} compared, ${exactMatches} matches, ${divergences} divergences`);
}

// ── Entry point ─────────────────────────────────────────────────────────────

function main() {
  const args = parseArgs(process.argv);

  if (!args.mode) {
    console.log(`HTML Normalization Pipeline for Web Forms ↔ Blazor Fidelity Audit

Usage:
  Normalize a single file:
    node scripts/normalize-html.mjs --input <file.html> --output <out.html>

  Normalize a directory tree:
    node scripts/normalize-html.mjs --input <dir/> --output <outdir/>

  Compare two normalized directories:
    node scripts/normalize-html.mjs --compare <dirA/> <dirB/> --report <report.md>

Options:
  --input   <path>   Input HTML file or directory
  --output  <path>   Output file or directory for normalized HTML
  --compare <a> <b>  Two directories of normalized HTML to compare
  --report  <path>   Path for the Markdown comparison report

Configuration:
  Edit scripts/normalize-rules.json to enable/disable normalization rules.
`);
    process.exit(0);
  }

  if (args.mode === 'normalize') {
    if (!args.input || !args.output) {
      console.error('--input and --output are required for normalization.');
      process.exit(1);
    }
    const rules = loadRules();
    console.log(`Loaded ${rules.length} active normalization rules.`);
    runNormalize(args.input, args.output, rules);
  }

  if (args.mode === 'compare') {
    if (!args.compareA || !args.compareB) {
      console.error('--compare requires two directory arguments.');
      process.exit(1);
    }
    runCompare(args.compareA, args.compareB, args.report);
  }
}

main();
