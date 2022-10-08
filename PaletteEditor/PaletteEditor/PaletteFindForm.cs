using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using CharactorLib;
using ControlLib;

namespace PaletteEditor;

public class PaletteFindForm : Form
{
	private const int FIND_PAL_NUM = 4;

	private byte[] mPalDataIndex = new byte[4];

	private ColorBit[] mPalDataRgb = new ColorBit[4];

	private IContainer components;

	private Button buttonFindPrev;

	private Button buttonFindNext;

	private Button buttonClose;

	private PaletteSelector paletteSelectorFind;

	private Label labelLeft;

	private Label labelRight;

	private PaletteSelector paletteSelectorSrcPal;

	private RGBEditor rgbEditor;

	public byte[] FindData { get; set; }

	public bool[] FindDataEnabled { get; set; }

	public int DirectionDelta { get; set; } = 1;


	internal PaletteType PaletteType { get; set; } = PaletteType.IndexedNes;


	private bool IsIndexedPalette
	{
		get
		{
			bool result = false;
			if (PaletteType == PaletteType.Indexed)
			{
				result = true;
			}
			if (PaletteType == PaletteType.IndexedNes)
			{
				result = true;
			}
			if (PaletteType == PaletteType.IndexedMsx)
			{
				result = true;
			}
			return result;
		}
	}

	public Color[] IndexedPalette { get; set; }

