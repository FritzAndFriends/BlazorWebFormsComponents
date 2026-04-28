using Microsoft.Playwright;
using System.Diagnostics;

namespace AfterBlazorServerSide.Tests;

[CollectionDefinition(nameof(WingtipToysPlaywrightCollection))]
public class WingtipToysPlaywrightCollection : ICollectionFixture<WingtipToysPlaywrightFixture>
{
}

public class WingtipToysPlaywrightFixture : IAsyncLifetime
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private Process? _serverProcess;
    private const int ServerPort = 5556;

    public string BaseUrl => $"http://localhost:{ServerPort}";

    public async Task InitializeAsync()
    {
        var dllPath = ResolveDllPath();

        _serverProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{dllPath}\" --urls {BaseUrl}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(dllPath)
            }
        };

        _serverProcess.Start();

        _serverProcess.OutputDataReceived += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine($"Wingtip Server Output: {e.Data}");
            }
        };
        _serverProcess.ErrorDataReceived += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine($"Wingtip Server Error: {e.Data}");
            }
        };
        _serverProcess.BeginOutputReadLine();
        _serverProcess.BeginErrorReadLine();

        using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        var isReady = false;

        for (var attempt = 0; attempt < 60 && !isReady; attempt++)
        {
            try
            {
                var response = await httpClient.GetAsync(BaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    isReady = content.Contains("</html>", StringComparison.OrdinalIgnoreCase)
                        || content.Contains("Wingtip Toys", StringComparison.Ordinal);
                }
            }
            catch (Exception ex) when (attempt < 59)
            {
                Console.WriteLine($"Wingtip startup attempt {attempt + 1}: {ex.Message}");
            }

            if (!isReady)
            {
                await Task.Delay(1000);
            }
        }

        if (!isReady)
        {
            throw new Exception("WingtipToys server failed to start within 60 seconds.");
        }

        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
    }

    public async Task DisposeAsync()
    {
        if (_browser is not null)
        {
            await _browser.CloseAsync();
        }

        _playwright?.Dispose();

        if (_serverProcess is not null && !_serverProcess.HasExited)
        {
            _serverProcess.Kill(true);
            _serverProcess.WaitForExit(5000);
            _serverProcess.Dispose();
        }
    }

    public async Task<IPage> NewPageAsync()
    {
        if (_browser is null)
        {
            throw new InvalidOperationException("Browser not initialized");
        }

        return await _browser.NewPageAsync();
    }

    private static string ResolveDllPath()
    {
        foreach (var configuration in new[] { "Release", "Debug" })
        {
            var candidate = Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory,
                "..", "..", "..", "..",
                "AfterWingtipToys",
                "bin",
                configuration,
                "net10.0",
                "WingtipToys.dll"));

            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        throw new FileNotFoundException("Could not find WingtipToys server DLL in Release or Debug output.");
    }
}
