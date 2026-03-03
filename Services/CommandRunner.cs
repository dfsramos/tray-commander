using System.Diagnostics;
using TrayCommander.Models;

namespace TrayCommander.Services;

public static class CommandRunner
{
    public static void Run(CommandEntry entry, bool isElevated)
    {
        bool hasScript = !string.IsNullOrWhiteSpace(entry.ScriptPath);

        ProcessStartInfo psi = (entry.Runner.ToLowerInvariant(), hasScript) switch
        {
            ("powershell", true)  => new ProcessStartInfo("powershell.exe", $"-NoProfile -ExecutionPolicy Bypass -File \"{entry.ScriptPath}\""),
            ("powershell", false) => new ProcessStartInfo("powershell.exe", $"-NoProfile -ExecutionPolicy Bypass -Command \"{entry.Command}\""),
            ("wsl",        true)  => new ProcessStartInfo("wsl.exe", $"-e bash \"{entry.ScriptPath}\""),
            ("wsl",        false) => new ProcessStartInfo("wsl.exe", $"-e bash -c \"{entry.Command}\""),
            (_,            true)  => new ProcessStartInfo("cmd.exe", $"/c \"{entry.ScriptPath}\""),
            _                     => new ProcessStartInfo("cmd.exe", $"/c {entry.Command}"),
        };

        psi.UseShellExecute = true;

        if (entry.RequiresAdmin && !isElevated)
            psi.Verb = "runas";

        Process.Start(psi);
    }
}
