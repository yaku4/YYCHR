using CharactorLib.Common;

namespace CharactorLib.Format;

public class _4bppPCE_SG : FormatBase
{
	public _4bppPCE_SG()
	{
		base.FormatText = "16x*16Y*4L";
		base.Name = "4BPP PCE(SG)";
		base.Extension = "pce";
		base.Author = "Yy";
		base.Url = "";
		base.Readonly = false;
		base.IsCompressed = false;
		base.EnableAdf = false;
		base.IsSupportMirror = false;
		base.IsSupportRotate = false;
		base.ColorBit = 4;
		base.ColorNum = 16;
		base.CharWidth = 16;
		base.CharHeight = 16;
		base.Width = 128;
		base.Height = 128;
	}

	public override void ConvertMemToChr(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		for (int i = 0; i < base.CharHeight; i++)
		{
			int num = i * 2 + addr;
			int num2 = num + 32;
			int num3 = num + 64;
			int num4 = num + 96;
			int pointAddress = bytemap.GetPointAddress(px, py + i);
			byte b = data[num + 1];
			byte b2 = data[num2 + 1];
			byte b3 = data[num3 + 1];
			byte b4 = data[num4 + 1];
			for (int j = 0; j < 8; j++)
			{
				byte b5 = (byte)((uint)(b >> 7 - j) & 1u);
				byte b6 = (byte)((uint)(b2 >> 7 - j) & 1u);
				byte b7 = (byte)((uint)(b3 >> 7 - j) & 1u);
				byte b8 = (byte)(((byte)((b4 >> 7 - j) & 1) << 3) | (b7 << 2) | (b6 << 1) | b5);
				bytemap.Data[pointAddress++] = b8;
			}
			pointAddress = bytemap.GetPointAddress(px + 8, py + i);
			b = data[num];
			b2 = data[num2];
			b3 = data[num3];
			b4 = data[num4];
			for (int k = 0; k < 8; k++)
			{
				byte b9 = (byte)((uint)(b >> 7 - k) & 1u);
				byte b10 = (byte)((uint)(b2 >> 7 - k) & 1u);
				byte b11 = (byte)((uint)(b3 >> 7 - k) & 1u);
				byte b12 = (byte)(((byte)((b4 >> 7 - k) & 1) << 3) | (b11 << 2) | (b10 << 1) | b9);
				bytemap.Data[pointAddress++] = b12;
			}
		}
	}

	public override void ConvertChrToMem(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		for (int i = 0; i < base.CharHeight; i++)
		{
			int num = i * 2 + addr;
			int num2 = num + 32;
			int num3 = num + 64;
			int num4 = num + 96;
			int pointAddress = bytemap.GetPointAddress(px, py + i);
			byte b = 0;
			byte b2 = 0;
			byte b3 = 0;
			byte b4 = 0;
			for (int j = 0; j < 8; j++)
			{
				byte b5 = bytemap.Data[pointAddress++];
				b = (byte)(b | (byte)((b5 & 1) << 7 - j));
				b2 = (byte)(b2 | (byte)(((b5 >> 1) & 1) << 7 - j));
				b3 = (byte)(b3 | (byte)(((b5 >> 2) & 1) << 7 - j));
				b4 = (byte)(b4 | (byte)(((b5 >> 3) & 1) << 7 - j));
			}
			data[num + 1] = b;
			data[num2 + 1] = b2;
			data[num3 + 1] = b3;
			data[num4 + 1] = b4;
			pointAddress = bytemap.GetPointAddress(px + 8, py + i);
			b = 0;
			b2 = 0;
			b3 = 0;
			b4 = 0;
			for (int k = 0; k < 8; k++)
			{
				byte b6 = bytemap.Data[pointAddress++];
				b = (byte)(b | (byte)((b6 & 1) << 7 - k));
				b2 = (byte)(b2 | (byte)(((b6 >> 1) & 1) << 7 - k));
				b3 = (byte)(b3 | (byte)(((b6 >> 2) & 1) << 7 - k));
				b4 = (byte)(b4 | (byte)(((b6 >> 3) & 1) << 7 - k));
			}
			data[num] = b;
			data[num2] = b2;
			data[num3] = b3;
			data[num4] = b4;
		}
	}
}
