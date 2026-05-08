using System.Text;

namespace WingtipToys.Logic;

public static class ExceptionUtility
{
    public static void LogException(Exception exc, string source)
    {
        try
        {
            var directory = Path.Combine(AppContext.BaseDirectory, "App_Data");
            Directory.CreateDirectory(directory);
            var logPath = Path.Combine(directory, "ErrorLog.txt");
            var builder = new StringBuilder()
                .AppendLine($"********** {DateTime.UtcNow:u} **********")
                .AppendLine($"Source: {source}")
                .AppendLine($"Exception: {exc.Message}")
                .AppendLine(exc.ToString())
                .AppendLine();
            File.AppendAllText(logPath, builder.ToString());
        }
        catch
        {
        }
    }
}
