using System.Drawing;
using CharactorLib.Common;
using CharactorLib.Data;

namespace CharactorLib.Format;

public class _2bppMSX_S2 : FormatBase
{
	public _2bppMSX_S2()
	{
		base.FormatText = "[32][32]";
		base.Name = "2BPP MSX Screen2";
		base.Extension = "bin,rom";
		base.Author = "Yy";
		base.Url = "";
		base.Readonly = true;
		base.IsCompressed = false;
		base.EnableAdf = true;
		base.IsSupportMirror = true;
		base.IsSupportRotate = true;
		base.ColorBit = 2;
		base.ColorNum = 16;
		base.CharWidth = 8;
		base.CharHeight = 8;
		base.Width = 128;
		base.Height = 128;
	}

	public int GetThisCharactorByteSize()
	{
		return base.CharWidth * base.CharHeight / 8;
	}

	public override void ConvertAllMemToChr(byte[] data, int addr, Bytemap bytemap, AdfInfo adfInfo)
	{
		bool flag = adfInfo?.IsDisableFF ?? false;
		byte[] array = adfInfo?.Pattern;
		bool flag2 = false;
		int bankByteSize = GetBankByteSize();
		if (addr + bankByteSize > data.Length)
		{
			return;
		}
		int thisCharactorByteSize = GetThisCharactorByteSize();
		for (int i = 0; i < base.RowsCount; i++)
		{
			int py = i * base.CharHeight;
			for (int j = 0; j < base.ColumnCount; j++)
			{
				byte b = (byte)(i * base.ColumnCount + j);
				if (array != null)
				{
					b = array[b];
					flag2 = flag && b == byte.MaxValue;
				}
				if (!flag2)
				{
					int addr2 = b * thisCharactorByteSize + addr;
					int px = j * base.CharWidth;
					ConvertMemToChr(data, addr2, bytemap, px, py);
				}
				else
				{
					int px2 = j * base.CharWidth;
					ConvertMemToChr(FormatBase._DmyData, 0, bytemap, px2, py);
				}
			}
		}
	}

	public override void ConvertAllChrToMem(byte[] data, int addr, Bytemap bytemap, AdfInfo adfInfo)
	{
		bool flag = adfInfo?.IsDisableFF ?? false;
		byte[] array = adfInfo?.Pattern;
		bool flag2 = false;
		int bankByteSize = GetBankByteSize();
		if (addr + bankByteSize > data.Length)
		{
			return;
		}
		int thisCharactorByteSize = GetThisCharactorByteSize();
		for (int i = 0; i < base.RowsCount; i++)
		{
			int py = i * base.CharHeight;
			for (int j = 0; j < base.ColumnCount; j++)
			{
				byte b = (byte)(i * base.ColumnCount + j);
				if (array != null)
				{
					b = array[b];
					flag2 = flag && b == byte.MaxValue;
				}
				if (!flag2)
				{
					int addr2 = b * thisCharactorByteSize + addr;
					int px = j * base.CharWidth;
					ConvertChrToMem(data, addr2, bytemap, px, py);
				}
			}
		}
	}

	public override void ConvertMemToChr(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		for (int i = 0; i < base.CharHeight; i++)
		{
			int num = i + addr;
			int num2 = num + 2048;
			byte b = data[num];
			byte b2 = data[num2];
			byte[] array = new byte[2]
			{
				(byte)(b2 & 0xFu),
				(byte)((uint)(b2 >> 4) & 0xFu)
			};
			for (int j = 0; j < base.CharWidth; j++)
			{
				byte b3 = (byte)((uint)(b >> 7 - j) & 1u);
				byte b4 = array[b3];
				Point advancePixelPoint = base.GetAdvancePixelPoint(px + j, py + i);
				int pointAddress = bytemap.GetPointAddress(advancePixelPoint.X, advancePixelPoint.Y);
				bytemap.Data[pointAddress] = b4;
			}
		}
	}

	public override void ConvertChrToMem(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
	}
}
