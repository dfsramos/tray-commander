using System.Diagnostics;
using TrayCommander.Forms;
using TrayCommander.Models;
using TrayCommander.Services;
using TrayCommander.UI;

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
            Icon = IconBuilder.Build(),
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
        var menu = new ContextMenuStrip
        {
            Renderer = new DarkMenuRenderer(),
            ShowImageMargin = false,
            ShowCheckMargin = false,
        };

        foreach (var cmd in _commands)
        {
            var item = new CommandMenuItem(cmd);
            var captured = cmd;
            item.Click += (_, _) => CommandRunner.Run(captured, _isElevated);
            menu.Items.Add(item);
        }

        menu.Items.Add(new ToolStripSeparator());

        var editItem = new ToolStripMenuItem("Edit Commands") { Padding = new Padding(12, 0, 0, 0) };
        editItem.Click += OnEditCommands;
        menu.Items.Add(editItem);

        var optionsItem = new ToolStripMenuItem("Options") { Padding = new Padding(12, 0, 0, 0) };
        optionsItem.Click += OnOptions;
        menu.Items.Add(optionsItem);

        var aboutItem = new ToolStripMenuItem("About") { Padding = new Padding(12, 0, 0, 0) };
        aboutItem.Click += OnAbout;
        menu.Items.Add(aboutItem);

        menu.Items.Add(new ToolStripSeparator());

        var quitItem = new ToolStripMenuItem("Quit") { Padding = new Padding(12, 0, 0, 0) };
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
