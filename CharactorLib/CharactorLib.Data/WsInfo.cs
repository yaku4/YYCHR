using System.Collections.Generic;
using System.Drawing;
using System.IO;
using CharactorLib.Format;

namespace CharactorLib.Data;

public class WsInfo
{
	private string mName = "New Workspace";

	private List<PatternInfo> mPatternList = new List<PatternInfo>();

	private List<FormatBase> mFormatList = new List<FormatBase>();

	private bool mModified;

	public string Name
	{
		get
		{
			return mName;
		}
		set
		{
			mName = value;
		}
	}

	public List<PatternInfo> PatternList => mPatternList;

	public List<FormatBase> FormatList => mFormatList;

	public bool Modified
	{
		get
		{
			return mModified;
		}
		set
		{
			mModified = value;
		}
	}

	public void AddPattern(PatternInfo pattern, FormatBase format)
	{
		if (!FormatList.Contains(format))
		{
			FormatList.Add(format);
		}
		pattern.Format = FormatList.IndexOf(format);
		PatternList.Add(pattern);
		Modified = true;
	}

	public void RemoveSelectedPattern()
	{
		for (int num = PatternList.Count - 1; num >= 0; num--)
		{
			if (PatternList[num].Selected)
			{
				PatternList.RemoveAt(num);
			}
		}
		Modified = true;
	}

	public void MoveSelectedPattern(int moveX, int moveY)
	{
		for (int i = 0; i < PatternList.Count; i++)
		{
			if (PatternList[i].Selected)
			{
				PatternList[i].X += moveX;
				PatternList[i].Y += moveY;
			}
		}
		Modified = true;
	}

	public void SelectAll()
	{
		foreach (PatternInfo pattern in PatternList)
		{
			pattern.Selected = true;
		}
	}

	public void UnSelectAll()
	{
		foreach (PatternInfo pattern in PatternList)
		{
			pattern.Selected = false;
		}
	}

	public PatternInfo GetPatternByPosition(int mX, int mY)
	{
		PatternInfo result = null;
		for (int num = PatternList.Count - 1; num >= 0; num--)
		{
			PatternInfo patternInfo = PatternList[num];
			if (patternInfo.Format < FormatList.Count)
			{
				FormatBase formatBase = FormatList[patternInfo.Format];
				if (formatBase != null && new Rectangle(patternInfo.X, patternInfo.Y, formatBase.CharWidth, formatBase.CharHeight).Contains(mX, mY))
				{
					result = patternInfo;
					break;
				}
			}
		}
		return result;
	}

	public PatternInfo SelectPatternByPosition(int mX, int mY)
	{
		PatternInfo patternByPosition = GetPatternByPosition(mX, mY);
		if (patternByPosition != null)
		{
			patternByPosition.Selected = !patternByPosition.Selected;
		}
		return patternByPosition;
	}

	public PatternInfo[] GetSelectedPattern()
	{
		List<PatternInfo> list = new List<PatternInfo>();
		foreach (PatternInfo pattern in PatternList)
		{
			if (pattern.Selected)
			{
				list.Add(pattern);
			}
		}
		return list.ToArray();
	}

	public static WsInfo FromString(string[] text)
	{
		WsInfo wsInfo = new WsInfo();
		wsInfo.Load(text);
		return wsInfo;
	}

	public override string ToString()
	{
		string empty = string.Empty;
		empty = empty + "#T " + Name + "\n";
		foreach (FormatBase format in FormatList)
		{
			empty = empty + "#F " + format.Name + "\n";
		}
		foreach (PatternInfo pattern in PatternList)
		{
			string text = pattern.ToString();
			empty = empty + text + "\n";
		}
		return empty;
	}

	public void Save(string filename)
	{
		string contents = ToString();
		File.WriteAllText(filename, contents);
	}

	public void Load(string filename)
	{
		string[] text = File.ReadAllLines(filename);
		Load(text);
	}

	public void Load(string[] text)
	{
		string[] array = text;
		foreach (string text2 in array)
		{
			if (text2.StartsWith("#T "))
			{
				Name = text2.Substring(3);
			}
		}
		FormatManager instance = FormatManager.GetInstance();
		FormatList.Clear();
		array = text;
		foreach (string text3 in array)
		{
			if (text3.StartsWith("#F "))
			{
				string name = text3.Substring(3);
				FormatBase format = instance.GetFormat(name);
				if (format != null)
				{
					FormatList.Add(format);
				}
			}
		}
		PatternList.Clear();
		array = text;
		foreach (string text4 in array)
		{
			if (!string.IsNullOrEmpty(text4) && !text4.StartsWith("#"))
			{
				PatternInfo patternInfo = PatternInfo.FromString(text4);
				if (patternInfo != null)
				{
					PatternList.Add(patternInfo);
				}
			}
		}
	}
}
