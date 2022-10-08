using System.IO;

namespace CharactorLib.Data;

public class TextFileBase : DataFileBase
{
	protected string[] mTextLines = new string[1] { string.Empty };

	public string Text
	{
		get
		{
			string text = string.Empty;
			string[] array = mTextLines;
			foreach (string text2 in array)
			{
				text = text + text2 + "\n";
			}
			return text;
		}
		set
		{
			string text = value.Replace("\r\n", "\n");
			mTextLines = text.Split('\n');
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

	public string GetKeyValue(string key)
	{
		string result = null;
		string[] array = mTextLines;
		foreach (string text in array)
		{
			if (!string.IsNullOrWhiteSpace(text) && !text.StartsWith(";") && !text.StartsWith("#") && text.Contains(key))
			{
				int num = text.IndexOf("=");
				if (num > 1)
				{
					result = text.Substring(num + 1);
				}
				break;
			}
		}
		return result;
	}

	public void SetKeyValue(string key, string value)
	{
		for (int i = 0; i < mTextLines.Length; i++)
		{
			string text = mTextLines[i];
			if (!string.IsNullOrWhiteSpace(text) && !text.StartsWith(";") && !text.StartsWith("#") && text.Contains(key))
			{
				if (text.IndexOf("=") > 1)
				{
					string[] array = text.Split('=');
					mTextLines[i] = array[0] + "=" + value;
				}
				break;
			}
		}
	}
}
