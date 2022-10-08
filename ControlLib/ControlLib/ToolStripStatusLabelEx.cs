using System;
using System.Drawing;
using System.Windows.Forms;

namespace ControlLib;

public class ToolStripStatusLabelEx : ToolStripStatusLabel
{
	public ToolStripStatusLabelEx()
	{
	}

	public ToolStripStatusLabelEx(Image image)
		: base(image)
	{
	}

	public ToolStripStatusLabelEx(string text)
		: base(text)
	{
	}

	public ToolStripStatusLabelEx(string text, Image image)
		: base(text, image)
	{
	}

	public ToolStripStatusLabelEx(string text, Image image, EventHandler onClick)
		: base(text, image, onClick)
	{
	}

	public ToolStripStatusLabelEx(string text, Image image, EventHandler onClick, string name)
		: base(text, image, onClick, name)
	{
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		if (base.Owner == null)
		{
			return;
		}
		ToolStripRenderer renderer = base.Owner.Renderer;
		Graphics graphics = e.Graphics;
		renderer.DrawToolStripStatusLabelBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));
		int num = 0;
		if ((DisplayStyle & ToolStripItemDisplayStyle.Image) == ToolStripItemDisplayStyle.Image && base.Image != null)
		{
			int width = base.Image.Width;
			int height = base.Image.Height;
			int x = 0;
			int y = (base.Height - height) / 2;
			int num2 = width;
			int num3 = height;
			if (num2 >= base.Width)
			{
				num2 = base.Width;
			}
			if (num3 >= base.Height)
			{
				num3 = base.Height;
			}
			renderer.DrawItemImage(new ToolStripItemImageRenderEventArgs(imageRectangle: new Rectangle(x, y, num2, num3), g: e.Graphics, item: this));
			num += width;
		}
		string text = base.Text;
		if ((DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text && !string.IsNullOrWhiteSpace(text))
		{
			Font font = Font;
			Color foreColor = ForeColor;
			Size size = TextRenderer.MeasureText(text, font);
			int num4 = 3;
			int width2 = size.Width;
			int height2 = size.Height;
			int num5 = num;
			int y2 = (base.Height - height2) / 2;
			int num6 = width2;
			int num7 = height2;
			if (num6 >= base.Width - num4 - num5)
			{
				num6 = base.Width - num4 - num5;
			}
			if (num7 >= base.Height)
			{
				num7 = base.Height;
			}
			Rectangle textRectangle = new Rectangle(num5, y2, num6, num7);
			renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(graphics, this, text, textRectangle, foreColor, font, base.TextAlign));
		}
	}
}
