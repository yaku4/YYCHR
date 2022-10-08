using System;
using System.Collections.Generic;

namespace CharactorLib.Data;

public class JumpListFile : TextFileBase
{
	private List<JumpListInfo> mJumpList = new List<JumpListInfo>();

	public int LastSelectedIndex = -1;

	public List<JumpListInfo> JumpList => mJumpList;

	public int HeaderSizeDefMaker { get; set; }

	public int HeaderSizeDefUser { get; set; }

	public bool DetectHeaderSize { get; set; } = true;


	public JumpListFile()
	{
		base.DataName = "Jump List File";
		base.Extension = "jumplist";
		base.IsAutoLoad = true;
		base.IsAutoSave = false;
		base.FixedByteSize = 0;
		base.Author = "-";
		base.Url = "-";
		base.Name = GetType().Name;
		base.InitializeIfNotFound = true;
	}

	public override void CreateNew(int size)
	{
		mJumpList.Clear();
		base.Modified = false;
		HeaderSizeDefMaker = 0;
		HeaderSizeDefUser = 0;
	}

	public override void LoadFromFile(string filename)
	{
		base.LoadFromTextFile(filename);
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
			HeaderSizeDefMaker = base.DataFileManager.RomHeaderSize;
			HeaderSizeDefUser = base.DataFileManager.RomHeaderSize;
		}
		try
		{
			mJumpList.Clear();
			string[] array = mTextLines;
			foreach (string text in array)
			{
				if (string.IsNullOrWhiteSpace(text) || text.StartsWith(";") || text.StartsWith("#"))
				{
					continue;
				}
				string line = text;
				if (text.Contains("HEAD_SIZE_") && text.Contains("="))
				{
					continue;
				}
				try
				{
					string[] array2 = text.Split(',');
					if (array2.Length >= 2)
					{
						int address = Convert.ToInt32(array2[1], 16);
						string text2 = (array2[1] = GetAddressProcessedHeaderSize(address).ToString("X8"));
						line = string.Join(",", array2);
					}
				}
				catch
				{
					line = text;
				}
				JumpListInfo item = new JumpListInfo(line);
				mJumpList.Add(item);
			}
		}
		catch
		{
		}
	}

	public override void SaveToFile(string filename)
	{
		List<string> list = new List<string>();
		List<JumpListInfo> list2 = new List<JumpListInfo>(JumpList.ToArray());
		try
		{
			for (int i = 0; i < mTextLines.Length; i++)
			{
				string text = mTextLines[i];
				if (string.IsNullOrWhiteSpace(text) || text.StartsWith(";") || text.StartsWith("#") || text.Contains("="))
				{
					list.Add(text);
					continue;
				}
				string[] array = text.Split(',');
				string text2 = array[0];
				string value = array[1];
				int num = 0;
				try
				{
					num = Convert.ToInt32(value, 16);
				}
				catch
				{
				}
				if (array.Length < 2 || string.IsNullOrWhiteSpace(text2) || string.IsNullOrWhiteSpace(value) || num < 0)
				{
					continue;
				}
				JumpListInfo jumpListInfo = null;
				if (jumpListInfo == null)
				{
					foreach (JumpListInfo item2 in list2)
					{
						if (text2 == item2.Name)
						{
							jumpListInfo = item2;
							break;
						}
					}
				}
				if (jumpListInfo == null)
				{
					foreach (JumpListInfo item3 in list2)
					{
						int addressProcessedHeaderSizeReverse = GetAddressProcessedHeaderSizeReverse(item3.Address);
						if (num == addressProcessedHeaderSizeReverse)
						{
							jumpListInfo = item3;
							break;
						}
					}
				}
				if (jumpListInfo != null)
				{
					int addressProcessedHeaderSizeReverse2 = GetAddressProcessedHeaderSizeReverse(jumpListInfo.Address);
					text = jumpListInfo.Name + "," + addressProcessedHeaderSizeReverse2.ToString("X8") + "," + jumpListInfo.Format.ToString("X") + "," + jumpListInfo.Pattern.ToString("X");
					list.Add(text);
					list2.Remove(jumpListInfo);
				}
			}
			foreach (JumpListInfo item4 in list2)
			{
				int addressProcessedHeaderSizeReverse3 = GetAddressProcessedHeaderSizeReverse(item4.Address);
				string item = item4.Name + "," + addressProcessedHeaderSizeReverse3.ToString("X8") + "," + item4.Format.ToString("X") + "," + item4.Pattern.ToString("X");
				list.Add(item);
			}
		}
		catch
		{
		}
		if (string.IsNullOrWhiteSpace(GetKeyValue("HEAD_SIZE_DEF_USER")))
		{
			HeaderSizeDefUser = base.DataFileManager.RomHeaderSize;
			list.Insert(0, "HEAD_SIZE_DEF_USER=" + HeaderSizeDefUser.ToString("X6"));
		}
		else
		{
			string value2 = HeaderSizeDefUser.ToString("X6");
			SetKeyValue("HEAD_SIZE_DEF_USER", value2);
		}
		if (string.IsNullOrWhiteSpace(GetKeyValue("HEAD_SIZE_DEF_MAKER")))
		{
			HeaderSizeDefMaker = base.DataFileManager.RomHeaderSize;
			list.Insert(0, "HEAD_SIZE_DEF_MAKER=" + HeaderSizeDefMaker.ToString("X6"));
		}
		else
		{
			string value3 = HeaderSizeDefMaker.ToString("X6");
			SetKeyValue("HEAD_SIZE_DEF_MAKER", value3);
		}
		mTextLines = list.ToArray();
		base.SaveToTextFile(filename);
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

	private int GetAddressProcessedHeaderSizeReverse(int address)
	{
		int num = address + (HeaderSizeDefMaker - HeaderSizeDefUser);
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}

	public void SelectIndexFromInfo(JumpListInfo info)
	{
		for (int i = 0; i < JumpList.Count; i++)
		{
			if (info == JumpList[i])
			{
				LastSelectedIndex = i;
				break;
			}
		}
	}

	public JumpListInfo GetNext()
	{
		int num = LastSelectedIndex + 1;
		if (num >= JumpList.Count)
		{
			num = JumpList.Count - 1;
		}
		if (num < 0)
		{
			num = 0;
		}
		return GetIndex(num);
	}

	public JumpListInfo GetPrevious()
	{
		int num = LastSelectedIndex - 1;
		if (num >= JumpList.Count)
		{
			num = JumpList.Count - 1;
		}
		if (num < 0)
		{
			num = 0;
		}
		return GetIndex(num);
	}

	public JumpListInfo GetIndex(int newIndex)
	{
		if (newIndex > JumpList.Count)
		{
			newIndex = JumpList.Count - 1;
		}
		if (newIndex < 0)
		{
			newIndex = 0;
		}
		LastSelectedIndex = newIndex;
		JumpListInfo result = null;
		if (newIndex >= 0 && newIndex < JumpList.Count)
		{
			result = JumpList[newIndex];
		}
		return result;
	}
}
