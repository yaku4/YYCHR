using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using CharactorLib.Common;
using CharactorLib.Data;
using ControlLib;
using PrgEditor.Forms;
using PrgEditor.Properties;

namespace PrgEditor;

public class MainForm : Form
{
	private enum EditMode
	{
		StepByStep,
		TypeText,
		DrawPicture
	}

	private enum PatternMode
	{
		P8x8,
		P8x16,
		P16x16
	}

	private const string APP_NAME = "PRG-ROM Pattern Editor";

	private const string APP_VER = "Version 0.50";

	private const string URL_WEB = "https://www45.atwiki.jp/yychr/";

	private const string URL_WIKI = "https://www45.atwiki.jp/yychr/";

	private const string URL_BOARD = "https://jbbs.shitaraba.net/bbs/read.cgi/computer/41853/1231162374/l50";

	private const string URL_WIKI_HINT = "http://www45.atwiki.jp/yychr/?page=";

	private Setting mConfiguration = new Setting();

	private string mIniFileName = "PrgEditor.ini";

	private PalInfo mPalInfo = new PalInfo();

	private DatInfo mDatInfo = new DatInfo();

	private DataFileBase mPrgFile = new DataFileBase();

	private DataFileBase mChrFile = new DataFileBase();

	private PatternManager mPatternManager = new PatternManager();

	private byte[] mPrgData;

	private Bytemap mPrgBytemap;

	private Bitmap mPrgBitmap;

	private Bytemap mChrBytemap;

	private Bitmap mChrBitmap;

	private int mAddr;

	private int mLength;

	private const int mCellZoom = 2;

	private const int mCellSize = 8;

	private const int mCellZoomedSize = 16;

	private int mSelectedX;

	private int mSelectedY;

	private int mSelectedAddr;

	private byte mSelectedByte;

	private string mTextAddrSize = "";

	private int mColCount = 32;

	private int mRowCount = 32;

	private Bytemap mLoadBytemap;

	private EditMode mEditMode;

	private byte[] mFindData = new byte[4] { 64, 65, 66, 67 };

	private SearchForm.FindType mFindType;

	private PropertyEditorForm propertyEditorForm;

	private PatternMode mPatternMode;

	private bool mSkip1;

	private IContainer components;

	private CellSelector selectorPrg;

	private ScrollPanel scrollPanelPrg;

	private PaletteSelector paletteSelectorSet;

	private PaletteSelector paletteSelectorNes;

	private ToolStripContainer tsContainer;

	private ToolStrip toolStrip1;

	private MenuStrip menuStrip1;

	private CellSelector selectorChr;

	private OpenFileDialog openFileDialogPrg;

	private SaveFileDialog saveFileDialogPrg;

	private OpenFileDialog openFileDialogChr;

	private OpenFileDialog openFileDialogEtc;

	private ToolStrip toolStripChr;

	private ToolStripMenuItem miFile;

	private ToolStripMenuItem miFileOpen;

	private ToolStripMenuItem miFileReload;

	private ToolStripMenuItem miFileSave;

	private ToolStripMenuItem miFileSaveAs;

	private ToolStripMenuItem miFileExit;

	private ToolStripMenuItem miEdit;

	private ToolStripMenuItem miView;

	private ToolStripMenuItem miOption;

	private ToolStripMenuItem miHelp;

	private ToolStripButton tsbFileOpen1;

	private ToolStripButton tsbFileSave;

	private ToolStripSeparator toolStripSeparator1;

	private ToolStripButton tsbViewOpenChr1;

	private Panel panelRight;

	private ToolStripMenuItem miViewGridPrg;

	private ToolStripMenuItem miViewGridChr;

	private ToolStripSeparator toolStripSeparator2;

	private ToolStripButton tsbViewGridPrg1;

	private ToolStripButton tsbViewGridChr1;

	private ToolStripSeparator toolStripSeparator3;

	private ToolStripMenuItem miEditMode;

	private ToolStripMenuItem miEditModeStepByStep;

	private ToolStripMenuItem miEditModeDrawPicture;

	private ToolStripMenuItem miEditModeTypeText;

	private ToolStripButton tsbEditModeTypeText;

	private ToolStripButton tsbEditModeDrawPict;

	private ToolStripButton tsbEditModeStep;

	private ToolStripSeparator toolStripSeparator4;

	private ToolStripMenuItem miOptionConfiguration;

	private ToolStripMenuItem miHelpAbout;

	private ToolStripSeparator toolStripMenuItem3;

	private ToolStripSeparator toolStripSeparator5;

	private ToolStripButton tsbViewOpenState1;

	private StatusStrip statusStrip1;

	private ToolStripStatusLabel toolStripStatusLabelAddrValue;

	private ToolStripStatusLabel toolStripStatusLabelWH;

	private ToolStripStatusLabel toolStripStatusLabelXY;

	private ToolStripStatusLabel toolStripStatusLabelCarretAddrValue;

	private ToolStripSeparator toolStripMenuItem2;

	private ToolStripMenuItem miViewOpenChr;

	private ToolStripSeparator toolStripMenuItem1;

	private ToolStripMenuItem miViewOpenState;

	private ToolStripButton tsbMode8x16;

	private Panel panel2;

	private ToolStrip toolStripPrgAddr;

	private ToolStripLabel tsPrgSpace1;

	private ToolStripButton tsbAddr0;

	private ToolStripButton tsbAddr1;

	private ToolStripButton tsbAddr2;

	private ToolStripButton tsbAddr3;

	private ToolStripButton tsbAddr4;

	private ToolStripButton tsbAddr5;

	private ToolStripButton tsbAddr6;

	private ToolStripButton tsbAddr7;

	private ToolStripButton tsbAddr8;

	private ToolStripButton tsbAddr9;

	private ToolStripSeparator toolStripSeparator6;

	private ToolStripButton tsbAddrInput;

	private ToolStripLabel tsChrSpace1;

	private ToolStripButton tsbViewOpenChr2;

	private ToolStripLabel tsChrSpace2;

	private ToolStripButton tsbViewOpenState2;

	private ToolStripButton tsbViewGridPrg2;

	private ToolStripSeparator toolStripSeparator7;

	private ToolStripButton tsbViewGridChr2;

	private ToolStripSeparator toolStripSeparator8;

	private ToolStripButton tsbFileOpen2;

	private ToolStripSeparator toolStripSeparator9;

	private ToolStripMenuItem miHelpCheckUpdate;

	private ToolStripSeparator toolStripMenuItem4;

	private ToolStripLabel toolStripLabel1;

	private ToolStripMenuItem miFileSavePrgBitmap;

	private ToolStripSeparator toolStripMenuItem5;

	private SaveFileDialog saveFileDialogBmp;

	private ToolStripButton tsbYxSwap;

	private ToolStripButton tsbViewFullPaletteMode;

	private ToolStripSeparator toolStripMenuItem6;

	private ToolStripMenuItem miEditSearch;

	private ToolStripMenuItem miEditFindPrevious;

	private ToolStripMenuItem miEditFindNext;

	private ToolStripMenuItem miHelpPropertyEditor;

	private ToolStripSeparator toolStripMenuItem7;

	private ToolStripMenuItem miViewFullPaletteMode;

	private ToolStripButton tsbAddrFind;

	private ToolStripSeparator toolStripSeparator10;

	private ToolStripButton tsbResizeHInc;

	private ToolStripButton tsbResizeHDec;

	private ToolStripButton tsbResizeWInc;

	private ToolStripButton tsbResizeWDec;

	private ToolStripButton tsbAddrFindPrev;

	private ToolStripButton tsbAddrFindNext;

	private ToolStripSeparator toolStripSeparator11;

	private ToolStripButton tsbAddrFindNext2;

	private ToolStripButton tsbAddrFindPrev2;

	private ToolStripButton tsbAddrSearch2;

	private ToolStripButton tbSkip1;

	private ToolStripButton tsbViewImagePaletteMode;

	private ToolStripMenuItem miViewImagePaletteMode;

	private ToolStripStatusLabel toolStripStatusLabelView;

	private ToolStripStatusLabel toolStripStatusLabelSelected;

	private Label lVersion;

	private ToolStripButton tsbMode16x16;

	public int Address
	{
		get
		{
			return scrollPanelPrg.Value;
		}
		set
		{
			if (value < scrollPanelPrg.Minimum)
			{
				value = scrollPanelPrg.Minimum;
			}
			int num = scrollPanelPrg.Maximum + 1 - scrollPanelPrg.LargeChange;
			if (value > num)
			{
				value = num;
			}
			if (scrollPanelPrg.Value != value)
			{
				scrollPanelPrg.Value = value;
			}
			DrawPrg();
		}
	}

	private int GetSelectorIndex(CellSelector selector)
	{
		int num = selector.Width / 16;
		_ = selector.Height / 16;
		int num2 = selector.SelectedRect.X / 8;
		return selector.SelectedRect.Y / 8 * num + num2;
	}

	private void SetSelectorIndex(CellSelector selector, int index)
	{
		int num = selector.Width / 16;
		_ = selector.Height / 16;
		int num2 = index % num;
		int num3 = index / num;
		selector.SelectedRect = new Rectangle(num2 * 8, num3 * 8, 8, 8);
	}

	private int GetSelectedPrgAddress()
	{
		int num = selectorPrg.Width / 16;
		_ = selectorPrg.Height / 16;
		int num2 = selectorPrg.SelectedRect.X / 8;
		int num3 = selectorPrg.SelectedRect.Y / 8 * num + num2;
		RectCopyInfo rectCopyInfo = null;
		if (num3 >= 0 && num3 < mPatternManager.Infos.Length)
		{
			rectCopyInfo = mPatternManager.Infos[num3];
		}
		int result = -1;
		if (rectCopyInfo != null)
		{
			result = rectCopyInfo.Address;
		}
		return result;
	}

	private byte GetSelectedChr()
	{
		return (byte)GetSelectorIndex(selectorChr);
	}

	private void SetSelectedChr(byte index)
	{
		SetSelectorIndex(selectorChr, index);
	}

	private byte GetSelectedPrgData(int addr)
	{
		byte result = 0;
		if (mPrgData != null && addr >= 0 && addr < mPrgData.Length)
		{
			result = mPrgData[addr];
		}
		return result;
	}

	private byte GetSelectedPrgData()
	{
		int selectedPrgAddress = GetSelectedPrgAddress();
		return GetSelectedPrgData(selectedPrgAddress);
	}

	private void SetSelectedPrgData(byte value)
	{
		if (mPrgData != null)
		{
			int selectedPrgAddress = GetSelectedPrgAddress();
			if (selectedPrgAddress >= 0 && selectedPrgAddress < mPrgData.Length)
			{
				mPrgData[selectedPrgAddress] = value;
			}
		}
	}

	public MainForm()
	{
		InitializeComponent();
		base.Icon = Resources.prgeditor;
		mIniFileName = Utility.GetExeDataFilename("ini");
		mConfiguration.LoadFromFile(mIniFileName);
		InitializeFromSetting();
		string path = "Resources/palette.pal";
		byte[] mem = new byte[768];
		if (File.Exists(path))
		{
			mem = File.ReadAllBytes(path);
		}
		mPalInfo.LoadFromMem(mem, 0, 0, 192);
		mDatInfo.PalInfo = mPalInfo;
		string path2 = "Resources/palette.dat";
		byte[] mem2 = new byte[256];
		if (File.Exists(path2))
		{
			mem2 = File.ReadAllBytes(path2);
		}
		mDatInfo.LoadFromMem(mem2, 0, 0, 256);
		CopyPaletteArray(paletteSelectorNes.Palette, mPalInfo.Colors);
		CopyPaletteArray(paletteSelectorSet.Palette, mDatInfo.Colors);
		AdjustSelectorPrg();
		InitializePrg();
		InitializeChr();
		UpdateTitle();
		UpdateEditMode();
		UpdateBuildDateLabel();
		LoadChrBitmap(Resources.chr_def);
		DrawChr();
		scrollPanelPrg.Focus();
		selectorPrg.Focus();
		base.ActiveControl = selectorPrg;
	}

	private void InitializeFromSetting()
	{
		selectorPrg.GridVisible = mConfiguration.GridPrgEnable;
		tsbViewGridPrg1.Checked = mConfiguration.GridPrgEnable;
		tsbViewGridPrg2.Checked = mConfiguration.GridPrgEnable;
		miViewGridPrg.Checked = mConfiguration.GridPrgEnable;
		selectorChr.GridVisible = mConfiguration.GridChrEnable;
		tsbViewGridChr1.Checked = mConfiguration.GridChrEnable;
		tsbViewGridChr2.Checked = mConfiguration.GridChrEnable;
		miViewGridChr.Checked = mConfiguration.GridChrEnable;
		selectorPrg.GridColor1 = mConfiguration.GridColor1;
		selectorPrg.GridColor2 = mConfiguration.GridColor2;
		selectorChr.GridColor1 = mConfiguration.GridColor1;
		selectorChr.GridColor2 = mConfiguration.GridColor2;
		selectorPrg.SelectedColor1 = mConfiguration.SelectorColor1;
		selectorPrg.SelectedColor2 = mConfiguration.SelectorColor2;
		selectorChr.SelectedColor1 = mConfiguration.SelectorColor1;
		selectorChr.SelectedColor2 = mConfiguration.SelectorColor2;
	}

