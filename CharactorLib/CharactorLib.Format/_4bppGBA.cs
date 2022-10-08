using CharactorLib.Common;

namespace CharactorLib.Format;

public class _4bppGBA : FormatBase
{
	public _4bppGBA()
	{
		base.FormatText = "[8*2][8*2]";
		base.Name = "4BPP GBA";
		base.Extension = "gba";
		base.Author = "Yy";
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
		int num = addr;
		for (int i = 0; i < base.CharHeight; i++)
		{
			int pointAddress = bytemap.GetPointAddress(px, py + i);
			for (int j = 0; j < 4; j++)
			{
				byte b = data[num++];
				bytemap.Data[pointAddress++] = (byte)(b & 0xFu);
				bytemap.Data[pointAddress++] = (byte)((uint)(b >> 4) & 0xFu);
			}
		}
	}

	public override void ConvertChrToMem(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		int num = addr;
		for (int i = 0; i < base.CharHeight; i++)
		{
			int pointAddress = bytemap.GetPointAddress(px, py + i);
			for (int j = 0; j < 4; j++)
			{
				byte b = bytemap.Data[pointAddress++];
				byte b2 = bytemap.Data[pointAddress++];
				data[num++] = (byte)(b | (b2 << 4));
			}
		}
	}
}
