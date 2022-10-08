using System.Drawing;
using CharactorLib.Common;

namespace CharactorLib.Format;

public class _2bppMSX : FormatBase
{
	public _2bppMSX()
	{
		base.FormatText = "[32][32]";
		base.Name = "2BPP MSX";
		base.Extension = "bin,rom";
		base.Author = "Yy";
		base.Url = "";
		base.Readonly = false;
		base.IsCompressed = false;
		base.EnableAdf = true;
		base.IsSupportMirror = true;
		base.IsSupportRotate = true;
		base.ColorBit = 2;
		base.ColorNum = 4;
		base.CharWidth = 8;
		base.CharHeight = 8;
		base.Width = 128;
		base.Height = 128;
	}

	protected int GetMemAddress(int address, byte tile)
	{
		int num = (int)tile / 4 * 64;
		int num2 = (int)tile % 4 * 8;
		return num + num2 + address;
	}

	public override void ConvertAllMemToChr(byte[] data, int addr, Bytemap bytemap)
	{
		int num = base.Width * base.Height * base.ColorBit / 8;
		if (addr + num > data.Length)
		{
			return;
		}
		int num2 = base.CharWidth * base.CharHeight * base.ColorBit / 8;
		int num3 = num2 * 4;
		for (int i = 0; i < base.RowsCount; i++)
		{
			int py = i * base.CharHeight;
			for (int j = 0; j < base.ColumnCount; j++)
			{
				byte b = (byte)(i * base.ColumnCount + j);
				if (base.EnableAdf && base.AdfPattern != null)
				{
					b = base.AdfPattern.Pattern[b];
				}
				int num4 = (int)b / 16;
				int num5 = (int)b % 16;
				int num6 = num5 / 4 * num3 + num5 % 4 * 8;
				int addr2 = num4 * (num2 * base.ColumnCount) + num6 + addr;
				int px = j * base.CharWidth;
				ConvertMemToChr(data, addr2, bytemap, px, py);
			}
		}
	}

	public override void ConvertAllChrToMem(byte[] data, int addr, Bytemap bytemap)
	{
		int num = base.Width * base.Height * base.ColorBit / 8;
		if (addr + num > data.Length)
		{
			return;
		}
		int num2 = base.CharWidth * base.CharHeight * base.ColorBit / 8;
		int num3 = num2 * 4;
		for (int i = 0; i < base.RowsCount; i++)
		{
			int py = i * base.CharHeight;
			for (int j = 0; j < base.ColumnCount; j++)
			{
				byte b = (byte)(i * base.ColumnCount + j);
				if (base.EnableAdf && base.AdfPattern != null)
				{
					b = base.AdfPattern.Pattern[b];
				}
				int num4 = (int)b / 16;
				int num5 = (int)b % 16;
				int num6 = num5 / 4 * num3 + num5 % 4 * 8;
				int addr2 = num4 * (num2 * base.ColumnCount) + num6 + addr;
				int px = j * base.CharWidth;
				ConvertChrToMem(data, addr2, bytemap, px, py);
			}
		}
	}

	public override void ConvertMemToChr(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		for (int i = 0; i < base.CharHeight; i++)
		{
			int num = i + addr;
			int num2 = num + 32;
			byte b = data[num];
			byte b2 = data[num2];
			for (int j = 0; j < base.CharWidth; j++)
			{
				byte b3 = (byte)((uint)(b >> 7 - j) & 1u);
				byte b4 = (byte)(((byte)((b2 >> 7 - j) & 1) << 1) | b3);
				Point advancePixelPoint = base.GetAdvancePixelPoint(px + j, py + i);
				int pointAddress = bytemap.GetPointAddress(advancePixelPoint.X, advancePixelPoint.Y);
				bytemap.Data[pointAddress] = b4;
			}
		}
	}

	public override void ConvertChrToMem(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		for (int i = 0; i < base.CharHeight; i++)
		{
			int num = i + addr;
			int num2 = num + 32;
			byte b = 0;
			byte b2 = 0;
			for (int j = 0; j < base.CharWidth; j++)
			{
				Point advancePixelPoint = base.GetAdvancePixelPoint(px + j, py + i);
				int pointAddress = bytemap.GetPointAddress(advancePixelPoint.X, advancePixelPoint.Y);
				byte num3 = bytemap.Data[pointAddress];
				int num4 = num3 & 1;
				int num5 = (num3 >> 1) & 1;
				b = (byte)(b | (byte)(num4 << 7 - j));
				b2 = (byte)(b2 | (byte)(num5 << 7 - j));
			}
			data[num] = b;
			data[num2] = b2;
		}
	}
}
