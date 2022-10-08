using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CharactorLib;
using CharactorLib.Common;
using ControlLib.Properties;

namespace ControlLib;

public class RGBEditor : Panel
{
	private enum RgbType
	{
		A,
		R,
		G,
		B
	}

	private static Type ResourceType = typeof(Resources);

	private const string CATEGORY_GRID = "Grid";

	private const string CATEGORY_VIEW = "View";

	private const string CATEGORY_OPTION = "Option";

	private bool mShowAlpha;

	private MouseButtons mMouseButtons;

	private Point mMouseDownPoint = new Point(0, 0);

	private Point mMouseMovePoint = new Point(0, 0);

	private Point mMouseUpPoint = new Point(0, 0);

	private int mMouseDownY_forRGB;

	[Browsable(false)]
	[Category("View")]
	public ColorBit ColorBit { get; private set; } = new ColorBit(PaletteType.R8G8B8, byte.MaxValue, 0, 0, 0);


	[Category("View")]
	[DefaultValue(false)]
	public bool ShowAlpha
	{
		get
		{
			return mShowAlpha;
		}
		set
		{
			mShowAlpha = value;
			CalcColorBarRect();
			Refresh();
		}
	}

	[Category("View")]
	[DefaultValue(true)]
	public bool ShowValueText { get; set; } = true;


	[Category("View")]
	[DefaultValue(true)]
	public bool ShowMeasure { get; set; } = true;


	[Category("View")]
	[DefaultValue(typeof(Color), "0x40, 0x80, 0x80, 0x80")]
	public Color MeasureColor { get; set; } = Color.FromArgb(64, 128, 128, 128);


	[Category("View")]
	[DefaultValue(false)]
	public bool GradeColorBar { get; set; }

	[Browsable(false)]
	private byte ColorBarGradeMin { get; set; }

	[Browsable(false)]
	private byte ColorBarGradeMax { get; set; } = 224;


	[Browsable(false)]
	private byte ColorBarGradeMinGray { get; set; } = 64;


	[Browsable(false)]
	private byte ColorBarGradeMaxGray { get; set; } = 128;


	[Category("Option")]
	[DefaultValue(true)]
	public bool MouseProcessSplitRGB { get; set; } = true;


	[Category("Option")]
	[DefaultValue(false)]
	public bool ReadOnly { get; set; }

	private Rectangle[] ColorBarRect { get; set; } = new Rectangle[4];


	public event EventHandler ColorChanged;

	private void CallColorChanged()
	{
		if (this.ColorChanged != null)
		{
			this.ColorChanged(this, EventArgs.Empty);
		}
		else
		{
			Refresh();
		}
	}

	public RGBEditor()
	{
		SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
	}

	protected override void OnClientSizeChanged(EventArgs e)
	{
		base.OnClientSizeChanged(e);
		CalcColorBarRect();
	}

	private void CalcColorBarRect()
	{
		int num = 0;
		int num2 = 0;
		int num3 = base.ClientRectangle.Width;
		int num4 = base.ClientRectangle.Height;
		int num5;
		int num6;
		int num7;
		int num8;
		if (ShowAlpha)
		{
			num5 = num2 + (int)((float)num4 * 0f / 4f);
			num6 = num2 + (int)((float)num4 * 1f / 4f);
			num7 = num2 + (int)((float)num4 * 2f / 4f);
			num8 = num2 + (int)((float)num4 * 3f / 4f);
		}
		else
		{
			num5 = num2 + (int)((float)num4 * 0f / 3f);
			num6 = num2 + (int)((float)num4 * 0f / 3f);
			num7 = num2 + (int)((float)num4 * 1f / 3f);
			num8 = num2 + (int)((float)num4 * 2f / 3f);
		}
		ColorBarRect[0] = new Rectangle(num, num5, num3, num6 - num5);
		ColorBarRect[1] = new Rectangle(num, num6, num3, num7 - num6);
		ColorBarRect[2] = new Rectangle(num, num7, num3, num8 - num7);
		ColorBarRect[3] = new Rectangle(num, num8, num3, num4 - num8);
	}

