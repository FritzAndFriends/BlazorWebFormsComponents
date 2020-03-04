using System;

namespace BlazorWebFormsComponents.Test {

  public static class TestConsole {

    public static void WriteLine(string text) {

      string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
      if (string.IsNullOrEmpty(envName) || envName != "Production")
        Console.WriteLine(text);

    }

  }

}