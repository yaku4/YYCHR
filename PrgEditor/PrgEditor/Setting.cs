using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms.Design;

namespace PrgEditor;

public class Setting
{
	private const string CATEGORY_MISC = null;

	private const string CATEGORY_GRID = "Grid";

	private const string CATEGORY_VIEW = "View";

	private const string CATEGORY_DIALOG = "Dialog";

	private const string CATEGORY_TOOL = "Tool";

	private const string CATEGORY_PATH = "Path";

	private const string CATEGORY_FILE = "File";

	[Category("Path")]
	[Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
	public string PrgPath { get; set; }

	[Category("Path")]
	[Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
	public string ChrPath { get; set; }

	[Category("Path")]
	[Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
	public string StatePath { get; set; }

	[Category("View")]
	public bool GridChrEnable { get; set; }

	[Category("View")]
	public bool GridPrgEnable { get; set; }

	[Category("View")]
	public Color GridColor1 { get; set; }

	[Category("View")]
	public Color GridColor2 { get; set; }

	[Category("View")]
	public Color SelectorColor1 { get; set; }

	[Category("View")]
	public Color SelectorColor2 { get; set; }

	[Category("View")]
	public bool SelectorNegative { get; set; }

	[Category("Dialog")]
	public ConfigAskSetting AutoLoadChr { get; set; }

	[Category("Dialog")]
	public ConfigAskSetting AutoReloadChr { get; set; }

	[Category("Dialog")]
	public ConfigAskSetting AutoReloadPrg { get; set; }

	[Category("View")]
	public bool YxSwap { get; set; }

	[Category("View")]
	public bool X8Y16 { get; set; }

	[Category("View")]
	public bool X16Y16 { get; set; }

	[Category("View")]
	public bool FullPalette { get; set; }

	[Category("View")]
	public bool ImagePalette { get; internal set; } = true;


	public Setting()
	{
		PrgPath = string.Empty;
		ChrPath = string.Empty;
		StatePath = string.Empty;
		GridChrEnable = true;
		GridPrgEnable = true;
		GridColor1 = Color.White;
		GridColor2 = Color.Gray;
		SelectorColor1 = Color.White;
		SelectorColor2 = Color.Aqua;
		SelectorNegative = false;
		AutoLoadChr = ConfigAskSetting.ShowDialog;
		AutoReloadChr = ConfigAskSetting.ShowDialog;
		AutoReloadPrg = ConfigAskSetting.ShowDialog;
	}

	public void CopyFromSetting(Setting setting)
	{
		PrgPath = setting.PrgPath;
		ChrPath = setting.ChrPath;
		StatePath = setting.StatePath;
		GridChrEnable = setting.GridChrEnable;
		GridPrgEnable = setting.GridPrgEnable;
		GridColor1 = setting.GridColor1;
		GridColor2 = setting.GridColor2;
		SelectorColor1 = setting.SelectorColor1;
		SelectorColor2 = setting.SelectorColor2;
		SelectorNegative = setting.SelectorNegative;
		AutoLoadChr = setting.AutoLoadChr;
		AutoReloadChr = setting.AutoReloadChr;
		AutoReloadPrg = setting.AutoReloadPrg;
	}

	public void LoadFromFile(string filename)
	{
		if (File.Exists(filename))
		{
			string[] array = File.ReadAllLines(filename);
			for (int i = 0; i < array.Length; i++)
			{
				GetSetting(array[i]);
			}
		}
	}

	private void GetSetting(string line)
	{
		if (line.Contains("PrgPath"))
		{
			PrgPath = GetSettingString(line, "PrgPath");
		}
		if (line.Contains("ChrPath"))
		{
			ChrPath = GetSettingString(line, "ChrPath");
		}
		if (line.Contains("StatePath"))
		{
			StatePath = GetSettingString(line, "StatePath");
		}
		if (line.Contains("GridChrEnable"))
		{
			GridChrEnable = GetSettingBool(line, "GridChrEnable");
		}
		if (line.Contains("GridPrgEnable"))
		{
			GridPrgEnable = GetSettingBool(line, "GridPrgEnable");
		}
		if (line.Contains("GridColor1"))
		{
			GridColor1 = Color.FromArgb(GetSettingHex(line, "GridColor1"));
		}
		if (line.Contains("GridColor2"))
		{
			GridColor2 = Color.FromArgb(GetSettingHex(line, "GridColor2"));
		}
		if (line.Contains("SelectorColor1"))
		{
			SelectorColor1 = Color.FromArgb(GetSettingHex(line, "SelectorColor1"));
		}
		if (line.Contains("SelectorColor2"))
		{
			SelectorColor2 = Color.FromArgb(GetSettingHex(line, "SelectorColor2"));
		}
		if (line.Contains("SelectorNegative"))
		{
			SelectorNegative = GetSettingBool(line, "SelectorNegative");
		}
		if (line.Contains("AutoLoadChr"))
		{
			AutoLoadChr = (ConfigAskSetting)GetSettingInt(line, "AutoLoadChr");
		}
		if (line.Contains("AutoReloadChr"))
		{
			AutoReloadChr = (ConfigAskSetting)GetSettingInt(line, "AutoReloadChr");
		}
		if (line.Contains("AutoReloadPrg"))
		{
			AutoReloadPrg = (ConfigAskSetting)GetSettingInt(line, "AutoReloadPrg");
		}
	}

	private string GetSettingString(string line, string key)
	{
		string result = null;
		if (line.ToLower().Contains(key.ToLower()))
		{
			string[] array = line.Split('=');
			if (array.Length == 2)
			{
				result = array[1];
			}
		}
		return result;
	}

	private int GetSettingInt(string line, string key)
	{
		int result = 0;
		try
		{
			result = Convert.ToInt32(GetSettingString(line, key));
			return result;
		}
		catch
		{
			return result;
		}
	}

	private int GetSettingHex(string line, string key)
	{
		int result = 0;
		try
		{
			result = Convert.ToInt32(GetSettingString(line, key), 16);
			return result;
		}
		catch
		{
			return result;
		}
	}

	private bool GetSettingBool(string line, string key)
	{
		bool result = false;
		try
		{
			if (GetSettingString(line, key) == "1")
			{
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

	private Size GetSettingSize(string line, string key)
	{
		Size result = new Size(32, 32);
		try
		{
			string[] array = GetSettingString(line, key).Split(',');
			if (array.Length == 2)
			{
				string value = array[0];
				string value2 = array[1];
				int width = Convert.ToInt32(value);
				int height = Convert.ToInt32(value2);
				result = new Size(width, height);
				return result;
			}
			return result;
		}
		catch
		{
			return result;
		}
	}

	public void SaveToFile(string filename)
	{
		List<string> list = new List<string>();
		SetSettingString(list, "PrgPath", PrgPath);
		SetSettingString(list, "ChrPath", ChrPath);
		SetSettingString(list, "StatePath", StatePath);
		SetSettingBool(list, "GridChrEnable", GridChrEnable);
		SetSettingBool(list, "GridPrgEnable", GridPrgEnable);
		SetSettingColor(list, "GridColor1", GridColor1);
		SetSettingColor(list, "GridColor2", GridColor2);
		SetSettingColor(list, "SelectorColor1", SelectorColor1);
		SetSettingColor(list, "SelectorColor2", SelectorColor2);
		SetSettingBool(list, "SelectorNegative", SelectorNegative);
		SetSettingInt(list, "AutoLoadChr", (int)AutoLoadChr);
		SetSettingInt(list, "AutoReloadChr", (int)AutoReloadChr);
		SetSettingInt(list, "AutoReloadPrg", (int)AutoReloadPrg);
		File.WriteAllLines(filename, list.ToArray());
	}

	private void SetSettingSize(List<string> lines, string key, Size value)
	{
		string value2 = value.Width + "," + value.Height;
		SetSettingString(lines, key, value2);
	}

	private void SetSettingColor(List<string> lines, string key, Color color)
	{
		string value = color.ToArgb().ToString("X8");
		SetSettingString(lines, key, value);
	}

	private void SetSettingBool(List<string> lines, string key, bool value)
	{
		int value2 = (value ? 1 : 0);
		SetSettingInt(lines, key, value2);
	}

	private void SetSettingInt(List<string> lines, string key, int value)
	{
		string value2 = value.ToString();
		SetSettingString(lines, key, value2);
	}

	private void SetSettingString(List<string> lines, string key, string value)
	{
		string item = key + "=" + value;
		lines.Add(item);
	}
}
