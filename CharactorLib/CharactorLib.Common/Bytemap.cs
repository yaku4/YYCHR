using System;
using System.Collections.Generic;
using System.Drawing;

namespace CharactorLib.Common;

public class Bytemap
{
	private Size mSize;

	private byte[] mData;

	private Color[] mPalette = new Color[256];

	public Size Size
	{
		get
		{
			return mSize;
		}
		set
		{
			mSize = value;
		}
	}

	public int Width
	{
		get
		{
			return mSize.Width;
		}
		set
		{
			Size = new Size(value, mSize.Height);
		}
	}

	public int Height
	{
		get
		{
			return mSize.Height;
		}
		set
		{
			Size = new Size(mSize.Width, value);
		}
	}

	public int DataSize => Width * Height;

	public byte[] Data => mData;

	public Color[] Palette => mPalette;

	public bool CanUpdatePixel { get; set; }

	public bool CanUpdatePalette { get; set; }

	public Bytemap(int width, int height)
	{
		Size = new Size(width, height);
		mData = new byte[DataSize];
		CanUpdatePixel = true;
		CanUpdatePalette = true;
	}

	public Bytemap(Size size)
	{
		Size = size;
		mData = new byte[DataSize];
		CanUpdatePixel = true;
		CanUpdatePalette = true;
	}

	public Bytemap Clone()
	{
		Bytemap bytemap = new Bytemap(Size);
		for (int i = 0; i < bytemap.Data.Length; i++)
		{
			bytemap.Data[i] = mData[i];
		}
		bytemap.SetPalette(Palette);
		return bytemap;
	}

	public void SetPalette(Color[] color)
	{
		if (color != null)
		{
			int num = Math.Min(Palette.Length, color.Length);
			for (int i = 0; i < num; i++)
			{
				Palette[i] = color[i];
			}
		}
	}

	public void SetPalette(Color[] color, bool halfBrightness)
	{
		if (color == null)
		{
			return;
		}
		int num = Math.Min(Palette.Length, color.Length);
		for (int i = 0; i < num; i++)
		{
			if (halfBrightness)
			{
				Color color2 = color[i];
				Palette[i] = Color.FromArgb(color2.A, (int)color2.R / 2, (int)color2.G / 2, (int)color2.B / 2);
			}
			else
			{
				Palette[i] = color[i];
			}
		}
	}

	public void Resize(Size size)
	{
		int width = Width;
		int height = Height;
		byte[] array = (byte[])Data.Clone();
		Width = size.Width;
		Height = size.Height;
		mData = new byte[DataSize];
		int num = Math.Min(height, Height);
		int num2 = Math.Min(width, Width);
		for (int i = 0; i < num; i++)
		{
			int num3 = i * width;
			int num4 = i * Width;
			for (int j = 0; j < num2; j++)
			{
				Data[num4++] = array[num3++];
			}
		}
	}

	public void CopyRect(Rectangle dstRect, Bytemap srcBytemap, Rectangle srcRect)
	{
		if (mData == null)
		{
			return;
		}
		byte[] data = srcBytemap.Data;
		byte[] array = mData;
		for (int i = 0; i < srcRect.Height; i++)
		{
			int num = dstRect.Top + i;
			if (num < 0 || num >= Height)
			{
				break;
			}
			int num2 = srcRect.Top + i;
			if (num2 < 0 || num2 >= srcBytemap.Height)
			{
				break;
			}
			for (int j = 0; j < srcRect.Width; j++)
			{
				int num3 = dstRect.Left + j;
				if (num3 < 0 || num3 >= Width)
				{
					break;
				}
				int num4 = srcRect.Left + j;
				if (num4 < 0 || num4 >= srcBytemap.Width)
				{
					break;
				}
				int pointAddress = GetPointAddress(srcBytemap, num4, num2, clip: true);
				int pointAddress2 = GetPointAddress(this, num3, num, clip: true);
				array[pointAddress2] = data[pointAddress];
			}
		}
		CanUpdatePixel = true;
	}

