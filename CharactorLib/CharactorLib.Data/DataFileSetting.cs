using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CharactorLib.Data;

public class DataFileSetting : SettingBase
{
	public List<TrapSetting> TrapList = new List<TrapSetting>();

	public int HeaderSizeDefMaker { get; set; }

	public int HeaderSizeDefUser { get; set; }

	public bool DetectHeaderSize { get; set; } = true;


	public DataFileSetting()
	{
		base.DataName = "Setting File";
		base.Extension = "setting";
		base.IsAutoLoad = true;
		base.IsAutoSave = false;
		base.InitializeIfNotFound = true;
		base.FixedByteSize = 0;
		base.Author = "-";
		base.Url = "-";
		base.Name = GetType().Name;
	}

	public override void Initialize(string filename)
	{
		base.Initialize(filename);
		HeaderSizeDefMaker = 0;
		HeaderSizeDefUser = 0;
		TrapList.Clear();
	}

	public override void LoadFromFile(string filename)
	{
		base.LoadFromTextFile(filename);
		mFileName = filename;
		try
		{
			string keyValue = GetKeyValue("HEAD_SIZE_DEF_MAKER");
			if (string.IsNullOrWhiteSpace(keyValue))
			{
				HeaderSizeDefMaker = base.DataFileManager.RomHeaderSize;
			}
			else
			{
				HeaderSizeDefMaker = Convert.ToInt32(keyValue, 16);
			}
			string keyValue2 = GetKeyValue("HEAD_SIZE_DEF_USER");
			if (string.IsNullOrWhiteSpace(keyValue2) || DetectHeaderSize)
			{
				HeaderSizeDefUser = base.DataFileManager.RomHeaderSize;
			}
			else
			{
				HeaderSizeDefUser = Convert.ToInt32(keyValue2, 16);
			}
		}
		catch
		{
			HeaderSizeDefMaker = 0;
			HeaderSizeDefUser = 0;
		}
		if (DetectHeaderSize)
		{
			HeaderSizeDefUser = base.DataFileManager.RomHeaderSize;
		}
		TrapList.Clear();
		foreach (SettingKeyValue mKeyValue in mKeyValueList)
		{
			if (mKeyValue.Key.StartsWith(";") || mKeyValue.Key.StartsWith("#") || string.IsNullOrEmpty(mKeyValue.Value))
			{
				continue;
			}
			string key = mKeyValue.Key;
			string text = mKeyValue.Value;
			if (text.Length > 10)
			{
				string pattern = "\\([0-9A-Fa-f]*,";
				Match match = Regex.Match(text, pattern);
				if (match != null)
				{
					string value = match.Value;
					int length = value.Length - 2;
					int address = Convert.ToInt32(value.Substring(1, length), 16);
					string replacement = "(" + GetAddressProcessedHeaderSize(address).ToString("X8") + ",";
					text = Regex.Replace(text, pattern, replacement);
				}
			}
			if (key.StartsWith("@"))
			{
				try
				{
					int address2 = Convert.ToInt32(key.Trim('@'), 16);
					address2 = GetAddressProcessedHeaderSize(address2);
					TrapSetting item = new TrapSetting(address2, text);
					TrapList.Add(item);
				}
				catch
				{
				}
			}
			else
			{
				base.DataFileManager.LoadDataFromBinary("", base.DataFileManager.RomData.Data, text);
			}
		}
	}

	public override void SaveToFile(string filename)
	{
	}

	public int GetAddressProcessedHeaderSize(int address)
	{
		int num = address - (HeaderSizeDefMaker - HeaderSizeDefUser);
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}

	public void ProcessTrap(int address)
	{
		foreach (TrapSetting trap in TrapList)
		{
			if (address == trap.Address)
			{
				base.DataFileManager.LoadDataFromBinary("", base.DataFileManager.RomData.Data, trap.Command);
			}
		}
	}
}
