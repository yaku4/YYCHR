using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace CharactorLib.Common;

public class BitmapEditor
{
	private Bitmap mBitmap;

	public Bitmap Bitmap => mBitmap;

	public BitmapEditor(Bitmap bitmap)
	{
		mBitmap = bitmap;
	}

	public byte GetPixel(Point point)
	{
		return GetPixel(point.X, point.Y);
	}

	public byte GetPixel(int x, int y)
	{
		x = CharactorCommon.NormalizeValue(0, mBitmap.Width - 1, x);
		y = CharactorCommon.NormalizeValue(0, mBitmap.Height - 1, y);
		Rectangle rect = new Rectangle(new Point(0, 0), mBitmap.Size);
		BitmapData bitmapData = mBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
		byte pixel = GetPixel(bitmapData, x, y);
		mBitmap.UnlockBits(bitmapData);
		return pixel;
	}

	public unsafe byte GetPixel(BitmapData bitmapData, int x, int y)
	{
		byte result = 0;
		int num = Math.Abs(bitmapData.Stride);
		_ = bitmapData.Height;
		byte* ptr = (byte*)(void*)bitmapData.Scan0;
		int num2 = y * num + x;
		if (num2 < 0)
		{
			return result;
		}
		return ptr[num2];
	}

	public void SetPixel(Point point, byte color)
	{
		SetPixel(point.X, point.Y, color);
	}

	public void SetPixel(int x, int y, byte color)
	{
		x = CharactorCommon.NormalizeValue(0, mBitmap.Width - 1, x);
		y = CharactorCommon.NormalizeValue(0, mBitmap.Height - 1, y);
		Rectangle rect = new Rectangle(new Point(0, 0), mBitmap.Size);
		BitmapData bitmapData = mBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
		SetPixel(bitmapData, x, y, color);
		mBitmap.UnlockBits(bitmapData);
	}

	public unsafe void SetPixel(BitmapData bitmapData, int x, int y, byte color)
	{
		int num = Math.Abs(bitmapData.Stride);
		_ = bitmapData.Height;
		byte* ptr = (byte*)(void*)bitmapData.Scan0;
		int num2 = y * num + x;
		if (num2 >= 0)
		{
			ptr[num2] = color;
		}
	}

	public void DrawLine(Point point1, Point point2, byte color)
	{
		DrawLine(point1.X, point1.Y, point2.X, point2.Y, color);
	}

	public unsafe void DrawLine(int x1, int y1, int x2, int y2, byte color)
	{
		x1 = CharactorCommon.NormalizeValue(0, mBitmap.Width - 1, x1);
		y1 = CharactorCommon.NormalizeValue(0, mBitmap.Height - 1, y1);
		x2 = CharactorCommon.NormalizeValue(0, mBitmap.Width - 1, x2);
		y2 = CharactorCommon.NormalizeValue(0, mBitmap.Height - 1, y2);
		SetPixel(x1, y1, color);
		if (x1 == x2 && y1 == y2)
		{
			return;
		}
		Rectangle rect = new Rectangle(new Point(0, 0), mBitmap.Size);
		BitmapData bitmapData = mBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
		int num = x2 - x1;
		int num2 = y2 - y1;
		int num3 = Math.Abs(num);
		int num4 = Math.Abs(num2);
		int num5 = Math.Abs(bitmapData.Stride);
		int num6 = ((x1 < x2) ? 1 : (-1));
		int num7 = ((y1 < y2) ? 1 : (-1));
		byte* ptr = (byte*)(void*)bitmapData.Scan0;
		if (num3 >= num4)
		{
			double num8 = (double)num2 / (double)num;
			for (int i = 0; i <= num3; i++)
			{
				int num9 = x1 + num6 * i;
				int num10 = (y1 + (int)((double)(num6 * i) * num8 + (double)num7 * 0.5)) * num5 + num9;
				ptr[num10] = color;
			}
		}
		else
		{
			double num8 = (double)num / (double)num2;
			for (int j = 0; j <= num4; j++)
			{
				int num11 = y1 + num7 * j;
				int num12 = x1 + (int)((double)(num7 * j) * num8 + (double)num6 * 0.5);
				int num10 = num11 * num5 + num12;
				ptr[num10] = color;
			}
		}
		mBitmap.UnlockBits(bitmapData);
	}

	public void DrawRect(Rectangle rect, byte color)
	{
		DrawRect(rect.Left, rect.Top, rect.Right, rect.Bottom, color);
	}