	private void UpdateTextAddrSize()
	{
		int num = 0;
		int num2 = 0;
		if (mPrgData != null)
		{
			num = Address;
			num2 = mPrgData.Length;
		}
		mAddr = num;
		mLength = num2;
		UpdateStatusbar();
	}

	private void UpdateTitle()
	{
		Text = "PRG-ROM Pattern Editor (" + mTextAddrSize + ") " + mPrgFile.FileNameName + " ";
	}

	private int GetPrgColumnCount()
	{
		return selectorPrg.ClientSize.Width / 16;
	}

	private int GetPrgRowCount()
	{
		return selectorPrg.ClientSize.Height / 16;
	}

	private void AdjustSelectorPrg()
	{
		int num = scrollPanelPrg.ClientAreaSize.Width / 16;
		int num2 = scrollPanelPrg.ClientAreaSize.Height / 16;
		if (num < 1)
		{
			num = 1;
		}
		if (num2 < 1)
		{
			num2 = 1;
		}
		Size size = new Size(num * 16, num2 * 16);
		if (selectorPrg.Size != size)
		{
			selectorPrg.Size = size;
		}
	}

	private void scrollPanelPrg_SizeChanged(object sender, EventArgs e)
	{
		AdjustSelectorPrg();
	}

	private void selectorPrg_SizeChanged(object sender, EventArgs e)
	{
		ResetPrgBitmapSize(forceReset: false);
	}

	private void scrollPanelPrg_Scrolled(object sender, EventArgs e)
	{
		DrawPrg();
		UpdateTextAddrSize();
		UpdateTitle();
	}

	private void selectorChr_Selected(object sender, MouseEventArgs e)
	{
		try
		{
			if (mEditMode == EditMode.StepByStep)
			{
				byte selectedChr = GetSelectedChr();
				SetSelectedPrgData(selectedChr);
				DrawPrg();
			}
			else if (mEditMode != EditMode.TypeText)
			{
				_ = mEditMode;
				_ = 2;
			}
		}
		catch
		{
		}
	}

	private void selectorChr_MouseDown(object sender, MouseEventArgs e)
	{
		if (mEditMode == EditMode.StepByStep)
		{
			base.OnMouseDown(e);
			return;
		}
		if (mEditMode == EditMode.TypeText)
		{
			try
			{
				if (e.Button == MouseButtons.Left)
				{
					byte selectedChr = GetSelectedChr();
					SetSelectedPrgData(selectedChr);
					int num = GetSelectorIndex(selectorPrg) + 1;
					int num2 = selectorPrg.Size.Width * selectorPrg.Size.Height;
					if (num >= num2)
					{
						num = 0;
						Address += scrollPanelPrg.LargeChange;
					}
					SetSelectorIndex(selectorPrg, num);
					byte selectedPrgData = GetSelectedPrgData();
					int num3 = (int)selectedPrgData % 16 * 8;
					int num4 = (int)selectedPrgData / 16 * 8;
					selectorChr.SelectedRect = new Rectangle(num3, num4, 8, 8);
					DrawBoth();
				}
				else if (e.Button == MouseButtons.Right)
				{
					int num5 = GetSelectorIndex(selectorPrg) - 1;
					int num6 = selectorPrg.Size.Width * selectorPrg.Size.Height;
					if (num5 < 0)
					{
						num5 = num6 - 1;
						Address -= scrollPanelPrg.LargeChange;
					}
					SetSelectorIndex(selectorPrg, num5);
					DrawBoth();
				}
				return;
			}
			catch
			{
				return;
			}
		}
		if (mEditMode == EditMode.DrawPicture)
		{
			base.OnMouseDown(e);
		}
	}

	private void selectorPrg_Selected(object sender, MouseEventArgs e)
	{
		try
		{
			mSelectedX = selectorPrg.SelectedRect.Left / 8;
			mSelectedY = selectorPrg.SelectedRect.Top / 8;
			if (mEditMode == EditMode.StepByStep || mEditMode == EditMode.TypeText)
			{
				mSelectedAddr = GetSelectedPrgAddress();
				byte selectedPrgData = GetSelectedPrgData(mSelectedAddr);
				int num = (int)selectedPrgData % 16 * 8;
				int num2 = (int)selectedPrgData / 16 * 8;
				selectorChr.SelectedRect = new Rectangle(num, num2, 8, 8);
				selectorChr.Refresh();
				mSelectedByte = selectedPrgData;
				UpdateStatusbar();
			}
			else if (mEditMode == EditMode.DrawPicture)
			{
				if (e.Button == MouseButtons.Left)
				{
					byte selectedChr = GetSelectedChr();
					SetSelectedPrgData(selectedChr);
					DrawPrg();
				}
				else
				{
					byte selectedPrgData2 = GetSelectedPrgData();
					SetSelectedChr(selectedPrgData2);
					DrawChr();
				}
			}
		}
		catch
		{
		}
	}

	private void UpdateStatusbar()
	{
		mTextAddrSize = mAddr.ToString("X6") + " / " + mLength.ToString("X6");
		toolStripStatusLabelAddrValue.Text = "ADDR : " + mTextAddrSize;
		string text = "W:" + mColCount.ToString("X2") + "  H:" + mRowCount.ToString("X2");
		toolStripStatusLabelWH.Text = text;
		string text2 = "X:" + mSelectedX.ToString("X2") + "  Y:" + mSelectedY.ToString("X2");
		toolStripStatusLabelXY.Text = text2;
		string text3 = mSelectedAddr.ToString("X6") + " = " + mSelectedByte.ToString("X2");
		toolStripStatusLabelCarretAddrValue.Text = text3;
	}

	private void ResetPrgBitmapSize(bool forceReset)
	{
		int prgColumnCount = GetPrgColumnCount();
		int prgRowCount = GetPrgRowCount();
		if (forceReset || mColCount != prgColumnCount || mRowCount != prgRowCount)
		{
			mColCount = prgColumnCount;
			mRowCount = prgRowCount;
			UpdateStatusbar();
			InitializePrg();
			SetScrollBar();
		}
		mPatternManager.Size = new Size(mColCount, mRowCount);
		DrawPrg();
		UpdateTitle();
	}

	private void SetScrollBar()
	{
		int maximum = 0;
		if (mPrgData != null)
		{
			maximum = mPrgData.Length - 1;
		}
		scrollPanelPrg.Minimum = 0;
		scrollPanelPrg.Maximum = maximum;
		scrollPanelPrg.LrChange = 1;
		if (mConfiguration.YxSwap)
		{
			scrollPanelPrg.SmallChange = mRowCount;
			scrollPanelPrg.LargeChange = mRowCount * mColCount;
		}
		else
		{
			scrollPanelPrg.SmallChange = mColCount;
			scrollPanelPrg.LargeChange = mColCount * mRowCount;
		}
	}

	private void paletteSelectorSet_OnPaletteSetChanged(object sender, EventArgs e)
	{
		UpdateBytemapPalette();
		DrawBoth();
	}

	private void paletteSelectorSet_OnPaletteSelect(object sender, EventArgs e)
	{
		byte b = (byte)paletteSelectorSet.SelectedIndex;
		byte selectedIndex = mDatInfo.Data[b];
		paletteSelectorNes.SelectedIndex = selectedIndex;
		paletteSelectorNes.Refresh();
	}

	private void paletteSelectorNes_OnPaletteSelect(object sender, EventArgs e)
	{
		byte paletteSelectorSetPalette = (byte)paletteSelectorNes.SelectedIndex;
		SetPaletteSelectorSetPalette(paletteSelectorSetPalette);
		UpdateBytemapPalette();
		DrawBoth();
	}

	private void SetPaletteSelectorSetPalette(byte pal)
	{
		if (!mConfiguration.ImagePalette)
		{
			int selectedIndex = paletteSelectorSet.SelectedIndex;
			mDatInfo.Data[selectedIndex] = pal;
			paletteSelectorSet.Palette[selectedIndex] = mDatInfo.Colors[selectedIndex];
			paletteSelectorSet.Refresh();
		}
		UpdateControlState();
	}

	private void UpdateBytemapPalette()
	{
		if (mConfiguration.ImagePalette)
		{
			if (mLoadBytemap != null)
			{
				UpdateBytemapPalette(mLoadBytemap.Palette);
			}
		}
		else
		{
			UpdateBytemapPalette(mDatInfo.Colors);
		}
		UpdateControlState();
	}

	private void UpdateBytemapPalette(Color[] colors)
	{
		if (mChrBytemap != null)
		{
			CopyPaletteArray(mChrBytemap.Palette, colors);
			BytemapConvertor.UpdateBitmapPaletteFromBytemap(mChrBitmap, mChrBytemap);
		}
		if (mPrgBytemap != null)
		{
			CopyPaletteArray(mPrgBytemap.Palette, colors);
			BytemapConvertor.UpdateBitmapPaletteFromBytemap(mPrgBitmap, mPrgBytemap);
		}
		if (mChrBytemap != null)
		{
			Rectangle rect = new Rectangle(new Point(0, 0), mChrBytemap.Size);
			if (!mConfiguration.FullPalette && !mConfiguration.ImagePalette)
			{
				mChrBytemap.SetPaletteSet(rect, (byte)paletteSelectorSet.SetSize, (byte)paletteSelectorSet.SelectedSet);
			}
		}
	}

	private void CopyPaletteArray(Color[] dst, Color[] src)
	{
		if (dst != null && src != null && dst.Length == src.Length)
		{
			for (int i = 0; i < dst.Length; i++)
			{
				dst[i] = src[i];
			}
		}
	}

