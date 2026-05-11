namespace WingtipToys.Logic;

public static class ExceptionUtility
{
    public static void LogException(Exception exception, string source)
    {
        var logDirectory = Path.Combine(AppContext.BaseDirectory, "App_Data");
        Directory.CreateDirectory(logDirectory);

        var logFile = Path.Combine(logDirectory, "ErrorLog.txt");
        var logEntry = $"********** {DateTime.Now:u} **********{Environment.NewLine}" +
                       $"Exception: {exception.Message}{Environment.NewLine}" +
                       $"Source: {source}{Environment.NewLine}" +
                       $"Stack Trace:{Environment.NewLine}{exception.StackTrace}{Environment.NewLine}{Environment.NewLine}";

        File.AppendAllText(logFile, logEntry);
    }
}
