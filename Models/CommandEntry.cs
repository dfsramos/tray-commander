namespace TrayCommander.Models;

public class CommandEntry
{
    public string Name { get; set; } = string.Empty;
    public string Command { get; set; } = string.Empty;
    public string Runner { get; set; } = "cmd";
    public bool RequiresAdmin { get; set; }
}
