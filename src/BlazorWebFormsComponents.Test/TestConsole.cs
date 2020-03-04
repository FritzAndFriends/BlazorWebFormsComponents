using System;

namespace BlazorWebFormsComponents.Test {

  public static class TestConsole {

    public static void WriteLine(string text) {

      if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production")
        Console.WriteLine(text);

    }

  }

}