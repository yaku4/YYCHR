using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace PaletteEditor;

public class Settings : ICloneable
{
	public enum ConfigAskSetting
	{
		ShowDialog,
		AutoYes,
		AutoNo
	}

	public enum ConfigSmallFileSaveSize
	{
		ShowDialog,
		AutoCutOff,
		AutoExpand
	}

	public enum ShowAlphaInRgbEditor
	{
		Disabled,
		Enabled,
		IfSupported
	}

	private const string CATEGORY_GRID = "Grid";

	private const string CATEGORY_DIALOG = "Dialog";

	private const string CATEGORY_PATH = "Path";

	private const string CATEGORY_FILE = "File";

	private const string CATEGORY_EDIT = "Edit";

	private const string CATEGORY_VIEW = "View";

	private const string CATEGORY_TOOL = "Tool";

	private const string CATEGORY_CONTROL = "Control";

	private const string CATEGORY_OPTION = "Option";

	private const string CATEGORY_LANGUAGE = "Language";

	private const string CATEGORY_ADDRESS = "Address";

	public static Settings Instance;

	public const int GUI_RATE_MIN = 2;

	public const int GUI_RATE_MAX = 5;

	private int mRate = 2;

	private string IniPath = "PaletteEditor.ini";

	[Browsable(false)]
	[Category("File")]
	[DefaultValue(true)]
	public bool ReadOnlyMode { get; set; } = true;


	[Category("File")]
	[Description("When file opened, this value set to ReadOnlyMode. ")]
	[DefaultValue(true)]
	public bool ReadOnlyModeDefault { get; set; } = true;


	[Category("Option")]
	[Description("Show red X mark on invalid palette. Show red triangle mark on special palette.")]
	[DefaultValue(true)]
	public bool CheckPAL { get; set; } = true;


	[Category("Option")]
	[Description("Show NES palette 0D-3D with red Triangle. This setting only affects to CheckNES setting (and [8bit indexed palette]). ")]
	[DefaultValue(false)]
	public bool EnableNesCheckXD { get; set; }

	[Category("Address")]
	[Description("Show address for file view. ")]
	[DefaultValue(true)]
	public bool AddressPanelVisible { get; set; } = true;


	[Category("Address")]
	[Description("Show detected/found address to this position of file view. For checking found previous data, this default value is set 1/4 of view.")]
	[DefaultValue(64)]
	public int FindAddressPalSelectorIndexPos { get; set; } = 64;


	[Category("Address")]
	[Description("Detect only palette table. When valid palette set is detected 3 times in a row, the palette is detected as table.")]
	[DefaultValue(true)]
	public bool DetectOnlyTable { get; set; } = true;


	[Category("Dialog")]
	[Description("When closing the modified file without saving, a confirmation dialog is displayed to continue. ")]
	[DefaultValue(true)]
	public bool ShowSaveModifiedDialog { get; set; } = true;


	[Category("Dialog")]
	[Description("When the opened file is modified externally, a confirmation dialog is displayed to reload the file. ")]
	[DefaultValue(true)]
	public bool ShowReloadExternalChangeDialog { get; set; } = true;


	[Category("View")]
	[Description("Set GUI size.")]
	[DefaultValue(2)]
	public int GuiSizeRate
	{
		get
		{
			int num = mRate;
			if (num <= 2)
			{
				num = 2;
			}
			if (num >= 5)
			{
				num = 5;
			}
			return num;
		}
		set
		{
			if (value <= 2)
			{
				value = 2;
			}
			if (value >= 5)
			{
				value = 5;
			}
			mRate = value;
		}
	}

	[Category("View")]
	[Description("Show [A] in RgbEditor. ")]
	[DefaultValue(ShowAlphaInRgbEditor.IfSupported)]
	public ShowAlphaInRgbEditor AlphaInRgbEditor { get; set; } = ShowAlphaInRgbEditor.IfSupported;


	public static Settings GetInstance()
	{
		if (Instance == null)
		{
			Instance = new Settings();
		}
		return Instance;
	}

