# Build Output — Run 3

**Duration:** 11.9 seconds
**Result:** ✅ Build succeeded — 0 errors, 63 warnings (all in BWFC library)

```
Build succeeded with 63 warning(s) in 11.6s
  WingtipToys net10.0 succeeded (2.0s) → bin\Debug\net10.0\WingtipToys.dll
  BlazorWebFormsComponents net10.0 succeeded with 63 warning(s) (7.5s)
```

All 63 warnings are in `BlazorWebFormsComponents` (nullable annotations, obsolete members, BL0005/BL0007 component parameter warnings) — none in the migrated WingtipToys app.
