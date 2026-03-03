using System.Diagnostics;
using TrayCommander.Forms;
using TrayCommander.Models;
using TrayCommander.Services;

namespace TrayCommander;

public class TrayApp : ApplicationContext
{
    private readonly NotifyIcon _trayIcon;
    private readonly bool _isElevated;
    private List<CommandEntry> _commands;
    private AppOptions _options;

    public TrayApp(bool isElevated)
    {
        _isElevated = isElevated;
        _commands = PersistenceService.LoadCommands();
        _options = PersistenceService.LoadOptions();

        _trayIcon = new NotifyIcon
        {
            Icon = SystemIcons.Application,
            Text = "TrayCommander",
            Visible = true,
        };

        _trayIcon.ContextMenuStrip = BuildContextMenu();

        // Left-click shows the context menu at cursor position.
        _trayIcon.MouseClick += (_, e) =>
        {
            if (e.Button == MouseButtons.Left)
                _trayIcon.ContextMenuStrip?.Show(Cursor.Position);
        };
    }

    private ContextMenuStrip BuildContextMenu()
    {
        var menu = new ContextMenuStrip();

        var cmdCommands = _commands.Where(c => c.Runner.Equals("cmd", StringComparison.OrdinalIgnoreCase)).ToList();
        var wslCommands = _commands.Where(c => c.Runner.Equals("wsl", StringComparison.OrdinalIgnoreCase)).ToList();

        if (cmdCommands.Count > 0)
        {
            menu.Items.Add(new ToolStripMenuItem("\u2500\u2500 CMD \u2500\u2500") { Enabled = false });

            foreach (var cmd in cmdCommands)
            {
                var label = cmd.RequiresAdmin ? $"[CMD] \U0001F6E1 {cmd.Name}" : $"[CMD] {cmd.Name}";
                var item = new ToolStripMenuItem(label);
                var captured = cmd;
                item.Click += (_, _) => CommandRunner.Run(captured, _isElevated);
                menu.Items.Add(item);
            }
        }

        if (wslCommands.Count > 0)
        {
            menu.Items.Add(new ToolStripMenuItem("\u2500\u2500 WSL \u2500\u2500") { Enabled = false });

            foreach (var cmd in wslCommands)
            {
                var label = cmd.RequiresAdmin ? $"[WSL] \U0001F6E1 {cmd.Name}" : $"[WSL] {cmd.Name}";
                var item = new ToolStripMenuItem(label);
                var captured = cmd;
                item.Click += (_, _) => CommandRunner.Run(captured, _isElevated);
                menu.Items.Add(item);
            }
        }

        menu.Items.Add(new ToolStripSeparator());

        var editItem = new ToolStripMenuItem("Edit Commands");
        editItem.Click += OnEditCommands;
        menu.Items.Add(editItem);

        var optionsItem = new ToolStripMenuItem("Options");
        optionsItem.Click += OnOptions;
        menu.Items.Add(optionsItem);

        var aboutItem = new ToolStripMenuItem("About");
        aboutItem.Click += OnAbout;
        menu.Items.Add(aboutItem);

        menu.Items.Add(new ToolStripSeparator());

        var quitItem = new ToolStripMenuItem("Quit");
        quitItem.Click += OnQuit;
        menu.Items.Add(quitItem);

        return menu;
    }

    private void OnEditCommands(object? sender, EventArgs e)
    {
        using var form = new CommandEditorForm(_commands);
        if (form.ShowDialog() == DialogResult.OK)
        {
            _commands = form.Commands;
            PersistenceService.SaveCommands(_commands);
            _trayIcon.ContextMenuStrip?.Dispose();
            _trayIcon.ContextMenuStrip = BuildContextMenu();
        }
    }

    private void OnOptions(object? sender, EventArgs e)
    {
        using var form = new OptionsForm(_options);
        if (form.ShowDialog() == DialogResult.OK)
        {
            _options = form.Options;
            PersistenceService.SaveOptions(_options);

            // If alwaysRunAsAdmin was just enabled and we're not already elevated, relaunch elevated.
            if (_options.AlwaysRunAsAdmin && !_isElevated)
            {
                var exe = Process.GetCurrentProcess().MainModule?.FileName;
                if (exe != null)
                {
                    Process.Start(new ProcessStartInfo(exe) { Verb = "runas", UseShellExecute = true });
                    ExitThread();
                }
            }
        }
    }

    private void OnAbout(object? sender, EventArgs e)
    {
        using var form = new AboutForm();
        form.ShowDialog();
    }

    private void OnQuit(object? sender, EventArgs e)
    {
        _trayIcon.Visible = false;
        ExitThread();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _trayIcon.ContextMenuStrip?.Dispose();
            _trayIcon.Dispose();
        }
        base.Dispose(disposing);
    }
}
