using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace YYCHR;

public static class ClipboardEx
{
	public static Image GetImageFromClipboard()
	{
		Image result = null;
		if (Clipboard.GetDataObject() is DataObject retrievedData)
		{
			result = GetClipboardImage(retrievedData);
		}
		return result;
	}

	public static Bitmap GetBitmapFromClipboard()
	{
		Bitmap result = null;
		if (Clipboard.GetDataObject() is DataObject retrievedData)
		{
			result = GetClipboardImage(retrievedData);
		}
		return result;
	}

	public static Bitmap GetClipboardImage(DataObject retrievedData)
	{
		Bitmap bitmap = null;
		if (retrievedData.GetDataPresent("PNG") && retrievedData.GetData("PNG") is MemoryStream stream)
		{
			using Bitmap bm = new Bitmap(stream);
			bitmap = CloneImage(bm);
		}
		if (bitmap == null && retrievedData.GetDataPresent(DataFormats.Dib) && retrievedData.GetData(DataFormats.Dib) is MemoryStream memoryStream)
		{
			bitmap = ImageFromClipboardDib(memoryStream.ToArray());
		}
		if (bitmap == null && retrievedData.GetDataPresent(DataFormats.Bitmap))
		{
			bitmap = new Bitmap(retrievedData.GetData(DataFormats.Bitmap) as Image);
		}
		if (bitmap == null && retrievedData.GetDataPresent(typeof(Image)))
		{
			bitmap = new Bitmap(retrievedData.GetData(typeof(Image)) as Image);
		}
		return bitmap;
	}

	public static Bitmap ImageFromClipboardDib(byte[] dibBytes)
	{
		if (dibBytes == null || dibBytes.Length < 4)
		{
			return null;
		}
		try
		{
			int num = (int)ReadIntFromByteArray(dibBytes, 0, 4, littleEndian: true);
			if (num != 40)
			{
				return null;
			}
			byte[] array = new byte[40];
			Array.Copy(dibBytes, array, 40);
			int num2 = (int)ReadIntFromByteArray(array, 4, 4, littleEndian: true);
			int num3 = (int)ReadIntFromByteArray(array, 8, 4, littleEndian: true);
			if ((short)ReadIntFromByteArray(array, 12, 2, littleEndian: true) != 1)
			{
				return null;
			}
			short num4 = (short)ReadIntFromByteArray(array, 14, 2, littleEndian: true);
			int num5 = 0;
			if (num4 == 1 || num4 == 4 || num4 == 8)
			{
				num5 = 1 << (int)num4;
			}
			int num6 = (int)ReadIntFromByteArray(array, 16, 4, littleEndian: true);
			if (num6 != 0 && num6 != 3)
			{
				return null;
			}
			int num7 = num;
			if (num6 == 3)
			{
				int num8 = num7;
				int num9 = 12;
				if (num8 + num9 >= dibBytes.Length)
				{
					return null;
				}
				num7 += num9;
			}
			Color[] array2 = null;
			if (num5 > 0)
			{
				int num10 = num5 * 4;
				int num11 = num7;
				if (num11 + num10 >= dibBytes.Length)
				{
					return null;
				}
				array2 = new Color[num5];
				for (int i = 0; i < num5; i++)
				{
					int num12 = i * 4 + num11;
					byte b = dibBytes[num12 + 3];
					if (b == 0)
					{
						b = byte.MaxValue;
					}
					byte red = dibBytes[num12 + 2];
					byte green = dibBytes[num12 + 1];
					byte blue = dibBytes[num12];
					array2[i] = Color.FromArgb(b, red, green, blue);
				}
				num7 += num10;
			}
			int num13 = ((num4 * num2 + 7) / 8 + 3) / 4 * 4;
			int num14 = num13 * num3;
			int num15 = num7;
			if (num15 + num14 > dibBytes.Length)
			{
				return null;
			}
			byte[] array3 = new byte[num14];
			Array.Copy(dibBytes, num15, array3, 0, array3.Length);
			PixelFormat pixelFormat;
			switch (num4)
			{
			case 32:
				pixelFormat = PixelFormat.Format32bppRgb;
				break;
			case 24:
				pixelFormat = PixelFormat.Format24bppRgb;
				break;
			case 16:
				pixelFormat = PixelFormat.Format16bppRgb555;
				break;
			case 8:
				pixelFormat = PixelFormat.Format8bppIndexed;
				break;
			case 4:
				pixelFormat = PixelFormat.Format4bppIndexed;
				break;
			case 1:
				pixelFormat = PixelFormat.Format1bppIndexed;
				break;
			default:
				return null;
			}
			if (num6 == 3)
			{
				uint num16 = ReadIntFromByteArray(dibBytes, num, 4, littleEndian: true);
				uint num17 = ReadIntFromByteArray(dibBytes, num + 4, 4, littleEndian: true);
				uint num18 = ReadIntFromByteArray(dibBytes, num + 8, 4, littleEndian: true);
				if (num4 != 32 || num16 != 16711680 || num17 != 65280 || num18 != 255)
				{
					return null;
				}
				for (int j = 3; j < array3.Length; j += 4)
				{
					if (array3[j] != 0)
					{
						pixelFormat = PixelFormat.Format32bppPArgb;
						break;
					}
				}
			}
			Bitmap bitmap = BuildImage(array3, num2, num3, num13, pixelFormat, array2, null);
			bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
			return bitmap;
		}
		catch
		{
			return null;
		}
	}

