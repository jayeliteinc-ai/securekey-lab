using System.Diagnostics;

namespace iPhoneTool;

public sealed class DeviceInfo
{
    public string UdId { get; init; } = string.Empty;
    public string ProductType { get; init; } = "Unknown";
    public string ProductVersion { get; init; } = "Unknown";
    public string DeviceName { get; init; } = "Unknown";

    public string DisplayName => $"{DeviceName} ({ProductType} / iOS {ProductVersion})";
}

public class DeviceManager
{
    private readonly string _binPath;

    public DeviceManager(string binPath)
    {
        _binPath = binPath;
    }

    public async Task<IReadOnlyList<DeviceInfo>> GetConnectedDevicesAsync()
    {
        var idsResult = await RunToolAsync("idevice_id.exe", "-l");
        var ids = idsResult
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(id => id.Trim())
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToList();

        var devices = new List<DeviceInfo>();
        foreach (var id in ids)
        {
            var name = await QueryInfoAsync(id, "DeviceName");
            var product = await QueryInfoAsync(id, "ProductType");
            var version = await QueryInfoAsync(id, "ProductVersion");

            devices.Add(new DeviceInfo
            {
                UdId = id,
                DeviceName = name,
                ProductType = product,
                ProductVersion = version
            });
        }

        return devices;
    }

    private Task<string> QueryInfoAsync(string udid, string key)
    {
        return RunToolAsync("ideviceinfo.exe", $"-u {udid} -k {key}");
    }

    private async Task<string> RunToolAsync(string toolName, string args)
    {
        var path = Path.Combine(_binPath, toolName);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Tool not found: {path}");
        }

        var psi = new ProcessStartInfo
        {
            FileName = path,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi) ?? throw new InvalidOperationException($"Unable to start {toolName}");
        var stdOut = await process.StandardOutput.ReadToEndAsync();
        var stdErr = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"{toolName} failed ({process.ExitCode}): {stdErr}");
        }

        return string.IsNullOrWhiteSpace(stdOut) ? stdErr.Trim() : stdOut.Trim();
    }
}
