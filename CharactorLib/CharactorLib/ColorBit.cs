using System;
using System.Collections.Generic;
using System.Drawing;

namespace CharactorLib;

public class ColorBit
{
	public byte BitA { get; set; }

	public byte BitR { get; set; }

	public byte BitG { get; set; }

	public byte BitB { get; set; }

	public PaletteType PalType { get; set; } = PaletteType.R8G8B8;


	public bool EnabledAlpha { get; private set; }

	public bool UseAlpha { get; private set; }

	public bool IsInvalidPalette { get; private set; }

	public bool IsDataLoaded { get; private set; }

	public byte ByteNum { get; private set; } = 3;


	public byte BitNumA { get; private set; }

	public byte BitNumR { get; private set; } = 8;


	public byte BitNumG { get; private set; } = 8;


	public byte BitNumB { get; private set; } = 8;


	public byte A
	{
		get
		{
			return BitToByte(BitA, BitNumA);
		}
		set
		{
			BitA = ByteToBit(value, BitNumA);
		}
	}

	public byte R
	{
		get
		{
			return BitToByte(BitR, BitNumR);
		}
		set
		{
			BitR = ByteToBit(value, BitNumR);
		}
	}

	public byte G
	{
		get
		{
			return BitToByte(BitG, BitNumG);
		}
		set
		{
			BitG = ByteToBit(value, BitNumG);
		}
	}

	public byte B
	{
		get
		{
			return BitToByte(BitB, BitNumB);
		}
		set
		{
			BitB = ByteToBit(value, BitNumB);
		}
	}

	public Color Color
	{
		get
		{
			if (EnabledAlpha && UseAlpha)
			{
				return Color.FromArgb(A, R, G, B);
			}
			return Color.FromArgb(R, G, B);
		}
		set
		{
			A = value.A;
			R = value.R;
			G = value.G;
			B = value.B;
		}
	}

	public static ColorBit FromData(byte[] data, int dataAddr, PaletteType palType)
	{
		ColorBit colorBit = new ColorBit(palType, 0, 0, 0, 0);
		colorBit.LoadData(data, dataAddr, palType);
		return colorBit;
	}

	public static ColorBit[] FromDatas(byte[] data, int dataAddr, PaletteType palType, int palNum)
	{
		byte byteSizeFromPaletteType = GetByteSizeFromPaletteType(palType);
		List<ColorBit> list = new List<ColorBit>();
		for (int i = 0; i < palNum; i++)
		{
			if (dataAddr >= 0 && dataAddr + byteSizeFromPaletteType <= data.Length)
			{
				ColorBit item = FromData(data, dataAddr, palType);
				list.Add(item);
			}
			else
			{
				ColorBit colorBit = new ColorBit(palType, 0, 0, 0, 0);
				colorBit.IsDataLoaded = false;
				colorBit.IsInvalidPalette = true;
				list.Add(colorBit);
			}
			dataAddr += byteSizeFromPaletteType;
		}
		return list.ToArray();
	}

	public static byte[] GetDataFromColorBits(ColorBit[] colorBits)
	{
		if (colorBits == null || colorBits.Length < 1)
		{
			return null;
		}
		PaletteType palType = colorBits[0].PalType;
		int palNum = colorBits.Length;
		return GetDataFromColorBits(colorBits, palType, palNum);
	}

