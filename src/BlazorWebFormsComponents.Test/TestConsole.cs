using System;

namespace BlazorWebFormsComponents.Test {

  public static class TestConsole {
		
    private static string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    private static bool hasTargetEnvironment = (string.IsNullOrEmpty(envName) || envName != "Production");

    public static void WriteLine(string text) {

			System.Diagnostics.Debug.WriteLineIf(hasTargetEnvironment, text);
			return;

    }

  }

}
