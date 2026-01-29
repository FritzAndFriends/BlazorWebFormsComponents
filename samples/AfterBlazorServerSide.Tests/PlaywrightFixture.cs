using Microsoft.Playwright;
using System.Diagnostics;

namespace AfterBlazorServerSide.Tests;

// Collection fixture to share the server and browser across all tests
[CollectionDefinition(nameof(PlaywrightCollection))]
public class PlaywrightCollection : ICollectionFixture<PlaywrightFixture>
{
}

public class PlaywrightFixture : IAsyncLifetime
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private Process? _serverProcess;
    private const int ServerPort = 5555;

    public string BaseUrl => $"http://localhost:{ServerPort}";

    public async Task InitializeAsync()
    {
        // Start the Blazor server application using the built DLL
        var dllPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "AfterBlazorServerSide", "bin", "Release", "net10.0", "AfterBlazorServerSide.dll"));
        
        if (!File.Exists(dllPath))
        {
            throw new FileNotFoundException($"Could not find server DLL at {dllPath}");
        }

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

        // Capture output for debugging
        _serverProcess.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine($"Server Output: {e.Data}");
            }
        };
        _serverProcess.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine($"Server Error: {e.Data}");
            }
        };
        _serverProcess.BeginOutputReadLine();
        _serverProcess.BeginErrorReadLine();

        // Wait for the server to be ready
        var isReady = false;
        var maxAttempts = 60; // Increased to 60 seconds
        var attempt = 0;

        using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        
        while (!isReady && attempt < maxAttempts)
        {
            attempt++;
            try
            {
                var response = await httpClient.GetAsync(BaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    // Check if we got actual HTML content
                    isReady = content.Contains("</html>") || content.Contains("blazor");
                }
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                // Ignore exceptions during startup
                Console.WriteLine($"Attempt {attempt}: {ex.Message}");
                await Task.Delay(1000);
            }
        }

        if (!isReady)
        {
            throw new Exception($"Server failed to start after {maxAttempts} attempts. Check server logs above.");
        }

        // Initialize Playwright
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
    }

    public async Task DisposeAsync()
    {
        if (_browser != null)
        {
            await _browser.CloseAsync();
        }

        _playwright?.Dispose();

        if (_serverProcess != null && !_serverProcess.HasExited)
        {
            _serverProcess.Kill(true);
            _serverProcess.WaitForExit(5000);
            _serverProcess.Dispose();
        }
    }

    public async Task<IPage> NewPageAsync()
    {
        if (_browser == null)
        {
            throw new InvalidOperationException("Browser not initialized");
        }

        return await _browser.NewPageAsync();
    }
}