	public static byte[] GetDataFromColorBits(ColorBit[] colorBits, PaletteType palType, int palNum)
	{
		if (colorBits == null || colorBits.Length < 1)
		{
			return null;
		}
		int byteSizeFromPaletteType = GetByteSizeFromPaletteType(palType);
		byte[] array = new byte[palNum * byteSizeFromPaletteType];
		int num = 0;
		for (int i = 0; i < palNum; i++)
		{
			if (i >= 0 && i < colorBits.Length)
			{
				ColorBit colorBit = colorBits[i];
				if (colorBit != null)
				{
					if (colorBit.PalType != palType)
					{
						throw new NotImplementedException("パレットタイプの変換は未実装");
					}
					byte[] byteData = colorBit.GetByteData();
					if (byteData != null)
					{
						for (int j = 0; j < byteData.Length; j++)
						{
							array[num] = byteData[j];
							num++;
						}
					}
					else
					{
						for (int k = 0; k < byteSizeFromPaletteType; k++)
						{
							array[num] = 0;
							num++;
						}
					}
				}
				else
				{
					for (int l = 0; l < byteSizeFromPaletteType; l++)
					{
						array[num] = 0;
						num++;
					}
				}
			}
			else
			{
				for (int m = 0; m < byteSizeFromPaletteType; m++)
				{
					array[num] = 0;
					num++;
				}
			}
		}
		return array;
	}

	public ColorBit(PaletteType palType, byte bitA, byte bitR, byte bitG, byte bitB)
	{
		SetPaletteType(palType);
		BitA = bitA;
		BitR = bitR;
		BitG = bitG;
		BitB = bitB;
	}

	public void CopyAllFrom(ColorBit colorBit)
	{
		SetPaletteType(colorBit.PalType);
		BitA = colorBit.BitA;
		BitR = colorBit.BitR;
		BitG = colorBit.BitG;
		BitB = colorBit.BitB;
	}

	public void CopyColorFrom(ColorBit colorBit)
	{
		if (PalType == colorBit.PalType)
		{
			BitA = colorBit.BitA;
			BitR = colorBit.BitR;
			BitG = colorBit.BitG;
			BitB = colorBit.BitB;
		}
		else
		{
			Color = colorBit.Color;
		}
	}

	private byte ByteToBit(byte byteValue, int bitCount)
	{
		if (bitCount == 0)
		{
			return 0;
		}
		int num = 0;
		int num2 = (1 << bitCount) - 1;
		int num3 = 8 - bitCount;
		int num4 = byteValue >> num3;
		if (num4 <= num)
		{
			num4 = num;
		}
		if (num4 >= num2)
		{
			num4 = num2;
		}
		return (byte)num4;
	}

	private byte BitToByte(byte bitValue, int bitCount)
	{
		if (bitCount == 0)
		{
			return byte.MaxValue;
		}
		int num = 0;
		int num2 = 255;
		int num4;
		if (bitCount < 8)
		{
			int num3 = 8 - bitCount;
			num4 = bitValue << num3;
		}
		else
		{
			num4 = bitValue;
		}
		if (num4 <= num)
		{
			num4 = num;
		}
		if (num4 >= num2)
		{
			num4 = num2;
		}
		return (byte)num4;
	}

	public void SetPaletteType(PaletteType palType)
	{
		PalType = palType;
		switch (palType)
		{
		case PaletteType.IndexedMsx:
		case PaletteType.IndexedNes:
		case PaletteType.Indexed:
			EnabledAlpha = false;
			UseAlpha = false;
			BitNumA = 0;
			BitNumR = 0;
			BitNumG = 0;
			BitNumB = 0;
			break;
		case PaletteType.R3G3B3:
			EnabledAlpha = false;
			UseAlpha = false;
			BitNumA = 0;
			BitNumR = 3;
			BitNumG = 3;
			BitNumB = 3;
			break;
		case PaletteType.R4G4B4:
			EnabledAlpha = false;
			UseAlpha = false;
			BitNumA = 0;
			BitNumR = 4;
			BitNumG = 4;
			BitNumB = 4;
			break;
		case PaletteType.R5G5B5:
			EnabledAlpha = false;
			UseAlpha = false;
			BitNumA = 0;
			BitNumR = 5;
			BitNumG = 5;
			BitNumB = 5;
			break;
		case PaletteType.R5G6B5:
			EnabledAlpha = false;
			UseAlpha = false;
			BitNumA = 0;
			BitNumR = 5;
			BitNumG = 6;
			BitNumB = 5;
			break;
		case PaletteType.R8G8B8:
			EnabledAlpha = false;
			UseAlpha = false;
			BitNumA = 0;
			BitNumR = 8;
			BitNumG = 8;
			BitNumB = 8;
			break;
		case PaletteType.A8R8G8B8:
			EnabledAlpha = true;
			UseAlpha = false;
			BitNumA = 8;
			BitNumR = 8;
			BitNumG = 8;
			BitNumB = 8;
			break;
		default:
			EnabledAlpha = false;
			UseAlpha = false;
			BitNumA = 0;
			BitNumR = 8;
			BitNumG = 8;
			BitNumB = 8;
			break;
		}
		ByteNum = GetByteSizeFromPaletteType(palType);
		IsInvalidPalette = false;
		IsDataLoaded = false;
	}

