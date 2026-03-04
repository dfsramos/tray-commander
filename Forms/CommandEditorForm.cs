using System.Reflection;
using TrayCommander.Models;

namespace TrayCommander.Forms;

public class CommandEditorForm : Form
{
    private readonly DataGridView _grid;

    public List<CommandEntry> Commands { get; private set; }

    public CommandEditorForm(List<CommandEntry> commands)
    {
        DoubleBuffered = true;

        Commands = commands
            .Select(c => new CommandEntry
            {
                Name = c.Name,
                Command = c.Command,
                ScriptPath = c.ScriptPath,
                Runner = c.Runner,
                RequiresAdmin = c.RequiresAdmin,
            })
            .ToList();

        Text = "Edit Commands";
        Size = new Size(800, 450);
        MinimumSize = new Size(700, 350);
        StartPosition = FormStartPosition.CenterScreen;

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            EditMode = DataGridViewEditMode.EditOnEnter,
        };

        typeof(DataGridView).InvokeMember(
            "DoubleBuffered",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
            null, _grid, [true]);

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Name",
            HeaderText = "Name",
            Width = 140,
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Command",
            HeaderText = "Command",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            FillWeight = 50,
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "ScriptPath",
            HeaderText = "Script Path",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            FillWeight = 50,
        });

        var runnerColumn = new DataGridViewComboBoxColumn
        {
            Name = "Runner",
            HeaderText = "Runner",
            Width = 105,
        };
        runnerColumn.Items.AddRange(["cmd", "powershell", "wsl"]);
        _grid.Columns.Add(runnerColumn);

        _grid.Columns.Add(new DataGridViewCheckBoxColumn
        {
            Name = "RequiresAdmin",
            HeaderText = "Requires Admin",
            Width = 95,
        });

        PopulateGrid();

        var addButton = new Button { Text = "Add", Width = 80 };
        addButton.Click += OnAdd;

        var deleteButton = new Button { Text = "Delete", Width = 80 };
        deleteButton.Click += OnDelete;

        var browseButton = new Button { Text = "Browse Script...", Width = 110 };
        browseButton.Click += OnBrowseScript;

        var okButton = new Button { Text = "OK", Width = 80, DialogResult = DialogResult.OK };
        okButton.Click += OnOk;

        var cancelButton = new Button { Text = "Cancel", Width = 80, DialogResult = DialogResult.Cancel };

        AcceptButton = okButton;
        CancelButton = cancelButton;

        var leftButtons = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            Padding = new Padding(0),
        };
        leftButtons.Controls.AddRange(new Control[] { addButton, deleteButton, browseButton });

        var rightButtons = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            Padding = new Padding(0),
        };
        rightButtons.Controls.AddRange(new Control[] { okButton, cancelButton });

        var bottomPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Bottom,
            ColumnCount = 2,
            RowCount = 1,
            AutoSize = true,
            Padding = new Padding(6),
        };
        bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        bottomPanel.Controls.Add(leftButtons, 0, 0);
        bottomPanel.Controls.Add(rightButtons, 1, 0);

        Controls.Add(_grid);
        Controls.Add(bottomPanel);
    }

    private void PopulateGrid()
    {
        _grid.Rows.Clear();
        foreach (var cmd in Commands)
            _grid.Rows.Add(cmd.Name, cmd.Command, cmd.ScriptPath, cmd.Runner, cmd.RequiresAdmin);
    }

    private void OnAdd(object? sender, EventArgs e)
    {
        _grid.Rows.Add("New Command", "", "", "cmd", false);
    }

    private void OnDelete(object? sender, EventArgs e)
    {
        if (_grid.SelectedRows.Count > 0)
            _grid.Rows.Remove(_grid.SelectedRows[0]);
    }

    private void OnBrowseScript(object? sender, EventArgs e)
    {
        if (_grid.SelectedRows.Count == 0) return;

        var row = _grid.SelectedRows[0];
        var runner = row.Cells["Runner"].Value?.ToString() ?? "cmd";

        var filter = runner.ToLowerInvariant() switch
        {
            "powershell" => "PowerShell Scripts (*.ps1)|*.ps1|All Files (*.*)|*.*",
            "wsl"        => "Shell Scripts (*.sh)|*.sh|All Files (*.*)|*.*",
            _            => "Batch Files (*.bat;*.cmd)|*.bat;*.cmd|All Files (*.*)|*.*",
        };

        using var dialog = new OpenFileDialog { Filter = filter };
        if (dialog.ShowDialog() == DialogResult.OK)
            row.Cells["ScriptPath"].Value = dialog.FileName;
    }

    private void OnOk(object? sender, EventArgs e)
    {
        _grid.EndEdit();

        Commands = [];

        foreach (DataGridViewRow row in _grid.Rows)
        {
            var name       = row.Cells["Name"].Value?.ToString() ?? string.Empty;
            var command    = row.Cells["Command"].Value?.ToString() ?? string.Empty;
            var scriptPath = row.Cells["ScriptPath"].Value?.ToString() ?? string.Empty;
            var runner     = row.Cells["Runner"].Value?.ToString() ?? "cmd";
            var admin      = row.Cells["RequiresAdmin"].Value is true;

            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(command) && string.IsNullOrWhiteSpace(scriptPath))
                continue;

            Commands.Add(new CommandEntry { Name = name, Command = command, ScriptPath = scriptPath, Runner = runner, RequiresAdmin = admin });
        }
    }
}
