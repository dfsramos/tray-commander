using System.Text.Json;
using TrayCommander.Models;

namespace TrayCommander.Services;

public static class PersistenceService
{
    private static readonly string BaseDir = AppContext.BaseDirectory;
    private static readonly string CommandsFile = Path.Combine(BaseDir, "commands.json");
    private static readonly string OptionsFile = Path.Combine(BaseDir, "options.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static List<CommandEntry> LoadCommands()
    {
        if (!File.Exists(CommandsFile))
        {
            var defaults = DefaultCommands();
            SaveCommands(defaults);
            return defaults;
        }

        var json = File.ReadAllText(CommandsFile);
        return JsonSerializer.Deserialize<List<CommandEntry>>(json, JsonOptions) ?? DefaultCommands();
    }

    public static void SaveCommands(List<CommandEntry> commands)
    {
        File.WriteAllText(CommandsFile, JsonSerializer.Serialize(commands, JsonOptions));
    }

    public static AppOptions LoadOptions()
    {
        if (!File.Exists(OptionsFile))
        {
            var defaults = new AppOptions();
            SaveOptions(defaults);
            return defaults;
        }

        var json = File.ReadAllText(OptionsFile);
        return JsonSerializer.Deserialize<AppOptions>(json, JsonOptions) ?? new AppOptions();
    }

    public static void SaveOptions(AppOptions options)
    {
        File.WriteAllText(OptionsFile, JsonSerializer.Serialize(options, JsonOptions));
    }

    private static List<CommandEntry> DefaultCommands() =>
    [
        new CommandEntry { Name = "Reset IIS",      Command = "iisreset",                        Runner = "cmd", RequiresAdmin = true  },
        new CommandEntry { Name = "Kill Notepad",   Command = "taskkill /F /IM notepad.exe",     Runner = "cmd", RequiresAdmin = false },
        new CommandEntry { Name = "Restart nginx",  Command = "sudo systemctl restart nginx",    Runner = "wsl", RequiresAdmin = false },
    ];
}
