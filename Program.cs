using HybridTest.Services;
using HybridTest.Components;
using HybridTest.Components.Web;
using RazorConsole.Core;

bool bothMode = args.Contains("--both");
bool webMode = args.Contains("--web") || bothMode;
bool consoleMode = !args.Contains("--web") || bothMode;

var tasks = new List<Task>();
WebApplication? app = null;
IHardwareService hardwareService = new HardwareService();

// Create a cancellation token source for graceful shutdown
using var cts = new CancellationTokenSource();

// Register signal handlers for Ctrl+C (SIGINT) and termination (SIGTERM)
Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
{
    cts.Cancel();
};

void ConfigureCommonServices(IServiceCollection services, HostingModeKind mode)
{
    services.AddSingleton<IHostingMode>(new HostingMode(mode));
    services.AddSingleton(hardwareService);
}

if (webMode)
{
    var webArgs = args.Where(a => a != "--both" && a != "--web").ToArray();
    var builder = WebApplication.CreateBuilder(webArgs);
    if (bothMode)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddDebug();
    }
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    ConfigureCommonServices(builder.Services, HostingModeKind.Web);

    app = builder.Build();
    app.UseStaticFiles();
    app.UseAntiforgery();
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    if (bothMode)
    {
        await app.StartAsync();
        foreach (var url in app.Urls)
        {
            Console.WriteLine($"Now listening on: {url}");
        }
        // Pass the cancellation token so the app stops when signaled
        tasks.Add(app.WaitForShutdownAsync(cts.Token));
    }
    else
    {
        tasks.Add(app.RunAsync());
    }
}

if (consoleMode)
{
    IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args)
        .ConfigureServices((s) => ConfigureCommonServices(s, HostingModeKind.Console))
        .UseRazorConsole<Counter>();

    IHost host = hostBuilder.Build();
    // Pass the cancellation token
    tasks.Add(host.RunAsync(cts.Token));
}

await Task.WhenAll(tasks);

if (bothMode)
{
    // When the console host exits, stop the web host too
    await app!.StopAsync();
}

