using System.Drawing;
using CharactorLib.Common;

namespace CharactorLib.Format;

public class _2bppNES : FormatBase
{
	public _2bppNES()
	{
		base.FormatText = "[8][8]";
		base.Name = "2BPP NES";
		base.Extension = "nes,fds,chr,bkg,spr";
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

	public override int GetDataAddress(byte[] data)
	{
		int num = 0;
		if (data != null && data.Length > 16)
		{
			try
			{
				NesHeader nesHeader = new NesHeader(data);
				if (!nesHeader.IsNesHeader)
				{
					return num;
				}
				byte prgRom = nesHeader.PrgRom;
				num = ((nesHeader.ChrRom > 0) ? (prgRom * 16384) : 0);
				if (nesHeader.IsTrainer)
				{
					num += 512;
				}
				num += 16;
				return num;
			}
			catch
			{
				return num;
			}
		}
		return num;
	}

	public override void ConvertMemToChr(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		for (int i = 0; i < base.CharHeight; i++)
		{
			int num = i + addr;
			int num2 = num + 8;
			byte b = data[num];
			byte b2 = data[num2];
			for (int j = 0; j < base.CharWidth; j++)
			{
				byte b3 = (byte)((uint)(b >> 7 - j) & 1u);
				byte b4 = (byte)(((byte)((b2 >> 7 - j) & 1) << 1) | b3);
				Point advancePixelPoint = base.GetAdvancePixelPoint(px + j, py + i);
				int pointAddress = bytemap.GetPointAddress(advancePixelPoint.X, advancePixelPoint.Y);
				bytemap.Data[pointAddress] = b4;
			}
		}
	}

	public override void ConvertChrToMem(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
		for (int i = 0; i < base.CharHeight; i++)
		{
			int num = i + addr;
			int num2 = num + 8;
			byte b = 0;
			byte b2 = 0;
			for (int j = 0; j < base.CharWidth; j++)
			{
				Point advancePixelPoint = base.GetAdvancePixelPoint(px + j, py + i);
				int pointAddress = bytemap.GetPointAddress(advancePixelPoint.X, advancePixelPoint.Y);
				byte num3 = bytemap.Data[pointAddress];
				int num4 = num3 & 1;
				int num5 = (num3 >> 1) & 1;
				b = (byte)(b | (byte)(num4 << 7 - j));
				b2 = (byte)(b2 | (byte)(num5 << 7 - j));
			}
			data[num] = b;
			data[num2] = b2;
		}
	}
}