	public static byte GetByteSizeFromPaletteType(PaletteType palType)
	{
		switch (palType)
		{
		case PaletteType.IndexedMsx:
		case PaletteType.IndexedNes:
		case PaletteType.Indexed:
			return 1;
		case PaletteType.R3G3B3:
			return 2;
		case PaletteType.R4G4B4:
			return 2;
		case PaletteType.R5G5B5:
			return 2;
		case PaletteType.R5G6B5:
			return 2;
		case PaletteType.R8G8B8:
			return 3;
		case PaletteType.A8R8G8B8:
			return 4;
		default:
			return 3;
		}
	}

	private void LoadData(byte[] data, int dataAddr, PaletteType palType)
	{
		byte byteSizeFromPaletteType = GetByteSizeFromPaletteType(palType);
		uint num = 0u;
		if (dataAddr + (byteSizeFromPaletteType - 1) < data.Length)
		{
			if (byteSizeFromPaletteType == 4)
			{
				byte b = data[dataAddr];
				byte b2 = data[dataAddr + 1];
				byte b3 = data[dataAddr + 2];
				num = (uint)((data[dataAddr + 3] << 24) | (b3 << 16) | (b2 << 8) | b);
			}
			if (byteSizeFromPaletteType == 3)
			{
				byte b4 = data[dataAddr];
				byte b5 = data[dataAddr + 1];
				num = (uint)((data[dataAddr + 2] << 16) | (b5 << 8) | b4);
			}
			if (byteSizeFromPaletteType == 2)
			{
				byte b6 = data[dataAddr];
				num = (uint)((data[dataAddr + 1] << 8) | b6);
			}
			if (byteSizeFromPaletteType == 1)
			{
				num = data[dataAddr];
			}
		}
		byte b7 = 0;
		byte b8 = 0;
		byte b9 = 0;
		byte b10 = 0;
		switch (palType)
		{
		case PaletteType.IndexedNes:
		case PaletteType.Indexed:
			b8 = 0;
			b9 = 0;
			b10 = 0;
			b7 = (byte)(num & 0xFFu);
			if ((num & 0xC0u) != 0)
			{
				IsInvalidPalette = true;
			}
			break;
		case PaletteType.IndexedMsx:
			b8 = 0;
			b9 = 0;
			b10 = 0;
			b7 = (byte)(num & 0xFFu);
			if ((num & 0xF0u) != 0)
			{
				IsInvalidPalette = true;
			}
			break;
		case PaletteType.R3G3B3:
			b7 = (byte)((num >> 12) & 0xFu);
			b9 = (byte)((num >> 8) & 7u);
			b8 = (byte)((num >> 4) & 7u);
			b10 = (byte)(num & 7u);
			if ((num & 0xF888u) != 0)
			{
				IsInvalidPalette = true;
			}
			break;
		case PaletteType.R4G4B4:
			b8 = (byte)(num & 0xFu);
			b9 = (byte)((num >> 4) & 0xFu);
			b10 = (byte)((num >> 8) & 0xFu);
			b7 = (byte)((num >> 12) & 0xFu);
			if ((num & 0xF000u) != 0)
			{
				IsInvalidPalette = true;
			}
			break;
		case PaletteType.R5G5B5:
			b8 = (byte)(num & 0x1Fu);
			b9 = (byte)((num >> 5) & 0x1Fu);
			b10 = (byte)((num >> 10) & 0x1Fu);
			b7 = (byte)((num >> 15) & 1u);
			if ((num & 0x8000u) != 0)
			{
				IsInvalidPalette = true;
			}
			break;
		case PaletteType.R5G6B5:
			b8 = (byte)(num & 0x1Fu);
			b9 = (byte)((num >> 5) & 0x3Fu);
			b10 = (byte)((num >> 11) & 0x1Fu);
			b7 = 0;
			break;
		case PaletteType.R8G8B8:
			b8 = (byte)(num & 0xFFu);
			b9 = (byte)((num >> 8) & 0xFFu);
			b10 = (byte)((num >> 16) & 0xFFu);
			b7 = 0;
			break;
		case PaletteType.A8R8G8B8:
			b8 = (byte)(num & 0xFFu);
			b9 = (byte)((num >> 8) & 0xFFu);
			b10 = (byte)((num >> 16) & 0xFFu);
			b7 = (byte)((num >> 24) & 0xFFu);
			break;
		default:
			b8 = (byte)(num & 0xFFu);
			b9 = (byte)((num >> 8) & 0xFFu);
			b10 = (byte)((num >> 16) & 0xFFu);
			b7 = 0;
			break;
		}
		byte b11 = (byte)((1 << (int)BitNumA) - 1);
		byte b12 = (byte)((1 << (int)BitNumR) - 1);
		byte b13 = (byte)((1 << (int)BitNumG) - 1);
		byte b14 = (byte)((1 << (int)BitNumB) - 1);
		BitA = (byte)(b7 & b11);
		BitR = (byte)(b8 & b12);
		BitG = (byte)(b9 & b13);
		BitB = (byte)(b10 & b14);
		IsDataLoaded = true;
	}

