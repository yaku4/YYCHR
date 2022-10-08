using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Windows.Forms.Design;
using ControlLib;

namespace YYCHR;

[TypeConverter(typeof(EnJaTypeConverter))]
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
		NoExpand,
		AutoExpand
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

	public static Settings Instance;

	private const GridStyle cGridStyleDefault = GridStyle.Line;

	private const bool cGridBankVisibleDefault = true;

	private const bool cGridEditVisibleDefault = true;

	public const int GUI_RATE_MIN = 2;

	public const int GUI_RATE_MAX = 5;

	private int mRate = 2;

	private int mPaletteRowNum = 8;

	private string IniPath = "YYCHR.ini";

	[Category("Path")]
	[Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
	[Description("Set initial path for open ROM file dialog.")]
	[DescriptionJa("ROMファイルを開くダイアログの初期ディレクトリを設定する。")]
	[DefaultValue("")]
	public string RomPath { get; set; } = "";


	[Category("Path")]
	[Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
	[Description("Set initial path for open additional file dialog.")]
	[DescriptionJa("追加ファイルを開くダイアログの初期ディレクトリを設定する。")]
	[DefaultValue("")]
	public string ExtPath { get; set; } = "";


	[Category("Path")]
	[Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
	[Description("Set initial path for open save state file dialog.")]
	[DescriptionJa("セーブステートファイルを開くダイアログの初期ディレクトリを設定する。")]
	[DefaultValue("")]
	public string StatePath { get; set; } = "";


	[Category("Path")]
	[Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
	[Description("Set initial path for open bitmap file dialog.")]
	[DescriptionJa("画像ファイルを開くダイアログの初期ディレクトリを設定する。")]
	[DefaultValue("")]
	public string BmpPath { get; set; } = "";


	[Category("Path")]
	[Description("Set initial path for save bitmap file.")]
	[DescriptionJa("BMPファイルのデフォルトの保存先ディレクトリを設定する。")]
	[DefaultValue(SaveDirType.SettingDir)]
	public SaveDirType BmpSaveDirType { get; set; } = SaveDirType.SettingDir;


	[Category("File")]
	[Description("Expand file size when saving a ROM file smaller than a bank.")]
	[DescriptionJa("バンク1画面より小さなサイズのROMファイルを保存するとき、ROMのサイズを拡張する。")]
	[DefaultValue(ConfigSmallFileSaveSize.ShowDialog)]
	public ConfigSmallFileSaveSize SmallFileSaveSize { get; set; }

	[Category("File")]
	[Description("Clear undo buffer when saving file.")]
	[DescriptionJa("ROMファイルを保存するとき、アンドゥバッファをクリアする。")]
	[DefaultValue(true)]
	public bool SaveClearUndoBuffer { get; set; } = true;


	[Category("File")]
	[Description("Set default image format on the open/save file dialog.")]
	[DescriptionJa("画像ファイルを開く/保存するダイアログを表示するときのデフォルトの画像形式を設定する。")]
	[DefaultValue(ImageFileType.Bmp)]
	public ImageFileType DefaultImageType { get; set; }

	[Category("File")]
	[Description("Enable loading palette from opened image file on CHR-ROM tab.")]
	[DescriptionJa("CHR-ROMタブで開いた画像ファイルからパレットを読み込むかを設定する。")]
	[DefaultValue(false)]
	public bool LoadImagePaletteOnChrRomTab { get; set; }

	[Category("File")]
	[Description("Enable palette editor for ReadOnly file. A palette template for game has fixed palette, disable editor for readonly palette file is better.")]
	[DescriptionJa("読み込み専用のパレットファイルでも編集を許可する。許可しない場合は別ファイルに保存して編集します。固定パレットを使うゲームのテンプレートを公開する場合、ファイルを編集禁止にすることで誤操作が減ります。")]
	[DefaultValue(false)]
	public bool EnableEditorForReadOnlyFile { get; set; }

	[Category("File")]
	[Description("Enable rom header size detect for *.setting and *.jumplist. (= ignore the header size value of those files)")]
	[DescriptionJa("テンプレートファイルの*.settingや*.jumplistで、ROMのヘッダサイズ検出を有効にします。それらの設定ファイルに定義されているヘッダサイズを無視します。")]
	[DefaultValue(true)]
	public bool EnableDetectRomHeaderSize { get; set; } = true;


	[Category("File")]
	[Description("Detected rom header size = remainder of [rom size] divided by [this value]")]
	[DescriptionJa("検出するROMのヘッダサイズ＝[ROMサイズ]÷[この設定値]の余り")]
	[DefaultValue(2048)]
	public int DivisionSizeForDetectRomHeaderSize { get; set; } = 2048;


	[Category("Grid")]
	[Description("Select Grid style for bank view and editor view.")]
	[DescriptionJa("バンクビュー(バンク表示)と エディタビュー(編集領域)のグリッドの種類を設定する。")]
	[DefaultValue(GridStyle.Line)]
	public GridStyle GridStyle { get; set; } = GridStyle.Line;


	[Category("Grid")]
	[Description("Set grid visible for bank view.")]
	[DescriptionJa("バンクビュー(バンク表示)の グリッドを表示するか設定する。")]
	[DefaultValue(true)]
	public bool GridBankVisible { get; set; } = true;


	[Category("Grid")]
	[Description("Set grid visible for editor view.")]
	[DescriptionJa("エディタビュー(編集領域)の グリッドを表示するか設定する。")]
	[DefaultValue(true)]
	public bool GridEditVisible { get; set; } = true;


	[Category("Grid")]
	[Description("Set grid1 color of bank view.")]
	[DescriptionJa("バンクビュー(バンク表示)の グリッド1の色を設定する。")]
	[DefaultValue(typeof(Color), "0x80, 0xFF, 0xFF, 0xFF")]
	public Color GridBankColor1 { get; set; } = Color.FromArgb(128, 255, 255, 255);


	[Category("Grid")]
	[Description("Set grid2 color of bank view.")]
	[DescriptionJa("バンクビュー(バンク表示)の グリッド2の色を設定する。")]
	[DefaultValue(typeof(Color), "0x20, 0xFF, 0xFF, 0xFF")]
	public Color GridBankColor2 { get; set; } = Color.FromArgb(32, 255, 255, 255);


	[Category("Grid")]
	[Description("Set grid1 color of editor view.")]
	[DescriptionJa("エディタビュー(編集領域)の グリッド1の色を設定する。")]
	[DefaultValue(typeof(Color), "0x80, 0xFF, 0xFF, 0xFF")]
	public Color GridEditColor1 { get; set; } = Color.FromArgb(128, 255, 255, 255);


	[Category("Grid")]
	[Description("Set grid2 color of editor view.")]
	[DescriptionJa("エディタビュー(編集領域)の グリッド2の色を設定する。")]
	[DefaultValue(typeof(Color), "0x20, 0xFF, 0xFF, 0xFF")]
	public Color GridEditColor2 { get; set; } = Color.FromArgb(32, 255, 255, 255);


	[Category("Grid")]
	[Description("Set editing rectangle frame color of editor view.")]
	[DescriptionJa("エディタビュー(編集領域)で 範囲指定するペンを使うときの 範囲の枠の色を設定する。")]
	[DefaultValue(typeof(Color), "0xFF, 0x80, 0xFF, 0xFF")]
	public Color EditingRectColor { get; set; } = Color.FromArgb(255, 128, 255, 255);


	[Category("Option")]
	[Description("Save window position.")]
	[DescriptionJa("ウィンドウの位置を保存する。")]
	[DefaultValue(false)]
	public bool OptionSaveWindowPosition { get; set; }

	[Category("Option")]
	[Description("Window position.")]
	[DescriptionJa("ウィンドウの位置。")]
	[DefaultValue(typeof(Point), "0,0")]
	public Point OptionSavedWindowPosition { get; set; } = new Point(0, 0);


	[Category("Option")]
	[Description("Show address in window title.")]
	[DescriptionJa("ウィンドウのタイトルにアドレスを表示する。")]
	[DefaultValue(true)]
	public bool OptionShowTitleAddress { get; set; } = true;


	[Category("Option")]
	[Description("Show all menu. If disabled, hide some menus items.")]
	[DescriptionJa("すべてのメニューを表示する。 無効な場合、ツールバーと重複したり一般的でないメニュー項目は隠される。")]
	[DefaultValue(false)]
	public bool OptionShowAllMenu { get; set; }

	[Category("Option")]
	[Description("Show toolbar as classicc style button. ")]
	[DescriptionJa("ツールバーをクラシックなスタイルのボタンで表示する。 一部のWindowsでは正しく表示できない可能性がある。")]
	[DefaultValue(false)]
	public bool OptionShowToolbarAsButton { get; set; }

	[Category("Option")]
	[Description("Check ctrl/alt/shift keys down on activated main form. If minimized YY-CHR does not back from taskbar, set this value to [false].")]
	[DescriptionJa("YY-CHRがアクティブになったときにCtrl/Alt/Shiftキーをチェックする。YY-CHRがタスクバーから復帰できないときはこの値を[false]にしてください。")]
	[DefaultValue(true)]
	public bool CheckKeyOnActivated { get; set; } = true;


	[Category("Language")]
	[Description("Check installed font. If GUI fonts not found, YY-CHR start up with English mode. Set [true] on Linux or Mac with wine.")]
	[DescriptionJa("インストールされたフォントをチェックする。 GUIフォントが存在しない場合、YY-CHRが英語モードで起動する。 LinuxかMacのwine環境では[true]を設定してください。")]
	[DefaultValue(true)]
	public bool CheckFont { get; set; } = true;


	[Category("Language")]
	[Description("Then YY-CHR startup, set language from [yychr.lng] language file.")]
	[DescriptionJa("YY-CHRを起動するときに、言語を言語ファイル[yychr.lng]から設定します。")]
	[DefaultValue(false)]
	public bool StartupLanguageFromLng { get; set; }

	[Category("Control")]
	[Description("Mouse wheel function on editor.")]
	[DescriptionJa("エディタのマウスホイールの機能。")]
	[DefaultValue(EditorMouseWheelFunction.BankScroll)]
	public EditorMouseWheelFunction EditorMouseWheel { get; set; } = EditorMouseWheelFunction.BankScroll;


	[Category("Control")]
	[Description("Mouse wheel function on editor.")]
	[DescriptionJa("エディタのCtrl+マウスホイールの機能。")]
	[DefaultValue(EditorMouseWheelFunction.PenSelect)]
	public EditorMouseWheelFunction EditorMouseWheelCtrl { get; set; } = EditorMouseWheelFunction.PenSelect;


	[Category("Control")]
	[Description("Mouse wheel function on editor.")]
	[DescriptionJa("エディタのShift+マウスホイールの機能。")]
	[DefaultValue(EditorMouseWheelFunction.PaletteSelect)]
	public EditorMouseWheelFunction EditorMouseWheelShift { get; set; } = EditorMouseWheelFunction.PaletteSelect;


	[Category("Control")]
	[Description("Mouse wheel function on editor.")]
	[DescriptionJa("エディタのAlt+マウスホイールの機能。")]
	[DefaultValue(EditorMouseWheelFunction.None)]
	public EditorMouseWheelFunction EditorMouseWheelAlt { get; set; }

	[Category("Control")]
	[Description("Enable right click menu for bank view.")]
	[DescriptionJa("バンクビュー(バンク表示)で 右クリックメニューを有効にする。")]
	[DefaultValue(false)]
	public bool EnableRightClickMenu { get; set; }

	[Category("Control")]
	[Description("Set scroll rate for bank view.")]
	[DescriptionJa("バンクビュー(バンク表示)のマウスホイールのスクロール量を設定する。")]
	[DefaultValue(4)]
	public int BankWheelScrollRate { get; set; } = 4;


	[Category("Dialog")]
	[Description("When closing the modified file without saving, a confirmation dialog is displayed to continue. ")]
	[DescriptionJa("変更したファイルを保存せずに閉じようとしたときに、処理を継続するかを確認するダイアログを表示する。")]
	[DefaultValue(true)]
	public bool ShowSaveModifiedDialog { get; set; } = true;


	[Category("Dialog")]
	[Description("When the opened file is modified externally, a confirmation dialog is displayed to reload the file. ")]
	[DescriptionJa("開いているファイルをYY-CHR以外で変更したとき、YY-CHRでファイルを再読み込みするか確認するダイアログを表示する。 再読み込みしない場合はYY-CHRで保存したときに外部ツールでの変更が上書きされてしまう。")]
	[DefaultValue(true)]
	public bool ShowReloadExternalChangeDialog { get; set; } = true;


	[Category("Dialog")]
	[Description("When opening a large image file, the truncation is displayed in a dialog. ")]
	[DescriptionJa("CHR-ROMタブで画像ファイルを開いたとき、範囲外を切り捨てたことをダイアログで表示する。")]
	[DefaultValue(true)]
	public bool ShowLoadImageCutoffDialog { get; set; } = true;


	[Category("View")]
	[Description("Set GUI size.")]
	[DescriptionJa("GUIのサイズを設定する。")]
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
	[Browsable(false)]
	[Description("Editor size. [pixel] You can set this value at main window left bottom.")]
	[DescriptionJa("エディタの領域のサイズ。 [ピクセル] メイン画面の左下から設定可能。")]
	[DefaultValue(typeof(Size), "32, 32")]
	public Size EditRectSize { get; set; } = new Size(32, 32);


	[Category("View")]
	[Description("Palette row num when window size is small. The value larger than 8 will expand window size on palette mode changed event. (Valid value: 8 or 16)")]
	[DescriptionJa("ウィンドウサイズが小さい場合のパレットの表示行数。8以上を指定する場合、表示のためにウィンドウサイズを拡大する。(適正値: 8または16)")]
	[DefaultValue(8)]
	public int DefaultPaletteRowNum
	{
		get
		{
			return mPaletteRowNum;
		}
		set
		{
			if (value < 1)
			{
				value = 1;
			}
			if (value > 16)
			{
				value = 16;
			}
			mPaletteRowNum = value;
		}
	}

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