	private Settings()
	{
		InitIniPath();
	}

	public object Clone()
	{
		Settings settings = new Settings();
		CopyProperty(this, settings);
		return settings;
	}

	public void CopyFrom(Settings from)
	{
		CopyProperty(from, this);
	}

	public static void CopyProperty(Settings from, Settings to)
	{
		PropertyInfo[] properties = from.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
		if (properties == null)
		{
			return;
		}
		PropertyInfo[] array = properties;
		foreach (PropertyInfo propertyInfo in array)
		{
			object value = propertyInfo.GetValue(from, null);
			if (value is ICloneable)
			{
				object value2 = ((ICloneable)value).Clone();
				propertyInfo.SetValue(to, value2, null);
			}
			else
			{
				propertyInfo.SetValue(to, value, null);
			}
		}
	}

	private void InitIniPath()
	{
		string text = Path.ChangeExtension(Environment.GetCommandLineArgs()[0], "ini");
		text = (IniPath = text.Replace(".vshost", ""));
	}

	public void Save()
	{
		string text = "";
		PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
		if (properties != null)
		{
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				object obj = propertyInfo.GetValue(this, null);
				if (obj == null)
				{
					obj = "null";
				}
				string text2;
				switch (propertyInfo.PropertyType.Name)
				{
				case "Color":
				{
					Color color = (Color)obj;
					string text3 = "#" + color.A.ToString("X2") + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
					text2 = propertyInfo.Name.ToString() + "=" + text3;
					break;
				}
				case "Point":
					text2 = propertyInfo.Name.ToString() + "=" + ((Point)obj).X + "," + ((Point)obj).Y;
					break;
				case "Size":
					text2 = propertyInfo.Name.ToString() + "=" + ((Size)obj).Width + "," + ((Size)obj).Height;
					break;
				case "Rectangle":
					text2 = propertyInfo.Name.ToString() + "=" + ((Rectangle)obj).X + "," + ((Rectangle)obj).Y + "," + ((Rectangle)obj).Width + "," + ((Rectangle)obj).Height;
					break;
				case "Font":
				{
					Font font = (Font)obj;
					text2 = propertyInfo.Name.ToString() + "=" + font.Name.ToString() + "," + font.Size + "," + (int)font.Unit + "," + font.GdiCharSet + "," + font.GdiVerticalFont;
					break;
				}
				default:
					text2 = ((!propertyInfo.PropertyType.IsEnum) ? (propertyInfo.Name.ToString() + "=" + obj.ToString()) : (propertyInfo.Name.ToString() + "=" + (int)obj));
					break;
				}
				text = text + text2 + "\r\n";
			}
		}
		File.WriteAllText(IniPath, text);
	}

	public void Load()
	{
		try
		{
			if (!File.Exists(IniPath))
			{
				return;
			}
			string[] array = File.ReadAllLines(IniPath);
			foreach (string text in array)
			{
				if (text.StartsWith(";") || text.StartsWith("#"))
				{
					continue;
				}
				string[] array2 = text.Split('=');
				if (array2.Length != 2)
				{
					continue;
				}
				string name = array2[0];
				string text2 = array2[1];
				PropertyInfo property = GetType().GetProperty(name);
				if (!(property != null))
				{
					continue;
				}
				switch (property.PropertyType.Name)
				{
				case "String":
					property.SetValue(this, text2, null);
					break;
				case "Boolean":
				{
					bool flag = Convert.ToBoolean(text2);
					property.SetValue(this, flag, null);
					break;
				}
				case "Color":
					if (text2.StartsWith("#"))
					{
						try
						{
							string value3 = text2.Substring(1, 2);
							string value4 = text2.Substring(3, 2);
							string value5 = text2.Substring(5, 2);
							string value6 = text2.Substring(7, 2);
							int alpha = Convert.ToInt32(value3, 16);
							int red = Convert.ToInt32(value4, 16);
							int green = Convert.ToInt32(value5, 16);
							int blue = Convert.ToInt32(value6, 16);
							Color color = Color.FromArgb(alpha, red, green, blue);
							property.SetValue(this, color, null);
						}
						catch
						{
						}
					}
					else
					{
						try
						{
							Color color2 = Color.FromName(text2);
							color2 = Color.FromArgb(color2.A, color2.R, color2.G, color2.B);
							property.SetValue(this, color2, null);
						}
						catch
						{
						}
					}
					break;
				case "Point":
				{
					string[] array6 = text2.Split(',');
					if (array6.Length == 2)
					{
						int x2 = int.Parse(array6[0].Trim());
						int y2 = int.Parse(array6[1].Trim());
						Point point = new Point(x2, y2);
						property.SetValue(this, point, null);
					}
					break;
				}
				case "Size":
				{
					string[] array5 = text2.Split(',');
					if (array5.Length == 2)
					{
						int width2 = int.Parse(array5[0].Trim());
						int height2 = int.Parse(array5[1].Trim());
						Size size = new Size(width2, height2);
						property.SetValue(this, size, null);
					}
					break;
				}
				case "Rectangle":
				{
					string[] array4 = text2.Split(',');
					if (array4.Length == 4)
					{
						int x = int.Parse(array4[0].Trim());
						int y = int.Parse(array4[1].Trim());
						int width = int.Parse(array4[2].Trim());
						int height = int.Parse(array4[3].Trim());
						Rectangle rectangle = new Rectangle(x, y, width, height);
						property.SetValue(this, rectangle, null);
					}
					break;
				}
				case "Font":
					try
					{
						string[] array3 = text2.Split(',');
						if (array3.Length == 5)
						{
							string familyName = array3[0].Trim();
							float emSize = float.Parse(array3[1].Trim());
							int unit = int.Parse(array3[2].Trim());
							int num2 = int.Parse(array3[3].Trim());
							bool gdiVerticalFont = bool.Parse(array3[4].Trim());
							FontStyle style = FontStyle.Regular;
							Font value = new Font(familyName, emSize, style, (GraphicsUnit)unit, (byte)num2, gdiVerticalFont);
							property.SetValue(this, value, null);
						}
					}
					catch
					{
						Font value2 = new Font(SystemFonts.DefaultFont, FontStyle.Regular);
						property.SetValue(this, value2, null);
					}
					break;
				default:
				{
					int num = Convert.ToInt32(text2);
					property.SetValue(this, num, null);
					break;
				}
				}
			}
		}
		catch
		{
		}
	}

	public void SaveAttributeInfo()
	{
		string text = "";
		PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
		if (properties != null)
		{
			PropertyInfo[] array = properties;
			foreach (PropertyInfo obj in array)
			{
				obj.GetValue(this, null);
				string name = obj.Name;
				string text2 = "";
				string text3 = "";
				string text4 = "";
				foreach (CustomAttributeData customAttributesDatum in obj.GetCustomAttributesData())
				{
					string name2 = customAttributesDatum.Constructor.DeclaringType.Name;
					string text5 = customAttributesDatum.ConstructorArguments[0].Value.ToString();
					if (customAttributesDatum.ConstructorArguments.Count >= 2)
					{
						string text6 = customAttributesDatum.ConstructorArguments[0].Value.ToString();
						int num = text6.LastIndexOf('.');
						if (num < 0)
						{
							num = 0;
						}
						string text7 = text6.Substring(num);
						string text8 = customAttributesDatum.ConstructorArguments[1].Value.ToString();
						text5 = text7 + "(" + text8 + ")";
					}
					if (name2 == "CategoryAttribute")
					{
						text2 = text5;
					}
					if (name2 == "DescriptionJaAttribute")
					{
						text3 = text5;
					}
					if (name2 == "DefaultValueAttribute")
					{
						text4 = text5;
					}
				}
				text = text + text2 + "\t" + name + " : " + text3 + " [default:" + text4 + "]\r\n";
			}
		}
		File.WriteAllText(Path.ChangeExtension(IniPath, ".attribute.txt"), text);
	}
}