	private Rectangle GetColorBarRect(RgbType rgbType)
	{
		return ColorBarRect[(int)rgbType];
	}

	public void SetColorBitInstance(ColorBit colorBit)
	{
		if (colorBit != null && ColorBit != colorBit)
		{
			ColorBit = colorBit;
		}
		Refresh();
	}

	public void SetColorBitFrom(ColorBit colorBit)
	{
		if (colorBit != null)
		{
			ColorBit.CopyAllFrom(colorBit);
		}
		Refresh();
	}

	private int GetColorBitNum(RgbType rgbType)
	{
		return rgbType switch
		{
			RgbType.A => ColorBit.BitNumA, 
			RgbType.R => ColorBit.BitNumR, 
			RgbType.G => ColorBit.BitNumG, 
			RgbType.B => ColorBit.BitNumB, 
			_ => 0, 
		};
	}

	private void SetBitValue(RgbType rgbType, byte bitValue)
	{
		switch (rgbType)
		{
		case RgbType.A:
			ColorBit.BitA = bitValue;
			break;
		case RgbType.R:
			ColorBit.BitR = bitValue;
			break;
		case RgbType.G:
			ColorBit.BitG = bitValue;
			break;
		case RgbType.B:
			ColorBit.BitB = bitValue;
			break;
		}
		CallColorChanged();
	}

	public void SetColor(Color color)
	{
		ColorBit.Color = color;
		CallColorChanged();
	}

