using HybridTest.Services;
using HybridTest.Components;
using HybridTest.Components.Web;
using RazorConsole.Core;

bool bothMode = args.Contains("--both");
bool webMode = args.Contains("--web") || bothMode;
bool consoleMode = !args.Contains("--web") || bothMode;

var tasks = new List<Task>();

// app out here so we can AsyncStop it later.
WebApplication? app = null;

// Little bit of de-duplication
void ConfigureCommonServices(IServiceCollection services, HostingModeKind mode)
{
    services.AddSingleton<IHostingMode>(_ => new HostingMode(mode));
    services.AddSingleton<IHardwareService>(_ => HardwareService.Instance);
}

if (webMode)
{
    var builder = WebApplication.CreateBuilder(args);
    
    if (bothMode)
    {
        // Suppress Log4Net tty output when RazorConsole is in use
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
        // Print the URLs since the Log4Net output is suppressed
        await app.StartAsync();
        foreach (var url in app.Urls)
        {
            Console.WriteLine($"Now listening on: {url}");
        }
    }
    else
    {
        tasks.Add(app.RunAsync());
    }
}

if (consoleMode)
{
    var hostBuilder = Host.CreateDefaultBuilder(args)
        .ConfigureServices(s => ConfigureCommonServices(s, HostingModeKind.Console))
        .UseRazorConsole<Counter>();

    var host = hostBuilder.Build();
    tasks.Add(host.RunAsync());
}

await Task.WhenAll(tasks);

Console.WriteLine("Shutdown sequence initiated...");

if (bothMode)
{
    // When the console host exits, stop the web host too
    Console.WriteLine("If stuck here, check if the web page is open somewhere.");
    await app!.StopAsync();
}

Console.WriteLine("And we're done.");

