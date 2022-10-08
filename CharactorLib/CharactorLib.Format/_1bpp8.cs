using System.Drawing;
using CharactorLib.Common;

namespace CharactorLib.Format;

public class _1bpp8 : FormatBase
{
	public _1bpp8()
	{
		base.FormatText = "[8]";
		base.Name = "1BPP 8x8";
		base.Extension = "";
		base.Author = "Yy";
		base.Url = "";
		base.Readonly = false;
		base.IsCompressed = false;
		base.EnableAdf = true;
		base.IsSupportMirror = true;
		base.IsSupportRotate = true;
		base.ColorBit = 1;
		base.ColorNum = 2;
		base.CharWidth = 8;
		base.CharHeight = 8;
		base.Width = 128;
		base.Height = 128;
	}

	public override void ConvertMemToChr(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		for (int i = 0; i < base.CharHeight; i++)
		{
			int num = i + addr;
			byte b = data[num];
			for (int j = 0; j < base.CharWidth; j++)
			{
				byte b2 = (byte)((uint)(b >> 7 - j) & 1u);
				Point advancePixelPoint = base.GetAdvancePixelPoint(px + j, py + i);
				int pointAddress = bytemap.GetPointAddress(advancePixelPoint.X, advancePixelPoint.Y);
				bytemap.Data[pointAddress] = b2;
			}
		}
	}

	public override void ConvertChrToMem(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		for (int i = 0; i < base.CharHeight; i++)
		{
			int num = i + addr;
			byte b = 0;
			for (int j = 0; j < base.CharWidth; j++)
			{
				Point advancePixelPoint = base.GetAdvancePixelPoint(px + j, py + i);
				int pointAddress = bytemap.GetPointAddress(advancePixelPoint.X, advancePixelPoint.Y);
				byte b2 = bytemap.Data[pointAddress];
				b = (byte)(b | (byte)((b2 & 1) << 7 - j));
			}
			data[num] = b;
		}
	}
}
