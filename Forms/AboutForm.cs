using System.Diagnostics;
using System.Reflection;

namespace TrayCommander.Forms;

public class AboutForm : Form
{
    public AboutForm()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "0.3.0";

        Text = "About TrayCommander";
        Size = new Size(360, 255);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        var titleLabel = new Label
        {
            Text = "TrayCommander",
            Font = new Font(Font.FontFamily, 14, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(16, 16),
        };

        var descLabel = new Label
        {
            Text = "A simple system tray command launcher",
            AutoSize = true,
            Location = new Point(16, 52),
        };

        var versionLabel = new Label
        {
            Text = $"Version {version}",
            AutoSize = true,
            ForeColor = Color.Gray,
            Location = new Point(16, 76),
        };

        var authorLabel = new Label
        {
            Text = "Author: dfsramos",
            AutoSize = true,
            Location = new Point(16, 116),
        };

        var repoLink = new LinkLabel
        {
            Text = "github.com/dfsramos/tray-commander",
            AutoSize = true,
            Location = new Point(16, 140),
        };
        repoLink.LinkClicked += (_, _) =>
            Process.Start(new ProcessStartInfo("https://github.com/dfsramos/tray-commander") { UseShellExecute = true });

        var downloadLink = new LinkLabel
        {
            Text = "dfsramos.github.io/tray-commander",
            AutoSize = true,
            Location = new Point(16, 164),
        };
        downloadLink.LinkClicked += (_, _) =>
            Process.Start(new ProcessStartInfo("https://dfsramos.github.io/tray-commander/") { UseShellExecute = true });

        var closeButton = new Button
        {
            Text = "Close",
            DialogResult = DialogResult.Cancel,
            Width = 80,
            Location = new Point(256, 196),
        };

        AcceptButton = closeButton;
        CancelButton = closeButton;

        Controls.AddRange(new Control[] { titleLabel, descLabel, versionLabel, authorLabel, repoLink, downloadLink, closeButton });
    }
}
