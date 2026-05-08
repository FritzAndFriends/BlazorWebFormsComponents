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

		using var sw = new StreamWriter(logFile, append: true, Encoding.UTF8);
		sw.WriteLine("********** {0} **********", DateTime.Now);
		if (exc.InnerException is not null)
		{
			sw.WriteLine("Inner Exception Type: {0}", exc.InnerException.GetType());
			sw.WriteLine("Inner Exception: {0}", exc.InnerException.Message);
			sw.WriteLine("Inner Source: {0}", exc.InnerException.Source);
			if (!string.IsNullOrEmpty(exc.InnerException.StackTrace))
			{
				sw.WriteLine("Inner Stack Trace:");
				sw.WriteLine(exc.InnerException.StackTrace);
			}
		}

		sw.WriteLine("Exception Type: {0}", exc.GetType());
		sw.WriteLine("Exception: {0}", exc.Message);
		sw.WriteLine("Source: {0}", source);
		sw.WriteLine("Stack Trace:");
		if (!string.IsNullOrEmpty(exc.StackTrace))
		{
			sw.WriteLine(exc.StackTrace);
		}
		sw.WriteLine();
	}
}