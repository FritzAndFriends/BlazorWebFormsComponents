using System;

namespace BlazorWebFormsComponents.Test {
  public static class TestConsole {
    public static void WriteLine(string text) {

#if DEBUG
			Console.WriteLine($"Are you using `dotnet test -c Debug , perhaps you meant to use -c Release");
			Console.WriteLine($"Remember that dotnet test defaults -c to Debug");
			Console.WriteLine($"Console WriteLine with DEBUG {text}");
		#else
			Console.WriteLine("Console WriteLine without DEBUG ");
#endif
			System.Diagnostics.Debug.WriteLine("Debug.WriteLine should only happen in DEBUG mode");
			System.Diagnostics.Debug.WriteLine($"Debug.WriteLine {text}");

    }
  }
}
