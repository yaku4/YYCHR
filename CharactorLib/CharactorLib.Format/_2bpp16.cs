using CharactorLib.Common;

namespace CharactorLib.Format;

public class _2bpp16 : FormatBase
{
	public _2bpp16()
	{
		base.FormatText = "[16][16]";
		base.Name = "2BPP 16x16 PCE";
		base.Extension = "";
		base.Author = ">>96";
		base.Url = "http://www45.atwiki.jp/yychr/";
		base.Readonly = false;
		base.IsCompressed = false;
		base.EnableAdf = true;
		base.IsSupportMirror = true;
		base.IsSupportRotate = true;
		base.ColorBit = 2;
		base.ColorNum = 4;
		base.CharWidth = 16;
		base.CharHeight = 16;
		base.Width = 128;
		base.Height = 128;
	}

	public override void ConvertMemToChr(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		for (int i = 0; i < base.CharHeight; i++)
		{
			int num = addr + i * 2;
			int num2 = num + 32;
			byte b = data[num];
			byte b2 = data[num2];
			byte b3 = data[num + 1];
			byte b4 = data[num2 + 1];
			int pointAddress = bytemap.GetPointAddress(px, py + i);
			for (int j = 0; j < 8; j++)
			{
				byte b5 = (byte)((uint)(b >> 7 - j) & 1u);
				byte b6 = (byte)(((byte)((b2 >> 7 - j) & 1) << 1) | b5);
				bytemap.Data[pointAddress++] = b6;
			}
			for (int k = 0; k < 8; k++)
			{
				byte b7 = (byte)((uint)(b3 >> 7 - k) & 1u);
				byte b8 = (byte)(((byte)((b4 >> 7 - k) & 1) << 1) | b7);
				bytemap.Data[pointAddress++] = b8;
			}
		}
	}

	public override void ConvertChrToMem(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		for (int i = 0; i < base.CharHeight; i++)
		{
			int num = addr + i * 2;
			int num2 = num + 32;
			int pointAddress = bytemap.GetPointAddress(px, py + i);
			byte b = 0;
			byte b2 = 0;
			for (int j = 0; j < 8; j++)
			{
				byte b3 = (byte)(bytemap.Data[pointAddress++] & 3u);
				b = (byte)(b | (byte)((b3 & 1) << 7 - j));
				b2 = (byte)(b2 | (byte)(((b3 >> 1) & 1) << 7 - j));
			}
			data[num] = b;
			data[num2] = b2;
			b = 0;
			b2 = 0;
			for (int k = 0; k < 8; k++)
			{
				byte b4 = (byte)(bytemap.Data[pointAddress++] & 3u);
				b = (byte)(b | (byte)((b4 & 1) << 7 - k));
				b2 = (byte)(b2 | (byte)(((b4 >> 1) & 1) << 7 - k));
			}
			data[num + 1] = b;
			data[num2 + 1] = b2;
		}
	}
}
