# Decision: Chart Images Replace ASCII Art in Executive Summary

**By:** Beast
**Date:** 2026-03-11
**Status:** Implemented

## What

The `EXECUTIVE-SUMMARY.md` performance charts are now PNG images (not ASCII art). Three chart files live in `dev-docs/migration-tests/images/` and are referenced via standard Markdown image syntax. A Python script (`generate-charts.py`) in the same directory regenerates them.

## Why

ASCII art charts are hard to maintain, don't render well in all Markdown viewers, and aren't presentation-ready. Real chart images with trend lines, annotations, and professional styling communicate the performance story more effectively to stakeholders.

## Impact on Other Agents

- **When new benchmark runs are added:** Update the data arrays at the top of `generate-charts.py` and re-run `python generate-charts.py` to regenerate all three charts.
- **Prerequisite:** `pip install matplotlib` (one-time install).
- **File paths to know:**
  - `dev-docs/migration-tests/images/wingtiptoys-layer1-perf.png`
  - `dev-docs/migration-tests/images/contosouniversity-layer1-perf.png`
  - `dev-docs/migration-tests/images/combined-improvement.png`
  - `dev-docs/migration-tests/images/generate-charts.py`
