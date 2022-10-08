using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Media;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using CharactorLib;
using CharactorLib.Common;
using CharactorLib.Data;
using CharactorLib.Format;
using ControlLib;
using ControlLib.EditFunction;
using Controls;
using YYCHR.Forms;
using YYCHR.Properties;

namespace YYCHR;

public class MainForm : Form
{
	private enum NaviType
	{
		None,
		BankSelect,
		BankSelectArea,
		BankFreeSelect,
		BankFreeSelectArea,
		BankScroll,
		EditDraw,
		EditSpoit,
		EditScroll,
		EditChangePen,
		EditChangePalette,
		EditChangeSize,
		DatSelect,
		DatEditRGB,
		DatScroll,
		PalSelect,
		PalEditRGB,
		PalScroll
	}

	private const string APP_NAME = "YY-CHR.NET";

	private const string APP_VER = "Ver.1.00 ";

	private const string URL_WEB = "https://www45.atwiki.jp/yychr/";

	private const string URL_WIKI = "https://www45.atwiki.jp/yychr/";

	private const string URL_BOARD = "https://jbbs.shitaraba.net/bbs/read.cgi/computer/41853/1231162374/l50";

	private const string URL_WIKI_HINT = "http://www45.atwiki.jp/yychr/?page=";

	private const string DEFAULT_FILENAME = ".\\Resources\\yychr.chr";

	private static Type ResourceType = typeof(Resources);

	private FormatManager mFormatManager = FormatManager.GetInstance();

	private DataFileManager mDataFileManager = DataFileManager.GetInstance();

	private EditFunctionManager mPenManager = EditFunctionManager.GetInstance();

	private Settings mSetting = Settings.GetInstance();

	private Bitmap mRomBitmap = new Bitmap(128, 128, PixelFormat.Format8bppIndexed);

	private Bytemap mRomBytemap = new Bytemap(128, 128);

	private Bitmap mBmpBitmap = new Bitmap(256, 256, PixelFormat.Format8bppIndexed);

	private Bytemap mBmpBytemap = new Bytemap(256, 256);

	private Bytemap mSourceBytemap;

	private Bitmap mSourceBitmap;

	private Rectangle mSourceRect = new Rectangle(0, 0, 1, 1);

	private UndoManager mBmpUndoManager = new UndoManager();

	private CellSelector mActiveSelector;

	private static Font GuiFont = new Font("Marlett", 16f);

	private static string CheckText = "a";

	private ToolStripButton tbPenColSet;

	private ToolStripMenuItem miPenColSet;

	private static byte mLastKeyState = 0;

	private EditFunctionBase mLastPen;

	private bool mShowClipboard;

	private Bitmap mClipboardImage;

	private MouseButtons mEditingMouseButton;

	private PaletteEditorForm mPaletteEditorForm = new PaletteEditorForm();

	private string mOpenedBmpFilename = "";

	private bool mOpenedBmpModified;

	private PaletteMode mRomTabPalMode;

	private PaletteMode mBmpTabPalMode;

	private byte mRomTabPalIndex;

	private byte mBmpTabPalIndex;

	private byte[] mFindData;

	private int mFindAddr;

	private int mFindAddAddr;

	private int mFindingAddr;

	private int mFindSize;

	private int mFoundAddr;

	private bool mFindAdvanceSearch;

	private Bytemap mBytemapForFind;

	private ToolStripMenuItem miJumpListAdd;

	private ToolStripMenuItem miJumpListRemove;

	private ToolStripSeparator miJumpListSeparator;

	private List<string> NaviList = new List<string>();

	private PropertyEditorForm mPropertyEditorForm;

	private FormatBase mFormat;

	private bool mSuppressRedraw = true;

	private Control _LastNavigateControl;

	private int mDebugCounter;

	private IContainer components;

	private StatusStrip statusStrip;

	private OneClickMenuStrip menuStripMain;

	private ToolStripMenuItem miFile;

	private ToolStripMenuItem miFileExit;

	private OneClickToolStrip toolStripView;

	private Panel rightPanel;

	private PaletteSelector datPaletteSelector;

	private PaletteSelector palPaletteSelector;

	private ToolStripMenuItem miEdit;

	private ToolStripMenuItem miPalette;

	private ToolStripMenuItem miPalettePalOpen;

	private ToolStripMenuItem miPalettePalSave;

	private ToolStripSeparator miPaletteSep1;

	private ToolStripMenuItem miPaletteDatOpen;

	private ToolStripMenuItem miPaletteDatSave;

	private ToolStripMenuItem optionToolStripMenuItem;

	private ToolStripMenuItem configurationToolStripMenuItem;

	private ToolStripMenuItem miHelp;

	private ToolStripMenuItem miHelpAbout;

	private ToolStripMenuItem miFileNew;

	private ToolStripMenuItem miFileOpenRom;

	private ToolStripMenuItem miFileReload;

	private ToolStripMenuItem miFileSaveRom;

	private ToolStripMenuItem miFileSaveAsRom;

	private ToolStripSeparator miFileSep2;

	private ToolStripMenuItem miEditUndo;

	private ToolStripMenuItem miEditRedo;

	private ToolStripSeparator miEditSep0;

	private ToolStripMenuItem miEditCut;

	private ToolStripMenuItem miEditCopy;

	private ToolStripMenuItem miEditPaste;

	private ToolStripMenuItem miEditClear;

	private ToolStripMenuItem miEditSelectAll;

	private ToolStripSeparator miEditSep2;

	private OneClickToolStrip toolStripMain;

	private Panel panelEdit;

	private OneClickToolStrip toolStripPen;

	private EditPanel editPanel;

	private ToolStripButton tbViewGridBank;

	private ToolStripButton tbViewGridEditor;

	private ToolStripButton tbFileNew;

	private ToolStripButton tbFileOpenRom;

	private ToolStripButton tbFileSaveRom;

	private ToolStripButton tbEditUndo;

	private ToolStripButton tbEditRedo;

	private ToolStripButton tbEditCut;

	private ToolStripSeparator tbEditSep0;

	private ToolStripButton tbEditCopy;

	private ToolStripButton tbEditPaste;

	private ToolStripButton tbEditClear;

	private ToolStripSeparator tbEditSep1;

	private ToolStripButton tbEditMirrorHorizontal;

	private ToolStripButton tbEditMirrorVertical;

	private ToolStripButton tbEditRotateLeft;

	private ToolStripButton tbEditRotateRight;

	private ToolStripSeparator tbFileSep0;

	private ToolStripButton tbEditShiftUp;

	private ToolStripButton tbEditShiftDown;

	private ToolStripButton tbEditShiftLeft;

	private ToolStripButton tbEditShiftRight;

	private ToolStripButton tbEditReplaceColor;

	private ToolStripSeparator tbEditSep2;

	private ToolStripMenuItem miHelpPropertyEditor;

	private ToolStripSeparator miHelpSep1;

	private ToolStripMenuItem miEditMirrorHorizontal;

	private ToolStripMenuItem miEditMirrorVertical;

	private ToolStripSeparator miEditSep3;

	private ToolStripMenuItem miEditRotateLeft;

	private ToolStripMenuItem miEditRotateRight;

	private ToolStripSeparator miEditSep4;

	private ToolStripMenuItem miEditShiftLeft;

	private ToolStripMenuItem miEditShiftRight;

	private ToolStripMenuItem miEditShiftUp;

	private ToolStripMenuItem miEditShiftDown;

	private ToolStripSeparator miEditSep5;

	private ToolStripMenuItem miEditReplaceColor;

	private ToolStripMenuItem miPaletteLoadEmulatorState;

	private ToolStripSeparator miPaletteSep0;

	private TabControl tabControl;

	private TabPage tabPageBitmap;

	private TabPage tabPageChrRom;

	private OpenFileDialog openDataFileDialog;

	private SaveFileDialog saveDataFileDialog;

	private OpenFileDialog openBitmapDialog;

	private SaveFileDialog saveBitmapDialog;

	private ToolStripSeparator miFileSep1;

	private ToolStripMenuItem miFileOpenBmp;

	private ToolStripMenuItem miFileSaveBmp;

	private Panel panelChr;

	private OpenFileDialog openFileDialog;

	private ToolStripSeparator tbFileSep1;

	private ToolStripButton tbPaletteLoadEmulatorState;

	private ComboBoxEx comboBoxFormat;

	private ToolStripMenuItem miAddress;

	private ToolStripMenuItem miAddress0;

	private ToolStripMenuItem miAddress1;

	private ToolStripMenuItem miAddress2;

	private ToolStripMenuItem miAddress3;

	private ToolStripMenuItem miAddress4;

	private ToolStripMenuItem miAddress5;

	private ToolStripMenuItem miAddress6;

	private ToolStripMenuItem miAddress7;

	private ToolStripMenuItem miAddress8;

	private ToolStripMenuItem miAddress9;

	private ToolStripSeparator miAddressSep0;

	private ToolStripMenuItem miAddressInputAddress;

	private ToolStripStatusLabel slAddr;

	private ScrollPanel scrollPanelRom;

	private OneClickToolStrip toolStripAddress;

	private ToolStripButton tbAddres0;

	private ToolStripButton tbAddres1;

	private ToolStripButton tbAddres2;

	private ToolStripButton tbAddres3;

	private ToolStripButton tbAddres4;

	private ToolStripButton tbAddres5;

	private ToolStripButton tbAddres6;

	private ToolStripButton tbAddres7;

	private ToolStripButton tbAddres8;

	private ToolStripButton tbAddres9;

	private ToolStripSeparator tbAddresSep0;

	private ToolStripButton tbAddresInputAddress;

	private ComboBoxEx comboBoxRotate;

	private ComboBoxEx comboBoxMirror;

	private ComboBoxEx comboBoxPattern;

	private Panel panelToolStripAddress;

	private CellSelector cellSelectorRom;

	private CellSelector cellSelectorBmp;

	private ScrollPanelHV scrollPanelBmp;

	private ToolStripStatusLabel slXY;

	private SaveFileDialog saveFileDialog;

	private ToolStripSeparator miPaletteSep2;

	private ToolStripMenuItem miPaletteOpenADF;

	private ToolStripMenuItem miPaletteSaveADF;

	private ToolStripMenuItem miHelpOpenWebsite;

	private ToolStripMenuItem miHelpReportBugs;

	private ToolStripSeparator miHelpSep2;

	private Panel panelPalette;

	private OneClickToolStrip toolStripPalette;

	private ToolStripButton tbPaletteTypePal;

	private ToolStripButton tbPaletteTypeDat;

	private ToolStripMenuItem miPalettePaletteType;

	private ToolStripSeparator miPaletteSep3;

	private ToolStripMenuItem miPaletteTypeDat;

	private ToolStripMenuItem miPaletteTypePal;

	private ToolStripStatusLabel slRightSpace;

	private ToolStripSeparator tbEditSep3;

	private Label labelPattern;

	private Label labelMirror;

	private Label labelRotate;

	private Label labelFormat;

	private ToolStripMenuItem miPaletteSelectPalette;

	private ToolStripMenuItem miPalettePrev;

	private ToolStripMenuItem miPaletteNext;

	private ToolStripSeparator miPaletteSelectSep0;

	private ToolStripMenuItem miPaletteSelect0;

	private ToolStripMenuItem miPaletteSelect1;

	private ToolStripMenuItem miPaletteSelect2;

	private ToolStripMenuItem miPaletteSelect3;

	private ToolStripMenuItem miPaletteSelect4;

	private ToolStripMenuItem miPaletteSelect5;

	private ToolStripMenuItem miPaletteSelect6;

	private ToolStripMenuItem miPaletteSelect7;

	private ToolStripMenuItem miPaletteSelect8;

	private ToolStripMenuItem miPaletteSelect9;

	private ToolStripMenuItem miPaletteSelectA;

	private ToolStripMenuItem miPaletteSelectB;

	private ToolStripMenuItem miPaletteSelectC;

	private ToolStripMenuItem miPaletteSelectD;

	private ToolStripMenuItem miPaletteSelectE;

	private ToolStripMenuItem miPaletteSelectF;

	private ToolStripMenuItem miOption;

	private ToolStripMenuItem miOptionSetting;

	private ToolStripMenuItem miOptionExecuteFile;

	private ButtonNoFocus buttonFormatInfo;

	private ToolStripMenuItem miHelpOpenWiki;

	private ToolStripSeparator miHelpSep0;

	private ToolStripButton tbFileQuickSaveBitmap;

	private ToolStripMenuItem miFileQuickSaveBitmap;

	private ButtonNoFocus buttonPatternEdit;

	private ToolStripStatusLabel slChr;

	private ToolStripSeparator miPaletteSep4;

	private ToolStripMenuItem miPaletteLoadDefaultSetting;

	private ToolStripMenuItem miPaletteQuickSaveRGBPalette;

	private ToolStripMenuItem miPaletteQuickSavePaletteTable;

	private ToolStripMenuItem miPaletteQuickSaveADFPattern;

	private ToolStripSeparator tbPaletteSep0;

	private ToolStripButton tbWorkspaceAdd;

	private TabPage tabWorkSpace;

	private Panel panelWorkSpace;

	private WorkSpaceSelector workSpaceSelector1;

	private ToolTip toolTip;

	private ToolStripSeparator tbPaletteSep1;

	private ToolStripButton tbPaletteOpenState;

	private ToolStripMenuItem miPaletteLoadRGBPaletteFromCommon;

	private ToolStripMenuItem miPaletteLoadPaletteTableFromCommon;

	private ToolStripMenuItem miPaletteLoadADFPatternFromCommon;

	private ToolStripSeparator miAddressSep1;

	private ToolStripMenuItem miAddressFindPrevious;

	private ToolStripMenuItem miAddressFindNext;

	private ToolStripButton tbWorkspaceLoad;

	private ToolStripButton tbWorkspaceSave;

	private ToolStripButton tbWorkspaceRemovePattern;

	private ToolStripButton tbAddressFindPrevious;

	private ToolStripButton tbAddressFindNext;

	private ToolStripButton tbAddressJumpList;

	private ContextMenuStrip popupJumpListMenu;

	private OneClickToolStrip tsWoekSpace;

	private Panel panelChrSetting;

	private Panel panelWorkSpaceTS;

	private ToolStripSeparator tbAddresSep1;

	private BackgroundWorker mFindWorker;

	private ContextMenuStrip contextMenuStripChr;

	private ToolStripMenuItem cmiEditCut;

	private ToolStripMenuItem cmiEditCopy;

	private ToolStripMenuItem cmiEditPaste;

	private ToolStripMenuItem cmiEditClear;

	private ToolStripMenuItem cmiEditSelectAll;

	private ToolStripMenuItem miEditPasteOptimizedImage;

	private ToolStripStatusLabelEx slHintMouseButtonR;

	private ToolStripStatusLabelEx slHintMouseButtonL;

	private ToolStripStatusLabelEx slHintMouseWheel;

	private ToolStripStatusLabelEx slKeyCtrl;

	private ToolStripStatusLabelEx slKeyShift;

	private ToolStripStatusLabelEx slKeyAlt;

	private Label lVersion;

	private ToolStripSeparator miEditSep1;

	private ToolStripMenuItem miEditClearClipboard;

	private ToolStripSeparator miOptionSep0;

	private ToolStripMenuItem miPen;

	private ToolStripMenuItem miOptionShowAllMenu;

	private Panel panelForPlugin;

	private ToolStripMenuItem miOptionLanguage;

	private ToolStripMenuItem miLanguageEnglish;

	private ToolStripMenuItem miLanguageJapanese;

	private ToolStripMenuItem miLanguageSystem;

	private ToolStripSeparator miLanguageSep0;

	private ToolStripSeparator tbFileSep3;

	private ToolStripButton tbOptionExecuteFile;

	private ToolStripButton tbPaletteTypeBmp;

	private ToolStripMenuItem miPaletteTypeBmp;

	private ToolStripButton tbFileOpenBmp;

	private ToolStripButton tbFileSaveBmp;

	private ToolStripButton tbAddressJumpListPrev;

	private ToolStripButton tbAddressJumpListNext;

	private ToolStripSeparator miLanguageSep1;

	private ToolStripMenuItem miLanguageExportLng;

	private ToolStripMenuItem miLanguageLngFile;

	private ToolStripSeparator miLanguageSep2;

	private ToolStripMenuItem miLanguageSettingAutoLoadLng;

	private ToolStripDropDownButton tbViewGuiRate;

	private ToolStripDropDownButton tbViewEditSize;

	private ToolStripDropDownButton tbViewGridStyle;

	private ToolStripMenuItem tbViewEditorSize8;

	private ToolStripMenuItem tbViewEditorSize16;

	private ToolStripMenuItem tbViewEditorSize32;

	private ToolStripMenuItem tbViewEditorSize64;

	private ToolStripMenuItem tbViewEditorSize128;

	private ToolStripSeparator toolStripSeparator1;

	private ToolStripSeparator toolStripSeparator2;

	private ToolStripSeparator toolStripSeparator3;

	private int Address
	{
		get
		{
			return scrollPanelRom.Value;
		}
		set
		{
			scrollPanelRom.Value = value;
		}
	}

	public bool WorkSpaceVisible
	{
		get
		{
			return tbWorkspaceAdd.Visible;
		}
		set
		{
			panelWorkSpaceTS.Visible = value;
			if (value)
			{
				tabControl.TabPages.Add(tabWorkSpace);
			}
			else
			{
				tabControl.TabPages.Remove(tabWorkSpace);
			}
		}
	}

	public bool TabIsRom => tabControl.SelectedTab == tabPageChrRom;

	public bool TabIsBmp => tabControl.SelectedTab == tabPageBitmap;

	public bool TabIsWorkspace => tabControl.SelectedTab == tabWorkSpace;

	public bool IsColMode
	{
		get
		{
			if (TabIsRom)
			{
				return mDataFileManager.ColSetData.IsColMode;
			}
			return false;
		}
	}

	public bool DisableKeyInput { get; set; }

