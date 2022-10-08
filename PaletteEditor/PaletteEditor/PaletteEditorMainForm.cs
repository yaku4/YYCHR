using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CharactorLib;
using ControlLib;
using PaletteEditor.Properties;

namespace PaletteEditor;

public class PaletteEditorMainForm : Form
{
	private const string APP_NAME = "ROM Palette Editor";

	private const string APP_VER = "Ver.0.90 ";

	private const string URL_WEB = "https://www45.atwiki.jp/yychr/";

	private const string URL_WIKI = "https://www45.atwiki.jp/yychr/";

	private const string URL_BOARD = "https://jbbs.shitaraba.net/bbs/read.cgi/computer/41853/1231162374/l50";

	private const string URL_WIKI_HINT = "http://www45.atwiki.jp/yychr/?page=";

	private Settings mSettings = Settings.GetInstance();

	private UndoManager mUndoManager = new UndoManager();

	internal ColorBit[] mColorBitForIndexedEditorNes;

	internal ColorBit[] mColorBitForIndexedEditorMsx;

	internal ColorBit[] mColorBitForIndexedEditor;

	private static Font GuiFont = new Font("Marlett", 16f);

	private static string CheckText = "a";

	private string mOpenedFilename = "";

	private string mOpenedFileNameName = "";

	private ColorBit dmyColorBit = new ColorBit(PaletteType.R8G8B8, byte.MaxValue, 0, 0, 0);

	private bool mLastClipboardReady;

	private byte[] mFindData;

	private bool[] mFindDataEnabled;

	private PaletteType mPaletteType = PaletteType.IndexedNes;

	private PropertyEditorForm mPropertyEditorForm;

	private bool DisableTextBoxChanged;

	private IContainer components;

	private PaletteSelector paletteSelectorFile;

	private MenuStrip menuStrip;

	private ToolStripMenuItem miFile;

	private ToolStripMenuItem miFileOpen;

	private ToolStripMenuItem miFileSave;

	private ToolStripMenuItem miFileSaveAs;

	private ToolStripSeparator toolStripMenuItem1;

	private ToolStripMenuItem miFileExit;

	private ToolStripMenuItem miEdit;

	private ToolStripMenuItem miEditCopy;

	private ToolStripMenuItem miEditPaste;

	private ToolStripMenuItem miAddress;

	private ToolStripMenuItem miAddressBeginOfFile;

	private ToolStripMenuItem miAddressP1Page;

	private ToolStripMenuItem miAddressP1Line;

	private ToolStripMenuItem miAddressP1Color;

	private ToolStripMenuItem miAddressN1Color;

	private ToolStripMenuItem miAddressN1Line;

	private ToolStripMenuItem miAddressN1Page;

	private ToolStripMenuItem miAddressEndOfFile;

	private ToolStripSeparator toolStripMenuItem3;

	private ToolStripMenuItem miAddressInput;

	private ToolStripSeparator toolStripMenuItem4;

	private ToolStripMenuItem miAddressSettingFile;

	private ToolStripMenuItem miType;

	private ToolStripMenuItem miType6;

	private ToolStripMenuItem miType15;

	private ToolStripMenuItem miType16;

	private ToolStripMenuItem miType24;

	private ToolStripMenuItem miType32;

	private ToolStripMenuItem miOption;

	private ToolStripMenuItem settingToolStripMenuItem;

	private ToolStripMenuItem miHelp;

	private ToolStripMenuItem aboutToolStripMenuItem;

	private StatusStrip statusStrip;

	private ToolStrip toolStrip;

	private ToolStripButton tbFileOpen;

	private ToolStripButton tbFileSave;

	private ToolStripSeparator toolStripSeparator1;

	private ToolStripLabel tlType;

	private ToolStripSeparator toolStripSeparator2;

	private ScrollPanel scrollPanel;

	private OpenFileDialog openFileDialog;

	private SaveFileDialog saveFileDialog;

	private ToolStripMenuItem miAddressP1Byte;

	private ToolStripMenuItem miAddressN1Byte;

	private ToolStripSeparator toolStripMenuItem2;

	private ToolStripMenuItem miAddressFind;

	private ToolStripMenuItem miAddressFindPrev;

	private ToolStripMenuItem miAddressFindNext;

	private ToolStripMenuItem miFileReload;

	private ToolStripButton tbAddressP1Byte;

	private ToolStripButton tbAddressN1Byte;

	private ToolStripStatusLabel slAddress;

	private PaletteSelector paletteSelectorSourcePal;

	private Panel panelEditorIndex;

	private Panel panelEditorRGB;

	private Label labelHexValue;

	private TextBox textBoxHexBit;

	private PicturePanel panelPreview;

	private TextBox textBoxHex;

	private RGBEditor rgbEditor1;

	private ButtonNoFocus buttonReset;

	private ButtonNoFocus buttonOK;

	private Panel panelToolStripAddress;

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

	private ToolStripButton tbType6;

	private ToolStripButton tbType15;

	private ToolStripButton tbType16;

	private ToolStripButton tbType32;

	private ToolStripButton tbType24;

	private ToolStripButton tbAddressDetectPrev;

	private ToolStripButton tbAddressDetectNext;

	private ToolStripSeparator toolStripMenuItem5;

	private ToolStripMenuItem miAddressDetectPrev;

	private ToolStripMenuItem miAddressDetectNext;

	private ToolStripSeparator toolStripSeparator4;

	private ToolStripButton tbAddressDetectOnlyTable;

	private ToolStripMenuItem miAddressDetectOnlyTable;

	private ToolStripSeparator toolStripSeparator5;

	private ToolStripStatusLabel slHint;

	private ToolStripLabel tlDetect;

	private ToolStripMenuItem miFileReadonlyMode;

	private ToolStripSeparator toolStripMenuItem6;

	private ToolStripButton tbFileReadonlyMode;

	private ToolStripButton tbEditCopy;

	private ToolStripButton tbEditPaste;

	private ToolStripSeparator toolStripSeparator6;

	private PicturePanel panelAddress;

	private ToolStripMenuItem miOptionShowAddress;

	private ToolStripSeparator toolStripMenuItem7;

	private ToolStripSeparator toolStripMenuItem8;

	private ToolStripMenuItem miEditUndo;

	private ToolStripMenuItem miEditRedo;

	private ToolStripSeparator toolStripMenuItem9;

	private ToolStripButton tbEditUndo;

	private ToolStripButton tbEditRedo;

	private ToolStripSeparator toolStripSeparator7;

	private ToolStripButton tbAddressFind;

	private ToolStripButton tbAddressFindPrev;

	private ToolStripButton tbAddressFindNext;

	private Label lVersion;

	private ToolStripMenuItem miHelpPropertyEditor;

	private ToolStripSeparator toolStripMenuItem10;

	private ToolStripMenuItem miType9;

	private ToolStripButton tbType9;

	private ToolStripMenuItem miOptionCheckPAL;

	private ToolStripButton tbOptionCheckPAL;

	private ToolStripButton tbType4;

	private ToolStripMenuItem miType4;

	private byte[] mData { get; set; }

	private DateTime mOpenedFileDate { get; set; } = DateTime.MinValue;


	private bool mDataModified => mUndoManager.CanUndo;

	private ColorBit[] mViewColorBits { get; set; }

	private int mAddress { get; set; }

	private bool IsIndexedPalette
	{
		get
		{
			bool result = false;
			if (mPaletteType == PaletteType.Indexed)
			{
				result = true;
			}
			if (mPaletteType == PaletteType.IndexedNes)
			{
				result = true;
			}
			if (mPaletteType == PaletteType.IndexedMsx)
			{
				result = true;
			}
			return result;
		}
	}

	public PaletteEditorMainForm()
	{
		InitializeComponent();
		paletteSelectorFile.BorderStyle = BorderStyle.None;
		UpdateStatusbarAddress();
		InitPaletteTypeToolStripItem();
		mColorBitForIndexedEditorNes = LoadPaletteForIndexed("\\Resources\\nes.pal");
		mColorBitForIndexedEditorMsx = LoadPaletteForIndexed("\\Resources\\msx.pal");
		mPaletteType = PaletteType.IndexedNes;
		UpdateIndexedPaletteTable(mPaletteType);
		UpdateBuildDateLabel();
	}

