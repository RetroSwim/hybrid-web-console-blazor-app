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
    tasks.Add(host.RunAsync());
}

await Task.WhenAll(tasks);

if (bothMode)
{
    // When the console host exits, stop the web host too
    await app!.StopAsync();
}
