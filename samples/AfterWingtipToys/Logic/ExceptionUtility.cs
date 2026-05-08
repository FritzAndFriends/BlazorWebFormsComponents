using System.Text;

namespace WingtipToys.Logic;

public sealed class ExceptionUtility
{
    private ExceptionUtility()
    {
    }

    public static void LogException(Exception exc, string source)
    {
        var appDataPath = Path.Combine(AppContext.BaseDirectory, "App_Data");
        Directory.CreateDirectory(appDataPath);
        var logFile = Path.Combine(appDataPath, "ErrorLog.txt");

        var builder = new StringBuilder();
        builder.AppendLine($"********** {DateTime.Now} **********");
        builder.AppendLine($"Exception Type: {exc.GetType()}");
        builder.AppendLine($"Exception: {exc.Message}");
        builder.AppendLine($"Source: {source}");
        if (!string.IsNullOrWhiteSpace(exc.StackTrace))
        {
            builder.AppendLine("Stack Trace:");
            builder.AppendLine(exc.StackTrace);
        }
        if (exc.InnerException is not null)
        {
            builder.AppendLine($"Inner Exception Type: {exc.InnerException.GetType()}");
            builder.AppendLine($"Inner Exception: {exc.InnerException.Message}");
        }
        builder.AppendLine();

        File.AppendAllText(logFile, builder.ToString());
    }
}