	public byte[] GetByteData(PaletteType palType)
	{
		uint num = 0u;
		switch (palType)
		{
		case PaletteType.A8R8G8B8:
			num = (uint)((BitA << 24) | (BitB << 16) | (BitG << 8) | BitR);
			break;
		default:
			num = (uint)((BitB << 16) | (BitG << 8) | BitR);
			break;
		case PaletteType.R5G5B5:
			num = (uint)((BitB << 10) | (BitG << 5) | BitR);
			break;
		case PaletteType.R5G6B5:
			num = (uint)((BitB << 11) | (BitG << 5) | BitR);
			break;
		case PaletteType.R4G4B4:
			num = (uint)((BitB << 8) | (BitG << 4) | BitR);
			break;
		case PaletteType.R3G3B3:
			num = (uint)((BitG << 8) | (BitR << 4) | BitB);
			break;
		case PaletteType.IndexedMsx:
		case PaletteType.IndexedNes:
		case PaletteType.Indexed:
			num = BitA;
			break;
		}
		int byteNum = ByteNum;
		List<byte> list = new List<byte>();
		for (int i = 0; i < byteNum; i++)
		{
			list.Add((byte)(num & 0xFFu));
			num >>= 8;
		}
		return list.ToArray();
	}

	public byte[] GetByteData()
	{
		return GetByteData(PalType);
	}

	public string GetByteDataText(PaletteType palType)
	{
		byte[] byteData = GetByteData(palType);
		string text = "";
		for (int i = 0; i < byteData.Length; i++)
		{
			text = text + byteData[i].ToString("X2") + " ";
		}
		return text.Trim();
	}

	public string GetByteDataText()
	{
		return GetByteDataText(PalType);
	}

	public string ToColorString()
	{
		string text = "";
		if (EnabledAlpha)
		{
			text += A.ToString("X2");
		}
		return text + R.ToString("X2") + G.ToString("X2") + B.ToString("X2");
	}
}
