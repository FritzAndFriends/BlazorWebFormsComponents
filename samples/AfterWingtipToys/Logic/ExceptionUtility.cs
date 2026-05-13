namespace WingtipToys.Logic;

public static class ExceptionUtility
{
    public static void LogException(Exception exc, string source)
    {
        System.Diagnostics.Debug.WriteLine($"[{source}] {exc}");
    }
}