	private static Bitmap CloneImage(Bitmap bm)
	{
		int stride;
		return BuildImage(GetImageData(bm, out stride), bm.Width, bm.Height, stride, bm.PixelFormat, null, null);
	}

	public static Bitmap BuildImage(byte[] sourceData, int width, int height, int stride, PixelFormat pixelFormat, Color[] palette, Color? defaultColor)
	{
		Bitmap bitmap = new Bitmap(width, height, pixelFormat);
		BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
		CopyToMemory(bitmapData.Scan0, sourceData, 0, sourceData.Length, stride, bitmapData.Stride);
		bitmap.UnlockBits(bitmapData);
		if ((pixelFormat & PixelFormat.Indexed) != 0 && palette != null)
		{
			ColorPalette palette2 = bitmap.Palette;
			for (int i = 0; i < palette2.Entries.Length; i++)
			{
				if (i < palette.Length)
				{
					palette2.Entries[i] = palette[i];
					continue;
				}
				if (!defaultColor.HasValue)
				{
					break;
				}
				palette2.Entries[i] = defaultColor.Value;
			}
			bitmap.Palette = palette2;
		}
		return bitmap;
	}

	public static void CopyToMemory(IntPtr target, byte[] bytes, int startPos, int length, int origStride, int targetStride)
	{
		int num = startPos;
		IntPtr destination = target;
		int length2 = Math.Min(origStride, targetStride);
		while (length >= targetStride)
		{
			Marshal.Copy(bytes, num, destination, length2);
			length -= origStride;
			num += origStride;
			destination = new IntPtr(destination.ToInt64() + targetStride);
		}
		if (length > 0)
		{
			Marshal.Copy(bytes, num, destination, length);
		}
	}

	public static uint ReadIntFromByteArray(byte[] data, int startIndex, int bytes, bool littleEndian)
	{
		int num = bytes - 1;
		if (data.Length < startIndex + bytes)
		{
			throw new ArgumentOutOfRangeException("startIndex", "Data array is too small to read a " + bytes + "-byte value at offset " + startIndex + ".");
		}
		uint num2 = 0u;
		for (int i = 0; i < bytes; i++)
		{
			int num3 = startIndex + (littleEndian ? i : (num - i));
			num2 += (uint)(data[num3] << 8 * i);
		}
		return num2;
	}

	public static byte[] GetImageData(Bitmap sourceImage, out int stride)
	{
		BitmapData bitmapData = sourceImage.LockBits(new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), ImageLockMode.ReadOnly, sourceImage.PixelFormat);
		stride = bitmapData.Stride;
		byte[] array = new byte[stride * sourceImage.Height];
		Marshal.Copy(bitmapData.Scan0, array, 0, array.Length);
		sourceImage.UnlockBits(bitmapData);
		return array;
	}
}