	public void CopyRect(Point destPoint, Bytemap srcBytemap, Rectangle srcRect)
	{
		int width = Math.Min(srcRect.Width, Width - destPoint.X);
		int height = Math.Min(srcRect.Height, Height - destPoint.Y);
		Rectangle dstRect = new Rectangle(destPoint.X, destPoint.Y, width, height);
		CopyRect(dstRect, srcBytemap, srcRect);
	}

	public void CopyRect(Rectangle dstRect, Rectangle srcRect)
	{
		if (mData == null)
		{
			return;
		}
		byte[] array = (byte[])mData.Clone();
		byte[] array2 = mData;
		for (int i = 0; i < srcRect.Height; i++)
		{
			int num = dstRect.Top + i;
			if (num < 0 || num >= Height)
			{
				break;
			}
			int num2 = srcRect.Top + i;
			if (num2 < 0 || num2 >= Height)
			{
				break;
			}
			for (int j = 0; j < srcRect.Width; j++)
			{
				int num3 = dstRect.Left + j;
				if (num3 < 0 || num3 >= Width)
				{
					break;
				}
				int num4 = srcRect.Left + j;
				if (num4 < 0 || num4 >= Width)
				{
					break;
				}
				int pointAddress = GetPointAddress(this, num4, num2, clip: true);
				int pointAddress2 = GetPointAddress(this, num3, num, clip: true);
				array2[pointAddress2] = array[pointAddress];
			}
		}
		CanUpdatePixel = true;
	}

	public void CopyRect(Point destPoint, Rectangle srcRect)
	{
		int width = Math.Min(srcRect.Width, Width - destPoint.X);
		int height = Math.Min(srcRect.Height, Height - destPoint.Y);
		Rectangle dstRect = new Rectangle(destPoint.X, destPoint.Y, width, height);
		CopyRect(dstRect, srcRect);
	}

	public void Clear(byte color = 0)
	{
		Rectangle rect = new Rectangle(0, 0, Size.Width, Size.Height);
		FillRect(rect, color);
	}

	internal void AddIndexRect(Rectangle rect, int addPal)
	{
		int num = rect.Left;
		int num2 = rect.Top;
		int num3 = rect.Width;
		int num4 = rect.Height;
		if (num < 0)
		{
			num = 0;
		}
		if (num2 < 0)
		{
			num2 = 0;
		}
		if (num + num3 >= Width)
		{
			num3 = Width - num;
		}
		if (num2 + num4 >= Height)
		{
			num4 = Height - num2;
		}
		for (int i = num2; i < num2 + num4; i++)
		{
			for (int j = num; j < num + num3; j++)
			{
				byte pixel = GetPixel(j, i);
				pixel = (byte)(pixel + addPal);
				SetPixel(j, i, pixel);
			}
		}
		CanUpdatePixel = true;
	}

	public void SetPaletteSet(byte paletteSetSize, byte paletteSetNo)
	{
		SetPaletteSet(new Rectangle(new Point(0, 0), Size), paletteSetSize, paletteSetNo);
	}

	public void SetPaletteSet(Rectangle rect, byte paletteSetSize, byte paletteSetNo)
	{
		if (paletteSetSize == 0)
		{
			return;
		}
		byte b = (byte)(paletteSetNo * paletteSetSize);
		for (int i = rect.Top; i < rect.Bottom; i++)
		{
			for (int j = rect.Left; j < rect.Right; j++)
			{
				int pointAddress = GetPointAddress(j, i);
				byte b2 = mData[pointAddress];
				mData[pointAddress] = (byte)((int)b2 % (int)paletteSetSize + b);
			}
		}
		CanUpdatePixel = true;
	}

