using System.Collections.Generic;
using System.IO;

namespace CharactorLib.Data;

public class SettingBase : DataFileBase
{
	protected List<SettingKeyValue> mKeyValueList = new List<SettingKeyValue>();

	public string Text
	{
		get
		{
			string text = string.Empty;
			foreach (SettingKeyValue mKeyValue in mKeyValueList)
			{
				string text2 = mKeyValue.ToString();
				if (!string.IsNullOrEmpty(text2))
				{
					text = text + text2 + "\n";
				}
			}
			return text;
		}
		set
		{
			mKeyValueList.Clear();
			string[] array = value.Replace("\r\n", "\n").Split('\n');
			foreach (string line in array)
			{
				AddLine(line);
			}
		}
	}

	public virtual void LoadFromTextFile(string filename)
	{
		if (File.Exists(filename))
		{
			Text = File.ReadAllText(filename);
			mFileName = filename;
			LoadFileDate();
			LoadFileInfo();
			OnDataLoaded();
		}
	}

	public virtual void SaveToTextFile(string filename)
	{
		File.WriteAllText(filename, Text);
	}

	protected string GetKeyValue(string key)
	{
		string result = null;
		foreach (SettingKeyValue mKeyValue in mKeyValueList)
		{
			if (mKeyValue.Key == key)
			{
				return mKeyValue.Value;
			}
		}
		return result;
	}

	protected void AddLine(string line)
	{
		mKeyValueList.Add(new SettingKeyValue(line));
	}

	protected void AddKeyValue(string key, string value)
	{
		mKeyValueList.Add(new SettingKeyValue(key, value));
	}

	protected void UpdateKeyValue(string key, string value)
	{
		bool flag = false;
		foreach (SettingKeyValue mKeyValue in mKeyValueList)
		{
			if (mKeyValue.Key == key)
			{
				mKeyValue.Value = value;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			AddKeyValue(key, value);
		}
	}

	protected void RemoveKey(string key)
	{
		for (int num = mKeyValueList.Count - 1; num >= 0; num--)
		{
			if (mKeyValueList[num].Key == key)
			{
				mKeyValueList.RemoveAt(num);
			}
		}
	}
}
