using System.Reflection;

namespace TrayCommander.Forms;

public class AboutForm : Form
{
    public AboutForm()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0";

        Text = "About TrayCommander";
        Size = new Size(340, 210);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        var titleLabel = new Label
        {
            Text = "TrayCommander",
            Font = new Font(Font.FontFamily, 14, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(16, 20),
        };

        var descLabel = new Label
        {
            Text = "A simple system tray command launcher",
            AutoSize = true,
            Location = new Point(16, 58),
        };

        var versionLabel = new Label
        {
            Text = $"Version {version}",
            AutoSize = true,
            ForeColor = Color.Gray,
            Location = new Point(16, 84),
        };

        var closeButton = new Button
        {
            Text = "Close",
            DialogResult = DialogResult.Cancel,
            Width = 80,
            Location = new Point(238, 140),
        };

        AcceptButton = closeButton;
        CancelButton = closeButton;

        Controls.AddRange(new Control[] { titleLabel, descLabel, versionLabel, closeButton });
    }
}