	private void UpdateIndexedPaletteTable(PaletteType mPaletteType)
	{
		if (mPaletteType == PaletteType.IndexedMsx)
		{
			mColorBitForIndexedEditor = mColorBitForIndexedEditorMsx;
		}
		else
		{
			mColorBitForIndexedEditor = mColorBitForIndexedEditorNes;
		}
		UpdateColorFromColorBits(paletteSelectorSourcePal.Palette, mColorBitForIndexedEditor);
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!base.DesignMode)
		{
			mSettings.ReadOnlyMode = mSettings.ReadOnlyModeDefault;
			InitMenuCheckStateRenderer();
			UpdateControlsPaletteType();
			UpdateClipboardToControls();
			UpdateControlsAddress();
			UpdateUndoRedoControl();
			UpdateAllControls();
			UpdateControlSizeFromGuiZoomRate();
		}
	}

	private void InitMenuCheckStateRenderer()
	{
		try
		{
			if (!base.DesignMode && menuStrip.Renderer != null)
			{
				menuStrip.Renderer.RenderItemCheck += Render_RenderItemCheck;
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

	private void UpdateBuildDateLabel()
	{
		lVersion.Text = "build " + GetBuildDate();
		int num = base.ClientSize.Width - lVersion.Width;
		lVersion.Location = new Point(num, lVersion.Top);
	}

	private ColorBit[] LoadPaletteForIndexed(string palFilename)
	{
		ColorBit[] result = null;
		string path = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0].Replace(".vshost", "")) + palFilename;
		if (File.Exists(path))
		{
			result = ColorBit.FromDatas(File.ReadAllBytes(path), 0, PaletteType.R8G8B8, 256);
		}
		return result;
	}

	private void UpdateColorFromColorBits(Color[] palPalette, ColorBit[] colorBits)
	{
		for (int i = 0; i < palPalette.Length; i++)
		{
			if (colorBits != null && i < colorBits.Length)
			{
				palPalette[i] = colorBits[i].Color;
			}
			else
			{
				palPalette[i] = Color.Black;
			}
		}
	}

	private void PaletteEditorMainForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		e.Cancel = CheckSaveModifiedData();
	}

	private void PaletteEditorMainForm_FormClosed(object sender, FormClosedEventArgs e)
	{
		mSettings.Save();
	}

	private bool CheckSaveModifiedData()
	{
		bool result = false;
		if (mDataModified && mSettings.ShowSaveModifiedDialog)
		{
			string caption = "ROM Palette Editor";
			string confirmModifiedSave = Resources.ConfirmModifiedSave;
			DialogResult num = MsgBox.Show(this, confirmModifiedSave, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
			if (num == DialogResult.Cancel)
			{
				result = true;
			}
			if (num == DialogResult.Yes)
			{
				miFileSave.PerformClick();
			}
		}
		return result;
	}

	private bool CheckReloadModifiedData()
	{
		bool result = true;
		if (mDataModified)
		{
			string caption = "ROM Palette Editor";
			string confirmModifiedReload = Resources.ConfirmModifiedReload;
			if (MsgBox.Show(this, confirmModifiedReload, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
			{
				result = false;
			}
		}
		return result;
	}

	private void PaletteEditorMainForm_Activated(object sender, EventArgs ev)
	{
		bool flag = GetFileDate(mOpenedFilename) != mOpenedFileDate;
		try
		{
			base.Activated -= PaletteEditorMainForm_Activated;
			if (flag && mSettings.ShowReloadExternalChangeDialog)
			{
				string caption = "ROM Palette Editor";
				string confirmReloadChangedExternalChange = Resources.ConfirmReloadChangedExternalChange;
				if (MsgBox.Show(this, confirmReloadChangedExternalChange, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
				{
					miFileReload.PerformClick();
				}
			}
			UpdateAllControls();
		}
		catch
		{
		}
		finally
		{
			base.Activated += PaletteEditorMainForm_Activated;
		}
	}

	private void PaletteEditorMainForm_DragEnter(object sender, DragEventArgs e)
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

	private void PaletteEditorMainForm_DragDrop(object sender, DragEventArgs e)
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
		_ = new FileInfo(filename).Length;
		char[] trimChars = new char[3] { ' ', '\t', '.' };
		Path.GetExtension(filename).Trim(trimChars).ToLower();
		if (!CheckSaveModifiedData())
		{
			OpenFile(filename);
			mSettings.ReadOnlyMode = mSettings.ReadOnlyModeDefault;
			UpdateReadonlyMode();
			UpdateUndoRedoControl();
		}
	}

	private void miFileOpen_Click(object sender, EventArgs e)
	{
		if (!CheckSaveModifiedData())
		{
			string text = mOpenedFilename;
			if (!string.IsNullOrWhiteSpace(text))
			{
				openFileDialog.InitialDirectory = Path.GetDirectoryName(text);
				openFileDialog.FileName = Path.GetFileName(text);
			}
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				string fileName = openFileDialog.FileName;
				OpenFile(fileName);
				mSettings.ReadOnlyMode = mSettings.ReadOnlyModeDefault;
				UpdateReadonlyMode();
			}
			UpdatePaletteSelectorFile();
			SetEditorTarget();
			UpdateUndoRedoControl();
		}
	}

	private void miFileReload_Click(object sender, EventArgs e)
	{
		if (CheckReloadModifiedData())
		{
			OpenFile(mOpenedFilename);
			UpdatePaletteSelectorFile();
			SetEditorTarget();
			UpdateUndoRedoControl();
		}
	}

	private void miFileSave_Click(object sender, EventArgs e)
	{
		SaveFile(mOpenedFilename);
		UpdatePaletteSelectorFile();
	}

	private void miFileSaveAs_Click(object sender, EventArgs e)
	{
		string text = mOpenedFilename;
		if (!string.IsNullOrWhiteSpace(text))
		{
			saveFileDialog.InitialDirectory = Path.GetDirectoryName(text);
			saveFileDialog.FileName = Path.GetFileName(text);
		}
		if (saveFileDialog.ShowDialog() == DialogResult.OK)
		{
			string fileName = saveFileDialog.FileName;
			SaveFile(fileName);
			OpenFile(fileName);
		}
		UpdatePaletteSelectorFile();
		UpdateUndoRedoControl();
	}

	private void miFileExit_Click(object sender, EventArgs e)
	{
		CloseMainForm();
	}

	private void OpenFile(string filename)
	{
		if (File.Exists(filename))
		{
			try
			{
				mData = File.ReadAllBytes(filename);
			}
			catch
			{
				string messageFailedFileLoad = Resources.MessageFailedFileLoad;
				string titleError = Resources.TitleError;
				MsgBox.Show(this, messageFailedFileLoad, titleError, MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			mUndoManager.ClearUndoBuffer();
			mFindData = null;
			mFindDataEnabled = null;
			char[] trimChars = new char[3] { ' ', '\t', '.' };
			string extension = Path.GetExtension(filename);
			extension = extension.Trim(trimChars).ToLower();
			PaletteType paletteType = mPaletteType;
			if (extension == "nes")
			{
				paletteType = PaletteType.IndexedNes;
			}
			if (extension == "msx1")
			{
				paletteType = PaletteType.IndexedMsx;
			}
			if (extension == "msx2")
			{
				paletteType = PaletteType.R3G3B3;
			}
			if (extension == "sfc" || extension == "smc")
			{
				paletteType = PaletteType.R5G5B5;
			}
			if (mPaletteType != paletteType)
			{
				mPaletteType = paletteType;
				UpdateControlsPaletteType();
			}
			else
			{
				UpdateScrollBar();
			}
			mOpenedFilename = filename;
			mOpenedFileNameName = Path.GetFileName(filename);
			mOpenedFileDate = GetFileDate(filename);
		}
		else
		{
			string messageFileNotExist = Resources.MessageFileNotExist;
			string titleError2 = Resources.TitleError;
			MsgBox.Show(this, messageFileNotExist, titleError2, MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	public DateTime GetFileDate(string filename)
	{
		try
		{
			if (File.Exists(filename))
			{
				return File.GetLastWriteTime(filename);
			}
			return DateTime.MinValue;
		}
		catch
		{
			return DateTime.MinValue;
		}
	}

	private void SaveFile(string filename)
	{
		if (mData == null)
		{
			return;
		}
		try
		{
			File.WriteAllBytes(filename, mData);
			mOpenedFilename = filename;
			mOpenedFileNameName = Path.GetFileName(filename);
		}
		catch
		{
			string messageFailedFileSave = Resources.MessageFailedFileSave;
			string titleError = Resources.TitleError;
			MsgBox.Show(this, messageFailedFileSave, titleError, MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void tbFileReadonlyMode_Click(object sender, EventArgs e)
	{
		mSettings.ReadOnlyMode = !mSettings.ReadOnlyMode;
		UpdateReadonlyMode();
	}

	private void UpdateReadonlyMode()
	{
		bool readOnlyMode = mSettings.ReadOnlyMode;
		bool enabled = !readOnlyMode;
		paletteSelectorSourcePal.Enabled = enabled;
		textBoxHex.ReadOnly = readOnlyMode;
		rgbEditor1.Enabled = enabled;
		buttonOK.Enabled = enabled;
		buttonReset.Enabled = enabled;
		SetEditorTarget();
		ToolStripMenuItem toolStripMenuItem = miFileReadonlyMode;
		bool @checked = (tbFileReadonlyMode.Checked = readOnlyMode);
		toolStripMenuItem.Checked = @checked;
		if (!readOnlyMode)
		{
			Image image2 = (miFileReadonlyMode.Image = (tbFileReadonlyMode.Image = Resources.IconEditEnabled));
		}
		else
		{
			Image image2 = (miFileReadonlyMode.Image = (tbFileReadonlyMode.Image = Resources.IconEditDisabled));
		}
		UpdateClipboardToControls();
		UpdateStatusbarAddress();
	}

	private void CloseMainForm()
	{
		Close();
	}

	private void UpdateScrollBar()
	{
		if (mData != null)
		{
			int colorSizeFromSelectedPaletteType = GetColorSizeFromSelectedPaletteType();
			int num = colorSizeFromSelectedPaletteType * paletteSelectorFile.CellColumnView;
			int largeChange = num * paletteSelectorFile.CellRowView;
			scrollPanel.LargeChange = largeChange;
			scrollPanel.SmallChange = num;
			scrollPanel.LrChange = colorSizeFromSelectedPaletteType;
			scrollPanel.Minimum = 0;
			scrollPanel.Maximum = mData.Length - 1;
			UpdateControlsAddress();
		}
	}

	private void UpdatePaletteSelectorFile()
	{
		if (mData == null)
		{
			return;
		}
		byte colorSizeFromSelectedPaletteType = GetColorSizeFromSelectedPaletteType();
		int viewColorNum = GetViewColorNum();
		if (mViewColorBits == null || mViewColorBits.Length < viewColorNum)
		{
			mViewColorBits = new ColorBit[viewColorNum];
		}
		byte[] array = new byte[viewColorNum];
		if (IsIndexedPalette)
		{
			for (int i = 0; i < viewColorNum; i++)
			{
				int num = mAddress + i;
				if (num >= 0 && num < mData.Length)
				{
					byte b = mData[num];
					if (b >= 0 && b < mColorBitForIndexedEditor.Length)
					{
						mViewColorBits[i] = mColorBitForIndexedEditor[b];
					}
					else
					{
						mViewColorBits[i] = dmyColorBit;
					}
				}
			}
		}
		else
		{
			byte[] array2 = new byte[viewColorNum * colorSizeFromSelectedPaletteType];
			int num2 = 0;
			int num3 = mAddress;
			for (int j = 0; j < viewColorNum; j++)
			{
				for (int k = 0; k < colorSizeFromSelectedPaletteType; k++)
				{
					if (num2 >= 0 && num2 < array2.Length && num3 >= 0 && num3 < mData.Length)
					{
						array2[num2] = mData[num3];
					}
					num2++;
					num3++;
				}
			}
			mViewColorBits = ColorBit.FromDatas(array2, 0, mPaletteType, viewColorNum);
		}
		UpdateColorFromColorBits(paletteSelectorFile.Palette, mViewColorBits);
		if (mSettings.CheckPAL)
		{
			for (int l = 0; l < mViewColorBits.Length; l++)
			{
				array[l] = (byte)(mViewColorBits[l].IsInvalidPalette ? 1 : 0);
			}
		}
		paletteSelectorFile.PaletteFlags = array;
		paletteSelectorFile.Refresh();
		UpdateStatusbarAddress();
	}

	private byte GetNesCheckFlags(byte srcPal)
	{
		bool enableNesCheckXD = mSettings.EnableNesCheckXD;
		if (srcPal >= 64)
		{
			return 1;
		}
		if ((srcPal == 13 && !enableNesCheckXD) || srcPal == 14 || (srcPal == 29 && !enableNesCheckXD) || srcPal == 30 || srcPal == 31 || (srcPal == 45 && !enableNesCheckXD) || srcPal == 46 || srcPal == 47 || (srcPal == 61 && !enableNesCheckXD) || srcPal == 62 || srcPal == 63)
		{
			return 4;
		}
		return 0;
	}

	private byte GetMsx1CheckFlags(byte srcPal)
	{
		if (srcPal >= 16)
		{
			return 1;
		}
		return 0;
	}

	private int GetViewColorNum()
	{
		return paletteSelectorFile.CellRowView * paletteSelectorFile.CellRowView;
	}

	public void UpdateStatusbarAddress()
	{
		int num = mAddress;
		int num2 = num + GetViewColorNum() * GetColorSizeFromSelectedPaletteType();
		int num3 = 0;
		if (mData != null)
		{
			num3 = mData.Length;
		}
		string text = num.ToString("X6");
		string text2 = num2.ToString("X6");
		string text3 = num3.ToString("X6");
		slAddress.Text = "Address: " + text + " - " + text2 + " / " + text3;
		string text4 = "";
		if (mDataModified)
		{
			text4 = " *";
		}
		string text5 = "";
		if (mSettings.ReadOnlyMode)
		{
			text5 = " " + Resources.TitleReadOnly;
		}
		Text = "ROM Palette Editor (" + text + "/" + text3 + ") " + mOpenedFileNameName + text4 + text5;
	}

	private void miEditCopy_Click(object sender, EventArgs e)
	{
		if (mData == null)
		{
			return;
		}
		if (base.ActiveControl != null && base.ActiveControl is TextBox)
		{
			((TextBox)base.ActiveControl).Copy();
			return;
		}
		int colorSizeFromSelectedPaletteType = GetColorSizeFromSelectedPaletteType();
		int selectedStart = paletteSelectorFile.SelectedStart;
		int selectedEnd = paletteSelectorFile.SelectedEnd;
		int addressOfFilePaletteSelectorIndex = GetAddressOfFilePaletteSelectorIndex(selectedStart);
		int num = GetAddressOfFilePaletteSelectorIndex(selectedEnd) + (colorSizeFromSelectedPaletteType - 1) - addressOfFilePaletteSelectorIndex + 1;
		byte[] array = new byte[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = mData[addressOfFilePaletteSelectorIndex + i];
		}
		CopyBinaryToClipboard(array);
		UpdateClipboardToControls();
	}

	private void CopyBinaryToClipboard(byte[] data)
	{
		string text = "";
		if (data != null)
		{
			for (int i = 0; i < data.Length; i++)
			{
				text = text + data[i].ToString("X2") + " ";
			}
		}
		if (!string.IsNullOrWhiteSpace(text))
		{
			Clipboard.SetText(text);
		}
		else
		{
			SystemSounds.Beep.Play();
		}
	}

	private void miEditPaste_Click(object sender, EventArgs e)
	{
		if (mSettings.ReadOnlyMode || mData == null)
		{
			return;
		}
		if (base.ActiveControl != null && base.ActiveControl is TextBox)
		{
			((TextBox)base.ActiveControl).Paste();
			return;
		}
		byte[] binaryFromClipboard = GetBinaryFromClipboard();
		if (binaryFromClipboard != null)
		{
			SetDataToSelectedPaletteAddress(binaryFromClipboard, makeUndoBuffer: true);
		}
	}

	private byte[] GetBinaryFromClipboard()
	{
		byte[] result = null;
		string text = null;
		try
		{
			text = Clipboard.GetText();
		}
		catch
		{
		}
		if (!string.IsNullOrWhiteSpace(text))
		{
			List<byte> list = new List<byte>();
			string[] array = null;
			try
			{
				char[] separator = new char[1] { ' ' };
				array = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			}
			catch
			{
			}
			if (array != null)
			{
				foreach (string value in array)
				{
					byte b = 0;
					try
					{
						b = Convert.ToByte(value, 16);
						list.Add(b);
					}
					catch
					{
						list.Clear();
						break;
					}
				}
			}
			result = list.ToArray();
		}
		return result;
	}

	private void UpdateClipboardToControls()
	{
		bool flag = false;
		try
		{
			string value = Clipboard.GetText();
			if (!string.IsNullOrWhiteSpace(value))
			{
				flag = true;
			}
		}
		catch
		{
		}
		if (mSettings.ReadOnlyMode)
		{
			flag = false;
		}
		bool enabled;
		if (mLastClipboardReady != flag)
		{
			mLastClipboardReady = flag;
			ToolStripMenuItem toolStripMenuItem = miEditPaste;
			enabled = (tbEditPaste.Enabled = flag);
			toolStripMenuItem.Enabled = enabled;
		}
		bool flag3 = false;
		if (mData != null)
		{
			int addressOfSelectedIndex = GetAddressOfSelectedIndex();
			if (addressOfSelectedIndex >= 0 && addressOfSelectedIndex < mData.Length)
			{
				flag3 = true;
			}
		}
		ToolStripMenuItem toolStripMenuItem2 = miEditCopy;
		enabled = (tbEditCopy.Enabled = flag3);
		toolStripMenuItem2.Enabled = enabled;
	}

	private void miEditUndo_Click(object sender, EventArgs e)
	{
		mUndoManager.WriteUndoData(mData);
		UpdatePaletteSelectorFile();
		SetEditorTarget();
		UpdateUndoRedoControl();
	}

	private void miEditRedo_Click(object sender, EventArgs e)
	{
		mUndoManager.WriteRedoData(mData);
		UpdatePaletteSelectorFile();
		SetEditorTarget();
		UpdateUndoRedoControl();
	}

	private void UpdateUndoRedoControl()
	{
		ToolStripMenuItem toolStripMenuItem = miEditUndo;
		bool enabled = (tbEditUndo.Enabled = mUndoManager.CanUndo);
		toolStripMenuItem.Enabled = enabled;
		ToolStripMenuItem toolStripMenuItem2 = miEditRedo;
		enabled = (tbEditRedo.Enabled = mUndoManager.CanRedo);
		toolStripMenuItem2.Enabled = enabled;
	}

	private int GetAddressOfSelectedIndex()
	{
		return GetAddressOfFilePaletteSelectorIndex(paletteSelectorFile.SelectedIndex);
	}

	private int GetAddressOfFilePaletteSelectorIndex(int palIndex)
	{
		int colorSizeFromSelectedPaletteType = GetColorSizeFromSelectedPaletteType();
		return mAddress + palIndex * colorSizeFromSelectedPaletteType;
	}

	private void miAddressInput_Click(object sender, EventArgs e)
	{
		AddressInput();
	}

	private void miAddress_Click(object sender, EventArgs e)
	{
		if (mData != null)
		{
			byte colorSizeFromSelectedPaletteType = GetColorSizeFromSelectedPaletteType();
			int cellColumnView = paletteSelectorFile.CellColumnView;
			int cellRowView = paletteSelectorFile.CellRowView;
			int num = mAddress;
			if (sender == miAddressBeginOfFile || sender == tbAddres0)
			{
				num = 0;
			}
			if (sender == miAddressP1Page || sender == tbAddres1)
			{
				num -= colorSizeFromSelectedPaletteType * cellColumnView * cellRowView;
			}
			if (sender == miAddressP1Line || sender == tbAddres2)
			{
				num -= colorSizeFromSelectedPaletteType * cellColumnView;
			}
			if (sender == miAddressP1Color || sender == tbAddres3)
			{
				num -= colorSizeFromSelectedPaletteType;
			}
			if (sender == miAddressP1Byte || sender == tbAddres4 || sender == tbAddressP1Byte)
			{
				num--;
			}
			if (sender == miAddressN1Byte || sender == tbAddres5 || sender == tbAddressN1Byte)
			{
				num++;
			}
			if (sender == miAddressN1Color || sender == tbAddres6)
			{
				num += colorSizeFromSelectedPaletteType;
			}
			if (sender == miAddressN1Line || sender == tbAddres7)
			{
				num += colorSizeFromSelectedPaletteType * cellColumnView;
			}
			if (sender == miAddressN1Page || sender == tbAddres8)
			{
				num += colorSizeFromSelectedPaletteType * cellColumnView * cellRowView;
			}
			if (sender == miAddressEndOfFile || sender == tbAddres9)
			{
				num = mData.Length - colorSizeFromSelectedPaletteType * cellColumnView * cellRowView;
			}
			SetAddress(num);
		}
	}

	private void scrollPanel_Scrolled(object sender, EventArgs e)
	{
		SetAddress(scrollPanel.Value);
	}

	private void AddressInput()
	{
		AddressInputForm addressInputForm = new AddressInputForm();
		addressInputForm.Address = mAddress;
		if (addressInputForm.ShowDialog(this) == DialogResult.OK)
		{
			SetAddress(addressInputForm.Address);
		}
	}

	private void SetAddress(int newAddr)
	{
		if (mData != null)
		{
			byte colorSizeFromSelectedPaletteType = GetColorSizeFromSelectedPaletteType();
			int cellColumnView = paletteSelectorFile.CellColumnView;
			int cellRowView = paletteSelectorFile.CellRowView;
			int num = 0;
			int num2 = mData.Length - colorSizeFromSelectedPaletteType * cellColumnView * cellRowView;
			if (newAddr > num2)
			{
				newAddr = num2;
			}
			if (newAddr < num)
			{
				newAddr = num;
			}
			if (mAddress != newAddr)
			{
				mAddress = newAddr;
				UpdateControlsAddress();
			}
		}
	}

	private void UpdateControlsAddress()
	{
		try
		{
			scrollPanel.Value = mAddress;
		}
		catch
		{
		}
		UpdatePaletteSelectorFile();
		SetEditorTarget();
		panelAddress.Refresh();
		int colorSizeFromSelectedPaletteType = GetColorSizeFromSelectedPaletteType();
		int num = 0;
		if (mData != null)
		{
			num = mData.Length;
		}
		int num2 = num - colorSizeFromSelectedPaletteType * 256;
		bool flag = mData != null;
		bool flag2 = mAddress > 0;
		bool flag3 = mAddress < num2;
		ToolStripMenuItem toolStripMenuItem = miAddressBeginOfFile;
		bool enabled = (tbAddres0.Enabled = flag && flag2);
		toolStripMenuItem.Enabled = enabled;
		ToolStripMenuItem toolStripMenuItem2 = miAddressP1Page;
		enabled = (tbAddres1.Enabled = flag && flag2);
		toolStripMenuItem2.Enabled = enabled;
		ToolStripMenuItem toolStripMenuItem3 = miAddressP1Line;
		enabled = (tbAddres2.Enabled = flag && flag2);
		toolStripMenuItem3.Enabled = enabled;
		ToolStripMenuItem toolStripMenuItem4 = miAddressP1Color;
		enabled = (tbAddres3.Enabled = flag && flag2);
		toolStripMenuItem4.Enabled = enabled;
		ToolStripMenuItem toolStripMenuItem5 = miAddressP1Byte;
		ToolStripButton toolStripButton = tbAddres4;
		bool flag9 = (tbAddressP1Byte.Enabled = flag && flag2);
		enabled = (toolStripButton.Enabled = flag9);
		toolStripMenuItem5.Enabled = enabled;
		ToolStripMenuItem toolStripMenuItem6 = miAddressN1Byte;
		ToolStripButton toolStripButton2 = tbAddres5;
		flag9 = (tbAddressN1Byte.Enabled = flag && flag3);
		enabled = (toolStripButton2.Enabled = flag9);
		toolStripMenuItem6.Enabled = enabled;
		ToolStripMenuItem toolStripMenuItem7 = miAddressN1Color;
		enabled = (tbAddres6.Enabled = flag && flag3);
		toolStripMenuItem7.Enabled = enabled;
		ToolStripMenuItem toolStripMenuItem8 = miAddressN1Line;
		enabled = (tbAddres7.Enabled = flag && flag3);
		toolStripMenuItem8.Enabled = enabled;
		ToolStripMenuItem toolStripMenuItem9 = miAddressN1Page;
		enabled = (tbAddres8.Enabled = flag && flag3);
		toolStripMenuItem9.Enabled = enabled;
		ToolStripMenuItem toolStripMenuItem10 = miAddressEndOfFile;
		enabled = (tbAddres9.Enabled = flag && flag3);
		toolStripMenuItem10.Enabled = enabled;
		ToolStripMenuItem toolStripMenuItem11 = miAddressInput;
		enabled = (tbAddresInputAddress.Enabled = flag);
		toolStripMenuItem11.Enabled = enabled;
		ToolStripMenuItem toolStripMenuItem12 = miAddressDetectPrev;
		enabled = (tbAddressDetectPrev.Enabled = flag);
		toolStripMenuItem12.Enabled = enabled;
		ToolStripMenuItem toolStripMenuItem13 = miAddressDetectNext;
		enabled = (tbAddressDetectNext.Enabled = flag);
		toolStripMenuItem13.Enabled = enabled;
		ToolStripMenuItem toolStripMenuItem14 = miAddressFind;
		enabled = (tbAddressFind.Enabled = flag);
		toolStripMenuItem14.Enabled = enabled;
		ToolStripMenuItem toolStripMenuItem15 = miAddressFindPrev;
		enabled = (tbAddressFindPrev.Enabled = flag);
		toolStripMenuItem15.Enabled = enabled;
		ToolStripMenuItem toolStripMenuItem16 = miAddressFindNext;
		enabled = (tbAddressFindNext.Enabled = flag);
		toolStripMenuItem16.Enabled = enabled;
	}

	private void SetFindDetectAddress(int newAddr)
	{
		byte colorSizeFromSelectedPaletteType = GetColorSizeFromSelectedPaletteType();
		int num = mSettings.FindAddressPalSelectorIndexPos;
		int num2 = newAddr;
		if (num2 - num * colorSizeFromSelectedPaletteType >= 0)
		{
			num2 -= num * colorSizeFromSelectedPaletteType;
		}
		else
		{
			num = 0;
		}
		if (paletteSelectorFile.SelectedIndex != num)
		{
			paletteSelectorFile.SelectedIndex = num;
			paletteSelectorFile.Refresh();
		}
		SetAddress(num2);
	}

	private int GetFindDetectStartAddr(int delta, int addSize)
	{
		byte colorSizeFromSelectedPaletteType = GetColorSizeFromSelectedPaletteType();
		int selectedIndex = paletteSelectorFile.SelectedIndex;
		int num = mAddress + selectedIndex * colorSizeFromSelectedPaletteType;
		num = ((delta <= 0) ? (num - addSize) : (num + addSize));
		if (num >= mData.Length)
		{
			num = mData.Length - addSize;
		}
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}

	private int GetQuarterViewPalByteSize()
	{
		int viewColorNum = GetViewColorNum();
		int colorSizeFromSelectedPaletteType = GetColorSizeFromSelectedPaletteType();
		return viewColorNum / 4 * colorSizeFromSelectedPaletteType;
	}

	private void tbAddressDetect_Click(object sender, EventArgs e)
	{
		if (mData != null)
		{
			int delta = 1;
			if (sender == miAddressDetectPrev || sender == tbAddressDetectPrev)
			{
				delta = -1;
			}
			if (sender == miAddressDetectNext || sender == tbAddressDetectNext)
			{
				delta = 1;
			}
			int quarterViewPalByteSize = GetQuarterViewPalByteSize();
			int findDetectStartAddr = GetFindDetectStartAddr(delta, quarterViewPalByteSize);
			int num = DetectAddress(findDetectStartAddr, delta);
			if (num >= 0)
			{
				SetFindDetectAddress(num);
				SetHint(Resources.HintDetect);
			}
			else
			{
				SystemSounds.Beep.Play();
				SetHint(Resources.HintDetectNotFound);
			}
		}
	}

	private bool CheckAddressRange(int addr)
	{
		if (mData == null)
		{
			return false;
		}
		if (addr >= 0)
		{
			return addr + 16 < mData.Length;
		}
		return false;
	}

	private int DetectAddress(int address, int delta)
	{
		int num = -1;
		if (mPaletteType == PaletteType.IndexedNes)
		{
			return DetectPaletteNES(address, delta);
		}
		if (mPaletteType == PaletteType.R5G5B5)
		{
			return DetectPaletteSNES(address, delta);
		}
		return DetectPaletteOther(address, delta);
	}

	private int DetectPaletteNES(int address, int delta)
	{
		int result = -1;
		for (int i = address; CheckAddressRange(i); i += delta)
		{
			if (CheckPaletteNES(i, first0F: true, mSettings.DetectOnlyTable))
			{
				result = i;
				break;
			}
		}
		return result;
	}

	private bool CheckPaletteNES(int addr, bool first0F, bool detectOnlyTable)
	{
		if (mData == null)
		{
			return false;
		}
		byte b = 0;
		int num = 0;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		for (int i = 0; i < 4; i++)
		{
			int num2 = addr + i;
			byte b2 = mData[num2];
			if (GetNesCheckFlags(b2) != 0)
			{
				return false;
			}
			if (first0F && i == 0 && b2 != 15)
			{
				return false;
			}
			if (b2 < 16)
			{
				flag = true;
				flag3 = true;
			}
			else if (b2 < 32)
			{
				flag = true;
				flag4 = true;
			}
			else if (b2 < 48)
			{
				flag2 = true;
				flag5 = true;
			}
			else
			{
				if (b2 >= 64)
				{
					return false;
				}
				flag2 = true;
				flag6 = true;
			}
			if (b2 == b)
			{
				num++;
			}
			if (num >= 3)
			{
				return false;
			}
			b = b2;
		}
		bool flag7 = false;
		flag7 = (flag3 && (flag4 || flag5) && flag6) || (flag && flag2) || ((flag3 && flag4) ? true : false);
		if (flag7 && detectOnlyTable)
		{
			if (CheckPaletteNES(addr + 4, first0F: true, detectOnlyTable: false))
			{
				if (!CheckPaletteNES(addr + 8, first0F: true, detectOnlyTable: false))
				{
					flag7 = false;
				}
			}
			else
			{
				flag7 = false;
			}
		}
		return flag7;
	}

	private int DetectPaletteSNES(int address, int delta)
	{
		int result = -1;
		for (int i = address; CheckAddressRange(i); i += delta)
		{
			if (CheckPaletteSNES(i, mSettings.DetectOnlyTable))
			{
				result = i;
				break;
			}
		}
		return result;
	}

	private bool CheckPaletteSNES(int addr, bool detectOnlyTable)
	{
		if (mData == null)
		{
			return false;
		}
		ushort num = 0;
		int num2 = 0;
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < 16; i++)
		{
			int num3 = addr + i * 2;
			byte b = mData[num3];
			byte b2 = mData[num3 + 1];
			if ((b2 & 0x80u) != 0)
			{
				return false;
			}
			ushort num4 = (ushort)(b | (b2 << 8));
			if (num4 == num)
			{
				num2++;
			}
			if (num2 >= 4)
			{
				return false;
			}
			num = num4;
			int num5 = (num4 >> 2) & 7;
			int num6 = (num4 >> 7) & 7;
			int num7 = (num4 >> 12) & 7;
			int num8 = (num5 + num6 + num7) / 3;
			if (num8 <= 1)
			{
				flag = true;
			}
			else if (num8 >= 6)
			{
				flag2 = true;
			}
		}
		bool flag3 = false;
		flag3 = ((flag && flag2) ? true : false);
		if (flag3 && detectOnlyTable)
		{
			if (CheckPaletteSNES(addr + 32, detectOnlyTable: false))
			{
				if (CheckPaletteSNES(addr + 64, detectOnlyTable: false))
				{
					if (!CheckPaletteSNES(addr + 96, detectOnlyTable: false))
					{
						flag3 = false;
					}
				}
				else
				{
					flag3 = false;
				}
			}
			else
			{
				flag3 = false;
			}
		}
		return flag3;
	}

	private int DetectPaletteOther(int address, int delta)
	{
		int result = -1;
		for (int i = address; CheckAddressRange(i); i += delta)
		{
			if (CheckPaletteOther(i, mSettings.DetectOnlyTable))
			{
				result = i;
				break;
			}
		}
		return result;
	}

	private bool CheckPaletteOther(int addr, bool detectOnlyTable)
	{
		if (mData == null)
		{
			return false;
		}
		int colorSizeFromSelectedPaletteType = GetColorSizeFromSelectedPaletteType();
		ulong num = 0uL;
		int num2 = 0;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		for (int i = 0; i < 16; i++)
		{
			int num3 = addr + i * colorSizeFromSelectedPaletteType;
			ulong num4 = 0uL;
			for (int j = 0; j < colorSizeFromSelectedPaletteType; j++)
			{
				int num5 = num3 + j;
				if (num5 >= 0 && num5 < mData.Length)
				{
					byte b = mData[num3 + j];
					num4 = (num4 << 8) | b;
				}
			}
			if (num4 == num)
			{
				num2++;
			}
			if (num2 >= 4)
			{
				return false;
			}
			num = num4;
			ColorBit colorBit = ColorBit.FromData(mData, num3, mPaletteType);
			byte r = colorBit.R;
			int g = colorBit.G;
			int b2 = colorBit.B;
			int num6 = (r + g + b2) / 3;
			if (num6 < 64)
			{
				flag = true;
			}
			else if (num6 >= 192)
			{
				flag2 = true;
			}
			else
			{
				flag3 = true;
			}
			if (detectOnlyTable && colorBit.IsInvalidPalette)
			{
				return false;
			}
		}
		bool flag4 = false;
		if (flag && flag2 && flag3)
		{
			return true;
		}
		return false;
	}

	private void tbAddressDetectOnlyTable_Click(object sender, EventArgs e)
	{
		mSettings.DetectOnlyTable = !mSettings.DetectOnlyTable;
		UpdateDetectSetting();
	}

	private void UpdateDetectSetting()
	{
		ToolStripMenuItem toolStripMenuItem = miAddressDetectOnlyTable;
		bool @checked = (tbAddressDetectOnlyTable.Checked = mSettings.DetectOnlyTable);
		toolStripMenuItem.Checked = @checked;
	}

	private void miAddressFind_Click(object sender, EventArgs e)
	{
		if (mData == null)
		{
			return;
		}
		int num = 0;
		int num2 = mAddress;
		bool flag = mFindData == null || mFindDataEnabled == null;
		if (sender == miAddressFind || sender == tbAddressFind || flag)
		{
			using PaletteFindForm paletteFindForm = new PaletteFindForm();
			paletteFindForm.FindData = mFindData;
			paletteFindForm.FindDataEnabled = mFindDataEnabled;
			paletteFindForm.PaletteType = mPaletteType;
			paletteFindForm.IndexedPalette = paletteSelectorSourcePal.Palette;
			if (paletteFindForm.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			mFindData = paletteFindForm.FindData;
			mFindDataEnabled = paletteFindForm.FindDataEnabled;
			num = paletteFindForm.DirectionDelta;
		}
		if ((sender == miAddressFindPrev || sender == tbAddressFindPrev) && !flag)
		{
			num = -1;
		}
		if ((sender == miAddressFindNext || sender == tbAddressFindNext) && !flag)
		{
			num = 1;
		}
		int addSize = 1;
		int findDetectStartAddr = GetFindDetectStartAddr(num, addSize);
		bool next = num > 0;
		num2 = FindData(findDetectStartAddr, next, mFindData, mFindDataEnabled);
		if (num2 < 0)
		{
			SystemSounds.Beep.Play();
			SetHint(Resources.HintFindNotFound);
			return;
		}
		SetFindDetectAddress(num2);
		int num3 = mFindDataEnabled.Length - 1;
		paletteSelectorFile.DrawMultiSelect = true;
		paletteSelectorFile.SelectEndIndex = paletteSelectorFile.SelectedIndex + num3;
		paletteSelectorFile.Refresh();
	}

	private int FindData(int addrStart, bool next, byte[] findData, bool[] findDataEnabled)
	{
		if (findData == null)
		{
			return -1;
		}
		if (findDataEnabled == null)
		{
			return -1;
		}
		int result = -1;
		if (next)
		{
			for (int i = addrStart; i < mData.Length; i++)
			{
				if (CheckDataMatch(i, findData, findDataEnabled))
				{
					result = i;
					break;
				}
			}
		}
		else
		{
			for (int num = addrStart; num >= 0; num--)
			{
				if (CheckDataMatch(num, findData, findDataEnabled))
				{
					result = num;
					break;
				}
			}
		}
		return result;
	}

	private bool CheckDataMatch(int dataAddr, byte[] findData, bool[] findDataEnabled)
	{
		if (mData == null || findData == null || findDataEnabled == null)
		{
			return false;
		}
		int colorSizeFromSelectedPaletteType = GetColorSizeFromSelectedPaletteType();
		bool result = true;
		for (int i = 0; i < findData.Length; i++)
		{
			int num = i / colorSizeFromSelectedPaletteType;
			if (findDataEnabled[num] && dataAddr >= 0 && dataAddr < mData.Length && findData[i] != mData[dataAddr])
			{
				result = false;
				break;
			}
			dataAddr++;
		}
		if (dataAddr >= mData.Length - 1)
		{
			return false;
		}
		return result;
	}

	private void InitPaletteTypeToolStripItem()
	{
		object obj3 = (miType6.Tag = (tbType6.Tag = PaletteType.IndexedNes));
		obj3 = (miType4.Tag = (tbType4.Tag = PaletteType.IndexedMsx));
		obj3 = (miType9.Tag = (tbType9.Tag = PaletteType.R3G3B3));
		obj3 = (miType15.Tag = (tbType15.Tag = PaletteType.R5G5B5));
		obj3 = (miType16.Tag = (tbType16.Tag = PaletteType.R5G6B5));
		obj3 = (miType24.Tag = (tbType24.Tag = PaletteType.R8G8B8));
		obj3 = (miType32.Tag = (tbType32.Tag = PaletteType.A8R8G8B8));
	}

	private void miTypeSelect_Click(object sender, EventArgs e)
	{
		if (sender is ToolStripItem && sender is ToolStripItem toolStripItem && toolStripItem.Tag is PaletteType)
		{
			PaletteType palType = (PaletteType)toolStripItem.Tag;
			UpdatePaletteType(palType);
		}
	}

	private void UpdatePaletteType(PaletteType palType)
	{
		mPaletteType = palType;
		UpdateControlsPaletteType();
	}

	private void UpdateControlsPaletteType()
	{
		bool isIndexedPalette = IsIndexedPalette;
		panelEditorIndex.Visible = isIndexedPalette;
		panelEditorRGB.Visible = !isIndexedPalette;
		if (isIndexedPalette)
		{
			UpdateIndexedPaletteTable(mPaletteType);
		}
		panelEditorRGB.Location = panelEditorIndex.Location;
		SetEditorTarget();
		UpdateScrollBar();
		UpdatePaletteSelectorFile();
		ToolStripMenuItem toolStripMenuItem = miType6;
		bool @checked = (tbType6.Checked = mPaletteType == PaletteType.IndexedNes);
		toolStripMenuItem.Checked = @checked;
		ToolStripMenuItem toolStripMenuItem2 = miType4;
		@checked = (tbType4.Checked = mPaletteType == PaletteType.IndexedMsx);
		toolStripMenuItem2.Checked = @checked;
		ToolStripMenuItem toolStripMenuItem3 = miType9;
		@checked = (tbType9.Checked = mPaletteType == PaletteType.R3G3B3);
		toolStripMenuItem3.Checked = @checked;
		ToolStripMenuItem toolStripMenuItem4 = miType15;
		@checked = (tbType15.Checked = mPaletteType == PaletteType.R5G5B5);
		toolStripMenuItem4.Checked = @checked;
		ToolStripMenuItem toolStripMenuItem5 = miType16;
		@checked = (tbType16.Checked = mPaletteType == PaletteType.R5G6B5);
		toolStripMenuItem5.Checked = @checked;
		ToolStripMenuItem toolStripMenuItem6 = miType24;
		@checked = (tbType24.Checked = mPaletteType == PaletteType.R8G8B8);
		toolStripMenuItem6.Checked = @checked;
		ToolStripMenuItem toolStripMenuItem7 = miType32;
		@checked = (tbType32.Checked = mPaletteType == PaletteType.A8R8G8B8);
		toolStripMenuItem7.Checked = @checked;
	}

	private byte GetColorSizeFromSelectedPaletteType()
	{
		return ColorBit.GetByteSizeFromPaletteType(mPaletteType);
	}

	private void settingToolStripMenuItem_Click(object sender, EventArgs e)
	{
		using SettingForm settingForm = new SettingForm();
		if (settingForm.ShowDialog() == DialogResult.OK)
		{
			mSettings.Save();
			UpdateAllControls();
			UpdateControlSizeFromGuiZoomRate();
		}
	}

	private void UpdateAllControls()
	{
		UpdateClipboardToControls();
		UpdateAddressPanelVisible();
		UpdateBuildDateLabel();
		UpdateCheckPAL();
		UpdateDetectSetting();
		UpdateReadonlyMode();
		UpdatePaletteSelectorFile();
		SetEditorTarget();
	}

	private void UpdateControlSizeFromGuiZoomRate()
	{
		if (base.DesignMode)
		{
			return;
		}
		int guiSizeRate = mSettings.GuiSizeRate;
		paletteSelectorFile.CellWidth = 8 * guiSizeRate;
		paletteSelectorFile.CellHeight = 8 * guiSizeRate;
		panelAddress.Height = paletteSelectorFile.Height;
		paletteSelectorSourcePal.CellWidth = 8 * guiSizeRate;
		paletteSelectorSourcePal.CellHeight = 8 * guiSizeRate;
		int num = paletteSelectorSourcePal.Width + 16;
		int num2 = paletteSelectorSourcePal.Height + 16;
		Size size = new Size(num, num2);
		if (panelEditorIndex.Size != size)
		{
			panelEditorIndex.Size = size;
		}
		rgbEditor1.ClientSize = new Size(128 * guiSizeRate, 32 * guiSizeRate);
		buttonOK.Top = rgbEditor1.Bottom + 3;
		buttonReset.Top = buttonOK.Top;
		int num3 = SystemInformation.Border3DSize.Width;
		int num4 = SystemInformation.Border3DSize.Height;
		int num5 = 128 * guiSizeRate + num3 * 2 + 16;
		int num6 = 128 * guiSizeRate + num4 * 2 + 16;
		Size size2 = new Size(num5, num6);
		if (panelEditorRGB.Size != size2)
		{
			panelEditorRGB.Size = size2;
		}
		Point point = new Point(0, paletteSelectorFile.Top);
		if (panelAddress.Location != point)
		{
			panelAddress.Location = point;
		}
		int num7 = 0;
		num7 = (panelAddress.Visible ? (panelAddress.Width + 1) : 0);
		Point point2 = new Point(num7, 0);
		if (paletteSelectorFile.Location != point2)
		{
			paletteSelectorFile.Location = point2;
		}
		Size size3 = new Size(paletteSelectorFile.Right, paletteSelectorFile.Bottom);
		if (scrollPanel.ClientAreaSize != size3)
		{
			scrollPanel.ClientAreaSize = size3;
		}
		int num8 = scrollPanel.Right + 5;
		Point point3 = new Point(num8, scrollPanel.Top);
		if (panelToolStripAddress.Location != point3)
		{
			panelToolStripAddress.Location = point3;
		}
		int num9 = panelToolStripAddress.Right + 10;
		Point point4 = new Point(num9, scrollPanel.Top);
		if (panelEditorIndex.Location != point4)
		{
			panelEditorIndex.Location = point4;
		}
		if (panelEditorRGB.Location != point4)
		{
			panelEditorRGB.Location = point4;
		}
		int bottom = paletteSelectorFile.Bottom;
		int bottom2 = panelEditorIndex.Bottom;
		int bottom3 = panelEditorRGB.Bottom;
		int val = Math.Max(bottom2, bottom3);
		int num10 = Math.Max(bottom, val);
		int num11 = statusStrip.Height;
		int num12 = num10 + 11 + num11;
		int num13 = SystemInformation.FixedFrameBorderSize.Height;
		int captionHeight = SystemInformation.CaptionHeight;
		int num14 = num13 * 2 + captionHeight + num12;
		int right = panelEditorIndex.Right;
		int right2 = panelEditorRGB.Right;
		int val2 = Math.Max(right, right2);
		int num15 = 0;
		foreach (ToolStripItem item in toolStrip.Items)
		{
			num15 += item.Size.Width;
		}
		int num16 = Math.Max(val2, num15) + 21;
		int num17 = SystemInformation.FixedFrameBorderSize.Width;
		int num18 = num16 + num17 * 2;
		Size size4 = new Size(num18, num14);
		if (base.Size != size4)
		{
			base.Size = size4;
		}
	}

	private void tbOptionCheckPAL_Click(object sender, EventArgs e)
	{
		mSettings.CheckPAL = !mSettings.CheckPAL;
		UpdateCheckPAL();
	}

	private void UpdateCheckPAL()
	{
		SetEditorTarget();
		UpdatePaletteSelectorFile();
		ToolStripMenuItem toolStripMenuItem = miOptionCheckPAL;
		bool @checked = (tbOptionCheckPAL.Checked = mSettings.CheckPAL);
		toolStripMenuItem.Checked = @checked;
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
			MsgBox.Show(this, ex.Message, "ROM Palette Editor");
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

	private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
	{
		string obj = "ROM Palette Editor Ver.0.90  build " + GetBuildDate() + "\r\n\t\t:: Copyright 2019 Yy, Yorn\r\n\t\t:: https://www45.atwiki.jp/yychr/\r\n\t\t:: https://sites.google.com/site/yornjp/\r\n\r\n------------------------lib------------------------\r\nFamicom Palette\t:: By Kasion\r\n\t\t:: http://hlc6502.web.fc2.com/NesPal2.htm\r\nFamicom Palette\t:: By misaki\r\n\t\t:: http://metalidol.xxxxxxxx.jp/famicom_palette.html\r\nMSX Palette\t:: By wizforest\r\n\t\t:: https://www.wizforest.com/OldGood/ntsc/msx.html\r\nGame console Icon :: By Yoshi\r\n\t\t:: http://yspixel.jpn.org\r\n";
		Size size = new Size(640, 320);
		MessageBoxForm.Show(obj, "About ROM Palette Editor", MessageBoxIcon.Asterisk, MessageBoxButtons.OK, size);
	}

	private string GetBuildDate()
	{
		Version version = GetType().Assembly.GetName().Version;
		int build = version.Build;
		int revision = version.Revision;
		return new DateTime(2000, 1, 1).AddDays(build).AddSeconds(revision * 2).ToString("yyyy/MM/dd");
	}

	private void paletteSelectorFile_OnPaletteSelect(object sender, EventArgs e)
	{
		SetEditorTarget();
	}

	private void SetEditorTarget()
	{
		SetIndexEditorTarget();
		SetRgbEditorTarget();
	}

	private void SetIndexEditorTarget()
	{
		if (!panelEditorIndex.Visible)
		{
			return;
		}
		if (mData != null)
		{
			int addressOfSelectedIndex = GetAddressOfSelectedIndex();
			if (addressOfSelectedIndex >= 0 && addressOfSelectedIndex < mData.Length)
			{
				byte selectedIndex = mData[addressOfSelectedIndex];
				paletteSelectorSourcePal.SelectedIndex = selectedIndex;
			}
		}
		byte[] array = new byte[256];
		if (mSettings.CheckPAL)
		{
			for (int i = 0; i < 256; i++)
			{
				if (mPaletteType == PaletteType.IndexedNes)
				{
					array[i] = GetNesCheckFlags((byte)i);
				}
				else if (mPaletteType == PaletteType.IndexedMsx)
				{
					array[i] = GetMsx1CheckFlags((byte)i);
				}
			}
		}
		paletteSelectorSourcePal.PaletteFlags = array;
		paletteSelectorSourcePal.Refresh();
	}

	private void SetRgbEditorTarget()
	{
		if (!panelEditorRGB.Visible)
		{
			return;
		}
		if (mViewColorBits != null && mViewColorBits.Length >= GetViewColorNum())
		{
			int selectedIndex = paletteSelectorFile.SelectedIndex;
			ColorBit colorBit = mViewColorBits[selectedIndex];
			rgbEditor1.SetColorBitFrom(colorBit);
			bool flag = false;
			if (mSettings.AlphaInRgbEditor == Settings.ShowAlphaInRgbEditor.Enabled)
			{
				flag = true;
			}
			else if (mSettings.AlphaInRgbEditor == Settings.ShowAlphaInRgbEditor.IfSupported)
			{
				flag = (colorBit.EnabledAlpha ? true : false);
			}
			if (rgbEditor1.ShowAlpha != flag)
			{
				rgbEditor1.ShowAlpha = flag;
			}
			panelPreview.Refresh();
			string text = colorBit.ToColorString();
			SetTextBoxWithoutChangedEvent(textBoxHex, text);
			textBoxHexBit.Text = colorBit.GetByteDataText();
		}
		else
		{
			rgbEditor1.ColorBit.SetPaletteType(mPaletteType);
			rgbEditor1.Refresh();
		}
	}

	private void SetTextBoxWithoutChangedEvent(TextBox textBox, string text)
	{
		DisableTextBoxChanged = true;
		textBox.Text = text;
		DisableTextBoxChanged = false;
	}

	private void textBoxHex_TextChanged(object sender, EventArgs e)
	{
		if (DisableTextBoxChanged)
		{
			return;
		}
		string text = textBoxHex.Text;
		if (text.Length == 6 && Regex.Match(text, "[0-9a-fA-F]{6}").Length == 6)
		{
			try
			{
				int num = Convert.ToInt32(text, 16);
				int red = (num >> 16) & 0xFF;
				int green = (num >> 8) & 0xFF;
				int blue = num & 0xFF;
				rgbEditor1.SetColor(Color.FromArgb(red, green, blue));
			}
			catch
			{
			}
		}
	}

	private void paletteSelectorSourcePal_OnPaletteSelected(object sender, EventArgs e)
	{
		if (mSettings.ReadOnlyMode || mData == null)
		{
			return;
		}
		byte b = (byte)paletteSelectorSourcePal.SelectedIndex;
		if ((mPaletteType != PaletteType.IndexedNes || !mSettings.CheckPAL || b < 64) && (mPaletteType != PaletteType.IndexedMsx || !mSettings.CheckPAL || b < 16))
		{
			byte[] array = new byte[1] { b };
			if (array != null)
			{
				SetDataToSelectedPaletteAddress(array, makeUndoBuffer: true);
			}
		}
	}

	private void rgbEditor1_ColorChanged(object sender, EventArgs e)
	{
		ColorBit colorBit = rgbEditor1.ColorBit;
		if (colorBit != null)
		{
			string text = colorBit.ToColorString();
			SetTextBoxWithoutChangedEvent(textBoxHex, text);
			textBoxHexBit.Text = colorBit.GetByteDataText();
		}
		rgbEditor1.Refresh();
		panelPreview.Refresh();
	}

	private void panelPreview_Paint(object sender, PaintEventArgs e)
	{
		Graphics graphics = e.Graphics;
		graphics.Clear(Color.Black);
		try
		{
			Color color = rgbEditor1.ColorBit.Color;
			graphics.Clear(color);
		}
		catch
		{
		}
	}

	private void buttonOK_Click(object sender, EventArgs e)
	{
		if (!mSettings.ReadOnlyMode && mData != null)
		{
			byte[] byteData = rgbEditor1.ColorBit.GetByteData();
			if (byteData != null)
			{
				SetDataToSelectedPaletteAddress(byteData, makeUndoBuffer: true);
			}
		}
	}

	private void buttonReset_Click(object sender, EventArgs e)
	{
		if (!mSettings.ReadOnlyMode)
		{
			UpdatePaletteSelectorFile();
			SetEditorTarget();
		}
	}

	private void SetDataToSelectedPaletteAddress(byte[] data, bool makeUndoBuffer)
	{
		if (!mSettings.ReadOnlyMode && mData != null)
		{
			if (data != null && data.Length != 0)
			{
				int addressOfSelectedIndex = GetAddressOfSelectedIndex();
				mUndoManager.WriteNewDataAndCreateUndoBuffer(mData, addressOfSelectedIndex, data);
			}
			else
			{
				SystemSounds.Beep.Play();
			}
			UpdatePaletteSelectorFile();
			SetEditorTarget();
			UpdateUndoRedoControl();
		}
	}

	private void panelAddress_Paint(object sender, PaintEventArgs e)
	{
		int num = paletteSelectorFile.CellRowView;
		if (num < 1)
		{
			num = 1;
		}
		int num2 = paletteSelectorFile.ClientSize.Height / num;
		Graphics graphics = e.Graphics;
		Color backColor = panelAddress.BackColor;
		graphics.Clear(backColor);
		Color foreColor = panelAddress.ForeColor;
		Font font = panelAddress.Font;
		int cellColumnView = paletteSelectorFile.CellColumnView;
		int colorSizeFromSelectedPaletteType = GetColorSizeFromSelectedPaletteType();
		int num3 = panelAddress.ClientSize.Width;
		int num4 = 0;
		using StringFormat stringFormat = new StringFormat(StringFormatFlags.NoWrap);
		using Brush brush = new SolidBrush(foreColor);
		stringFormat.Alignment = StringAlignment.Center;
		stringFormat.LineAlignment = StringAlignment.Center;
		for (int i = 0; i < num; i++)
		{
			string s = (mAddress + i * cellColumnView * colorSizeFromSelectedPaletteType).ToString("X8");
			Rectangle rectangle = new Rectangle(0, num4, num3, num2);
			graphics.DrawString(s, font, brush, rectangle, stringFormat);
			num4 += num2;
		}
	}

	private void miOptionShowAddress_Click(object sender, EventArgs e)
	{
		mSettings.AddressPanelVisible = !mSettings.AddressPanelVisible;
		UpdateAddressPanelVisible();
	}

	private void UpdateAddressPanelVisible()
	{
		bool addressPanelVisible = mSettings.AddressPanelVisible;
		if (panelAddress.Visible != addressPanelVisible)
		{
			panelAddress.Visible = addressPanelVisible;
			UpdateControlSizeFromGuiZoomRate();
		}
		miOptionShowAddress.Checked = addressPanelVisible;
	}

	private void hintControl_MouseEnter(object sender, EventArgs e)
	{
		string hint = "";
		if (sender == paletteSelectorFile)
		{
			hint = Resources.HintRightDragSelect;
		}
		if ((sender == panelEditorIndex || sender == panelEditorRGB || sender == rgbEditor1) && mSettings.ReadOnlyMode)
		{
			hint = Resources.HintReadOnlyMode;
		}
		SetHint(hint);
	}

	private void hintControl_MouseLeave(object sender, EventArgs e)
	{
	}

	private void SetHint(string hint)
	{
		slHint.Text = hint;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PaletteEditor.PaletteEditorMainForm));
		this.menuStrip = new System.Windows.Forms.MenuStrip();
		this.miFile = new System.Windows.Forms.ToolStripMenuItem();
		this.miFileOpen = new System.Windows.Forms.ToolStripMenuItem();
		this.miFileReload = new System.Windows.Forms.ToolStripMenuItem();
		this.miFileSave = new System.Windows.Forms.ToolStripMenuItem();
		this.miFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
		this.miFileReadonlyMode = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
		this.miFileExit = new System.Windows.Forms.ToolStripMenuItem();
		this.miEdit = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditUndo = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditRedo = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripSeparator();
		this.miEditCopy = new System.Windows.Forms.ToolStripMenuItem();
		this.miEditPaste = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddress = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddressInput = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
		this.miAddressBeginOfFile = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddressP1Page = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddressP1Line = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddressP1Color = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddressP1Byte = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddressN1Byte = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddressN1Color = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddressN1Line = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddressN1Page = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddressEndOfFile = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
		this.miAddressDetectOnlyTable = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddressDetectPrev = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddressDetectNext = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
		this.miAddressFind = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddressFindPrev = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddressFindNext = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
		this.miAddressSettingFile = new System.Windows.Forms.ToolStripMenuItem();
		this.miType = new System.Windows.Forms.ToolStripMenuItem();
		this.miType6 = new System.Windows.Forms.ToolStripMenuItem();
		this.miType4 = new System.Windows.Forms.ToolStripMenuItem();
		this.miType9 = new System.Windows.Forms.ToolStripMenuItem();
		this.miType15 = new System.Windows.Forms.ToolStripMenuItem();
		this.miType16 = new System.Windows.Forms.ToolStripMenuItem();
		this.miType24 = new System.Windows.Forms.ToolStripMenuItem();
		this.miType32 = new System.Windows.Forms.ToolStripMenuItem();
		this.miOption = new System.Windows.Forms.ToolStripMenuItem();
		this.settingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
		this.miOptionShowAddress = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripSeparator();
		this.miOptionCheckPAL = new System.Windows.Forms.ToolStripMenuItem();
		this.miHelp = new System.Windows.Forms.ToolStripMenuItem();
		this.miHelpPropertyEditor = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripSeparator();
		this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.statusStrip = new System.Windows.Forms.StatusStrip();
		this.slAddress = new System.Windows.Forms.ToolStripStatusLabel();
		this.slHint = new System.Windows.Forms.ToolStripStatusLabel();
		this.toolStrip = new System.Windows.Forms.ToolStrip();
		this.tbFileOpen = new System.Windows.Forms.ToolStripButton();
		this.tbFileSave = new System.Windows.Forms.ToolStripButton();
		this.tbFileReadonlyMode = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.tbEditUndo = new System.Windows.Forms.ToolStripButton();
		this.tbEditRedo = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
		this.tbEditCopy = new System.Windows.Forms.ToolStripButton();
		this.tbEditPaste = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
		this.tlType = new System.Windows.Forms.ToolStripLabel();
		this.tbType6 = new System.Windows.Forms.ToolStripButton();
		this.tbType4 = new System.Windows.Forms.ToolStripButton();
		this.tbType9 = new System.Windows.Forms.ToolStripButton();
		this.tbType15 = new System.Windows.Forms.ToolStripButton();
		this.tbType16 = new System.Windows.Forms.ToolStripButton();
		this.tbType24 = new System.Windows.Forms.ToolStripButton();
		this.tbType32 = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
		this.tbOptionCheckPAL = new System.Windows.Forms.ToolStripButton();
		this.tbAddressP1Byte = new System.Windows.Forms.ToolStripButton();
		this.tbAddressN1Byte = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
		this.tlDetect = new System.Windows.Forms.ToolStripLabel();
		this.tbAddressDetectOnlyTable = new System.Windows.Forms.ToolStripButton();
		this.tbAddressDetectPrev = new System.Windows.Forms.ToolStripButton();
		this.tbAddressDetectNext = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
		this.tbAddressFind = new System.Windows.Forms.ToolStripButton();
		this.tbAddressFindPrev = new System.Windows.Forms.ToolStripButton();
		this.tbAddressFindNext = new System.Windows.Forms.ToolStripButton();
		this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
		this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
		this.panelEditorIndex = new System.Windows.Forms.Panel();
		this.paletteSelectorSourcePal = new ControlLib.PaletteSelector();
		this.panelEditorRGB = new System.Windows.Forms.Panel();
		this.labelHexValue = new System.Windows.Forms.Label();
		this.textBoxHexBit = new System.Windows.Forms.TextBox();
		this.panelPreview = new ControlLib.PicturePanel();
		this.textBoxHex = new System.Windows.Forms.TextBox();
		this.rgbEditor1 = new ControlLib.RGBEditor();
		this.buttonReset = new ControlLib.ButtonNoFocus();
		this.buttonOK = new ControlLib.ButtonNoFocus();
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
		this.lVersion = new System.Windows.Forms.Label();
		this.scrollPanel = new ControlLib.ScrollPanel();
		this.panelAddress = new ControlLib.PicturePanel();
		this.paletteSelectorFile = new ControlLib.PaletteSelector();
		this.menuStrip.SuspendLayout();
		this.statusStrip.SuspendLayout();
		this.toolStrip.SuspendLayout();
		this.panelEditorIndex.SuspendLayout();
		this.panelEditorRGB.SuspendLayout();
		this.panelToolStripAddress.SuspendLayout();
		this.toolStripAddress.SuspendLayout();
		this.scrollPanel.SuspendLayout();
		base.SuspendLayout();
		this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[6] { this.miFile, this.miEdit, this.miAddress, this.miType, this.miOption, this.miHelp });
		resources.ApplyResources(this.menuStrip, "menuStrip");
		this.menuStrip.Name = "menuStrip";
		this.menuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		this.miFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[8] { this.miFileOpen, this.miFileReload, this.miFileSave, this.miFileSaveAs, this.toolStripMenuItem1, this.miFileReadonlyMode, this.toolStripMenuItem6, this.miFileExit });
		this.miFile.Name = "miFile";
		resources.ApplyResources(this.miFile, "miFile");
		this.miFileOpen.Image = PaletteEditor.Properties.Resources.FileOpen;
		resources.ApplyResources(this.miFileOpen, "miFileOpen");
		this.miFileOpen.Name = "miFileOpen";
		this.miFileOpen.Click += new System.EventHandler(miFileOpen_Click);
		resources.ApplyResources(this.miFileReload, "miFileReload");
		this.miFileReload.Name = "miFileReload";
		this.miFileReload.Click += new System.EventHandler(miFileReload_Click);
		this.miFileSave.Image = PaletteEditor.Properties.Resources.FileSave;
		resources.ApplyResources(this.miFileSave, "miFileSave");
		this.miFileSave.Name = "miFileSave";
		this.miFileSave.Click += new System.EventHandler(miFileSave_Click);
		this.miFileSaveAs.Image = PaletteEditor.Properties.Resources.FileSave;
		resources.ApplyResources(this.miFileSaveAs, "miFileSaveAs");
		this.miFileSaveAs.Name = "miFileSaveAs";
		this.miFileSaveAs.Click += new System.EventHandler(miFileSaveAs_Click);
		this.toolStripMenuItem1.Name = "toolStripMenuItem1";
		resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
		this.miFileReadonlyMode.Image = PaletteEditor.Properties.Resources.IconEditDisabled;
		resources.ApplyResources(this.miFileReadonlyMode, "miFileReadonlyMode");
		this.miFileReadonlyMode.Name = "miFileReadonlyMode";
		this.miFileReadonlyMode.Click += new System.EventHandler(tbFileReadonlyMode_Click);
		this.toolStripMenuItem6.Name = "toolStripMenuItem6";
		resources.ApplyResources(this.toolStripMenuItem6, "toolStripMenuItem6");
		this.miFileExit.Image = PaletteEditor.Properties.Resources.FileExit;
		resources.ApplyResources(this.miFileExit, "miFileExit");
		this.miFileExit.Name = "miFileExit";
		this.miFileExit.Click += new System.EventHandler(miFileExit_Click);
		this.miEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.miEditUndo, this.miEditRedo, this.toolStripMenuItem9, this.miEditCopy, this.miEditPaste });
		this.miEdit.Name = "miEdit";
		resources.ApplyResources(this.miEdit, "miEdit");
		this.miEditUndo.Image = PaletteEditor.Properties.Resources.EditUndo;
		resources.ApplyResources(this.miEditUndo, "miEditUndo");
		this.miEditUndo.Name = "miEditUndo";
		this.miEditUndo.Click += new System.EventHandler(miEditUndo_Click);
		this.miEditRedo.Image = PaletteEditor.Properties.Resources.EditRedo;
		resources.ApplyResources(this.miEditRedo, "miEditRedo");
		this.miEditRedo.Name = "miEditRedo";
		this.miEditRedo.Click += new System.EventHandler(miEditRedo_Click);
		this.toolStripMenuItem9.Name = "toolStripMenuItem9";
		resources.ApplyResources(this.toolStripMenuItem9, "toolStripMenuItem9");
		this.miEditCopy.Image = PaletteEditor.Properties.Resources.EditCopy;
		resources.ApplyResources(this.miEditCopy, "miEditCopy");
		this.miEditCopy.Name = "miEditCopy";
		this.miEditCopy.Click += new System.EventHandler(miEditCopy_Click);
		resources.ApplyResources(this.miEditPaste, "miEditPaste");
		this.miEditPaste.Image = PaletteEditor.Properties.Resources.EditPaste;
		this.miEditPaste.Name = "miEditPaste";
		this.miEditPaste.Click += new System.EventHandler(miEditPaste_Click);
		this.miAddress.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[22]
		{
			this.miAddressInput, this.toolStripMenuItem3, this.miAddressBeginOfFile, this.miAddressP1Page, this.miAddressP1Line, this.miAddressP1Color, this.miAddressP1Byte, this.miAddressN1Byte, this.miAddressN1Color, this.miAddressN1Line,
			this.miAddressN1Page, this.miAddressEndOfFile, this.toolStripMenuItem5, this.miAddressDetectOnlyTable, this.miAddressDetectPrev, this.miAddressDetectNext, this.toolStripMenuItem4, this.miAddressFind, this.miAddressFindPrev, this.miAddressFindNext,
			this.toolStripMenuItem2, this.miAddressSettingFile
		});
		this.miAddress.Name = "miAddress";
		resources.ApplyResources(this.miAddress, "miAddress");
		this.miAddressInput.Image = PaletteEditor.Properties.Resources.AddrA;
		resources.ApplyResources(this.miAddressInput, "miAddressInput");
		this.miAddressInput.Name = "miAddressInput";
		this.miAddressInput.Click += new System.EventHandler(miAddressInput_Click);
		this.toolStripMenuItem3.Name = "toolStripMenuItem3";
		resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
		this.miAddressBeginOfFile.Image = PaletteEditor.Properties.Resources.Addr0;
		resources.ApplyResources(this.miAddressBeginOfFile, "miAddressBeginOfFile");
		this.miAddressBeginOfFile.Name = "miAddressBeginOfFile";
		this.miAddressBeginOfFile.Click += new System.EventHandler(miAddress_Click);
		this.miAddressP1Page.Image = PaletteEditor.Properties.Resources.Addr1;
		resources.ApplyResources(this.miAddressP1Page, "miAddressP1Page");
		this.miAddressP1Page.Name = "miAddressP1Page";
		this.miAddressP1Page.Click += new System.EventHandler(miAddress_Click);
		this.miAddressP1Line.Image = PaletteEditor.Properties.Resources.Addr2;
		resources.ApplyResources(this.miAddressP1Line, "miAddressP1Line");
		this.miAddressP1Line.Name = "miAddressP1Line";
		this.miAddressP1Line.Click += new System.EventHandler(miAddress_Click);
		this.miAddressP1Color.Image = PaletteEditor.Properties.Resources.Addr3;
		resources.ApplyResources(this.miAddressP1Color, "miAddressP1Color");
		this.miAddressP1Color.Name = "miAddressP1Color";
		this.miAddressP1Color.Click += new System.EventHandler(miAddress_Click);
		this.miAddressP1Byte.Image = PaletteEditor.Properties.Resources.Addr4;
		resources.ApplyResources(this.miAddressP1Byte, "miAddressP1Byte");
		this.miAddressP1Byte.Name = "miAddressP1Byte";
		this.miAddressP1Byte.Click += new System.EventHandler(miAddress_Click);
		this.miAddressN1Byte.Image = PaletteEditor.Properties.Resources.Addr5;
		resources.ApplyResources(this.miAddressN1Byte, "miAddressN1Byte");
		this.miAddressN1Byte.Name = "miAddressN1Byte";
		this.miAddressN1Byte.Click += new System.EventHandler(miAddress_Click);
		this.miAddressN1Color.Image = PaletteEditor.Properties.Resources.Addr6;
		resources.ApplyResources(this.miAddressN1Color, "miAddressN1Color");
		this.miAddressN1Color.Name = "miAddressN1Color";
		this.miAddressN1Color.Click += new System.EventHandler(miAddress_Click);
		this.miAddressN1Line.Image = PaletteEditor.Properties.Resources.Addr7;
		resources.ApplyResources(this.miAddressN1Line, "miAddressN1Line");
		this.miAddressN1Line.Name = "miAddressN1Line";
		this.miAddressN1Line.Click += new System.EventHandler(miAddress_Click);
		this.miAddressN1Page.Image = PaletteEditor.Properties.Resources.Addr8;
		resources.ApplyResources(this.miAddressN1Page, "miAddressN1Page");
		this.miAddressN1Page.Name = "miAddressN1Page";
		this.miAddressN1Page.Click += new System.EventHandler(miAddress_Click);
		this.miAddressEndOfFile.Image = PaletteEditor.Properties.Resources.Addr9;
		resources.ApplyResources(this.miAddressEndOfFile, "miAddressEndOfFile");
		this.miAddressEndOfFile.Name = "miAddressEndOfFile";
		this.miAddressEndOfFile.Click += new System.EventHandler(miAddress_Click);
		this.toolStripMenuItem5.Name = "toolStripMenuItem5";
		resources.ApplyResources(this.toolStripMenuItem5, "toolStripMenuItem5");
		this.miAddressDetectOnlyTable.Image = PaletteEditor.Properties.Resources.IconOptionSetting;
		resources.ApplyResources(this.miAddressDetectOnlyTable, "miAddressDetectOnlyTable");
		this.miAddressDetectOnlyTable.Name = "miAddressDetectOnlyTable";
		this.miAddressDetectOnlyTable.Click += new System.EventHandler(tbAddressDetectOnlyTable_Click);
		this.miAddressDetectPrev.Image = PaletteEditor.Properties.Resources.AddrDetecPrev;
		resources.ApplyResources(this.miAddressDetectPrev, "miAddressDetectPrev");
		this.miAddressDetectPrev.Name = "miAddressDetectPrev";
		this.miAddressDetectPrev.Click += new System.EventHandler(tbAddressDetect_Click);
		this.miAddressDetectNext.Image = PaletteEditor.Properties.Resources.AddrDetectNext;
		resources.ApplyResources(this.miAddressDetectNext, "miAddressDetectNext");
		this.miAddressDetectNext.Name = "miAddressDetectNext";
		this.miAddressDetectNext.Click += new System.EventHandler(tbAddressDetect_Click);
		this.toolStripMenuItem4.Name = "toolStripMenuItem4";
		resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
		resources.ApplyResources(this.miAddressFind, "miAddressFind");
		this.miAddressFind.Image = PaletteEditor.Properties.Resources.AddrFind;
		this.miAddressFind.Name = "miAddressFind";
		this.miAddressFind.Click += new System.EventHandler(miAddressFind_Click);
		resources.ApplyResources(this.miAddressFindPrev, "miAddressFindPrev");
		this.miAddressFindPrev.Image = PaletteEditor.Properties.Resources.AddrFindPrev;
		this.miAddressFindPrev.Name = "miAddressFindPrev";
		this.miAddressFindPrev.Click += new System.EventHandler(miAddressFind_Click);
		resources.ApplyResources(this.miAddressFindNext, "miAddressFindNext");
		this.miAddressFindNext.Image = PaletteEditor.Properties.Resources.AddrFindNext;
		this.miAddressFindNext.Name = "miAddressFindNext";
		this.miAddressFindNext.Click += new System.EventHandler(miAddressFind_Click);
		this.toolStripMenuItem2.Name = "toolStripMenuItem2";
		resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
		resources.ApplyResources(this.miAddressSettingFile, "miAddressSettingFile");
		this.miAddressSettingFile.Image = PaletteEditor.Properties.Resources.AddrJumpList;
		this.miAddressSettingFile.Name = "miAddressSettingFile";
		this.miType.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[7] { this.miType6, this.miType4, this.miType9, this.miType15, this.miType16, this.miType24, this.miType32 });
		this.miType.Name = "miType";
		resources.ApplyResources(this.miType, "miType");
		this.miType6.Image = PaletteEditor.Properties.Resources.PaletteType8bitNES;
		resources.ApplyResources(this.miType6, "miType6");
		this.miType6.Name = "miType6";
		this.miType6.Click += new System.EventHandler(miTypeSelect_Click);
		this.miType4.Image = PaletteEditor.Properties.Resources.PaletteType8bitMSX;
		resources.ApplyResources(this.miType4, "miType4");
		this.miType4.Name = "miType4";
		this.miType4.Click += new System.EventHandler(miTypeSelect_Click);
		this.miType9.Image = PaletteEditor.Properties.Resources.PaletteType9bit;
		resources.ApplyResources(this.miType9, "miType9");
		this.miType9.Name = "miType9";
		this.miType9.Click += new System.EventHandler(miTypeSelect_Click);
		this.miType15.Image = PaletteEditor.Properties.Resources.PaletteType15bit;
		resources.ApplyResources(this.miType15, "miType15");
		this.miType15.Name = "miType15";
		this.miType15.Click += new System.EventHandler(miTypeSelect_Click);
		this.miType16.Image = PaletteEditor.Properties.Resources.PaletteType16bit;
		resources.ApplyResources(this.miType16, "miType16");
		this.miType16.Name = "miType16";
		this.miType16.Click += new System.EventHandler(miTypeSelect_Click);
		this.miType24.Image = PaletteEditor.Properties.Resources.PaletteType24bit;
		resources.ApplyResources(this.miType24, "miType24");
		this.miType24.Name = "miType24";
		this.miType24.Click += new System.EventHandler(miTypeSelect_Click);
		this.miType32.Image = PaletteEditor.Properties.Resources.PaletteType32bit;
		resources.ApplyResources(this.miType32, "miType32");
		this.miType32.Name = "miType32";
		this.miType32.Click += new System.EventHandler(miTypeSelect_Click);
		this.miOption.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.settingToolStripMenuItem, this.toolStripMenuItem7, this.miOptionShowAddress, this.toolStripMenuItem8, this.miOptionCheckPAL });
		this.miOption.Name = "miOption";
		resources.ApplyResources(this.miOption, "miOption");
		this.settingToolStripMenuItem.Image = PaletteEditor.Properties.Resources.IconOptionSetting;
		resources.ApplyResources(this.settingToolStripMenuItem, "settingToolStripMenuItem");
		this.settingToolStripMenuItem.Name = "settingToolStripMenuItem";
		this.settingToolStripMenuItem.Click += new System.EventHandler(settingToolStripMenuItem_Click);
		this.toolStripMenuItem7.Name = "toolStripMenuItem7";
		resources.ApplyResources(this.toolStripMenuItem7, "toolStripMenuItem7");
		this.miOptionShowAddress.Image = PaletteEditor.Properties.Resources.IconOptionSetting;
		resources.ApplyResources(this.miOptionShowAddress, "miOptionShowAddress");
		this.miOptionShowAddress.Name = "miOptionShowAddress";
		this.miOptionShowAddress.Click += new System.EventHandler(miOptionShowAddress_Click);
		this.toolStripMenuItem8.Name = "toolStripMenuItem8";
		resources.ApplyResources(this.toolStripMenuItem8, "toolStripMenuItem8");
		this.miOptionCheckPAL.Image = PaletteEditor.Properties.Resources.IconNG;
		resources.ApplyResources(this.miOptionCheckPAL, "miOptionCheckPAL");
		this.miOptionCheckPAL.Name = "miOptionCheckPAL";
		this.miOptionCheckPAL.Click += new System.EventHandler(tbOptionCheckPAL_Click);
		this.miHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.miHelpPropertyEditor, this.toolStripMenuItem10, this.aboutToolStripMenuItem });
		this.miHelp.Name = "miHelp";
		resources.ApplyResources(this.miHelp, "miHelp");
		resources.ApplyResources(this.miHelpPropertyEditor, "miHelpPropertyEditor");
		this.miHelpPropertyEditor.Name = "miHelpPropertyEditor";
		this.miHelpPropertyEditor.Click += new System.EventHandler(ActionHelpPropertyEditor);
		this.toolStripMenuItem10.Name = "toolStripMenuItem10";
		resources.ApplyResources(this.toolStripMenuItem10, "toolStripMenuItem10");
		this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
		resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
		this.aboutToolStripMenuItem.Click += new System.EventHandler(aboutToolStripMenuItem_Click);
		this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.slAddress, this.slHint });
		resources.ApplyResources(this.statusStrip, "statusStrip");
		this.statusStrip.Name = "statusStrip";
		this.statusStrip.SizingGrip = false;
		this.slAddress.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.All;
		resources.ApplyResources(this.slAddress, "slAddress");
		this.slAddress.Margin = new System.Windows.Forms.Padding(1, 2, 1, 0);
		this.slAddress.Name = "slAddress";
		this.slHint.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.All;
		this.slHint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
		resources.ApplyResources(this.slHint, "slHint");
		this.slHint.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
		this.slHint.Name = "slHint";
		this.slHint.Spring = true;
		this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[31]
		{
			this.tbFileOpen, this.tbFileSave, this.tbFileReadonlyMode, this.toolStripSeparator1, this.tbEditUndo, this.tbEditRedo, this.toolStripSeparator7, this.tbEditCopy, this.tbEditPaste, this.toolStripSeparator6,
			this.tlType, this.tbType6, this.tbType4, this.tbType9, this.tbType15, this.tbType16, this.tbType24, this.tbType32, this.toolStripSeparator2, this.tbOptionCheckPAL,
			this.tbAddressP1Byte, this.tbAddressN1Byte, this.toolStripSeparator4, this.tlDetect, this.tbAddressDetectOnlyTable, this.tbAddressDetectPrev, this.tbAddressDetectNext, this.toolStripSeparator5, this.tbAddressFind, this.tbAddressFindPrev,
			this.tbAddressFindNext
		});
		resources.ApplyResources(this.toolStrip, "toolStrip");
		this.toolStrip.Name = "toolStrip";
		this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		this.tbFileOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbFileOpen.Image = PaletteEditor.Properties.Resources.FileOpen;
		resources.ApplyResources(this.tbFileOpen, "tbFileOpen");
		this.tbFileOpen.Name = "tbFileOpen";
		this.tbFileOpen.Click += new System.EventHandler(miFileOpen_Click);
		this.tbFileSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbFileSave.Image = PaletteEditor.Properties.Resources.FileSave;
		resources.ApplyResources(this.tbFileSave, "tbFileSave");
		this.tbFileSave.Name = "tbFileSave";
		this.tbFileSave.Click += new System.EventHandler(miFileSave_Click);
		this.tbFileReadonlyMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbFileReadonlyMode.Image = PaletteEditor.Properties.Resources.IconEditDisabled;
		resources.ApplyResources(this.tbFileReadonlyMode, "tbFileReadonlyMode");
		this.tbFileReadonlyMode.Name = "tbFileReadonlyMode";
		this.tbFileReadonlyMode.Click += new System.EventHandler(tbFileReadonlyMode_Click);
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
		this.tbEditUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbEditUndo.Image = PaletteEditor.Properties.Resources.EditUndo;
		resources.ApplyResources(this.tbEditUndo, "tbEditUndo");
		this.tbEditUndo.Name = "tbEditUndo";
		this.tbEditUndo.Click += new System.EventHandler(miEditUndo_Click);
		this.tbEditRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbEditRedo.Image = PaletteEditor.Properties.Resources.EditRedo;
		resources.ApplyResources(this.tbEditRedo, "tbEditRedo");
		this.tbEditRedo.Name = "tbEditRedo";
		this.tbEditRedo.Click += new System.EventHandler(miEditRedo_Click);
		this.toolStripSeparator7.Name = "toolStripSeparator7";
		resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
		this.tbEditCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbEditCopy.Image = PaletteEditor.Properties.Resources.EditCopy;
		resources.ApplyResources(this.tbEditCopy, "tbEditCopy");
		this.tbEditCopy.Name = "tbEditCopy";
		this.tbEditCopy.Click += new System.EventHandler(miEditCopy_Click);
		this.tbEditPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		resources.ApplyResources(this.tbEditPaste, "tbEditPaste");
		this.tbEditPaste.Image = PaletteEditor.Properties.Resources.EditPaste;
		this.tbEditPaste.Name = "tbEditPaste";
		this.tbEditPaste.Click += new System.EventHandler(miEditPaste_Click);
		this.toolStripSeparator6.Name = "toolStripSeparator6";
		resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
		this.tlType.Name = "tlType";
		resources.ApplyResources(this.tlType, "tlType");
		this.tbType6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbType6.Image = PaletteEditor.Properties.Resources.PaletteType8bitNES;
		resources.ApplyResources(this.tbType6, "tbType6");
		this.tbType6.Name = "tbType6";
		this.tbType6.Click += new System.EventHandler(miTypeSelect_Click);
		this.tbType4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbType4.Image = PaletteEditor.Properties.Resources.PaletteType8bitMSX;
		resources.ApplyResources(this.tbType4, "tbType4");
		this.tbType4.Name = "tbType4";
		this.tbType4.Tag = "";
		this.tbType4.Click += new System.EventHandler(miTypeSelect_Click);
		this.tbType9.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbType9.Image = PaletteEditor.Properties.Resources.PaletteType9bit;
		resources.ApplyResources(this.tbType9, "tbType9");
		this.tbType9.Name = "tbType9";
		this.tbType9.Click += new System.EventHandler(miTypeSelect_Click);
		this.tbType15.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbType15.Image = PaletteEditor.Properties.Resources.PaletteType15bit;
		resources.ApplyResources(this.tbType15, "tbType15");
		this.tbType15.Name = "tbType15";
		this.tbType15.Click += new System.EventHandler(miTypeSelect_Click);
		this.tbType16.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbType16.Image = PaletteEditor.Properties.Resources.PaletteType16bit;
		resources.ApplyResources(this.tbType16, "tbType16");
		this.tbType16.Name = "tbType16";
		this.tbType16.Click += new System.EventHandler(miTypeSelect_Click);
		this.tbType24.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbType24.Image = PaletteEditor.Properties.Resources.PaletteType24bit;
		resources.ApplyResources(this.tbType24, "tbType24");
		this.tbType24.Name = "tbType24";
		this.tbType24.Click += new System.EventHandler(miTypeSelect_Click);
		this.tbType32.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbType32.Image = PaletteEditor.Properties.Resources.PaletteType32bit;
		resources.ApplyResources(this.tbType32, "tbType32");
		this.tbType32.Name = "tbType32";
		this.tbType32.Click += new System.EventHandler(miTypeSelect_Click);
		this.toolStripSeparator2.Name = "toolStripSeparator2";
		resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
		this.tbOptionCheckPAL.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbOptionCheckPAL.Image = PaletteEditor.Properties.Resources.IconNG;
		resources.ApplyResources(this.tbOptionCheckPAL, "tbOptionCheckPAL");
		this.tbOptionCheckPAL.Name = "tbOptionCheckPAL";
		this.tbOptionCheckPAL.Click += new System.EventHandler(tbOptionCheckPAL_Click);
		this.tbAddressP1Byte.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddressP1Byte.Image = PaletteEditor.Properties.Resources.Addr4;
		resources.ApplyResources(this.tbAddressP1Byte, "tbAddressP1Byte");
		this.tbAddressP1Byte.Name = "tbAddressP1Byte";
		this.tbAddressP1Byte.Click += new System.EventHandler(miAddress_Click);
		this.tbAddressN1Byte.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddressN1Byte.Image = PaletteEditor.Properties.Resources.Addr5;
		resources.ApplyResources(this.tbAddressN1Byte, "tbAddressN1Byte");
		this.tbAddressN1Byte.Name = "tbAddressN1Byte";
		this.tbAddressN1Byte.Click += new System.EventHandler(miAddress_Click);
		this.toolStripSeparator4.Name = "toolStripSeparator4";
		resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
		this.tlDetect.Name = "tlDetect";
		resources.ApplyResources(this.tlDetect, "tlDetect");
		this.tbAddressDetectOnlyTable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddressDetectOnlyTable.Image = PaletteEditor.Properties.Resources.IconOptionSetting;
		resources.ApplyResources(this.tbAddressDetectOnlyTable, "tbAddressDetectOnlyTable");
		this.tbAddressDetectOnlyTable.Name = "tbAddressDetectOnlyTable";
		this.tbAddressDetectOnlyTable.Click += new System.EventHandler(tbAddressDetectOnlyTable_Click);
		this.tbAddressDetectPrev.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddressDetectPrev.Image = PaletteEditor.Properties.Resources.AddrDetecPrev;
		resources.ApplyResources(this.tbAddressDetectPrev, "tbAddressDetectPrev");
		this.tbAddressDetectPrev.Name = "tbAddressDetectPrev";
		this.tbAddressDetectPrev.Click += new System.EventHandler(tbAddressDetect_Click);
		this.tbAddressDetectNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddressDetectNext.Image = PaletteEditor.Properties.Resources.AddrDetectNext;
		resources.ApplyResources(this.tbAddressDetectNext, "tbAddressDetectNext");
		this.tbAddressDetectNext.Name = "tbAddressDetectNext";
		this.tbAddressDetectNext.Click += new System.EventHandler(tbAddressDetect_Click);
		this.toolStripSeparator5.Name = "toolStripSeparator5";
		resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
		this.tbAddressFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddressFind.Image = PaletteEditor.Properties.Resources.AddrFind;
		resources.ApplyResources(this.tbAddressFind, "tbAddressFind");
		this.tbAddressFind.Name = "tbAddressFind";
		this.tbAddressFind.Click += new System.EventHandler(miAddressFind_Click);
		this.tbAddressFindPrev.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddressFindPrev.Image = PaletteEditor.Properties.Resources.AddrFindPrev;
		resources.ApplyResources(this.tbAddressFindPrev, "tbAddressFindPrev");
		this.tbAddressFindPrev.Name = "tbAddressFindPrev";
		this.tbAddressFindPrev.Click += new System.EventHandler(miAddressFind_Click);
		this.tbAddressFindNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddressFindNext.Image = PaletteEditor.Properties.Resources.AddrFindNext;
		resources.ApplyResources(this.tbAddressFindNext, "tbAddressFindNext");
		this.tbAddressFindNext.Name = "tbAddressFindNext";
		this.tbAddressFindNext.Click += new System.EventHandler(miAddressFind_Click);
		this.openFileDialog.FileName = "openFileDialog1";
		this.panelEditorIndex.Controls.Add(this.paletteSelectorSourcePal);
		resources.ApplyResources(this.panelEditorIndex, "panelEditorIndex");
		this.panelEditorIndex.Name = "panelEditorIndex";
		this.panelEditorIndex.MouseEnter += new System.EventHandler(hintControl_MouseEnter);
		this.panelEditorIndex.MouseLeave += new System.EventHandler(hintControl_MouseLeave);
		this.paletteSelectorSourcePal.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.paletteSelectorSourcePal.CellColumnCount = 16;
		this.paletteSelectorSourcePal.CellColumnView = 16;
		this.paletteSelectorSourcePal.CellHeight = 16;
		this.paletteSelectorSourcePal.CellRowCount = 16;
		this.paletteSelectorSourcePal.CellRowView = 16;
		this.paletteSelectorSourcePal.CellWidth = 16;
		this.paletteSelectorSourcePal.ColorCount = 256;
		this.paletteSelectorSourcePal.DrawMultiSelect = false;
		this.paletteSelectorSourcePal.EnableMultiSelect = false;
		this.paletteSelectorSourcePal.EnableSelectM = false;
		this.paletteSelectorSourcePal.EnableSelectR = false;
		this.paletteSelectorSourcePal.EnableSetUnknownMarkR = false;
		resources.ApplyResources(this.paletteSelectorSourcePal, "paletteSelectorSourcePal");
		this.paletteSelectorSourcePal.Label = new string[256];
		this.paletteSelectorSourcePal.LabelItem = ControlLib.LabelItem.Index;
		this.paletteSelectorSourcePal.LabelStyle = ControlLib.LabelStyle.SelectedAll;
		this.paletteSelectorSourcePal.Name = "paletteSelectorSourcePal";
		this.paletteSelectorSourcePal.Palette = new System.Drawing.Color[256]
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
		this.paletteSelectorSourcePal.PaletteFlags = new byte[256];
		this.paletteSelectorSourcePal.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.paletteSelectorSourcePal.SelectedIndex = 0;
		this.paletteSelectorSourcePal.SelectEndIndex = 1;
		this.paletteSelectorSourcePal.SetSize = 0;
		this.paletteSelectorSourcePal.ShowSetRect = true;
		this.paletteSelectorSourcePal.OnPaletteSelected += new ControlLib.PaletteSelector.OnPaletteSelectedHandler(paletteSelectorSourcePal_OnPaletteSelected);
		this.panelEditorRGB.Controls.Add(this.labelHexValue);
		this.panelEditorRGB.Controls.Add(this.textBoxHexBit);
		this.panelEditorRGB.Controls.Add(this.panelPreview);
		this.panelEditorRGB.Controls.Add(this.textBoxHex);
		this.panelEditorRGB.Controls.Add(this.rgbEditor1);
		this.panelEditorRGB.Controls.Add(this.buttonReset);
		this.panelEditorRGB.Controls.Add(this.buttonOK);
		resources.ApplyResources(this.panelEditorRGB, "panelEditorRGB");
		this.panelEditorRGB.Name = "panelEditorRGB";
		this.panelEditorRGB.MouseEnter += new System.EventHandler(hintControl_MouseEnter);
		this.panelEditorRGB.MouseLeave += new System.EventHandler(hintControl_MouseLeave);
		resources.ApplyResources(this.labelHexValue, "labelHexValue");
		this.labelHexValue.Name = "labelHexValue";
		resources.ApplyResources(this.textBoxHexBit, "textBoxHexBit");
		this.textBoxHexBit.Name = "textBoxHexBit";
		this.textBoxHexBit.ReadOnly = true;
		this.panelPreview.BackColor = System.Drawing.Color.Black;
		this.panelPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		resources.ApplyResources(this.panelPreview, "panelPreview");
		this.panelPreview.Name = "panelPreview";
		this.panelPreview.Paint += new System.Windows.Forms.PaintEventHandler(panelPreview_Paint);
		resources.ApplyResources(this.textBoxHex, "textBoxHex");
		this.textBoxHex.Name = "textBoxHex";
		this.textBoxHex.TextChanged += new System.EventHandler(textBoxHex_TextChanged);
		this.rgbEditor1.BackColor = System.Drawing.Color.Black;
		this.rgbEditor1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		resources.ApplyResources(this.rgbEditor1, "rgbEditor1");
		this.rgbEditor1.Name = "rgbEditor1";
		this.rgbEditor1.ShowAlpha = true;
		this.rgbEditor1.ColorChanged += new System.EventHandler(rgbEditor1_ColorChanged);
		this.rgbEditor1.MouseEnter += new System.EventHandler(hintControl_MouseEnter);
		this.rgbEditor1.MouseLeave += new System.EventHandler(hintControl_MouseLeave);
		resources.ApplyResources(this.buttonReset, "buttonReset");
		this.buttonReset.Name = "buttonReset";
		this.buttonReset.UseVisualStyleBackColor = true;
		this.buttonReset.Click += new System.EventHandler(buttonReset_Click);
		resources.ApplyResources(this.buttonOK, "buttonOK");
		this.buttonOK.Name = "buttonOK";
		this.buttonOK.UseVisualStyleBackColor = true;
		this.buttonOK.Click += new System.EventHandler(buttonOK_Click);
		this.panelToolStripAddress.Controls.Add(this.toolStripAddress);
		resources.ApplyResources(this.panelToolStripAddress, "panelToolStripAddress");
		this.panelToolStripAddress.Name = "panelToolStripAddress";
		resources.ApplyResources(this.toolStripAddress, "toolStripAddress");
		this.toolStripAddress.CanOverflow = false;
		this.toolStripAddress.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		this.toolStripAddress.Items.AddRange(new System.Windows.Forms.ToolStripItem[12]
		{
			this.tbAddres0, this.tbAddres1, this.tbAddres2, this.tbAddres3, this.tbAddres4, this.tbAddres5, this.tbAddres6, this.tbAddres7, this.tbAddres8, this.tbAddres9,
			this.tbAddresSep0, this.tbAddresInputAddress
		});
		this.toolStripAddress.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
		this.toolStripAddress.Name = "toolStripAddress";
		this.toolStripAddress.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		resources.ApplyResources(this.tbAddres0, "tbAddres0");
		this.tbAddres0.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddres0.Image = PaletteEditor.Properties.Resources.Addr0;
		this.tbAddres0.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddres0.Name = "tbAddres0";
		this.tbAddres0.Tag = "0";
		this.tbAddres0.Click += new System.EventHandler(miAddress_Click);
		resources.ApplyResources(this.tbAddres1, "tbAddres1");
		this.tbAddres1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddres1.Image = PaletteEditor.Properties.Resources.Addr1;
		this.tbAddres1.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddres1.Name = "tbAddres1";
		this.tbAddres1.Tag = "1";
		this.tbAddres1.Click += new System.EventHandler(miAddress_Click);
		resources.ApplyResources(this.tbAddres2, "tbAddres2");
		this.tbAddres2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddres2.Image = PaletteEditor.Properties.Resources.Addr2;
		this.tbAddres2.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddres2.Name = "tbAddres2";
		this.tbAddres2.Tag = "2";
		this.tbAddres2.Click += new System.EventHandler(miAddress_Click);
		resources.ApplyResources(this.tbAddres3, "tbAddres3");
		this.tbAddres3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddres3.Image = PaletteEditor.Properties.Resources.Addr3;
		this.tbAddres3.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddres3.Name = "tbAddres3";
		this.tbAddres3.Tag = "3";
		this.tbAddres3.Click += new System.EventHandler(miAddress_Click);
		resources.ApplyResources(this.tbAddres4, "tbAddres4");
		this.tbAddres4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddres4.Image = PaletteEditor.Properties.Resources.Addr4;
		this.tbAddres4.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddres4.Name = "tbAddres4";
		this.tbAddres4.Tag = "4";
		this.tbAddres4.Click += new System.EventHandler(miAddress_Click);
		resources.ApplyResources(this.tbAddres5, "tbAddres5");
		this.tbAddres5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddres5.Image = PaletteEditor.Properties.Resources.Addr5;
		this.tbAddres5.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddres5.Name = "tbAddres5";
		this.tbAddres5.Tag = "5";
		this.tbAddres5.Click += new System.EventHandler(miAddress_Click);
		resources.ApplyResources(this.tbAddres6, "tbAddres6");
		this.tbAddres6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddres6.Image = PaletteEditor.Properties.Resources.Addr6;
		this.tbAddres6.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddres6.Name = "tbAddres6";
		this.tbAddres6.Tag = "6";
		this.tbAddres6.Click += new System.EventHandler(miAddress_Click);
		resources.ApplyResources(this.tbAddres7, "tbAddres7");
		this.tbAddres7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddres7.Image = PaletteEditor.Properties.Resources.Addr7;
		this.tbAddres7.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddres7.Name = "tbAddres7";
		this.tbAddres7.Tag = "7";
		this.tbAddres7.Click += new System.EventHandler(miAddress_Click);
		resources.ApplyResources(this.tbAddres8, "tbAddres8");
		this.tbAddres8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddres8.Image = PaletteEditor.Properties.Resources.Addr8;
		this.tbAddres8.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddres8.Name = "tbAddres8";
		this.tbAddres8.Tag = "8";
		this.tbAddres8.Click += new System.EventHandler(miAddress_Click);
		resources.ApplyResources(this.tbAddres9, "tbAddres9");
		this.tbAddres9.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddres9.Image = PaletteEditor.Properties.Resources.Addr9;
		this.tbAddres9.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddres9.Name = "tbAddres9";
		this.tbAddres9.Tag = "9";
		this.tbAddres9.Click += new System.EventHandler(miAddress_Click);
		resources.ApplyResources(this.tbAddresSep0, "tbAddresSep0");
		this.tbAddresSep0.Name = "tbAddresSep0";
		resources.ApplyResources(this.tbAddresInputAddress, "tbAddresInputAddress");
		this.tbAddresInputAddress.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbAddresInputAddress.Image = PaletteEditor.Properties.Resources.AddrA;
		this.tbAddresInputAddress.Margin = new System.Windows.Forms.Padding(0);
		this.tbAddresInputAddress.Name = "tbAddresInputAddress";
		this.tbAddresInputAddress.Tag = "10";
		this.tbAddresInputAddress.Click += new System.EventHandler(miAddressInput_Click);
		resources.ApplyResources(this.lVersion, "lVersion");
		this.lVersion.ForeColor = System.Drawing.Color.Red;
		this.lVersion.Name = "lVersion";
		this.scrollPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.scrollPanel.ClientAreaSize = new System.Drawing.Size(316, 256);
		this.scrollPanel.Controls.Add(this.panelAddress);
		this.scrollPanel.Controls.Add(this.paletteSelectorFile);
		this.scrollPanel.LargeChange = 10;
		resources.ApplyResources(this.scrollPanel, "scrollPanel");
		this.scrollPanel.LrChange = 1;
		this.scrollPanel.Maximum = 100;
		this.scrollPanel.Minimum = 0;
		this.scrollPanel.Name = "scrollPanel";
		this.scrollPanel.ScrollBarType = ControlLib.ScrollPanel.ScrollBarTypes.Vertical;
		this.scrollPanel.SmallChange = 1;
		this.scrollPanel.Value = 0;
		this.scrollPanel.WheelRate = 4;
		this.scrollPanel.Scrolled += new System.EventHandler(scrollPanel_Scrolled);
		this.panelAddress.BackColor = System.Drawing.Color.Black;
		resources.ApplyResources(this.panelAddress, "panelAddress");
		this.panelAddress.ForeColor = System.Drawing.Color.White;
		this.panelAddress.Name = "panelAddress";
		this.panelAddress.Paint += new System.Windows.Forms.PaintEventHandler(panelAddress_Paint);
		this.paletteSelectorFile.BackColor = System.Drawing.Color.Black;
		this.paletteSelectorFile.CellColumnCount = 16;
		this.paletteSelectorFile.CellColumnView = 16;
		this.paletteSelectorFile.CellHeight = 16;
		this.paletteSelectorFile.CellRowCount = 16;
		this.paletteSelectorFile.CellRowView = 16;
		this.paletteSelectorFile.CellWidth = 16;
		this.paletteSelectorFile.ColorCount = 256;
		this.paletteSelectorFile.DrawMultiSelect = false;
		this.paletteSelectorFile.EnableMultiSelect = true;
		this.paletteSelectorFile.EnableSelectM = false;
		this.paletteSelectorFile.EnableSelectR = true;
		this.paletteSelectorFile.EnableSetUnknownMarkR = false;
		resources.ApplyResources(this.paletteSelectorFile, "paletteSelectorFile");
		this.paletteSelectorFile.Label = new string[256];
		this.paletteSelectorFile.LabelItem = ControlLib.LabelItem.Index;
		this.paletteSelectorFile.LabelStyle = ControlLib.LabelStyle.None;
		this.paletteSelectorFile.Name = "paletteSelectorFile";
		this.paletteSelectorFile.Palette = new System.Drawing.Color[256]
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
		this.paletteSelectorFile.PaletteFlags = new byte[256];
		this.paletteSelectorFile.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.paletteSelectorFile.SelectedIndex = 0;
		this.paletteSelectorFile.SelectEndIndex = 1;
		this.paletteSelectorFile.SetSize = 0;
		this.paletteSelectorFile.ShowSetRect = true;
		this.paletteSelectorFile.OnPaletteSelect += new ControlLib.PaletteSelector.OnPaletteSelectHandler(paletteSelectorFile_OnPaletteSelect);
		this.paletteSelectorFile.MouseEnter += new System.EventHandler(hintControl_MouseEnter);
		this.paletteSelectorFile.MouseLeave += new System.EventHandler(hintControl_MouseLeave);
		this.AllowDrop = true;
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.lVersion);
		base.Controls.Add(this.panelEditorRGB);
		base.Controls.Add(this.panelToolStripAddress);
		base.Controls.Add(this.panelEditorIndex);
		base.Controls.Add(this.scrollPanel);
		base.Controls.Add(this.toolStrip);
		base.Controls.Add(this.statusStrip);
		base.Controls.Add(this.menuStrip);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MainMenuStrip = this.menuStrip;
		base.MaximizeBox = false;
		base.Name = "PaletteEditorMainForm";
		base.Activated += new System.EventHandler(PaletteEditorMainForm_Activated);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(PaletteEditorMainForm_FormClosing);
		base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(PaletteEditorMainForm_FormClosed);
		base.DragDrop += new System.Windows.Forms.DragEventHandler(PaletteEditorMainForm_DragDrop);
		base.DragEnter += new System.Windows.Forms.DragEventHandler(PaletteEditorMainForm_DragEnter);
		this.menuStrip.ResumeLayout(false);
		this.menuStrip.PerformLayout();
		this.statusStrip.ResumeLayout(false);
		this.statusStrip.PerformLayout();
		this.toolStrip.ResumeLayout(false);
		this.toolStrip.PerformLayout();
		this.panelEditorIndex.ResumeLayout(false);
		this.panelEditorRGB.ResumeLayout(false);
		this.panelEditorRGB.PerformLayout();
		this.panelToolStripAddress.ResumeLayout(false);
		this.toolStripAddress.ResumeLayout(false);
		this.toolStripAddress.PerformLayout();
		this.scrollPanel.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