	private void MainForm_Activated(object sender, EventArgs e)
	{
		if (mPrgFile.CheckFileDateChanged())
		{
			mPrgFile.LoadFileDate();
			bool flag = false;
			if (mConfiguration.AutoReloadPrg == ConfigAskSetting.ShowDialog)
			{
				string fileName = Path.GetFileName(mPrgFile.FileName);
				string text = PrgEditor.Properties.String.MsgReloadPrg.Replace("@FILE", fileName);
				if (MsgBox.Show(this, text, "PRG-ROM Pattern Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					flag = true;
				}
			}
			else if (mConfiguration.AutoReloadPrg == ConfigAskSetting.AutoYes)
			{
				flag = true;
			}
			if (flag)
			{
				OpenPrgFile(mPrgFile.FileName);
				DrawBoth();
			}
		}
		if (!mChrFile.CheckFileDateChanged())
		{
			return;
		}
		mChrFile.LoadFileDate();
		bool flag2 = false;
		if (mConfiguration.AutoReloadChr == ConfigAskSetting.ShowDialog)
		{
			string fileName2 = Path.GetFileName(mChrFile.FileName);
			string text2 = PrgEditor.Properties.String.MsgReloadChr.Replace("@FILE", fileName2);
			if (MsgBox.Show(this, text2, "PRG-ROM Pattern Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				flag2 = true;
			}
		}
		else if (mConfiguration.AutoReloadChr == ConfigAskSetting.AutoYes)
		{
			flag2 = true;
		}
		if (flag2)
		{
			LoadChrBitmap(mChrFile.FileName);
			DrawBoth();
		}
	}

	private void InitializeChr()
	{
		int num = 128;
		int num2 = 128;
		if (num < 1)
		{
			num = 1;
		}
		if (num2 < 1)
		{
			num2 = 1;
		}
		if (mChrBytemap == null)
		{
			mChrBytemap = new Bytemap(num, num2);
		}
		if (mChrBitmap == null)
		{
			mChrBitmap = new Bitmap(num, num2, PixelFormat.Format8bppIndexed);
			selectorChr.Image = mChrBitmap;
		}
		UpdateBytemapPalette();
	}

	private void DrawChr()
	{
		BytemapConvertor.UpdateBitmapFromBytemap(mChrBitmap, mChrBytemap);
		if (selectorChr.SelectedColorNegative != mConfiguration.SelectorNegative)
		{
			selectorChr.SelectedColorNegative = mConfiguration.SelectorNegative;
		}
		selectorChr.Refresh();
	}

	private void LoadChrBitmap(string filename)
	{
		if (mChrBytemap == null)
		{
			return;
		}
		mChrFile.FileName = filename;
		mChrFile.LoadFileDate();
		using Bitmap loadImage = Image.FromFile(filename) as Bitmap;
		LoadChrBitmap(loadImage);
	}

	private void LoadChrBitmap(Bitmap loadImage)
	{
		mLoadBytemap = new Bytemap(loadImage.Size);
		BytemapConvertor.UpdateBytemapPaletteFromBitmap(loadImage, mLoadBytemap);
		BytemapConvertor.UpdateBytemapFromBitmap(loadImage, mLoadBytemap);
		LoadChrBytemap();
	}

	private void LoadChrBytemap()
	{
		if (mLoadBytemap != null)
		{
			mChrBytemap.CopyRect(new Point(0, 0), mLoadBytemap, new Rectangle(0, 0, 128, 128));
			UpdateBytemapPalette();
		}
	}

	private void InitializePrg()
	{
		int num = selectorPrg.ClientSize.Width / 2;
		int num2 = selectorPrg.ClientSize.Height / 2;
		if (num < 1)
		{
			num = 1;
		}
		if (num2 < 1)
		{
			num2 = 1;
		}
		if (mPrgBytemap != null && (mPrgBytemap.Width != num || mPrgBytemap.Height != num2))
		{
			mPrgBytemap = null;
		}
		if (mPrgBytemap == null)
		{
			mPrgBytemap = new Bytemap(num, num2);
		}
		if (mPrgBitmap != null && (mPrgBitmap.Width != num || mPrgBitmap.Height != num2))
		{
			mPrgBitmap.Dispose();
			mPrgBitmap = null;
		}
		if (mPrgBitmap == null)
		{
			mPrgBitmap = new Bitmap(num, num2, PixelFormat.Format8bppIndexed);
			selectorPrg.Image = mPrgBitmap;
		}
		UpdateBytemapPalette();
	}

	private void DrawPrg()
	{
		DrawPrgImage();
		BytemapConvertor.UpdateBitmapFromBytemap(mPrgBitmap, mPrgBytemap);
		if (selectorPrg.SelectedColorNegative != mConfiguration.SelectorNegative)
		{
			selectorPrg.SelectedColorNegative = mConfiguration.SelectorNegative;
		}
		selectorPrg.Refresh();
	}

	private void DrawPrgImage()
	{
		if (selectorChr.Image != null && selectorPrg.Image != null && mPrgData != null)
		{
			int skipByte = (mSkip1 ? 1 : 0);
			mPatternManager.CreateInfoFromByteData(mPrgData, Address, new Size(8, 8), mConfiguration.X8Y16, mConfiguration.X16Y16, mConfiguration.YxSwap, skipByte);
			DrawPatternData(mPatternManager.Infos, mPrgBytemap, mChrBytemap);
		}
	}

	private void DrawPatternData(RectCopyInfo[] rectInfos, Bytemap prgBytemap, Bytemap chrBytemap)
	{
		prgBytemap.Clear(0);
		foreach (RectCopyInfo rectCopyInfo in rectInfos)
		{
			prgBytemap.CopyRect(rectCopyInfo.DestPoint, chrBytemap, rectCopyInfo.SrcRect);
		}
	}

	private int GetDataAddr(int x, int y, int w, int h, PatternMode patternMode)
	{
		if (mConfiguration.YxSwap)
		{
			int num = y;
			y = x;
			x = num;
			int num2 = h;
			h = w;
			w = num2;
		}
		int num3 = 0;
		if (patternMode == PatternMode.P8x16)
		{
			return y / 2 * (w * 2) + x * 2 + y % 2;
		}
		return y * w + x;
	}

	private void DrawBoth()
	{
		DrawPrgImage();
		BytemapConvertor.UpdateBitmapFromBytemap(mPrgBitmap, mPrgBytemap);
		BytemapConvertor.UpdateBitmapFromBytemap(mChrBitmap, mChrBytemap);
		if (selectorChr.SelectedColorNegative != mConfiguration.SelectorNegative)
		{
			selectorChr.SelectedColorNegative = mConfiguration.SelectorNegative;
		}
		if (selectorPrg.SelectedColorNegative != mConfiguration.SelectorNegative)
		{
			selectorPrg.SelectedColorNegative = mConfiguration.SelectorNegative;
		}
		selectorPrg.Refresh();
		selectorChr.Refresh();
	}

	private void UpdateControlState()
	{
		bool fullPalette = mConfiguration.FullPalette;
		miViewFullPaletteMode.Checked = fullPalette;
		tsbViewFullPaletteMode.Checked = fullPalette;
		bool imagePalette = mConfiguration.ImagePalette;
		miViewImagePaletteMode.Checked = imagePalette;
		tsbViewImagePaletteMode.Checked = imagePalette;
		paletteSelectorNes.Enabled = !imagePalette;
		paletteSelectorSet.Enabled = !imagePalette;
		miViewFullPaletteMode.Enabled = !imagePalette;
		tsbViewFullPaletteMode.Enabled = !imagePalette;
	}

	private void OpenPrgFile(string filename)
	{
		if (!string.IsNullOrEmpty(filename))
		{
			mPrgData = File.ReadAllBytes(filename);
			mPrgFile.FileName = filename;
			mPrgFile.LoadFileDate();
			string fullPath = Path.GetFullPath(filename);
			mConfiguration.PrgPath = Path.GetDirectoryName(fullPath);
			SetScrollBar();
			UpdateTextAddrSize();
		}
	}

	private void OpenPrgFileDataFile(string filename)
	{
		string dataFilename = Utility.GetDataFilename(filename, "bmp");
		if (!File.Exists(dataFilename))
		{
			return;
		}
		bool flag = false;
		if (mConfiguration.AutoLoadChr == ConfigAskSetting.ShowDialog)
		{
			string fileName = Path.GetFileName(dataFilename);
			string text = PrgEditor.Properties.String.MsgAutoLoadChr.Replace("@FILE", fileName);
			if (MsgBox.Show(this, text, "PRG-ROM Pattern Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				flag = true;
			}
		}
		else if (mConfiguration.AutoLoadChr == ConfigAskSetting.AutoYes)
		{
			flag = true;
		}
		if (flag)
		{
			LoadChrBitmap(dataFilename);
		}
	}

	private void actionFileOpen(object sender, EventArgs e)
	{
		if (!string.IsNullOrWhiteSpace(mConfiguration.PrgPath) && Directory.Exists(mConfiguration.PrgPath))
		{
			openFileDialogPrg.InitialDirectory = mConfiguration.PrgPath;
		}
		if (string.IsNullOrWhiteSpace(mConfiguration.PrgPath) && !string.IsNullOrWhiteSpace(openFileDialogPrg.FileName))
		{
			string directoryName = Path.GetDirectoryName(openFileDialogPrg.FileName);
			if (Directory.Exists(directoryName))
			{
				openFileDialogPrg.InitialDirectory = directoryName;
			}
		}
		if (openFileDialogPrg.ShowDialog() == DialogResult.OK)
		{
			string fileName = openFileDialogPrg.FileName;
			Address = 0;
			OpenPrgFile(fileName);
			OpenPrgFileDataFile(fileName);
		}
		DrawBoth();
		UpdateTitle();
	}

	private void actionFileReload(object sender, EventArgs e)
	{
		string fileName = mPrgFile.FileName;
		if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
		{
			OpenPrgFile(fileName);
		}
		DrawBoth();
		UpdateTitle();
	}

	private void WritePrgFile(string filename)
	{
		if (!string.IsNullOrEmpty(filename))
		{
			File.WriteAllBytes(filename, mPrgData);
			mPrgFile.FileName = filename;
			mPrgFile.LoadFileDate();
			string fullPath = Path.GetFullPath(filename);
			mConfiguration.PrgPath = Path.GetDirectoryName(fullPath);
			SetScrollBar();
			UpdateTextAddrSize();
		}
	}

	private void actionFileSave(object sender, EventArgs e)
	{
		WritePrgFile(mPrgFile.FileName);
		DrawPrg();
		UpdateTitle();
	}

	private void actionFileSaveAs(object sender, EventArgs e)
	{
		if (!string.IsNullOrEmpty(mConfiguration.PrgPath) && Directory.Exists(mConfiguration.PrgPath))
		{
			saveFileDialogPrg.InitialDirectory = mConfiguration.PrgPath;
		}
		if (saveFileDialogPrg.ShowDialog() == DialogResult.OK)
		{
			string fileName = saveFileDialogPrg.FileName;
			WritePrgFile(fileName);
		}
		DrawPrg();
		UpdateTitle();
	}

	private void actionFileSavePrgBitmap(object sender, EventArgs e)
	{
		if (!string.IsNullOrEmpty(mConfiguration.PrgPath) && Directory.Exists(mConfiguration.PrgPath))
		{
			saveFileDialogBmp.InitialDirectory = mConfiguration.PrgPath;
		}
		if (saveFileDialogBmp.ShowDialog() == DialogResult.OK)
		{
			string fileName = saveFileDialogBmp.FileName;
			mPrgBitmap.Save(fileName, ImageFormat.Bmp);
		}
	}

	private void actionFileExit(object sender, EventArgs e)
	{
		Close();
	}

	private void actionEditMode(object sender, EventArgs e)
	{
		if (sender == tsbEditModeStep || sender == miEditModeStepByStep)
		{
			mEditMode = EditMode.StepByStep;
		}
		if (sender == tsbEditModeTypeText || sender == miEditModeTypeText)
		{
			mEditMode = EditMode.TypeText;
		}
		if (sender == tsbEditModeDrawPict || sender == miEditModeDrawPicture)
		{
			mEditMode = EditMode.DrawPicture;
		}
		UpdateEditMode();
		DrawBoth();
	}

	private void UpdateEditMode()
	{
		selectorPrg.EnableRightDragSelect = false;
		selectorPrg.SelectorVisible = mEditMode != EditMode.DrawPicture;
		selectorChr.EnableRightDragSelect = false;
		selectorChr.SelectorVisible = mEditMode != EditMode.TypeText;
		tsbEditModeStep.Checked = mEditMode == EditMode.StepByStep;
		miEditModeStepByStep.Checked = mEditMode == EditMode.StepByStep;
		tsbEditModeTypeText.Checked = mEditMode == EditMode.TypeText;
		miEditModeTypeText.Checked = mEditMode == EditMode.TypeText;
		tsbEditModeDrawPict.Checked = mEditMode == EditMode.DrawPicture;
		miEditModeDrawPicture.Checked = mEditMode == EditMode.DrawPicture;
	}

	private void actionEditSearch(object sender, EventArgs e)
	{
		SearchForm searchForm = new SearchForm(mChrBytemap, mFindData);
		if (searchForm.ShowDialog(this) != DialogResult.OK)
		{
			return;
		}
		if (searchForm.Data != null)
		{
			int num = Math.Min(mFindData.Length, searchForm.Data.Length);
			for (int i = 0; i < num; i++)
			{
				mFindData[i] = searchForm.Data[i];
			}
		}
		mFindType = searchForm.Type;
		if (searchForm.Direction == SearchForm.FindDirection.Previous)
		{
			Find(mPrgData, mFindData, Address, SearchForm.FindDirection.Previous);
		}
		if (searchForm.Direction == SearchForm.FindDirection.Next)
		{
			Find(mPrgData, mFindData, Address, SearchForm.FindDirection.Next);
		}
	}

	private void actionEditFind(object sender, EventArgs e)
	{
		if (sender == miEditFindPrevious || sender == tsbAddrFindPrev || sender == tsbAddrFindPrev2)
		{
			Find(mPrgData, mFindData, Address, SearchForm.FindDirection.Previous);
		}
		if (sender == miEditFindNext || sender == tsbAddrFindNext || sender == tsbAddrFindNext2)
		{
			Find(mPrgData, mFindData, Address, SearchForm.FindDirection.Next);
		}
	}

	private void Find(byte[] data, byte[] findData, int address, SearchForm.FindDirection findDirection)
	{
		if (data == null)
		{
			return;
		}
		bool flag = true;
		int num = data.Length;
		if (findDirection == SearchForm.FindDirection.Next)
		{
			int num2 = address + 1;
			if (mFindType == SearchForm.FindType.Text)
			{
				for (int i = 0; i < num; i++)
				{
					if (FindText(data, num2, findData))
					{
						Address = num2;
						mConfiguration.X8Y16 = false;
						flag = false;
						break;
					}
					num2++;
					if (num2 >= num)
					{
						num2 = 0;
					}
				}
			}
			else if (mFindType == SearchForm.FindType.Graphic)
			{
				for (int j = 0; j < num; j++)
				{
					if (FindText(data, num2, findData))
					{
						Address = num2;
						mConfiguration.X8Y16 = false;
						flag = false;
						break;
					}
					if (Find8x16(data, num2, findData))
					{
						Address = num2;
						mConfiguration.X8Y16 = true;
						flag = false;
						break;
					}
					if (FindBG(data, num2, findData))
					{
						Address = num2;
						mConfiguration.X8Y16 = false;
						flag = false;
						break;
					}
					num2++;
					if (num2 >= num)
					{
						num2 = 0;
					}
				}
			}
		}
		else
		{
			int num3 = address - 1;
			if (mFindType == SearchForm.FindType.Text)
			{
				for (int k = 0; k < num; k++)
				{
					if (FindText(data, num3, findData))
					{
						Address = num3;
						mConfiguration.X8Y16 = false;
						flag = false;
						break;
					}
					num3--;
					if (num3 <= 0)
					{
						num3 = num - 1;
					}
				}
			}
			else if (mFindType == SearchForm.FindType.Graphic)
			{
				for (int l = 0; l < num; l++)
				{
					if (FindText(data, num3, findData))
					{
						Address = num3;
						mConfiguration.X8Y16 = false;
						flag = false;
						break;
					}
					if (Find8x16(data, num3, findData))
					{
						Address = num3;
						mConfiguration.X8Y16 = true;
						flag = false;
						break;
					}
					if (FindBG(data, num3, findData))
					{
						Address = num3;
						mConfiguration.X8Y16 = false;
						flag = false;
						break;
					}
					num3--;
					if (num3 <= 0)
					{
						num3 = num - 1;
					}
				}
			}
		}
		UpdateMenuPatternMode();
		DrawBoth();
		if (flag)
		{
			MsgBox.Show(this, PrgEditor.Properties.String.MsgNotFound, "PRG-ROM Pattern Editor", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}
	}

	private bool FindText(byte[] data, int addr, byte[] findData)
	{
		bool result = false;
		if (GetData(data, addr) == findData[0] && GetData(data, addr + 1) == findData[1] && GetData(data, addr + 2) == findData[2] && GetData(data, addr + 3) == findData[3])
		{
			result = true;
		}
		return result;
	}

	private bool Find8x16(byte[] data, int addr, byte[] findData)
	{
		bool result = false;
		if (GetData(data, addr) == findData[0] && GetData(data, addr + 1) == findData[2] && GetData(data, addr + 2) == findData[1] && GetData(data, addr + 3) == findData[3])
		{
			result = true;
		}
		return result;
	}

	private bool FindBG(byte[] data, int addr, byte[] findData)
	{
		bool result = false;
		if (GetData(data, addr) == findData[0] && GetData(data, addr + 1) == findData[1] && GetData(data, addr + 32) == findData[2] && GetData(data, addr + 33) == findData[3])
		{
			result = true;
		}
		return result;
	}

	private byte GetData(byte[] data, int di)
	{
		int num = ((di < data.Length) ? di : (di % data.Length));
		return data[num];
	}

	private void actionViewGridPrg(object sender, EventArgs e)
	{
		mConfiguration.GridPrgEnable = !mConfiguration.GridPrgEnable;
		InitializeFromSetting();
		DrawBoth();
	}

	private void actionViewGridChr(object sender, EventArgs e)
	{
		mConfiguration.GridChrEnable = !mConfiguration.GridChrEnable;
		InitializeFromSetting();
		DrawBoth();
	}

	private void actionViewOpenChr(object sender, EventArgs e)
	{
		if (!string.IsNullOrWhiteSpace(mConfiguration.ChrPath) && Directory.Exists(mConfiguration.ChrPath))
		{
			openFileDialogChr.InitialDirectory = mConfiguration.ChrPath;
		}
		if (string.IsNullOrWhiteSpace(mConfiguration.ChrPath) && !string.IsNullOrWhiteSpace(openFileDialogChr.FileName))
		{
			string directoryName = Path.GetDirectoryName(openFileDialogChr.FileName);
			if (Directory.Exists(directoryName))
			{
				openFileDialogChr.InitialDirectory = directoryName;
			}
		}
		if (openFileDialogChr.ShowDialog() == DialogResult.OK)
		{
			string fileName = openFileDialogChr.FileName;
			string fullPath = Path.GetFullPath(fileName);
			mConfiguration.ChrPath = Path.GetDirectoryName(fullPath);
			LoadChrBitmap(fileName);
		}
		DrawBoth();
	}

	private void actionViewOpenPaletteFromState(object sender, EventArgs e)
	{
		if (!string.IsNullOrWhiteSpace(mConfiguration.StatePath) && Directory.Exists(mConfiguration.StatePath))
		{
			openFileDialogEtc.InitialDirectory = mConfiguration.StatePath;
		}
		if (string.IsNullOrWhiteSpace(mConfiguration.StatePath) && !string.IsNullOrWhiteSpace(openFileDialogEtc.FileName))
		{
			string directoryName = Path.GetDirectoryName(openFileDialogEtc.FileName);
			if (Directory.Exists(directoryName))
			{
				openFileDialogEtc.InitialDirectory = directoryName;
			}
		}
		if (openFileDialogEtc.ShowDialog() == DialogResult.OK)
		{
			string fileName = openFileDialogEtc.FileName;
			string fullPath = Path.GetFullPath(fileName);
			mConfiguration.StatePath = Path.GetDirectoryName(fullPath);
			DataFileManager instance = DataFileManager.GetInstance();
			new DataFileState().LoadFromFile(fileName);
			for (int i = 0; i < 32; i++)
			{
				mDatInfo.Data[i] = instance.DatInfoNes.Data[i];
			}
			CopyPaletteArray(paletteSelectorSet.Palette, mDatInfo.Colors);
			UpdateBytemapPalette();
		}
		paletteSelectorSet.Refresh();
		DrawBoth();
	}

	private void actionViewFullPaletteMode(object sender, EventArgs e)
	{
		mConfiguration.FullPalette = !mConfiguration.FullPalette;
		UpdateControlState();
		if (mConfiguration.FullPalette)
		{
			paletteSelectorSet.SelectedIndex = 0;
			paletteSelectorSet.SetSize = 32;
		}
		else
		{
			paletteSelectorSet.SelectedIndex = 0;
			paletteSelectorSet.SetSize = 4;
		}
		LoadChrBytemap();
		DrawBoth();
	}

	private void actionViewImagePaletteMode(object sender, EventArgs e)
	{
		mConfiguration.ImagePalette = !mConfiguration.ImagePalette;
		UpdateControlState();
		if (mConfiguration.ImagePalette)
		{
			paletteSelectorSet.SelectedIndex = 0;
			paletteSelectorSet.SetSize = 256;
		}
		else
		{
			paletteSelectorSet.SelectedIndex = 0;
			paletteSelectorSet.SetSize = 4;
		}
		LoadChrBytemap();
		DrawBoth();
	}

	private void actionAddrMove(object sender, EventArgs e)
	{
		int num = Address;
		if (sender == tsbAddr0)
		{
			num = scrollPanelPrg.Minimum;
		}
		if (sender == tsbAddr1)
		{
			num -= scrollPanelPrg.LargeChange;
		}
		if (sender == tsbAddr2)
		{
			num -= scrollPanelPrg.LargeChange / 2;
		}
		if (sender == tsbAddr3)
		{
			num -= scrollPanelPrg.SmallChange;
		}
		if (sender == tsbAddr4)
		{
			num--;
		}
		if (sender == tsbAddr5)
		{
			num++;
		}
		if (sender == tsbAddr6)
		{
			num += scrollPanelPrg.SmallChange;
		}
		if (sender == tsbAddr7)
		{
			num += scrollPanelPrg.LargeChange / 2;
		}
		if (sender == tsbAddr8)
		{
			num += scrollPanelPrg.LargeChange;
		}
		if (sender == tsbAddr9)
		{
			num = scrollPanelPrg.Maximum;
		}
		Address = num;
	}

	private void actionAddrInput(object sender, EventArgs e)
	{
		AddressInputForm addressInputForm = new AddressInputForm();
		addressInputForm.Address = Address;
		if (addressInputForm.ShowDialog(this) == DialogResult.OK)
		{
			Address = addressInputForm.Address;
		}
	}

	private void actionOptionConfiguration(object sender, EventArgs e)
	{
		SettingForm settingForm = new SettingForm();
		settingForm.LoadSetting(mConfiguration);
		if (settingForm.ShowDialog() == DialogResult.OK)
		{
			settingForm.SaveSetting(mConfiguration);
			mConfiguration.SaveToFile(mIniFileName);
		}
		InitializeFromSetting();
		DrawBoth();
	}

	private void actionHelpCheckUpdate(object sender, EventArgs e)
	{
		Process.Start(new ProcessStartInfo("https://www45.atwiki.jp/yychr/"));
	}

	private void actionHelpPropertyEditor(object sender, EventArgs ev)
	{
		try
		{
			if (propertyEditorForm != null && propertyEditorForm.IsDisposed)
			{
				propertyEditorForm = null;
			}
			if (propertyEditorForm == null)
			{
				propertyEditorForm = new PropertyEditorForm();
			}
			propertyEditorForm.EditObject = this;
			propertyEditorForm.Show();
		}
		catch (Exception ex)
		{
			MsgBox.Show(this, ex.Message, "PRG-ROM Pattern Editor");
		}
	}

	private void actionHelpAbout(object sender, EventArgs e)
	{
		string obj = "PRG-ROM Pattern Editor Version 0.50 build " + GetBuildDate() + "\r\n\t\t:: Copyright 2012 Yy, Yorn\r\n\t\t:: https://www45.atwiki.jp/yychr/\r\n\r\n------------------------lib------------------------\r\nFamicom Palette\t:: By Kasion\r\n\t\t:: http://hlc6502.web.fc2.com/NesPal2.htm\r\nFamicom Palette\t:: By misaki\r\n\t\t:: http://metalidol.xxxxxxxx.jp/famicom_palette.html\r\nMSX Palette\t:: By wizforest\r\n\t\t:: https://www.wizforest.com/OldGood/ntsc/msx.html\r\n";
		Size size = new Size(640, 320);
		MessageBoxForm.Show(obj, "About PRG-ROM Pattern Editor", MessageBoxIcon.Asterisk, MessageBoxButtons.OK, size);
	}

	private void actionResize(object sender, EventArgs e)
	{
		Size size = base.Size;
		if (sender == tsbResizeWDec)
		{
			size.Width -= 16;
		}
		if (sender == tsbResizeWInc)
		{
			size.Width += 16;
		}
		if (sender == tsbResizeHDec)
		{
			size.Height -= 16;
		}
		if (sender == tsbResizeHInc)
		{
			size.Height += 16;
		}
		if (base.Size != size)
		{
			base.Size = size;
		}
	}

	private void UpdateMenuPatternMode()
	{
		tsbMode8x16.Checked = mConfiguration.X8Y16;
		tsbMode16x16.Checked = mConfiguration.X16Y16;
		if (mConfiguration.X8Y16)
		{
			mPatternMode = PatternMode.P8x16;
		}
		else if (mConfiguration.X16Y16)
		{
			mPatternMode = PatternMode.P16x16;
		}
		else
		{
			mPatternMode = PatternMode.P8x8;
		}
	}

	private void actionMiscMode8x16(object sender, EventArgs e)
	{
		mConfiguration.X8Y16 = !mConfiguration.X8Y16;
		if (mConfiguration.X8Y16)
		{
			mConfiguration.X16Y16 = !mConfiguration.X8Y16;
		}
		UpdateMenuPatternMode();
		DrawPrg();
	}

	private void actionMiscMode16x16(object sender, EventArgs e)
	{
		mConfiguration.X16Y16 = !mConfiguration.X16Y16;
		if (mConfiguration.X16Y16)
		{
			mConfiguration.X8Y16 = !mConfiguration.X16Y16;
		}
		UpdateMenuPatternMode();
		DrawPrg();
	}

	private void actionMiscYxSwap(object sender, EventArgs e)
	{
		mConfiguration.YxSwap = !mConfiguration.YxSwap;
		UpdateYxSwapMode();
		AdjustSelectorPrg();
	}

	private void UpdateYxSwapMode()
	{
		tsbYxSwap.Checked = mConfiguration.YxSwap;
		if (mConfiguration.YxSwap)
		{
			scrollPanelPrg.ScrollBarType = ScrollPanel.ScrollBarTypes.Horizontal;
		}
		else
		{
			scrollPanelPrg.ScrollBarType = ScrollPanel.ScrollBarTypes.Vertical;
		}
	}

	private void actionMiscSkip1(object sender, EventArgs e)
	{
		mSkip1 = !mSkip1;
		UpdateMenuSkip();
		DrawPrg();
	}

	private void UpdateMenuSkip()
	{
		tbSkip1.Checked = mSkip1;
	}

	private void ActionDragEnter(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(DataFormats.FileDrop))
		{
			if (sender == scrollPanelPrg || sender == panelRight)
			{
				e.Effect = DragDropEffects.Copy;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}
		else
		{
			e.Effect = DragDropEffects.None;
		}
	}

	private void ActionDragDrop(object sender, DragEventArgs e)
	{
		string[] array = (string[])e.Data.GetData(DataFormats.FileDrop, autoConvert: false);
		if (array == null || string.IsNullOrEmpty(array[0]))
		{
			return;
		}
		string text = array[0];
		switch (Path.GetExtension(text).Trim('.').ToLower())
		{
		case "bmp":
			LoadChrBitmap(text);
			break;
		case "nes":
		case "prg":
		case "rom":
		case "msx1":
		case "msx2":
		case "smc":
		case "sfc":
		case "gen":
		case "gb":
		case "gbc":
			OpenPrgFile(text);
			break;
		default:
			if (sender == scrollPanelPrg)
			{
				OpenPrgFile(text);
			}
			else if (sender == panelRight)
			{
				LoadChrBitmap(text);
			}
			break;
		}
		DrawBoth();
		UpdateTitle();
		UpdateStatusbar();
	}

	private string GetBuildDate()
	{
		Version version = GetType().Assembly.GetName().Version;
		int build = version.Build;
		int revision = version.Revision;
		return new DateTime(2000, 1, 1).AddDays(build).AddSeconds(revision * 2).ToString("yyyy/MM/dd");
	}

	private void UpdateBuildDateLabel()
	{
		lVersion.Text = "build " + GetBuildDate();
		int num = base.ClientSize.Width - lVersion.Width;
		lVersion.Location = new Point(num, lVersion.Top);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrgEditor.MainForm));
		this.tsContainer = new System.Windows.Forms.ToolStripContainer();
		this.panel2 = new System.Windows.Forms.Panel();
		this.toolStripPrgAddr = new System.Windows.Forms.ToolStrip();
		this.tsPrgSpace1 = new System.Windows.Forms.ToolStripLabel();
		this.tsbViewGridPrg2 = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
		this.tsbFileOpen2 = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
		this.tsbAddr0 = new System.Windows.Forms.ToolStripButton();
		this.tsbAddr1 = new System.Windows.Forms.ToolStripButton();
		this.tsbAddr2 = new System.Windows.Forms.ToolStripButton();
		this.tsbAddr3 = new System.Windows.Forms.ToolStripButton();
		this.tsbAddr4 = new System.Windows.Forms.ToolStripButton();
		this.tsbAddr5 = new System.Windows.Forms.ToolStripButton();
		this.tsbAddr6 = new System.Windows.Forms.ToolStripButton();
		this.tsbAddr7 = new System.Windows.Forms.ToolStripButton();
		this.tsbAddr8 = new System.Windows.Forms.ToolStripButton();
		this.tsbAddr9 = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
		this.tsbAddrInput = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
		this.tsbAddrFind = new System.Windows.Forms.ToolStripButton();
		this.tsbAddrFindPrev = new System.Windows.Forms.ToolStripButton();
		this.tsbAddrFindNext = new System.Windows.Forms.ToolStripButton();
		this.scrollPanelPrg = new ControlLib.ScrollPanel();
		this.selectorPrg = new ControlLib.CellSelector();
		this.panelRight = new System.Windows.Forms.Panel();
		this.toolStripChr = new System.Windows.Forms.ToolStrip();
		this.tsChrSpace1 = new System.Windows.Forms.ToolStripLabel();
		this.tsbViewGridChr2 = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
		this.tsbViewOpenChr2 = new System.Windows.Forms.ToolStripButton();
		this.tsChrSpace2 = new System.Windows.Forms.ToolStripLabel();
		this.tsbViewOpenState2 = new System.Windows.Forms.ToolStripButton();
		this.tsbViewFullPaletteMode = new System.Windows.Forms.ToolStripButton();
		this.tsbViewImagePaletteMode = new System.Windows.Forms.ToolStripButton();
		this.paletteSelectorNes = new ControlLib.PaletteSelector();
		this.paletteSelectorSet = new ControlLib.PaletteSelector();
		this.selectorChr = new ControlLib.CellSelector();
		this.statusStrip1 = new System.Windows.Forms.StatusStrip();
		this.toolStripStatusLabelView = new System.Windows.Forms.ToolStripStatusLabel();
		this.toolStripStatusLabelAddrValue = new System.Windows.Forms.ToolStripStatusLabel();
		this.toolStripStatusLabelWH = new System.Windows.Forms.ToolStripStatusLabel();
		this.toolStripStatusLabelSelected = new System.Windows.Forms.ToolStripStatusLabel();
		this.toolStripStatusLabelXY = new System.Windows.Forms.ToolStripStatusLabel();
		this.toolStripStatusLabelCarretAddrValue = new System.Windows.Forms.ToolStripStatusLabel();
		this.toolStrip1 = new System.Windows.Forms.ToolStrip();
		this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
		this.tsbFileOpen1 = new System.Windows.Forms.ToolStripButton();
		this.tsbFileSave = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.tsbViewOpenChr1 = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
		this.tsbViewOpenState1 = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
		this.tsbResizeHDec = new System.Windows.Forms.ToolStripButton();
		this.tsbResizeHInc = new System.Windows.Forms.ToolStripButton();
		this.tsbResizeWDec = new System.Windows.Forms.ToolStripButton();
		this.tsbResizeWInc = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
		this.tsbViewGridPrg1 = new System.Windows.Forms.ToolStripButton();
		this.tsbViewGridChr1 = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
		this.tsbEditModeStep = new System.Windows.Forms.ToolStripButton();
		this.tsbEditModeTypeText = new System.Windows.Forms.ToolStripButton();
		this.tsbEditModeDrawPict = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
		this.tsbMode8x16 = new System.Windows.Forms.ToolStripButton();
		this.tsbMode16x16 = new System.Windows.Forms.ToolStripButton();
		this.tsbYxSwap = new System.Windows.Forms.ToolStripButton();
		this.tsbAddrFindNext2 = new System.Windows.Forms.ToolStripButton();
		this.tsbAddrFindPrev2 = new System.Windows.Forms.ToolStripButton();
		this.tsbAddrSearch2 = new System.Windows.Forms.ToolStripButton();
		this.tbSkip1 = new System.Windows.Forms.ToolStripButton();
		this.menuStrip1 = new System.Windows.Forms.MenuStrip();
		this.miFile = new System.Windows.Forms.ToolStripMenuItem();
		this.miFileOpen = new System.Windows.Forms.ToolStripMenuItem();
		this.miFileReload = new System.Windows.Forms.ToolStripMenuItem();
		this.miFileSave = new System.Windows.Forms.ToolStripMenuItem();
		this.miFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
		this.miFileSavePrgBitmap = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
		this.miFileExit = new System.Windows.Forms.ToolStripMenuItem();
		this.miEdit = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditMode = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditModeStepByStep = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditModeTypeText = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditModeDrawPicture = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
		this.miEditSearch = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditFindPrevious = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditFindNext = new System.Windows.Forms.ToolStripMenuItem();
		this.miView = new System.Windows.Forms.ToolStripMenuItem();
		this.miViewGridPrg = new System.Windows.Forms.ToolStripMenuItem();
		this.miViewGridChr = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
		this.miViewOpenChr = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
		this.miViewOpenState = new System.Windows.Forms.ToolStripMenuItem();
		this.miViewFullPaletteMode = new System.Windows.Forms.ToolStripMenuItem();
		this.miViewImagePaletteMode = new System.Windows.Forms.ToolStripMenuItem();
		this.miOption = new System.Windows.Forms.ToolStripMenuItem();
		this.miOptionConfiguration = new System.Windows.Forms.ToolStripMenuItem();
		this.miHelp = new System.Windows.Forms.ToolStripMenuItem();
		this.miHelpCheckUpdate = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
		this.miHelpPropertyEditor = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
		this.miHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
		this.openFileDialogPrg = new System.Windows.Forms.OpenFileDialog();
		this.saveFileDialogPrg = new System.Windows.Forms.SaveFileDialog();
		this.openFileDialogChr = new System.Windows.Forms.OpenFileDialog();
		this.openFileDialogEtc = new System.Windows.Forms.OpenFileDialog();
		this.saveFileDialogBmp = new System.Windows.Forms.SaveFileDialog();
		this.lVersion = new System.Windows.Forms.Label();
		this.tsContainer.ContentPanel.SuspendLayout();
		this.tsContainer.TopToolStripPanel.SuspendLayout();
		this.tsContainer.SuspendLayout();
		this.panel2.SuspendLayout();
		this.toolStripPrgAddr.SuspendLayout();
		this.scrollPanelPrg.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.selectorPrg).BeginInit();
		this.panelRight.SuspendLayout();
		this.toolStripChr.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.selectorChr).BeginInit();
		this.statusStrip1.SuspendLayout();
		this.toolStrip1.SuspendLayout();
		this.menuStrip1.SuspendLayout();
		base.SuspendLayout();
		resources.ApplyResources(this.tsContainer.ContentPanel, "tsContainer.ContentPanel");
		this.tsContainer.ContentPanel.Controls.Add(this.panel2);
		this.tsContainer.ContentPanel.Controls.Add(this.panelRight);
		this.tsContainer.ContentPanel.Controls.Add(this.statusStrip1);
		resources.ApplyResources(this.tsContainer, "tsContainer");
		this.tsContainer.Name = "tsContainer";
		this.tsContainer.RightToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		this.tsContainer.TopToolStripPanel.Controls.Add(this.toolStrip1);
		this.tsContainer.TopToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		this.panel2.Controls.Add(this.toolStripPrgAddr);
		this.panel2.Controls.Add(this.scrollPanelPrg);
		resources.ApplyResources(this.panel2, "panel2");
		this.panel2.Name = "panel2";
		resources.ApplyResources(this.toolStripPrgAddr, "toolStripPrgAddr");
		this.toolStripPrgAddr.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		this.toolStripPrgAddr.Items.AddRange(new System.Windows.Forms.ToolStripItem[21]
		{
			this.tsPrgSpace1, this.tsbViewGridPrg2, this.toolStripSeparator7, this.tsbFileOpen2, this.toolStripSeparator9, this.tsbAddr0, this.tsbAddr1, this.tsbAddr2, this.tsbAddr3, this.tsbAddr4,
			this.tsbAddr5, this.tsbAddr6, this.tsbAddr7, this.tsbAddr8, this.tsbAddr9, this.toolStripSeparator6, this.tsbAddrInput, this.toolStripSeparator10, this.tsbAddrFind, this.tsbAddrFindPrev,
			this.tsbAddrFindNext
		});
		this.toolStripPrgAddr.Name = "toolStripPrgAddr";
		this.toolStripPrgAddr.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		this.toolStripPrgAddr.Stretch = true;
		resources.ApplyResources(this.tsPrgSpace1, "tsPrgSpace1");
		this.tsPrgSpace1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
		this.tsPrgSpace1.Name = "tsPrgSpace1";
		this.tsbViewGridPrg2.Checked = true;
		this.tsbViewGridPrg2.CheckOnClick = true;
		this.tsbViewGridPrg2.CheckState = System.Windows.Forms.CheckState.Checked;
		this.tsbViewGridPrg2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbViewGridPrg2.Image = PrgEditor.Properties.Resources.GridEdit;
		resources.ApplyResources(this.tsbViewGridPrg2, "tsbViewGridPrg2");
		this.tsbViewGridPrg2.Name = "tsbViewGridPrg2";
		this.tsbViewGridPrg2.Click += new System.EventHandler(actionViewGridPrg);
		this.toolStripSeparator7.Name = "toolStripSeparator7";
		resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
		this.tsbFileOpen2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbFileOpen2.Image = PrgEditor.Properties.Resources.FileOpenPrg;
		resources.ApplyResources(this.tsbFileOpen2, "tsbFileOpen2");
		this.tsbFileOpen2.Name = "tsbFileOpen2";
		this.tsbFileOpen2.Click += new System.EventHandler(actionFileOpen);
		this.toolStripSeparator9.Name = "toolStripSeparator9";
		resources.ApplyResources(this.toolStripSeparator9, "toolStripSeparator9");
		this.tsbAddr0.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbAddr0.Image = PrgEditor.Properties.Resources.Addr0;
		resources.ApplyResources(this.tsbAddr0, "tsbAddr0");
		this.tsbAddr0.Name = "tsbAddr0";
		this.tsbAddr0.Click += new System.EventHandler(actionAddrMove);
		this.tsbAddr1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbAddr1.Image = PrgEditor.Properties.Resources.Addr1;
		resources.ApplyResources(this.tsbAddr1, "tsbAddr1");
		this.tsbAddr1.Name = "tsbAddr1";
		this.tsbAddr1.Click += new System.EventHandler(actionAddrMove);
		this.tsbAddr2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbAddr2.Image = PrgEditor.Properties.Resources.Addr2;
		resources.ApplyResources(this.tsbAddr2, "tsbAddr2");
		this.tsbAddr2.Name = "tsbAddr2";
		this.tsbAddr2.Click += new System.EventHandler(actionAddrMove);
		this.tsbAddr3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbAddr3.Image = PrgEditor.Properties.Resources.Addr3;
		resources.ApplyResources(this.tsbAddr3, "tsbAddr3");
		this.tsbAddr3.Name = "tsbAddr3";
		this.tsbAddr3.Click += new System.EventHandler(actionAddrMove);
		this.tsbAddr4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbAddr4.Image = PrgEditor.Properties.Resources.Addr4;
		resources.ApplyResources(this.tsbAddr4, "tsbAddr4");
		this.tsbAddr4.Name = "tsbAddr4";
		this.tsbAddr4.Click += new System.EventHandler(actionAddrMove);
		this.tsbAddr5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbAddr5.Image = PrgEditor.Properties.Resources.Addr5;
		resources.ApplyResources(this.tsbAddr5, "tsbAddr5");
		this.tsbAddr5.Name = "tsbAddr5";
		this.tsbAddr5.Click += new System.EventHandler(actionAddrMove);
		this.tsbAddr6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbAddr6.Image = PrgEditor.Properties.Resources.Addr6;
		resources.ApplyResources(this.tsbAddr6, "tsbAddr6");
		this.tsbAddr6.Name = "tsbAddr6";
		this.tsbAddr6.Click += new System.EventHandler(actionAddrMove);
		this.tsbAddr7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbAddr7.Image = PrgEditor.Properties.Resources.Addr7;
		resources.ApplyResources(this.tsbAddr7, "tsbAddr7");
		this.tsbAddr7.Name = "tsbAddr7";
		this.tsbAddr7.Click += new System.EventHandler(actionAddrMove);
		this.tsbAddr8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbAddr8.Image = PrgEditor.Properties.Resources.Addr8;
		resources.ApplyResources(this.tsbAddr8, "tsbAddr8");
		this.tsbAddr8.Name = "tsbAddr8";
		this.tsbAddr8.Click += new System.EventHandler(actionAddrMove);
		this.tsbAddr9.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbAddr9.Image = PrgEditor.Properties.Resources.Addr9;
		resources.ApplyResources(this.tsbAddr9, "tsbAddr9");
		this.tsbAddr9.Name = "tsbAddr9";
		this.tsbAddr9.Click += new System.EventHandler(actionAddrMove);
		this.toolStripSeparator6.Name = "toolStripSeparator6";
		resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
		this.tsbAddrInput.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbAddrInput.Image = PrgEditor.Properties.Resources.AddrA;
		resources.ApplyResources(this.tsbAddrInput, "tsbAddrInput");
		this.tsbAddrInput.Name = "tsbAddrInput";
		this.tsbAddrInput.Click += new System.EventHandler(actionAddrInput);
		this.toolStripSeparator10.Name = "toolStripSeparator10";
		resources.ApplyResources(this.toolStripSeparator10, "toolStripSeparator10");
		this.tsbAddrFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbAddrFind.Image = PrgEditor.Properties.Resources.AddrFind;
		resources.ApplyResources(this.tsbAddrFind, "tsbAddrFind");
		this.tsbAddrFind.Name = "tsbAddrFind";
		this.tsbAddrFind.Click += new System.EventHandler(actionEditSearch);
		this.tsbAddrFindPrev.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbAddrFindPrev.Image = PrgEditor.Properties.Resources.EditShiftU;
		resources.ApplyResources(this.tsbAddrFindPrev, "tsbAddrFindPrev");
		this.tsbAddrFindPrev.Name = "tsbAddrFindPrev";
		this.tsbAddrFindPrev.Click += new System.EventHandler(actionEditFind);
		this.tsbAddrFindNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbAddrFindNext.Image = PrgEditor.Properties.Resources.EditShiftD;
		resources.ApplyResources(this.tsbAddrFindNext, "tsbAddrFindNext");
		this.tsbAddrFindNext.Name = "tsbAddrFindNext";
		this.tsbAddrFindNext.Click += new System.EventHandler(actionEditFind);
		this.scrollPanelPrg.AllowDrop = true;
		resources.ApplyResources(this.scrollPanelPrg, "scrollPanelPrg");
		this.scrollPanelPrg.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.scrollPanelPrg.ClientAreaSize = new System.Drawing.Size(519, 485);
		this.scrollPanelPrg.Controls.Add(this.selectorPrg);
		this.scrollPanelPrg.LargeChange = 10;
		this.scrollPanelPrg.LrChange = 1;
		this.scrollPanelPrg.Maximum = 100;
		this.scrollPanelPrg.Minimum = 0;
		this.scrollPanelPrg.Name = "scrollPanelPrg";
		this.scrollPanelPrg.ScrollBarType = ControlLib.ScrollPanel.ScrollBarTypes.Vertical;
		this.scrollPanelPrg.SmallChange = 1;
		this.scrollPanelPrg.Value = 0;
		this.scrollPanelPrg.WheelRate = 4;
		this.scrollPanelPrg.Scrolled += new System.EventHandler(scrollPanelPrg_Scrolled);
		this.scrollPanelPrg.SizeChanged += new System.EventHandler(scrollPanelPrg_SizeChanged);
		this.scrollPanelPrg.DragDrop += new System.Windows.Forms.DragEventHandler(ActionDragDrop);
		this.scrollPanelPrg.DragEnter += new System.Windows.Forms.DragEventHandler(ActionDragEnter);
		this.selectorPrg.DefaultSelectSize = new System.Drawing.Size(8, 8);
		this.selectorPrg.EnableRightDragSelect = true;
		this.selectorPrg.FreeSelect = false;
		this.selectorPrg.GridColor1 = System.Drawing.Color.White;
		this.selectorPrg.GridColor2 = System.Drawing.Color.Gray;
		this.selectorPrg.GridStyle = ControlLib.GridStyle.Dot;
		this.selectorPrg.Image = null;
		resources.ApplyResources(this.selectorPrg, "selectorPrg");
		this.selectorPrg.MouseDownNew = true;
		this.selectorPrg.Name = "selectorPrg";
		this.selectorPrg.PixelSelect = false;
		this.selectorPrg.SelectedColor1 = System.Drawing.Color.Aqua;
		this.selectorPrg.SelectedColor2 = System.Drawing.Color.Yellow;
		this.selectorPrg.SelectedIndex = 0;
		this.selectorPrg.SelectedRect = new System.Drawing.Rectangle(0, 0, 8, 8);
		this.selectorPrg.TabStop = false;
		this.selectorPrg.ZoomRate = 2;
		this.selectorPrg.Selected += new System.Windows.Forms.MouseEventHandler(selectorPrg_Selected);
		this.selectorPrg.SizeChanged += new System.EventHandler(selectorPrg_SizeChanged);
		this.panelRight.AllowDrop = true;
		this.panelRight.Controls.Add(this.toolStripChr);
		this.panelRight.Controls.Add(this.paletteSelectorNes);
		this.panelRight.Controls.Add(this.paletteSelectorSet);
		this.panelRight.Controls.Add(this.selectorChr);
		resources.ApplyResources(this.panelRight, "panelRight");
		this.panelRight.Name = "panelRight";
		this.panelRight.DragDrop += new System.Windows.Forms.DragEventHandler(ActionDragDrop);
		this.panelRight.DragEnter += new System.Windows.Forms.DragEventHandler(ActionDragEnter);
		resources.ApplyResources(this.toolStripChr, "toolStripChr");
		this.toolStripChr.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		this.toolStripChr.Items.AddRange(new System.Windows.Forms.ToolStripItem[8] { this.tsChrSpace1, this.tsbViewGridChr2, this.toolStripSeparator8, this.tsbViewOpenChr2, this.tsChrSpace2, this.tsbViewOpenState2, this.tsbViewFullPaletteMode, this.tsbViewImagePaletteMode });
		this.toolStripChr.Name = "toolStripChr";
		this.toolStripChr.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		this.toolStripChr.Stretch = true;
		resources.ApplyResources(this.tsChrSpace1, "tsChrSpace1");
		this.tsChrSpace1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
		this.tsChrSpace1.Name = "tsChrSpace1";
		this.tsbViewGridChr2.Checked = true;
		this.tsbViewGridChr2.CheckOnClick = true;
		this.tsbViewGridChr2.CheckState = System.Windows.Forms.CheckState.Checked;
		this.tsbViewGridChr2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbViewGridChr2.Image = PrgEditor.Properties.Resources.GridChr;
		resources.ApplyResources(this.tsbViewGridChr2, "tsbViewGridChr2");
		this.tsbViewGridChr2.Name = "tsbViewGridChr2";
		this.tsbViewGridChr2.Click += new System.EventHandler(actionViewGridChr);
		this.toolStripSeparator8.Name = "toolStripSeparator8";
		resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
		this.tsbViewOpenChr2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbViewOpenChr2.Image = PrgEditor.Properties.Resources.FileOpenChr;
		resources.ApplyResources(this.tsbViewOpenChr2, "tsbViewOpenChr2");
		this.tsbViewOpenChr2.Name = "tsbViewOpenChr2";
		this.tsbViewOpenChr2.Click += new System.EventHandler(actionViewOpenChr);
		resources.ApplyResources(this.tsChrSpace2, "tsChrSpace2");
		this.tsChrSpace2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
		this.tsChrSpace2.Name = "tsChrSpace2";
		this.tsbViewOpenState2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbViewOpenState2.Image = PrgEditor.Properties.Resources.PaletteOpenState;
		resources.ApplyResources(this.tsbViewOpenState2, "tsbViewOpenState2");
		this.tsbViewOpenState2.Name = "tsbViewOpenState2";
		this.tsbViewOpenState2.Click += new System.EventHandler(actionViewOpenPaletteFromState);
		this.tsbViewFullPaletteMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbViewFullPaletteMode.Image = PrgEditor.Properties.Resources.PaletteTypePalTable;
		resources.ApplyResources(this.tsbViewFullPaletteMode, "tsbViewFullPaletteMode");
		this.tsbViewFullPaletteMode.Name = "tsbViewFullPaletteMode";
		this.tsbViewFullPaletteMode.Click += new System.EventHandler(actionViewFullPaletteMode);
		this.tsbViewImagePaletteMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbViewImagePaletteMode.Image = PrgEditor.Properties.Resources.PaletteTypeBmp;
		resources.ApplyResources(this.tsbViewImagePaletteMode, "tsbViewImagePaletteMode");
		this.tsbViewImagePaletteMode.Name = "tsbViewImagePaletteMode";
		this.tsbViewImagePaletteMode.Click += new System.EventHandler(actionViewImagePaletteMode);
		this.paletteSelectorNes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.paletteSelectorNes.CellColumnCount = 16;
		this.paletteSelectorNes.CellColumnView = 16;
		this.paletteSelectorNes.CellHeight = 16;
		this.paletteSelectorNes.CellRowCount = 4;
		this.paletteSelectorNes.CellRowView = 4;
		this.paletteSelectorNes.CellWidth = 16;
		this.paletteSelectorNes.ColorCount = 256;
		this.paletteSelectorNes.DrawMultiSelect = false;
		this.paletteSelectorNes.EnableMultiSelect = false;
		this.paletteSelectorNes.EnableSelectM = false;
		this.paletteSelectorNes.EnableSelectR = true;
		this.paletteSelectorNes.EnableSetUnknownMarkR = false;
		resources.ApplyResources(this.paletteSelectorNes, "paletteSelectorNes");
		this.paletteSelectorNes.Label = new string[256];
		this.paletteSelectorNes.LabelItem = ControlLib.LabelItem.Index;
		this.paletteSelectorNes.LabelStyle = ControlLib.LabelStyle.SelectedAll;
		this.paletteSelectorNes.Name = "paletteSelectorNes";
		this.paletteSelectorNes.Palette = new System.Drawing.Color[256]
		{
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
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
		this.paletteSelectorNes.PaletteFlags = new byte[256];
		this.paletteSelectorNes.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.paletteSelectorNes.SelectedIndex = 0;
		this.paletteSelectorNes.SelectEndIndex = 1;
		this.paletteSelectorNes.SetSize = 0;
		this.paletteSelectorNes.ShowSetRect = true;
		this.paletteSelectorNes.OnPaletteSelect += new ControlLib.PaletteSelector.OnPaletteSelectHandler(paletteSelectorNes_OnPaletteSelect);
		this.paletteSelectorSet.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.paletteSelectorSet.CellColumnCount = 16;
		this.paletteSelectorSet.CellColumnView = 16;
		this.paletteSelectorSet.CellHeight = 16;
		this.paletteSelectorSet.CellRowCount = 2;
		this.paletteSelectorSet.CellRowView = 2;
		this.paletteSelectorSet.CellWidth = 16;
		this.paletteSelectorSet.ColorCount = 256;
		this.paletteSelectorSet.DrawMultiSelect = false;
		this.paletteSelectorSet.EnableMultiSelect = false;
		this.paletteSelectorSet.EnableSelectM = false;
		this.paletteSelectorSet.EnableSelectR = true;
		this.paletteSelectorSet.EnableSetUnknownMarkR = false;
		resources.ApplyResources(this.paletteSelectorSet, "paletteSelectorSet");
		this.paletteSelectorSet.Label = new string[256];
		this.paletteSelectorSet.LabelItem = ControlLib.LabelItem.LabelsProperty;
		this.paletteSelectorSet.LabelStyle = ControlLib.LabelStyle.SelectedAll;
		this.paletteSelectorSet.Name = "paletteSelectorSet";
		this.paletteSelectorSet.Palette = new System.Drawing.Color[256]
		{
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
			System.Drawing.Color.Empty,
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
		this.paletteSelectorSet.PaletteFlags = new byte[256];
		this.paletteSelectorSet.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.paletteSelectorSet.SelectedIndex = 0;
		this.paletteSelectorSet.SelectEndIndex = 1;
		this.paletteSelectorSet.SetSize = 4;
		this.paletteSelectorSet.ShowSetRect = true;
		this.paletteSelectorSet.OnPaletteSelect += new ControlLib.PaletteSelector.OnPaletteSelectHandler(paletteSelectorSet_OnPaletteSelect);
		this.paletteSelectorSet.OnPaletteSetChanged += new ControlLib.PaletteSelector.OnPaletteSetChangedHandler(paletteSelectorSet_OnPaletteSetChanged);
		this.selectorChr.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.selectorChr.DefaultSelectSize = new System.Drawing.Size(8, 8);
		this.selectorChr.EnableRightDragSelect = true;
		this.selectorChr.FreeSelect = false;
		this.selectorChr.GridColor1 = System.Drawing.Color.White;
		this.selectorChr.GridColor2 = System.Drawing.Color.Gray;
		this.selectorChr.GridStyle = ControlLib.GridStyle.Dot;
		this.selectorChr.Image = null;
		resources.ApplyResources(this.selectorChr, "selectorChr");
		this.selectorChr.MouseDownNew = true;
		this.selectorChr.Name = "selectorChr";
		this.selectorChr.PixelSelect = false;
		this.selectorChr.SelectedColor1 = System.Drawing.Color.Aqua;
		this.selectorChr.SelectedColor2 = System.Drawing.Color.Yellow;
		this.selectorChr.SelectedIndex = 0;
		this.selectorChr.SelectedRect = new System.Drawing.Rectangle(0, 0, 8, 8);
		this.selectorChr.TabStop = false;
		this.selectorChr.ZoomRate = 2;
		this.selectorChr.Selected += new System.Windows.Forms.MouseEventHandler(selectorChr_Selected);
		this.selectorChr.MouseDown += new System.Windows.Forms.MouseEventHandler(selectorChr_MouseDown);
		resources.ApplyResources(this.statusStrip1, "statusStrip1");
		this.statusStrip1.GripMargin = new System.Windows.Forms.Padding(0);
		this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[6] { this.toolStripStatusLabelView, this.toolStripStatusLabelAddrValue, this.toolStripStatusLabelWH, this.toolStripStatusLabelSelected, this.toolStripStatusLabelXY, this.toolStripStatusLabelCarretAddrValue });
		this.statusStrip1.Name = "statusStrip1";
		this.toolStripStatusLabelView.Name = "toolStripStatusLabelView";
		resources.ApplyResources(this.toolStripStatusLabelView, "toolStripStatusLabelView");
		resources.ApplyResources(this.toolStripStatusLabelAddrValue, "toolStripStatusLabelAddrValue");
		this.toolStripStatusLabelAddrValue.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.All;
		this.toolStripStatusLabelAddrValue.Margin = new System.Windows.Forms.Padding(0, 3, 1, 1);
		this.toolStripStatusLabelAddrValue.Name = "toolStripStatusLabelAddrValue";
		resources.ApplyResources(this.toolStripStatusLabelWH, "toolStripStatusLabelWH");
		this.toolStripStatusLabelWH.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.All;
		this.toolStripStatusLabelWH.Margin = new System.Windows.Forms.Padding(0, 3, 1, 1);
		this.toolStripStatusLabelWH.Name = "toolStripStatusLabelWH";
		this.toolStripStatusLabelSelected.Name = "toolStripStatusLabelSelected";
		resources.ApplyResources(this.toolStripStatusLabelSelected, "toolStripStatusLabelSelected");
		resources.ApplyResources(this.toolStripStatusLabelXY, "toolStripStatusLabelXY");
		this.toolStripStatusLabelXY.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.All;
		this.toolStripStatusLabelXY.Margin = new System.Windows.Forms.Padding(0, 3, 1, 1);
		this.toolStripStatusLabelXY.Name = "toolStripStatusLabelXY";
		resources.ApplyResources(this.toolStripStatusLabelCarretAddrValue, "toolStripStatusLabelCarretAddrValue");
		this.toolStripStatusLabelCarretAddrValue.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.All;
		this.toolStripStatusLabelCarretAddrValue.Margin = new System.Windows.Forms.Padding(0, 3, 1, 1);
		this.toolStripStatusLabelCarretAddrValue.Name = "toolStripStatusLabelCarretAddrValue";
		resources.ApplyResources(this.toolStrip1, "toolStrip1");
		this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[27]
		{
			this.toolStripLabel1, this.tsbFileOpen1, this.tsbFileSave, this.toolStripSeparator1, this.tsbViewOpenChr1, this.toolStripSeparator5, this.tsbViewOpenState1, this.toolStripSeparator2, this.tsbResizeHDec, this.tsbResizeHInc,
			this.tsbResizeWDec, this.tsbResizeWInc, this.toolStripSeparator11, this.tsbViewGridPrg1, this.tsbViewGridChr1, this.toolStripSeparator3, this.tsbEditModeStep, this.tsbEditModeTypeText, this.tsbEditModeDrawPict, this.toolStripSeparator4,
			this.tsbMode8x16, this.tsbMode16x16, this.tsbYxSwap, this.tsbAddrFindNext2, this.tsbAddrFindPrev2, this.tsbAddrSearch2, this.tbSkip1
		});
		this.toolStrip1.Name = "toolStrip1";
		this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		this.toolStrip1.Stretch = true;
		resources.ApplyResources(this.toolStripLabel1, "toolStripLabel1");
		this.toolStripLabel1.Name = "toolStripLabel1";
		this.tsbFileOpen1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbFileOpen1.Image = PrgEditor.Properties.Resources.FileOpenPrg;
		resources.ApplyResources(this.tsbFileOpen1, "tsbFileOpen1");
		this.tsbFileOpen1.Name = "tsbFileOpen1";
		this.tsbFileOpen1.Click += new System.EventHandler(actionFileOpen);
		this.tsbFileSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbFileSave.Image = PrgEditor.Properties.Resources.FileSave;
		resources.ApplyResources(this.tsbFileSave, "tsbFileSave");
		this.tsbFileSave.Name = "tsbFileSave";
		this.tsbFileSave.Click += new System.EventHandler(actionFileSave);
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
		this.tsbViewOpenChr1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbViewOpenChr1.Image = PrgEditor.Properties.Resources.FileOpenChr;
		resources.ApplyResources(this.tsbViewOpenChr1, "tsbViewOpenChr1");
		this.tsbViewOpenChr1.Name = "tsbViewOpenChr1";
		this.tsbViewOpenChr1.Click += new System.EventHandler(actionViewOpenChr);
		this.toolStripSeparator5.Name = "toolStripSeparator5";
		resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
		this.tsbViewOpenState1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbViewOpenState1.Image = PrgEditor.Properties.Resources.PaletteOpenState;
		resources.ApplyResources(this.tsbViewOpenState1, "tsbViewOpenState1");
		this.tsbViewOpenState1.Name = "tsbViewOpenState1";
		this.tsbViewOpenState1.Click += new System.EventHandler(actionViewOpenPaletteFromState);
		this.toolStripSeparator2.Name = "toolStripSeparator2";
		resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
		this.tsbResizeHDec.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbResizeHDec.Image = PrgEditor.Properties.Resources.EditShiftU;
		resources.ApplyResources(this.tsbResizeHDec, "tsbResizeHDec");
		this.tsbResizeHDec.Name = "tsbResizeHDec";
		this.tsbResizeHDec.Click += new System.EventHandler(actionResize);
		this.tsbResizeHInc.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbResizeHInc.Image = PrgEditor.Properties.Resources.EditShiftD;
		resources.ApplyResources(this.tsbResizeHInc, "tsbResizeHInc");
		this.tsbResizeHInc.Name = "tsbResizeHInc";
		this.tsbResizeHInc.Click += new System.EventHandler(actionResize);
		this.tsbResizeWDec.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbResizeWDec.Image = PrgEditor.Properties.Resources.EditShiftL;
		resources.ApplyResources(this.tsbResizeWDec, "tsbResizeWDec");
		this.tsbResizeWDec.Name = "tsbResizeWDec";
		this.tsbResizeWDec.Click += new System.EventHandler(actionResize);
		this.tsbResizeWInc.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbResizeWInc.Image = PrgEditor.Properties.Resources.EditShiftR;
		resources.ApplyResources(this.tsbResizeWInc, "tsbResizeWInc");
		this.tsbResizeWInc.Name = "tsbResizeWInc";
		this.tsbResizeWInc.Click += new System.EventHandler(actionResize);
		this.toolStripSeparator11.Name = "toolStripSeparator11";
		resources.ApplyResources(this.toolStripSeparator11, "toolStripSeparator11");
		this.tsbViewGridPrg1.Checked = true;
		this.tsbViewGridPrg1.CheckOnClick = true;
		this.tsbViewGridPrg1.CheckState = System.Windows.Forms.CheckState.Checked;
		this.tsbViewGridPrg1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbViewGridPrg1.Image = PrgEditor.Properties.Resources.GridEdit;
		resources.ApplyResources(this.tsbViewGridPrg1, "tsbViewGridPrg1");
		this.tsbViewGridPrg1.Name = "tsbViewGridPrg1";
		this.tsbViewGridPrg1.Click += new System.EventHandler(actionViewGridPrg);
		this.tsbViewGridChr1.Checked = true;
		this.tsbViewGridChr1.CheckOnClick = true;
		this.tsbViewGridChr1.CheckState = System.Windows.Forms.CheckState.Checked;
		this.tsbViewGridChr1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbViewGridChr1.Image = PrgEditor.Properties.Resources.GridChr;
		resources.ApplyResources(this.tsbViewGridChr1, "tsbViewGridChr1");
		this.tsbViewGridChr1.Name = "tsbViewGridChr1";
		this.tsbViewGridChr1.Click += new System.EventHandler(actionViewGridChr);
		this.toolStripSeparator3.Name = "toolStripSeparator3";
		resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
		this.tsbEditModeStep.Checked = true;
		this.tsbEditModeStep.CheckOnClick = true;
		this.tsbEditModeStep.CheckState = System.Windows.Forms.CheckState.Checked;
		this.tsbEditModeStep.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbEditModeStep.Image = PrgEditor.Properties.Resources.EditMode0;
		resources.ApplyResources(this.tsbEditModeStep, "tsbEditModeStep");
		this.tsbEditModeStep.Name = "tsbEditModeStep";
		this.tsbEditModeStep.Click += new System.EventHandler(actionEditMode);
		this.tsbEditModeTypeText.CheckOnClick = true;
		this.tsbEditModeTypeText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbEditModeTypeText.Image = PrgEditor.Properties.Resources.EditMode1;
		resources.ApplyResources(this.tsbEditModeTypeText, "tsbEditModeTypeText");
		this.tsbEditModeTypeText.Name = "tsbEditModeTypeText";
		this.tsbEditModeTypeText.Click += new System.EventHandler(actionEditMode);
		this.tsbEditModeDrawPict.CheckOnClick = true;
		this.tsbEditModeDrawPict.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbEditModeDrawPict.Image = PrgEditor.Properties.Resources.EditMode2;
		resources.ApplyResources(this.tsbEditModeDrawPict, "tsbEditModeDrawPict");
		this.tsbEditModeDrawPict.Name = "tsbEditModeDrawPict";
		this.tsbEditModeDrawPict.Click += new System.EventHandler(actionEditMode);
		this.toolStripSeparator4.Name = "toolStripSeparator4";
		resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
		this.tsbMode8x16.CheckOnClick = true;
		this.tsbMode8x16.Image = PrgEditor.Properties.Resources.TbPrgOrder8x16;
		resources.ApplyResources(this.tsbMode8x16, "tsbMode8x16");
		this.tsbMode8x16.Name = "tsbMode8x16";
		this.tsbMode8x16.Click += new System.EventHandler(actionMiscMode8x16);
		this.tsbMode16x16.Image = PrgEditor.Properties.Resources.TbPrgOrder16x16;
		resources.ApplyResources(this.tsbMode16x16, "tsbMode16x16");
		this.tsbMode16x16.Name = "tsbMode16x16";
		this.tsbMode16x16.Click += new System.EventHandler(actionMiscMode16x16);
		this.tsbYxSwap.Image = PrgEditor.Properties.Resources.TbPrgOrderHV;
		resources.ApplyResources(this.tsbYxSwap, "tsbYxSwap");
		this.tsbYxSwap.Name = "tsbYxSwap";
		this.tsbYxSwap.Click += new System.EventHandler(actionMiscYxSwap);
		this.tsbAddrFindNext2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
		this.tsbAddrFindNext2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbAddrFindNext2.Image = PrgEditor.Properties.Resources.EditShiftD;
		resources.ApplyResources(this.tsbAddrFindNext2, "tsbAddrFindNext2");
		this.tsbAddrFindNext2.Name = "tsbAddrFindNext2";
		this.tsbAddrFindNext2.Click += new System.EventHandler(actionEditFind);
		this.tsbAddrFindPrev2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
		this.tsbAddrFindPrev2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbAddrFindPrev2.Image = PrgEditor.Properties.Resources.EditShiftU;
		resources.ApplyResources(this.tsbAddrFindPrev2, "tsbAddrFindPrev2");
		this.tsbAddrFindPrev2.Name = "tsbAddrFindPrev2";
		this.tsbAddrFindPrev2.Click += new System.EventHandler(actionEditFind);
		this.tsbAddrSearch2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
		this.tsbAddrSearch2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbAddrSearch2.Image = PrgEditor.Properties.Resources.AddrFind;
		resources.ApplyResources(this.tsbAddrSearch2, "tsbAddrSearch2");
		this.tsbAddrSearch2.Name = "tsbAddrSearch2";
		this.tsbAddrSearch2.Click += new System.EventHandler(actionEditSearch);
		this.tbSkip1.CheckOnClick = true;
		this.tbSkip1.Image = PrgEditor.Properties.Resources.TbPrgOrderSkip1b;
		resources.ApplyResources(this.tbSkip1, "tbSkip1");
		this.tbSkip1.Name = "tbSkip1";
		this.tbSkip1.Click += new System.EventHandler(actionMiscSkip1);
		this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.miFile, this.miEdit, this.miView, this.miOption, this.miHelp });
		resources.ApplyResources(this.menuStrip1, "menuStrip1");
		this.menuStrip1.Name = "menuStrip1";
		this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		this.miFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[8] { this.miFileOpen, this.miFileReload, this.miFileSave, this.miFileSaveAs, this.toolStripMenuItem3, this.miFileSavePrgBitmap, this.toolStripMenuItem5, this.miFileExit });
		this.miFile.Name = "miFile";
		resources.ApplyResources(this.miFile, "miFile");
		this.miFileOpen.Image = PrgEditor.Properties.Resources.FileOpenPrg;
		resources.ApplyResources(this.miFileOpen, "miFileOpen");
		this.miFileOpen.Name = "miFileOpen";
		this.miFileOpen.Click += new System.EventHandler(actionFileOpen);
		this.miFileReload.Name = "miFileReload";
		resources.ApplyResources(this.miFileReload, "miFileReload");
		this.miFileReload.Click += new System.EventHandler(actionFileReload);
		this.miFileSave.Image = PrgEditor.Properties.Resources.FileSave;
		resources.ApplyResources(this.miFileSave, "miFileSave");
		this.miFileSave.Name = "miFileSave";
		this.miFileSave.Click += new System.EventHandler(actionFileSave);
		this.miFileSaveAs.Image = PrgEditor.Properties.Resources.FileSave;
		resources.ApplyResources(this.miFileSaveAs, "miFileSaveAs");
		this.miFileSaveAs.Name = "miFileSaveAs";
		this.miFileSaveAs.Click += new System.EventHandler(actionFileSaveAs);
		this.toolStripMenuItem3.Name = "toolStripMenuItem3";
		resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
		this.miFileSavePrgBitmap.Image = PrgEditor.Properties.Resources.FileSave;
		resources.ApplyResources(this.miFileSavePrgBitmap, "miFileSavePrgBitmap");
		this.miFileSavePrgBitmap.Name = "miFileSavePrgBitmap";
		this.miFileSavePrgBitmap.Click += new System.EventHandler(actionFileSavePrgBitmap);
		this.toolStripMenuItem5.Name = "toolStripMenuItem5";
		resources.ApplyResources(this.toolStripMenuItem5, "toolStripMenuItem5");
		this.miFileExit.Image = PrgEditor.Properties.Resources.FileExit;
		resources.ApplyResources(this.miFileExit, "miFileExit");
		this.miFileExit.Name = "miFileExit";
		this.miFileExit.Click += new System.EventHandler(actionFileExit);
		this.miEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.miEditMode, this.toolStripMenuItem6, this.miEditSearch, this.miEditFindPrevious, this.miEditFindNext });
		this.miEdit.Name = "miEdit";
		resources.ApplyResources(this.miEdit, "miEdit");
		this.miEditMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.miEditModeStepByStep, this.miEditModeTypeText, this.miEditModeDrawPicture });
		this.miEditMode.Name = "miEditMode";
		resources.ApplyResources(this.miEditMode, "miEditMode");
		this.miEditModeStepByStep.Checked = true;
		this.miEditModeStepByStep.CheckOnClick = true;
		this.miEditModeStepByStep.CheckState = System.Windows.Forms.CheckState.Checked;
		this.miEditModeStepByStep.Image = PrgEditor.Properties.Resources.EditMode0;
		resources.ApplyResources(this.miEditModeStepByStep, "miEditModeStepByStep");
		this.miEditModeStepByStep.Name = "miEditModeStepByStep";
		this.miEditModeStepByStep.Click += new System.EventHandler(actionEditMode);
		this.miEditModeTypeText.CheckOnClick = true;
		this.miEditModeTypeText.Image = PrgEditor.Properties.Resources.EditMode1;
		resources.ApplyResources(this.miEditModeTypeText, "miEditModeTypeText");
		this.miEditModeTypeText.Name = "miEditModeTypeText";
		this.miEditModeTypeText.Click += new System.EventHandler(actionEditMode);
		this.miEditModeDrawPicture.CheckOnClick = true;
		this.miEditModeDrawPicture.Image = PrgEditor.Properties.Resources.EditMode2;
		resources.ApplyResources(this.miEditModeDrawPicture, "miEditModeDrawPicture");
		this.miEditModeDrawPicture.Name = "miEditModeDrawPicture";
		this.miEditModeDrawPicture.Click += new System.EventHandler(actionEditMode);
		this.toolStripMenuItem6.Name = "toolStripMenuItem6";
		resources.ApplyResources(this.toolStripMenuItem6, "toolStripMenuItem6");
		this.miEditSearch.Image = PrgEditor.Properties.Resources.AddrFind;
		resources.ApplyResources(this.miEditSearch, "miEditSearch");
		this.miEditSearch.Name = "miEditSearch";
		this.miEditSearch.Click += new System.EventHandler(actionEditSearch);
		this.miEditFindPrevious.Image = PrgEditor.Properties.Resources.EditShiftU;
		resources.ApplyResources(this.miEditFindPrevious, "miEditFindPrevious");
		this.miEditFindPrevious.Name = "miEditFindPrevious";
		this.miEditFindPrevious.Click += new System.EventHandler(actionEditFind);
		this.miEditFindNext.Image = PrgEditor.Properties.Resources.EditShiftD;
		resources.ApplyResources(this.miEditFindNext, "miEditFindNext");
		this.miEditFindNext.Name = "miEditFindNext";
		this.miEditFindNext.Click += new System.EventHandler(actionEditFind);
		this.miView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[8] { this.miViewGridPrg, this.miViewGridChr, this.toolStripMenuItem2, this.miViewOpenChr, this.toolStripMenuItem1, this.miViewOpenState, this.miViewFullPaletteMode, this.miViewImagePaletteMode });
		this.miView.Name = "miView";
		resources.ApplyResources(this.miView, "miView");
		this.miViewGridPrg.Checked = true;
		this.miViewGridPrg.CheckOnClick = true;
		this.miViewGridPrg.CheckState = System.Windows.Forms.CheckState.Checked;
		this.miViewGridPrg.Image = PrgEditor.Properties.Resources.GridEdit;
		resources.ApplyResources(this.miViewGridPrg, "miViewGridPrg");
		this.miViewGridPrg.Name = "miViewGridPrg";
		this.miViewGridPrg.Click += new System.EventHandler(actionViewGridPrg);
		this.miViewGridChr.Checked = true;
		this.miViewGridChr.CheckOnClick = true;
		this.miViewGridChr.CheckState = System.Windows.Forms.CheckState.Checked;
		this.miViewGridChr.Image = PrgEditor.Properties.Resources.GridChr;
		resources.ApplyResources(this.miViewGridChr, "miViewGridChr");
		this.miViewGridChr.Name = "miViewGridChr";
		this.miViewGridChr.Click += new System.EventHandler(actionViewGridChr);
		this.toolStripMenuItem2.Name = "toolStripMenuItem2";
		resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
		this.miViewOpenChr.Image = PrgEditor.Properties.Resources.FileOpenChr;
		resources.ApplyResources(this.miViewOpenChr, "miViewOpenChr");
		this.miViewOpenChr.Name = "miViewOpenChr";
		this.miViewOpenChr.Click += new System.EventHandler(actionViewOpenChr);
		this.toolStripMenuItem1.Name = "toolStripMenuItem1";
		resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
		this.miViewOpenState.Image = PrgEditor.Properties.Resources.PaletteOpenState;
		resources.ApplyResources(this.miViewOpenState, "miViewOpenState");
		this.miViewOpenState.Name = "miViewOpenState";
		this.miViewOpenState.Click += new System.EventHandler(actionViewOpenPaletteFromState);
		this.miViewFullPaletteMode.CheckOnClick = true;
		this.miViewFullPaletteMode.Image = PrgEditor.Properties.Resources.PaletteTypePalTable;
		resources.ApplyResources(this.miViewFullPaletteMode, "miViewFullPaletteMode");
		this.miViewFullPaletteMode.Name = "miViewFullPaletteMode";
		this.miViewFullPaletteMode.Click += new System.EventHandler(actionViewFullPaletteMode);
		this.miViewImagePaletteMode.Image = PrgEditor.Properties.Resources.PaletteTypeBmp;
		resources.ApplyResources(this.miViewImagePaletteMode, "miViewImagePaletteMode");
		this.miViewImagePaletteMode.Name = "miViewImagePaletteMode";
		this.miViewImagePaletteMode.Click += new System.EventHandler(actionViewImagePaletteMode);
		this.miOption.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.miOptionConfiguration });
		this.miOption.Name = "miOption";
		resources.ApplyResources(this.miOption, "miOption");
		this.miOptionConfiguration.Name = "miOptionConfiguration";
		resources.ApplyResources(this.miOptionConfiguration, "miOptionConfiguration");
		this.miOptionConfiguration.Click += new System.EventHandler(actionOptionConfiguration);
		this.miHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.miHelpCheckUpdate, this.toolStripMenuItem4, this.miHelpPropertyEditor, this.toolStripMenuItem7, this.miHelpAbout });
		this.miHelp.Name = "miHelp";
		resources.ApplyResources(this.miHelp, "miHelp");
		this.miHelpCheckUpdate.Image = PrgEditor.Properties.Resources.ToolShortcut;
		resources.ApplyResources(this.miHelpCheckUpdate, "miHelpCheckUpdate");
		this.miHelpCheckUpdate.Name = "miHelpCheckUpdate";
		this.miHelpCheckUpdate.Click += new System.EventHandler(actionHelpCheckUpdate);
		this.toolStripMenuItem4.Name = "toolStripMenuItem4";
		resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
		this.miHelpPropertyEditor.Image = PrgEditor.Properties.Resources.IconControl;
		resources.ApplyResources(this.miHelpPropertyEditor, "miHelpPropertyEditor");
		this.miHelpPropertyEditor.Name = "miHelpPropertyEditor";
		this.miHelpPropertyEditor.Click += new System.EventHandler(actionHelpPropertyEditor);
		this.toolStripMenuItem7.Name = "toolStripMenuItem7";
		resources.ApplyResources(this.toolStripMenuItem7, "toolStripMenuItem7");
		this.miHelpAbout.Name = "miHelpAbout";
		resources.ApplyResources(this.miHelpAbout, "miHelpAbout");
		this.miHelpAbout.Click += new System.EventHandler(actionHelpAbout);
		this.openFileDialogPrg.DefaultExt = "nes";
		resources.ApplyResources(this.openFileDialogPrg, "openFileDialogPrg");
		this.saveFileDialogPrg.DefaultExt = "nes";
		resources.ApplyResources(this.saveFileDialogPrg, "saveFileDialogPrg");
		this.openFileDialogChr.DefaultExt = "bmp";
		resources.ApplyResources(this.openFileDialogChr, "openFileDialogChr");
		this.openFileDialogEtc.DefaultExt = "st*";
		resources.ApplyResources(this.openFileDialogEtc, "openFileDialogEtc");
		this.saveFileDialogBmp.DefaultExt = "bmp";
		resources.ApplyResources(this.saveFileDialogBmp, "saveFileDialogBmp");
		resources.ApplyResources(this.lVersion, "lVersion");
		this.lVersion.ForeColor = System.Drawing.Color.Red;
		this.lVersion.Name = "lVersion";
		this.AllowDrop = true;
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
		resources.ApplyResources(this, "$this");
		base.Controls.Add(this.lVersion);
		base.Controls.Add(this.tsContainer);
		base.Controls.Add(this.menuStrip1);
		this.DoubleBuffered = true;
		base.MainMenuStrip = this.menuStrip1;
		base.Name = "MainForm";
		base.Activated += new System.EventHandler(MainForm_Activated);
		this.tsContainer.ContentPanel.ResumeLayout(false);
		this.tsContainer.ContentPanel.PerformLayout();
		this.tsContainer.TopToolStripPanel.ResumeLayout(false);
		this.tsContainer.ResumeLayout(false);
		this.tsContainer.PerformLayout();
		this.panel2.ResumeLayout(false);
		this.toolStripPrgAddr.ResumeLayout(false);
		this.toolStripPrgAddr.PerformLayout();
		this.scrollPanelPrg.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.selectorPrg).EndInit();
		this.panelRight.ResumeLayout(false);
		this.toolStripChr.ResumeLayout(false);
		this.toolStripChr.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.selectorChr).EndInit();
		this.statusStrip1.ResumeLayout(false);
		this.statusStrip1.PerformLayout();
		this.toolStrip1.ResumeLayout(false);
		this.toolStrip1.PerformLayout();
		this.menuStrip1.ResumeLayout(false);
		this.menuStrip1.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
