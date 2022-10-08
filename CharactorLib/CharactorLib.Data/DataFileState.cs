using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CharactorLib.Data;

public class DataFileState : SettingBase
{
	public string SupportedExt = "st[0-9a]|sv[0-9x]";

	public DataFileState()
	{
		base.DataName = "Data File";
		base.Extension = "*";
		base.IsAutoLoad = false;
		base.IsAutoSave = false;
		base.FixedByteSize = 0;
		base.Author = "-";
		base.Url = "-";
		base.Name = GetType().Name;
		try
		{
			string path = Environment.GetCommandLineArgs()[0];
			string text = Path.ChangeExtension(path, "StateSetting");
			if (!File.Exists(text))
			{
				text = text.Replace(".vshost", "");
			}
			if (!File.Exists(text))
			{
				text = Path.GetDirectoryName(path) + "\\Resources\\YYCHR.StateSetting";
			}
			base.LoadFromTextFile(text);
			foreach (SettingKeyValue mKeyValue in mKeyValueList)
			{
				if (mKeyValue.Valid)
				{
					string key = mKeyValue.Key;
					if (!string.IsNullOrWhiteSpace(key) && !key.StartsWith(";") && !key.StartsWith("#") && !CheckExtSupported(key))
					{
						SupportedExt = SupportedExt + "|" + key;
					}
				}
			}
		}
		catch
		{
		}
	}

	public override bool CheckExtSupported(string ext)
	{
		bool result = false;
		if (!string.IsNullOrWhiteSpace(ext) && !string.IsNullOrWhiteSpace(SupportedExt))
		{
			result = CheckExt(ext, SupportedExt);
		}
		return result;
	}

	public override void LoadFromFile(string filename)
	{
		base.LoadFromFile(filename);
		byte[] data = base.Data;
		if (!LoadState(filename, data))
		{
			string extension = Path.GetExtension(filename);
			LoadDataFromSetting(extension, data);
		}
	}

	public void LoadDataFromSetting(string ext, byte[] data)
	{
		foreach (SettingKeyValue mKeyValue in mKeyValueList)
		{
			if (!mKeyValue.Key.StartsWith(";") && !mKeyValue.Key.StartsWith("#") && !string.IsNullOrEmpty(mKeyValue.Value))
			{
				string key = mKeyValue.Key;
				if (Regex.IsMatch(ext, key, RegexOptions.IgnoreCase))
				{
					base.DataFileManager.LoadDataFromBinary(ext, data, mKeyValue.Value);
				}
			}
		}
	}

	private bool LoadState(string filename, byte[] data)
	{
		bool result = false;
		try
		{
			string ext = Path.GetExtension(filename).Trim('.');
			if (CheckExt(ext, "st[0-9a]"))
			{
				LoadVirtuaNesState(data);
				result = true;
			}
			if (CheckExt(ext, "sv[0-9x]"))
			{
				LoadStateSnesGt(data);
				result = true;
				return result;
			}
			return result;
		}
		catch
		{
			return result;
		}
	}

	private bool CheckExt(string ext, string extPattern)
	{
		return new Regex(extPattern).IsMatch(ext);
	}

	private void LoadStateSnesGt(byte[] data)
	{
		int palAddrSnesGT = GetPalAddrSnesGT(data);
		int num = 512;
		if (palAddrSnesGT + num > data.Length)
		{
			num = data.Length - palAddrSnesGT;
		}
		if (num < 0)
		{
			num = 0;
		}
		base.DataFileManager.PalInfo256.LoadPal16FromMem(data, palAddrSnesGT, 0, num);
	}

	private int GetPalAddrSnesGT(byte[] data)
	{
		int result = -1;
		if (!ByteStringCompair(data, 0, "GTSF"))
		{
			return result;
		}
		for (int i = 0; i < data.Length - 4; i++)
		{
			if (data[i] == 80 && data[i + 1] == 65 && data[i + 2] == 76 && data[i + 3] == 32)
			{
				result = i + 8;
				break;
			}
		}
		return result;
	}

	private void LoadVirtuaNesState(byte[] data)
	{
		int datAddrVirtuanes = GetDatAddrVirtuanes(data);
		int num = 32;
		if (datAddrVirtuanes + num > data.Length)
		{
			num = data.Length - datAddrVirtuanes;
		}
		if (datAddrVirtuanes < 0)
		{
			num = 0;
		}
		base.DataFileManager.DatInfoNes.LoadFromMem(data, datAddrVirtuanes, 0, num);
	}

	private int GetDatAddrVirtuanes(byte[] data)
	{
		int result = -1;
		if (!ByteStringCompair(data, 0, "VirtuaNES"))
		{
			return result;
		}
		if (data[15] == 1)
		{
			result = 2440;
		}
		else if (data[15] == 2)
		{
			int int32FromByte = GetInt32FromByte(data, 44);
			result = 48 + int32FromByte;
			result += 16;
			result += 2048;
		}
		return result;
	}

	public static bool ByteStringCompair(byte[] data, int dataAddr, string str)
	{
		bool result = true;
		for (int i = dataAddr; i < data.Length && i < str.Length; i++)
		{
			if (data[i] != (byte)str[i])
			{
				result = false;
				break;
			}
		}
		return result;
	}

	private int GetInt32FromByte(byte[] data, int addr)
	{
		byte b = data[addr];
		byte b2 = data[addr + 1];
		byte b3 = data[addr + 2];
		return (data[addr + 3] << 24) | (b3 << 16) | (b2 << 8) | b;
	}
}
