using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace CharactorLib.Common;

public static class BytemapConvertor
{
	public static void UpdateBitmapPaletteFromBytemap(Bitmap bitmap, Bytemap bytemap)
	{
		ColorPalette palette = bitmap.Palette;
		for (int i = 0; i < 256; i++)
		{
			palette.Entries[i] = bytemap.Palette[i];
		}
		bitmap.Palette = palette;
	}

	public unsafe static void UpdateBitmapFromBytemap(Bitmap bitmap, Bytemap bytemap)
	{
		if (bitmap.PixelFormat != PixelFormat.Format8bppIndexed)
		{
			throw new ArgumentException("Check PixelFormat is Format8bppIndexed.");
		}
		if (bitmap.Width != bytemap.Width || bitmap.Height != bytemap.Height)
		{
			throw new ArgumentException("Check Width and Height.");
		}
		Rectangle rect = new Rectangle(new Point(0, 0), bitmap.Size);
		BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
		int num = Math.Abs(bitmapData.Stride);
		byte* ptr = (byte*)(void*)bitmapData.Scan0;
		for (int i = 0; i < rect.Height; i++)
		{
			for (int j = 0; j < rect.Width; j++)
			{
				int num2 = i * num + j;
				int num3 = i * bytemap.Width + j;
				ptr[num2] = bytemap.Data[num3];
			}
		}
		bitmap.UnlockBits(bitmapData);
	}

	public static void UpdateBitmapAllFromBytemap(Bitmap bitmap, Bytemap bytemap)
	{
		UpdateBitmapPaletteFromBytemap(bitmap, bytemap);
		UpdateBitmapFromBytemap(bitmap, bytemap);
	}

	public static void UpdateBytemapPaletteFromBitmap(Bitmap bitmap, Bytemap bytemap)
	{
		if (bitmap.Palette != null && bitmap.Palette.Entries != null)
		{
			ColorPalette palette = bitmap.Palette;
			for (int i = 0; i < 256; i++)
			{
				bytemap.Palette[i] = palette.Entries[i];
			}
			bytemap.CanUpdatePalette = true;
		}
	}

	public unsafe static void UpdateBytemapFromBitmap(Bitmap bitmap, Bytemap bytemap)
	{
		if (bitmap.PixelFormat != PixelFormat.Format8bppIndexed)
		{
			throw new ArgumentException("Check PixelFormat is Format8bppIndexed.");
		}
		if (bitmap.Width != bytemap.Width || bitmap.Height != bytemap.Height)
		{
			throw new ArgumentException("Check Width and Height.");
		}
		Rectangle rect = new Rectangle(new Point(0, 0), bitmap.Size);
		BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
		int num = Math.Abs(bitmapData.Stride);
		byte* ptr = (byte*)(void*)bitmapData.Scan0;
		for (int i = 0; i < rect.Height; i++)
		{
			for (int j = 0; j < rect.Width; j++)
			{
				int num2 = i * num + j;
				int num3 = i * bytemap.Width + j;
				bytemap.Data[num3] = ptr[num2];
			}
		}
		bitmap.UnlockBits(bitmapData);
		bytemap.CanUpdatePixel = true;
	}

	public static void AddPalSet(Bytemap bankBytemap, byte[] colPattern, int paletteSetColorNum)
	{
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				int num = i * 16 + j;
				if (num > 255)
				{
					num = (byte)num;
				}
				if (num < 0)
				{
					num = (byte)num;
				}
				byte b = 0;
				if (num >= 0 && num < colPattern.Length)
				{
					b = colPattern[num];
				}
				int num2 = b * paletteSetColorNum;
				if (num2 > 255)
				{
					num2 = (byte)num2;
				}
				if (num2 < 0)
				{
					num2 = (byte)num2;
				}
				Rectangle rect = new Rectangle(j * 8, i * 8, 8, 8);
				bankBytemap.AddIndexRect(rect, num2);
			}
		}
	}
}
