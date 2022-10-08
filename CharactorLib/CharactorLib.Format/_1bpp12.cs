using CharactorLib.Common;
using CharactorLib.Data;

namespace CharactorLib.Format;

public class _1bpp12 : FormatBase
{
	public _1bpp12()
	{
		base.Name = "1BPP 16x12 (FF5)";
		base.Extension = "";
		base.Author = "Yy";
		base.Url = "TODO: make plugin";
		base.Readonly = false;
		base.IsCompressed = false;
		base.EnableAdf = false;
		base.IsSupportMirror = false;
		base.IsSupportRotate = false;
		base.ColorBit = 1;
		base.ColorNum = 2;
		base.Width = 128;
		base.Height = 128;
		base.CharWidth = 16;
		base.CharHeight = 12;
	}

	public override void ConvertAllMemToChr(byte[] data, int addr, Bytemap bytemap, AdfInfo adfInfo)
	{
		int bankByteSize = GetBankByteSize();
		if (addr + bankByteSize > data.Length)
		{
			return;
		}
		int charactorByteSize = GetCharactorByteSize();
		int num = 0;
		for (int i = 0; i < base.Height; i += base.CharHeight)
		{
			for (int j = 0; j < base.Width; j += base.CharWidth)
			{
				int addr2 = addr + num * charactorByteSize;
				ConvertMemToChr(data, addr2, bytemap, j, i);
				num++;
			}
		}
	}

	public override void ConvertAllChrToMem(byte[] data, int addr, Bytemap bytemap, AdfInfo adfInfo)
	{
		int bankByteSize = GetBankByteSize();
		if (addr + bankByteSize > data.Length)
		{
			return;
		}
		int charactorByteSize = GetCharactorByteSize();
		int num = 0;
		for (int i = 0; i < base.Height; i += base.CharHeight)
		{
			for (int j = 0; j < base.Width; j += base.CharWidth)
			{
				int addr2 = addr + num * charactorByteSize;
				ConvertChrToMem(data, addr2, bytemap, j, i);
				num++;
			}
		}
	}

	public override void ConvertMemToChr(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		for (int i = 0; i < base.CharHeight; i++)
		{
			int num = py + i;
			if (num >= base.Height)
			{
				continue;
			}
			int num2 = addr + i;
			int pointAddress = bytemap.GetPointAddress(px, num);
			if (px + 7 < base.Width)
			{
				byte b = data[num2];
				for (int j = 0; j < 8; j++)
				{
					byte b2 = (byte)((uint)(b >> 7 - j) & 1u);
					bytemap.Data[pointAddress++] = b2;
				}
			}
			if (px + 15 < base.Width)
			{
				byte b3 = data[num2 + 12];
				for (int k = 0; k < 8; k++)
				{
					byte b4 = (byte)((uint)(b3 >> 7 - k) & 1u);
					bytemap.Data[pointAddress++] = b4;
				}
			}
		}
	}

	public override void ConvertChrToMem(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		for (int i = 0; i < base.CharHeight; i++)
		{
			int num = py + i;
			if (num >= base.Height)
			{
				continue;
			}
			int pointAddress = bytemap.GetPointAddress(px, num);
			int num2 = addr + i;
			if (px + 7 < base.Width)
			{
				byte b = 0;
				for (int j = 0; j < 8; j++)
				{
					byte b2 = (byte)(bytemap.Data[pointAddress++] & 1u);
					b = (byte)(b | (byte)(b2 << 7 - j));
				}
				data[num2] = b;
			}
			if (px + 15 < base.Width)
			{
				byte b3 = 0;
				for (int k = 0; k < 8; k++)
				{
					byte b4 = (byte)(bytemap.Data[pointAddress++] & 1u);
					b3 = (byte)(b3 | (byte)(b4 << 7 - k));
				}
				data[num2 + 12] = b3;
			}
		}
	}
}
