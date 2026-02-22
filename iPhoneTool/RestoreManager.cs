using System.Diagnostics;

namespace iPhoneTool;

public class RestoreManager
{
    private readonly string _binPath;

    public RestoreManager(string binPath)
    {
        _binPath = binPath;
    }

    public async Task<string> RestoreAsync(string udid, string ipswPath)
    {
        if (!File.Exists(ipswPath))
        {
            throw new FileNotFoundException("IPSW file not found.", ipswPath);
        }

        var restoreTool = Path.Combine(_binPath, "idevicerestore.exe");
        if (!File.Exists(restoreTool))
        {
            throw new FileNotFoundException($"Tool not found: {restoreTool}");
        }

        var psi = new ProcessStartInfo
        {
            FileName = restoreTool,
            Arguments = $"-u {udid} \"{ipswPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi) ?? throw new InvalidOperationException("Unable to start restore tool.");
        var stdOut = await process.StandardOutput.ReadToEndAsync();
        var stdErr = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"Restore failed ({process.ExitCode}): {stdErr}");
        }

        return string.IsNullOrWhiteSpace(stdOut) ? "Restore completed." : stdOut.Trim();
    }
}
