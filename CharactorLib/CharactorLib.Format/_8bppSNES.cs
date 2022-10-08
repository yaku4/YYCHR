using CharactorLib.Common;

namespace CharactorLib.Format;

public class _8bppSNES : FormatBase
{
	public _8bppSNES()
	{
		base.FormatText = "8X*8Y";
		base.Name = "8BPP SNES(128x128)";
		base.Extension = "";
		base.Author = "Yy";
		base.Url = "";
		base.Readonly = false;
		base.IsCompressed = false;
		base.EnableAdf = true;
		base.IsSupportMirror = false;
		base.IsSupportRotate = false;
		base.ColorBit = 8;
		base.ColorNum = 256;
		base.CharWidth = 128;
		base.CharHeight = 128;
		base.Width = 128;
		base.Height = 128;
	}

	public override void ConvertMemToChr(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		int num = addr;
		for (int i = 0; i < base.CharHeight; i++)
		{
			int pointAddress = bytemap.GetPointAddress(px, py + i);
			for (int j = 0; j < base.CharWidth; j++)
			{
				bytemap.Data[pointAddress++] = data[num++];
			}
		}
	}

	public override void ConvertChrToMem(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		int num = addr;
		for (int i = 0; i < base.CharHeight; i++)
		{
			int pointAddress = bytemap.GetPointAddress(px, py + i);
			for (int j = 0; j < base.CharWidth; j++)
			{
				data[num++] = bytemap.Data[pointAddress++];
			}
		}
	}
}