	public MainForm()
	{
		InitializeComponent();
		InitializeControlLang();
		InitializeJumpList();
		Text = "YY-CHR.NET";
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		string directoryName = Path.GetDirectoryName(commandLineArgs[0]);
		InitializeFormat();
		UpdateContextMenu();
		SetMouseWheelScrollRate();
		AddComboBoxMirror(MirrorType.None);
		AddComboBoxRotate(RotateType.None);
		AddComboBoxGridStyle(mSetting.GridStyle);
		AddComboBoxGuiRate();
		mDataFileManager.RomData.CreateNew(16384);
		string filename = directoryName + "\\Resources\\chr001.bmp";
		LoadBitmapTabBmp(filename, enableNoExistMsg: false);
		mSourceBitmap = mRomBitmap;
		mSourceBytemap = mRomBytemap;
		cellSelectorRom.Image = mRomBitmap;
		cellSelectorBmp.Image = mBmpBitmap;
		mActiveSelector = cellSelectorRom;
		UpdateFormSize();
		UpdateGuiRateToolStripButtonChecked();
		mDataFileManager.RomData.DataLoaded += RomData_DataLoaded;
		mDataFileManager.AdfPattern.DataLoaded += AdfPattern_DataLoaded;
		mDataFileManager.PalInfoNes.DataLoaded += PalInfo_DataLoaded;
		mDataFileManager.PalInfo256.DataLoaded += PalInfo_DataLoaded;
		mDataFileManager.DatInfoNes.DataLoaded += DatInfo_DataLoaded;
		mDataFileManager.DatInfoNes.DataModified += DatInfo_DataModified;
		LoadDefaultExtFiles();
		CreatePenToolBar();
		SelectEditPanelPen(mPenManager.SelectedFunction);
		WorkSpaceVisible = false;
		tabControl.SelectedIndex = 0;
		try
		{
			if (commandLineArgs.Length >= 2 && (commandLineArgs[1].Length != 3 || !commandLineArgs[1].StartsWith("-")))
			{
				string filename2 = commandLineArgs[1];
				OpenFile(filename2, initAddress: true);
			}
			else if (File.Exists(".\\Resources\\yychr.chr"))
			{
				OpenFile(".\\Resources\\yychr.chr", initAddress: true);
			}
		}
		catch
		{
			string resourceString = ResourceUtility.GetResourceString(ResourceType, "Resources.TitleError");
			string text = ResourceUtility.GetResourceString(ResourceType, "Resources.MessageCommandlineError").Replace("\\r", "\r").Replace("\\n", "\n") + commandLineArgs[1];
			MsgBox.Show(this, text, resourceString, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
		SetControlFocus();
		UpdateGrid();
		UpdateEditRectSize();
		UpdateMenuPenColSetVisible();
		UpdateStatusbarSelection(mSourceRect);
		InitMenuCheckStateRenderer();
		UpdateBuildDateLabel();
		if (mSetting.StartupLanguageFromLng)
		{
			LoadLanguageFileIfExists();
		}
		UpdateLanguageMenuState();
		LoadNaviText();
		if (mSetting.OptionSaveWindowPosition)
		{
			Rectangle bounds = Screen.FromControl(this).Bounds;
			if (new Rectangle(bounds.Left, bounds.Top, bounds.Width - 300, bounds.Height - 150).Contains(mSetting.OptionSavedWindowPosition))
			{
				base.Location = mSetting.OptionSavedWindowPosition;
			}
		}
	}

	private void SetControlFocus()
	{
		Control control = null;
		control = ((mActiveSelector == null) ? this : ((mActiveSelector.Parent == null || !(mActiveSelector.Parent is ScrollPanel)) ? ((Control)mActiveSelector) : ((Control)(mActiveSelector.Parent as ScrollPanel))));
		if (control != null)
		{
			if (base.ActiveControl != control)
			{
				base.ActiveControl = control;
			}
			if (!control.Focused && control.CanSelect)
			{
				control.Focus();
			}
		}
	}

	private void SetMainFormControlSize()
	{
		scrollPanelRom.ClientAreaSize = cellSelectorRom.Size;
		panelToolStripAddress.Left = scrollPanelRom.Right + 3;
		panelChrSetting.Top = scrollPanelRom.Top * 2 + scrollPanelRom.Height;
		int num = panelChrSetting.Bottom - 20;
		panelForPlugin.SetBounds(0, num, panelForPlugin.Width, panelChr.Height - num);
		panelPalette.Top = editPanel.Top * 2 + editPanel.Height;
		UpdatePalettePanelHeight();
	}

	private void UpdatePalettePanelHeight()
	{
		int bottom = palPaletteSelector.Bottom;
		if (datPaletteSelector.Bottom > bottom)
		{
			bottom = datPaletteSelector.Bottom;
		}
		panelPalette.Height = bottom + 5;
	}

	private void UpdateContextMenu()
	{
		ContextMenuStrip contextMenuStrip = null;
		if (mSetting.EnableRightClickMenu)
		{
			contextMenuStrip = contextMenuStripChr;
		}
		cellSelectorRom.ContextMenuStrip = contextMenuStrip;
		cellSelectorBmp.ContextMenuStrip = contextMenuStrip;
	}

	private void SetMouseWheelScrollRate()
	{
		int num = mSetting.BankWheelScrollRate;
		if (num < 1)
		{
			num = 1;
		}
		if (num > 16)
		{
			num = 16;
		}
		if (scrollPanelRom.WheelRate != num)
		{
			scrollPanelRom.WheelRate = num;
		}
	}

	private void InitializeControlLang()
	{
		tabPageChrRom.ToolTipText = GetResourceText(tabPageChrRom.ToolTipText);
		tabPageBitmap.ToolTipText = GetResourceText(tabPageBitmap.ToolTipText);
		tabWorkSpace.ToolTipText = GetResourceText(tabWorkSpace.ToolTipText);
		tbAddressFindPrevious.ToolTipText = GetResourceText(tbAddressFindPrevious.ToolTipText);
		tbAddressFindNext.ToolTipText = GetResourceText(tbAddressFindNext.ToolTipText);
	}

	private string GetResourceText(string text)
	{
		return text.Replace("\\n", "\n");
	}

	private void InitMenuCheckStateRenderer()
	{
		try
		{
			if (base.DesignMode)
			{
				return;
			}
			if (menuStripMain.Renderer != null)
			{
				menuStripMain.Renderer.RenderItemCheck += Render_RenderItemCheck;
			}
			if (contextMenuStripChr.Renderer != null)
			{
				contextMenuStripChr.Renderer.RenderItemCheck += Render_RenderItemCheck;
			}
			if (tbViewGuiRate.DropDown.Renderer != null)
			{
				tbViewGuiRate.DropDown.Renderer.RenderItemCheck += Render_RenderItemCheck;
			}
			if (tbViewEditSize.DropDown.Renderer != null)
			{
				tbViewEditSize.DropDown.Renderer.RenderItemCheck += Render_RenderItemCheck;
			}
			if (tbViewGridStyle.DropDown.Renderer != null)
			{
				tbViewGridStyle.DropDown.Renderer.RenderItemCheck += Render_RenderItemCheck;
			}
			if (mSetting.OptionShowToolbarAsButton)
			{
				if (toolStripMain.Renderer != null)
				{
					toolStripMain.Renderer.RenderButtonBackground += RenderT_RenderItemBackground;
				}
				if (toolStripAddress.Renderer != null)
				{
					toolStripAddress.Renderer.RenderButtonBackground += RenderT_RenderItemBackground;
				}
				if (toolStripPen.Renderer != null)
				{
					toolStripPen.Renderer.RenderButtonBackground += RenderT_RenderItemBackground;
				}
				if (toolStripPalette.Renderer != null)
				{
					toolStripPalette.Renderer.RenderButtonBackground += RenderT_RenderItemBackground;
				}
				if (toolStripView.Renderer != null)
				{
					toolStripView.Renderer.RenderButtonBackground += RenderT_RenderItemBackground;
				}
				if (tsWoekSpace.Renderer != null)
				{
					tsWoekSpace.Renderer.RenderButtonBackground += RenderT_RenderItemBackground;
				}
			}
		}
		catch
		{
		}
	}

	private void Render_RenderItemCheck(object sender, ToolStripItemImageRenderEventArgs e)
	{
		try
		{
			Graphics graphics = e.Graphics;
			if (!(e.Item is ToolStripMenuItem toolStripMenuItem) || !toolStripMenuItem.Checked)
			{
				return;
			}
			Rectangle imageRectangle = e.ImageRectangle;
			Rectangle rect = new Rectangle(imageRectangle.Left - 2, imageRectangle.Top - 2, imageRectangle.Width + 3, imageRectangle.Height + 3);
			if (toolStripMenuItem.Selected)
			{
				using Pen pen = new Pen(Color.Orange);
				using Brush brush = new SolidBrush(Color.Gold);
				graphics.FillRectangle(brush, rect);
				graphics.DrawRectangle(pen, rect);
			}
			else
			{
				using Pen pen2 = new Pen(Color.Orange);
				using Brush brush2 = new SolidBrush(Color.Gold);
				graphics.FillRectangle(brush2, rect);
				graphics.DrawRectangle(pen2, rect);
			}
			if (toolStripMenuItem.Image == null)
			{
				graphics.DrawString(point: new Point(rect.X - 3, rect.Y), s: CheckText, font: GuiFont, brush: SystemBrushes.MenuText);
			}
		}
		catch
		{
		}
	}

	private void RenderT_RenderItemBackground(object sender, ToolStripItemRenderEventArgs e)
	{
		try
		{
			Graphics graphics = e.Graphics;
			if (e.Item is ToolStripButton toolStripButton)
			{
				Pen pen;
				Pen pen2;
				Pen pen3;
				Pen pen4;
				if (toolStripButton.Pressed || toolStripButton.Checked)
				{
					pen = SystemPens.ControlDarkDark;
					pen2 = SystemPens.ControlLightLight;
					pen3 = SystemPens.ControlDark;
					pen4 = SystemPens.ControlLight;
				}
				else
				{
					pen = SystemPens.ControlLightLight;
					pen2 = SystemPens.ControlDarkDark;
					pen3 = SystemPens.ControlLight;
					pen4 = SystemPens.ControlDark;
				}
				Rectangle contentRectangle = e.Item.ContentRectangle;
				Point point = new Point(contentRectangle.Left - 2, contentRectangle.Top - 2);
				Point point2 = new Point(contentRectangle.Right + 1, contentRectangle.Top - 2);
				Point point3 = new Point(contentRectangle.Left - 2, contentRectangle.Bottom);
				Point point4 = new Point(contentRectangle.Right + 1, contentRectangle.Bottom);
				graphics.DrawLine(pen, point3, point);
				graphics.DrawLine(pen, point, point2);
				graphics.DrawLine(pen2, point2, point4);
				graphics.DrawLine(pen2, point4, point3);
				Point point5 = new Point(contentRectangle.Left - 1, contentRectangle.Top - 1);
				Point point6 = new Point(contentRectangle.Right, contentRectangle.Top - 1);
				Point point7 = new Point(contentRectangle.Left - 1, contentRectangle.Bottom - 1);
				Point point8 = new Point(contentRectangle.Right, contentRectangle.Bottom - 1);
				graphics.DrawLine(pen3, point7, point5);
				graphics.DrawLine(pen3, point5, point6);
				graphics.DrawLine(pen4, point6, point8);
				graphics.DrawLine(pen4, point8, point7);
			}
		}
		catch
		{
		}
	}

	private void AddComboBoxMirror(MirrorType defaultMirrorType)
	{
		comboBoxMirror.Items.Clear();
		comboBoxMirror.Items.Add(MirrorType.None);
		comboBoxMirror.Items.Add(MirrorType.Horizontal);
		comboBoxMirror.Items.Add(MirrorType.Vertical);
		comboBoxMirror.Items.Add(MirrorType.Both);
		comboBoxMirror.SelectedItem = defaultMirrorType;
	}

	private void AddComboBoxRotate(RotateType defaultRotateType)
	{
		comboBoxRotate.Items.Clear();
		comboBoxRotate.Items.Add(RotateType.None);
		comboBoxRotate.Items.Add(RotateType.Right);
		comboBoxRotate.Items.Add(RotateType.Turn);
		comboBoxRotate.Items.Add(RotateType.Left);
		comboBoxRotate.SelectedItem = defaultRotateType;
	}

	private void AddComboBoxGuiRate()
	{
		tbViewGuiRate.DropDownItems.Clear();
		for (int i = 2; i <= 5; i++)
		{
			int num = 128 * i;
			int num2 = 128 * i;
			ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("x" + i + " (" + num + "x" + num2 + ")", null, ActionGuiRateSelected);
			toolStripMenuItem.Tag = i;
			tbViewGuiRate.DropDownItems.Add(toolStripMenuItem);
		}
		tbViewGuiRate.Text = tbViewGuiRate.DropDownItems[0].Text;
	}

	private void ActionGuiRateSelected(object sender, EventArgs e)
	{
		if (sender is ToolStripMenuItem)
		{
			int guiSizeRate = (int)(sender as ToolStripMenuItem).Tag;
			mSetting.GuiSizeRate = guiSizeRate;
		}
		UpdateGuiRateToolStripButtonChecked();
	}

	private void UpdateGuiRateToolStripButtonChecked()
	{
		UpdateControlSizeFromGuiZoomRate();
		UpdateFormSize();
		UpdateStatusbarVisible();
		foreach (ToolStripMenuItem dropDownItem in tbViewGuiRate.DropDownItems)
		{
			bool flag = (int)dropDownItem.Tag == mSetting.GuiSizeRate;
			if (dropDownItem.Checked != flag)
			{
				dropDownItem.Checked = flag;
			}
			if (flag)
			{
				tbViewGuiRate.Text = dropDownItem.Text;
			}
		}
	}

	private void ActionViewEditorSizeSelected(object sender, EventArgs e)
	{
		if (sender is ToolStripItem)
		{
			ToolStripItem toolStripItem = sender as ToolStripItem;
			int num = 32;
			if (toolStripItem.Tag is string)
			{
				string value = (string)toolStripItem.Tag;
				if (!string.IsNullOrWhiteSpace(value))
				{
					num = Convert.ToInt32(value);
				}
			}
			else if (toolStripItem.Tag is int)
			{
				num = (int)toolStripItem.Tag;
			}
			if (num < 8)
			{
				num = 8;
			}
			if (num >= 128)
			{
				num = 128;
			}
			Size size = new Size(num, num);
			if (mSetting.EditRectSize != size)
			{
				mSetting.EditRectSize = size;
			}
		}
		UpdateEditRectSize();
	}

	private void SelectEditSizeItem(int delta)
	{
		int count = tbViewEditSize.DropDownItems.Count;
		if (count < 1)
		{
			return;
		}
		for (int i = 0; i < count; i++)
		{
			if ((tbViewEditSize.DropDownItems[i] as ToolStripMenuItem).Checked)
			{
				int index = (i + delta * -1 + count) % count;
				tbViewEditSize.DropDownItems[index].PerformClick();
				break;
			}
		}
	}

	private void UpdateEditRectSize()
	{
		if (mActiveSelector.DefaultSelectSize != mSetting.EditRectSize)
		{
			mActiveSelector.DefaultSelectSize = mSetting.EditRectSize;
			mActiveSelector.SelectedRect = new Rectangle(mActiveSelector.SelectedRect.Location, mActiveSelector.DefaultSelectSize);
			mActiveSelector.Refresh();
		}
		mSourceRect = mActiveSelector.SelectedRect;
		UpdateEditPanelSource();
		UpdateStatusbarSelection(mSourceRect);
		foreach (ToolStripMenuItem dropDownItem in tbViewEditSize.DropDownItems)
		{
			int num = 32;
			if (dropDownItem.Tag is string)
			{
				string value = (string)dropDownItem.Tag;
				if (!string.IsNullOrWhiteSpace(value))
				{
					num = Convert.ToInt32(value);
				}
			}
			else if (dropDownItem.Tag is int)
			{
				num = (int)dropDownItem.Tag;
			}
			bool flag = num == mSetting.EditRectSize.Width;
			if (dropDownItem.Checked != flag)
			{
				dropDownItem.Checked = flag;
			}
			if (flag)
			{
				tbViewEditSize.Text = dropDownItem.Text;
			}
		}
	}

	private void AddComboBoxGridStyle(GridStyle defaultGridStyle)
	{
		tbViewGridStyle.DropDownItems.Clear();
		foreach (GridStyle value in Enum.GetValues(typeof(GridStyle)))
		{
			ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(value.ToString(), null, ActionGridStyleSelected);
			toolStripMenuItem.Tag = value;
			tbViewGridStyle.DropDownItems.Add(toolStripMenuItem);
		}
		tbViewGridStyle.Text = tbViewGridStyle.DropDownItems[0].Text;
	}

	private void ActionGridStyleSelected(object sender, EventArgs e)
	{
		if (sender is ToolStripItem)
		{
			ToolStripItem toolStripItem = sender as ToolStripItem;
			GridStyle gridStyle = GridStyle.None;
			if (toolStripItem.Tag is GridStyle)
			{
				gridStyle = (GridStyle)toolStripItem.Tag;
			}
			else if (toolStripItem.Tag is int)
			{
				gridStyle = (GridStyle)(int)toolStripItem.Tag;
			}
			if (gridStyle < GridStyle.None)
			{
				gridStyle = GridStyle.None;
			}
			if (gridStyle >= GridStyle.Line)
			{
				gridStyle = GridStyle.Line;
			}
			if (mSetting.GridStyle != gridStyle)
			{
				mSetting.GridStyle = gridStyle;
			}
		}
		UpdateGrid();
	}

	private void UpdateGrid()
	{
		editPanel.GridStyle = mSetting.GridStyle;
		editPanel.GridVisible = mSetting.GridEditVisible;
		editPanel.GridColor1 = mSetting.GridEditColor1;
		editPanel.GridColor2 = mSetting.GridEditColor2;
		editPanel.EditingRectColor = mSetting.EditingRectColor;
		editPanel.Refresh();
		cellSelectorRom.GridStyle = mSetting.GridStyle;
		cellSelectorRom.GridVisible = mSetting.GridBankVisible;
		cellSelectorRom.GridColor1 = mSetting.GridBankColor1;
		cellSelectorRom.GridColor2 = mSetting.GridBankColor2;
		cellSelectorRom.Refresh();
		cellSelectorBmp.GridStyle = mSetting.GridStyle;
		cellSelectorBmp.GridColor1 = mSetting.GridBankColor1;
		cellSelectorBmp.GridColor2 = mSetting.GridBankColor2;
		cellSelectorBmp.GridVisible = mSetting.GridBankVisible;
		cellSelectorBmp.Refresh();
		if (tbViewGridBank.Checked != mSetting.GridBankVisible)
		{
			tbViewGridBank.Checked = mSetting.GridBankVisible;
		}
		if (tbViewGridEditor.Checked != mSetting.GridEditVisible)
		{
			tbViewGridEditor.Checked = mSetting.GridEditVisible;
		}
		foreach (ToolStripMenuItem dropDownItem in tbViewGridStyle.DropDownItems)
		{
			bool flag = (int)dropDownItem.Tag == (int)mSetting.GridStyle;
			if (dropDownItem.Checked != flag)
			{
				dropDownItem.Checked = flag;
			}
			if (flag)
			{
				tbViewGridStyle.Text = dropDownItem.Text;
			}
		}
	}

	private void CreatePenToolBar()
	{
		foreach (ToolStripButton item in toolStripPen.Items)
		{
			if (item.Image != null)
			{
				item.Image.Dispose();
			}
			item.Image = null;
		}
		foreach (ToolStripMenuItem dropDownItem in miPen.DropDownItems)
		{
			dropDownItem.Image = null;
		}
		miPen.DropDownItems.Clear();
		toolStripPen.Items.Clear();
		int num = 1;
		foreach (EditFunctionBase function in mPenManager.FunctionList)
		{
			ToolStripButton toolStripButton2;
			if (function.Icon != null)
			{
				Bitmap bitmap = new Bitmap(function.Icon);
				toolStripButton2 = new ToolStripButton(bitmap);
				toolStripButton2.ImageTransparentColor = bitmap.GetPixel(0, 0);
				toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Image;
			}
			else
			{
				toolStripButton2 = new ToolStripButton();
				toolStripButton2.Text = function.Name.Substring(0, 1);
			}
			toolStripButton2.AutoSize = false;
			toolStripButton2.Name = "tbPen" + num;
			toolStripButton2.Checked = false;
			toolStripButton2.Margin = new Padding(0, 0, 0, 0);
			toolStripButton2.Size = new Size(25, 23);
			toolStripButton2.Text = function.Name;
			toolStripButton2.ToolTipText = function.Name;
			toolStripButton2.Tag = function;
			toolStripButton2.CheckOnClick = true;
			toolStripButton2.Click += ActionPenSelect;
			toolStripPen.Items.Add(toolStripButton2);
			ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
			toolStripMenuItem.Name = "miPen" + num;
			toolStripMenuItem.Checked = toolStripButton2.Checked;
			toolStripMenuItem.Image = toolStripButton2.Image;
			toolStripMenuItem.ImageTransparentColor = toolStripButton2.ImageTransparentColor;
			toolStripMenuItem.Text = toolStripButton2.Text;
			toolStripMenuItem.ToolTipText = toolStripButton2.ToolTipText;
			toolStripMenuItem.Tag = toolStripButton2.Tag;
			toolStripMenuItem.Click += ActionPenSelect;
			if (num >= 1 && num <= 10)
			{
				toolStripMenuItem.ShortcutKeyDisplayString = (num % 10).ToString("D1");
			}
			miPen.DropDownItems.Add(toolStripMenuItem);
			num++;
			if (function is ColSet)
			{
				tbPenColSet = toolStripButton2;
				miPenColSet = toolStripMenuItem;
				tbPenColSet.Visible = false;
				miPenColSet.Visible = false;
			}
		}
	}

	private void LoadDefaultExtFiles()
	{
		string text = "";
		try
		{
			text = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
		}
		catch (Exception)
		{
		}
		string exeDirectory = Utility.GetExeDirectory();
		string text2 = "\\Resources\\";
		string text3 = "yychr";
		string filename = exeDirectory + text2 + text3 + ".pal";
		string filename2 = exeDirectory + text2 + text3 + ".dat";
		string text4 = exeDirectory + text2 + text3 + "." + text + ".adf";
		if (!File.Exists(text4))
		{
			text4 = exeDirectory + text2 + text3 + ".adf";
		}
		mDataFileManager.PalInfoNes.LoadFromFile(filename);
		mDataFileManager.DatInfoNes.PalInfo = mDataFileManager.PalInfoNes;
		mDataFileManager.PalInfo256.LoadFromFile(filename);
		mDataFileManager.DatInfo256.PalInfo = mDataFileManager.PalInfo256;
		mDataFileManager.DatInfoNes.LoadFromFile(filename2);
		mDataFileManager.AdfPattern.LoadFromFile(text4);
	}

	private void RomData_DataLoaded(object sender, EventArgs e)
	{
	}

	private void DatInfo_DataLoaded(object sender, EventArgs e)
	{
		UpdateDatPaletteLabel();
		UpdateDatPalette();
		SetBytemapPalette();
		RefreshBitmapFromBytemap();
		UpdatePalSelectorSelection();
	}

	private void DatInfo_DataModified(object sender, EventArgs e)
	{
		UpdateDatPaletteLabel();
		UpdateDatPalette();
		SetBytemapPalette();
		RefreshBitmapFromBytemap();
		UpdatePalSelectorSelection();
	}

	private void UpdateDatPaletteLabel()
	{
		DatInfo datInfo = mDataFileManager.DatInfo;
		for (int i = 0; i < datInfo.Data.Length; i++)
		{
			datPaletteSelector.Label[i] = datInfo.Data[i].ToString("X2");
		}
	}

	private void PalInfo_DataLoaded(object sender, EventArgs e)
	{
		UpdatePalPalette();
		UpdateDatPalette();
		SetBytemapPalette();
		RefreshBitmapFromBytemap();
	}

	private void AdfPattern_DataLoaded(object sender, EventArgs e)
	{
		ReloadComboboxAdfPattern();
		SetBytemapPalette();
		RefreshBitmapFromBytemap();
	}

	private void UpdatePalPalette()
	{
		palPaletteSelector.Palette = mDataFileManager.PalInfo.Colors;
		RefreshPalSelector();
	}

	private void RefreshPalSelector()
	{
		palPaletteSelector.Refresh();
	}

	private void UpdateDatPalette()
	{
		datPaletteSelector.Palette = mDataFileManager.DatInfo.Colors;
		RefreshDatSelector();
	}

	private void RefreshDatSelector()
	{
		datPaletteSelector.Refresh();
	}

	private void ReloadComboboxAdfPattern()
	{
		comboBoxPattern.Items.Clear();
		AdfInfo[] adfPatterns = mDataFileManager.AdfPattern.AdfPatterns;
		foreach (AdfInfo item in adfPatterns)
		{
			comboBoxPattern.Items.Add(item);
		}
		comboBoxPattern.SelectedIndex = 0;
	}

	private void comboBoxPattern_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (mFormat != null)
		{
			if (comboBoxPattern.SelectedIndex == -1)
			{
				mFormat.AdfPattern = null;
			}
			else
			{
				mFormat.AdfPattern = (AdfInfo)comboBoxPattern.SelectedItem;
			}
		}
		RedrawFormat();
	}

	private bool CheckSaveModifiedData()
	{
		bool result = false;
		if (mDataFileManager.RomData.Modified && mSetting.ShowSaveModifiedDialog)
		{
			string caption = "YY-CHR.NET";
			string resourceString = ResourceUtility.GetResourceString(ResourceType, "Resources.ConfirmModifiedSave");
			DialogResult num = MsgBox.Show(this, resourceString, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
			if (num == DialogResult.Cancel)
			{
				result = true;
			}
			if (num == DialogResult.Yes)
			{
				miFileSaveRom.PerformClick();
			}
		}
		return result;
	}

	private bool CheckReloadModifiedData()
	{
		bool result = true;
		if (mDataFileManager.RomData.Modified)
		{
			string caption = "YY-CHR.NET";
			string resourceString = ResourceUtility.GetResourceString(ResourceType, "Resources.ConfirmModifiedReload");
			if (MsgBox.Show(this, resourceString, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
			{
				result = false;
			}
		}
		return result;
	}

	private void MainForm_Load(object sender, EventArgs e)
	{
		editPanel.MouseWheel += EditPanel_MouseWheel;
		comboBoxFormat.SelectedIndexChanged += comboBoxFormat_SelectedIndexChanged;
		comboBoxMirror.SelectedIndexChanged += comboBoxFormat_SelectedIndexChanged;
		comboBoxRotate.SelectedIndexChanged += comboBoxFormat_SelectedIndexChanged;
		comboBoxPattern.SelectedIndexChanged += comboBoxPattern_SelectedIndexChanged;
		RefreshPalSelector();
		AfterDatPaletteSelected();
		UpdateUndoMenu();
		UpdateStatusbarVisible();
		mSuppressRedraw = false;
		RedrawFormat();
	}

	private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (mSetting.OptionSaveWindowPosition)
		{
			mSetting.OptionSavedWindowPosition = base.Location;
		}
		e.Cancel = CheckSaveModifiedData();
	}

	private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
	{
		mSetting.Save();
	}

	private void MainForm_Activated(object sender, EventArgs ev)
	{
		try
		{
			base.Activated -= MainForm_Activated;
			bool num = mDataFileManager.RomData.CheckFileDateChanged();
			mDataFileManager.RomData.LoadFileDate();
			if (num && mSetting.ShowReloadExternalChangeDialog)
			{
				string caption = "YY-CHR.NET";
				string resourceString = ResourceUtility.GetResourceString(ResourceType, "Resources.ConfirmReloadChangedExternalChange");
				if (MsgBox.Show(this, resourceString, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
				{
					miFileReload.PerformClick();
				}
			}
			if (mShowClipboard)
			{
				RefreshClipboardImage();
				UpdateEditPanelSource();
			}
			UpdateMenuAll();
			if (mSetting.CheckKeyOnActivated)
			{
				UpdateKeyStateByControl();
				UpdateKeyStatus();
			}
			SetControlFocus();
		}
		catch
		{
		}
		finally
		{
			base.Activated += MainForm_Activated;
		}
	}

	private void MainForm_DragEnter(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(DataFormats.FileDrop))
		{
			e.Effect = DragDropEffects.Copy;
		}
		else
		{
			e.Effect = DragDropEffects.None;
		}
	}

	private void MainForm_DragDrop(object sender, DragEventArgs e)
	{
		string[] array = (string[])e.Data.GetData(DataFormats.FileDrop, autoConvert: false);
		if (array != null && !string.IsNullOrEmpty(array[0]))
		{
			string filename = array[0];
			OpenDragDropFile(filename);
		}
	}

	private void OpenDragDropFile(string filename)
	{
		long length = new FileInfo(filename).Length;
		char[] trimChars = new char[3] { ' ', '\t', '.' };
		string extension = Path.GetExtension(filename);
		extension = extension.Trim(trimChars).ToLower();
		if (extension == "bmp" || extension == "png")
		{
			LoadImageFile(filename);
			return;
		}
		DataFileBase dataFileBase = null;
		dataFileBase = ((extension == "dat" && length <= 256) ? mDataFileManager.DatInfoNes : ((!(extension == "pal")) ? mDataFileManager.GetFormatByFilename(filename) : mDataFileManager.PalInfo));
		if (dataFileBase == mDataFileManager.RomData)
		{
			dataFileBase = null;
		}
		if (dataFileBase == null && mDataFileManager.DataFileCustom.CheckExtSupported(extension))
		{
			string resourceString = ResourceUtility.GetResourceString(ResourceType, "Resources.TitleConfirm");
			string resourceString2 = ResourceUtility.GetResourceString(ResourceType, "Resources.ConfirmOpenStateAsRom");
			switch (MsgBox.Show(this, resourceString2, resourceString, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
			{
			case DialogResult.Yes:
				mDataFileManager.DataFileCustom.LoadFromFile(filename);
				return;
			case DialogResult.No:
				break;
			default:
				return;
			}
			dataFileBase = null;
		}
		if (dataFileBase != null)
		{
			dataFileBase.LoadFromFile(filename);
			UpdatePalPalette();
			UpdateDatPalette();
			SetBytemapPalette();
			RedrawFormat();
			RefreshBitmapFromBytemap();
		}
		else
		{
			if (CheckSaveModifiedData())
			{
				return;
			}
			OpenFile(filename, initAddress: true);
		}
		UpdateMenuPenColSetVisible();
	}

	private void MainForm_KeyDown(object sender, KeyEventArgs e)
	{
		UpdateKeyState(e);
		UpdateKeyStatus();
		if (DisableKeyInput)
		{
			e.Handled = true;
		}
		else
		{
			bool flag2 = (e.Handled = ProcessKey_Main(e.KeyData));
		}
	}

	private void MainForm_KeyUp(object sender, KeyEventArgs e)
	{
		UpdateKeyState(e);
		UpdateKeyStatus();
	}

	private void UpdateKeyState(KeyEventArgs e)
	{
		KeyState.Control = e.Control;
		KeyState.Shift = e.Shift;
		KeyState.Alt = e.Alt;
	}

	private void UpdateKeyStateByControl()
	{
		KeyState.Control = (Control.ModifierKeys & Keys.Control) == Keys.Control;
		KeyState.Shift = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
		KeyState.Alt = (Control.ModifierKeys & Keys.Alt) == Keys.Alt;
	}

	private void UpdateKeyStatus()
	{
		cellSelectorRom.PixelSelect = KeyState.Alt;
		cellSelectorBmp.PixelSelect = KeyState.Alt;
		Image image = (KeyState.Alt ? Resources.ControlKeyAltON : Resources.ControlKeyAlt);
		Image image2 = (KeyState.Control ? Resources.ControlKeyControlON : Resources.ControlKeyControl);
		Image image3 = (KeyState.Shift ? Resources.ControlKeyShiftON : Resources.ControlKeyShift);
		if (slKeyAlt.Image != image)
		{
			slKeyAlt.Image = image;
		}
		if (slKeyCtrl.Image != image2)
		{
			slKeyCtrl.Image = image2;
		}
		if (slKeyShift.Image != image3)
		{
			slKeyShift.Image = image3;
		}
		if (mLastKeyState != KeyState.State)
		{
			mLastKeyState = KeyState.State;
			ActionControlNavigationUpdate_MouseEnter(null, EventArgs.Empty);
		}
		if (KeyState.Control)
		{
			if (mLastPen == null)
			{
				mLastPen = mPenManager.SelectedFunction;
				EditFunctionBase fillPaintPen = mPenManager.FillPaintPen;
				SelectEditPanelPen(fillPaintPen);
			}
		}
		else if (mLastPen != null)
		{
			SelectEditPanelPen(mLastPen);
			mLastPen = null;
		}
	}

	private bool ProcessKey_Main(Keys keyData)
	{
		bool result = true;
		switch (keyData)
		{
		case Keys.D0:
		case Keys.D1:
		case Keys.D2:
		case Keys.D3:
		case Keys.D4:
		case Keys.D5:
		case Keys.D6:
		case Keys.D7:
		case Keys.D8:
		case Keys.D9:
		case Keys.NumPad0:
		case Keys.NumPad1:
		case Keys.NumPad2:
		case Keys.NumPad3:
		case Keys.NumPad4:
		case Keys.NumPad5:
		case Keys.NumPad6:
		case Keys.NumPad7:
		case Keys.NumPad8:
		case Keys.NumPad9:
		{
			int num = 0;
			if (keyData >= Keys.NumPad0 && keyData <= Keys.NumPad9)
			{
				num = (int)(keyData - 96);
			}
			if (keyData >= Keys.D0 && keyData <= Keys.D9)
			{
				num = (int)(keyData - 48);
			}
			num = (num + 9) % 10;
			if (num >= 0 && num < mPenManager.FunctionList.Count)
			{
				EditFunctionBase pen = mPenManager.FunctionList[num];
				ActionPenSelect(pen);
			}
			break;
		}
		case Keys.Z:
			miPaletteSelect0.PerformClick();
			break;
		case Keys.X:
			miPaletteSelect1.PerformClick();
			break;
		case Keys.C:
			miPaletteSelect2.PerformClick();
			break;
		case Keys.V:
			miPaletteSelect3.PerformClick();
			break;
		case Keys.N:
			miPalettePrev.PerformClick();
			break;
		case Keys.M:
			miPaletteNext.PerformClick();
			break;
		default:
			result = false;
			break;
		}
		return result;
	}

	private void scrollPanel_KeyDown(object sender, KeyEventArgs e)
	{
		bool flag2 = (e.Handled = ProcessKey_BankView(e.KeyData));
	}

	private bool ProcessKey_BankView(Keys keyData)
	{
		bool result = true;
		switch (keyData)
		{
		case Keys.Home:
			miAddress0.PerformClick();
			break;
		case Keys.Prior:
			miAddress1.PerformClick();
			break;
		case Keys.Up:
			miAddress2.PerformClick();
			break;
		case Keys.Left:
			miAddress3.PerformClick();
			break;
		case Keys.OemMinus:
			miAddress4.PerformClick();
			break;
		case Keys.Oemplus:
			miAddress5.PerformClick();
			break;
		case Keys.Right:
			miAddress6.PerformClick();
			break;
		case Keys.Down:
			miAddress7.PerformClick();
			break;
		case Keys.Next:
			miAddress8.PerformClick();
			break;
		case Keys.End:
			miAddress9.PerformClick();
			break;
		case Keys.OemOpenBrackets:
			tbAddressJumpListPrev.PerformClick();
			break;
		case Keys.OemCloseBrackets:
			tbAddressJumpListNext.PerformClick();
			break;
		case Keys.Up | Keys.Shift:
			tbEditShiftUp.PerformClick();
			break;
		case Keys.Left | Keys.Shift:
			tbEditShiftLeft.PerformClick();
			break;
		case Keys.Right | Keys.Shift:
			tbEditShiftRight.PerformClick();
			break;
		case Keys.Down | Keys.Shift:
			tbEditShiftDown.PerformClick();
			break;
		default:
			result = false;
			break;
		}
		return result;
	}

	private void pictTabControl_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (TabIsRom)
		{
			mSourceBytemap = mRomBytemap;
			mSourceBitmap = mRomBitmap;
			mActiveSelector = cellSelectorRom;
		}
		else if (TabIsBmp)
		{
			mSourceBytemap = mBmpBytemap;
			mSourceBitmap = mBmpBitmap;
			mActiveSelector = cellSelectorBmp;
		}
		else if (TabIsWorkspace)
		{
			mSourceBytemap = workSpaceSelector1.Bytemap;
			mSourceBitmap = workSpaceSelector1.Bitmap;
			mActiveSelector = workSpaceSelector1;
			workSpaceSelector1.UpdateSelectorVisibleBySelection();
		}
		else
		{
			mSourceBytemap = mRomBytemap;
			mSourceBitmap = mRomBitmap;
			mActiveSelector = cellSelectorRom;
		}
		mActiveSelector.DefaultSelectSize = mSetting.EditRectSize;
		mSourceRect = mActiveSelector.SelectedRect;
		SetControlFocus();
		UpdateUndoMenu();
		UpdateAddressMenuEnabled();
		UpdateFileMenuEnabled();
		PaletteMode paletteMode = PaletteMode.Dat;
		if (TabIsRom)
		{
			paletteMode = mRomTabPalMode;
			_ = mRomTabPalIndex;
		}
		else if (TabIsBmp)
		{
			paletteMode = mBmpTabPalMode;
			_ = mBmpTabPalIndex;
		}
		if (paletteMode == PaletteMode.Dat)
		{
			miPaletteTypeDat.PerformClick();
		}
		if (paletteMode == PaletteMode.Pal)
		{
			miPaletteTypePal.PerformClick();
		}
		if (paletteMode == PaletteMode.Bmp)
		{
			miPaletteTypeBmp.Enabled = true;
			miPaletteTypeBmp.PerformClick();
		}
		UpdatePalMode(paletteMode);
		UpdateFormSize();
		UpdateStatusbarVisible();
		SetBytemapPalette();
		RefreshBitmapFromBytemap();
	}

	private void cellSelector_Selected(object sender, MouseEventArgs e)
	{
		OnSelectedRect();
	}

	private void OnSelectedRect()
	{
		mSourceRect = mActiveSelector.SelectedRect;
		mShowClipboard = mActiveSelector.FreeSelect;
		if (mShowClipboard && mActiveSelector.MouseDownNew)
		{
			mActiveSelector.SetMouseDownNew(state: false);
			RefreshClipboardImage();
		}
		UpdateEditPanelSource();
		UpdateStatusbarSelection(mSourceRect);
		UpdateEditRotateMenu();
	}

	private void ActionBankWheelSizeChange(object sender, MouseEventArgs e)
	{
		int delta = 0;
		if (e.Delta < 0)
		{
			delta = -1;
		}
		if (e.Delta > 0)
		{
			delta = 1;
		}
		SelectEditSizeItem(delta);
	}

	private void RefreshClipboardImage()
	{
		if (mClipboardImage != null)
		{
			mClipboardImage.Dispose();
			mClipboardImage = null;
		}
		try
		{
			Bitmap bitmapFromClipboard = ClipboardEx.GetBitmapFromClipboard();
			if (bitmapFromClipboard != null)
			{
				mClipboardImage = bitmapFromClipboard;
			}
			else
			{
				mClipboardImage = null;
			}
		}
		catch
		{
			mClipboardImage = null;
		}
	}

	private void UpdateEditPanelSource()
	{
		editPanel.ShowClipboard = mShowClipboard;
		if (mShowClipboard)
		{
			if (mClipboardImage != null)
			{
				editPanel.ClipboardBitmap = mClipboardImage;
			}
			else
			{
				editPanel.ClipboardBitmap = null;
			}
		}
		else
		{
			editPanel.ClipboardBitmap = null;
		}
		if (editPanel.SourceBytemap != mSourceBytemap)
		{
			editPanel.SourceBytemap = mSourceBytemap;
		}
		if (editPanel.SourceBitmap != mSourceBitmap)
		{
			editPanel.SourceBitmap = mSourceBitmap;
		}
		editPanel.SourceRect = mSourceRect;
		editPanel.Refresh();
	}

	private void editPanel_MouseDown(object sender, MouseEventArgs e)
	{
		mEditingMouseButton |= e.Button;
		if (mEditingMouseButton != 0)
		{
			DisableKeyInput = true;
		}
		if (e.Button == MouseButtons.Left && !scrollPanelRom.Focused && !scrollPanelBmp.Focused)
		{
			SetControlFocus();
		}
	}

	private void editPanel_MouseUp(object sender, MouseEventArgs e)
	{
		if ((mEditingMouseButton & e.Button) == e.Button)
		{
			mEditingMouseButton ^= e.Button;
		}
		if (mEditingMouseButton == MouseButtons.None)
		{
			DisableKeyInput = false;
		}
	}

	private void Edited(object sender, EventArgs e)
	{
		if (!editPanel.EditFunction.EditingFlag)
		{
			ConvertBytemapToFile();
			if (TabIsRom)
			{
				ConvertFileDataToBytemap();
			}
			ConvertBytemapToBitmap();
		}
		if (TabIsRom)
		{
			cellSelectorRom.Refresh();
		}
		if (TabIsBmp)
		{
			cellSelectorBmp.Refresh();
		}
	}

	private void editPanel_OnPicked(object sender, EventArgs e)
	{
		int selectedPalette = editPanel.EditFunction.SelectedPalette;
		int num;
		if (IsColMode)
		{
			num = selectedPalette;
		}
		else
		{
			int num2 = datPaletteSelector.SelectedSet * datPaletteSelector.SetSize;
			int paletteIndexInSet = GetPaletteIndexInSet(selectedPalette);
			num = (num2 + paletteIndexInSet) % 256;
		}
		datPaletteSelector.SelectedIndex = (byte)num;
		RefreshDatSelector();
		UpdatePalSelectorSelection();
	}

	private void EditPanel_MouseWheel(object sender, MouseEventArgs e)
	{
		int num = e.Delta;
		if (num == 0)
		{
			return;
		}
		if (num > 0)
		{
			num = 1;
		}
		if (num < 0)
		{
			num = -1;
		}
		EditorMouseWheelFunction editorMouseWheelFunction = EditorMouseWheelFunction.None;
		editorMouseWheelFunction = (KeyState.Control ? mSetting.EditorMouseWheelCtrl : (KeyState.Shift ? mSetting.EditorMouseWheelShift : ((!KeyState.Alt) ? mSetting.EditorMouseWheel : mSetting.EditorMouseWheelAlt)));
		if (editorMouseWheelFunction == EditorMouseWheelFunction.PaletteSelect)
		{
			if (num > 0)
			{
				miPalettePrev.PerformClick();
			}
			else
			{
				miPaletteNext.PerformClick();
			}
		}
		if (editorMouseWheelFunction == EditorMouseWheelFunction.PenSelect)
		{
			EditFunctionBase pen = mPenManager.SelectFunctionAdd(num, IsColMode);
			ActionPenSelect(pen);
		}
		if (editorMouseWheelFunction == EditorMouseWheelFunction.BankScroll)
		{
			if (num > 0)
			{
				miAddress2.PerformClick();
			}
			else
			{
				miAddress7.PerformClick();
			}
		}
		if (editorMouseWheelFunction == EditorMouseWheelFunction.EditorSizeSelect)
		{
			SelectEditSizeItem(num);
		}
	}

	private void UpdatePalSelectorSelection()
	{
		DatInfo datInfo = mDataFileManager.DatInfo;
		palPaletteSelector.SelectedIndex = datInfo.Data[datPaletteSelector.SelectedIndex];
		RefreshPalSelector();
	}

	private void ActionPalPaletteSelected(object sender, EventArgs e)
	{
		int selectedIndex = palPaletteSelector.SelectedIndex;
		int selectedIndex2 = datPaletteSelector.SelectedIndex;
		mDataFileManager.DatInfoNes.Data[selectedIndex2] = (byte)selectedIndex;
		mDataFileManager.DatInfoNes.Modified = true;
	}

	private void ActionDatPaletteSelected(object sender, EventArgs e)
	{
		AfterDatPaletteSelected();
	}

	private void AfterDatPaletteSelected()
	{
		SetPenPaletteFromDatSelector();
		UpdatePalSelectorSelection();
		UpdateDatPalette();
	}

	private void SetPenPaletteFromDatSelector()
	{
		int selectedSet = datPaletteSelector.SelectedSet;
		int selectedIndex = datPaletteSelector.SelectedIndex;
		int num = ((!IsColMode) ? GetPaletteIndexInSet(selectedIndex) : selectedIndex);
		editPanel.EditFunction.SelectedPalette = (byte)num;
		editPanel.EditFunction.SelectedPaletteSet = (byte)selectedSet;
	}

	private int GetPaletteIndexInSet(int index)
	{
		if (datPaletteSelector.SetSize != 0)
		{
			return index % datPaletteSelector.SetSize;
		}
		return index;
	}

	private void ActionPaletteSetChanged(object sender, EventArgs e)
	{
		AfterPaletteSetChanged();
	}

	private void AfterPaletteSetChanged()
	{
		SetBytemapPalette();
		RefreshBitmapFromBytemap();
	}

	private void ActionPaletteSelector_MouseDown(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left && !scrollPanelRom.Focused && !scrollPanelBmp.Focused)
		{
			SetControlFocus();
		}
	}

	private void ActionPopupRgbEditorFromDat(object sender, EventArgs e)
	{
		int selectedIndex = datPaletteSelector.SelectedIndex;
		int palIndex = mDataFileManager.DatInfo.Data[selectedIndex];
		PopupPaletteEditor(palIndex, datPaletteSelector);
	}

	private void ActionPopupRgbEditorFromPal(object sender, EventArgs e)
	{
		int selectedIndex = palPaletteSelector.SelectedIndex;
		PopupPaletteEditor(selectedIndex, palPaletteSelector);
	}

	private void PopupPaletteEditor(int palIndex, Control parentControl)
	{
		Point point = parentControl.PointToScreen(new Point(0, 0));
		if (!mPaletteEditorForm.Visible)
		{
			mPaletteEditorForm.Location = new Point(point.X - 4, point.Y - 4 - mPaletteEditorForm.Height);
			mPaletteEditorForm.Index = palIndex;
			mPaletteEditorForm.ColorBit.CopyAllFrom(mDataFileManager.PalInfo.ColorBits[palIndex]);
			mPaletteEditorForm.ReadOnly = !mSetting.EnableEditorForReadOnlyFile && mDataFileManager.PalInfo.ReadOnly;
			mPaletteEditorForm.DialogResult = DialogResult.None;
			mPaletteEditorForm.VisibleChanged += mPaletteEditorForm_Closed;
			mPaletteEditorForm.Show();
		}
	}

	private void mPaletteEditorForm_Closed(object sender, EventArgs e)
	{
		if (mPaletteEditorForm.Visible)
		{
			return;
		}
		mPaletteEditorForm.VisibleChanged -= mPaletteEditorForm_Closed;
		if (mPaletteEditorForm.DialogResult == DialogResult.OK)
		{
			int index = mPaletteEditorForm.Index;
			mDataFileManager.PalInfo.ColorBits[index].CopyAllFrom(mPaletteEditorForm.ColorBit);
			if (mDataFileManager.PaletteMode == PaletteMode.Bmp)
			{
				UpdatePalette_BmpFromPal(mDataFileManager.PalInfoBmp, mBmpBytemap);
			}
			UpdatePalPalette();
			UpdateDatPalette();
			SetBytemapPalette();
			RedrawFormat();
		}
	}

	private void ActionShowFormatInfo(object sender, EventArgs e)
	{
		if (mFormat != null)
		{
			MsgBox.Show(this, mFormat.GetFormatInfo(), mFormat.Name, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}
	}

	private void UpdateMenuAll()
	{
		UpdateMenuFileOpened();
		UpdateUndoMenu();
		UpdateClipboardMenu();
		UpdateEditRotateMenu();
		UpdateAddressMenuEnabled();
		UpdateFileMenuEnabled();
		UpdateMenuOption();
		UpdateFilenameMenu();
	}

	private void UpdateMenuFileOpened()
	{
		DataFileBase romData = mDataFileManager.RomData;
		bool enabled = romData.Data != null;
		miFileSaveAsRom.Enabled = enabled;
		bool enabled2 = romData.Data != null && romData.Exist;
		miFileReload.Enabled = enabled2;
		miFileSaveRom.Enabled = enabled2;
		tbFileSaveRom.Enabled = enabled2;
		miOptionExecuteFile.Enabled = enabled2;
		tbAddressJumpList.Enabled = enabled2;
	}

	private void UpdateFileMenuEnabled()
	{
		bool tabIsRom = TabIsRom;
		miFileOpenRom.Enabled = tabIsRom;
		tbFileOpenRom.Enabled = tabIsRom;
		miFileReload.Enabled = tabIsRom;
		miFileSaveRom.Enabled = tabIsRom;
		tbFileSaveRom.Enabled = tabIsRom;
		miFileSaveAsRom.Enabled = tabIsRom;
		tbFileQuickSaveBitmap.Enabled = tabIsRom;
		miFileQuickSaveBitmap.Enabled = tabIsRom;
		tbFileOpenRom.Visible = tabIsRom;
		tbFileSaveRom.Visible = tabIsRom;
		tbFileOpenBmp.Visible = !tabIsRom;
		tbFileSaveBmp.Visible = !tabIsRom;
	}

	private void UpdateFilenameMenu()
	{
		string dataFilenameName = Utility.GetDataFilenameName(mDataFileManager.RomData.FileName, "bmp");
		string dataFilenameName2 = Utility.GetDataFilenameName(mDataFileManager.RomData.FileName, "pal");
		string dataFilenameName3 = Utility.GetDataFilenameName(mDataFileManager.RomData.FileName, "dat");
		string dataFilenameName4 = Utility.GetDataFilenameName(mDataFileManager.RomData.FileName, "adf");
		string resourceString = ResourceUtility.GetResourceString(ResourceType, "Resources.MenuTextFileSaveBmp");
		string resourceString2 = ResourceUtility.GetResourceString(ResourceType, "Resources.MenuTextPaletteSavePal");
		string resourceString3 = ResourceUtility.GetResourceString(ResourceType, "Resources.MenuTextPaletteSaveDat");
		string resourceString4 = ResourceUtility.GetResourceString(ResourceType, "Resources.MenuTextPaletteSaveAdf");
		miFileQuickSaveBitmap.Text = resourceString.Replace("%F", dataFilenameName);
		tbFileQuickSaveBitmap.Text = resourceString.Replace("%F", dataFilenameName);
		miPaletteQuickSaveRGBPalette.Text = resourceString2.Replace("%F", dataFilenameName2);
		miPaletteQuickSavePaletteTable.Text = resourceString3.Replace("%F", dataFilenameName3);
		miPaletteQuickSaveADFPattern.Text = resourceString4.Replace("%F", dataFilenameName4);
		CreateMenuCommonData();
	}

	private void CreateMenuCommonData()
	{
		string path = Utility.GetExeDirectory() + "\\CommonData";
		bool flag = Directory.Exists(path);
		miPaletteLoadRGBPaletteFromCommon.Enabled = flag;
		miPaletteLoadPaletteTableFromCommon.Enabled = flag;
		miPaletteLoadADFPatternFromCommon.Enabled = flag;
		if (flag)
		{
			string[] files = Directory.GetFiles(path, "*.pal");
			CreateChildMenuItem(miPaletteLoadRGBPaletteFromCommon, files);
			string[] files2 = Directory.GetFiles(path, "*.dat");
			CreateChildMenuItem(miPaletteLoadPaletteTableFromCommon, files2);
			string[] files3 = Directory.GetFiles(path, "*.adf");
			CreateChildMenuItem(miPaletteLoadADFPatternFromCommon, files3);
		}
	}

	private void CreateChildMenuItem(ToolStripMenuItem menuItem, string[] fileList)
	{
		if (fileList != null && fileList.Length >= 1)
		{
			menuItem.DropDownItems.Clear();
			foreach (string text in fileList)
			{
				ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(Path.GetFileName(text));
				toolStripMenuItem.Tag = text;
				toolStripMenuItem.Click += ActionCommonDataLoad;
				menuItem.DropDownItems.Add(toolStripMenuItem);
			}
		}
		else
		{
			menuItem.Enabled = false;
		}
	}

	private void ActionFileNew(object sender, EventArgs e)
	{
		if (TabIsBmp)
		{
			ActionFileNewBmp(sender, e);
		}
		else
		{
			ActionFileNewRom(sender, e);
		}
	}

	private void ActionFileNewRom(object sender, EventArgs e)
	{
		if (!CheckSaveModifiedData())
		{
			mDataFileManager.RomData.CreateNew(16384);
			Rectangle rect = new Rectangle(new Point(0, 0), mSourceBytemap.Size);
			mSourceBytemap.FillRect(rect, 0);
			SetFormat();
			CheckMinimumFileBufferLength();
			Address = 0;
			RedrawFormat();
			mDataFileManager.UnboundsFiles();
			UpdateUndoMenu();
			UpdateFilenameMenu();
			UpdateMenuFileOpened();
		}
	}

	private void ActionFileNewBmp(object sender, EventArgs e)
	{
		Bitmap bitmap = mBmpBitmap;
		Bytemap bytemap = mBmpBytemap;
		bytemap.Clear(0);
		mOpenedBmpFilename = Path.GetFullPath("NewFile.bmp");
		mOpenedBmpModified = false;
		BytemapConvertor.UpdateBitmapPaletteFromBytemap(bitmap, bytemap);
		BytemapConvertor.UpdateBitmapFromBytemap(bitmap, bytemap);
		UpdatePalette_PalFromBmp(mDataFileManager.PalInfoBmp, bytemap);
		mBmpUndoManager.ClearUndoBuffer();
		mBmpUndoManager.CreateUndoBuffer(bytemap.Data);
		UpdatePalPalette();
		UpdateDatPalette();
		SetBytemapPalette();
		RedrawFormat();
		UpdateUndoMenu();
		UpdateFilenameMenu();
		UpdateMenuFileOpened();
	}

	private void ActionFileOpen(object sender, EventArgs e)
	{
		if (CheckSaveModifiedData())
		{
			return;
		}
		if (!string.IsNullOrEmpty(mSetting.RomPath) && Directory.Exists(mSetting.RomPath))
		{
			openFileDialog.InitialDirectory = mSetting.RomPath;
		}
		else
		{
			openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
		}
		string extension = mFormatManager.GetExtension();
		openFileDialog.Filter = "ROM image|" + extension + "|All files (*.*)|*.*";
		DialogResult dialogResult = DialogResult.Cancel;
		try
		{
			base.Activated -= MainForm_Activated;
			dialogResult = openFileDialog.ShowDialog();
		}
		catch
		{
		}
		finally
		{
			base.Activated += MainForm_Activated;
		}
		if (dialogResult == DialogResult.OK)
		{
			string fileName = openFileDialog.FileName;
			OpenFile(fileName, initAddress: true);
			string fullPath = Path.GetFullPath(fileName);
			mSetting.RomPath = Path.GetDirectoryName(fullPath);
		}
		else
		{
			RedrawFormat();
			UpdateUndoMenu();
			UpdateFilenameMenu();
			UpdateMenuFileOpened();
		}
		UpdateMenuPenColSetVisible();
	}

	private void ActionFileReload(object sender, EventArgs e)
	{
		if (CheckReloadModifiedData())
		{
			OpenFile(mDataFileManager.RomData.FileName, initAddress: false);
		}
	}

	private void OpenFile(string filename, bool initAddress)
	{
		try
		{
			mDataFileManager.RomData.LoadFromFile(filename);
			bool enableDetectRomHeaderSize = mSetting.EnableDetectRomHeaderSize;
			mDataFileManager.JumpListFile.DetectHeaderSize = enableDetectRomHeaderSize;
			mDataFileManager.SettingInfo.DetectHeaderSize = enableDetectRomHeaderSize;
			if (enableDetectRomHeaderSize)
			{
				int num = mSetting.DivisionSizeForDetectRomHeaderSize;
				if (num > 65536)
				{
					num = 65536;
				}
				if (num < 256)
				{
					num = 256;
				}
				int romHeaderSize = (int)mDataFileManager.RomData.FileSize % num;
				mDataFileManager.RomHeaderSize = romHeaderSize;
			}
			else
			{
				mDataFileManager.RomHeaderSize = 0;
			}
			mDataFileManager.UnboundsFiles();
			mDataFileManager.AutoLoadFiles(filename);
			FormatBase formatByFilename = mFormatManager.GetFormatByFilename(filename);
			if (formatByFilename != null)
			{
				comboBoxFormat.SelectedItem = formatByFilename;
			}
			SetFormat();
			CheckMinimumFileBufferLength();
			if (initAddress)
			{
				Address = mFormat.GetDataAddress(mDataFileManager.RomData.Data);
			}
			else
			{
				Address = Address;
			}
		}
		catch (Exception ex)
		{
			MsgBox.Show(this, ex.Message, "YY-CHR.NET");
		}
		workSpaceSelector1.Data = mDataFileManager.RomData.Data;
		RedrawFormat();
		SetPenPaletteFromDatSelector();
		UpdateUndoMenu();
		UpdateFilenameMenu();
		UpdateMenuFileOpened();
	}

	private void CheckMinimumFileBufferLength()
	{
		int num = 8192;
		if (mFormat != null)
		{
			num = mFormat.GetBankByteSize();
		}
		if (mDataFileManager.RomData.Data.Length < num)
		{
			mDataFileManager.RomData.SetMinLength(num);
		}
	}

	private void ActionFileSave(object sender, EventArgs e)
	{
		SaveFile(mDataFileManager.RomData.FileName);
	}

	private void ActionFileSaveAs(object sender, EventArgs e)
	{
		string extension = mFormatManager.GetExtension();
		saveFileDialog.Filter = "ROM image|" + extension + "|All files (*.*)|*.*";
		string fileName = Path.GetFileName(mDataFileManager.RomData.FileName);
		saveFileDialog.FileName = fileName;
		string directoryName = Path.GetDirectoryName(mDataFileManager.RomData.FileName);
		saveFileDialog.InitialDirectory = directoryName;
		if (saveFileDialog.ShowDialog() == DialogResult.OK)
		{
			string fileName2 = saveFileDialog.FileName;
			SaveFile(fileName2);
			string fullPath = Path.GetFullPath(fileName2);
			mSetting.RomPath = Path.GetDirectoryName(fullPath);
		}
	}

	private void SaveFile(string filename)
	{
		bool flag = false;
		if (mFormat != null && mDataFileManager.RomData.FileSize < mFormat.GetBankByteSize())
		{
			string resourceString = ResourceUtility.GetResourceString(ResourceType, "Resources.TitleConfirm");
			string resourceString2 = ResourceUtility.GetResourceString(ResourceType, "Resources.ConfirmCutOffSmallFile");
			if (mSetting.SmallFileSaveSize == Settings.ConfigSmallFileSaveSize.ShowDialog)
			{
				switch (MsgBox.Show(this, resourceString2, resourceString, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
				case DialogResult.Cancel:
					return;
				case DialogResult.Yes:
					flag = true;
					break;
				case DialogResult.No:
					flag = false;
					break;
				}
			}
			else if (mSetting.SmallFileSaveSize == Settings.ConfigSmallFileSaveSize.AutoExpand)
			{
				flag = true;
			}
			else if (mSetting.SmallFileSaveSize == Settings.ConfigSmallFileSaveSize.NoExpand)
			{
				flag = false;
			}
		}
		try
		{
			if (!flag)
			{
				mDataFileManager.RomData.SaveToFile(filename, mDataFileManager.RomData.FileSize);
				mDataFileManager.RomData.LoadFromFile(filename, clearUndoBuffer: false);
				CheckMinimumFileBufferLength();
			}
			else
			{
				mDataFileManager.RomData.SaveToFile(filename);
			}
			if (mSetting.SaveClearUndoBuffer)
			{
				if (TabIsRom)
				{
					mDataFileManager.RomData.ClearUndoBuffer();
				}
				else if (TabIsBmp)
				{
					mBmpUndoManager.ClearUndoBuffer();
					mBmpUndoManager.CreateUndoBuffer(mBmpBytemap.Data);
				}
			}
			UpdateUndoMenu();
			mDataFileManager.AutoSaveFiles(filename);
			mRomBytemap.CanUpdatePixel = true;
		}
		catch (Exception ex)
		{
			MsgBox.Show(this, ex.Message, "YY-CHR.NET");
		}
		RedrawFormat();
	}

	private void ActionFileOpenBitmap(object sender, EventArgs e)
	{
		string text = "";
		string text2 = "";
		text = ((string.IsNullOrWhiteSpace(mSetting.BmpPath) || !Directory.Exists(mSetting.BmpPath)) ? Directory.GetCurrentDirectory() : mSetting.BmpPath);
		text2 = GetScreenshotFilename(newFile: false);
		int filterIndex = 1;
		string text3 = "bmp";
		if (mSetting.DefaultImageType == ImageFileType.Bmp)
		{
			filterIndex = 1;
			text3 = "bmp";
		}
		if (mSetting.DefaultImageType == ImageFileType.Png)
		{
			filterIndex = 2;
			text3 = "png";
		}
		openBitmapDialog.FilterIndex = filterIndex;
		openBitmapDialog.DefaultExt = text3;
		openBitmapDialog.InitialDirectory = text;
		openBitmapDialog.FileName = Path.ChangeExtension(text2, text3);
		if (openBitmapDialog.ShowDialog() == DialogResult.OK)
		{
			string fileName = openBitmapDialog.FileName;
			LoadImageFile(fileName);
			if (string.IsNullOrWhiteSpace(mSetting.BmpPath) || !File.Exists(mSetting.BmpPath))
			{
				string fullPath = Path.GetFullPath(fileName);
				mSetting.BmpPath = Path.GetDirectoryName(fullPath);
			}
		}
	}

	private string GetScreenshotFilename(bool newFile)
	{
		string text = Directory.GetCurrentDirectory() + "\\";
		string result = string.Empty;
		for (int i = 0; i <= 999; i++)
		{
			string text2 = "CHR" + i.ToString("D3") + ".bmp";
			string path = text + text2;
			if (newFile)
			{
				result = text2;
				if (!File.Exists(path))
				{
					break;
				}
			}
			else
			{
				if (!File.Exists(path))
				{
					break;
				}
				result = text2;
			}
		}
		return result;
	}

	private void LoadImageFile(string filename)
	{
		if (TabIsBmp)
		{
			LoadBitmapTabBmp(filename);
			UpdatePalPalette();
			UpdateDatPalette();
			SetBytemapPalette();
			RedrawFormat();
		}
		else
		{
			LoadBitmap(filename);
			UpdatePalPalette();
			UpdateDatPalette();
			SetBytemapPalette();
			RedrawFormat();
		}
	}

	private Bytemap GetBytemapFromImageFile(string filename)
	{
		Bytemap bytemap = null;
		string text = "";
		if (File.Exists(filename))
		{
			using Bitmap bitmap = new Bitmap(filename);
			if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				bytemap = new Bytemap(bitmap.Size);
				BytemapConvertor.UpdateBytemapPaletteFromBitmap(bitmap, bytemap);
				BytemapConvertor.UpdateBytemapFromBitmap(bitmap, bytemap);
			}
			else
			{
				text = ResourceUtility.GetResourceString(ResourceType, "Resources.MessageFileNotSupported");
			}
		}
		else
		{
			text = ResourceUtility.GetResourceString(ResourceType, "Resources.MessageFileNotExist");
		}
		if (bytemap == null)
		{
			string resourceString = ResourceUtility.GetResourceString(ResourceType, "Resources.TitleError");
			MsgBox.Show(this, text, resourceString, MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		return bytemap;
	}

	private Bytemap GetBytemapFromImageFileWithoutNotExistsMessage(string filename)
	{
		Bytemap bytemap = null;
		string text = "";
		if (File.Exists(filename))
		{
			using (Bitmap bitmap = new Bitmap(filename))
			{
				if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
				{
					bytemap = new Bytemap(bitmap.Size);
					BytemapConvertor.UpdateBytemapPaletteFromBitmap(bitmap, bytemap);
					BytemapConvertor.UpdateBytemapFromBitmap(bitmap, bytemap);
				}
				else
				{
					text = ResourceUtility.GetResourceString(ResourceType, "Resources.MessageFileNotSupported");
				}
			}
			if (bytemap == null)
			{
				string resourceString = ResourceUtility.GetResourceString(ResourceType, "Resources.TitleError");
				MsgBox.Show(this, text, resourceString, MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}
		return bytemap;
	}

	private void LoadBitmapTabBmp(string filename, bool enableNoExistMsg = true)
	{
		Bytemap bytemap = mBmpBytemap;
		Bytemap bytemap2 = ((!enableNoExistMsg) ? GetBytemapFromImageFileWithoutNotExistsMessage(filename) : GetBytemapFromImageFile(filename));
		if (bytemap2 != null)
		{
			mOpenedBmpFilename = filename;
			if (bytemap2 != null && bytemap != null)
			{
				int num = bytemap2.Width;
				int num2 = bytemap2.Height;
				int num3 = bytemap.Width;
				int num4 = bytemap.Height;
				if (num > num3)
				{
					num = num3;
				}
				if (num2 > num4)
				{
					num2 = num4;
				}
				if (num < num3)
				{
					num3 = num;
				}
				if (num2 < num4)
				{
					num4 = num2;
				}
				Rectangle srcRect = new Rectangle(0, 0, num, num2);
				Rectangle dstRect = new Rectangle(0, 0, num3, num4);
				bytemap.SetPalette(bytemap2.Palette);
				bytemap.Clear(0);
				bytemap.CopyRect(dstRect, bytemap2, srcRect);
			}
			mBmpTabPalMode = PaletteMode.Bmp;
		}
		mOpenedBmpModified = false;
		mBmpUndoManager.ClearUndoBuffer();
		mBmpUndoManager.CreateUndoBuffer(mBmpBytemap.Data);
		UpdatePalette_PalFromBmp(mDataFileManager.PalInfoBmp, bytemap);
	}

	private void LoadBitmap(string filename)
	{
		Bytemap bytemapFromImageFile = GetBytemapFromImageFile(filename);
		Bytemap bytemap = mSourceBytemap;
		bool flag = false;
		if (bytemapFromImageFile != null && bytemap != null)
		{
			int num = bytemapFromImageFile.Width;
			int num2 = bytemapFromImageFile.Height;
			int num3 = bytemap.Width;
			int num4 = bytemap.Height;
			if (num > num3)
			{
				num = num3;
				flag = true;
			}
			if (num2 > num4)
			{
				num2 = num4;
				flag = true;
			}
			if (num < num3)
			{
				num3 = num;
			}
			if (num2 < num4)
			{
				num4 = num2;
			}
			Rectangle srcRect = new Rectangle(0, 0, num, num2);
			Rectangle dstRect = new Rectangle(0, 0, num3, num4);
			bytemap.SetPalette(bytemapFromImageFile.Palette);
			bytemap.Clear(0);
			bytemap.CopyRect(dstRect, bytemapFromImageFile, srcRect);
		}
		if (flag && mSetting.ShowLoadImageCutoffDialog)
		{
			string resourceString = ResourceUtility.GetResourceString(ResourceType, "Resources.TitleWarning");
			string resourceString2 = ResourceUtility.GetResourceString(ResourceType, "Resources.MessageImageSizeNotSupported");
			MsgBox.Show(this, resourceString2, resourceString, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
		if (mSetting.LoadImagePaletteOnChrRomTab)
		{
			UpdatePalette_PalFromBmp(mDataFileManager.PalInfo256, bytemap);
			mRomTabPalMode = PaletteMode.Pal;
		}
		ConvertBytemapToFile();
		ConvertBytemapToBitmap();
		RefreshBitmapFromBytemap();
	}

	private void ActionFileSaveBitmap(object sender, EventArgs e)
	{
		string text = "";
		string text2 = "";
		if (TabIsRom && mSetting.BmpSaveDirType == SaveDirType.OpenedFileDir)
		{
			string fullPath = Path.GetFullPath(mDataFileManager.RomData.FileName);
			if (!string.IsNullOrWhiteSpace(fullPath) && File.Exists(fullPath))
			{
				string dataFilename = Utility.GetDataFilename(fullPath, "bmp");
				text = Path.GetDirectoryName(dataFilename);
				text2 = Path.GetFileName(dataFilename);
			}
		}
		if (TabIsBmp)
		{
			string text3 = mOpenedBmpFilename;
			if (!string.IsNullOrWhiteSpace(text3) && File.Exists(text3))
			{
				text = Path.GetDirectoryName(text3);
				text2 = Path.GetFileName(text3);
			}
		}
		if (mSetting.BmpSaveDirType == SaveDirType.SettingDir && !string.IsNullOrEmpty(mSetting.BmpPath) && Directory.Exists(mSetting.BmpPath))
		{
			text = mSetting.BmpPath;
		}
		if (string.IsNullOrWhiteSpace(text))
		{
			text = Directory.GetCurrentDirectory();
		}
		if (string.IsNullOrWhiteSpace(text2))
		{
			text2 = GetScreenshotFilename(newFile: true);
		}
		int filterIndex = 1;
		string text4 = "bmp";
		if (mSetting.DefaultImageType == ImageFileType.Bmp)
		{
			filterIndex = 1;
			text4 = "bmp";
		}
		if (mSetting.DefaultImageType == ImageFileType.Png)
		{
			filterIndex = 2;
			text4 = "png";
		}
		saveBitmapDialog.FilterIndex = filterIndex;
		saveBitmapDialog.DefaultExt = text4;
		saveBitmapDialog.InitialDirectory = text;
		saveBitmapDialog.FileName = Path.ChangeExtension(text2, text4);
		if (saveBitmapDialog.ShowDialog() == DialogResult.OK)
		{
			string fileName = saveBitmapDialog.FileName;
			ImageFileType imageType = ((saveBitmapDialog.FilterIndex == 2) ? ImageFileType.Png : ImageFileType.Bmp);
			SaveBitmap(fileName, imageType);
			if (string.IsNullOrWhiteSpace(mSetting.BmpPath) || !File.Exists(mSetting.BmpPath))
			{
				string fullPath2 = Path.GetFullPath(fileName);
				mSetting.BmpPath = Path.GetDirectoryName(fullPath2);
			}
		}
	}

	private void SaveBitmap(string filename, ImageFileType imageType)
	{
		if (mSourceBitmap == null || mSourceBytemap == null)
		{
			return;
		}
		using Bitmap bitmap = new Bitmap(mSourceBitmap.Width, mSourceBitmap.Height, PixelFormat.Format8bppIndexed);
		BytemapConvertor.UpdateBitmapAllFromBytemap(bitmap, mSourceBytemap);
		ImageFormat format = ImageFormat.Bmp;
		if (imageType == ImageFileType.Png)
		{
			format = ImageFormat.Png;
		}
		bitmap.Save(filename, format);
		mOpenedBmpFilename = filename;
		mOpenedBmpModified = false;
	}

	private void ActionFileQuickSaveBitmap(object sender, EventArgs e)
	{
		string dataFilename = Utility.GetDataFilename(mDataFileManager.RomData.FileName, "bmp");
		SaveBitmap(dataFilename, ImageFileType.Bmp);
	}

	private void ActionFileExit(object sender, EventArgs e)
	{
		Close();
	}

	private void UpdateUndoMenu()
	{
		bool flag = false;
		bool flag2 = false;
		if (TabIsRom)
		{
			flag = mDataFileManager.RomData.CanUndo;
			flag2 = mDataFileManager.RomData.CanRedo;
		}
		else if (TabIsBmp)
		{
			flag = mBmpUndoManager.CanUndo;
			flag2 = mBmpUndoManager.CanRedo;
		}
		else
		{
			flag = false;
			flag2 = false;
		}
		miEditUndo.Enabled = flag;
		tbEditUndo.Enabled = flag;
		miEditRedo.Enabled = flag2;
		tbEditRedo.Enabled = flag2;
	}

	private void ActionEditUndo(object sender, EventArgs e)
	{
		if (TabIsRom)
		{
			mDataFileManager.RomData.Undo();
		}
		else if (TabIsBmp)
		{
			UndoManager.UndoInfo undoInfo = mBmpUndoManager.Undo();
			if (undoInfo != null)
			{
				for (int i = 0; i < mBmpBytemap.Data.Length; i++)
				{
					mBmpBytemap.Data[i] = undoInfo.Buffer[i];
				}
				mBmpBytemap.CanUpdatePalette = true;
				mBmpBytemap.CanUpdatePixel = true;
			}
		}
		UpdateUndoMenu();
		RedrawFormat();
		mDataFileManager.RomData.Modified = true;
		UpdateTitle();
	}

	private void ActionEditRedo(object sender, EventArgs e)
	{
		if (TabIsRom)
		{
			mDataFileManager.RomData.Redo();
		}
		else if (TabIsBmp)
		{
			UndoManager.UndoInfo undoInfo = mBmpUndoManager.Redo();
			if (undoInfo != null)
			{
				for (int i = 0; i < mBmpBytemap.Data.Length; i++)
				{
					mBmpBytemap.Data[i] = undoInfo.Buffer[i];
				}
				mBmpBytemap.CanUpdatePalette = true;
				mBmpBytemap.CanUpdatePixel = true;
			}
		}
		UpdateUndoMenu();
		RedrawFormat();
		mDataFileManager.RomData.Modified = true;
		UpdateTitle();
	}

	private void ActionEditCopy(object sender, EventArgs e)
	{
		try
		{
			using (MemoryStream data = new MemoryStream(DIB.ConvertFromBitmap(mSourceBitmap, mSourceRect)))
			{
				Clipboard.SetData(DataFormats.Dib, data);
			}
			RefreshClipboardImage();
			mShowClipboard = true;
			UpdateEditPanelSource();
			UpdateClipboardMenu();
		}
		catch
		{
		}
	}

	private void ActionEditCut(object sender, EventArgs e)
	{
		miEditCopy.PerformClick();
		miEditClear.PerformClick();
	}

	private void UpdateClipboardMenu()
	{
		bool enabled = Clipboard.ContainsImage();
		miEditPaste.Enabled = enabled;
		tbEditPaste.Enabled = enabled;
		miEditPasteOptimizedImage.Enabled = enabled;
		miEditClearClipboard.Enabled = enabled;
	}

	private void ActionEditPaste(object sender, EventArgs e)
	{
		Bitmap bitmapFromClipboard = ClipboardEx.GetBitmapFromClipboard();
		if (bitmapFromClipboard == null)
		{
			SystemSounds.Beep.Play();
			return;
		}
		if ((bitmapFromClipboard.PixelFormat == PixelFormat.Format4bppIndexed || bitmapFromClipboard.PixelFormat == PixelFormat.Format8bppIndexed || bitmapFromClipboard.PixelFormat == PixelFormat.Format1bppIndexed) && bitmapFromClipboard.Palette != null)
		{
			_ = bitmapFromClipboard.Palette.Entries.Length;
		}
		_ = mFormat.ColorNum;
		if (bitmapFromClipboard.PixelFormat == PixelFormat.Format8bppIndexed)
		{
			PaseWithoutOptimize(bitmapFromClipboard);
		}
		else
		{
			PasteOptimizedImage(bitmapFromClipboard);
		}
		bitmapFromClipboard.Dispose();
	}

	private void ActionEditPasteWithOptimise(object sender, EventArgs e)
	{
		Bitmap bitmapFromClipboard = ClipboardEx.GetBitmapFromClipboard();
		if (bitmapFromClipboard == null)
		{
			SystemSounds.Beep.Play();
			return;
		}
		PasteOptimizedImage(bitmapFromClipboard);
		bitmapFromClipboard.Dispose();
	}

	private void PaseWithoutOptimize(Bitmap indexedBitmap)
	{
		if (indexedBitmap == null)
		{
			SystemSounds.Beep.Play();
			return;
		}
		try
		{
			if (indexedBitmap == null || indexedBitmap.PixelFormat != PixelFormat.Format8bppIndexed)
			{
				SystemSounds.Beep.Play();
			}
			else
			{
				PasteFromIndexedBitmap(indexedBitmap);
			}
		}
		catch (Exception ex)
		{
			_ = ex.Message;
			SystemSounds.Beep.Play();
		}
	}

	private void PasteOptimizedImage(Bitmap clipboardBitmap)
	{
		if (clipboardBitmap == null)
		{
			SystemSounds.Beep.Play();
			return;
		}
		Size pasteTargetCanvasSize = ((!TabIsBmp) ? new Size(128, 128) : new Size(256, 256));
		try
		{
			Bitmap bitmap = null;
			ColorOptimizeForm colorOptimizeForm = new ColorOptimizeForm();
			colorOptimizeForm.InputBitmap = clipboardBitmap;
			colorOptimizeForm.OutColorBit = mFormat.ColorBit;
			colorOptimizeForm.PasteTargetCanvasSize = pasteTargetCanvasSize;
			if (colorOptimizeForm.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			bitmap = colorOptimizeForm.OutputBitmap;
			if (bitmap == null || bitmap.PixelFormat != PixelFormat.Format8bppIndexed)
			{
				SystemSounds.Beep.Play();
				return;
			}
			PasteFromIndexedBitmap(bitmap);
			if (!colorOptimizeForm.PastePalette)
			{
				return;
			}
			if (bitmap != null && bitmap.Palette != null && bitmap.Palette.Entries != null)
			{
				for (int i = 0; i < colorOptimizeForm.OutColorNum; i++)
				{
					if (i < bitmap.Palette.Entries.Length)
					{
						mDataFileManager.PalInfo.ColorBits[i].A = bitmap.Palette.Entries[i].A;
						mDataFileManager.PalInfo.ColorBits[i].R = bitmap.Palette.Entries[i].R;
						mDataFileManager.PalInfo.ColorBits[i].G = bitmap.Palette.Entries[i].G;
						mDataFileManager.PalInfo.ColorBits[i].B = bitmap.Palette.Entries[i].B;
					}
				}
			}
			if (mDataFileManager.PaletteMode == PaletteMode.Bmp)
			{
				UpdatePalette_BmpFromPal(mDataFileManager.PalInfoBmp, mBmpBytemap);
			}
			UpdatePalPalette();
			UpdateDatPalette();
			SetBytemapPalette();
			RedrawFormat();
		}
		catch (Exception ex)
		{
			_ = ex.Message;
			SystemSounds.Beep.Play();
		}
	}

	private void PasteFromIndexedBitmap(Bitmap cbBitmap)
	{
		Rectangle srcRect = new Rectangle(new Point(0, 0), cbBitmap.Size);
		if (srcRect.Width > mSourceBitmap.Width)
		{
			srcRect.Width = mSourceBitmap.Width;
		}
		if (srcRect.Height > mSourceBitmap.Height)
		{
			srcRect.Height = mSourceBitmap.Height;
		}
		Bytemap bytemap = new Bytemap(cbBitmap.Width, cbBitmap.Height);
		BytemapConvertor.UpdateBytemapFromBitmap(cbBitmap, bytemap);
		if (TabIsRom)
		{
			bytemap.SetPaletteSet((byte)mFormat.ColorNum, 0);
		}
		else
		{
			_ = TabIsBmp;
		}
		mSourceBytemap.CopyRect(mSourceRect.Location, bytemap, srcRect);
		ConvertBytemapToFile();
		ConvertBytemapToBitmap();
		RefreshBitmapFromBytemap();
	}

	private void ActionEditClear(object sender, EventArgs e)
	{
		_ = datPaletteSelector.SelectedIndex;
		mSourceBytemap.FillRect(mSourceRect, 0);
		ConvertBytemapToFile();
		ConvertBytemapToBitmap();
		RefreshBitmapFromBytemap();
	}

	private void ActionEditSelectAll(object sender, EventArgs e)
	{
		Rectangle selectedRect = new Rectangle(new Point(0, 0), mSourceBytemap.Size);
		mActiveSelector.FreeSelect = true;
		mActiveSelector.SelectedRect = selectedRect;
		OnSelectedRect();
		RedrawFormat();
	}

	private void ActionEditClearClipboard(object sender, EventArgs e)
	{
		Clipboard.Clear();
		RedrawFormat();
		UpdateClipboardMenu();
	}

	private void UpdateEditRotateMenu()
	{
		bool enabled = mSourceRect.Width == mSourceRect.Height;
		tbEditRotateLeft.Enabled = enabled;
		miEditRotateLeft.Enabled = enabled;
		tbEditRotateRight.Enabled = enabled;
		miEditRotateRight.Enabled = enabled;
	}

	private void ActionEditRotate(object sender, EventArgs e)
	{
		RotateType rotateType = RotateType.Left;
		if (sender == tbEditRotateLeft || sender == miEditRotateLeft)
		{
			rotateType = RotateType.Left;
		}
		if (sender == tbEditRotateRight || sender == miEditRotateRight)
		{
			rotateType = RotateType.Right;
		}
		mSourceBytemap.Rotate(mSourceRect, rotateType);
		ConvertBytemapToFile();
		ConvertBytemapToBitmap();
		RefreshBitmapFromBytemap();
	}

	private void ActionEditMirror(object sender, EventArgs e)
	{
		MirrorType mirrorType = MirrorType.Horizontal;
		if (sender == tbEditMirrorHorizontal || sender == miEditMirrorHorizontal)
		{
			mirrorType = MirrorType.Horizontal;
		}
		if (sender == tbEditMirrorVertical || sender == miEditMirrorVertical)
		{
			mirrorType = MirrorType.Vertical;
		}
		mSourceBytemap.Mirror(mSourceRect, mirrorType);
		ConvertBytemapToFile();
		ConvertBytemapToBitmap();
		RefreshBitmapFromBytemap();
	}

	private void ActionEditShift(object sender, EventArgs e)
	{
		ShiftType shiftType = ShiftType.Left;
		if (sender == miEditShiftUp || sender == tbEditShiftUp)
		{
			shiftType = ShiftType.Up;
		}
		if (sender == miEditShiftDown || sender == tbEditShiftDown)
		{
			shiftType = ShiftType.Down;
		}
		if (sender == miEditShiftLeft || sender == tbEditShiftLeft)
		{
			shiftType = ShiftType.Left;
		}
		if (sender == miEditShiftRight || sender == tbEditShiftRight)
		{
			shiftType = ShiftType.Right;
		}
		mSourceBytemap.Shift(mSourceRect, shiftType);
		ConvertBytemapToFile();
		ConvertBytemapToBitmap();
		RefreshBitmapFromBytemap();
	}

	private void ActionEditReplaceColor(object sender, EventArgs e)
	{
		ReplaceColorForm replaceColorForm = new ReplaceColorForm();
		replaceColorForm.FromPaletteSet = datPaletteSelector.SelectedSet;
		replaceColorForm.FromSize = datPaletteSelector.SetSize;
		replaceColorForm.DatInfo = mDataFileManager.DatInfo;
		replaceColorForm.Bytemap = mSourceBytemap;
		replaceColorForm.SelectedRect = mSourceRect;
		if (replaceColorForm.ShowDialog() == DialogResult.OK)
		{
			mSourceBytemap.ReplaceColor(mSourceRect, replaceColorForm.ToPalette);
			ConvertBytemapToFile();
			ConvertBytemapToBitmap();
			RefreshBitmapFromBytemap();
		}
	}

	private void ActionViewGridPict(object sender, EventArgs e)
	{
		mSetting.GridBankVisible = !mSetting.GridBankVisible;
		UpdateGrid();
	}

	private void ActionViewGridEdit(object sender, EventArgs e)
	{
		mSetting.GridEditVisible = !mSetting.GridEditVisible;
		UpdateGrid();
	}

	private void ActionPaletteType(object sender, EventArgs e)
	{
		if (sender is ToolStripItem)
		{
			PaletteMode palMode = (PaletteMode)Convert.ToInt32(((ToolStripItem)sender).Tag);
			if (mDataFileManager.PaletteMode == PaletteMode.Bmp)
			{
				mBmpTabPalIndex = (byte)datPaletteSelector.SelectedIndex;
			}
			else
			{
				mRomTabPalIndex = (byte)datPaletteSelector.SelectedIndex;
			}
			if (TabIsRom)
			{
				mRomTabPalMode = palMode;
			}
			if (TabIsBmp)
			{
				mBmpTabPalMode = palMode;
			}
			if (UpdatePalMode(palMode))
			{
				UpdateFormSize();
			}
		}
	}

	private bool UpdatePalMode(PaletteMode palMode)
	{
		bool result = false;
		mDataFileManager.PaletteMode = palMode;
		if (palPaletteSelector.CanSelect && palPaletteSelector.Focused && datPaletteSelector.CanSelect)
		{
			datPaletteSelector.Focus();
		}
		bool flag;
		int num;
		int setSize;
		LabelStyle labelStyle;
		bool showSetRect;
		switch (palMode)
		{
		default:
			flag = true;
			num = 4;
			setSize = mFormat.ColorNum;
			labelStyle = LabelStyle.SelectedSet;
			showSetRect = true;
			break;
		case PaletteMode.Pal:
			flag = false;
			num = ((mActiveSelector == null || mActiveSelector.Height > 256) ? 16 : mSetting.DefaultPaletteRowNum);
			setSize = mFormat.ColorNum;
			labelStyle = LabelStyle.Selected;
			showSetRect = true;
			mDataFileManager.DatInfo256.PalInfo = mDataFileManager.PalInfo;
			break;
		case PaletteMode.Bmp:
			flag = false;
			num = 16;
			setSize = 256;
			labelStyle = LabelStyle.Selected;
			showSetRect = false;
			mDataFileManager.DatInfo256.PalInfo = mDataFileManager.PalInfo;
			break;
		}
		if (palPaletteSelector.Visible != flag)
		{
			palPaletteSelector.Visible = flag;
			palPaletteSelector.CellColumnView = 16;
			result = true;
		}
		if (datPaletteSelector.CellRowView != num)
		{
			datPaletteSelector.CellRowView = num;
			result = true;
		}
		datPaletteSelector.SetSize = setSize;
		datPaletteSelector.LabelStyle = labelStyle;
		datPaletteSelector.ShowSetRect = showSetRect;
		if (mDataFileManager.PaletteMode == PaletteMode.Bmp)
		{
			SelectDatPalette(mBmpTabPalIndex);
		}
		else
		{
			SelectDatPalette(mRomTabPalIndex);
		}
		UpdateMenuPaletteMode(palMode);
		UpdateDatPaletteLabel();
		UpdatePalPalette();
		UpdateDatPalette();
		SetBytemapPalette();
		RedrawFormat();
		return result;
	}

	private void UpdatePalette_PalFromBmp(PalInfo palInfo, Bytemap bytemap)
	{
		for (int i = 0; i < palInfo.ColorBits.Length; i++)
		{
			Color color = ((i >= bytemap.Palette.Length) ? Color.Black : bytemap.Palette[i]);
			palInfo.ColorBits[i] = new ColorBit(PaletteType.R8G8B8, byte.MaxValue, color.R, color.G, color.B);
		}
	}

	private void UpdatePalette_BmpFromPal(PalInfo palInfo, Bytemap bytemap)
	{
		ColorBit[] colorBits = palInfo.ColorBits;
		for (int i = 0; i < palInfo.ColorBits.Length; i++)
		{
			if (i < bytemap.Palette.Length)
			{
				bytemap.Palette[i] = colorBits[i].Color;
			}
		}
	}

	private void UpdateMenuPaletteMode(PaletteMode palMode)
	{
		bool tabIsBmp = TabIsBmp;
		bool @checked = palMode == PaletteMode.Dat;
		bool checked2 = palMode == PaletteMode.Pal;
		bool checked3 = palMode == PaletteMode.Bmp;
		tbPaletteTypeDat.Checked = @checked;
		miPaletteTypeDat.Checked = @checked;
		tbPaletteTypePal.Checked = checked2;
		miPaletteTypePal.Checked = checked2;
		miPaletteTypeBmp.Checked = checked3;
		tbPaletteTypeBmp.Checked = checked3;
		miPaletteTypeBmp.Enabled = tabIsBmp;
		tbPaletteTypeBmp.Enabled = tabIsBmp;
	}

	private void ActionPaletteSelectPrevious(object sender, EventArgs e)
	{
		SelectDatPalette(datPaletteSelector.SelectedIndex + 255);
	}

	private void ActionPaletteSelectNext(object sender, EventArgs e)
	{
		SelectDatPalette(datPaletteSelector.SelectedIndex + 1);
	}

	private void ActionPaletteSelect(object sender, EventArgs e)
	{
		int num = Convert.ToInt32(((ToolStripMenuItem)sender).Tag);
		int num2 = datPaletteSelector.SelectedSet * datPaletteSelector.SetSize;
		SelectDatPalette(num2 + num);
	}

	private void SelectDatPalette(int datIndex)
	{
		datPaletteSelector.SelectPaletteWithScroll((byte)(datIndex % 256));
		AfterDatPaletteSelected();
		AfterPaletteSetChanged();
	}

	private bool OpenDataFileDialog(DataFileBase datafile, string initialDir)
	{
		try
		{
			string fileName = mDataFileManager.RomData.FileName;
			string filter = datafile.DataName + " (*." + datafile.Extension + ")|*." + datafile.Extension + "|All files (*.*)|*.*";
			string directoryName = Path.GetDirectoryName(fileName);
			string text = Path.GetFileNameWithoutExtension(fileName) + "." + datafile.Extension;
			openDataFileDialog.Filter = filter;
			if (!string.IsNullOrEmpty(initialDir) && Directory.Exists(initialDir))
			{
				openDataFileDialog.InitialDirectory = initialDir;
			}
			else
			{
				openDataFileDialog.InitialDirectory = directoryName;
			}
			if (File.Exists(directoryName + "\\" + text))
			{
				openDataFileDialog.FileName = text;
			}
			else
			{
				openDataFileDialog.FileName = string.Empty;
			}
			if (openDataFileDialog.ShowDialog() == DialogResult.OK)
			{
				string fileName2 = openDataFileDialog.FileName;
				datafile.LoadFromFile(fileName2);
				return true;
			}
			return false;
		}
		catch (NotSupportedException)
		{
			string caption = "YY-CHR.NET";
			string resourceString = ResourceUtility.GetResourceString(ResourceType, "Resources.LoadingDataFileErrorNotSupport");
			MsgBox.Show(this, resourceString, caption, MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return false;
		}
		catch (Exception)
		{
			string caption2 = "YY-CHR.NET";
			string resourceString2 = ResourceUtility.GetResourceString(ResourceType, "Resources.LoadingDataFileError");
			MsgBox.Show(this, resourceString2, caption2, MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return false;
		}
	}

	private bool SaveDataFileDialog(DataFileBase datafile, string initialDir)
	{
		string fileName = mDataFileManager.RomData.FileName;
		string filter = datafile.DataName + " (*." + datafile.Extension + ")|*." + datafile.Extension + "|All files (*.*)|*.*";
		string directoryName = Path.GetDirectoryName(fileName);
		string fileName2 = Path.GetFileNameWithoutExtension(fileName) + "." + datafile.Extension;
		saveDataFileDialog.Filter = filter;
		if (!string.IsNullOrEmpty(initialDir) && Directory.Exists(initialDir))
		{
			saveDataFileDialog.InitialDirectory = initialDir;
		}
		else
		{
			saveDataFileDialog.InitialDirectory = directoryName;
		}
		saveDataFileDialog.FileName = fileName2;
		if (saveDataFileDialog.ShowDialog() == DialogResult.OK)
		{
			string fileName3 = saveDataFileDialog.FileName;
			datafile.SaveToFile(fileName3);
			return true;
		}
		return false;
	}

	private void ActionCommonDataLoad(object sender, EventArgs e)
	{
		try
		{
			string text = (string)(sender as ToolStripMenuItem).Tag;
			string obj = Path.GetExtension(text).ToLower();
			if (obj.Contains("pal"))
			{
				mDataFileManager.PalInfo.LoadFromFile(text);
				UpdatePalPalette();
			}
			if (obj.Contains("dat"))
			{
				mDataFileManager.DatInfoNes.LoadFromFile(text);
			}
			if (obj.Contains("adf"))
			{
				mDataFileManager.AdfPattern.LoadFromFile(text);
			}
		}
		catch (Exception)
		{
		}
	}

	private void ActionPaletteLoadEmulatorState(object sender, EventArgs e)
	{
		if (OpenDataFileDialog(mDataFileManager.DataFileCustom, mSetting.StatePath))
		{
			RedrawFormat();
			RefreshPalSelector();
			RefreshDatSelector();
			string fullPath = Path.GetFullPath(mDataFileManager.DataFileCustom.FileName);
			mSetting.StatePath = Path.GetDirectoryName(fullPath);
		}
	}

	private void ActionPaletteOpenPal(object sender, EventArgs e)
	{
		if (OpenDataFileDialog(mDataFileManager.PalInfo, mSetting.ExtPath))
		{
			string fullPath = Path.GetFullPath(mDataFileManager.PalInfo.FileName);
			mSetting.ExtPath = Path.GetDirectoryName(fullPath);
			UpdatePalPalette();
			UpdateDatPalette();
			SetBytemapPalette();
			RefreshBitmapFromBytemap();
		}
	}

	private void ActionPaletteSavePal(object sender, EventArgs e)
	{
		if (SaveDataFileDialog(mDataFileManager.PalInfo, mSetting.ExtPath))
		{
			string fullPath = Path.GetFullPath(mDataFileManager.PalInfo.FileName);
			mSetting.ExtPath = Path.GetDirectoryName(fullPath);
		}
	}

	private void ActionPaletteQuickSavePal(object sender, EventArgs e)
	{
		string dataFilename = Utility.GetDataFilename(mDataFileManager.RomData.FileName, "pal");
		mDataFileManager.PalInfo.SaveToFile(dataFilename);
	}

	private void ActionPaletteOpenDat(object sender, EventArgs e)
	{
		if (OpenDataFileDialog(mDataFileManager.DatInfoNes, mSetting.ExtPath))
		{
			RefreshDatSelector();
			string fullPath = Path.GetFullPath(mDataFileManager.DatInfoNes.FileName);
			mSetting.ExtPath = Path.GetDirectoryName(fullPath);
			UpdatePalPalette();
			UpdateDatPalette();
			SetBytemapPalette();
			RefreshBitmapFromBytemap();
		}
	}

	private void ActionPaletteSaveDat(object sender, EventArgs e)
	{
		if (SaveDataFileDialog(mDataFileManager.DatInfoNes, mSetting.ExtPath))
		{
			string fullPath = Path.GetFullPath(mDataFileManager.DatInfoNes.FileName);
			mSetting.ExtPath = Path.GetDirectoryName(fullPath);
		}
	}

	private void ActionPaletteQuickSaveDat(object sender, EventArgs e)
	{
		string dataFilename = Utility.GetDataFilename(mDataFileManager.RomData.FileName, "dat");
		mDataFileManager.DatInfoNes.SaveToFile(dataFilename);
	}

	private void ActionPaletteOpenAdf(object sender, EventArgs e)
	{
		if (OpenDataFileDialog(mDataFileManager.AdfPattern, mSetting.ExtPath))
		{
			string fullPath = Path.GetFullPath(mDataFileManager.AdfPattern.FileName);
			mSetting.ExtPath = Path.GetDirectoryName(fullPath);
		}
	}

	private void ActionPaletteSaveAdf(object sender, EventArgs e)
	{
		if (SaveDataFileDialog(mDataFileManager.AdfPattern, mSetting.ExtPath))
		{
			string fullPath = Path.GetFullPath(mDataFileManager.AdfPattern.FileName);
			mSetting.ExtPath = Path.GetDirectoryName(fullPath);
		}
	}

	private void ActionPaletteQuickSaveAdf(object sender, EventArgs e)
	{
		string dataFilename = Utility.GetDataFilename(mDataFileManager.RomData.FileName, "adf");
		mDataFileManager.AdfPattern.SaveToFile(dataFilename);
	}

	private void actionPaletteLoadDefault(object sender, EventArgs e)
	{
		LoadDefaultExtFiles();
	}

	private void UpdateAddressMenuEnabled()
	{
		bool tabIsRom = TabIsRom;
		miAddress.Enabled = tabIsRom;
		foreach (ToolStripItem dropDownItem in miAddress.DropDownItems)
		{
			dropDownItem.Enabled = tabIsRom;
		}
	}

	private void ActionAddresChange(object sender, EventArgs e)
	{
		if (TabIsRom && mDataFileManager.RomData.Data != null)
		{
			int num;
			try
			{
				num = Convert.ToInt32(((ToolStripItem)sender).Tag);
			}
			catch
			{
				num = 0;
			}
			AddressChange addressChange = AddressChange.None;
			if (num == 0)
			{
				addressChange = AddressChange.Begin;
			}
			if (num == 1)
			{
				addressChange = AddressChange.BlockM100;
			}
			if (num == 2)
			{
				addressChange = AddressChange.BlockM10;
			}
			if (num == 3)
			{
				addressChange = AddressChange.BlockM1;
			}
			if (num == 4)
			{
				addressChange = AddressChange.ByteM1;
			}
			if (num == 5)
			{
				addressChange = AddressChange.ByteP1;
			}
			if (num == 6)
			{
				addressChange = AddressChange.BlockP1;
			}
			if (num == 7)
			{
				addressChange = AddressChange.BlockP10;
			}
			if (num == 8)
			{
				addressChange = AddressChange.BlockP100;
			}
			if (num == 9)
			{
				addressChange = AddressChange.End;
			}
			if (num == 0)
			{
				int num2 = (Address = mFormat.GetDataAddress(mDataFileManager.RomData.Data));
				return;
			}
			int address = Address;
			int num3 = (Address = mFormat.GetAddress(address, mDataFileManager.RomData.Data.Length, addressChange));
		}
	}

	private void ActionAddressInputAddress(object sender, EventArgs e)
	{
		if (TabIsRom)
		{
			AddressInputForm addressInputForm = new AddressInputForm();
			addressInputForm.Address = Address;
			if (addressInputForm.ShowDialog(this) == DialogResult.OK)
			{
				Address = addressInputForm.Address;
			}
			if (scrollPanelRom.CanSelect)
			{
				scrollPanelRom.Focus();
			}
		}
	}

	private void ActionAddressFindPrevious(object sender, EventArgs e)
	{
		if (TabIsRom && !mFindWorker.IsBusy)
		{
			mFindData = mDataFileManager.RomData.Data;
			mFindAddr = Address;
			mFindAddAddr = -1;
			mFindSize = mFindData.Length;
			mFindAdvanceSearch = KeyState.Control;
			mFindWorker.RunWorkerAsync();
		}
	}

	private void ActionAddressFindNext(object sender, EventArgs e)
	{
		if (TabIsRom && !mFindWorker.IsBusy)
		{
			mFindData = mDataFileManager.RomData.Data;
			mFindAddr = Address;
			mFindAddAddr = 1;
			mFindSize = mFindData.Length;
			mFindAdvanceSearch = KeyState.Control;
			mFindWorker.RunWorkerAsync();
		}
	}

	private void mFindWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		if (TabIsRom && mFoundAddr >= 0)
		{
			Address = mFoundAddr;
			slRightSpace.Text = "";
		}
	}

	private void mFindWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
	{
		int num = mFindingAddr;
		int num2 = mFindSize;
		string text = "Find ... " + num.ToString("X") + "/" + num2.ToString("X");
		slRightSpace.Text = text;
	}

	private void mFindWorker_DoWork(object sender, DoWorkEventArgs e)
	{
		int num = -1;
		int charactorByteSize = mFormat.GetCharactorByteSize();
		if (mBytemapForFind == null)
		{
			mBytemapForFind = new Bytemap(mFormat.CharSize);
		}
		Point pt = new Point(0, 0);
		mFindingAddr = mFindAddr + mFindAddAddr;
		while (mFindingAddr >= 0 && mFindingAddr < mFindSize)
		{
			if (mFindingAddr + charactorByteSize < mFindSize && ((mFindingAddr & 0xF) == 0 || mFindAdvanceSearch))
			{
				mFormat.ConvertMemToChr(mFindData, mFindingAddr, mBytemapForFind, pt);
				byte b = mBytemapForFind.Data[0];
				bool flag = true;
				byte[] data = mBytemapForFind.Data;
				foreach (byte b2 in data)
				{
					if (b != b2)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					num = mFindingAddr;
					break;
				}
			}
			if ((mFindingAddr & 0xF) == 0)
			{
				int percentProgress = mFindingAddr / mFindSize;
				mFindWorker.ReportProgress(percentProgress);
			}
			mFindingAddr += mFindAddAddr;
		}
		mFoundAddr = num;
	}

	private void InitializeJumpList()
	{
		miJumpListAdd = new ToolStripMenuItem("");
		miJumpListRemove = new ToolStripMenuItem();
		miJumpListSeparator = new ToolStripSeparator();
		miJumpListAdd.Click += ActionAddressJumpListAddClick;
		miJumpListRemove.Click += ActionAddressJumpListRemoveClick;
		ResetJumpListMenuText();
	}

	private void ResetJumpListMenuText()
	{
		miJumpListAdd.Text = ResourceUtility.GetResourceString(ResourceType, "Resources.JumpListMenuAdd");
		miJumpListRemove.Text = ResourceUtility.GetResourceString(ResourceType, "Resources.JumpListMenuRemove");
	}

	private void ActionAddressJumpList(object sender, EventArgs e)
	{
		popupJumpListMenu.Items.Clear();
		popupJumpListMenu.Items.Add(miJumpListAdd);
		popupJumpListMenu.Items.Add(miJumpListRemove);
		popupJumpListMenu.Items.Add(miJumpListSeparator);
		foreach (JumpListInfo jump in mDataFileManager.JumpListFile.JumpList)
		{
			ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(jump.Name);
			toolStripMenuItem.Tag = jump;
			toolStripMenuItem.Click += ActionAddressJumpListitemClick;
			popupJumpListMenu.Items.Add(toolStripMenuItem);
		}
		int num = tbAddressJumpList.Bounds.Location.X + tbAddressJumpList.Width;
		int num2 = tbAddressJumpList.Bounds.Location.Y;
		popupJumpListMenu.Show(tbAddressJumpList.Owner, num, num2);
	}

	private void ActionAddressJumpListAddClick(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(mDataFileManager.JumpListFile.Text))
		{
			string fileName = mDataFileManager.RomData.FileName;
			mDataFileManager.JumpListFile.Initialize(fileName);
		}
		string textValue = Path.GetFileName(mDataFileManager.RomData.FileName) + " [" + Address.ToString("X6") + "]";
		InputTextForm inputTextForm = new InputTextForm();
		inputTextForm.TextName = "Title";
		inputTextForm.TextValue = textValue;
		if (inputTextForm.ShowDialog() == DialogResult.OK)
		{
			textValue = inputTextForm.TextValue;
			JumpListInfo jumpListInfo = new JumpListInfo();
			jumpListInfo.Address = Address;
			jumpListInfo.Format = comboBoxFormat.SelectedIndex;
			jumpListInfo.Pattern = comboBoxPattern.SelectedIndex;
			jumpListInfo.Name = textValue;
			mDataFileManager.JumpListFile.JumpList.Add(jumpListInfo);
			mDataFileManager.JumpListFile.Save();
			mDataFileManager.JumpListFile.Load();
		}
	}

	private void ActionAddressJumpListRemoveClick(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(mDataFileManager.JumpListFile.Text))
		{
			string fileName = mDataFileManager.RomData.FileName;
			mDataFileManager.JumpListFile.Initialize(fileName);
		}
		SelectComboBoxForm selectComboBoxForm = new SelectComboBoxForm();
		selectComboBoxForm.TextName = "Remove ";
		ComboBox.ObjectCollection items = selectComboBoxForm.ComboBox.Items;
		object[] items2 = mDataFileManager.JumpListFile.JumpList.ToArray();
		items.AddRange(items2);
		if (selectComboBoxForm.ShowDialog() == DialogResult.OK && selectComboBoxForm.ComboBox.SelectedItem is JumpListInfo item)
		{
			mDataFileManager.JumpListFile.JumpList.Remove(item);
			mDataFileManager.JumpListFile.Save();
		}
	}

	private void ActionAddressJumpListitemClick(object sender, EventArgs e)
	{
		if (!TabIsRom)
		{
			return;
		}
		ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)sender;
		if (toolStripMenuItem != null)
		{
			JumpListInfo jumpListInfo = (JumpListInfo)toolStripMenuItem.Tag;
			if (jumpListInfo != null)
			{
				ProcessSelectJumpList(jumpListInfo);
				mDataFileManager.JumpListFile.SelectIndexFromInfo(jumpListInfo);
			}
		}
	}

	private void ProcessSelectJumpList(JumpListInfo info)
	{
		if (info == null)
		{
			return;
		}
		try
		{
			if (info.Format >= 0)
			{
				comboBoxFormat.SelectedIndex = info.Format;
			}
			Address = info.Address;
			if (info.Pattern >= 0)
			{
				comboBoxPattern.SelectedIndex = info.Pattern;
			}
		}
		catch
		{
		}
	}

	public void ActionAddressJumpListPrevNext(object sender, EventArgs e)
	{
		JumpListInfo jumpListInfo = null;
		if (sender == tbAddressJumpListPrev)
		{
			jumpListInfo = mDataFileManager.JumpListFile.GetPrevious();
		}
		if (sender == tbAddressJumpListNext)
		{
			jumpListInfo = mDataFileManager.JumpListFile.GetNext();
		}
		if (jumpListInfo != null)
		{
			ProcessSelectJumpList(jumpListInfo);
		}
	}

	private void UpdateMenuPenColSetVisible()
	{
		bool isColMode = IsColMode;
		if (tbPenColSet != null)
		{
			tbPenColSet.Enabled = isColMode;
			tbPenColSet.Visible = isColMode;
		}
		if (miPenColSet != null)
		{
			miPenColSet.Enabled = isColMode;
			miPenColSet.Visible = isColMode;
		}
		if (isColMode)
		{
			mPenManager.ColSetPen.ColSetData = mDataFileManager.ColSetData;
		}
		else
		{
			mPenManager.ColSetPen.ColSetData = null;
		}
	}

	private void ActionPenSelect(object sender, EventArgs e)
	{
		if (sender is ToolStripItem)
		{
			object tag = ((ToolStripItem)sender).Tag;
			if (tag is EditFunctionBase)
			{
				ActionPenSelect((EditFunctionBase)tag);
			}
		}
	}

	private void ActionPenSelect(EditFunctionBase pen)
	{
		mPenManager.SelectedFunction = pen;
		if (mLastPen != null)
		{
			mLastPen = pen;
		}
		SelectEditPanelPen(mPenManager.SelectedFunction);
		editPanel.Refresh();
	}

	private void SelectEditPanelPen(EditFunctionBase pen)
	{
		if (editPanel.EditFunction != null)
		{
			editPanel.EditFunction.OnLeave();
		}
		editPanel.EditFunction = pen;
		SetPenPaletteFromDatSelector();
		if (editPanel.EditFunction != null)
		{
			editPanel.EditFunction.OnEnter();
		}
		ActionControlNavigationUpdate_MouseEnter(null, EventArgs.Empty);
		UpdatePenSelection();
	}

	private void UpdatePenSelection()
	{
		foreach (ToolStripButton item in toolStripPen.Items)
		{
			item.Checked = item.Tag == editPanel.EditFunction;
		}
		foreach (ToolStripMenuItem dropDownItem in miPen.DropDownItems)
		{
			dropDownItem.Checked = dropDownItem.Tag == editPanel.EditFunction;
		}
	}

	private void ActionOptionSetting(object sender, EventArgs e)
	{
		using SettingForm settingForm = new SettingForm();
		if (settingForm.ShowDialog() == DialogResult.OK)
		{
			mSetting.Save();
			if (KeyState.Control)
			{
				mSetting.SaveAttributeInfo();
			}
		}
		UpdateContextMenu();
		SetMouseWheelScrollRate();
		UpdateGrid();
		UpdateEditRectSize();
		PaletteMode palMode = PaletteMode.Dat;
		if (TabIsRom)
		{
			palMode = mRomTabPalMode;
		}
		if (TabIsBmp)
		{
			palMode = mBmpTabPalMode;
		}
		UpdatePalMode(palMode);
		UpdateGuiRateToolStripButtonChecked();
		UpdateTitle();
	}

	private void miOptionShowAllMenu_Click(object sender, EventArgs e)
	{
		mSetting.OptionShowAllMenu = !mSetting.OptionShowAllMenu;
		UpdateMenuOption();
	}

	private void UpdateMenuOption()
	{
		miOptionShowAllMenu.Checked = mSetting.OptionShowAllMenu;
		bool optionShowAllMenu = mSetting.OptionShowAllMenu;
		miPen.Visible = optionShowAllMenu;
		miAddress.Visible = optionShowAllMenu;
		miPalettePaletteType.Visible = optionShowAllMenu;
	}

	private void ActionOptionRunFile(object sender, EventArgs e)
	{
		try
		{
			if (!CheckSaveModifiedData())
			{
				Process.Start(new ProcessStartInfo(mDataFileManager.RomData.FileName));
			}
		}
		catch
		{
			string resourceString = ResourceUtility.GetResourceString(ResourceType, "Resources.TitleError");
			string text = ResourceUtility.GetResourceString(ResourceType, "Resources.MessageExecuteError").Replace("\\r", "\r").Replace("\\n", "\n") + mDataFileManager.RomData.FileName;
			MsgBox.Show(this, text, resourceString, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}

	private void LoadNaviText()
	{
		NaviList.Clear();
		NaviList.Add("");
		NaviList.Add(ResourceUtility.GetResourceString(ResourceType, "Resources.NaviBankL"));
		NaviList.Add(ResourceUtility.GetResourceString(ResourceType, "Resources.NaviBankR"));
		NaviList.Add(ResourceUtility.GetResourceString(ResourceType, "Resources.NaviBankLAlt"));
		NaviList.Add(ResourceUtility.GetResourceString(ResourceType, "Resources.NaviBankRAlt"));
		NaviList.Add(ResourceUtility.GetResourceString(ResourceType, "Resources.NaviBankW"));
		NaviList.Add("");
		NaviList.Add("");
		NaviList.Add(ResourceUtility.GetResourceString(ResourceType, "Resources.NaviEditW_Scroll"));
		NaviList.Add(ResourceUtility.GetResourceString(ResourceType, "Resources.NaviEditW_Pen"));
		NaviList.Add(ResourceUtility.GetResourceString(ResourceType, "Resources.NaviEditW_Palette"));
		NaviList.Add(ResourceUtility.GetResourceString(ResourceType, "Resources.NaviEditW_Size"));
		NaviList.Add(ResourceUtility.GetResourceString(ResourceType, "Resources.NaviDatL"));
		NaviList.Add(ResourceUtility.GetResourceString(ResourceType, "Resources.NaviDatR"));
		NaviList.Add(ResourceUtility.GetResourceString(ResourceType, "Resources.NaviDatW"));
		NaviList.Add(ResourceUtility.GetResourceString(ResourceType, "Resources.NaviPalL"));
		NaviList.Add(ResourceUtility.GetResourceString(ResourceType, "Resources.NaviPalR"));
		NaviList.Add(ResourceUtility.GetResourceString(ResourceType, "Resources.NaviPalW"));
	}

	private string GetNavi(NaviType navitype)
	{
		if ((int)navitype < NaviList.Count)
		{
			return NaviList[(int)navitype];
		}
		return "!!!NaviError!!!";
	}

	private void UpdateLanguageMenuState()
	{
		bool loadFromLngFile = ResourceUtility.LoadFromLngFile;
		bool startupLanguageFromLng = mSetting.StartupLanguageFromLng;
		try
		{
			string twoLetterISOLanguageName = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
			string twoLetterISOLanguageName2 = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
			miLanguageSystem.Checked = twoLetterISOLanguageName == twoLetterISOLanguageName2 && !loadFromLngFile;
			miLanguageEnglish.Checked = twoLetterISOLanguageName == "iv" && !loadFromLngFile;
			miLanguageJapanese.Checked = twoLetterISOLanguageName == "ja" && !loadFromLngFile;
			miLanguageLngFile.Checked = loadFromLngFile;
			miLanguageSettingAutoLoadLng.Checked = startupLanguageFromLng;
			EnJaPropertyDescriptor.EnJaConvert = miLanguageJapanese.Checked;
		}
		catch
		{
		}
	}

	private void UpdateBuildDateLabel()
	{
		lVersion.Text = "build " + GetBuildDate();
		int num = base.ClientSize.Width - lVersion.Width;
		lVersion.Location = new Point(num, lVersion.Top);
	}

	private void ActionOptionLanguageSet(object sender, EventArgs e)
	{
		if (sender == miLanguageLngFile)
		{
			LoadLanguageFile();
		}
		else
		{
			CultureInfo culture = Thread.CurrentThread.CurrentUICulture;
			try
			{
				if (sender == miLanguageSystem)
				{
					culture = CultureInfo.InstalledUICulture;
				}
				if (sender == miLanguageEnglish)
				{
					culture = CultureInfo.GetCultureInfo("");
				}
				if (sender == miLanguageJapanese)
				{
					culture = CultureInfo.GetCultureInfo("ja");
				}
				ResourceUtility.LoadFromLngFile = false;
				ReloadResource(culture);
			}
			catch
			{
			}
		}
		UpdateLanguageMenuState();
	}

	private void ReloadResource(CultureInfo culture)
	{
		try
		{
			SetGuiCulture(culture);
			LoadNaviText();
			mPenManager.InitFunctionList();
			CreatePenToolBar();
			SelectEditPanelPen(mPenManager.SelectedFunction);
			UpdateMenuPenColSetVisible();
			ResetJumpListMenuText();
			UpdateBuildDateLabel();
			UpdateMenuAll();
			UpdateEditRectSize();
			UpdateGrid();
			UpdateGuiRateToolStripButtonChecked();
			UpdateStatusbarAddress();
			UpdateStatusbarSelection(mSourceRect);
			UpdateTitle();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	private void SetGuiCulture(CultureInfo culture)
	{
		try
		{
			Thread.CurrentThread.CurrentUICulture = culture;
			try
			{
				ResourceUtility.ChangeLanguage(this, toolTip, Thread.CurrentThread.CurrentUICulture);
				ResourceUtility.ChangeLanguage(mPaletteEditorForm, mPaletteEditorForm.ToolTip, culture);
				if (mPropertyEditorForm != null && !mPropertyEditorForm.IsDisposed && !mPropertyEditorForm.Disposing)
				{
					mPropertyEditorForm.Close();
					mPropertyEditorForm.Dispose();
					mPropertyEditorForm = null;
				}
			}
			catch (Exception ex)
			{
				string resourceString = ResourceUtility.GetResourceString(ResourceType, "Resources.TitleError");
				string text = "Exception \r\n\r\n" + ex.ToString();
				MsgBox.Show(this, text, resourceString, MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}
		catch (Exception ex2)
		{
			MessageBox.Show(ex2.Message);
		}
	}

	private void miLanguageOutputLng_Click(object sender, EventArgs e)
	{
		CultureInfo culture = new CultureInfo(Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName);
		string path = Environment.GetCommandLineArgs()[0].Replace(".vshost", "");
		string text = Path.ChangeExtension(path, ".output.lng");
		string directoryName = Path.GetDirectoryName(path);
		ResourceUtility.CreateNewLngList();
		string fileName = Path.GetFileName(path);
		Assembly assembly = Assembly.LoadFile(directoryName + "\\" + fileName);
		ResourceManager rm = new ResourceManager("YYCHR.Properties.Resources", assembly);
		AddPropertiesResourceTextToList(fileName, culture, rm);
		AddControlResourceTextToList(assembly, culture);
		string text2 = "ControlLib.dll";
		Assembly assembly2 = Assembly.LoadFile(directoryName + "\\" + text2);
		ResourceManager rm2 = new ResourceManager("ControlLib.Properties.Resources", assembly2);
		AddPropertiesResourceTextToList(text2, culture, rm2);
		AddControlResourceTextToList(assembly2, culture);
		ResourceUtility.SaveLngFile(text);
		string resourceString = ResourceUtility.GetResourceString(ResourceType, "Resources.TitleInformation");
		string text3 = ResourceUtility.GetResourceString(ResourceType, "Resources.MessageLngFileExported") + "\r\n\r\n" + text;
		MsgBox.Show(this, text3, resourceString, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
	}

	private void AddPropertiesResourceTextToList(string targetName, CultureInfo culture, ResourceManager rm)
	{
		try
		{
			if (rm == null)
			{
				return;
			}
			using ResourceSet resourceSet = rm.GetResourceSet(culture, createIfNotExists: true, tryParents: true);
			if (resourceSet == null)
			{
				return;
			}
			IDictionaryEnumerator enumerator = resourceSet.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Value is string)
				{
					string text = (string)enumerator.Key;
					string value = (string)enumerator.Value;
					if (!text.Contains(">>") && !string.IsNullOrWhiteSpace(value))
					{
						ResourceUtility.SetValue(targetName, text, value);
					}
				}
			}
			resourceSet.Close();
		}
		catch
		{
		}
	}

	private void AddControlResourceTextToList(Assembly asm, CultureInfo culture)
	{
		List<Type> list = new List<Type>();
		Type[] types = asm.GetTypes();
		foreach (Type type in types)
		{
			if (type.IsSubclassOf(typeof(Control)))
			{
				list.Add(type);
			}
		}
		foreach (Type item in list)
		{
			string name = item.Name;
			ComponentResourceManager rm = new ComponentResourceManager(item);
			AddPropertiesResourceTextToList(name, culture, rm);
		}
	}

	private void LoadLanguageFile()
	{
		string lngFile = Path.ChangeExtension(Environment.GetCommandLineArgs()[0].Replace(".vshost", ""), "lng");
		LoadLanguageFile(lngFile);
	}

	private void LoadLanguageFileIfExists()
	{
		string text = Path.ChangeExtension(Environment.GetCommandLineArgs()[0].Replace(".vshost", ""), "lng");
		if (File.Exists(text))
		{
			LoadLanguageFile(text);
		}
	}

	private void LoadLanguageFile(string lngFile)
	{
		if (ResourceUtility.LoadLngFile(lngFile))
		{
			ResourceUtility.LoadFromLngFile = true;
			CultureInfo culture = new CultureInfo("iv");
			ReloadResource(culture);
			return;
		}
		ResourceUtility.LoadFromLngFile = false;
		CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
		ReloadResource(currentUICulture);
		string resourceString = ResourceUtility.GetResourceString(ResourceType, "Resources.TitleWarning");
		string text = ResourceUtility.GetResourceString(ResourceType, "Resources.MessageLngFileNotExist") + "\r\n\r\n" + lngFile;
		MsgBox.Show(this, text, resourceString, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
	}

	private void miLanguageLoadLngStartup_Click(object sender, EventArgs e)
	{
		mSetting.StartupLanguageFromLng = !mSetting.StartupLanguageFromLng;
		UpdateLanguageMenuState();
	}

	private void ActionHelpPropertyEditor(object sender, EventArgs ev)
	{
		try
		{
			if (mPropertyEditorForm != null)
			{
				GC.Collect();
				mPropertyEditorForm.Dispose();
				mPropertyEditorForm = null;
				GC.Collect();
			}
			if (mPropertyEditorForm == null)
			{
				mPropertyEditorForm = new PropertyEditorForm();
				mPropertyEditorForm.EditObject = this;
				mPropertyEditorForm.Show();
			}
		}
		catch (Exception ex)
		{
			MsgBox.Show(this, ex.Message, "YY-CHR.NET");
		}
	}

	private void ActionHelpOpenWiki(object sender, EventArgs e)
	{
		Process.Start(new ProcessStartInfo("https://www45.atwiki.jp/yychr/"));
	}

	private void ActionHelpOpenWeb(object sender, EventArgs e)
	{
		Process.Start(new ProcessStartInfo("https://www45.atwiki.jp/yychr/"));
	}

	private void ActionHelpOpenBbs(object sender, EventArgs e)
	{
		Process.Start(new ProcessStartInfo("https://jbbs.shitaraba.net/bbs/read.cgi/computer/41853/1231162374/l50"));
	}

	private void ActionHelpAbout(object sender, EventArgs e)
	{
		string obj = "YY-CHR.NET Ver.1.00  build " + GetBuildDate() + "\r\n\t\t:: Copyright 2010 Yy, Yorn\r\n\t\t:: https://www45.atwiki.jp/yychr/\r\n\r\n------------------------lib------------------------\r\nFamicom Palette\t:: By Kasion\r\n\t\t:: http://hlc6502.web.fc2.com/NesPal2.htm\r\nFamicom Palette\t:: By misaki\r\n\t\t:: http://metalidol.xxxxxxxx.jp/famicom_palette.html\r\nMSX Palette\t:: By wizforest\r\n\t\t:: https://www.wizforest.com/OldGood/ntsc/msx.html\r\n";
		Size size = new Size(640, 320);
		MessageBoxForm.Show(obj, "About YY-CHR.NET", MessageBoxIcon.Asterisk, MessageBoxButtons.OK, size);
	}

	private string GetBuildDate()
	{
		Version version = GetType().Assembly.GetName().Version;
		int build = version.Build;
		int revision = version.Revision;
		return new DateTime(2000, 1, 1).AddDays(build).AddSeconds(revision * 2).ToString("yyyy/MM/dd");
	}

	private void ActionShowHint(object sender, EventArgs e)
	{
		Control control = null;
		if (control != null)
		{
			string name = control.Name;
			Process.Start(new ProcessStartInfo("http://www45.atwiki.jp/yychr/?page=" + name));
		}
	}

	private void scrollPanelRom_Scrolled(object sender, EventArgs e)
	{
		mDataFileManager.SettingInfo.ProcessTrap(Address);
		if (mFormat != null && mDataFileManager.RomData.Data != null)
		{
			RedrawFormat();
		}
	}

	private void InitializeFormat()
	{
		comboBoxFormat.Items.Clear();
		FormatBase[] formats = mFormatManager.GetFormats();
		foreach (FormatBase item in formats)
		{
			comboBoxFormat.Items.Add(item);
		}
		comboBoxFormat.SelectedIndex = 0;
		SetFormat();
	}

	private void comboBoxFormat_SelectedIndexChanged(object sender, EventArgs e)
	{
		SetFormat();
		RedrawFormat();
		RefreshPalSelector();
		RefreshDatSelector();
	}

	private void SetFormat()
	{
		if (comboBoxFormat.SelectedItem != null)
		{
			if (mFormat != null)
			{
				mFormat.UnloadFormat();
			}
			mFormat = (FormatBase)comboBoxFormat.SelectedItem;
			if (mFormat != null)
			{
				mFormat.LoadFormat(this);
			}
		}
		if (mDataFileManager.RomData.Data == null)
		{
			return;
		}
		datPaletteSelector.SetSize = mFormat.ColorNum;
		mSourceBytemap.CanUpdatePalette = true;
		comboBoxMirror.Visible = mFormat.IsSupportMirror;
		labelMirror.Visible = mFormat.IsSupportMirror;
		mFormat.MirrorType = (MirrorType)comboBoxMirror.SelectedItem;
		mSourceBytemap.CanUpdatePixel = true;
		comboBoxRotate.Visible = mFormat.IsSupportRotate;
		labelRotate.Visible = mFormat.IsSupportRotate;
		mFormat.RotateType = (RotateType)comboBoxRotate.SelectedItem;
		mSourceBytemap.CanUpdatePixel = true;
		int charactorByteSize = mFormat.GetCharactorByteSize();
		int smallChange = charactorByteSize * mFormat.ColumnCount;
		int bankByteSize = mFormat.GetBankByteSize();
		mFormat.AdfPattern = (AdfInfo)comboBoxPattern.SelectedItem;
		scrollPanelRom.LargeChange = bankByteSize;
		scrollPanelRom.SmallChange = smallChange;
		scrollPanelRom.LrChange = charactorByteSize;
		scrollPanelRom.Minimum = 0;
		scrollPanelRom.Maximum = mDataFileManager.RomData.Data.Length - 1;
		CheckMinimumFileBufferLength();
		try
		{
			if (mFormat.ColorBit > 2)
			{
				miPaletteTypePal.PerformClick();
			}
			else
			{
				miPaletteTypeDat.PerformClick();
			}
		}
		catch
		{
		}
		UpdateDatPaletteLabel();
		SetBytemapPalette();
	}

	private void SetBytemapPalette()
	{
		if (mSourceBytemap == null)
		{
			return;
		}
		if (IsColMode)
		{
			mRomBytemap.SetPalette(mDataFileManager.DatInfo.Colors);
		}
		else
		{
			int num = datPaletteSelector.SelectedSet * datPaletteSelector.SetSize;
			for (int i = 0; i < datPaletteSelector.SetSize; i++)
			{
				Color color = datPaletteSelector.Palette[num + i];
				mSourceBytemap.Palette[i] = color;
			}
		}
		mSourceBytemap.CanUpdatePalette = true;
		workSpaceSelector1.SetPalette(mSourceBytemap.Palette);
	}

	private void RefreshBitmapFromBytemap()
	{
		if (mSourceBytemap != null)
		{
			ConvertBytemapToBitmap();
			mActiveSelector.Refresh();
			UpdateEditPanelSource();
		}
	}

	private void RedrawFormat()
	{
		if (mSuppressRedraw)
		{
			return;
		}
		if (editPanel.EditFunction != null)
		{
			editPanel.EditFunction.OnChrLoad();
		}
		try
		{
			if (TabIsRom)
			{
				ConvertFileDataToBytemap();
				RefreshBitmapFromBytemap();
				if (mDataFileManager.RomData.Data != null)
				{
					UpdateStatusbarAddress();
				}
			}
			else if (TabIsBmp)
			{
				RefreshBitmapFromBytemap();
			}
			else if (TabIsWorkspace)
			{
				workSpaceSelector1.ForceDraw = true;
				workSpaceSelector1.Refresh();
			}
			UpdateTitle();
		}
		catch
		{
		}
	}

	private void ConvertFileDataToBytemap()
	{
		if (mDataFileManager.RomData.Data != null)
		{
			mFormat.ConvertAllMemToChr(mDataFileManager.RomData.Data, Address, mRomBytemap);
			if (IsColMode && mFormat.CharSize.Width == 8 && mFormat.CharSize.Height == 8)
			{
				int address = Address;
				mPenManager.ColSetPen.ColSetDataAddr = mDataFileManager.ColSetData.GetBankPaletteSetAddr(address);
				mPenManager.ColSetPen.AdfPattern = mFormat.AdfPattern;
				byte[] bankPaletteSet = mDataFileManager.ColSetData.GetBankPaletteSet(address, mFormat.AdfPattern.Pattern);
				BytemapConvertor.AddPalSet(mRomBytemap, bankPaletteSet, mFormat.ColorNum);
			}
			mRomBytemap.CanUpdatePixel = true;
		}
	}

	private void ConvertBytemapToFile()
	{
		if (TabIsRom)
		{
			if (mDataFileManager.RomData.Data == null)
			{
				return;
			}
			if (!mFormat.Readonly)
			{
				int address = Address;
				int bankByteSize = mFormat.GetBankByteSize();
				mDataFileManager.RomData.CreateUndoBuffer(address, bankByteSize);
				mFormat.ConvertAllChrToMem(mDataFileManager.RomData.Data, address, mRomBytemap);
				if (!mDataFileManager.RomData.Modified)
				{
					mDataFileManager.RomData.Modified = true;
					UpdateTitle();
				}
			}
		}
		else if (TabIsBmp)
		{
			mBmpUndoManager.CreateUndoBuffer(mBmpBytemap.Data);
			mOpenedBmpModified = true;
			UpdateTitle();
		}
		UpdateUndoMenu();
	}

	private void ConvertBytemapToBitmap()
	{
		if (mSourceBytemap.CanUpdatePalette)
		{
			BytemapConvertor.UpdateBitmapPaletteFromBytemap(mSourceBitmap, mSourceBytemap);
			mSourceBytemap.CanUpdatePalette = false;
		}
		if (mSourceBytemap.CanUpdatePixel)
		{
			BytemapConvertor.UpdateBitmapFromBytemap(mSourceBitmap, mSourceBytemap);
			mSourceBytemap.CanUpdatePixel = false;
		}
	}

	private void ConvertBitmapToBytemap()
	{
		BytemapConvertor.UpdateBytemapFromBitmap(mSourceBitmap, mSourceBytemap);
	}

	private void actionPatternEdit(object sender, EventArgs e)
	{
		Bytemap bytemap = mRomBytemap.Clone();
		mFormat.ConvertAllMemToChr(mDataFileManager.RomData.Data, Address, bytemap, null);
		if (new PatternEditorForm(mDataFileManager.AdfPattern, bytemap).ShowDialog(this) == DialogResult.OK)
		{
			mDataFileManager.AdfPattern.UpdateAdf();
			RedrawFormat();
		}
	}

	private void ActionWorkspaceLoad(object sender, EventArgs e)
	{
		openDataFileDialog.Filter = "Workspace (*.workspace)|*.workspace|All files|*.*";
		if (openDataFileDialog.ShowDialog() == DialogResult.OK)
		{
			string fileName = openDataFileDialog.FileName;
			workSpaceSelector1.WsInfo.Load(fileName);
		}
	}

	private void ActionWorkspaceSave(object sender, EventArgs e)
	{
		saveDataFileDialog.Filter = "Workspace (*.workspace)|*.workspace|All files|*.*";
		if (saveDataFileDialog.ShowDialog() == DialogResult.OK)
		{
			string fileName = saveDataFileDialog.FileName;
			workSpaceSelector1.WsInfo.Save(fileName);
		}
	}

	private void ActionWorkspaceAdd(object sender, EventArgs e)
	{
		workSpaceSelector1.WsInfo.UnSelectAll();
		if (!TabIsRom)
		{
			return;
		}
		int address = Address;
		int charactorByteSize = mFormat.GetCharactorByteSize();
		Rectangle selectedRect = cellSelectorRom.SelectedRect;
		int num = selectedRect.X / 8;
		int num2 = selectedRect.Y / 8;
		int num3 = selectedRect.Width / 8;
		int num4 = selectedRect.Height / 8;
		for (int i = 0; i < num4; i++)
		{
			for (int j = 0; j < num3; j++)
			{
				byte b = 0;
				int num5 = (num2 + i) * 16 + (num + j);
				b = mFormat.AdfPattern.Pattern[num5];
				PatternInfo patternInfo = new PatternInfo();
				patternInfo.Address = address + b * charactorByteSize;
				patternInfo.Format = 0;
				patternInfo.PalSet = 0;
				patternInfo.X = j * 8;
				patternInfo.Y = i * 8;
				patternInfo.Rotate = RotateType.None;
				patternInfo.Mirror = MirrorType.None;
				patternInfo.Selected = true;
				workSpaceSelector1.WsInfo.AddPattern(patternInfo, mFormat);
			}
		}
		tabControl.SelectedTab = tabWorkSpace;
	}

	private void ActionWorkspaceRemove(object sender, EventArgs e)
	{
		if (TabIsWorkspace)
		{
			workSpaceSelector1.WsInfo.RemoveSelectedPattern();
		}
		RedrawFormat();
	}

	private void UpdateControlSizeFromGuiZoomRate()
	{
		int guiSizeRate = mSetting.GuiSizeRate;
		int num = 128 * guiSizeRate + 70;
		editPanel.ClientSize = new Size(128 * guiSizeRate, 128 * guiSizeRate);
		editPanel.ZoomRate = guiSizeRate;
		rightPanel.Width = num;
		cellSelectorRom.ClientSize = new Size(128 * guiSizeRate, 128 * guiSizeRate);
		cellSelectorRom.ZoomRate = guiSizeRate;
		cellSelectorBmp.ClientSize = new Size(512, 512);
		int num2 = SystemInformation.VerticalScrollBarWidth + 4;
		datPaletteSelector.Width = 128 * guiSizeRate + num2;
		palPaletteSelector.Width = 128 * guiSizeRate + num2;
		datPaletteSelector.CellWidth = 8 * guiSizeRate;
		palPaletteSelector.CellWidth = 8 * guiSizeRate;
		panelPalette.Width = palPaletteSelector.Width + 60;
	}

	private void UpdateFormSize()
	{
		int num = mActiveSelector.ClientSize.Width;
		int num2 = mActiveSelector.ClientSize.Height;
		if (TabIsRom)
		{
			num += 86;
			num2 += 100;
		}
		if (TabIsBmp)
		{
			num += 60;
			num2 += 40;
		}
		int num3 = editPanel.ClientSize.Width + 70;
		int num4 = editPanel.ClientSize.Height + 12;
		int num5 = menuStripMain.Height + toolStripMain.Height;
		int num6 = toolStripView.Height + statusStrip.Height;
		scrollPanelBmp.ClientAreaSize = new Size(512, 512);
		SetMainFormControlSize();
		int num7 = panelPalette.Height;
		int num8 = ((num4 + num7 <= num2) ? num2 : (num4 + num7));
		base.ClientSize = new Size(num + num3, num5 + num8 + num6);
	}

	private void UpdateTitle()
	{
		if (TabIsRom)
		{
			SetTitleRom();
		}
		else if (TabIsBmp)
		{
			SetTitleBmp();
		}
		else
		{
			SetTitle("");
		}
	}

	private void SetTitleRom()
	{
		if (mDataFileManager.RomData != null)
		{
			string fileName = Path.GetFileName(mDataFileManager.RomData.FileName);
			string text = "";
			if (mDataFileManager.RomData.Modified)
			{
				text = " *";
			}
			string title;
			if (mSetting.OptionShowTitleAddress)
			{
				int address = Address;
				int num = (int)mDataFileManager.RomData.FileSize;
				string text2 = address.ToString("X6");
				string text3 = num.ToString("X6");
				title = "(" + text2 + " / " + text3 + ") " + fileName + text;
			}
			else
			{
				title = " " + fileName + text;
			}
			SetTitle(title);
		}
	}

	private void SetTitleBmp()
	{
		string fileName = Path.GetFileName(mOpenedBmpFilename);
		string text = "";
		if (mOpenedBmpModified)
		{
			text = " *";
		}
		string title = " " + fileName + text;
		SetTitle(title);
	}

	private void SetTitle(string addText)
	{
		string text = "YY-CHR.NET " + addText;
		if (Text != text)
		{
			Text = text;
		}
	}

	private void UpdateStatusbarVisible()
	{
		bool tabIsRom = TabIsRom;
		if (slAddr.Visible != tabIsRom)
		{
			slAddr.Visible = tabIsRom;
		}
		if (slXY.Width != 90)
		{
			slXY.Width = 90;
		}
		if (slChr.Width != 60)
		{
			slChr.Width = 60;
		}
		if (slHintMouseWheel.Width != slHintMouseButtonL.Width)
		{
			slHintMouseWheel.Width = slHintMouseButtonL.Width;
		}
		int num = statusStrip.Width;
		int num2 = 0;
		foreach (ToolStripStatusLabel item in statusStrip.Items)
		{
			if (item.Visible && !item.Spring && item != slHintMouseWheel && item != slChr)
			{
				num2 += item.Width;
			}
		}
		num2 += slHintMouseWheel.Width;
		slHintMouseWheel.Visible = num2 < num;
		if (tabIsRom && num2 + slChr.Width < num)
		{
			slChr.Visible = true;
			num2 += slChr.Width;
		}
		else
		{
			slChr.Visible = false;
		}
	}

	private void UpdateStatusbarAddress()
	{
		int address = Address;
		int bankByteSize = mFormat.GetBankByteSize();
		int num = mDataFileManager.RomData.Data.Length;
		string text = address.ToString("X6");
		string text2 = (address + bankByteSize - 1).ToString("X6");
		string text3 = num.ToString("X6");
		slAddr.Text = "CHR: " + text + " - " + text2 + " / " + text3;
	}

	private void UpdateStatusbarSelection(Rectangle rect)
	{
		if (TabIsRom)
		{
			int num = (rect.Y / 8 * 16 + rect.X / 8 % 16) & 0xFF;
			int num2 = mFormat.AdfPattern.Pattern[num];
			string text = "[" + num.ToString("X2") + "] " + num2.ToString("X2");
			slChr.Text = text;
		}
		string text2 = rect.X.ToString("X2") + "," + rect.Y.ToString("X2") + "," + rect.Width.ToString("X2") + "," + rect.Height.ToString("X2");
		slXY.Text = text2;
	}

	private void ActionControlNavigationUpdate_MouseEnter(object sender, EventArgs e)
	{
		try
		{
			Control control = null;
			control = ((sender == null) ? _LastNavigateControl : (_LastNavigateControl = sender as Control));
			if (control == null)
			{
				ActionControlNavigationUpdate_MouseLeave(sender, e);
				return;
			}
			string hintL = "";
			string hintR = "";
			string hintW = "";
			if (control == cellSelectorRom || control == cellSelectorBmp)
			{
				if (KeyState.Alt)
				{
					hintL = GetNavi(NaviType.BankFreeSelect);
					hintR = GetNavi(NaviType.BankFreeSelectArea);
				}
				else
				{
					hintL = GetNavi(NaviType.BankSelect);
					hintR = GetNavi(NaviType.BankSelectArea);
				}
				if (KeyState.Control)
				{
					hintW = GetNavi(NaviType.EditChangeSize);
				}
				else if (control == cellSelectorRom)
				{
					hintW = GetNavi(NaviType.BankScroll);
				}
			}
			else if (control == editPanel)
			{
				string[] naviText = editPanel.EditFunction.GetNaviText();
				if (naviText != null && naviText.Length == 2)
				{
					hintL = naviText[0];
					hintR = naviText[1];
				}
				EditorMouseWheelFunction editorMouseWheelFunction = mSetting.EditorMouseWheel;
				if (KeyState.Control)
				{
					editorMouseWheelFunction = mSetting.EditorMouseWheelCtrl;
				}
				if (KeyState.Shift)
				{
					editorMouseWheelFunction = mSetting.EditorMouseWheelShift;
				}
				if (KeyState.Alt)
				{
					editorMouseWheelFunction = mSetting.EditorMouseWheelAlt;
				}
				switch (editorMouseWheelFunction)
				{
				case EditorMouseWheelFunction.BankScroll:
					if (TabIsRom)
					{
						hintW = GetNavi(NaviType.EditScroll);
					}
					break;
				case EditorMouseWheelFunction.PenSelect:
					hintW = GetNavi(NaviType.EditChangePen);
					break;
				case EditorMouseWheelFunction.PaletteSelect:
					hintW = GetNavi(NaviType.EditChangePalette);
					break;
				case EditorMouseWheelFunction.EditorSizeSelect:
					hintW = GetNavi(NaviType.EditChangeSize);
					break;
				default:
					hintW = "";
					break;
				}
			}
			else if (control == datPaletteSelector)
			{
				hintL = GetNavi(NaviType.DatSelect);
				hintR = GetNavi(NaviType.DatEditRGB);
				hintW = GetNavi(NaviType.DatScroll);
			}
			else if (control == palPaletteSelector)
			{
				hintL = GetNavi(NaviType.PalSelect);
				hintR = GetNavi(NaviType.PalEditRGB);
				hintW = GetNavi(NaviType.PalScroll);
			}
			SetMouseNavigateText(hintL, hintR, hintW);
		}
		catch
		{
		}
	}

	private void ActionControlNavigationUpdate_MouseLeave(object sender, EventArgs e)
	{
		try
		{
			string hintL = "";
			string hintR = "";
			string hintW = "";
			SetMouseNavigateText(hintL, hintR, hintW);
			_LastNavigateControl = null;
		}
		catch
		{
		}
	}

	private void SetMouseNavigateText(string hintL, string hintR, string hintW)
	{
		if (slHintMouseButtonL.Text != hintL)
		{
			slHintMouseButtonL.Text = hintL;
		}
		if (slHintMouseButtonR.Text != hintR)
		{
			slHintMouseButtonR.Text = hintR;
		}
		if (slHintMouseWheel.Text != hintW)
		{
			slHintMouseWheel.Text = hintW;
		}
	}

	private void editPanel_OnNaviUpdated(object sender, EventArgs e)
	{
		ActionControlNavigationUpdate_MouseEnter(sender, e);
	}

	public void DebugCount()
	{
		mDebugCounter++;
		DebugCount(mDebugCounter);
	}

	private void DebugCount(int count)
	{
		lVersion.Text = count.ToString();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(YYCHR.MainForm));
		this.toolStripView = new ControlLib.OneClickToolStrip();
		this.tbViewGuiRate = new System.Windows.Forms.ToolStripDropDownButton();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.tbViewEditSize = new System.Windows.Forms.ToolStripDropDownButton();
		this.tbViewEditorSize8 = new System.Windows.Forms.ToolStripMenuItem();
		this.tbViewEditorSize16 = new System.Windows.Forms.ToolStripMenuItem();
		this.tbViewEditorSize32 = new System.Windows.Forms.ToolStripMenuItem();
		this.tbViewEditorSize64 = new System.Windows.Forms.ToolStripMenuItem();
		this.tbViewEditorSize128 = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
		this.tbViewGridStyle = new System.Windows.Forms.ToolStripDropDownButton();
		this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
		this.tbViewGridBank = new System.Windows.Forms.ToolStripButton();
		this.tbViewGridEditor = new System.Windows.Forms.ToolStripButton();
		this.statusStrip = new System.Windows.Forms.StatusStrip();
		this.slAddr = new System.Windows.Forms.ToolStripStatusLabel();
		this.slXY = new System.Windows.Forms.ToolStripStatusLabel();
		this.slChr = new System.Windows.Forms.ToolStripStatusLabel();
		this.slRightSpace = new System.Windows.Forms.ToolStripStatusLabel();
		this.slKeyCtrl = new ControlLib.ToolStripStatusLabelEx();
		this.slKeyShift = new ControlLib.ToolStripStatusLabelEx();
		this.slKeyAlt = new ControlLib.ToolStripStatusLabelEx();
		this.slHintMouseButtonL = new ControlLib.ToolStripStatusLabelEx();
		this.slHintMouseWheel = new ControlLib.ToolStripStatusLabelEx();
		this.slHintMouseButtonR = new ControlLib.ToolStripStatusLabelEx();
		this.tabControl = new System.Windows.Forms.TabControl();
		this.tabPageChrRom = new System.Windows.Forms.TabPage();
		this.panelChr = new System.Windows.Forms.Panel();
		this.panelForPlugin = new System.Windows.Forms.Panel();
		this.panelChrSetting = new System.Windows.Forms.Panel();
		this.labelFormat = new System.Windows.Forms.Label();
		this.buttonPatternEdit = new ControlLib.ButtonNoFocus();
		this.comboBoxFormat = new Controls.ComboBoxEx();
		this.buttonFormatInfo = new ControlLib.ButtonNoFocus();
		this.comboBoxMirror = new Controls.ComboBoxEx();
		this.labelPattern = new System.Windows.Forms.Label();
		this.comboBoxPattern = new Controls.ComboBoxEx();
		this.labelMirror = new System.Windows.Forms.Label();
		this.comboBoxRotate = new Controls.ComboBoxEx();
		this.labelRotate = new System.Windows.Forms.Label();
		this.panelToolStripAddress = new System.Windows.Forms.Panel();
		this.toolStripAddress = new ControlLib.OneClickToolStrip();
		this.tbAddres0 = new System.Windows.Forms.ToolStripButton();
		this.tbAddres1 = new System.Windows.Forms.ToolStripButton();
		this.tbAddres2 = new System.Windows.Forms.ToolStripButton();
		this.tbAddres3 = new System.Windows.Forms.ToolStripButton();
		this.tbAddres4 = new System.Windows.Forms.ToolStripButton();
		this.tbAddres5 = new System.Windows.Forms.ToolStripButton();
		this.tbAddres6 = new System.Windows.Forms.ToolStripButton();
		this.tbAddres7 = new System.Windows.Forms.ToolStripButton();
		this.tbAddres8 = new System.Windows.Forms.ToolStripButton();
		this.tbAddres9 = new System.Windows.Forms.ToolStripButton();
		this.tbAddresSep0 = new System.Windows.Forms.ToolStripSeparator();
		this.tbAddresInputAddress = new System.Windows.Forms.ToolStripButton();
		this.tbAddressJumpList = new System.Windows.Forms.ToolStripButton();
		this.tbAddressJumpListPrev = new System.Windows.Forms.ToolStripButton();
		this.tbAddressJumpListNext = new System.Windows.Forms.ToolStripButton();
		this.tbAddresSep1 = new System.Windows.Forms.ToolStripSeparator();
		this.tbAddressFindPrevious = new System.Windows.Forms.ToolStripButton();
		this.tbAddressFindNext = new System.Windows.Forms.ToolStripButton();
		this.scrollPanelRom = new ControlLib.ScrollPanel();
		this.cellSelectorRom = new ControlLib.CellSelector();
		this.tabPageBitmap = new System.Windows.Forms.TabPage();
		this.scrollPanelBmp = new ControlLib.ScrollPanelHV();
		this.cellSelectorBmp = new ControlLib.CellSelector();
		this.tabWorkSpace = new System.Windows.Forms.TabPage();
		this.panelWorkSpace = new System.Windows.Forms.Panel();
		this.workSpaceSelector1 = new ControlLib.WorkSpaceSelector();
		this.rightPanel = new System.Windows.Forms.Panel();
		this.panelPalette = new System.Windows.Forms.Panel();
		this.toolStripPalette = new ControlLib.OneClickToolStrip();
		this.tbPaletteTypeDat = new System.Windows.Forms.ToolStripButton();
		this.tbPaletteTypePal = new System.Windows.Forms.ToolStripButton();
		this.tbPaletteTypeBmp = new System.Windows.Forms.ToolStripButton();
		this.tbPaletteSep1 = new System.Windows.Forms.ToolStripSeparator();
		this.tbPaletteOpenState = new System.Windows.Forms.ToolStripButton();
		this.palPaletteSelector = new ControlLib.PaletteSelector();
		this.datPaletteSelector = new ControlLib.PaletteSelector();
		this.panelEdit = new System.Windows.Forms.Panel();
		this.toolStripPen = new ControlLib.OneClickToolStrip();
		this.editPanel = new ControlLib.EditPanel();
		this.toolStripMain = new ControlLib.OneClickToolStrip();
		this.tbFileNew = new System.Windows.Forms.ToolStripButton();
		this.tbFileSep0 = new System.Windows.Forms.ToolStripSeparator();
		this.tbFileOpenRom = new System.Windows.Forms.ToolStripButton();
		this.tbFileSaveRom = new System.Windows.Forms.ToolStripButton();
		this.tbFileOpenBmp = new System.Windows.Forms.ToolStripButton();
		this.tbFileSaveBmp = new System.Windows.Forms.ToolStripButton();
		this.tbFileSep1 = new System.Windows.Forms.ToolStripSeparator();
		this.tbEditCut = new System.Windows.Forms.ToolStripButton();
		this.tbEditCopy = new System.Windows.Forms.ToolStripButton();
		this.tbEditPaste = new System.Windows.Forms.ToolStripButton();
		this.tbEditClear = new System.Windows.Forms.ToolStripButton();
		this.tbEditSep0 = new System.Windows.Forms.ToolStripSeparator();
		this.tbEditUndo = new System.Windows.Forms.ToolStripButton();
		this.tbEditRedo = new System.Windows.Forms.ToolStripButton();
		this.tbEditSep1 = new System.Windows.Forms.ToolStripSeparator();
		this.tbEditMirrorHorizontal = new System.Windows.Forms.ToolStripButton();
		this.tbEditMirrorVertical = new System.Windows.Forms.ToolStripButton();
		this.tbEditRotateLeft = new System.Windows.Forms.ToolStripButton();
		this.tbEditRotateRight = new System.Windows.Forms.ToolStripButton();
		this.tbEditShiftUp = new System.Windows.Forms.ToolStripButton();
		this.tbEditShiftDown = new System.Windows.Forms.ToolStripButton();
		this.tbEditShiftLeft = new System.Windows.Forms.ToolStripButton();
		this.tbEditShiftRight = new System.Windows.Forms.ToolStripButton();
		this.tbEditSep2 = new System.Windows.Forms.ToolStripSeparator();
		this.tbEditReplaceColor = new System.Windows.Forms.ToolStripButton();
		this.tbEditSep3 = new System.Windows.Forms.ToolStripSeparator();
		this.tbPaletteLoadEmulatorState = new System.Windows.Forms.ToolStripButton();
		this.tbPaletteSep0 = new System.Windows.Forms.ToolStripSeparator();
		this.tbFileQuickSaveBitmap = new System.Windows.Forms.ToolStripButton();
		this.tbFileSep3 = new System.Windows.Forms.ToolStripSeparator();
		this.tbOptionExecuteFile = new System.Windows.Forms.ToolStripButton();
		this.tbWorkspaceLoad = new System.Windows.Forms.ToolStripButton();
		this.tbWorkspaceSave = new System.Windows.Forms.ToolStripButton();
		this.tbWorkspaceRemovePattern = new System.Windows.Forms.ToolStripButton();
		this.tbWorkspaceAdd = new System.Windows.Forms.ToolStripButton();
		this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.optionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.openDataFileDialog = new System.Windows.Forms.OpenFileDialog();
		this.saveDataFileDialog = new System.Windows.Forms.SaveFileDialog();
		this.openBitmapDialog = new System.Windows.Forms.OpenFileDialog();
		this.saveBitmapDialog = new System.Windows.Forms.SaveFileDialog();
		this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
		this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
		this.menuStripMain = new ControlLib.OneClickMenuStrip();
		this.miFile = new System.Windows.Forms.ToolStripMenuItem();
		this.miFileNew = new System.Windows.Forms.ToolStripMenuItem();
		this.miFileOpenRom = new System.Windows.Forms.ToolStripMenuItem();
		this.miFileReload = new System.Windows.Forms.ToolStripMenuItem();
		this.miFileSaveRom = new System.Windows.Forms.ToolStripMenuItem();
		this.miFileSaveAsRom = new System.Windows.Forms.ToolStripMenuItem();
		this.miFileSep1 = new System.Windows.Forms.ToolStripSeparator();
		this.miFileOpenBmp = new System.Windows.Forms.ToolStripMenuItem();
		this.miFileSaveBmp = new System.Windows.Forms.ToolStripMenuItem();
		this.miFileQuickSaveBitmap = new System.Windows.Forms.ToolStripMenuItem();
		this.miFileSep2 = new System.Windows.Forms.ToolStripSeparator();
		this.miFileExit = new System.Windows.Forms.ToolStripMenuItem();
		this.miEdit = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditUndo = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditRedo = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditSep0 = new System.Windows.Forms.ToolStripSeparator();
		this.miEditCut = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditCopy = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditPaste = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditPasteOptimizedImage = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditClear = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditSelectAll = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditSep1 = new System.Windows.Forms.ToolStripSeparator();
		this.miEditClearClipboard = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditSep2 = new System.Windows.Forms.ToolStripSeparator();
		this.miEditMirrorHorizontal = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditMirrorVertical = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditSep3 = new System.Windows.Forms.ToolStripSeparator();
		this.miEditRotateLeft = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditRotateRight = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditSep4 = new System.Windows.Forms.ToolStripSeparator();
		this.miEditShiftUp = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditShiftDown = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditShiftLeft = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditShiftRight = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditSep5 = new System.Windows.Forms.ToolStripSeparator();
		this.miEditReplaceColor = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddress = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddress0 = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddress1 = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddress2 = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddress3 = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddress4 = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddress5 = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddress6 = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddress7 = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddress8 = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddress9 = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddressSep0 = new System.Windows.Forms.ToolStripSeparator();
		this.miAddressInputAddress = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddressSep1 = new System.Windows.Forms.ToolStripSeparator();
		this.miAddressFindPrevious = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddressFindNext = new System.Windows.Forms.ToolStripMenuItem();
		this.miPen = new System.Windows.Forms.ToolStripMenuItem();
		this.miPalette = new System.Windows.Forms.ToolStripMenuItem();
		this.miPalettePaletteType = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteTypeDat = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteTypePal = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteTypeBmp = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteSelectPalette = new System.Windows.Forms.ToolStripMenuItem();
		this.miPalettePrev = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteNext = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteSelectSep0 = new System.Windows.Forms.ToolStripSeparator();
		this.miPaletteSelect0 = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteSelect1 = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteSelect2 = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteSelect3 = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteSelect4 = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteSelect5 = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteSelect6 = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteSelect7 = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteSelect8 = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteSelect9 = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteSelectA = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteSelectB = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteSelectC = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteSelectD = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteSelectE = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteSelectF = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteSep0 = new System.Windows.Forms.ToolStripSeparator();
		this.miPaletteLoadEmulatorState = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteSep1 = new System.Windows.Forms.ToolStripSeparator();
		this.miPalettePalOpen = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteLoadRGBPaletteFromCommon = new System.Windows.Forms.ToolStripMenuItem();
		this.miPalettePalSave = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteQuickSaveRGBPalette = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteSep2 = new System.Windows.Forms.ToolStripSeparator();
		this.miPaletteDatOpen = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteLoadPaletteTableFromCommon = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteDatSave = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteQuickSavePaletteTable = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteSep3 = new System.Windows.Forms.ToolStripSeparator();
		this.miPaletteOpenADF = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteLoadADFPatternFromCommon = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteSaveADF = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteQuickSaveADFPattern = new System.Windows.Forms.ToolStripMenuItem();
		this.miPaletteSep4 = new System.Windows.Forms.ToolStripSeparator();
		this.miPaletteLoadDefaultSetting = new System.Windows.Forms.ToolStripMenuItem();
		this.miOption = new System.Windows.Forms.ToolStripMenuItem();
		this.miOptionSetting = new System.Windows.Forms.ToolStripMenuItem();
		this.miOptionShowAllMenu = new System.Windows.Forms.ToolStripMenuItem();
		this.miOptionSep0 = new System.Windows.Forms.ToolStripSeparator();
		this.miOptionExecuteFile = new System.Windows.Forms.ToolStripMenuItem();
		this.miHelp = new System.Windows.Forms.ToolStripMenuItem();
		this.miHelpOpenWiki = new System.Windows.Forms.ToolStripMenuItem();
		this.miHelpSep0 = new System.Windows.Forms.ToolStripSeparator();
		this.miHelpOpenWebsite = new System.Windows.Forms.ToolStripMenuItem();
		this.miHelpReportBugs = new System.Windows.Forms.ToolStripMenuItem();
		this.miHelpSep1 = new System.Windows.Forms.ToolStripSeparator();
		this.miHelpPropertyEditor = new System.Windows.Forms.ToolStripMenuItem();
		this.miHelpSep2 = new System.Windows.Forms.ToolStripSeparator();
		this.miHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
		this.miOptionLanguage = new System.Windows.Forms.ToolStripMenuItem();
		this.miLanguageSystem = new System.Windows.Forms.ToolStripMenuItem();
		this.miLanguageSep0 = new System.Windows.Forms.ToolStripSeparator();
		this.miLanguageEnglish = new System.Windows.Forms.ToolStripMenuItem();
		this.miLanguageJapanese = new System.Windows.Forms.ToolStripMenuItem();
		this.miLanguageSep1 = new System.Windows.Forms.ToolStripSeparator();
		this.miLanguageLngFile = new System.Windows.Forms.ToolStripMenuItem();
		this.miLanguageSep2 = new System.Windows.Forms.ToolStripSeparator();
		this.miLanguageSettingAutoLoadLng = new System.Windows.Forms.ToolStripMenuItem();
		this.miLanguageExportLng = new System.Windows.Forms.ToolStripMenuItem();
		this.toolTip = new System.Windows.Forms.ToolTip(this.components);
		this.popupJumpListMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.tsWoekSpace = new ControlLib.OneClickToolStrip();
		this.panelWorkSpaceTS = new System.Windows.Forms.Panel();
		this.contextMenuStripChr = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.cmiEditCut = new System.Windows.Forms.ToolStripMenuItem();
		this.cmiEditCopy = new System.Windows.Forms.ToolStripMenuItem();
		this.cmiEditPaste = new System.Windows.Forms.ToolStripMenuItem();
		this.cmiEditClear = new System.Windows.Forms.ToolStripMenuItem();
		this.cmiEditSelectAll = new System.Windows.Forms.ToolStripMenuItem();
		this.lVersion = new System.Windows.Forms.Label();
		this.mFindWorker = new System.ComponentModel.BackgroundWorker();
		this.toolStripView.SuspendLayout();
		this.statusStrip.SuspendLayout();
		this.tabControl.SuspendLayout();
		this.tabPageChrRom.SuspendLayout();
		this.panelChr.SuspendLayout();
		this.panelChrSetting.SuspendLayout();
		this.panelToolStripAddress.SuspendLayout();
		this.toolStripAddress.SuspendLayout();
		this.scrollPanelRom.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cellSelectorRom).BeginInit();
		this.tabPageBitmap.SuspendLayout();
		this.scrollPanelBmp.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cellSelectorBmp).BeginInit();
		this.tabWorkSpace.SuspendLayout();
		this.panelWorkSpace.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.workSpaceSelector1).BeginInit();
		this.rightPanel.SuspendLayout();
		this.panelPalette.SuspendLayout();
		this.toolStripPalette.SuspendLayout();
		this.panelEdit.SuspendLayout();
		this.toolStripMain.SuspendLayout();
		this.menuStripMain.SuspendLayout();
		this.tsWoekSpace.SuspendLayout();
		this.panelWorkSpaceTS.SuspendLayout();
		this.contextMenuStripChr.SuspendLayout();
		base.SuspendLayout();
		resources.ApplyResources(this.toolStripView, "toolStripView");
		this.toolStripView.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		this.toolStripView.Items.AddRange(new System.Windows.Forms.ToolStripItem[8] { this.tbViewGuiRate, this.toolStripSeparator1, this.tbViewEditSize, this.toolStripSeparator2, this.tbViewGridStyle, this.toolStripSeparator3, this.tbViewGridBank, this.tbViewGridEditor });
		this.toolStripView.Name = "toolStripView";
		this.toolStripView.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		resources.ApplyResources(this.tbViewGuiRate, "tbViewGuiRate");
		this.tbViewGuiRate.Image = YYCHR.Properties.Resources.IconOptionSetting;
		this.tbViewGuiRate.Name = "tbViewGuiRate";
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
		resources.ApplyResources(this.tbViewEditSize, "tbViewEditSize");
		this.tbViewEditSize.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.tbViewEditorSize8, this.tbViewEditorSize16, this.tbViewEditorSize32, this.tbViewEditorSize64, this.tbViewEditorSize128 });
		this.tbViewEditSize.Image = YYCHR.Properties.Resources.IconOptionSetting;
		this.tbViewEditSize.Name = "tbViewEditSize";
		this.tbViewEditorSize8.Name = "tbViewEditorSize8";
		resources.ApplyResources(this.tbViewEditorSize8, "tbViewEditorSize8");
		this.tbViewEditorSize8.Tag = "8";
		this.tbViewEditorSize8.Click += new System.EventHandler(ActionViewEditorSizeSelected);
		this.tbViewEditorSize16.Name = "tbViewEditorSize16";
		resources.ApplyResources(this.tbViewEditorSize16, "tbViewEditorSize16");
		this.tbViewEditorSize16.Tag = "16";
		this.tbViewEditorSize16.Click += new System.EventHandler(ActionViewEditorSizeSelected);
		this.tbViewEditorSize32.Name = "tbViewEditorSize32";
		resources.ApplyResources(this.tbViewEditorSize32, "tbViewEditorSize32");
		this.tbViewEditorSize32.Tag = "32";
		this.tbViewEditorSize32.Click += new System.EventHandler(ActionViewEditorSizeSelected);
		this.tbViewEditorSize64.Name = "tbViewEditorSize64";
		resources.ApplyResources(this.tbViewEditorSize64, "tbViewEditorSize64");
		this.tbViewEditorSize64.Tag = "64";
		this.tbViewEditorSize64.Click += new System.EventHandler(ActionViewEditorSizeSelected);
		this.tbViewEditorSize128.Name = "tbViewEditorSize128";
		resources.ApplyResources(this.tbViewEditorSize128, "tbViewEditorSize128");
		this.tbViewEditorSize128.Tag = "128";
		this.tbViewEditorSize128.Click += new System.EventHandler(ActionViewEditorSizeSelected);
		this.toolStripSeparator2.Name = "toolStripSeparator2";
		resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
		resources.ApplyResources(this.tbViewGridStyle, "tbViewGridStyle");
		this.tbViewGridStyle.Image = YYCHR.Properties.Resources.IconOptionSetting;
		this.tbViewGridStyle.Name = "tbViewGridStyle";
		this.toolStripSeparator3.Name = "toolStripSeparator3";
		resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
		this.tbViewGridBank.Checked = true;
		this.tbViewGridBank.CheckOnClick = true;
		this.tbViewGridBank.CheckState = System.Windows.Forms.CheckState.Checked;
		this.tbViewGridBank.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbViewGridBank.Image = YYCHR.Properties.Resources.TbGridChr;
		resources.ApplyResources(this.tbViewGridBank, "tbViewGridBank");
		this.tbViewGridBank.Name = "tbViewGridBank";
		this.tbViewGridBank.Click += new System.EventHandler(ActionViewGridPict);
		this.tbViewGridEditor.Checked = true;
		this.tbViewGridEditor.CheckOnClick = true;
		this.tbViewGridEditor.CheckState = System.Windows.Forms.CheckState.Checked;
		this.tbViewGridEditor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbViewGridEditor.Image = YYCHR.Properties.Resources.TbGridEdit;
		resources.ApplyResources(this.tbViewGridEditor, "tbViewGridEditor");
		this.tbViewGridEditor.Name = "tbViewGridEditor";
		this.tbViewGridEditor.Click += new System.EventHandler(ActionViewGridEdit);
		this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[10] { this.slAddr, this.slXY, this.slChr, this.slRightSpace, this.slKeyCtrl, this.slKeyShift, this.slKeyAlt, this.slHintMouseButtonL, this.slHintMouseWheel, this.slHintMouseButtonR });
		resources.ApplyResources(this.statusStrip, "statusStrip");
		this.statusStrip.Name = "statusStrip";
		this.statusStrip.ShowItemToolTips = true;
		this.statusStrip.SizingGrip = false;
		this.slAddr.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.All;
		resources.ApplyResources(this.slAddr, "slAddr");
		this.slAddr.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
		this.slAddr.Name = "slAddr";
		resources.ApplyResources(this.slXY, "slXY");
		this.slXY.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.All;
		this.slXY.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
		this.slXY.Name = "slXY";
		resources.ApplyResources(this.slChr, "slChr");
		this.slChr.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.All;
		this.slChr.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
		this.slChr.Name = "slChr";
		this.slRightSpace.Name = "slRightSpace";
		resources.ApplyResources(this.slRightSpace, "slRightSpace");
		this.slRightSpace.Spring = true;
		this.slKeyCtrl.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.slKeyCtrl.Image = YYCHR.Properties.Resources.ControlKeyControl;
		resources.ApplyResources(this.slKeyCtrl, "slKeyCtrl");
		this.slKeyCtrl.Name = "slKeyCtrl";
		this.slKeyShift.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.slKeyShift.Image = YYCHR.Properties.Resources.ControlKeyShift;
		resources.ApplyResources(this.slKeyShift, "slKeyShift");
		this.slKeyShift.Name = "slKeyShift";
		this.slKeyAlt.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.slKeyAlt.Image = YYCHR.Properties.Resources.ControlKeyAlt;
		resources.ApplyResources(this.slKeyAlt, "slKeyAlt");
		this.slKeyAlt.Name = "slKeyAlt";
		resources.ApplyResources(this.slHintMouseButtonL, "slHintMouseButtonL");
		this.slHintMouseButtonL.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.All;
		this.slHintMouseButtonL.Image = YYCHR.Properties.Resources.ControlMouseButtonL;
		this.slHintMouseButtonL.Name = "slHintMouseButtonL";
		resources.ApplyResources(this.slHintMouseWheel, "slHintMouseWheel");
		this.slHintMouseWheel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.All;
		this.slHintMouseWheel.Image = YYCHR.Properties.Resources.ControlMouseWheel;
		this.slHintMouseWheel.Name = "slHintMouseWheel";
		resources.ApplyResources(this.slHintMouseButtonR, "slHintMouseButtonR");
		this.slHintMouseButtonR.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.All;
		this.slHintMouseButtonR.Image = YYCHR.Properties.Resources.ControlMouseButtonR;
		this.slHintMouseButtonR.Name = "slHintMouseButtonR";
		resources.ApplyResources(this.tabControl, "tabControl");
		this.tabControl.Controls.Add(this.tabPageChrRom);
		this.tabControl.Controls.Add(this.tabPageBitmap);
		this.tabControl.Controls.Add(this.tabWorkSpace);
		this.tabControl.Multiline = true;
		this.tabControl.Name = "tabControl";
		this.tabControl.SelectedIndex = 0;
		this.tabControl.TabStop = false;
		this.tabControl.SelectedIndexChanged += new System.EventHandler(pictTabControl_SelectedIndexChanged);
		this.tabPageChrRom.Controls.Add(this.panelChr);
		resources.ApplyResources(this.tabPageChrRom, "tabPageChrRom");
		this.tabPageChrRom.Name = "tabPageChrRom";
		this.panelChr.Controls.Add(this.panelForPlugin);
		this.panelChr.Controls.Add(this.panelChrSetting);
		this.panelChr.Controls.Add(this.panelToolStripAddress);
		this.panelChr.Controls.Add(this.scrollPanelRom);
		resources.ApplyResources(this.panelChr, "panelChr");
		this.panelChr.Name = "panelChr";
		resources.ApplyResources(this.panelForPlugin, "panelForPlugin");
		this.panelForPlugin.BackColor = System.Drawing.SystemColors.Control;
		this.panelForPlugin.Name = "panelForPlugin";
		this.panelChrSetting.Controls.Add(this.labelFormat);
		this.panelChrSetting.Controls.Add(this.buttonPatternEdit);
		this.panelChrSetting.Controls.Add(this.comboBoxFormat);
		this.panelChrSetting.Controls.Add(this.buttonFormatInfo);
		this.panelChrSetting.Controls.Add(this.comboBoxMirror);
		this.panelChrSetting.Controls.Add(this.labelPattern);
		this.panelChrSetting.Controls.Add(this.comboBoxPattern);
		this.panelChrSetting.Controls.Add(this.labelMirror);
		this.panelChrSetting.Controls.Add(this.comboBoxRotate);
		this.panelChrSetting.Controls.Add(this.labelRotate);
		resources.ApplyResources(this.panelChrSetting, "panelChrSetting");
		this.panelChrSetting.Name = "panelChrSetting";
		resources.ApplyResources(this.labelFormat, "labelFormat");
		this.labelFormat.Name = "labelFormat";
		resources.ApplyResources(this.buttonPatternEdit, "buttonPatternEdit");
		this.buttonPatternEdit.Name = "buttonPatternEdit";
		this.buttonPatternEdit.UseVisualStyleBackColor = true;
		this.buttonPatternEdit.Click += new System.EventHandler(actionPatternEdit);
		this.comboBoxFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxFormat.DropDownWidth = 160;
		this.comboBoxFormat.FormattingEnabled = true;
		resources.ApplyResources(this.comboBoxFormat, "comboBoxFormat");
		this.comboBoxFormat.Name = "comboBoxFormat";
		resources.ApplyResources(this.buttonFormatInfo, "buttonFormatInfo");
		this.buttonFormatInfo.Name = "buttonFormatInfo";
		this.buttonFormatInfo.UseVisualStyleBackColor = true;
		this.buttonFormatInfo.Click += new System.EventHandler(ActionShowFormatInfo);
		this.comboBoxMirror.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxMirror.FormattingEnabled = true;
		resources.ApplyResources(this.comboBoxMirror, "comboBoxMirror");
		this.comboBoxMirror.Name = "comboBoxMirror";
		resources.ApplyResources(this.labelPattern, "labelPattern");
		this.labelPattern.Name = "labelPattern";
		this.comboBoxPattern.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxPattern.DropDownWidth = 160;
		this.comboBoxPattern.FormattingEnabled = true;
		resources.ApplyResources(this.comboBoxPattern, "comboBoxPattern");
		this.comboBoxPattern.Name = "comboBoxPattern";
		resources.ApplyResources(this.labelMirror, "labelMirror");
		this.labelMirror.Name = "labelMirror";
		this.comboBoxRotate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxRotate.FormattingEnabled = true;
		resources.ApplyResources(this.comboBoxRotate, "comboBoxRotate");
		this.comboBoxRotate.Name = "comboBoxRotate";
		resources.ApplyResources(this.labelRotate, "labelRotate");
		this.labelRotate.Name = "labelRotate";
		this.panelToolStripAddress.Controls.Add(this.toolStripAddress);
		resources.ApplyResources(this.panelToolStripAddress, "panelToolStripAddress");
		this.panelToolStripAddress.Name = "panelToolStripAddress";
		resources.ApplyResources(this.toolStripAddress, "toolStripAddress");
		this.toolStripAddress.CanOverflow = false;
		this.toolStripAddress.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		this.toolStripAddress.Items.AddRange(new System.Windows.Forms.ToolStripItem[18]
		{
			this.tbAddres0, this.tbAddres1, this.tbAddres2, this.tbAddres3, this.tbAddres4, this.tbAddres5, this.tbAddres6, this.tbAddres7, this.tbAddres8, this.tbAddres9,
			this.tbAddresSep0, this.tbAddresInputAddress, this.tbAddressJumpList, this.tbAddressJumpListPrev, this.tbAddressJumpListNext, this.tbAddresSep1, this.tbAddressFindPrevious, this.tbAddressFindNext
		});
		this.toolStripAddress.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
		this.toolStripAddress.Name = "toolStripAddress";
		this.toolStripAddress.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		resources.ApplyResources(this.tbAddres0, "tbAddres0");
		this.tbAddres0.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddres0.Image = YYCHR.Properties.Resources.Addr0;
		this.tbAddres0.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddres0.Name = "tbAddres0";
		this.tbAddres0.Tag = "0";
		this.tbAddres0.Click += new System.EventHandler(ActionAddresChange);
		resources.ApplyResources(this.tbAddres1, "tbAddres1");
		this.tbAddres1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddres1.Image = YYCHR.Properties.Resources.Addr1;
		this.tbAddres1.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddres1.Name = "tbAddres1";
		this.tbAddres1.Tag = "1";
		this.tbAddres1.Click += new System.EventHandler(ActionAddresChange);
		resources.ApplyResources(this.tbAddres2, "tbAddres2");
		this.tbAddres2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddres2.Image = YYCHR.Properties.Resources.Addr2;
		this.tbAddres2.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddres2.Name = "tbAddres2";
		this.tbAddres2.Tag = "2";
		this.tbAddres2.Click += new System.EventHandler(ActionAddresChange);
		resources.ApplyResources(this.tbAddres3, "tbAddres3");
		this.tbAddres3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddres3.Image = YYCHR.Properties.Resources.Addr3;
		this.tbAddres3.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddres3.Name = "tbAddres3";
		this.tbAddres3.Tag = "3";
		this.tbAddres3.Click += new System.EventHandler(ActionAddresChange);
		resources.ApplyResources(this.tbAddres4, "tbAddres4");
		this.tbAddres4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddres4.Image = YYCHR.Properties.Resources.Addr4;
		this.tbAddres4.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddres4.Name = "tbAddres4";
		this.tbAddres4.Tag = "4";
		this.tbAddres4.Click += new System.EventHandler(ActionAddresChange);
		resources.ApplyResources(this.tbAddres5, "tbAddres5");
		this.tbAddres5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddres5.Image = YYCHR.Properties.Resources.Addr5;
		this.tbAddres5.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddres5.Name = "tbAddres5";
		this.tbAddres5.Tag = "5";
		this.tbAddres5.Click += new System.EventHandler(ActionAddresChange);
		resources.ApplyResources(this.tbAddres6, "tbAddres6");
		this.tbAddres6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddres6.Image = YYCHR.Properties.Resources.Addr6;
		this.tbAddres6.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddres6.Name = "tbAddres6";
		this.tbAddres6.Tag = "6";
		this.tbAddres6.Click += new System.EventHandler(ActionAddresChange);
		resources.ApplyResources(this.tbAddres7, "tbAddres7");
		this.tbAddres7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddres7.Image = YYCHR.Properties.Resources.Addr7;
		this.tbAddres7.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddres7.Name = "tbAddres7";
		this.tbAddres7.Tag = "7";
		this.tbAddres7.Click += new System.EventHandler(ActionAddresChange);
		resources.ApplyResources(this.tbAddres8, "tbAddres8");
		this.tbAddres8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddres8.Image = YYCHR.Properties.Resources.Addr8;
		this.tbAddres8.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddres8.Name = "tbAddres8";
		this.tbAddres8.Tag = "8";
		this.tbAddres8.Click += new System.EventHandler(ActionAddresChange);
		resources.ApplyResources(this.tbAddres9, "tbAddres9");
		this.tbAddres9.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddres9.Image = YYCHR.Properties.Resources.Addr9;
		this.tbAddres9.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddres9.Name = "tbAddres9";
		this.tbAddres9.Tag = "9";
		this.tbAddres9.Click += new System.EventHandler(ActionAddresChange);
		resources.ApplyResources(this.tbAddresSep0, "tbAddresSep0");
		this.tbAddresSep0.Name = "tbAddresSep0";
		resources.ApplyResources(this.tbAddresInputAddress, "tbAddresInputAddress");
		this.tbAddresInputAddress.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddresInputAddress.Image = YYCHR.Properties.Resources.AddrA;
		this.tbAddresInputAddress.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddresInputAddress.Name = "tbAddresInputAddress";
		this.tbAddresInputAddress.Tag = "10";
		this.tbAddresInputAddress.Click += new System.EventHandler(ActionAddressInputAddress);
		resources.ApplyResources(this.tbAddressJumpList, "tbAddressJumpList");
		this.tbAddressJumpList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddressJumpList.Image = YYCHR.Properties.Resources.AddrJumpList;
		this.tbAddressJumpList.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddressJumpList.Name = "tbAddressJumpList";
		this.tbAddressJumpList.Click += new System.EventHandler(ActionAddressJumpList);
		resources.ApplyResources(this.tbAddressJumpListPrev, "tbAddressJumpListPrev");
		this.tbAddressJumpListPrev.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddressJumpListPrev.Image = YYCHR.Properties.Resources.AddrJumpListPrev;
		this.tbAddressJumpListPrev.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddressJumpListPrev.Name = "tbAddressJumpListPrev";
		this.tbAddressJumpListPrev.Click += new System.EventHandler(ActionAddressJumpListPrevNext);
		resources.ApplyResources(this.tbAddressJumpListNext, "tbAddressJumpListNext");
		this.tbAddressJumpListNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddressJumpListNext.Image = YYCHR.Properties.Resources.AddrJumpListNext;
		this.tbAddressJumpListNext.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddressJumpListNext.Name = "tbAddressJumpListNext";
		this.tbAddressJumpListNext.Click += new System.EventHandler(ActionAddressJumpListPrevNext);
		resources.ApplyResources(this.tbAddresSep1, "tbAddresSep1");
		this.tbAddresSep1.Name = "tbAddresSep1";
		resources.ApplyResources(this.tbAddressFindPrevious, "tbAddressFindPrevious");
		this.tbAddressFindPrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddressFindPrevious.Image = YYCHR.Properties.Resources.AddrFindPrev;
		this.tbAddressFindPrevious.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddressFindPrevious.Name = "tbAddressFindPrevious";
		this.tbAddressFindPrevious.Click += new System.EventHandler(ActionAddressFindPrevious);
		resources.ApplyResources(this.tbAddressFindNext, "tbAddressFindNext");
		this.tbAddressFindNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddressFindNext.Image = YYCHR.Properties.Resources.AddrFindNext;
		this.tbAddressFindNext.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddressFindNext.Name = "tbAddressFindNext";
		this.tbAddressFindNext.Click += new System.EventHandler(ActionAddressFindNext);
		this.scrollPanelRom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.scrollPanelRom.ClientAreaSize = new System.Drawing.Size(256, 256);
		this.scrollPanelRom.Controls.Add(this.cellSelectorRom);
		this.scrollPanelRom.LargeChange = 10;
		resources.ApplyResources(this.scrollPanelRom, "scrollPanelRom");
		this.scrollPanelRom.LrChange = 1;
		this.scrollPanelRom.Maximum = 100;
		this.scrollPanelRom.Minimum = 0;
		this.scrollPanelRom.Name = "scrollPanelRom";
		this.scrollPanelRom.ScrollBarType = ControlLib.ScrollPanel.ScrollBarTypes.Vertical;
		this.scrollPanelRom.SmallChange = 1;
		this.scrollPanelRom.TabStop = true;
		this.scrollPanelRom.Value = 0;
		this.scrollPanelRom.WheelRate = 4;
		this.scrollPanelRom.Scrolled += new System.EventHandler(scrollPanelRom_Scrolled);
		this.scrollPanelRom.KeyDown += new System.Windows.Forms.KeyEventHandler(scrollPanel_KeyDown);
		this.scrollPanelRom.WheelSizeChange += new System.Windows.Forms.MouseEventHandler(ActionBankWheelSizeChange);
		this.cellSelectorRom.DefaultSelectSize = new System.Drawing.Size(16, 16);
		this.cellSelectorRom.EnableRightDragSelect = true;
		this.cellSelectorRom.FreeSelect = false;
		this.cellSelectorRom.GridColor1 = System.Drawing.Color.FromArgb(128, 255, 255, 255);
		this.cellSelectorRom.GridColor2 = System.Drawing.Color.FromArgb(128, 0, 0, 0);
		this.cellSelectorRom.GridStyle = ControlLib.GridStyle.Dot;
		this.cellSelectorRom.Image = null;
		resources.ApplyResources(this.cellSelectorRom, "cellSelectorRom");
		this.cellSelectorRom.MouseDownNew = true;
		this.cellSelectorRom.Name = "cellSelectorRom";
		this.cellSelectorRom.PixelSelect = false;
		this.cellSelectorRom.SelectedColor1 = System.Drawing.Color.White;
		this.cellSelectorRom.SelectedColor2 = System.Drawing.Color.Aqua;
		this.cellSelectorRom.SelectedIndex = 0;
		this.cellSelectorRom.SelectedRect = new System.Drawing.Rectangle(8, 8, 16, 16);
		this.cellSelectorRom.TabStop = false;
		this.cellSelectorRom.ZoomRate = 2;
		this.cellSelectorRom.Selected += new System.Windows.Forms.MouseEventHandler(cellSelector_Selected);
		this.cellSelectorRom.MouseEnter += new System.EventHandler(ActionControlNavigationUpdate_MouseEnter);
		this.cellSelectorRom.MouseLeave += new System.EventHandler(ActionControlNavigationUpdate_MouseLeave);
		this.tabPageBitmap.BackColor = System.Drawing.SystemColors.Control;
		this.tabPageBitmap.Controls.Add(this.scrollPanelBmp);
		resources.ApplyResources(this.tabPageBitmap, "tabPageBitmap");
		this.tabPageBitmap.Name = "tabPageBitmap";
		this.scrollPanelBmp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.scrollPanelBmp.ClientAreaSize = new System.Drawing.Size(576, 576);
		this.scrollPanelBmp.Controls.Add(this.cellSelectorBmp);
		this.scrollPanelBmp.LargeChange = 10;
		resources.ApplyResources(this.scrollPanelBmp, "scrollPanelBmp");
		this.scrollPanelBmp.Name = "scrollPanelBmp";
		this.scrollPanelBmp.SmallChange = 8;
		this.scrollPanelBmp.WheelRate = 4;
		this.scrollPanelBmp.WheelSizeChange += new System.Windows.Forms.MouseEventHandler(ActionBankWheelSizeChange);
		this.scrollPanelBmp.KeyDown += new System.Windows.Forms.KeyEventHandler(scrollPanel_KeyDown);
		this.cellSelectorBmp.DefaultSelectSize = new System.Drawing.Size(16, 16);
		this.cellSelectorBmp.EnableRightDragSelect = true;
		this.cellSelectorBmp.FreeSelect = false;
		this.cellSelectorBmp.GridColor1 = System.Drawing.Color.White;
		this.cellSelectorBmp.GridColor2 = System.Drawing.Color.Black;
		this.cellSelectorBmp.GridStyle = ControlLib.GridStyle.Dot;
		this.cellSelectorBmp.Image = null;
		resources.ApplyResources(this.cellSelectorBmp, "cellSelectorBmp");
		this.cellSelectorBmp.MouseDownNew = true;
		this.cellSelectorBmp.Name = "cellSelectorBmp";
		this.cellSelectorBmp.PixelSelect = false;
		this.cellSelectorBmp.SelectedColor1 = System.Drawing.Color.White;
		this.cellSelectorBmp.SelectedColor2 = System.Drawing.Color.Aqua;
		this.cellSelectorBmp.SelectedIndex = 0;
		this.cellSelectorBmp.SelectedRect = new System.Drawing.Rectangle(0, 0, 16, 16);
		this.cellSelectorBmp.TabStop = false;
		this.cellSelectorBmp.ZoomRate = 2;
		this.cellSelectorBmp.Selected += new System.Windows.Forms.MouseEventHandler(cellSelector_Selected);
		this.cellSelectorBmp.MouseEnter += new System.EventHandler(ActionControlNavigationUpdate_MouseEnter);
		this.cellSelectorBmp.MouseLeave += new System.EventHandler(ActionControlNavigationUpdate_MouseLeave);
		this.tabWorkSpace.Controls.Add(this.panelWorkSpace);
		resources.ApplyResources(this.tabWorkSpace, "tabWorkSpace");
		this.tabWorkSpace.Name = "tabWorkSpace";
		this.tabWorkSpace.UseVisualStyleBackColor = true;
		this.panelWorkSpace.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.panelWorkSpace.Controls.Add(this.workSpaceSelector1);
		resources.ApplyResources(this.panelWorkSpace, "panelWorkSpace");
		this.panelWorkSpace.Name = "panelWorkSpace";
		this.workSpaceSelector1.BackGroundColor = System.Drawing.Color.Green;
		this.workSpaceSelector1.Data = null;
		this.workSpaceSelector1.DefaultSelectSize = new System.Drawing.Size(16, 16);
		this.workSpaceSelector1.EnableRightDragSelect = true;
		this.workSpaceSelector1.ForceDraw = false;
		this.workSpaceSelector1.FreeSelect = false;
		this.workSpaceSelector1.GridColor1 = System.Drawing.Color.White;
		this.workSpaceSelector1.GridColor2 = System.Drawing.Color.Gray;
		this.workSpaceSelector1.GridStyle = ControlLib.GridStyle.Dot;
		this.workSpaceSelector1.Image = null;
		resources.ApplyResources(this.workSpaceSelector1, "workSpaceSelector1");
		this.workSpaceSelector1.MouseDownNew = true;
		this.workSpaceSelector1.Name = "workSpaceSelector1";
		this.workSpaceSelector1.PixelSelect = false;
		this.workSpaceSelector1.SelectedColor1 = System.Drawing.Color.White;
		this.workSpaceSelector1.SelectedColor2 = System.Drawing.Color.Aqua;
		this.workSpaceSelector1.SelectedIndex = 0;
		this.workSpaceSelector1.SelectedRect = new System.Drawing.Rectangle(0, 0, 16, 16);
		this.workSpaceSelector1.TabStop = false;
		this.workSpaceSelector1.ZoomRate = 2;
		this.workSpaceSelector1.Selected += new System.Windows.Forms.MouseEventHandler(cellSelector_Selected);
		this.rightPanel.Controls.Add(this.panelPalette);
		this.rightPanel.Controls.Add(this.panelEdit);
		resources.ApplyResources(this.rightPanel, "rightPanel");
		this.rightPanel.Name = "rightPanel";
		this.panelPalette.Controls.Add(this.toolStripPalette);
		this.panelPalette.Controls.Add(this.palPaletteSelector);
		this.panelPalette.Controls.Add(this.datPaletteSelector);
		resources.ApplyResources(this.panelPalette, "panelPalette");
		this.panelPalette.Name = "panelPalette";
		resources.ApplyResources(this.toolStripPalette, "toolStripPalette");
		this.toolStripPalette.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		this.toolStripPalette.Items.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.tbPaletteTypeDat, this.tbPaletteTypePal, this.tbPaletteTypeBmp, this.tbPaletteSep1, this.tbPaletteOpenState });
		this.toolStripPalette.Name = "toolStripPalette";
		this.toolStripPalette.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		resources.ApplyResources(this.tbPaletteTypeDat, "tbPaletteTypeDat");
		this.tbPaletteTypeDat.Checked = true;
		this.tbPaletteTypeDat.CheckOnClick = true;
		this.tbPaletteTypeDat.CheckState = System.Windows.Forms.CheckState.Checked;
		this.tbPaletteTypeDat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbPaletteTypeDat.Image = YYCHR.Properties.Resources.PaletteTypePalTable;
		this.tbPaletteTypeDat.Margin = new System.Windows.Forms.Padding(0);
		this.tbPaletteTypeDat.Name = "tbPaletteTypeDat";
		this.tbPaletteTypeDat.Tag = "0";
		this.tbPaletteTypeDat.Click += new System.EventHandler(ActionPaletteType);
		resources.ApplyResources(this.tbPaletteTypePal, "tbPaletteTypePal");
		this.tbPaletteTypePal.CheckOnClick = true;
		this.tbPaletteTypePal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbPaletteTypePal.Image = YYCHR.Properties.Resources.PaletteTypePal;
		this.tbPaletteTypePal.Margin = new System.Windows.Forms.Padding(0);
		this.tbPaletteTypePal.Name = "tbPaletteTypePal";
		this.tbPaletteTypePal.Tag = "1";
		this.tbPaletteTypePal.Click += new System.EventHandler(ActionPaletteType);
		resources.ApplyResources(this.tbPaletteTypeBmp, "tbPaletteTypeBmp");
		this.tbPaletteTypeBmp.CheckOnClick = true;
		this.tbPaletteTypeBmp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbPaletteTypeBmp.Image = YYCHR.Properties.Resources.PaletteTypeBmp;
		this.tbPaletteTypeBmp.Margin = new System.Windows.Forms.Padding(0);
		this.tbPaletteTypeBmp.Name = "tbPaletteTypeBmp";
		this.tbPaletteTypeBmp.Tag = "2";
		this.tbPaletteTypeBmp.Click += new System.EventHandler(ActionPaletteType);
		this.tbPaletteSep1.Name = "tbPaletteSep1";
		resources.ApplyResources(this.tbPaletteSep1, "tbPaletteSep1");
		resources.ApplyResources(this.tbPaletteOpenState, "tbPaletteOpenState");
		this.tbPaletteOpenState.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbPaletteOpenState.Image = YYCHR.Properties.Resources.PaletteOpenState;
		this.tbPaletteOpenState.Margin = new System.Windows.Forms.Padding(0);
		this.tbPaletteOpenState.Name = "tbPaletteOpenState";
		this.tbPaletteOpenState.Click += new System.EventHandler(ActionPaletteLoadEmulatorState);
		this.palPaletteSelector.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.palPaletteSelector.CellColumnCount = 16;
		this.palPaletteSelector.CellColumnView = 16;
		this.palPaletteSelector.CellHeight = 16;
		this.palPaletteSelector.CellRowCount = 16;
		this.palPaletteSelector.CellRowView = 4;
		this.palPaletteSelector.CellWidth = 16;
		this.palPaletteSelector.ColorCount = 256;
		resources.ApplyResources(this.palPaletteSelector, "palPaletteSelector");
		this.palPaletteSelector.LabelItem = ControlLib.LabelItem.Index;
		this.palPaletteSelector.LabelStyle = ControlLib.LabelStyle.Selected;
		this.palPaletteSelector.Name = "palPaletteSelector";
		this.palPaletteSelector.Palette = new System.Drawing.Color[256]
		{
			System.Drawing.Color.White,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty
		};
		this.palPaletteSelector.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.palPaletteSelector.SelectedIndex = 0;
		this.palPaletteSelector.SetSize = 0;
		this.palPaletteSelector.ShowSetRect = true;
		this.palPaletteSelector.OnPaletteSelect += new ControlLib.PaletteSelector.OnPaletteSelectHandler(ActionPalPaletteSelected);
		this.palPaletteSelector.PopupEditor += new System.EventHandler(ActionPopupRgbEditorFromPal);
		this.palPaletteSelector.MouseDown += new System.Windows.Forms.MouseEventHandler(ActionPaletteSelector_MouseDown);
		this.palPaletteSelector.MouseEnter += new System.EventHandler(ActionControlNavigationUpdate_MouseEnter);
		this.palPaletteSelector.MouseLeave += new System.EventHandler(ActionControlNavigationUpdate_MouseLeave);
		this.datPaletteSelector.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.datPaletteSelector.CellColumnCount = 16;
		this.datPaletteSelector.CellColumnView = 16;
		this.datPaletteSelector.CellHeight = 16;
		this.datPaletteSelector.CellRowCount = 16;
		this.datPaletteSelector.CellRowView = 4;
		this.datPaletteSelector.CellWidth = 16;
		this.datPaletteSelector.ColorCount = 256;
		resources.ApplyResources(this.datPaletteSelector, "datPaletteSelector");
		this.datPaletteSelector.LabelItem = ControlLib.LabelItem.LabelsProperty;
		this.datPaletteSelector.LabelStyle = ControlLib.LabelStyle.SelectedAll;
		this.datPaletteSelector.Name = "datPaletteSelector";
		this.datPaletteSelector.Palette = new System.Drawing.Color[256]
		{
			System.Drawing.Color.White,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty
		};
		this.datPaletteSelector.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.datPaletteSelector.SelectedIndex = 0;
		this.datPaletteSelector.SetSize = 4;
		this.datPaletteSelector.ShowSetRect = true;
		this.datPaletteSelector.OnPaletteSelect += new ControlLib.PaletteSelector.OnPaletteSelectHandler(ActionDatPaletteSelected);
		this.datPaletteSelector.OnPaletteSetChanged += new ControlLib.PaletteSelector.OnPaletteSetChangedHandler(ActionPaletteSetChanged);
		this.datPaletteSelector.PopupEditor += new System.EventHandler(ActionPopupRgbEditorFromDat);
		this.datPaletteSelector.MouseDown += new System.Windows.Forms.MouseEventHandler(ActionPaletteSelector_MouseDown);
		this.datPaletteSelector.MouseEnter += new System.EventHandler(ActionControlNavigationUpdate_MouseEnter);
		this.datPaletteSelector.MouseLeave += new System.EventHandler(ActionControlNavigationUpdate_MouseLeave);
		this.panelEdit.Controls.Add(this.toolStripPen);
		this.panelEdit.Controls.Add(this.editPanel);
		resources.ApplyResources(this.panelEdit, "panelEdit");
		this.panelEdit.Name = "panelEdit";
		resources.ApplyResources(this.toolStripPen, "toolStripPen");
		this.toolStripPen.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		this.toolStripPen.Name = "toolStripPen";
		this.toolStripPen.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		this.editPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.editPanel.ClipboardBitmap = null;
		this.editPanel.EditFunction = null;
		this.editPanel.GridColor1 = System.Drawing.Color.FromArgb(128, 255, 255, 255);
		this.editPanel.GridColor2 = System.Drawing.Color.FromArgb(128, 0, 0, 0);
		resources.ApplyResources(this.editPanel, "editPanel");
		this.editPanel.Name = "editPanel";
		this.editPanel.SelectedPalette = 0;
		this.editPanel.SourceBitmap = null;
		this.editPanel.SourceBytemap = null;
		this.editPanel.SourceRect = new System.Drawing.Rectangle(0, 0, 8, 8);
		this.editPanel.OnEdited += new System.EventHandler(Edited);
		this.editPanel.OnPicked += new System.EventHandler(editPanel_OnPicked);
		this.editPanel.OnNaviUpdated += new System.EventHandler(editPanel_OnNaviUpdated);
		this.editPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(editPanel_MouseDown);
		this.editPanel.MouseEnter += new System.EventHandler(ActionControlNavigationUpdate_MouseEnter);
		this.editPanel.MouseLeave += new System.EventHandler(ActionControlNavigationUpdate_MouseLeave);
		this.editPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(editPanel_MouseUp);
		resources.ApplyResources(this.toolStripMain, "toolStripMain");
		this.toolStripMain.CanOverflow = false;
		this.toolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[31]
		{
			this.tbFileNew, this.tbFileSep0, this.tbFileOpenRom, this.tbFileSaveRom, this.tbFileOpenBmp, this.tbFileSaveBmp, this.tbFileSep1, this.tbEditCut, this.tbEditCopy, this.tbEditPaste,
			this.tbEditClear, this.tbEditSep0, this.tbEditUndo, this.tbEditRedo, this.tbEditSep1, this.tbEditMirrorHorizontal, this.tbEditMirrorVertical, this.tbEditRotateLeft, this.tbEditRotateRight, this.tbEditShiftUp,
			this.tbEditShiftDown, this.tbEditShiftLeft, this.tbEditShiftRight, this.tbEditSep2, this.tbEditReplaceColor, this.tbEditSep3, this.tbPaletteLoadEmulatorState, this.tbPaletteSep0, this.tbFileQuickSaveBitmap, this.tbFileSep3,
			this.tbOptionExecuteFile
		});
		this.toolStripMain.Name = "toolStripMain";
		this.toolStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		this.toolStripMain.Stretch = true;
		resources.ApplyResources(this.tbFileNew, "tbFileNew");
		this.tbFileNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbFileNew.Image = YYCHR.Properties.Resources.FileNew;
		this.tbFileNew.Margin = new System.Windows.Forms.Padding(0);
		this.tbFileNew.Name = "tbFileNew";
		this.tbFileNew.Click += new System.EventHandler(ActionFileNew);
		this.tbFileSep0.Name = "tbFileSep0";
		resources.ApplyResources(this.tbFileSep0, "tbFileSep0");
		resources.ApplyResources(this.tbFileOpenRom, "tbFileOpenRom");
		this.tbFileOpenRom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbFileOpenRom.Image = YYCHR.Properties.Resources.FileOpen;
		this.tbFileOpenRom.Margin = new System.Windows.Forms.Padding(0);
		this.tbFileOpenRom.Name = "tbFileOpenRom";
		this.tbFileOpenRom.Click += new System.EventHandler(ActionFileOpen);
		resources.ApplyResources(this.tbFileSaveRom, "tbFileSaveRom");
		this.tbFileSaveRom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbFileSaveRom.Image = YYCHR.Properties.Resources.FileSave;
		this.tbFileSaveRom.Margin = new System.Windows.Forms.Padding(0);
		this.tbFileSaveRom.Name = "tbFileSaveRom";
		this.tbFileSaveRom.Click += new System.EventHandler(ActionFileSave);
		resources.ApplyResources(this.tbFileOpenBmp, "tbFileOpenBmp");
		this.tbFileOpenBmp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbFileOpenBmp.Image = YYCHR.Properties.Resources.FileOpenBmp;
		this.tbFileOpenBmp.Margin = new System.Windows.Forms.Padding(0);
		this.tbFileOpenBmp.Name = "tbFileOpenBmp";
		this.tbFileOpenBmp.Click += new System.EventHandler(ActionFileOpenBitmap);
		resources.ApplyResources(this.tbFileSaveBmp, "tbFileSaveBmp");
		this.tbFileSaveBmp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbFileSaveBmp.Image = YYCHR.Properties.Resources.FileSaveBmp;
		this.tbFileSaveBmp.Margin = new System.Windows.Forms.Padding(0);
		this.tbFileSaveBmp.Name = "tbFileSaveBmp";
		this.tbFileSaveBmp.Click += new System.EventHandler(ActionFileSaveBitmap);
		this.tbFileSep1.Name = "tbFileSep1";
		resources.ApplyResources(this.tbFileSep1, "tbFileSep1");
		resources.ApplyResources(this.tbEditCut, "tbEditCut");
		this.tbEditCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbEditCut.Image = YYCHR.Properties.Resources.EditCut;
		this.tbEditCut.Margin = new System.Windows.Forms.Padding(0);
		this.tbEditCut.Name = "tbEditCut";
		this.tbEditCut.Click += new System.EventHandler(ActionEditCut);
		resources.ApplyResources(this.tbEditCopy, "tbEditCopy");
		this.tbEditCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbEditCopy.Image = YYCHR.Properties.Resources.EditCopy;
		this.tbEditCopy.Margin = new System.Windows.Forms.Padding(0);
		this.tbEditCopy.Name = "tbEditCopy";
		this.tbEditCopy.Click += new System.EventHandler(ActionEditCopy);
		resources.ApplyResources(this.tbEditPaste, "tbEditPaste");
		this.tbEditPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbEditPaste.Image = YYCHR.Properties.Resources.EditPaste;
		this.tbEditPaste.Margin = new System.Windows.Forms.Padding(0);
		this.tbEditPaste.Name = "tbEditPaste";
		this.tbEditPaste.Click += new System.EventHandler(ActionEditPaste);
		resources.ApplyResources(this.tbEditClear, "tbEditClear");
		this.tbEditClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbEditClear.Image = YYCHR.Properties.Resources.EditDelete;
		this.tbEditClear.Margin = new System.Windows.Forms.Padding(0);
		this.tbEditClear.Name = "tbEditClear";
		this.tbEditClear.Click += new System.EventHandler(ActionEditClear);
		this.tbEditSep0.Name = "tbEditSep0";
		resources.ApplyResources(this.tbEditSep0, "tbEditSep0");
		resources.ApplyResources(this.tbEditUndo, "tbEditUndo");
		this.tbEditUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbEditUndo.Image = YYCHR.Properties.Resources.EditUndo;
		this.tbEditUndo.Margin = new System.Windows.Forms.Padding(0);
		this.tbEditUndo.Name = "tbEditUndo";
		this.tbEditUndo.Click += new System.EventHandler(ActionEditUndo);
		resources.ApplyResources(this.tbEditRedo, "tbEditRedo");
		this.tbEditRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbEditRedo.Image = YYCHR.Properties.Resources.EditRedo;
		this.tbEditRedo.Margin = new System.Windows.Forms.Padding(0);
		this.tbEditRedo.Name = "tbEditRedo";
		this.tbEditRedo.Click += new System.EventHandler(ActionEditRedo);
		this.tbEditSep1.Name = "tbEditSep1";
		resources.ApplyResources(this.tbEditSep1, "tbEditSep1");
		resources.ApplyResources(this.tbEditMirrorHorizontal, "tbEditMirrorHorizontal");
		this.tbEditMirrorHorizontal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbEditMirrorHorizontal.Image = YYCHR.Properties.Resources.EditMirrorH;
		this.tbEditMirrorHorizontal.Margin = new System.Windows.Forms.Padding(0);
		this.tbEditMirrorHorizontal.Name = "tbEditMirrorHorizontal";
		this.tbEditMirrorHorizontal.Click += new System.EventHandler(ActionEditMirror);
		resources.ApplyResources(this.tbEditMirrorVertical, "tbEditMirrorVertical");
		this.tbEditMirrorVertical.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbEditMirrorVertical.Image = YYCHR.Properties.Resources.EditMirrorV;
		this.tbEditMirrorVertical.Margin = new System.Windows.Forms.Padding(0);
		this.tbEditMirrorVertical.Name = "tbEditMirrorVertical";
		this.tbEditMirrorVertical.Click += new System.EventHandler(ActionEditMirror);
		resources.ApplyResources(this.tbEditRotateLeft, "tbEditRotateLeft");
		this.tbEditRotateLeft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbEditRotateLeft.Image = YYCHR.Properties.Resources.EditRotateL;
		this.tbEditRotateLeft.Margin = new System.Windows.Forms.Padding(0);
		this.tbEditRotateLeft.Name = "tbEditRotateLeft";
		this.tbEditRotateLeft.Click += new System.EventHandler(ActionEditRotate);
		resources.ApplyResources(this.tbEditRotateRight, "tbEditRotateRight");
		this.tbEditRotateRight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbEditRotateRight.Image = YYCHR.Properties.Resources.EditRotateR;
		this.tbEditRotateRight.Margin = new System.Windows.Forms.Padding(0);
		this.tbEditRotateRight.Name = "tbEditRotateRight";
		this.tbEditRotateRight.Click += new System.EventHandler(ActionEditRotate);
		resources.ApplyResources(this.tbEditShiftUp, "tbEditShiftUp");
		this.tbEditShiftUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbEditShiftUp.Image = YYCHR.Properties.Resources.EditShiftU;
		this.tbEditShiftUp.Margin = new System.Windows.Forms.Padding(0);
		this.tbEditShiftUp.Name = "tbEditShiftUp";
		this.tbEditShiftUp.Click += new System.EventHandler(ActionEditShift);
		resources.ApplyResources(this.tbEditShiftDown, "tbEditShiftDown");
		this.tbEditShiftDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbEditShiftDown.Image = YYCHR.Properties.Resources.EditShiftD;
		this.tbEditShiftDown.Margin = new System.Windows.Forms.Padding(0);
		this.tbEditShiftDown.Name = "tbEditShiftDown";
		this.tbEditShiftDown.Click += new System.EventHandler(ActionEditShift);
		resources.ApplyResources(this.tbEditShiftLeft, "tbEditShiftLeft");
		this.tbEditShiftLeft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbEditShiftLeft.Image = YYCHR.Properties.Resources.EditShiftL;
		this.tbEditShiftLeft.Margin = new System.Windows.Forms.Padding(0);
		this.tbEditShiftLeft.Name = "tbEditShiftLeft";
		this.tbEditShiftLeft.Click += new System.EventHandler(ActionEditShift);
		resources.ApplyResources(this.tbEditShiftRight, "tbEditShiftRight");
		this.tbEditShiftRight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbEditShiftRight.Image = YYCHR.Properties.Resources.EditShiftR;
		this.tbEditShiftRight.Margin = new System.Windows.Forms.Padding(0);
		this.tbEditShiftRight.Name = "tbEditShiftRight";
		this.tbEditShiftRight.Click += new System.EventHandler(ActionEditShift);
		this.tbEditSep2.Name = "tbEditSep2";
		resources.ApplyResources(this.tbEditSep2, "tbEditSep2");
		resources.ApplyResources(this.tbEditReplaceColor, "tbEditReplaceColor");
		this.tbEditReplaceColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbEditReplaceColor.Image = YYCHR.Properties.Resources.EditParetteReplace;
		this.tbEditReplaceColor.Margin = new System.Windows.Forms.Padding(0);
		this.tbEditReplaceColor.Name = "tbEditReplaceColor";
		this.tbEditReplaceColor.Click += new System.EventHandler(ActionEditReplaceColor);
		this.tbEditSep3.Name = "tbEditSep3";
		resources.ApplyResources(this.tbEditSep3, "tbEditSep3");
		resources.ApplyResources(this.tbPaletteLoadEmulatorState, "tbPaletteLoadEmulatorState");
		this.tbPaletteLoadEmulatorState.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbPaletteLoadEmulatorState.Image = YYCHR.Properties.Resources.PaletteOpenState;
		this.tbPaletteLoadEmulatorState.Margin = new System.Windows.Forms.Padding(0);
		this.tbPaletteLoadEmulatorState.Name = "tbPaletteLoadEmulatorState";
		this.tbPaletteLoadEmulatorState.Click += new System.EventHandler(ActionPaletteLoadEmulatorState);
		this.tbPaletteSep0.Name = "tbPaletteSep0";
		resources.ApplyResources(this.tbPaletteSep0, "tbPaletteSep0");
		resources.ApplyResources(this.tbFileQuickSaveBitmap, "tbFileQuickSaveBitmap");
		this.tbFileQuickSaveBitmap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbFileQuickSaveBitmap.Image = YYCHR.Properties.Resources.FileSaveBmp;
		this.tbFileQuickSaveBitmap.Margin = new System.Windows.Forms.Padding(0);
		this.tbFileQuickSaveBitmap.Name = "tbFileQuickSaveBitmap";
		this.tbFileQuickSaveBitmap.Click += new System.EventHandler(ActionFileQuickSaveBitmap);
		this.tbFileSep3.Name = "tbFileSep3";
		resources.ApplyResources(this.tbFileSep3, "tbFileSep3");
		this.tbOptionExecuteFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbOptionExecuteFile.Image = YYCHR.Properties.Resources.EtcShortcut;
		resources.ApplyResources(this.tbOptionExecuteFile, "tbOptionExecuteFile");
		this.tbOptionExecuteFile.Name = "tbOptionExecuteFile";
		this.tbOptionExecuteFile.Click += new System.EventHandler(ActionOptionRunFile);
		resources.ApplyResources(this.tbWorkspaceLoad, "tbWorkspaceLoad");
		this.tbWorkspaceLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbWorkspaceLoad.Image = YYCHR.Properties.Resources.FileOpen;
		this.tbWorkspaceLoad.Margin = new System.Windows.Forms.Padding(0);
		this.tbWorkspaceLoad.Name = "tbWorkspaceLoad";
		this.tbWorkspaceLoad.Click += new System.EventHandler(ActionWorkspaceLoad);
		resources.ApplyResources(this.tbWorkspaceSave, "tbWorkspaceSave");
		this.tbWorkspaceSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbWorkspaceSave.Image = YYCHR.Properties.Resources.FileSave;
		this.tbWorkspaceSave.Margin = new System.Windows.Forms.Padding(0);
		this.tbWorkspaceSave.Name = "tbWorkspaceSave";
		this.tbWorkspaceSave.Click += new System.EventHandler(ActionWorkspaceSave);
		resources.ApplyResources(this.tbWorkspaceRemovePattern, "tbWorkspaceRemovePattern");
		this.tbWorkspaceRemovePattern.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbWorkspaceRemovePattern.Image = YYCHR.Properties.Resources.EditDelete;
		this.tbWorkspaceRemovePattern.Margin = new System.Windows.Forms.Padding(0);
		this.tbWorkspaceRemovePattern.Name = "tbWorkspaceRemovePattern";
		this.tbWorkspaceRemovePattern.Click += new System.EventHandler(ActionWorkspaceRemove);
		resources.ApplyResources(this.tbWorkspaceAdd, "tbWorkspaceAdd");
		this.tbWorkspaceAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbWorkspaceAdd.Image = YYCHR.Properties.Resources.PenStomp;
		this.tbWorkspaceAdd.Margin = new System.Windows.Forms.Padding(0);
		this.tbWorkspaceAdd.Name = "tbWorkspaceAdd";
		this.tbWorkspaceAdd.Click += new System.EventHandler(ActionWorkspaceAdd);
		this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
		resources.ApplyResources(this.configurationToolStripMenuItem, "configurationToolStripMenuItem");
		this.optionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.configurationToolStripMenuItem });
		this.optionToolStripMenuItem.Name = "optionToolStripMenuItem";
		resources.ApplyResources(this.optionToolStripMenuItem, "optionToolStripMenuItem");
		this.openBitmapDialog.DefaultExt = "bmp";
		resources.ApplyResources(this.openBitmapDialog, "openBitmapDialog");
		this.saveBitmapDialog.DefaultExt = "bmp";
		resources.ApplyResources(this.saveBitmapDialog, "saveBitmapDialog");
		this.openFileDialog.DefaultExt = "chr";
		resources.ApplyResources(this.openFileDialog, "openFileDialog");
		resources.ApplyResources(this.menuStripMain, "menuStripMain");
		this.menuStripMain.GripMargin = new System.Windows.Forms.Padding(2, 0, 0, 0);
		this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[8] { this.miFile, this.miEdit, this.miAddress, this.miPen, this.miPalette, this.miOption, this.miHelp, this.miOptionLanguage });
		this.menuStripMain.Name = "menuStripMain";
		this.menuStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		this.miFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[11]
		{
			this.miFileNew, this.miFileOpenRom, this.miFileReload, this.miFileSaveRom, this.miFileSaveAsRom, this.miFileSep1, this.miFileOpenBmp, this.miFileSaveBmp, this.miFileQuickSaveBitmap, this.miFileSep2,
			this.miFileExit
		});
		this.miFile.Name = "miFile";
		resources.ApplyResources(this.miFile, "miFile");
		this.miFileNew.Image = YYCHR.Properties.Resources.FileNew;
		resources.ApplyResources(this.miFileNew, "miFileNew");
		this.miFileNew.Name = "miFileNew";
		this.miFileNew.Click += new System.EventHandler(ActionFileNew);
		this.miFileOpenRom.Image = YYCHR.Properties.Resources.FileOpen;
		resources.ApplyResources(this.miFileOpenRom, "miFileOpenRom");
		this.miFileOpenRom.Name = "miFileOpenRom";
		this.miFileOpenRom.Click += new System.EventHandler(ActionFileOpen);
		resources.ApplyResources(this.miFileReload, "miFileReload");
		this.miFileReload.Name = "miFileReload";
		this.miFileReload.Click += new System.EventHandler(ActionFileReload);
		this.miFileSaveRom.Image = YYCHR.Properties.Resources.FileSave;
		resources.ApplyResources(this.miFileSaveRom, "miFileSaveRom");
		this.miFileSaveRom.Name = "miFileSaveRom";
		this.miFileSaveRom.Click += new System.EventHandler(ActionFileSave);
		this.miFileSaveAsRom.Image = YYCHR.Properties.Resources.FileSave;
		resources.ApplyResources(this.miFileSaveAsRom, "miFileSaveAsRom");
		this.miFileSaveAsRom.Name = "miFileSaveAsRom";
		this.miFileSaveAsRom.Click += new System.EventHandler(ActionFileSaveAs);
		this.miFileSep1.Name = "miFileSep1";
		resources.ApplyResources(this.miFileSep1, "miFileSep1");
		this.miFileOpenBmp.Image = YYCHR.Properties.Resources.FileOpenBmp;
		resources.ApplyResources(this.miFileOpenBmp, "miFileOpenBmp");
		this.miFileOpenBmp.Name = "miFileOpenBmp";
		this.miFileOpenBmp.Click += new System.EventHandler(ActionFileOpenBitmap);
		this.miFileSaveBmp.Image = YYCHR.Properties.Resources.FileSaveBmp;
		resources.ApplyResources(this.miFileSaveBmp, "miFileSaveBmp");
		this.miFileSaveBmp.Name = "miFileSaveBmp";
		this.miFileSaveBmp.Click += new System.EventHandler(ActionFileSaveBitmap);
		this.miFileQuickSaveBitmap.Image = YYCHR.Properties.Resources.FileSaveBmp;
		resources.ApplyResources(this.miFileQuickSaveBitmap, "miFileQuickSaveBitmap");
		this.miFileQuickSaveBitmap.Name = "miFileQuickSaveBitmap";
		this.miFileQuickSaveBitmap.Click += new System.EventHandler(ActionFileQuickSaveBitmap);
		this.miFileSep2.Name = "miFileSep2";
		resources.ApplyResources(this.miFileSep2, "miFileSep2");
		this.miFileExit.Image = YYCHR.Properties.Resources.FileExit;
		resources.ApplyResources(this.miFileExit, "miFileExit");
		this.miFileExit.Name = "miFileExit";
		this.miFileExit.Click += new System.EventHandler(ActionFileExit);
		this.miEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[24]
		{
			this.miEditUndo, this.miEditRedo, this.miEditSep0, this.miEditCut, this.miEditCopy, this.miEditPaste, this.miEditPasteOptimizedImage, this.miEditClear, this.miEditSelectAll, this.miEditSep1,
			this.miEditClearClipboard, this.miEditSep2, this.miEditMirrorHorizontal, this.miEditMirrorVertical, this.miEditSep3, this.miEditRotateLeft, this.miEditRotateRight, this.miEditSep4, this.miEditShiftUp, this.miEditShiftDown,
			this.miEditShiftLeft, this.miEditShiftRight, this.miEditSep5, this.miEditReplaceColor
		});
		this.miEdit.Name = "miEdit";
		resources.ApplyResources(this.miEdit, "miEdit");
		this.miEditUndo.Image = YYCHR.Properties.Resources.EditUndo;
		resources.ApplyResources(this.miEditUndo, "miEditUndo");
		this.miEditUndo.Name = "miEditUndo";
		this.miEditUndo.Click += new System.EventHandler(ActionEditUndo);
		this.miEditRedo.Image = YYCHR.Properties.Resources.EditRedo;
		resources.ApplyResources(this.miEditRedo, "miEditRedo");
		this.miEditRedo.Name = "miEditRedo";
		this.miEditRedo.Click += new System.EventHandler(ActionEditRedo);
		this.miEditSep0.Name = "miEditSep0";
		resources.ApplyResources(this.miEditSep0, "miEditSep0");
		this.miEditCut.Image = YYCHR.Properties.Resources.EditCut;
		resources.ApplyResources(this.miEditCut, "miEditCut");
		this.miEditCut.Name = "miEditCut";
		this.miEditCut.Click += new System.EventHandler(ActionEditCut);
		this.miEditCopy.Image = YYCHR.Properties.Resources.EditCopy;
		resources.ApplyResources(this.miEditCopy, "miEditCopy");
		this.miEditCopy.Name = "miEditCopy";
		this.miEditCopy.Click += new System.EventHandler(ActionEditCopy);
		this.miEditPaste.Image = YYCHR.Properties.Resources.EditPaste;
		resources.ApplyResources(this.miEditPaste, "miEditPaste");
		this.miEditPaste.Name = "miEditPaste";
		this.miEditPaste.Click += new System.EventHandler(ActionEditPaste);
		this.miEditPasteOptimizedImage.Image = YYCHR.Properties.Resources.EditPaste;
		resources.ApplyResources(this.miEditPasteOptimizedImage, "miEditPasteOptimizedImage");
		this.miEditPasteOptimizedImage.Name = "miEditPasteOptimizedImage";
		this.miEditPasteOptimizedImage.Click += new System.EventHandler(ActionEditPasteWithOptimise);
		this.miEditClear.Image = YYCHR.Properties.Resources.EditDelete;
		resources.ApplyResources(this.miEditClear, "miEditClear");
		this.miEditClear.Name = "miEditClear";
		this.miEditClear.Click += new System.EventHandler(ActionEditClear);
		resources.ApplyResources(this.miEditSelectAll, "miEditSelectAll");
		this.miEditSelectAll.Name = "miEditSelectAll";
		this.miEditSelectAll.Click += new System.EventHandler(ActionEditSelectAll);
		this.miEditSep1.Name = "miEditSep1";
		resources.ApplyResources(this.miEditSep1, "miEditSep1");
		this.miEditClearClipboard.Name = "miEditClearClipboard";
		resources.ApplyResources(this.miEditClearClipboard, "miEditClearClipboard");
		this.miEditClearClipboard.Click += new System.EventHandler(ActionEditClearClipboard);
		this.miEditSep2.Name = "miEditSep2";
		resources.ApplyResources(this.miEditSep2, "miEditSep2");
		this.miEditMirrorHorizontal.Image = YYCHR.Properties.Resources.EditMirrorH;
		resources.ApplyResources(this.miEditMirrorHorizontal, "miEditMirrorHorizontal");
		this.miEditMirrorHorizontal.Name = "miEditMirrorHorizontal";
		this.miEditMirrorHorizontal.Click += new System.EventHandler(ActionEditMirror);
		this.miEditMirrorVertical.Image = YYCHR.Properties.Resources.EditMirrorV;
		resources.ApplyResources(this.miEditMirrorVertical, "miEditMirrorVertical");
		this.miEditMirrorVertical.Name = "miEditMirrorVertical";
		this.miEditMirrorVertical.Click += new System.EventHandler(ActionEditMirror);
		this.miEditSep3.Name = "miEditSep3";
		resources.ApplyResources(this.miEditSep3, "miEditSep3");
		this.miEditRotateLeft.Image = YYCHR.Properties.Resources.EditRotateL;
		resources.ApplyResources(this.miEditRotateLeft, "miEditRotateLeft");
		this.miEditRotateLeft.Name = "miEditRotateLeft";
		this.miEditRotateLeft.Click += new System.EventHandler(ActionEditRotate);
		this.miEditRotateRight.Image = YYCHR.Properties.Resources.EditRotateR;
		resources.ApplyResources(this.miEditRotateRight, "miEditRotateRight");
		this.miEditRotateRight.Name = "miEditRotateRight";
		this.miEditRotateRight.Click += new System.EventHandler(ActionEditRotate);
		this.miEditSep4.Name = "miEditSep4";
		resources.ApplyResources(this.miEditSep4, "miEditSep4");
		this.miEditShiftUp.Image = YYCHR.Properties.Resources.EditShiftU;
		resources.ApplyResources(this.miEditShiftUp, "miEditShiftUp");
		this.miEditShiftUp.Name = "miEditShiftUp";
		this.miEditShiftUp.Click += new System.EventHandler(ActionEditShift);
		this.miEditShiftDown.Image = YYCHR.Properties.Resources.EditShiftD;
		resources.ApplyResources(this.miEditShiftDown, "miEditShiftDown");
		this.miEditShiftDown.Name = "miEditShiftDown";
		this.miEditShiftDown.Click += new System.EventHandler(ActionEditShift);
		this.miEditShiftLeft.Image = YYCHR.Properties.Resources.EditShiftL;
		resources.ApplyResources(this.miEditShiftLeft, "miEditShiftLeft");
		this.miEditShiftLeft.Name = "miEditShiftLeft";
		this.miEditShiftLeft.Click += new System.EventHandler(ActionEditShift);
		this.miEditShiftRight.Image = YYCHR.Properties.Resources.EditShiftR;
		resources.ApplyResources(this.miEditShiftRight, "miEditShiftRight");
		this.miEditShiftRight.Name = "miEditShiftRight";
		this.miEditShiftRight.Click += new System.EventHandler(ActionEditShift);
		this.miEditSep5.Name = "miEditSep5";
		resources.ApplyResources(this.miEditSep5, "miEditSep5");
		this.miEditReplaceColor.Image = YYCHR.Properties.Resources.EditParetteReplace;
		resources.ApplyResources(this.miEditReplaceColor, "miEditReplaceColor");
		this.miEditReplaceColor.Name = "miEditReplaceColor";
		this.miEditReplaceColor.Click += new System.EventHandler(ActionEditReplaceColor);
		this.miAddress.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[15]
		{
			this.miAddress0, this.miAddress1, this.miAddress2, this.miAddress3, this.miAddress4, this.miAddress5, this.miAddress6, this.miAddress7, this.miAddress8, this.miAddress9,
			this.miAddressSep0, this.miAddressInputAddress, this.miAddressSep1, this.miAddressFindPrevious, this.miAddressFindNext
		});
		this.miAddress.Name = "miAddress";
		resources.ApplyResources(this.miAddress, "miAddress");
		this.miAddress0.Image = YYCHR.Properties.Resources.Addr0;
		resources.ApplyResources(this.miAddress0, "miAddress0");
		this.miAddress0.Name = "miAddress0";
		this.miAddress0.Tag = "0";
		this.miAddress0.Click += new System.EventHandler(ActionAddresChange);
		this.miAddress1.Image = YYCHR.Properties.Resources.Addr1;
		resources.ApplyResources(this.miAddress1, "miAddress1");
		this.miAddress1.Name = "miAddress1";
		this.miAddress1.Tag = "1";
		this.miAddress1.Click += new System.EventHandler(ActionAddresChange);
		this.miAddress2.Image = YYCHR.Properties.Resources.Addr2;
		resources.ApplyResources(this.miAddress2, "miAddress2");
		this.miAddress2.Name = "miAddress2";
		this.miAddress2.Tag = "2";
		this.miAddress2.Click += new System.EventHandler(ActionAddresChange);
		this.miAddress3.Image = YYCHR.Properties.Resources.Addr3;
		resources.ApplyResources(this.miAddress3, "miAddress3");
		this.miAddress3.Name = "miAddress3";
		this.miAddress3.Tag = "3";
		this.miAddress3.Click += new System.EventHandler(ActionAddresChange);
		this.miAddress4.Image = YYCHR.Properties.Resources.Addr4;
		resources.ApplyResources(this.miAddress4, "miAddress4");
		this.miAddress4.Name = "miAddress4";
		this.miAddress4.Tag = "4";
		this.miAddress4.Click += new System.EventHandler(ActionAddresChange);
		this.miAddress5.Image = YYCHR.Properties.Resources.Addr5;
		resources.ApplyResources(this.miAddress5, "miAddress5");
		this.miAddress5.Name = "miAddress5";
		this.miAddress5.Tag = "5";
		this.miAddress5.Click += new System.EventHandler(ActionAddresChange);
		this.miAddress6.Image = YYCHR.Properties.Resources.Addr6;
		resources.ApplyResources(this.miAddress6, "miAddress6");
		this.miAddress6.Name = "miAddress6";
		this.miAddress6.Tag = "6";
		this.miAddress6.Click += new System.EventHandler(ActionAddresChange);
		this.miAddress7.Image = YYCHR.Properties.Resources.Addr7;
		resources.ApplyResources(this.miAddress7, "miAddress7");
		this.miAddress7.Name = "miAddress7";
		this.miAddress7.Tag = "7";
		this.miAddress7.Click += new System.EventHandler(ActionAddresChange);
		this.miAddress8.Image = YYCHR.Properties.Resources.Addr8;
		resources.ApplyResources(this.miAddress8, "miAddress8");
		this.miAddress8.Name = "miAddress8";
		this.miAddress8.Tag = "8";
		this.miAddress8.Click += new System.EventHandler(ActionAddresChange);
		this.miAddress9.Image = YYCHR.Properties.Resources.Addr9;
		resources.ApplyResources(this.miAddress9, "miAddress9");
		this.miAddress9.Name = "miAddress9";
		this.miAddress9.Tag = "9";
		this.miAddress9.Click += new System.EventHandler(ActionAddresChange);
		this.miAddressSep0.Name = "miAddressSep0";
		resources.ApplyResources(this.miAddressSep0, "miAddressSep0");
		this.miAddressInputAddress.Image = YYCHR.Properties.Resources.AddrA;
		resources.ApplyResources(this.miAddressInputAddress, "miAddressInputAddress");
		this.miAddressInputAddress.Name = "miAddressInputAddress";
		this.miAddressInputAddress.Tag = "10";
		this.miAddressInputAddress.Click += new System.EventHandler(ActionAddressInputAddress);
		this.miAddressSep1.Name = "miAddressSep1";
		resources.ApplyResources(this.miAddressSep1, "miAddressSep1");
		this.miAddressFindPrevious.Image = YYCHR.Properties.Resources.AddrFindPrev;
		resources.ApplyResources(this.miAddressFindPrevious, "miAddressFindPrevious");
		this.miAddressFindPrevious.Name = "miAddressFindPrevious";
		this.miAddressFindPrevious.Click += new System.EventHandler(ActionAddressFindPrevious);
		this.miAddressFindNext.Image = YYCHR.Properties.Resources.AddrFindNext;
		resources.ApplyResources(this.miAddressFindNext, "miAddressFindNext");
		this.miAddressFindNext.Name = "miAddressFindNext";
		this.miAddressFindNext.Click += new System.EventHandler(ActionAddressFindNext);
		this.miPen.Name = "miPen";
		resources.ApplyResources(this.miPen, "miPen");
		this.miPalette.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[21]
		{
			this.miPalettePaletteType, this.miPaletteSelectPalette, this.miPaletteSep0, this.miPaletteLoadEmulatorState, this.miPaletteSep1, this.miPalettePalOpen, this.miPaletteLoadRGBPaletteFromCommon, this.miPalettePalSave, this.miPaletteQuickSaveRGBPalette, this.miPaletteSep2,
			this.miPaletteDatOpen, this.miPaletteLoadPaletteTableFromCommon, this.miPaletteDatSave, this.miPaletteQuickSavePaletteTable, this.miPaletteSep3, this.miPaletteOpenADF, this.miPaletteLoadADFPatternFromCommon, this.miPaletteSaveADF, this.miPaletteQuickSaveADFPattern, this.miPaletteSep4,
			this.miPaletteLoadDefaultSetting
		});
		this.miPalette.Name = "miPalette";
		resources.ApplyResources(this.miPalette, "miPalette");
		this.miPalettePaletteType.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.miPaletteTypeDat, this.miPaletteTypePal, this.miPaletteTypeBmp });
		this.miPalettePaletteType.Name = "miPalettePaletteType";
		resources.ApplyResources(this.miPalettePaletteType, "miPalettePaletteType");
		this.miPaletteTypeDat.Image = YYCHR.Properties.Resources.PaletteTypePalTable;
		resources.ApplyResources(this.miPaletteTypeDat, "miPaletteTypeDat");
		this.miPaletteTypeDat.Name = "miPaletteTypeDat";
		this.miPaletteTypeDat.Tag = "0";
		this.miPaletteTypeDat.Click += new System.EventHandler(ActionPaletteType);
		this.miPaletteTypePal.Image = YYCHR.Properties.Resources.PaletteTypePal;
		resources.ApplyResources(this.miPaletteTypePal, "miPaletteTypePal");
		this.miPaletteTypePal.Name = "miPaletteTypePal";
		this.miPaletteTypePal.Tag = "1";
		this.miPaletteTypePal.Click += new System.EventHandler(ActionPaletteType);
		this.miPaletteTypeBmp.Image = YYCHR.Properties.Resources.PaletteTypeBmp;
		resources.ApplyResources(this.miPaletteTypeBmp, "miPaletteTypeBmp");
		this.miPaletteTypeBmp.Name = "miPaletteTypeBmp";
		this.miPaletteTypeBmp.Tag = "2";
		this.miPaletteTypeBmp.Click += new System.EventHandler(ActionPaletteType);
		this.miPaletteSelectPalette.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[19]
		{
			this.miPalettePrev, this.miPaletteNext, this.miPaletteSelectSep0, this.miPaletteSelect0, this.miPaletteSelect1, this.miPaletteSelect2, this.miPaletteSelect3, this.miPaletteSelect4, this.miPaletteSelect5, this.miPaletteSelect6,
			this.miPaletteSelect7, this.miPaletteSelect8, this.miPaletteSelect9, this.miPaletteSelectA, this.miPaletteSelectB, this.miPaletteSelectC, this.miPaletteSelectD, this.miPaletteSelectE, this.miPaletteSelectF
		});
		this.miPaletteSelectPalette.Name = "miPaletteSelectPalette";
		resources.ApplyResources(this.miPaletteSelectPalette, "miPaletteSelectPalette");
		this.miPalettePrev.Name = "miPalettePrev";
		resources.ApplyResources(this.miPalettePrev, "miPalettePrev");
		this.miPalettePrev.Click += new System.EventHandler(ActionPaletteSelectPrevious);
		this.miPaletteNext.Name = "miPaletteNext";
		resources.ApplyResources(this.miPaletteNext, "miPaletteNext");
		this.miPaletteNext.Click += new System.EventHandler(ActionPaletteSelectNext);
		this.miPaletteSelectSep0.Name = "miPaletteSelectSep0";
		resources.ApplyResources(this.miPaletteSelectSep0, "miPaletteSelectSep0");
		this.miPaletteSelect0.Name = "miPaletteSelect0";
		resources.ApplyResources(this.miPaletteSelect0, "miPaletteSelect0");
		this.miPaletteSelect0.Tag = "0";
		this.miPaletteSelect0.Click += new System.EventHandler(ActionPaletteSelect);
		this.miPaletteSelect1.Name = "miPaletteSelect1";
		resources.ApplyResources(this.miPaletteSelect1, "miPaletteSelect1");
		this.miPaletteSelect1.Tag = "1";
		this.miPaletteSelect1.Click += new System.EventHandler(ActionPaletteSelect);
		this.miPaletteSelect2.Name = "miPaletteSelect2";
		resources.ApplyResources(this.miPaletteSelect2, "miPaletteSelect2");
		this.miPaletteSelect2.Tag = "2";
		this.miPaletteSelect2.Click += new System.EventHandler(ActionPaletteSelect);
		this.miPaletteSelect3.Name = "miPaletteSelect3";
		resources.ApplyResources(this.miPaletteSelect3, "miPaletteSelect3");
		this.miPaletteSelect3.Tag = "3";
		this.miPaletteSelect3.Click += new System.EventHandler(ActionPaletteSelect);
		this.miPaletteSelect4.Name = "miPaletteSelect4";
		resources.ApplyResources(this.miPaletteSelect4, "miPaletteSelect4");
		this.miPaletteSelect4.Tag = "4";
		this.miPaletteSelect4.Click += new System.EventHandler(ActionPaletteSelect);
		this.miPaletteSelect5.Name = "miPaletteSelect5";
		resources.ApplyResources(this.miPaletteSelect5, "miPaletteSelect5");
		this.miPaletteSelect5.Tag = "5";
		this.miPaletteSelect5.Click += new System.EventHandler(ActionPaletteSelect);
		this.miPaletteSelect6.Name = "miPaletteSelect6";
		resources.ApplyResources(this.miPaletteSelect6, "miPaletteSelect6");
		this.miPaletteSelect6.Tag = "6";
		this.miPaletteSelect6.Click += new System.EventHandler(ActionPaletteSelect);
		this.miPaletteSelect7.Name = "miPaletteSelect7";
		resources.ApplyResources(this.miPaletteSelect7, "miPaletteSelect7");
		this.miPaletteSelect7.Tag = "7";
		this.miPaletteSelect7.Click += new System.EventHandler(ActionPaletteSelect);
		this.miPaletteSelect8.Name = "miPaletteSelect8";
		resources.ApplyResources(this.miPaletteSelect8, "miPaletteSelect8");
		this.miPaletteSelect8.Tag = "8";
		this.miPaletteSelect8.Click += new System.EventHandler(ActionPaletteSelect);
		this.miPaletteSelect9.Name = "miPaletteSelect9";
		resources.ApplyResources(this.miPaletteSelect9, "miPaletteSelect9");
		this.miPaletteSelect9.Tag = "9";
		this.miPaletteSelect9.Click += new System.EventHandler(ActionPaletteSelect);
		this.miPaletteSelectA.Name = "miPaletteSelectA";
		resources.ApplyResources(this.miPaletteSelectA, "miPaletteSelectA");
		this.miPaletteSelectA.Tag = "10";
		this.miPaletteSelectA.Click += new System.EventHandler(ActionPaletteSelect);
		this.miPaletteSelectB.Name = "miPaletteSelectB";
		resources.ApplyResources(this.miPaletteSelectB, "miPaletteSelectB");
		this.miPaletteSelectB.Tag = "11";
		this.miPaletteSelectB.Click += new System.EventHandler(ActionPaletteSelect);
		this.miPaletteSelectC.Name = "miPaletteSelectC";
		resources.ApplyResources(this.miPaletteSelectC, "miPaletteSelectC");
		this.miPaletteSelectC.Tag = "12";
		this.miPaletteSelectC.Click += new System.EventHandler(ActionPaletteSelect);
		this.miPaletteSelectD.Name = "miPaletteSelectD";
		resources.ApplyResources(this.miPaletteSelectD, "miPaletteSelectD");
		this.miPaletteSelectD.Tag = "13";
		this.miPaletteSelectD.Click += new System.EventHandler(ActionPaletteSelect);
		this.miPaletteSelectE.Name = "miPaletteSelectE";
		resources.ApplyResources(this.miPaletteSelectE, "miPaletteSelectE");
		this.miPaletteSelectE.Tag = "14";
		this.miPaletteSelectE.Click += new System.EventHandler(ActionPaletteSelect);
		this.miPaletteSelectF.Name = "miPaletteSelectF";
		resources.ApplyResources(this.miPaletteSelectF, "miPaletteSelectF");
		this.miPaletteSelectF.Tag = "15";
		this.miPaletteSelectF.Click += new System.EventHandler(ActionPaletteSelect);
		this.miPaletteSep0.Name = "miPaletteSep0";
		resources.ApplyResources(this.miPaletteSep0, "miPaletteSep0");
		this.miPaletteLoadEmulatorState.Image = YYCHR.Properties.Resources.PaletteOpenState;
		resources.ApplyResources(this.miPaletteLoadEmulatorState, "miPaletteLoadEmulatorState");
		this.miPaletteLoadEmulatorState.Name = "miPaletteLoadEmulatorState";
		this.miPaletteLoadEmulatorState.Click += new System.EventHandler(ActionPaletteLoadEmulatorState);
		this.miPaletteSep1.Name = "miPaletteSep1";
		resources.ApplyResources(this.miPaletteSep1, "miPaletteSep1");
		this.miPalettePalOpen.Image = YYCHR.Properties.Resources.FileOpenPal;
		resources.ApplyResources(this.miPalettePalOpen, "miPalettePalOpen");
		this.miPalettePalOpen.Name = "miPalettePalOpen";
		this.miPalettePalOpen.Click += new System.EventHandler(ActionPaletteOpenPal);
		this.miPaletteLoadRGBPaletteFromCommon.Image = YYCHR.Properties.Resources.FileOpenPal;
		resources.ApplyResources(this.miPaletteLoadRGBPaletteFromCommon, "miPaletteLoadRGBPaletteFromCommon");
		this.miPaletteLoadRGBPaletteFromCommon.Name = "miPaletteLoadRGBPaletteFromCommon";
		this.miPalettePalSave.Image = YYCHR.Properties.Resources.FileSavePal;
		resources.ApplyResources(this.miPalettePalSave, "miPalettePalSave");
		this.miPalettePalSave.Name = "miPalettePalSave";
		this.miPalettePalSave.Click += new System.EventHandler(ActionPaletteSavePal);
		this.miPaletteQuickSaveRGBPalette.Image = YYCHR.Properties.Resources.FileSavePal;
		resources.ApplyResources(this.miPaletteQuickSaveRGBPalette, "miPaletteQuickSaveRGBPalette");
		this.miPaletteQuickSaveRGBPalette.Name = "miPaletteQuickSaveRGBPalette";
		this.miPaletteQuickSaveRGBPalette.Click += new System.EventHandler(ActionPaletteQuickSavePal);
		this.miPaletteSep2.Name = "miPaletteSep2";
		resources.ApplyResources(this.miPaletteSep2, "miPaletteSep2");
		this.miPaletteDatOpen.Image = YYCHR.Properties.Resources.FileOpenDat;
		resources.ApplyResources(this.miPaletteDatOpen, "miPaletteDatOpen");
		this.miPaletteDatOpen.Name = "miPaletteDatOpen";
		this.miPaletteDatOpen.Click += new System.EventHandler(ActionPaletteOpenDat);
		this.miPaletteLoadPaletteTableFromCommon.Image = YYCHR.Properties.Resources.FileOpenDat;
		resources.ApplyResources(this.miPaletteLoadPaletteTableFromCommon, "miPaletteLoadPaletteTableFromCommon");
		this.miPaletteLoadPaletteTableFromCommon.Name = "miPaletteLoadPaletteTableFromCommon";
		this.miPaletteDatSave.Image = YYCHR.Properties.Resources.FileSaveDat;
		resources.ApplyResources(this.miPaletteDatSave, "miPaletteDatSave");
		this.miPaletteDatSave.Name = "miPaletteDatSave";
		this.miPaletteDatSave.Click += new System.EventHandler(ActionPaletteSaveDat);
		this.miPaletteQuickSavePaletteTable.Image = YYCHR.Properties.Resources.FileSaveDat;
		resources.ApplyResources(this.miPaletteQuickSavePaletteTable, "miPaletteQuickSavePaletteTable");
		this.miPaletteQuickSavePaletteTable.Name = "miPaletteQuickSavePaletteTable";
		this.miPaletteQuickSavePaletteTable.Click += new System.EventHandler(ActionPaletteQuickSaveDat);
		this.miPaletteSep3.Name = "miPaletteSep3";
		resources.ApplyResources(this.miPaletteSep3, "miPaletteSep3");
		this.miPaletteOpenADF.Image = YYCHR.Properties.Resources.FileOpenAdf;
		resources.ApplyResources(this.miPaletteOpenADF, "miPaletteOpenADF");
		this.miPaletteOpenADF.Name = "miPaletteOpenADF";
		this.miPaletteOpenADF.Click += new System.EventHandler(ActionPaletteOpenAdf);
		this.miPaletteLoadADFPatternFromCommon.Image = YYCHR.Properties.Resources.FileOpenAdf;
		resources.ApplyResources(this.miPaletteLoadADFPatternFromCommon, "miPaletteLoadADFPatternFromCommon");
		this.miPaletteLoadADFPatternFromCommon.Name = "miPaletteLoadADFPatternFromCommon";
		this.miPaletteSaveADF.Image = YYCHR.Properties.Resources.FileSaveAdf;
		resources.ApplyResources(this.miPaletteSaveADF, "miPaletteSaveADF");
		this.miPaletteSaveADF.Name = "miPaletteSaveADF";
		this.miPaletteSaveADF.Click += new System.EventHandler(ActionPaletteSaveAdf);
		this.miPaletteQuickSaveADFPattern.Image = YYCHR.Properties.Resources.FileSaveAdf;
		resources.ApplyResources(this.miPaletteQuickSaveADFPattern, "miPaletteQuickSaveADFPattern");
		this.miPaletteQuickSaveADFPattern.Name = "miPaletteQuickSaveADFPattern";
		this.miPaletteQuickSaveADFPattern.Click += new System.EventHandler(ActionPaletteQuickSaveAdf);
		this.miPaletteSep4.Name = "miPaletteSep4";
		resources.ApplyResources(this.miPaletteSep4, "miPaletteSep4");
		this.miPaletteLoadDefaultSetting.Name = "miPaletteLoadDefaultSetting";
		resources.ApplyResources(this.miPaletteLoadDefaultSetting, "miPaletteLoadDefaultSetting");
		this.miPaletteLoadDefaultSetting.Click += new System.EventHandler(actionPaletteLoadDefault);
		this.miOption.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.miOptionSetting, this.miOptionShowAllMenu, this.miOptionSep0, this.miOptionExecuteFile });
		this.miOption.Name = "miOption";
		resources.ApplyResources(this.miOption, "miOption");
		this.miOptionSetting.Image = YYCHR.Properties.Resources.IconOptionSetting;
		this.miOptionSetting.Name = "miOptionSetting";
		resources.ApplyResources(this.miOptionSetting, "miOptionSetting");
		this.miOptionSetting.Click += new System.EventHandler(ActionOptionSetting);
		this.miOptionShowAllMenu.Image = YYCHR.Properties.Resources.IconOptionSetting;
		this.miOptionShowAllMenu.Name = "miOptionShowAllMenu";
		resources.ApplyResources(this.miOptionShowAllMenu, "miOptionShowAllMenu");
		this.miOptionShowAllMenu.Click += new System.EventHandler(miOptionShowAllMenu_Click);
		this.miOptionSep0.Name = "miOptionSep0";
		resources.ApplyResources(this.miOptionSep0, "miOptionSep0");
		this.miOptionExecuteFile.Image = YYCHR.Properties.Resources.EtcShortcut;
		resources.ApplyResources(this.miOptionExecuteFile, "miOptionExecuteFile");
		this.miOptionExecuteFile.Name = "miOptionExecuteFile";
		this.miOptionExecuteFile.Click += new System.EventHandler(ActionOptionRunFile);
		this.miHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[8] { this.miHelpOpenWiki, this.miHelpSep0, this.miHelpOpenWebsite, this.miHelpReportBugs, this.miHelpSep1, this.miHelpPropertyEditor, this.miHelpSep2, this.miHelpAbout });
		this.miHelp.Name = "miHelp";
		resources.ApplyResources(this.miHelp, "miHelp");
		resources.ApplyResources(this.miHelpOpenWiki, "miHelpOpenWiki");
		this.miHelpOpenWiki.Image = YYCHR.Properties.Resources.EtcShortcut;
		this.miHelpOpenWiki.Name = "miHelpOpenWiki";
		this.miHelpOpenWiki.Click += new System.EventHandler(ActionHelpOpenWiki);
		this.miHelpSep0.Name = "miHelpSep0";
		resources.ApplyResources(this.miHelpSep0, "miHelpSep0");
		this.miHelpOpenWebsite.Image = YYCHR.Properties.Resources.EtcShortcut;
		resources.ApplyResources(this.miHelpOpenWebsite, "miHelpOpenWebsite");
		this.miHelpOpenWebsite.Name = "miHelpOpenWebsite";
		this.miHelpOpenWebsite.Click += new System.EventHandler(ActionHelpOpenWeb);
		this.miHelpReportBugs.Image = YYCHR.Properties.Resources.EtcShortcut;
		resources.ApplyResources(this.miHelpReportBugs, "miHelpReportBugs");
		this.miHelpReportBugs.Name = "miHelpReportBugs";
		this.miHelpReportBugs.Click += new System.EventHandler(ActionHelpOpenBbs);
		this.miHelpSep1.Name = "miHelpSep1";
		resources.ApplyResources(this.miHelpSep1, "miHelpSep1");
		this.miHelpPropertyEditor.Image = YYCHR.Properties.Resources.IconControl;
		resources.ApplyResources(this.miHelpPropertyEditor, "miHelpPropertyEditor");
		this.miHelpPropertyEditor.Name = "miHelpPropertyEditor";
		this.miHelpPropertyEditor.Click += new System.EventHandler(ActionHelpPropertyEditor);
		this.miHelpSep2.Name = "miHelpSep2";
		resources.ApplyResources(this.miHelpSep2, "miHelpSep2");
		resources.ApplyResources(this.miHelpAbout, "miHelpAbout");
		this.miHelpAbout.Name = "miHelpAbout";
		this.miHelpAbout.Click += new System.EventHandler(ActionHelpAbout);
		this.miOptionLanguage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[9] { this.miLanguageSystem, this.miLanguageSep0, this.miLanguageEnglish, this.miLanguageJapanese, this.miLanguageSep1, this.miLanguageLngFile, this.miLanguageSep2, this.miLanguageSettingAutoLoadLng, this.miLanguageExportLng });
		this.miOptionLanguage.Name = "miOptionLanguage";
		resources.ApplyResources(this.miOptionLanguage, "miOptionLanguage");
		resources.ApplyResources(this.miLanguageSystem, "miLanguageSystem");
		this.miLanguageSystem.Name = "miLanguageSystem";
		this.miLanguageSystem.Click += new System.EventHandler(ActionOptionLanguageSet);
		this.miLanguageSep0.Name = "miLanguageSep0";
		resources.ApplyResources(this.miLanguageSep0, "miLanguageSep0");
		resources.ApplyResources(this.miLanguageEnglish, "miLanguageEnglish");
		this.miLanguageEnglish.Name = "miLanguageEnglish";
		this.miLanguageEnglish.Click += new System.EventHandler(ActionOptionLanguageSet);
		resources.ApplyResources(this.miLanguageJapanese, "miLanguageJapanese");
		this.miLanguageJapanese.Name = "miLanguageJapanese";
		this.miLanguageJapanese.Click += new System.EventHandler(ActionOptionLanguageSet);
		this.miLanguageSep1.Name = "miLanguageSep1";
		resources.ApplyResources(this.miLanguageSep1, "miLanguageSep1");
		this.miLanguageLngFile.Image = YYCHR.Properties.Resources.IconDocument;
		resources.ApplyResources(this.miLanguageLngFile, "miLanguageLngFile");
		this.miLanguageLngFile.Name = "miLanguageLngFile";
		this.miLanguageLngFile.Click += new System.EventHandler(ActionOptionLanguageSet);
		this.miLanguageSep2.Name = "miLanguageSep2";
		resources.ApplyResources(this.miLanguageSep2, "miLanguageSep2");
		this.miLanguageSettingAutoLoadLng.Image = YYCHR.Properties.Resources.IconOptionSetting;
		resources.ApplyResources(this.miLanguageSettingAutoLoadLng, "miLanguageSettingAutoLoadLng");
		this.miLanguageSettingAutoLoadLng.Name = "miLanguageSettingAutoLoadLng";
		this.miLanguageSettingAutoLoadLng.Click += new System.EventHandler(miLanguageLoadLngStartup_Click);
		this.miLanguageExportLng.Image = YYCHR.Properties.Resources.FileSave;
		resources.ApplyResources(this.miLanguageExportLng, "miLanguageExportLng");
		this.miLanguageExportLng.Name = "miLanguageExportLng";
		this.miLanguageExportLng.Click += new System.EventHandler(miLanguageOutputLng_Click);
		this.toolTip.ShowAlways = true;
		this.popupJumpListMenu.Name = "popupJumpListMenu";
		this.popupJumpListMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		resources.ApplyResources(this.popupJumpListMenu, "popupJumpListMenu");
		resources.ApplyResources(this.tsWoekSpace, "tsWoekSpace");
		this.tsWoekSpace.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.tbWorkspaceLoad, this.tbWorkspaceSave, this.tbWorkspaceRemovePattern, this.tbWorkspaceAdd });
		this.tsWoekSpace.Name = "tsWoekSpace";
		this.tsWoekSpace.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		this.panelWorkSpaceTS.Controls.Add(this.tsWoekSpace);
		resources.ApplyResources(this.panelWorkSpaceTS, "panelWorkSpaceTS");
		this.panelWorkSpaceTS.Name = "panelWorkSpaceTS";
		this.contextMenuStripChr.Items.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.cmiEditCut, this.cmiEditCopy, this.cmiEditPaste, this.cmiEditClear, this.cmiEditSelectAll });
		this.contextMenuStripChr.Name = "contextMenuStripChr";
		this.contextMenuStripChr.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		resources.ApplyResources(this.contextMenuStripChr, "contextMenuStripChr");
		this.cmiEditCut.Image = YYCHR.Properties.Resources.EditCut;
		resources.ApplyResources(this.cmiEditCut, "cmiEditCut");
		this.cmiEditCut.Name = "cmiEditCut";
		this.cmiEditCut.Click += new System.EventHandler(ActionEditCut);
		this.cmiEditCopy.Image = YYCHR.Properties.Resources.EditCopy;
		resources.ApplyResources(this.cmiEditCopy, "cmiEditCopy");
		this.cmiEditCopy.Name = "cmiEditCopy";
		this.cmiEditCopy.Click += new System.EventHandler(ActionEditCopy);
		this.cmiEditPaste.Image = YYCHR.Properties.Resources.EditPaste;
		resources.ApplyResources(this.cmiEditPaste, "cmiEditPaste");
		this.cmiEditPaste.Name = "cmiEditPaste";
		this.cmiEditPaste.Click += new System.EventHandler(ActionEditPaste);
		this.cmiEditClear.Image = YYCHR.Properties.Resources.EditDelete;
		resources.ApplyResources(this.cmiEditClear, "cmiEditClear");
		this.cmiEditClear.Name = "cmiEditClear";
		this.cmiEditClear.Click += new System.EventHandler(ActionEditClear);
		resources.ApplyResources(this.cmiEditSelectAll, "cmiEditSelectAll");
		this.cmiEditSelectAll.Name = "cmiEditSelectAll";
		this.cmiEditSelectAll.Click += new System.EventHandler(ActionEditSelectAll);
		resources.ApplyResources(this.lVersion, "lVersion");
		this.lVersion.ForeColor = System.Drawing.Color.Red;
		this.lVersion.Name = "lVersion";
		this.mFindWorker.WorkerReportsProgress = true;
		this.mFindWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(mFindWorker_DoWork);
		this.mFindWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(mFindWorker_ProgressChanged);
		this.mFindWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(mFindWorker_RunWorkerCompleted);
		this.AllowDrop = true;
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
		resources.ApplyResources(this, "$this");
		base.Controls.Add(this.lVersion);
		base.Controls.Add(this.panelWorkSpaceTS);
		base.Controls.Add(this.tabControl);
		base.Controls.Add(this.rightPanel);
		base.Controls.Add(this.toolStripMain);
		base.Controls.Add(this.menuStripMain);
		base.Controls.Add(this.toolStripView);
		base.Controls.Add(this.statusStrip);
		this.DoubleBuffered = true;
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.KeyPreview = true;
		base.MainMenuStrip = this.menuStripMain;
		base.MaximizeBox = false;
		base.Name = "MainForm";
		base.Activated += new System.EventHandler(MainForm_Activated);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(MainForm_FormClosing);
		base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(MainForm_FormClosed);
		base.Load += new System.EventHandler(MainForm_Load);
		base.DragDrop += new System.Windows.Forms.DragEventHandler(MainForm_DragDrop);
		base.DragEnter += new System.Windows.Forms.DragEventHandler(MainForm_DragEnter);
		base.KeyDown += new System.Windows.Forms.KeyEventHandler(MainForm_KeyDown);
		base.KeyUp += new System.Windows.Forms.KeyEventHandler(MainForm_KeyUp);
		this.toolStripView.ResumeLayout(false);
		this.toolStripView.PerformLayout();
		this.statusStrip.ResumeLayout(false);
		this.statusStrip.PerformLayout();
		this.tabControl.ResumeLayout(false);
		this.tabPageChrRom.ResumeLayout(false);
		this.panelChr.ResumeLayout(false);
		this.panelChrSetting.ResumeLayout(false);
		this.panelChrSetting.PerformLayout();
		this.panelToolStripAddress.ResumeLayout(false);
		this.toolStripAddress.ResumeLayout(false);
		this.toolStripAddress.PerformLayout();
		this.scrollPanelRom.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cellSelectorRom).EndInit();
		this.tabPageBitmap.ResumeLayout(false);
		this.scrollPanelBmp.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cellSelectorBmp).EndInit();
		this.tabWorkSpace.ResumeLayout(false);
		this.panelWorkSpace.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.workSpaceSelector1).EndInit();
		this.rightPanel.ResumeLayout(false);
		this.panelPalette.ResumeLayout(false);
		this.toolStripPalette.ResumeLayout(false);
		this.toolStripPalette.PerformLayout();
		this.panelEdit.ResumeLayout(false);
		this.toolStripMain.ResumeLayout(false);
		this.toolStripMain.PerformLayout();
		this.menuStripMain.ResumeLayout(false);
		this.menuStripMain.PerformLayout();
		this.tsWoekSpace.ResumeLayout(false);
		this.tsWoekSpace.PerformLayout();
		this.panelWorkSpaceTS.ResumeLayout(false);
		this.panelWorkSpaceTS.PerformLayout();
		this.contextMenuStripChr.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
