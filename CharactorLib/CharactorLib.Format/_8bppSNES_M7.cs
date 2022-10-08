using CharactorLib.Common;

namespace CharactorLib.Format;

public class _8bppSNES_M7 : FormatBase
{
	public _8bppSNES_M7()
	{
		base.FormatText = "[8*2][8*2]";
		base.Name = "8BPP SNES Mode7";
		base.Extension = "smc,sfc,fig";
		base.Author = "";
		base.Url = "";
		base.Readonly = false;
		base.IsCompressed = false;
		base.EnableAdf = true;
		base.IsSupportMirror = false;
		base.IsSupportRotate = false;
		base.ColorBit = 8;
		base.ColorNum = 256;
		base.CharWidth = 8;
		base.CharHeight = 8;
		base.Width = 128;
		base.Height = 128;
	}

	public override void ConvertMemToChr(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		for (int i = 0; i < base.CharHeight; i++)
		{
			int num = addr + i * base.CharWidth;
			int pointAddress = bytemap.GetPointAddress(px, py + i);
			for (int j = 0; j < base.CharWidth; j++)
			{
				int num2 = num + j;
				byte b = data[num2];
				bytemap.Data[pointAddress++] = b;
			}
		}
	}

	public override void ConvertChrToMem(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		for (int i = 0; i < base.CharHeight; i++)
		{
			int num = addr + i * base.CharWidth;
			int pointAddress = bytemap.GetPointAddress(px, py + i);
			for (int j = 0; j < base.CharWidth; j++)
			{
				byte b = bytemap.Data[pointAddress++];
				int num2 = num + j;
				data[num2] = b;
			}
		}
	}
}
