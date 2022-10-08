using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ControlLib;

public static class GraphicsEx
{
	public static void InitGraphics(Graphics g)
	{
		g.InterpolationMode = InterpolationMode.NearestNeighbor;
		g.SmoothingMode = SmoothingMode.None;
		g.PixelOffsetMode = PixelOffsetMode.None;
	}

	public static void DrawImage(Graphics g, Image image, Rectangle destRect, Rectangle srcRect)
	{
		g.DrawImage(srcRect: new RectangleF((float)srcRect.Left - 0.5f, (float)srcRect.Top - 0.5f, srcRect.Width, srcRect.Height), image: image, destRect: destRect, srcUnit: GraphicsUnit.Pixel);
	}

	public static void DrawImage(Graphics g, Image image, Rectangle destRect, Rectangle srcRect, ImageAttributes imageAttributes)
	{
		RectangleF rectangleF = new RectangleF((float)srcRect.Left - 0.5f, (float)srcRect.Top - 0.5f, srcRect.Width, srcRect.Height);
		g.DrawImage(image, destRect, rectangleF.Left, rectangleF.Top, rectangleF.Width, rectangleF.Height, GraphicsUnit.Pixel, imageAttributes);
	}

	public static void DrawString4(Graphics g, string text, Font font, Brush brushOuter, Brush brushInner, Rectangle rect, StringFormat sf)
	{
		Rectangle rectangle = new Rectangle(rect.X - 1, rect.Y, rect.Width, rect.Height);
		Rectangle rectangle2 = new Rectangle(rect.X + 1, rect.Y, rect.Width, rect.Height);
		Rectangle rectangle3 = new Rectangle(rect.X, rect.Y - 1, rect.Width, rect.Height);
		Rectangle rectangle4 = new Rectangle(rect.X, rect.Y + 1, rect.Width, rect.Height);
		g.DrawString(text, font, brushOuter, rectangle, sf);
		g.DrawString(text, font, brushOuter, rectangle2, sf);
		g.DrawString(text, font, brushOuter, rectangle3, sf);
		g.DrawString(text, font, brushOuter, rectangle4, sf);
		g.DrawString(text, font, brushInner, rect, sf);
	}

	public static void DrawString8(Graphics g, string text, Font font, Brush brushOuter, Brush brushInner, Rectangle rect, StringFormat sf)
	{
		Rectangle rectangle = new Rectangle(rect.Left, rect.Top + 1, rect.Width, rect.Height);
		Rectangle rectangle2 = new Rectangle(rect.Left, rect.Top - 1, rect.Width, rect.Height);
		Rectangle rectangle3 = new Rectangle(rect.Left - 1, rect.Top, rect.Width, rect.Height);
		Rectangle rectangle4 = new Rectangle(rect.Left + 1, rect.Top, rect.Width, rect.Height);
		Rectangle rectangle5 = new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width, rect.Height);
		Rectangle rectangle6 = new Rectangle(rect.Left + 1, rect.Top - 1, rect.Width, rect.Height);
		Rectangle rectangle7 = new Rectangle(rect.Left - 1, rect.Top + 1, rect.Width, rect.Height);
		Rectangle rectangle8 = new Rectangle(rect.Left - 1, rect.Top - 1, rect.Width, rect.Height);
		g.DrawString(text, font, brushOuter, rectangle, sf);
		g.DrawString(text, font, brushOuter, rectangle2, sf);
		g.DrawString(text, font, brushOuter, rectangle3, sf);
		g.DrawString(text, font, brushOuter, rectangle4, sf);
		g.DrawString(text, font, brushOuter, rectangle5, sf);
		g.DrawString(text, font, brushOuter, rectangle6, sf);
		g.DrawString(text, font, brushOuter, rectangle7, sf);
		g.DrawString(text, font, brushOuter, rectangle8, sf);
		g.DrawString(text, font, brushInner, rect, sf);
	}

	public static Color[] GetTextColorFromBackColor(Color backColor)
	{
		Color color = Color.FromArgb(160, 0, 0, 0);
		Color white = Color.White;
		if ((backColor.R + backColor.G + backColor.B) / 3 >= 128)
		{
			color = Color.FromArgb(160, 255, 255, 255);
			white = Color.FromArgb(255, 0, 0, 0);
		}
		else
		{
			color = Color.FromArgb(160, 0, 0, 0);
			white = Color.FromArgb(255, 255, 255, 255);
		}
		return new Color[2] { color, white };
	}
}
