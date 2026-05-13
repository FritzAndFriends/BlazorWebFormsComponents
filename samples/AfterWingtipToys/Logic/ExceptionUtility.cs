// Compile-safe stub for quarantined file 'Logic\ExceptionUtility.cs'
// Reason: Legacy HttpContext access indicates runtime assumptions that are unsafe in the generated SSR compile surface.
// Full original source preserved in migration-artifacts/compile-surface/Logic\ExceptionUtility.cs.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WingtipToys.Logic;

/// <summary>
/// Stub for quarantined class. Provides compile compatibility for dependent code.
/// Replace with a proper implementation during Layer 2 migration.
/// </summary>
public class ExceptionUtility
{
    public ExceptionUtility() { }
    public static void LogException(Exception ex, string handler) { }
}