	protected override void OnPaintBackground(PaintEventArgs e)
	{
		base.OnPaintBackground(e);
		Graphics graphics = e.Graphics;
		graphics.Clear(BackColor);
		for (int i = 0; i < 4; i++)
		{
			int num = 8;
			if (i == 0)
			{
				num = ColorBit.BitNumA;
			}
			if (i == 1)
			{
				num = ColorBit.BitNumR;
			}
			if (i == 2)
			{
				num = ColorBit.BitNumG;
			}
			if (i == 3)
			{
				num = ColorBit.BitNumB;
			}
			if (num == 0 || num == 1 || num >= 8 || (i == 0 && !ShowAlpha))
			{
				continue;
			}
			Rectangle colorBarRect = GetColorBarRect((RgbType)i);
			int num2 = 1 << num;
			if (num2 <= 1)
			{
				num2 = 1;
			}
			if (num2 >= 256)
			{
				num2 = 256;
			}
			if (num2 > 128)
			{
				continue;
			}
			int num3 = base.ClientSize.Width;
			int top = colorBarRect.Top;
			int y = colorBarRect.Bottom - 1;
			using Pen pen = new Pen(MeasureColor);
			int num4 = 0;
			for (int j = 0; j < num3; j++)
			{
				int num5 = num3 * num4 / num2;
				if (j >= num5)
				{
					graphics.DrawLine(pen, j, top, j, y);
					num4++;
				}
			}
		}
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		Graphics graphics = e.Graphics;
		ColorBit colorBit = ColorBit;
		byte b;
		byte b2;
		if (!base.Enabled || ReadOnly)
		{
			b = ColorBarGradeMaxGray;
			b2 = ColorBarGradeMinGray;
		}
		else
		{
			b = ColorBarGradeMax;
			b2 = ColorBarGradeMin;
		}
		try
		{
			Rectangle colorBarRect = GetColorBarRect(RgbType.A);
			Rectangle colorBarRect2 = GetColorBarRect(RgbType.R);
			Rectangle colorBarRect3 = GetColorBarRect(RgbType.G);
			Rectangle colorBarRect4 = GetColorBarRect(RgbType.B);
			if (GradeColorBar)
			{
				byte b3 = (byte)((int)b / 2);
				using Brush brush = new LinearGradientBrush(colorBarRect, Color.FromArgb(b3, b3, b3), Color.FromArgb(b, b, b), LinearGradientMode.Horizontal);
				using Brush brush2 = new LinearGradientBrush(colorBarRect2, Color.FromArgb(b3, b2, b2), Color.FromArgb(b, b2, b2), LinearGradientMode.Horizontal);
				using Brush brush3 = new LinearGradientBrush(colorBarRect3, Color.FromArgb(b2, b3, b2), Color.FromArgb(b2, b, b2), LinearGradientMode.Horizontal);
				using Brush brush4 = new LinearGradientBrush(colorBarRect4, Color.FromArgb(b2, b2, b3), Color.FromArgb(b2, b2, b), LinearGradientMode.Horizontal);
				if (ShowAlpha)
				{
					DrawColorBarFore(graphics, brush, colorBarRect, colorBit, RgbType.A);
				}
				DrawColorBarFore(graphics, brush2, colorBarRect2, colorBit, RgbType.R);
				DrawColorBarFore(graphics, brush3, colorBarRect3, colorBit, RgbType.G);
				DrawColorBarFore(graphics, brush4, colorBarRect4, colorBit, RgbType.B);
			}
			else
			{
				using Brush brush5 = new SolidBrush(Color.FromArgb(b, b, b));
				using Brush brush6 = new SolidBrush(Color.FromArgb(b, b2, b2));
				using Brush brush7 = new SolidBrush(Color.FromArgb(b2, b, b2));
				using Brush brush8 = new SolidBrush(Color.FromArgb(b2, b2, b));
				if (ShowAlpha)
				{
					DrawColorBarFore(graphics, brush5, colorBarRect, colorBit, RgbType.A);
				}
				DrawColorBarFore(graphics, brush6, colorBarRect2, colorBit, RgbType.R);
				DrawColorBarFore(graphics, brush7, colorBarRect3, colorBit, RgbType.G);
				DrawColorBarFore(graphics, brush8, colorBarRect4, colorBit, RgbType.B);
			}
			if (ShowValueText)
			{
				Font font = Font;
				Color[] textColorFromBackColor = GraphicsEx.GetTextColorFromBackColor(Color.Black);
				Color color = textColorFromBackColor[0];
				Color color2 = textColorFromBackColor[1];
				using Brush brushB = new SolidBrush(color);
				using Brush brushW = new SolidBrush(color2);
				using StringFormat stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Near;
				stringFormat.LineAlignment = StringAlignment.Center;
				string text;
				if (ShowAlpha)
				{
					text = ((colorBit.EnabledAlpha && colorBit.BitNumA != 0) ? colorBit.BitA.ToString("X2") : "--");
					DrawColorBarText(graphics, text, font, brushB, brushW, colorBarRect, stringFormat);
				}
				text = colorBit.BitR.ToString("X2");
				DrawColorBarText(graphics, text, font, brushB, brushW, colorBarRect2, stringFormat);
				text = colorBit.BitG.ToString("X2");
				DrawColorBarText(graphics, text, font, brushB, brushW, colorBarRect3, stringFormat);
				text = colorBit.BitB.ToString("X2");
				DrawColorBarText(graphics, text, font, brushB, brushW, colorBarRect4, stringFormat);
			}
			if (ReadOnly)
			{
				Font font2 = Font;
				string s = ResourceUtility.GetResourceString(ResourceType, "Resources.LabelReadOnly").Replace("\\n", "\n");
				using StringFormat stringFormat2 = new StringFormat();
				stringFormat2.Alignment = StringAlignment.Center;
				stringFormat2.LineAlignment = StringAlignment.Center;
				Rectangle clipRectangle = e.ClipRectangle;
				graphics.DrawString(s, font2, Brushes.White, clipRectangle, stringFormat2);
				return;
			}
		}
		catch
		{
		}
	}

