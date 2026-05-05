namespace HybridTest.Services;

/// <summary>
/// Abstracts hardware access so it can be shared between console and web modes.
/// Both modes run this code on the same machine — Blazor Server executes C# server-side.
/// </summary>
public interface IHardwareService
{
    string GetDeviceStatus();
    double ReadSensorValue();
    void Dispose();
    event EventHandler<CustomEventArgs>? OnHardwareEvent;

}

public class HardwareService : IHardwareService
{
    private bool _disposed;

    public HardwareService() {

        new Thread(() =>
        {
            while (!_disposed)
            {
                Thread.Sleep(1000);
                OnHardwareEvent?.Invoke(this, new CustomEventArgs($"Hardware event at {DateTime.Now}"));
            }
        }).Start();

    }

    // Replace these with real hardware calls (serial ports, GPIO, USB, etc.)
    public string GetDeviceStatus()
    {
        // Example: real C# code accessing hardware
        return $"Device online (PID {Environment.ProcessId})";
    }

    public double ReadSensorValue()
    {
        // Example: simulated sensor reading
        return Random.Shared.NextDouble() * 100.0;
    }

    public event EventHandler<CustomEventArgs>? OnHardwareEvent;

    public void Dispose()
    {
        _disposed = true;
    }
}

public class CustomEventArgs(string message) : EventArgs
{
    public string Message { get; } = message;
}