	public void Rotate(Rectangle srcRect, RotateType rotateType)
	{
		if (mData == null || srcRect.Width != srcRect.Height)
		{
			return;
		}
		byte[] array = (byte[])mData.Clone();
		byte[] array2 = mData;
		Rectangle rectangle = srcRect;
		int height = srcRect.Height;
		for (int i = 0; i < height; i++)
		{
			int num = rectangle.Top + i;
			if (num < 0 || num >= Width)
			{
				break;
			}
			for (int j = 0; j < height; j++)
			{
				int num2 = rectangle.Left + j;
				if (num2 < 0 || num2 >= Width)
				{
					break;
				}
				int num4;
				int num3;
				if (rotateType == RotateType.Left)
				{
					num3 = srcRect.Top + j;
					num4 = srcRect.Right - 1 - i;
				}
				else
				{
					if (rotateType != RotateType.Right)
					{
						num4 = num2;
						num3 = num;
						break;
					}
					num3 = srcRect.Bottom - 1 - j;
					num4 = srcRect.Left + i;
				}
				if (num4 < 0 || num4 >= Width || num3 < 0 || num3 >= Width)
				{
					break;
				}
				int pointAddress = GetPointAddress(num4, num3);
				int pointAddress2 = GetPointAddress(num2, num);
				array2[pointAddress2] = array[pointAddress];
			}
		}
		CanUpdatePixel = true;
	}

	public void Mirror(Rectangle srcRect, MirrorType mirrorType)
	{
		if (mData == null)
		{
			return;
		}
		byte[] array = (byte[])mData.Clone();
		byte[] array2 = mData;
		Rectangle rectangle = srcRect;
		for (int i = 0; i < srcRect.Height; i++)
		{
			int num = rectangle.Top + i;
			if (num < 0 || num >= Width)
			{
				break;
			}
			int num2 = ((mirrorType != MirrorType.Vertical && mirrorType != MirrorType.Both) ? (srcRect.Top + i) : (srcRect.Bottom - 1 - i));
			if (num2 < 0 || num2 >= Width)
			{
				break;
			}
			for (int j = 0; j < srcRect.Width; j++)
			{
				int num3 = rectangle.Left + j;
				if (num3 < 0 || num3 >= Width)
				{
					break;
				}
				int num4 = ((mirrorType != MirrorType.Horizontal && mirrorType != MirrorType.Both) ? (srcRect.Left + j) : (srcRect.Right - 1 - j));
				if (num4 < 0 || num4 >= Width)
				{
					break;
				}
				int pointAddress = GetPointAddress(num4, num2);
				int pointAddress2 = GetPointAddress(num3, num);
				array2[pointAddress2] = array[pointAddress];
			}
		}
		CanUpdatePixel = true;
	}