	public PaletteFindForm()
	{
		InitializeComponent();
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!base.DesignMode)
		{
			int colorSizeFromSelectedPaletteType = GetColorSizeFromSelectedPaletteType();
			int num = 4 * colorSizeFromSelectedPaletteType;
			if (FindData == null || FindData.Length != num)
			{
				FindData = new byte[num];
			}
			int num2 = mPalDataRgb.Length;
			for (int i = 0; i < num2; i++)
			{
				if (mPalDataRgb[i] == null)
				{
					mPalDataRgb[i] = new ColorBit(PaletteType, byte.MaxValue, 0, 0, 0);
				}
				else
				{
					mPalDataRgb[i].SetPaletteType(PaletteType);
				}
			}
			int num3 = num2 % 16;
			int num4 = num2 / 16;
			if (num3 > 0)
			{
				num4++;
			}
			if (num2 >= 16)
			{
				num3 = 16;
			}
			paletteSelectorFind.CellColumnCount = num3;
			paletteSelectorFind.CellColumnView = num3;
			paletteSelectorFind.CellRowCount = num4;
			paletteSelectorFind.CellRowView = num4;
			if (IsIndexedPalette)
			{
				paletteSelectorFind.LabelStyle = LabelStyle.All;
			}
			else
			{
				paletteSelectorFind.LabelStyle = LabelStyle.None;
			}
		}
		SetFormDataFromProperty();
	}

	private void buttonClose_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
	}

	private void buttonFindPrev_Click(object sender, EventArgs e)
	{
		SetPropertyFromFormData();
		DirectionDelta = -1;
		base.DialogResult = DialogResult.OK;
	}

	private void buttonFindNext_Click(object sender, EventArgs e)
	{
		SetPropertyFromFormData();
		DirectionDelta = 1;
		base.DialogResult = DialogResult.OK;
	}

	private void UpdatePaletteFind()
	{
		Color[] array = new Color[256];
		string[] array2 = new string[256];
		if (IsIndexedPalette)
		{
			if (IndexedPalette == null || IndexedPalette.Length < 256)
			{
				return;
			}
			for (int i = 0; i < mPalDataIndex.Length; i++)
			{
				int num = mPalDataIndex[i];
				array[i] = IndexedPalette[num];
				array2[i] = num.ToString("X2");
			}
		}
		else
		{
			for (int j = 0; j < mPalDataRgb.Length; j++)
			{
				array[j] = mPalDataRgb[j].Color;
				array2[j] = "";
			}
		}
		for (int k = 0; k < array.Length; k++)
		{
			_ = ref array[k];
		}
		paletteSelectorFind.Palette = array;
		paletteSelectorFind.Label = array2;
		paletteSelectorFind.Refresh();
	}

	private void paletteSelectorFind_OnPaletteSelect(object sender, EventArgs e)
	{
		SetEditor();
	}

	private void SetEditor()
	{
		bool isIndexedPalette = IsIndexedPalette;
		paletteSelectorSrcPal.Visible = isIndexedPalette;
		rgbEditor.Visible = !isIndexedPalette;
		if (isIndexedPalette)
		{
			if (IndexedPalette != null && paletteSelectorSrcPal.Palette != IndexedPalette)
			{
				paletteSelectorSrcPal.Palette = IndexedPalette;
			}
			try
			{
				int selectedIndex = paletteSelectorFind.SelectedIndex;
				int selectedIndex2 = mPalDataIndex[selectedIndex];
				paletteSelectorSrcPal.SelectedIndex = selectedIndex2;
				paletteSelectorSrcPal.Refresh();
				return;
			}
			catch
			{
				return;
			}
		}
		try
		{
			int selectedIndex3 = paletteSelectorFind.SelectedIndex;
			rgbEditor.SetColorBitInstance(mPalDataRgb[selectedIndex3]);
		}
		catch
		{
		}
	}

	private void paletteSelectorSrcPal_OnPaletteSelect(object sender, EventArgs e)
	{
		int selectedIndex = paletteSelectorFind.SelectedIndex;
		int selectedIndex2 = paletteSelectorSrcPal.SelectedIndex;
		mPalDataIndex[selectedIndex] = (byte)selectedIndex2;
		UpdatePaletteFind();
	}

	private void rgbEditor_ColorChanged(object sender, EventArgs e)
	{
		rgbEditor.Refresh();
		int selectedIndex = paletteSelectorFind.SelectedIndex;
		mPalDataRgb[selectedIndex].CopyColorFrom(rgbEditor.ColorBit);
		UpdatePaletteFind();
	}

	private void SetPropertyFromFormData()
	{
		int num = 4;
		int colorSizeFromSelectedPaletteType = GetColorSizeFromSelectedPaletteType();
		byte[] array = new byte[num * colorSizeFromSelectedPaletteType];
		if (IsIndexedPalette)
		{
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				array[num2] = mPalDataIndex[i];
				num2++;
			}
		}
		else
		{
			int num3 = 0;
			for (int j = 0; j < num; j++)
			{
				byte[] byteData = mPalDataRgb[j].GetByteData();
				for (int k = 0; k < byteData.Length; k++)
				{
					array[num3] = byteData[k];
					num3++;
				}
			}
		}
		FindData = array;
		bool[] array2 = new bool[num];
		for (int l = 0; l < num; l++)
		{
			if (l >= 0 && l < paletteSelectorFind.PaletteFlags.Length)
			{
				array2[l] = (paletteSelectorFind.PaletteFlags[l] & 2) == 0;
			}
		}
		FindDataEnabled = array2;
	}

	private void SetFormDataFromProperty()
	{
		int num = 4;
		int colorSizeFromSelectedPaletteType = GetColorSizeFromSelectedPaletteType();
		byte[] array = new byte[num * colorSizeFromSelectedPaletteType];
		if (FindData != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (i >= 0 && i < FindData.Length)
				{
					array[i] = FindData[i];
				}
			}
		}
		if (IsIndexedPalette)
		{
			mPalDataIndex = array;
		}
		else
		{
			mPalDataRgb = ColorBit.FromDatas(array, 0, PaletteType, num);
		}
		byte[] array2 = new byte[num];
		if (FindDataEnabled != null)
		{
			for (int j = 0; j < num; j++)
			{
				if (j >= 0 && j < FindDataEnabled.Length)
				{
					array2[j] = (byte)((!FindDataEnabled[j]) ? 2u : 0u);
				}
			}
		}
		paletteSelectorFind.PaletteFlags = array2;
		SetEditor();
		UpdatePaletteFind();
	}

	private byte GetColorSizeFromSelectedPaletteType()
	{
		return ColorBit.GetByteSizeFromPaletteType(PaletteType);
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if (e.KeyData == (Keys.C | Keys.Control))
		{
			SetPropertyFromFormData();
			CopyBinaryToClipboard();
		}
		if (e.KeyData == (Keys.V | Keys.Control))
		{
			GetBinaryFromClipboard();
		}
	}

	private void CopyBinaryToClipboard()
	{
		byte[] findData = FindData;
		bool[] findDataEnabled = FindDataEnabled;
		int colorSizeFromSelectedPaletteType = GetColorSizeFromSelectedPaletteType();
		string text = "";
		if (findData != null)
		{
			for (int i = 0; i < findData.Length; i++)
			{
				bool flag = true;
				int num = i / colorSizeFromSelectedPaletteType;
				flag = num < 0 || num >= findDataEnabled.Length || findDataEnabled[num];
				text = ((!flag) ? (text + "?? ") : (text + findData[i].ToString("X2") + " "));
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

	private void GetBinaryFromClipboard()
	{
		string text = null;
		try
		{
			text = Clipboard.GetText();
		}
		catch
		{
			SystemSounds.Beep.Play();
		}
		List<short> list = new List<short>();
		if (!string.IsNullOrWhiteSpace(text))
		{
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
				foreach (string text2 in array)
				{
					byte b = 0;
					if (text2 == "??")
					{
						list.Add(-1);
						continue;
					}
					try
					{
						b = Convert.ToByte(text2, 16);
						list.Add(b);
					}
					catch
					{
						list.Clear();
						break;
					}
				}
			}
		}
		int byteSizeFromPaletteType = ColorBit.GetByteSizeFromPaletteType(PaletteType);
		int num = paletteSelectorFind.SelectedIndex * byteSizeFromPaletteType;
		int selectedIndex = paletteSelectorFind.SelectedIndex;
		for (int j = 0; j < list.Count; j++)
		{
			short num2 = list[j];
			int num3 = j + num;
			int num4 = j / byteSizeFromPaletteType + selectedIndex;
			if (num3 < 0 || num3 >= FindData.Length)
			{
				continue;
			}
			byte b2;
			if (num2 < 0 || num2 >= 256)
			{
				b2 = 0;
				if (num4 >= 0 && num4 < FindDataEnabled.Length)
				{
					FindDataEnabled[num4] = false;
				}
			}
			else
			{
				b2 = (byte)num2;
			}
			FindData[num3] = b2;
		}
		SetFormDataFromProperty();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PaletteEditor.PaletteFindForm));
		this.buttonFindPrev = new System.Windows.Forms.Button();
		this.buttonFindNext = new System.Windows.Forms.Button();
		this.buttonClose = new System.Windows.Forms.Button();
		this.labelLeft = new System.Windows.Forms.Label();
		this.labelRight = new System.Windows.Forms.Label();
		this.rgbEditor = new ControlLib.RGBEditor();
		this.paletteSelectorSrcPal = new ControlLib.PaletteSelector();
		this.paletteSelectorFind = new ControlLib.PaletteSelector();
		base.SuspendLayout();
		resources.ApplyResources(this.buttonFindPrev, "buttonFindPrev");
		this.buttonFindPrev.Name = "buttonFindPrev";
		this.buttonFindPrev.UseVisualStyleBackColor = true;
		this.buttonFindPrev.Click += new System.EventHandler(buttonFindPrev_Click);
		resources.ApplyResources(this.buttonFindNext, "buttonFindNext");
		this.buttonFindNext.Name = "buttonFindNext";
		this.buttonFindNext.UseVisualStyleBackColor = true;
		this.buttonFindNext.Click += new System.EventHandler(buttonFindNext_Click);
		this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		resources.ApplyResources(this.buttonClose, "buttonClose");
		this.buttonClose.Name = "buttonClose";
		this.buttonClose.UseVisualStyleBackColor = true;
		this.buttonClose.Click += new System.EventHandler(buttonClose_Click);
		resources.ApplyResources(this.labelLeft, "labelLeft");
		this.labelLeft.Name = "labelLeft";
		resources.ApplyResources(this.labelRight, "labelRight");
		this.labelRight.Name = "labelRight";
		this.rgbEditor.BackColor = System.Drawing.Color.Black;
		this.rgbEditor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		resources.ApplyResources(this.rgbEditor, "rgbEditor");
		this.rgbEditor.Name = "rgbEditor";
		this.rgbEditor.ShowAlpha = true;
		this.rgbEditor.ColorChanged += new System.EventHandler(rgbEditor_ColorChanged);
		this.paletteSelectorSrcPal.BackColor = System.Drawing.Color.Black;
		this.paletteSelectorSrcPal.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.paletteSelectorSrcPal.CellColumnCount = 16;
		this.paletteSelectorSrcPal.CellColumnView = 16;
		this.paletteSelectorSrcPal.CellHeight = 16;
		this.paletteSelectorSrcPal.CellRowCount = 16;
		this.paletteSelectorSrcPal.CellRowView = 4;
		this.paletteSelectorSrcPal.CellWidth = 16;
		this.paletteSelectorSrcPal.ColorCount = 256;
		this.paletteSelectorSrcPal.EnableMultiSelect = false;
		this.paletteSelectorSrcPal.EnableSelectM = false;
		this.paletteSelectorSrcPal.EnableSelectR = true;
		this.paletteSelectorSrcPal.EnableSetUnknownMarkR = false;
		resources.ApplyResources(this.paletteSelectorSrcPal, "paletteSelectorSrcPal");
		this.paletteSelectorSrcPal.Label = new string[256];
		this.paletteSelectorSrcPal.LabelItem = ControlLib.LabelItem.Index;
		this.paletteSelectorSrcPal.LabelStyle = ControlLib.LabelStyle.Selected;
		this.paletteSelectorSrcPal.Name = "paletteSelectorSrcPal";
		this.paletteSelectorSrcPal.Palette = new System.Drawing.Color[256]
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
		this.paletteSelectorSrcPal.PaletteFlags = new byte[256];
		this.paletteSelectorSrcPal.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.paletteSelectorSrcPal.SelectedIndex = 0;
		this.paletteSelectorSrcPal.SelectEndIndex = 1;
		this.paletteSelectorSrcPal.SetSize = 0;
		this.paletteSelectorSrcPal.ShowSetRect = true;
		this.paletteSelectorSrcPal.OnPaletteSelect += new ControlLib.PaletteSelector.OnPaletteSelectHandler(paletteSelectorSrcPal_OnPaletteSelect);
		this.paletteSelectorFind.BackColor = System.Drawing.Color.Black;
		this.paletteSelectorFind.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.paletteSelectorFind.CellColumnCount = 4;
		this.paletteSelectorFind.CellColumnView = 4;
		this.paletteSelectorFind.CellHeight = 16;
		this.paletteSelectorFind.CellRowCount = 1;
		this.paletteSelectorFind.CellRowView = 1;
		this.paletteSelectorFind.CellWidth = 16;
		this.paletteSelectorFind.ColorCount = 256;
		this.paletteSelectorFind.EnableMultiSelect = false;
		this.paletteSelectorFind.EnableSelectM = false;
		this.paletteSelectorFind.EnableSelectR = false;
		this.paletteSelectorFind.EnableSetUnknownMarkR = true;
		resources.ApplyResources(this.paletteSelectorFind, "paletteSelectorFind");
		this.paletteSelectorFind.Label = new string[256];
		this.paletteSelectorFind.LabelItem = ControlLib.LabelItem.LabelsProperty;
		this.paletteSelectorFind.LabelStyle = ControlLib.LabelStyle.All;
		this.paletteSelectorFind.Name = "paletteSelectorFind";
		this.paletteSelectorFind.Palette = new System.Drawing.Color[256]
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
		this.paletteSelectorFind.PaletteFlags = new byte[256];
		this.paletteSelectorFind.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.paletteSelectorFind.SelectedIndex = 0;
		this.paletteSelectorFind.SelectEndIndex = 1;
		this.paletteSelectorFind.SetSize = 0;
		this.paletteSelectorFind.ShowSetRect = true;
		this.paletteSelectorFind.OnPaletteSelect += new ControlLib.PaletteSelector.OnPaletteSelectHandler(paletteSelectorFind_OnPaletteSelect);
		base.AcceptButton = this.buttonFindNext;
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.buttonClose;
		base.Controls.Add(this.rgbEditor);
		base.Controls.Add(this.paletteSelectorSrcPal);
		base.Controls.Add(this.labelRight);
		base.Controls.Add(this.labelLeft);
		base.Controls.Add(this.paletteSelectorFind);
		base.Controls.Add(this.buttonClose);
		base.Controls.Add(this.buttonFindNext);
		base.Controls.Add(this.buttonFindPrev);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.KeyPreview = true;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "PaletteFindForm";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