	public unsafe void DrawRect(int x1, int y1, int x2, int y2, byte color)
	{
		x1 = CharactorCommon.NormalizeValue(0, mBitmap.Width - 1, x1);
		y1 = CharactorCommon.NormalizeValue(0, mBitmap.Height - 1, y1);
		x2 = CharactorCommon.NormalizeValue(0, mBitmap.Width - 1, x2);
		y2 = CharactorCommon.NormalizeValue(0, mBitmap.Height - 1, y2);
		int[] array = CharactorCommon.SortValue(x1, x2);
		int[] array2 = CharactorCommon.SortValue(y1, y2);
		x1 = array[0];
		x2 = array[1];
		y1 = array2[0];
		y2 = array2[1];
		Rectangle rect = new Rectangle(new Point(0, 0), mBitmap.Size);
		BitmapData bitmapData = mBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
		int w = Math.Abs(bitmapData.Stride);
		int height = bitmapData.Height;
		byte* scanline = (byte*)(void*)bitmapData.Scan0;
		DrawLineH(scanline, w, height, y1, x1, x2, color);
		DrawLineH(scanline, w, height, y2, x1, x2, color);
		DrawLineV(scanline, w, height, x1, y1, y2, color);
		DrawLineV(scanline, w, height, x2, y1, y2, color);
		mBitmap.UnlockBits(bitmapData);
	}

	private unsafe void DrawLineH(byte* scanline, int w, int h, int y, int x1, int x2, byte color)
	{
		for (int i = x1; i <= x2; i++)
		{
			int num = y * w + i;
			scanline[num] = color;
		}
	}

	private unsafe void DrawLineV(byte* scanline, int w, int h, int x, int y1, int y2, byte color)
	{
		for (int i = y1; i < y2; i++)
		{
			int num = i * w + x;
			scanline[num] = color;
		}
	}

	public void FillRect(Rectangle rect, byte color)
	{
		FillRect(rect.Left, rect.Top, rect.Right, rect.Bottom, color);
	}

	public unsafe void FillRect(int x1, int y1, int x2, int y2, byte color)
	{
		x1 = CharactorCommon.NormalizeValue(0, mBitmap.Width - 1, x1);
		y1 = CharactorCommon.NormalizeValue(0, mBitmap.Height - 1, y1);
		x2 = CharactorCommon.NormalizeValue(0, mBitmap.Width - 1, x2);
		y2 = CharactorCommon.NormalizeValue(0, mBitmap.Height - 1, y2);
		int[] array = CharactorCommon.SortValue(x1, x2);
		int[] array2 = CharactorCommon.SortValue(y1, y2);
		x1 = array[0];
		x2 = array[1];
		y1 = array2[0];
		y2 = array2[1];
		Rectangle rect = new Rectangle(new Point(0, 0), mBitmap.Size);
		BitmapData bitmapData = mBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
		int w = Math.Abs(bitmapData.Stride);
		int height = bitmapData.Height;
		byte* scanline = (byte*)(void*)bitmapData.Scan0;
		for (int i = y1; i <= y2; i++)
		{
			DrawLineH(scanline, w, height, i, x1, x2, color);
		}
		mBitmap.UnlockBits(bitmapData);
	}

	public void DrawCircle(Rectangle rect, byte color)
	{
		DrawCircle(rect.Left, rect.Top, rect.Right, rect.Bottom, color);
	}

	public unsafe void DrawCircle(int x1, int y1, int x2, int y2, byte color)
	{
		x1 = CharactorCommon.NormalizeValue(0, mBitmap.Width - 1, x1);
		y1 = CharactorCommon.NormalizeValue(0, mBitmap.Height - 1, y1);
		x2 = CharactorCommon.NormalizeValue(0, mBitmap.Width - 1, x2);
		y2 = CharactorCommon.NormalizeValue(0, mBitmap.Height - 1, y2);
		int[] array = CharactorCommon.SortValue(x1, x2);
		int[] array2 = CharactorCommon.SortValue(y1, y2);
		x1 = array[0];
		x2 = array[1];
		y1 = array2[0];
		y2 = array2[1];
		Rectangle rect = new Rectangle(new Point(0, 0), mBitmap.Size);
		BitmapData bitmapData = mBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
		Math.Abs(bitmapData.Stride);
		_ = bitmapData.Height;
		byte* ptr = (byte*)(void*)bitmapData.Scan0;
		_ = (x1 + x2) / 2;
		_ = (y1 + y2) / 2;
		Ellipse(bitmapData, color, x1, y1, x2, y2);
		mBitmap.UnlockBits(bitmapData);
	}

