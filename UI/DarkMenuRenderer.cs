using System.Drawing;
using System.Windows.Forms;

namespace TrayCommander.UI;

internal sealed class DarkMenuRenderer : ToolStripProfessionalRenderer
{
    internal static readonly Color BgColor     = Color.FromArgb(24, 24, 27);    // #18181b
    internal static readonly Color HoverBg     = Color.FromArgb(35, 32, 26);    // warm dark
    internal static readonly Color BorderColor = Color.FromArgb(39, 39, 42);    // #27272a
    internal static readonly Color TextMuted   = Color.FromArgb(113, 113, 122); // #71717a
    internal static readonly Color TextFaint   = Color.FromArgb(82, 82, 91);    // #52525b

    public DarkMenuRenderer() : base(new DarkColorTable()) { }

    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
        e.Graphics.Clear(BgColor);
    }

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        var g = e.Graphics;
        var bounds = new Rectangle(0, 0, e.Item.Width, e.Item.Height);

        var bg = e.Item.Selected && e.Item.Enabled ? HoverBg : BgColor;
        using var brush = new SolidBrush(bg);
        g.FillRectangle(brush, bounds);

        if (e.Item is CommandMenuItem cmd)
        {
            using var strip = new SolidBrush(cmd.AccentColor);
            g.FillRectangle(strip, 0, 0, 3, e.Item.Height);
        }
    }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
        if (e.Item is CommandMenuItem) return; // CommandMenuItem paints its own text in OnPaint

        e.TextColor = e.Item.Enabled ? TextMuted : TextFaint;
        base.OnRenderItemText(e);
    }

    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
        int y = e.Item.Height / 2;
        using var pen = new Pen(BorderColor);
        e.Graphics.DrawLine(pen, 10, y, e.Item.Width - 10, y);
    }

    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    {
        var b = e.AffectedBounds;
        using var pen = new Pen(BorderColor);
        e.Graphics.DrawRectangle(pen, b.X, b.Y, b.Width - 1, b.Height - 1);
    }

    protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
    {
        // Suppress default icon rendering — command items draw their own dot in OnPaint.
    }
}

internal sealed class DarkColorTable : ProfessionalColorTable
{
    private static readonly Color Bg     = Color.FromArgb(24, 24, 27);
    private static readonly Color Border = Color.FromArgb(39, 39, 42);
    private static readonly Color Hover  = Color.FromArgb(35, 32, 26);

    public override Color MenuItemSelected                  => Hover;
    public override Color MenuItemBorder                    => Border;
    public override Color MenuBorder                        => Border;
    public override Color ToolStripDropDownBackground       => Bg;
    public override Color ImageMarginGradientBegin          => Bg;
    public override Color ImageMarginGradientMiddle         => Bg;
    public override Color ImageMarginGradientEnd            => Bg;
    public override Color MenuItemSelectedGradientBegin     => Hover;
    public override Color MenuItemSelectedGradientEnd       => Hover;
    public override Color MenuItemPressedGradientBegin      => Hover;
    public override Color MenuItemPressedGradientEnd        => Hover;
    public override Color SeparatorDark                     => Border;
    public override Color SeparatorLight                    => Border;
    public override Color MenuStripGradientBegin            => Bg;
    public override Color MenuStripGradientEnd              => Bg;
}