	public void Shift(Rectangle rect, ShiftType shiftType)
	{
		if (mData != null)
		{
			Bytemap srcBytemap = Clone();
			Rectangle srcRect;
			Point destPoint;
			Rectangle srcRect2;
			Point destPoint2;
			switch (shiftType)
			{
			case ShiftType.Up:
				srcRect = new Rectangle(rect.Left, rect.Top + 1, rect.Width, rect.Height - 1);
				destPoint = new Point(rect.Left, rect.Top);
				srcRect2 = new Rectangle(rect.Left, rect.Top, rect.Width, 1);
				destPoint2 = new Point(rect.Left, rect.Bottom - 1);
				break;
			case ShiftType.Down:
				srcRect = new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height - 1);
				destPoint = new Point(rect.Left, rect.Top + 1);
				srcRect2 = new Rectangle(rect.Left, rect.Bottom - 1, rect.Width, 1);
				destPoint2 = new Point(rect.Left, rect.Top);
				break;
			case ShiftType.Right:
				srcRect = new Rectangle(rect.Left, rect.Top, rect.Width - 1, rect.Height);
				destPoint = new Point(rect.Left + 1, rect.Top);
				srcRect2 = new Rectangle(rect.Right - 1, rect.Top, 1, rect.Height);
				destPoint2 = new Point(rect.Left, rect.Top);
				break;
			default:
				srcRect = new Rectangle(rect.Left + 1, rect.Top, rect.Width - 1, rect.Height);
				destPoint = new Point(rect.Left, rect.Top);
				srcRect2 = new Rectangle(rect.Left, rect.Top, 1, rect.Height);
				destPoint2 = new Point(rect.Right - 1, rect.Top);
				break;
			}
			CopyRect(destPoint, srcBytemap, srcRect);
			CopyRect(destPoint2, srcBytemap, srcRect2);
		}
	}

	public void ReplaceColor(Rectangle rect, byte[] newPalette)
	{
		for (int i = rect.Top; i < rect.Bottom; i++)
		{
			for (int j = rect.Left; j < rect.Right; j++)
			{
				int pointAddress = GetPointAddress(j, i);
				byte b = mData[pointAddress];
				byte b2 = b;
				if (b < newPalette.Length)
				{
					b2 = newPalette[b];
				}
				mData[pointAddress] = b2;
			}
		}
		CanUpdatePixel = true;
	}

	public static int GetPointAddress(Bytemap bytemap, int x, int y, bool clip)
	{
		int num = x;
		int num2 = y;
		if (!clip)
		{
			if (num < 0 || num >= bytemap.Width || num2 < 0 || num2 >= bytemap.Height)
			{
				return -1;
			}
		}
		else
		{
			if (num < 0)
			{
				num = 0;
			}
			if (num >= bytemap.Width)
			{
				num = bytemap.Width - 1;
			}
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num2 >= bytemap.Height)
			{
				num2 = bytemap.Height - 1;
			}
		}
		return num2 * bytemap.Width + num;
	}

	public int GetPointAddress(int x, int y, bool clip)
	{
		return GetPointAddress(this, x, y, clip);
	}

	public int GetPointAddress(int x, int y)
	{
		return GetPointAddress(this, x, y, clip: true);
	}

	private int GetPointAddress(Point p)
	{
		return GetPointAddress(this, p.X, p.Y, clip: true);
	}

	public static Point GetPointOnRect(Rectangle rect, int x, int y)
	{
		if (x < rect.Left)
		{
			x = rect.Left;
		}
		if (x >= rect.Right)
		{
			x = rect.Right - 1;
		}
		if (y < rect.Top)
		{
			y = rect.Top;
		}
		if (y >= rect.Bottom)
		{
			y = rect.Bottom - 1;
		}
		return new Point(x, y);
	}

	public static Point GetPointOnRect(Rectangle rect, Point p)
	{
		return GetPointOnRect(rect, p.X, p.Y);
	}

	public static Point GetIfPointOnRect(Rectangle rect, int x, int y)
	{
		if (x < rect.Left)
		{
			x = -1;
		}
		if (x >= rect.Right)
		{
			x = -1;
		}
		if (y < rect.Top)
		{
			y = -1;
		}
		if (y >= rect.Bottom)
		{
			y = -1;
		}
		return new Point(x, y);
	}

	public static Point GetIfPointOnRect(Rectangle rect, Point p)
	{
		return GetIfPointOnRect(rect, p.X, p.Y);
	}

	public static Rectangle ClipRectOnRect(Rectangle rect, Rectangle baseRect)
	{
		return rect;
	}

	public byte GetPixel(int x, int y)
	{
		int pointAddress = GetPointAddress(x, y);
		return mData[pointAddress];
	}

	public byte GetPixel(Point p)
	{
		return GetPixel(p.X, p.Y);
	}

	public void SetPixel(int x, int y, byte pixel)
	{
		SetPixel(x, y, pixel, clip: true);
	}

	public void SetPixel(int x, int y, byte pixel, bool clip)
	{
		int pointAddress = GetPointAddress(x, y, clip);
		if (pointAddress >= 0)
		{
			mData[pointAddress] = pixel;
			CanUpdatePixel = true;
		}
	}

	public void SetPixel(Point p, byte pixel)
	{
		SetPixel(p.X, p.Y, pixel);
	}

	public void DrawLine(Point p1, Point p2, byte color)
	{
		DrawLine(p1.X, p1.Y, p2.X, p2.Y, color);
	}

	public void DrawLine(int x1, int y1, int x2, int y2, byte color)
	{
		if (x1 == x2 && y1 == y2)
		{
			SetPixel(x1, y1, color);
		}
		else if (x1 == x2)
		{
			DrawLineV(x1, y1, y2, color);
		}
		else if (y1 == y2)
		{
			DrawLineH(y1, x1, x2, color);
		}
		else
		{
			int num = Math.Abs(x2 - x1);
			int num2 = Math.Abs(y2 - y1);
			if (num >= num2)
			{
				DrawLineXBase(x1, y1, x2, y2, color);
			}
			else
			{
				DrawLineYBase(x1, y1, x2, y2, color);
			}
		}
		CanUpdatePixel = true;
	}

	private void DrawLineXBase(int x1, int y1, int x2, int y2, byte color)
	{
		int num;
		int num2;
		int num3;
		int num4;
		if (x1 < x2)
		{
			num = x1;
			num2 = y1;
			num3 = x2;
			num4 = y2;
		}
		else
		{
			num = x2;
			num2 = y2;
			num3 = x1;
			num4 = y1;
		}
		double num5 = num3 - num;
		double num6 = num4 - num2;
		for (int i = 0; (double)i <= num5; i++)
		{
			int num7 = (int)Math.Round((double)i * num6 / num5);
			SetPixel(i + num, num7 + num2, color);
		}
	}

	private void DrawLineYBase(int x1, int y1, int x2, int y2, byte color)
	{
		int num;
		int num2;
		int num3;
		int num4;
		if (y1 < y2)
		{
			num = x1;
			num2 = y1;
			num3 = x2;
			num4 = y2;
		}
		else
		{
			num = x2;
			num2 = y2;
			num3 = x1;
			num4 = y1;
		}
		double num5 = num3 - num;
		double num6 = num4 - num2;
		for (int i = 0; (double)i <= num6; i++)
		{
			int num7 = (int)Math.Round((double)i * num5 / num6);
			SetPixel(num7 + num, i + num2, color);
		}
	}

	private void DrawLineH(int y, int x1, int x2, byte color)
	{
		DrawLineH(y, x1, x2, color, clip: true);
	}

	private void DrawLineH(int y, int x1, int x2, byte color, bool clip)
	{
		int num;
		int num2;
		if (x1 < x2)
		{
			num = x1;
			num2 = x2;
		}
		else
		{
			num = x2;
			num2 = x1;
		}
		for (int i = num; i <= num2; i++)
		{
			SetPixel(i, y, color, clip);
		}
	}

	private void DrawLineV(int x, int y1, int y2, byte color)
	{
		DrawLineV(x, y1, y2, color, clip: true);
	}

	private void DrawLineV(int x, int y1, int y2, byte color, bool clip)
	{
		int num;
		int num2;
		if (y1 < y2)
		{
			num = y1;
			num2 = y2;
		}
		else
		{
			num = y2;
			num2 = y1;
		}
		for (int i = num; i <= num2; i++)
		{
			SetPixel(x, i, color, clip);
		}
	}

	public void DrawRect(Rectangle rect, byte color)
	{
		DrawRect(rect.Left, rect.Top, rect.Width, rect.Height, color);
	}

	public void DrawRect(int l, int t, int w, int h, byte color)
	{
		DrawRect(l, t, w, h, color, clip: true);
	}

	public void DrawRect(int l, int t, int w, int h, byte color, bool clip)
	{
		if (clip)
		{
			if (l < 0)
			{
				l = 0;
			}
			if (t < 0)
			{
				t = 0;
			}
			if (l + w >= Width)
			{
				w = Width - l;
			}
			if (t + h >= Height)
			{
				h = Height - t;
			}
		}
		int num = l + w - 1;
		int num2 = t + h - 1;
		DrawLineV(l, t, num2, color, clip);
		DrawLineV(num, t, num2, color, clip);
		DrawLineH(t, l, num, color, clip);
		DrawLineH(num2, l, num, color, clip);
		CanUpdatePixel = true;
	}

	public void FillRect(Rectangle rect, byte color)
	{
		FillRect(rect.Left, rect.Top, rect.Width, rect.Height, color);
	}

	public void FillRect(int l, int t, int w, int h, byte color)
	{
		if (l < 0)
		{
			l = 0;
		}
		if (t < 0)
		{
			t = 0;
		}
		if (l + w >= Width)
		{
			w = Width - l;
		}
		if (t + h >= Height)
		{
			h = Height - t;
		}
		int x = l + w - 1;
		for (int i = t; i < t + h; i++)
		{
			DrawLineH(i, l, x, color);
		}
		CanUpdatePixel = true;
	}

	public void DrawCircle(Rectangle rect, byte color)
	{
		DrawCircle(rect.Left, rect.Top, rect.Width, rect.Height, color);
	}

	public void DrawCircle(int l, int t, int w, int h, byte color)
	{
		if (w < 2 || h < 2)
		{
			DrawRect(l, t, w + 1, h + 1, color);
			return;
		}
		List<Point> list = new List<Point>();
		int num = w + 1;
		int num2 = h + 1;
		bool flag = (num & 1) == 1;
		bool flag2 = (num2 & 1) == 1;
		int num3 = num / 2 + (flag ? 1 : 0);
		int num4 = num2 / 2 + (flag2 ? 1 : 0);
		int num5 = num3 - 1;
		int num6 = num5 + ((!flag) ? 1 : 0);
		int num7 = num4 - 1;
		int num8 = num7 + ((!flag2) ? 1 : 0);
		List<Point> list2 = new List<Point>();
		double num9 = (double)num4 - 1.0;
		double num10 = (double)num3 - 1.0;
		double num11 = num9 / num10;
		double num12 = num10 * num10;
		for (int i = 0; i < num3; i++)
		{
			double num13 = i * i;
			double num14 = num12 - num13;
			if (num14 > 0.0)
			{
				double num15 = Math.Sqrt(num14) * num11;
				list2.Add(new Point(i, (int)(num15 + 0.5)));
			}
		}
		num12 = num9 * num9;
		for (int j = 0; j < num4; j++)
		{
			double num16 = j * j;
			double num17 = num12 - num16;
			if (num17 > 0.0 && num11 != 0.0)
			{
				double num18 = Math.Sqrt(num17) / num11;
				_ = (double)j / num11;
				list2.Add(new Point((int)(num18 + 0.5), j));
			}
		}
		foreach (Point item in list2)
		{
			list.Add(new Point(l + num5 - item.X, t + num7 - item.Y));
			list.Add(new Point(l + num6 + item.X, t + num7 - item.Y));
			list.Add(new Point(l + num5 - item.X, t + num8 + item.Y));
			list.Add(new Point(l + num6 + item.X, t + num8 + item.Y));
		}
		foreach (Point item2 in list)
		{
			SetPixel(item2.X, item2.Y, color);
		}
		CanUpdatePixel = true;
	}

	public void FillCircle(Rectangle rect, byte color)
	{
		FillCircle(rect.Left, rect.Top, rect.Width, rect.Height, color);
	}

	public void FillCircle(int l, int t, int w, int h, byte color)
	{
		if (w <= 2 || h <= 2)
		{
			FillRect(l, t, w, h, color);
			return;
		}
		w--;
		h--;
		double num = (double)w / 2.0;
		double num2 = (double)h / 2.0;
		double num3 = (double)l + num;
		double num4 = (double)t + num2;
		if (num == 0.0 || num2 == 0.0)
		{
			return;
		}
		int num5 = l + w / 2;
		int num6 = l + w / 2 + w % 2;
		int num7 = t + h / 2;
		int num8 = t + h / 2 + h % 2;
		double num9 = num2 / num;
		double num10 = num * num;
		for (int i = 0; (double)i < num; i++)
		{
			double num11 = i;
			double num12 = num11 * num11;
			double num13 = num10 - num12;
			if (num13 != 0.0)
			{
				double num14 = Math.Sqrt(num13) * num9;
				int num15 = (int)(num4 + num14 + 0.5);
				int num16 = (int)(num3 + num11) - num6;
				int num17 = num15 - num8;
				SetPixel(num6 + num16, num8 + num17, color);
				SetPixel(num6 + num16, num7 - num17, color);
				SetPixel(num5 - num16, num8 + num17, color);
				SetPixel(num5 - num16, num7 - num17, color);
			}
		}
		List<Rnage> list = new List<Rnage>();
		num9 = num / num2;
		num10 = num2 * num2;
		for (int j = 0; (double)j <= num2; j++)
		{
			double num18 = j;
			double num19 = num18 * num18;
			double num20 = num10 - num19;
			if (num20 != 0.0)
			{
				double num21 = Math.Sqrt(num20) * num9;
				int num22 = (int)(num3 + num21 + 0.5);
				int num23 = (int)(num4 + num18);
				int num24 = num22 - num6;
				int num25 = num23 - num8;
				int l2 = num6 + num24;
				int r = num5 - num24;
				int y = num8 + num25;
				int y2 = num7 - num25;
				list.Add(new Rnage(l2, r, y));
				list.Add(new Rnage(l2, r, y2));
			}
		}
		DrawRange(list.ToArray(), color);
		DrawCircle(l, t, w, h, color);
		CanUpdatePixel = true;
	}

	public void Fill(Point p, byte color)
	{
		Fill(p.X, p.Y, color);
	}

	public void Fill(int x, int y, byte newColor)
	{
		List<Rnage> list = new List<Rnage>();
		byte pixel = GetPixel(x, y);
		RegistRange(list, x, y, pixel);
		for (int i = 0; i < list.Count; i++)
		{
			int l = list[i].L;
			int r = list[i].R;
			int y2 = list[i].Y;
			FindRegistUD(list, l, r, y2, pixel);
		}
		for (int j = 0; j < list.Count; j++)
		{
			DrawRange(list.ToArray(), newColor);
		}
		CanUpdatePixel = true;
	}

	private void DrawRange(Rnage[] ranges, byte color)
	{
		foreach (Rnage rnage in ranges)
		{
			DrawLine(rnage.L, rnage.Y, rnage.R, rnage.Y, color);
		}
	}

	private int RegistRange(List<Rnage> list, int x, int y, byte color)
	{
		int result = x;
		if (GetPixel(x, y) == color && !CheckPointInclude(list, x, y))
		{
			Rnage rnage = FindLR(x, y, color);
			list.Add(rnage);
			result = rnage.R;
		}
		return result;
	}

	private bool CheckPointInclude(List<Rnage> list, int x, int y)
	{
		bool result = false;
		foreach (Rnage item in list)
		{
			int l = item.L;
			int r = item.R;
			int y2 = item.Y;
			if (y == y2 && x >= l && x <= r)
			{
				return true;
			}
		}
		return result;
	}

	private Rnage FindLR(int x, int y, byte color)
	{
		int l = FindL(x, y, color);
		int r = FindR(x, y, color);
		return new Rnage(l, r, y);
	}

	private int FindL(int x, int y, byte color)
	{
		int result = x;
		int num = x;
		while (num >= 0 && GetPixel(num, y) == color)
		{
			result = num;
			num--;
		}
		return result;
	}

	private int FindR(int x, int y, byte color)
	{
		int result = x;
		for (int i = x; i < Width && GetPixel(i, y) == color; i++)
		{
			result = i;
		}
		return result;
	}

	private void FindRegistUD(List<Rnage> list, int l, int r, int y, byte color)
	{
		int num = y - 1;
		if (num >= 0)
		{
			int num2;
			for (num2 = l; num2 <= r; num2++)
			{
				num2 = RegistRange(list, num2, num, color);
			}
		}
		int num3 = y + 1;
		if (num3 < Height)
		{
			int num4;
			for (num4 = l; num4 <= r; num4++)
			{
				num4 = RegistRange(list, num4, num3, color);
			}
		}
	}
}
