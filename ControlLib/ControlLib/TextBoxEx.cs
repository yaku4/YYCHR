using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ControlLib;

public class TextBoxEx : TextBox
{
	private const int WM_NCPAINT = 133;

	private const int WM_PAINT = 15;

	private Color _borderColor = Color.LightGray;

	private Color _borderFocusedColor = Color.DarkGray;

	[DefaultValue(typeof(Color), "LightGray")]
	public Color BorderColor
	{
		get
		{
			return _borderColor;
		}
		set
		{
			_borderColor = value;
		}
	}

	[DefaultValue(typeof(Color), "DarkGray")]
	public Color BorderFocusedColor
	{
		get
		{
			return _borderFocusedColor;
		}
		set
		{
			_borderFocusedColor = value;
		}
	}

	protected override void WndProc(ref Message m)
	{
		if (m.Msg == 15)
		{
			Rectangle bounds = new Rectangle(base.Left - 1, base.Top - 1, base.Width + 2, base.Height + 2);
			using Graphics graphics = base.Parent.CreateGraphics();
			Color color = (Focused ? _borderFocusedColor : _borderColor);
			ControlPaint.DrawBorder(graphics, bounds, color, ButtonBorderStyle.Solid);
		}
		base.WndProc(ref m);
	}
}
