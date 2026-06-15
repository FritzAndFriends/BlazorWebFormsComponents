using System.Text;

namespace WingtipToys.Logic;

public static class ExceptionUtility
{
  // Create our own utility for exceptions
  public sealed class ExceptionUtility
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ExceptionUtility(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // All methods are static, so this can be private
    private ExceptionUtility()
    { }

    // Log an Exception
    public static void LogException(Exception exc, string source)
    {
      // Include logic for logging exceptions
      // Get the absolute path to the log file
      string logFile = Path.Combine(AppContext.BaseDirectory, "App_Data", "ErrorLog.txt");

      // Open the log file for append and write the log
      StreamWriter sw = new StreamWriter(logFile, true);
      sw.WriteLine("********** {0} **********", DateTime.Now);
      if (exc.InnerException != null)
      {
        sw.Write("Inner Exception Type: ");
        sw.WriteLine(exc.InnerException.GetType().ToString());
        sw.Write("Inner Exception: ");
        sw.WriteLine(exc.InnerException.Message);
        sw.Write("Inner Source: ");
        sw.WriteLine(exc.InnerException.Source);
        if (exc.InnerException.StackTrace != null)
        {
        }
    }
}
