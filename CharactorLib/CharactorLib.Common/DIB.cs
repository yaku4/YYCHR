using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CharactorLib.Common;

public static class DIB
{
	private const int BITMAPFILEHEADER_LENGTH = 14;

	public static byte[] ConvertFromBitmap(Bitmap bitmap)
	{
		byte[] array = null;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			bitmap.Save(memoryStream, ImageFormat.Bmp);
			memoryStream.Close();
			array = memoryStream.GetBuffer();
		}
		byte[] array2 = new byte[array.Length - 14];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = array[14 + i];
		}
		return array2;
	}

	public static byte[] ConvertFromBitmap(Bitmap bitmap, Rectangle rect)
	{
		using Bitmap bitmap2 = new Bitmap(rect.Width, rect.Height, PixelFormat.Format8bppIndexed);
		BitmapEditor.CopyRect(bitmap2, new Point(0, 0), bitmap, rect);
		bitmap2.Palette = bitmap.Palette;
		return ConvertFromBitmap(bitmap2);
	}

	public static Bitmap ConvertToBitmap(byte[] dib)
	{
		int num = ReadInt32(dib, 0);
		byte[] array = new byte[dib.Length + 14];
		array[0] = 66;
		array[1] = 77;
		WriteInt32(array, 2, dib.Length + 14);
		WriteInt32(array, 6, 0);
		WriteInt32(array, 10, num + 1024 + 14);
		for (int i = 0; i < dib.Length; i++)
		{
			array[14 + i] = dib[i];
		}
		Bitmap bitmap = null;
		using MemoryStream memoryStream = new MemoryStream(array);
		while (memoryStream.Length != array.Length)
		{
		}
		bitmap = new Bitmap(memoryStream);
		memoryStream.Close();
		return bitmap;
	}

	public static Bitmap ConvertToBitmap(MemoryStream stream)
	{
		int num = (int)stream.Length;
		byte[] array = new byte[num];
		stream.Read(array, 0, num);
		stream.Close();
		return ConvertToBitmap(array);
	}

	private static int ReadInt32(byte[] data, int addr)
	{
		byte b = data[addr];
		byte b2 = data[addr + 1];
		byte b3 = data[addr + 2];
		return (data[addr + 3] << 24) | (b3 << 16) | (b2 << 8) | b;
	}

	private static void WriteInt32(byte[] data, int addr, int value)
	{
		data[addr] = (byte)((uint)value & 0xFFu);
		data[addr + 1] = (byte)((uint)(value >> 8) & 0xFFu);
		data[addr + 2] = (byte)((uint)(value >> 16) & 0xFFu);
		data[addr + 3] = (byte)((uint)(value >> 24) & 0xFFu);
	}
}
