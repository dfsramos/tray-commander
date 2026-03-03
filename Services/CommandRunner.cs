using System.Diagnostics;
using TrayCommander.Models;

namespace TrayCommander.Services;

public static class CommandRunner
{
    public static void Run(CommandEntry entry, bool isElevated)
    {
        ProcessStartInfo psi = entry.Runner.ToLowerInvariant() switch
        {
            "wsl" => new ProcessStartInfo("wsl.exe", $"-e bash -c \"{entry.Command}\""),
            _     => new ProcessStartInfo("cmd.exe", $"/c {entry.Command}"),
        };

        psi.UseShellExecute = true;

        if (entry.RequiresAdmin && !isElevated)
            psi.Verb = "runas";

        Process.Start(psi);
    }
}
