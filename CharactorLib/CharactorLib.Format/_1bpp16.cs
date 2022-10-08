using CharactorLib.Common;

namespace CharactorLib.Format;

public class _1bpp16 : FormatBase
{
	public _1bpp16()
	{
		base.Name = "1BPP 16x16";
		base.Extension = "";
		base.Author = "";
		base.Url = "";
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
		base.CharHeight = 16;
	}

	public override void ConvertMemToChr(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		for (int i = 0; i < base.CharHeight; i++)
		{
			int num = i * 2 + addr;
			byte b = data[num];
			byte b2 = data[num + 1];
			int pointAddress = bytemap.GetPointAddress(px, py + i);
			for (int j = 0; j < 8; j++)
			{
				byte b3 = (byte)((uint)(b >> 7 - j) & 1u);
				bytemap.Data[pointAddress++] = b3;
			}
			for (int k = 0; k < 8; k++)
			{
				byte b4 = (byte)((uint)(b2 >> 7 - k) & 1u);
				bytemap.Data[pointAddress++] = b4;
			}
		}
	}

	public override void ConvertChrToMem(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		for (int i = 0; i < base.CharHeight; i++)
		{
			int num = i * 2 + addr;
			int pointAddress = bytemap.GetPointAddress(px, py + i);
			byte b = 0;
			for (int j = 0; j < 8; j++)
			{
				byte b2 = (byte)(bytemap.Data[pointAddress++] & 1u);
				b = (byte)(b | (byte)(b2 << 7 - j));
			}
			data[num] = b;
			byte b3 = 0;
			for (int k = 0; k < 8; k++)
			{
				byte b4 = (byte)(bytemap.Data[pointAddress++] & 1u);
				b3 = (byte)(b3 | (byte)(b4 << 7 - k));
			}
			data[num + 1] = b3;
		}
	}
}