	private void DrawColorBarFore(Graphics g, Brush brush, Rectangle rect, ColorBit colorBit, RgbType rgbType)
	{
		int colorBitNum = GetColorBitNum(rgbType);
		int num = 0;
		switch (rgbType)
		{
		case RgbType.A:
			num = colorBit.BitA;
			break;
		case RgbType.R:
			num = colorBit.BitR;
			break;
		case RgbType.G:
			num = colorBit.BitG;
			break;
		case RgbType.B:
			num = colorBit.BitB;
			break;
		}
		num++;
		int num3;
		if (colorBitNum < 8)
		{
			int num2 = 8 - colorBitNum;
			num3 = num << num2;
		}
		else
		{
			num3 = num;
		}
		if (ShowAlpha && rgbType == RgbType.A && (!colorBit.EnabledAlpha || colorBit.BitNumA == 0))
		{
			num3 = 256;
		}
		int num4 = num3 * rect.Width / 256;
		Rectangle rect2 = new Rectangle(rect.X, rect.Y, num4, rect.Height);
		g.FillRectangle(brush, rect2);
	}

	private void DrawColorBarText(Graphics g, string text, Font font, Brush brushB, Brush brushW, Rectangle rect, StringFormat sf)
	{
		Rectangle rect2 = new Rectangle(rect.X, rect.Y, 64, rect.Height);
		GraphicsEx.DrawString4(g, text, font, brushB, brushW, rect2, sf);
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		base.OnMouseDown(e);
		if (!base.Enabled || ReadOnly)
		{
			return;
		}
		int num = CharactorCommon.NormalizeValue(0, base.Width - 1, e.X);
		int num2 = CharactorCommon.NormalizeValue(0, base.Height - 1, e.Y);
		mMouseButtons = e.Button;
		mMouseDownPoint = new Point(num, num2);
		for (int i = 0; i < 4; i++)
		{
			if (GetColorBarRect((RgbType)i).Contains(num, num2))
			{
				mMouseDownY_forRGB = i;
			}
		}
		OnMouseMove(e);
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		base.OnMouseMove(e);
		if (!base.Enabled || ReadOnly)
		{
			return;
		}
		int mX = CharactorCommon.NormalizeValue(0, base.Width - 1, e.X);
		int num = CharactorCommon.NormalizeValue(0, base.Height - 1, e.Y);
		mMouseMovePoint = new Point(mX, num);
		if (mMouseButtons != MouseButtons.Left)
		{
			return;
		}
		if (MouseProcessSplitRGB)
		{
			RgbType rgbType = (RgbType)mMouseDownY_forRGB;
			SetRgbBitValueFromMouse(rgbType, mX);
			return;
		}
		Rectangle colorBarRect = GetColorBarRect(RgbType.A);
		Rectangle colorBarRect2 = GetColorBarRect(RgbType.R);
		Rectangle colorBarRect3 = GetColorBarRect(RgbType.G);
		GetColorBarRect(RgbType.B);
		RgbType rgbType2 = RgbType.B;
		if (num <= colorBarRect3.Bottom)
		{
			rgbType2 = RgbType.G;
		}
		if (num <= colorBarRect2.Bottom)
		{
			rgbType2 = RgbType.R;
		}
		if (ShowAlpha && num <= colorBarRect.Bottom)
		{
			rgbType2 = RgbType.A;
		}
		SetRgbBitValueFromMouse(rgbType2, mX);
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		base.OnMouseUp(e);
		if (base.Enabled && !ReadOnly)
		{
			int num = CharactorCommon.NormalizeValue(0, base.Width - 1, e.X);
			int num2 = CharactorCommon.NormalizeValue(0, base.Height - 1, e.Y);
			mMouseUpPoint = new Point(num, num2);
			if ((mMouseButtons & e.Button) > MouseButtons.None)
			{
				mMouseButtons ^= e.Button;
			}
		}
	}

	private void SetRgbBitValueFromMouse(RgbType rgbType, int mX)
	{
		int num = GetColorBarRect(rgbType).Width;
		int colorBitNum = GetColorBitNum(rgbType);
		int num2 = 8 - colorBitNum;
		int value = mX;
		if (num > 0)
		{
			value = mX * 256 / num;
		}
		value = CharactorCommon.NormalizeValue(0, 255, value);
		int num3 = value >> num2;
		SetBitValue(rgbType, (byte)num3);
	}
}
