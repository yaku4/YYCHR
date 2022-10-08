using CharactorLib.Common;

namespace CharactorLib.Format;

public class _3bppSNES : FormatBase
{
	public _3bppSNES()
	{
		base.FormatText = "[8x2L8Y][8x]";
		base.Name = "3BPP SNES";
		base.Extension = "";
		base.Author = "Yy";
		base.Url = "";
		base.Readonly = false;
		base.IsCompressed = false;
		base.EnableAdf = true;
		base.IsSupportMirror = false;
		base.IsSupportRotate = false;
		base.ColorBit = 3;
		base.ColorNum = 8;
		base.CharWidth = 8;
		base.CharHeight = 8;
		base.Width = 128;
		base.Height = 128;
	}

	public override void ConvertMemToChr(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		for (int i = 0; i < base.CharHeight; i++)
		{
			int num = i * 2 + addr;
			int num2 = i * 2 + addr + 1;
			int num3 = i + addr + 16;
			byte b = data[num];
			byte b2 = data[num2];
			byte b3 = data[num3];
			int pointAddress = bytemap.GetPointAddress(px, py + i);
			for (int j = 0; j < base.CharWidth; j++)
			{
				byte b4 = (byte)((uint)(b >> 7 - j) & 1u);
				byte b5 = (byte)((uint)(b2 >> 7 - j) & 1u);
				byte b6 = (byte)(((byte)((b3 >> 7 - j) & 1) << 2) | (b5 << 1) | b4);
				bytemap.Data[pointAddress++] = b6;
			}
		}
	}

	public override void ConvertChrToMem(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		for (int i = 0; i < base.CharHeight; i++)
		{
			int num = i * 2 + addr;
			int num2 = i * 2 + addr + 1;
			int num3 = i + addr + 16;
			int pointAddress = bytemap.GetPointAddress(px, py + i);
			byte b = 0;
			byte b2 = 0;
			byte b3 = 0;
			for (int j = 0; j < base.CharWidth; j++)
			{
				byte num4 = bytemap.Data[pointAddress++];
				int num5 = num4 & 1;
				int num6 = (num4 >> 1) & 1;
				int num7 = (num4 >> 2) & 1;
				b = (byte)(b | (byte)(num5 << 7 - j));
				b2 = (byte)(b2 | (byte)(num6 << 7 - j));
				b3 = (byte)(b3 | (byte)(num7 << 7 - j));
			}
			data[num] = b;
			data[num2] = b2;
			data[num3] = b3;
		}
	}
}
