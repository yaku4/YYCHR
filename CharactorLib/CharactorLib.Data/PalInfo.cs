using System;
using System.Drawing;
using System.IO;

namespace CharactorLib.Data;

public class PalInfo : DataFileBase
{
	private ColorBit[] mColorBits = new ColorBit[256];

	public PaletteType PaletteType { get; set; } = PaletteType.R8G8B8;


	public ColorBit[] ColorBits => mColorBits;

	public Color[] Colors
	{
		get
		{
			Color[] array = new Color[256];
			for (int i = 0; i < array.Length; i++)
			{
				if (i >= 0 && i < mColorBits.Length && mColorBits[i] != null)
				{
					array[i] = mColorBits[i].Color;
				}
				else
				{
					array[i] = Color.Black;
				}
			}
			return array;
		}
	}

	public PalInfo()
	{
		CreateNew(768);
		base.DataName = "Palette File";
		base.Extension = "pal";
		base.IsAutoLoad = true;
		base.IsAutoSave = true;
		base.FixedByteSize = 0;
		base.Author = "-";
		base.Url = "-";
		base.Name = GetType().Name;
	}

	public override void LoadFromFile(string filename)
	{
		base.LoadFromFile(filename);
		byte[] data = base.Data;
		int dataAddr = 0;
		PaletteType paletteType = PaletteType.R8G8B8;
		if (data.Length < 24 || data[0] != 82 || data[1] != 73 || data[2] != 70 || data[3] != 70 || data[8] != 80 || data[9] != 65 || data[10] != 76 || data[11] != 32 || data[12] != 100 || data[13] != 97 || data[14] != 116 || data[15] != 97)
		{
			paletteType = ((data.Length == 1024) ? PaletteType.A8R8G8B8 : ((data.Length == 768 || data.Length == 192) ? PaletteType.R8G8B8 : ((data.Length == 512) ? PaletteType.R5G5B5 : ((data.Length % 3 == 0 && data.Length % 2 != 0) ? PaletteType.R8G8B8 : ((data.Length % 3 != 0 && data.Length % 2 == 0) ? PaletteType.R5G5B5 : ((data.Length % 3 != 0 || data.Length % 2 != 0) ? PaletteType.R8G8B8 : ((data.Length / 3 % 16 == 0 && data.Length / 2 % 16 != 0) ? PaletteType.R8G8B8 : ((data.Length / 3 % 16 == 0 || data.Length / 2 % 16 != 0) ? PaletteType.R8G8B8 : PaletteType.R5G5B5))))))));
		}
		else
		{
			dataAddr = 24;
			paletteType = PaletteType.A8R8G8B8;
		}
		ConvertDataToColor(paletteType, data, dataAddr, 0, 256);
	}

	public override void LoadFromMem(byte[] data, int address, int dest, int byteSize)
	{
		base.LoadFromMem(data, address, dest, byteSize);
		PaletteType palType = PaletteType.R8G8B8;
		int byteSizeFromPaletteType = ColorBit.GetByteSizeFromPaletteType(palType);
		int palReadNum = byteSize / byteSizeFromPaletteType;
		ConvertDataToColor(palType, data, address, dest, palReadNum);
	}

	public void LoadPal32FromMem(byte[] data, int address, int dest, int byteSize)
	{
		PaletteType palType = PaletteType.A8R8G8B8;
		int byteSizeFromPaletteType = ColorBit.GetByteSizeFromPaletteType(palType);
		int palReadNum = byteSize / byteSizeFromPaletteType;
		ConvertDataToColor(palType, data, address, dest, palReadNum);
		mFileName = "NewFile" + base.Extension;
		base.FileDate = DateTime.MinValue;
		OnDataLoaded();
	}

	public void LoadPal24FromMem(byte[] data, int address, int dest, int byteSize)
	{
		PaletteType palType = PaletteType.R8G8B8;
		int byteSizeFromPaletteType = ColorBit.GetByteSizeFromPaletteType(palType);
		int palReadNum = byteSize / byteSizeFromPaletteType;
		ConvertDataToColor(palType, data, address, dest, palReadNum);
		mFileName = "NewFile" + base.Extension;
		base.FileDate = DateTime.MinValue;
		OnDataLoaded();
	}

	public void LoadPal16FromMem(byte[] data, int address, int dest, int byteSize)
	{
		PaletteType palType = PaletteType.R5G5B5;
		int byteSizeFromPaletteType = ColorBit.GetByteSizeFromPaletteType(palType);
		int palReadNum = byteSize / byteSizeFromPaletteType;
		ConvertDataToColor(palType, data, address, dest, palReadNum);
		mFileName = "NewFile" + base.Extension;
		base.FileDate = DateTime.MinValue;
		OnDataLoaded();
	}

	public void LoadPal9FromMem(byte[] data, int address, int dest, int byteSize)
	{
		PaletteType palType = PaletteType.R3G3B3;
		int byteSizeFromPaletteType = ColorBit.GetByteSizeFromPaletteType(palType);
		int palReadNum = byteSize / byteSizeFromPaletteType;
		ConvertDataToColor(palType, data, address, dest, palReadNum);
		mFileName = "NewFile" + base.Extension;
		base.FileDate = DateTime.MinValue;
		OnDataLoaded();
	}

	private void ConvertDataToColor(PaletteType palType, byte[] data, int dataAddr, int palDestIndex, int palReadNum)
	{
		ColorBit[] array = ColorBit.FromDatas(data, dataAddr, palType, palReadNum);
		for (int i = 0; i < array.Length; i++)
		{
			int num = palDestIndex + i;
			if (num >= 0 && num < mColorBits.Length)
			{
				mColorBits[num] = array[i];
			}
		}
		PaletteType = palType;
		OnDataLoaded();
	}

	public override void SaveToFile(string filename)
	{
		ConvertColorToData(base.Data, 0, 256);
		int num = ColorBit.GetByteSizeFromPaletteType(PaletteType) * 256;
		if (num > 2048)
		{
			num = 2048;
		}
		if (num < 0)
		{
			num = 0;
		}
		base.SaveToFile(filename, num);
	}

	public override void SaveToMem(byte[] mem, int address, int dest, int size)
	{
		ConvertColorToData(base.Data, 0, 256);
		base.SaveToMem(mem, address, dest, size);
	}

	private void ConvertColorToData(byte[] data, int addr, int count)
	{
		int num = addr;
		for (int i = 0; i < count; i++)
		{
			if (i < 0 || i >= mColorBits.Length)
			{
				continue;
			}
			byte[] byteData = mColorBits[i].GetByteData();
			if (byteData != null && byteData.Length != 0)
			{
				for (int j = 0; j < byteData.Length; j++)
				{
					data[num] = byteData[j];
					num++;
				}
			}
		}
	}

	public override bool CheckAutoLoad(string filename)
	{
		bool result = true;
		long length = new FileInfo(filename).Length;
		int num = 192;
		int num2 = 4096;
		if (length < num || length > num2)
		{
			result = false;
		}
		else if (length % 3 != 0L && length % 2 != 0L)
		{
			result = false;
		}
		return result;
	}
}