	private void Ellipse(BitmapData bitmapData, byte color, int x1, int y1, int x2, int y2)
	{
		float num = (float)(x2 + x1) / 2f;
		float num2 = (float)(y2 + y1) / 2f;
		float num3 = (float)(x2 - x1) / 2f;
		float num4 = (float)(y2 - y1) / 2f;
		int num5 = 2 * (x2 - x1 + (y2 - y1));
		List<PointF> list = new List<PointF>();
		for (int i = 0; i < num5 / 6; i++)
		{
			double num6 = (double)(i * 2) * Math.PI / (double)(num5 - 1);
			float num7 = (float)Math.Cos(num6);
			float num8 = (float)Math.Sin(num6);
			list.Add(new PointF(num + num3 * num7, num2 + num4 * num8));
			list.Add(new PointF(num - num3 * num7, num2 + num4 * num8));
			list.Add(new PointF(num + num3 * num7, num2 - num4 * num8));
			list.Add(new PointF(num - num3 * num7, num2 - num4 * num8));
			list.Add(new PointF(num + num3 * num8, num2 + num4 * num7));
			list.Add(new PointF(num - num3 * num8, num2 + num4 * num7));
			list.Add(new PointF(num + num3 * num8, num2 - num4 * num7));
			list.Add(new PointF(num - num3 * num8, num2 - num4 * num7));
		}
		foreach (PointF item in list)
		{
			Point point = new Point((int)(item.X + 0.5f), (int)(item.Y + 0.5f));
			SetPixel(bitmapData, point.X, point.Y, color);
		}
	}

	public void FillCircle(Rectangle rect, byte color)
	{
		FillCircle(rect.Left, rect.Top, rect.Right, rect.Bottom, color);
	}

	public void FillCircle(int x1, int y1, int x2, int y2, byte color)
	{
		DrawRect(x1, y1, x2, y2, color);
	}

