using TrayCommander.Models;

namespace TrayCommander.Forms;

public class OptionsForm : Form
{
    private readonly CheckBox _alwaysAdminCheckBox;

    public AppOptions Options { get; private set; }

    public OptionsForm(AppOptions options)
    {
        Options = new AppOptions { AlwaysRunAsAdmin = options.AlwaysRunAsAdmin };

        Text = "Options";
        Size = new Size(340, 170);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        _alwaysAdminCheckBox = new CheckBox
        {
            Text = "Always run as administrator",
            Checked = options.AlwaysRunAsAdmin,
            AutoSize = true,
            Location = new Point(16, 24),
        };

        var okButton = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Width = 80,
            Location = new Point(148, 100),
        };
        okButton.Click += (_, _) =>
            Options = new AppOptions { AlwaysRunAsAdmin = _alwaysAdminCheckBox.Checked };

        var cancelButton = new Button
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Width = 80,
            Location = new Point(238, 100),
        };

        AcceptButton = okButton;
        CancelButton = cancelButton;

        Controls.AddRange(new Control[] { _alwaysAdminCheckBox, okButton, cancelButton });
    }
}
