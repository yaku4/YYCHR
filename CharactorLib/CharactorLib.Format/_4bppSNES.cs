using CharactorLib.Common;

namespace CharactorLib.Format;

public class _4bppSNES : FormatBase
{
	public _4bppSNES()
	{
		base.FormatText = "[8*2][8*2]";
		base.Name = "4BPP SNES/PCE(CG)";
		base.Extension = "smc,sfc,fig,pce";
		base.Author = "";
		base.Url = "";
		base.Readonly = false;
		base.IsCompressed = false;
		base.EnableAdf = true;
		base.IsSupportMirror = false;
		base.IsSupportRotate = false;
		base.ColorBit = 4;
		base.ColorNum = 16;
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
			int num2 = num + 1;
			int num3 = num + 16;
			int num4 = num + 17;
			byte b = data[num];
			byte b2 = data[num2];
			byte b3 = data[num3];
			byte b4 = data[num4];
			int pointAddress = bytemap.GetPointAddress(px, py + i);
			for (int j = 0; j < base.CharWidth; j++)
			{
				byte b5 = (byte)((uint)(b >> 7 - j) & 1u);
				byte b6 = (byte)((uint)(b2 >> 7 - j) & 1u);
				byte b7 = (byte)((uint)(b3 >> 7 - j) & 1u);
				byte b8 = (byte)(((byte)((b4 >> 7 - j) & 1) << 3) | (b7 << 2) | (b6 << 1) | b5);
				bytemap.Data[pointAddress++] = b8;
			}
		}
	}

	public override void ConvertChrToMem(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		for (int i = 0; i < base.CharHeight; i++)
		{
			int num = i * 2 + addr;
			int num2 = num + 1;
			int num3 = num + 16;
			int num4 = num + 17;
			int pointAddress = bytemap.GetPointAddress(px, py + i);
			byte b = 0;
			byte b2 = 0;
			byte b3 = 0;
			byte b4 = 0;
			for (int j = 0; j < base.CharWidth; j++)
			{
				byte num5 = bytemap.Data[pointAddress++];
				int num6 = num5 & 1;
				int num7 = (num5 >> 1) & 1;
				int num8 = (num5 >> 2) & 1;
				int num9 = (num5 >> 3) & 1;
				b = (byte)(b | (byte)(num6 << 7 - j));
				b2 = (byte)(b2 | (byte)(num7 << 7 - j));
				b3 = (byte)(b3 | (byte)(num8 << 7 - j));
				b4 = (byte)(b4 | (byte)(num9 << 7 - j));
			}
			data[num] = b;
			data[num2] = b2;
			data[num3] = b3;
			data[num4] = b4;
		}
	}
}
