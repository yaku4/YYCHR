using System.Drawing;
using CharactorLib.Common;

namespace CharactorLib.Format;

public class _2bppVB : FormatBase
{
	public _2bppVB()
	{
		base.FormatText = "[8*2]";
		base.Name = "2BPP VB";
		base.Extension = "vb";
		base.Author = "Yy";
		base.Url = "";
		base.Readonly = false;
		base.IsCompressed = false;
		base.EnableAdf = true;
		base.IsSupportMirror = true;
		base.IsSupportRotate = true;
		base.ColorBit = 2;
		base.ColorNum = 4;
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
			byte b = data[num];
			byte b2 = data[num + 1];
			for (int j = 0; j < 4; j++)
			{
				byte b3 = (byte)((uint)(b >> j * 2) & 3u);
				Point advancePixelPoint = base.GetAdvancePixelPoint(px + j, py + i);
				int pointAddress = bytemap.GetPointAddress(advancePixelPoint.X, advancePixelPoint.Y);
				bytemap.Data[pointAddress] = b3;
			}
			for (int k = 0; k < 4; k++)
			{
				byte b4 = (byte)((uint)(b2 >> k * 2) & 3u);
				Point advancePixelPoint2 = base.GetAdvancePixelPoint(px + k + 4, py + i);
				int pointAddress2 = bytemap.GetPointAddress(advancePixelPoint2.X, advancePixelPoint2.Y);
				bytemap.Data[pointAddress2] = b4;
			}
		}
	}

	public override void ConvertChrToMem(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		for (int i = 0; i < base.CharHeight; i++)
		{
			int num = i * 2 + addr;
			byte b = 0;
			for (int j = 0; j < 4; j++)
			{
				Point advancePixelPoint = base.GetAdvancePixelPoint(px + j, py + i);
				int pointAddress = bytemap.GetPointAddress(advancePixelPoint.X, advancePixelPoint.Y);
				byte b2 = (byte)(bytemap.Data[pointAddress] & 3u);
				b = (byte)(b | (byte)(b2 << j * 2));
			}
			data[num] = b;
			byte b3 = 0;
			for (int k = 0; k < 4; k++)
			{
				Point advancePixelPoint2 = base.GetAdvancePixelPoint(px + k + 4, py + i);
				int pointAddress2 = bytemap.GetPointAddress(advancePixelPoint2.X, advancePixelPoint2.Y);
				byte b4 = (byte)(bytemap.Data[pointAddress2] & 3u);
				b3 = (byte)(b3 | (byte)(b4 << k * 2));
			}
			data[num + 1] = b3;
		}
	}
}
