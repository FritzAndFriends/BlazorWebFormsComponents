using System;

namespace WingtipToys.Logic
{
	public static class ExceptionUtility
	{
		public static void LogException(Exception ex, string source)
		{
			// In the original Web Forms app this logged to a file.
			// In the migrated app we rely on ASP.NET Core logging infrastructure.
			System.Diagnostics.Debug.WriteLine($"[{source}] {ex.Message}");
		}
	}
}