	public unsafe void CopyRect(Point destPoint, Bitmap srcBitmap, Rectangle srcRect)
	{
		Rectangle rect = new Rectangle(new Point(0, 0), mBitmap.Size);
		BitmapData bitmapData = mBitmap.LockBits(rect, ImageLockMode.WriteOnly, mBitmap.PixelFormat);
		BitmapData bitmapData2 = srcBitmap.LockBits(srcRect, ImageLockMode.ReadOnly, srcBitmap.PixelFormat);
		int num = Math.Abs(bitmapData.Stride) / 4;
		int height = bitmapData.Height;
		int* ptr = (int*)(void*)bitmapData.Scan0;
		int* ptr2 = (int*)(void*)bitmapData2.Scan0;
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < num; j++)
			{
				int num2 = i * num + j;
				ptr[num2] = ptr2[num2];
			}
		}
		mBitmap.UnlockBits(bitmapData);
		srcBitmap.UnlockBits(bitmapData2);
	}

	public unsafe static void CopyRect(Bitmap destBitmap, Point destPoint, Bitmap srcBitmap, Rectangle srcRect)
	{
		Rectangle rect = new Rectangle(new Point(0, 0), srcBitmap.Size);
		Rectangle rect2 = new Rectangle(new Point(0, 0), destBitmap.Size);
		BitmapData bitmapData = srcBitmap.LockBits(rect, ImageLockMode.ReadOnly, srcBitmap.PixelFormat);
		BitmapData bitmapData2 = destBitmap.LockBits(rect2, ImageLockMode.WriteOnly, destBitmap.PixelFormat);
		int num = Math.Abs(bitmapData.Stride);
		int num2 = Math.Abs(bitmapData2.Stride);
		byte* ptr = (byte*)(void*)bitmapData2.Scan0;
		byte* ptr2 = (byte*)(void*)bitmapData.Scan0;
		for (int i = 0; i < srcRect.Height; i++)
		{
			int num3 = i + srcRect.Top;
			int num4 = i + destPoint.Y;
			if (num3 >= rect.Height || num4 >= rect2.Height)
			{
				break;
			}
			for (int j = 0; j < srcRect.Width; j++)
			{
				int num5 = j + srcRect.Left;
				int num6 = j + destPoint.X;
				if (num5 >= rect.Width || num6 >= rect2.Width)
				{
					break;
				}
				int num7 = num3 * num + num5;
				int num8 = num4 * num2 + num6;
				ptr[num8] = ptr2[num7];
			}
		}
		srcBitmap.UnlockBits(bitmapData);
		destBitmap.UnlockBits(bitmapData2);
	}

	public Bitmap GetRectImage(Rectangle rectangle)
	{
		Bitmap bitmap = Bitmap;
		Bitmap obj = new Bitmap(rectangle.Width, rectangle.Height, PixelFormat.Format8bppIndexed)
		{
			Palette = bitmap.Palette
		};
		CopyRect(obj, new Point(0, 0), bitmap, rectangle);
		return obj;
	}

	public static byte[] GetDibFromBitmap(Bitmap bitmap, Rectangle rect)
	{
		return new List<byte>().ToArray();
	}

	public static Bitmap GetBitmapFromDib(byte[] dib)
	{
		return null;
	}

	public static void UpdatePalette(Bitmap bitmap, Color[] colors)
	{
		ColorPalette palette = bitmap.Palette;
		for (int i = 0; i < colors.Length; i++)
		{
			palette.Entries[i] = colors[i];
		}
		bitmap.Palette = palette;
	}

	public void UpdatePalette(Color[] colors)
	{
		UpdatePalette(Bitmap, colors);
	}

	public static void UpdatePalette(Bitmap bitmap, int index, Color color)
	{
		ColorPalette palette = bitmap.Palette;
		palette.Entries[index] = color;
		bitmap.Palette = palette;
	}

	public void UpdatePalette(int index, Color color)
	{
		UpdatePalette(Bitmap, index, color);
	}

	public unsafe void SetPaletteSet(byte paletteSetSize, byte paletteSetNo)
	{
		byte b = (byte)(paletteSetNo * paletteSetSize);
		Rectangle rect = new Rectangle(new Point(0, 0), Bitmap.Size);
		BitmapData bitmapData = Bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
		int num = Math.Abs(bitmapData.Stride);
		int height = bitmapData.Height;
		byte* ptr = (byte*)(void*)bitmapData.Scan0;
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < num; j++)
			{
				int num2 = i * num + j;
				byte b2 = ptr[num2];
				ptr[num2] = (byte)((int)b2 % (int)paletteSetSize + b);
			}
		}
		Bitmap.UnlockBits(bitmapData);
	}

	public unsafe static void Rotate(Bitmap destBitmap, Rectangle destRect, Bitmap srcBitmap, Rectangle srcRect, RotateType rotateType)
	{
		if (srcBitmap == null || destBitmap == null || srcRect.Width != srcRect.Height || destRect.Width != destRect.Height)
		{
			return;
		}
		Rectangle rect = new Rectangle(new Point(0, 0), srcBitmap.Size);
		Rectangle rect2 = new Rectangle(new Point(0, 0), destBitmap.Size);
		BitmapData bitmapData = srcBitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
		BitmapData bitmapData2 = destBitmap.LockBits(rect2, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
		int height = srcRect.Height;
		Math.Abs(bitmapData2.Stride);
		int height2 = bitmapData2.Height;
		byte* ptr = (byte*)(void*)bitmapData.Scan0;
		byte* ptr2 = (byte*)(void*)bitmapData2.Scan0;
		for (int i = 0; i < height; i++)
		{
			int num = destRect.Top + i;
			if (num < 0 || num >= height2)
			{
				break;
			}
			for (int j = 0; j < height; j++)
			{
				int num2 = destRect.Left + j;
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
				if (num4 < 0 || num4 >= height2 || num3 < 0 || num3 >= height2 || num2 < 0 || num2 >= height2)
				{
					break;
				}
				int num5 = num3 * rect.Width + num4;
				int num6 = num * rect2.Width + num2;
				ptr2[num6] = ptr[num5];
			}
		}
		srcBitmap.UnlockBits(bitmapData);
		destBitmap.UnlockBits(bitmapData2);
	}

	public static void Rotate(Bitmap srcBitmap, Rectangle srcRect, RotateType rotateType)
	{
		if (srcBitmap == null)
		{
			return;
		}
		using Bitmap bitmap = new Bitmap(srcBitmap.Width, srcBitmap.Height, srcBitmap.PixelFormat);
		CopyRect(bitmap, new Point(0, 0), srcBitmap, new Rectangle(new Point(0, 0), srcBitmap.Size));
		Rotate(srcBitmap, srcRect, bitmap, srcRect, rotateType);
	}

	public void Rotate(Rectangle srcRect, RotateType rotateType)
	{
		if (mBitmap != null)
		{
			Rotate(mBitmap, srcRect, rotateType);
		}
	}

	public unsafe static void Mirror(Bitmap dstBitmap, Point dstPoint, Bitmap srcBitmap, Rectangle srcRect, MirrorType mirrorType)
	{
		if (srcBitmap == null || dstBitmap == null)
		{
			return;
		}
		Rectangle rect = new Rectangle(new Point(0, 0), srcBitmap.Size);
		Rectangle rect2 = new Rectangle(new Point(0, 0), dstBitmap.Size);
		BitmapData bitmapData = srcBitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
		BitmapData bitmapData2 = dstBitmap.LockBits(rect2, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
		int num = Math.Abs(bitmapData2.Stride);
		int num2 = Math.Abs(bitmapData.Stride);
		byte* ptr = (byte*)(void*)bitmapData.Scan0;
		byte* ptr2 = (byte*)(void*)bitmapData2.Scan0;
		for (int i = 0; i < srcRect.Height; i++)
		{
			int num3 = dstPoint.Y + i;
			if (num3 < 0 || num3 >= rect2.Height)
			{
				break;
			}
			int num4 = ((mirrorType != MirrorType.Vertical && mirrorType != MirrorType.Both) ? (srcRect.Top + i) : (srcRect.Bottom - 1 - i));
			if (num4 < 0 || num4 >= rect.Height)
			{
				break;
			}
			for (int j = 0; j < srcRect.Width; j++)
			{
				int num5 = dstPoint.X + j;
				if (num5 < 0 || num5 >= rect2.Width)
				{
					break;
				}
				int num6 = ((mirrorType != MirrorType.Horizontal && mirrorType != MirrorType.Both) ? (srcRect.Left + j) : (srcRect.Right - 1 - j));
				if (num6 < 0 || num6 >= rect.Width)
				{
					break;
				}
				int num7 = num4 * num2 + num6;
				int num8 = num3 * num + num5;
				ptr2[num8] = ptr[num7];
			}
		}
		srcBitmap.UnlockBits(bitmapData);
		dstBitmap.UnlockBits(bitmapData2);
	}

	public static void Mirror(Bitmap srcBitmap, Rectangle srcRect, MirrorType mirrorType)
	{
		if (srcBitmap == null)
		{
			return;
		}
		using Bitmap bitmap = new Bitmap(srcBitmap.Width, srcBitmap.Height, srcBitmap.PixelFormat);
		CopyRect(bitmap, new Point(0, 0), srcBitmap, new Rectangle(new Point(0, 0), srcBitmap.Size));
		Mirror(srcBitmap, srcRect.Location, bitmap, srcRect, mirrorType);
	}

	public void Mirror(Rectangle srcRect, MirrorType mirrorType)
	{
		if (mBitmap != null)
		{
			Mirror(mBitmap, srcRect, mirrorType);
		}
	}

	public static void Shift(Bitmap dstBitmap, Point dstPoint, Bitmap srcBitmap, Rectangle rect, ShiftType shiftType)
	{
		if (srcBitmap != null && dstBitmap != null)
		{
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
			CopyRect(dstBitmap, destPoint, srcBitmap, srcRect);
			CopyRect(dstBitmap, destPoint2, srcBitmap, srcRect2);
		}
	}

	public static void Shift(Bitmap srcBitmap, Rectangle srcRect, ShiftType shiftType)
	{
		if (srcBitmap == null)
		{
			return;
		}
		using Bitmap bitmap = new Bitmap(srcBitmap.Width, srcBitmap.Height, srcBitmap.PixelFormat);
		CopyRect(bitmap, new Point(0, 0), srcBitmap, new Rectangle(new Point(0, 0), srcBitmap.Size));
		Shift(srcBitmap, srcRect.Location, bitmap, srcRect, shiftType);
	}

	public void Shift(Rectangle srcRect, ShiftType shiftType)
	{
		if (mBitmap != null)
		{
			Shift(mBitmap, srcRect, shiftType);
		}
	}

	public void LoadBitmap(Bitmap bitmap)
	{
		if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
		{
			mBitmap = bitmap;
		}
	}
}
