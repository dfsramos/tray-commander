using System.Drawing;
using System.Windows.Forms;
using TrayCommander.Models;

namespace TrayCommander.UI;

internal sealed class CommandMenuItem : ToolStripMenuItem
{
    private const int ItemHeight = 52;

    internal Color AccentColor { get; }
    private readonly CommandEntry _command;

    internal CommandMenuItem(CommandEntry command) : base(string.Empty)
    {
        _command = command;
        AccentColor = RunnerColor(command.Runner);
        AutoSize = false;
        Height = ItemHeight;
        Width = 300;
    }

    internal static Color RunnerColor(string runner) => runner.ToLowerInvariant() switch
    {
        "powershell" => Color.FromArgb(132, 204, 18),  // lime
        "wsl"        => Color.FromArgb(56, 189, 248),  // sky
        _            => Color.FromArgb(249, 115, 22),  // orange (cmd + default)
    };

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;

        // Dot
        const int dotX = 18;
        int dotY = ItemHeight / 2 - 4;
        using (var dot = new SolidBrush(AccentColor))
            g.FillEllipse(dot, dotX - 4, dotY, 8, 8);

        // Command name
        using var nameFont = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        TextRenderer.DrawText(g, _command.Name, nameFont, new Point(32, 9), AccentColor);

        // Description
        using var descFont = new Font("Consolas", 7.5f, FontStyle.Regular);
        TextRenderer.DrawText(g, BuildDescription(), descFont, new Point(32, 30), DarkMenuRenderer.TextFaint);

        // UAC badge
        if (_command.RequiresAdmin)
        {
            const string badge = "UAC";
            using var badgeFont = new Font("Consolas", 7f, FontStyle.Bold);
            var sz = TextRenderer.MeasureText(g, badge, badgeFont);
            int bx = Width - sz.Width - 14;
            int by = (ItemHeight - sz.Height) / 2;
            var rect = new Rectangle(bx - 4, by - 2, sz.Width + 8, sz.Height + 4);
            using var bg = new SolidBrush(Color.FromArgb(40, 245, 158, 11));
            g.FillRectangle(bg, rect);
            TextRenderer.DrawText(g, badge, badgeFont, new Point(bx, by), Color.FromArgb(245, 158, 11));
        }
    }

    private string BuildDescription()
    {
        var parts = new List<string> { _command.Runner.ToLowerInvariant() };

        if (!string.IsNullOrWhiteSpace(_command.ScriptPath))
            parts.Add(_command.ScriptPath);
        else if (!string.IsNullOrWhiteSpace(_command.Command))
            parts.Add(_command.Command);

        if (_command.RequiresAdmin)
            parts.Add("admin");

        return string.Join(" · ", parts);
    }
}
