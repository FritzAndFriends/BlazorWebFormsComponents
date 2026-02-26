#!/usr/bin/env node
/**
 * Capture rendered HTML from AfterBlazorServerSide sample pages running via dotnet.
 *
 * Usage:
 *   node scripts/capture-blazor.mjs [--url https://localhost:7235] [--output audit-output/blazor] [--pages Button,Calendar]
 */

import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { parseArgs, runCapture } from './capture-utils.mjs';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const repoRoot = path.resolve(__dirname, '..');

const defaults = {
  url: 'https://localhost:7235',
  output: 'audit-output/blazor',
};

const { url, output, pages } = parseArgs(process.argv, defaults);

const samplesDir = path.join(repoRoot, 'samples', 'AfterBlazorServerSide', 'Pages', 'ControlSamples');

await runCapture({
  baseUrl: url,
  outputDir: output,
  pageNames: pages,
  samplesDir,
  // Blazor uses friendly routes at /ControlSamples/{ComponentName}
  buildUrl: (controlName) => `${url}/ControlSamples/${controlName}`,
  ignoreHttpsErrors: true,
});
