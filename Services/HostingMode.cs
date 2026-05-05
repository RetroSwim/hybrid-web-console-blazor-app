namespace HybridTest.Services;

public enum HostingModeKind
{
    Console,
    Web
}

public interface IHostingMode
{
    HostingModeKind Mode { get; }
    bool IsWeb => Mode == HostingModeKind.Web;
    bool IsConsole => Mode == HostingModeKind.Console;
}

public class HostingMode : IHostingMode
{
    public HostingMode(HostingModeKind mode) => Mode = mode;
    public HostingModeKind Mode { get; }
}
