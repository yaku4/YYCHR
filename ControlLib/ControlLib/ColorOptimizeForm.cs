using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using CharactorLib.Common;

namespace ControlLib;

public class ColorOptimizeForm : Form
{
	private Bitmap tmpBmpIn;

	private Bitmap tmpBmpOut;

	private bool[] disabledPalette = new bool[256];

	private List<ColorCount> mColorList = new List<ColorCount>();

	private IContainer components;

	private Label labelClipboard;

	private Label labelPreview;

	private CheckBox cbClipImageColorReduce;

	private Panel panel1;

	private CheckBox cbGroupMinorSimilerColor;

	private CheckBox cbOrderByFreq;

	private CheckBox cbSetPalNum;

	private TrackBar tbPalNum;

	private TrackBar tbCbBpp;

	private Button buttonOK;

	private Button buttonCancel;

	private PaletteSelector paletteSelectorOut;

	private Label labelHintPalette;

	private Label labelColorCount4;

	private Label labelColorCount3;

	private Label labelColorCount2;

	private Label labelColorCount1;

	private PaletteSelector paletteSelectorIn;

	private PicturePanel panelImageIn;

	private PicturePanel panelImageOut;

	private Label labelManualDisablePalette;

	private Label labelColorCount5;

	private ToolTip toolTip1;

	private CheckBox cbListOrgPalette;

	private Label labelHintOrderSource;

	private Label labelHintReorder;

	private CheckBox cbUpdatePalette;

	private Label label1;

	public Size PasteTargetCanvasSize { get; set; } = new Size(128, 128);


	public Bitmap InputBitmap { get; set; }

	public Bitmap OutputBitmap { get; private set; }

	public int OutColorBit { get; set; } = 4;


	public int OutColorNum { get; private set; } = 256;


	public bool PastePalette { get; private set; }

	public ColorOptimizeForm()
	{
		InitializeComponent();
		ResourceUtility.UpdateTextIfLngEnabled(this, null);
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		try
		{
			tbPalNum.Value = OutColorBit;
			tbCbBpp.Value = OutColorBit + 1;
			if (InputBitmap.PixelFormat == PixelFormat.Format1bppIndexed || InputBitmap.PixelFormat == PixelFormat.Format4bppIndexed || InputBitmap.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				cbClipImageColorReduce.Checked = false;
				cbListOrgPalette.Checked = true;
				cbListOrgPalette.Enabled = true;
				cbOrderByFreq.Checked = false;
				cbSetPalNum.Checked = false;
				cbGroupMinorSimilerColor.Checked = false;
			}
			else
			{
				cbClipImageColorReduce.Checked = true;
				cbListOrgPalette.Checked = false;
				cbListOrgPalette.Enabled = false;
				cbOrderByFreq.Checked = true;
				cbSetPalNum.Checked = true;
				cbGroupMinorSimilerColor.Checked = true;
			}
		}
		catch
		{
		}
	}

	protected override void OnShown(EventArgs e)
	{
		base.OnShown(e);
		CreatePasteImage();
		UpdateControlState();
	}

	private void UpdateControlState()
	{
		bool @checked = cbClipImageColorReduce.Checked;
		bool checked2 = cbOrderByFreq.Checked;
		bool checked3 = cbSetPalNum.Checked;
		tbCbBpp.Enabled = @checked;
		tbPalNum.Enabled = checked2 && checked3;
		cbSetPalNum.Enabled = checked2;
		labelColorCount2.Enabled = checked2;
	}

	private void ActionSetControlState(object sender, EventArgs e)
	{
		UpdateControlState();
		CreatePasteImage();
	}

	private void ActionSourceColorBitChanged(object sender, EventArgs e)
	{
		CreatePasteImage();
	}

	private void tbPalNum_ValueChanged(object sender, EventArgs e)
	{
		CreatePasteImage();
	}

	private void CreatePasteImage()
	{
		try
		{
			Bitmap inputBitmap = InputBitmap;
			if (inputBitmap == null)
			{
				MessageBox.Show("InputImage error");
				throw new NullReferenceException();
			}
			int num = inputBitmap.Size.Width;
			int num2 = inputBitmap.Size.Height;
			if (num > PasteTargetCanvasSize.Width)
			{
				num = PasteTargetCanvasSize.Width;
			}
			if (num2 > PasteTargetCanvasSize.Height)
			{
				num2 = PasteTargetCanvasSize.Height;
			}
			if (tmpBmpIn != null && (num != tmpBmpIn.Width || num2 != tmpBmpIn.Height))
			{
				tmpBmpIn.Dispose();
				tmpBmpIn = null;
			}
			if (tmpBmpIn == null)
			{
				tmpBmpIn = new Bitmap(num, num2, PixelFormat.Format32bppRgb);
			}
			if (tmpBmpOut != null && (num != tmpBmpOut.Width || num2 != tmpBmpOut.Height))
			{
				tmpBmpOut.Dispose();
				tmpBmpOut = null;
			}
			if (tmpBmpOut == null)
			{
				tmpBmpOut = new Bitmap(num, num2, PixelFormat.Format32bppRgb);
			}
			int num3 = tbCbBpp.Value;
			if (!cbClipImageColorReduce.Checked)
			{
				num3 = 8;
			}
			int num4 = (1 << num3) * (1 << num3) * (1 << num3);
			for (int i = 0; i < tmpBmpIn.Height; i++)
			{
				for (int j = 0; j < tmpBmpIn.Width; j++)
				{
					Color color = ColorManager.BitOptColor(inputBitmap.GetPixel(j, i), num3);
					tmpBmpIn.SetPixel(j, i, color);
				}
			}
			mColorList.Clear();
			if (cbListOrgPalette.Checked)
			{
				ColorManager.AddListPaletteEntry(mColorList, inputBitmap, num3);
			}
			ColorManager.CountColorUsed(mColorList, tmpBmpIn, num3);
			if (cbOrderByFreq.Checked)
			{
				try
				{
					mColorList.Sort();
				}
				catch (Exception ex)
				{
					_ = ex.Message;
				}
			}
			for (int k = 0; k < 256; k++)
			{
				if (k < mColorList.Count)
				{
					mColorList[k].ManualDisabled = disabledPalette[k];
				}
			}
			int value = tbPalNum.Value;
			int num5 = 1 << value;
			int num6 = mColorList.Count;
			if (num6 >= num5)
			{
				num6 = num5;
			}
			List<int> list = new List<int>();
			for (int l = 0; l < mColorList.Count; l++)
			{
				if (list.Count < num5)
				{
					if (!mColorList[l].ManualDisabled)
					{
						mColorList[l].Enabled = true;
						int count = list.Count;
						list.Add(l);
						mColorList[l].OutPaletteIndex = count;
					}
					else
					{
						mColorList[l].Enabled = false;
					}
				}
				else
				{
					mColorList[l].Enabled = false;
				}
			}
			MakeColorReplaceMap(mColorList);
			for (int m = 0; m < tmpBmpOut.Height; m++)
			{
				if (m >= tmpBmpIn.Height)
				{
					continue;
				}
				for (int n = 0; n < tmpBmpOut.Width; n++)
				{
					if (n >= tmpBmpIn.Width)
					{
						continue;
					}
					Color pixel = tmpBmpIn.GetPixel(n, m);
					int num7 = 255;
					for (int num8 = 0; num8 < mColorList.Count; num8++)
					{
						ColorCount colorCount = mColorList[num8];
						if (colorCount.Color == pixel)
						{
							num7 = list[colorCount.OutPaletteIndex];
							break;
						}
					}
					Color color2 = ((num7 >= mColorList.Count) ? Color.Fuchsia : mColorList[num7].Color);
					try
					{
						tmpBmpOut.SetPixel(n, m, color2);
					}
					catch (Exception)
					{
					}
				}
			}
			for (int num9 = 0; num9 < 256; num9++)
			{
				Color color3;
				if (num9 < mColorList.Count)
				{
					color3 = mColorList[num9].Color;
					string text = mColorList[num9].OutPaletteIndex.ToString();
					paletteSelectorIn.Label[num9] = (mColorList[num9].ManualDisabled ? ("X" + text) : text);
				}
				else
				{
					color3 = Color.Black;
					paletteSelectorIn.Label[num9] = "";
				}
				paletteSelectorIn.Palette[num9] = color3;
			}
			int num10 = 0;
			for (int num11 = 0; num11 < 256; num11++)
			{
				paletteSelectorOut.Palette[num11] = Color.Black;
			}
			for (int num12 = 0; num12 < 256; num12++)
			{
				Color black = Color.Black;
				if (num12 < mColorList.Count && mColorList[num12].Enabled)
				{
					black = mColorList[num12].Color;
					paletteSelectorOut.Palette[num10] = black;
					num10++;
				}
			}
			string text2 = mColorList.Count.ToString();
			string text3 = num4.ToString();
			labelColorCount1.Text = "Color : " + text2 + " / " + text3;
			string text4 = num6.ToString();
			string text5 = num5.ToString();
			labelColorCount2.Text = "Color : " + text4 + " / " + text5;
			string text6 = list.Count.ToString();
			labelColorCount5.Text = "Color : " + text6;
			OutColorNum = list.Count;
		}
		catch
		{
		}
		panelImageIn.Refresh();
		panelImageOut.Refresh();
		paletteSelectorIn.Refresh();
		paletteSelectorOut.Refresh();
	}

	private void MakeColorReplaceMap(List<ColorCount> colorList)
	{
		for (int i = 0; i < colorList.Count; i++)
		{
			ColorCount colorCount = colorList[i];
			if (colorCount.Enabled)
			{
				continue;
			}
			int outPaletteIndex = 255;
			int num = 16777215;
			for (int j = 0; j < colorList.Count; j++)
			{
				ColorCount colorCount2 = colorList[j];
				if (colorCount2.Enabled)
				{
					int num2 = Math.Abs(colorCount.Color.R - colorCount2.Color.R);
					int num3 = Math.Abs(colorCount.Color.G - colorCount2.Color.G);
					int num4 = Math.Abs(colorCount.Color.B - colorCount2.Color.B);
					int num5 = (int)(0.299f * (float)(int)colorCount.Color.R + 0.587f * (float)(int)colorCount.Color.G + 0.114f * (float)(int)colorCount.Color.B);
					int num6 = (int)(0.299f * (float)(int)colorCount2.Color.R + 0.587f * (float)(int)colorCount2.Color.G + 0.114f * (float)(int)colorCount2.Color.B);
					int num7 = Math.Abs(num5 - num6);
					int num8 = num2 + num3 + num4 + num7;
					if (num8 < num)
					{
						num = num8;
						outPaletteIndex = colorCount2.OutPaletteIndex;
					}
				}
			}
			colorCount.OutPaletteIndex = outPaletteIndex;
		}
	}

	private void panelImage_Paint(object sender, PaintEventArgs e)
	{
		Image image = null;
		if (sender == panelImageIn)
		{
			image = tmpBmpIn;
		}
		if (sender == panelImageOut)
		{
			image = tmpBmpOut;
		}
		if (image == null)
		{
			return;
		}
		try
		{
			Graphics graphics = e.Graphics;
			GraphicsEx.InitGraphics(graphics);
			if (!(sender is Control))
			{
				return;
			}
			if (image != null)
			{
				int num = 2;
				if (PasteTargetCanvasSize.Width > 128 || PasteTargetCanvasSize.Height > 128)
				{
					num = 1;
				}
				Rectangle srcRect = Rectangle.FromLTRB(0, 0, image.Size.Width, image.Size.Height);
				if (srcRect.Width > PasteTargetCanvasSize.Width)
				{
					srcRect.Width = PasteTargetCanvasSize.Width;
				}
				if (srcRect.Height > PasteTargetCanvasSize.Height)
				{
					srcRect.Height = PasteTargetCanvasSize.Height;
				}
				Rectangle destRect = new Rectangle(0, 0, srcRect.Width * num, srcRect.Height * num);
				GraphicsEx.DrawImage(graphics, image, destRect, srcRect);
			}
			else
			{
				graphics.Clear(Color.Gray);
			}
		}
		catch
		{
		}
	}

	private void paletteSelectorIn_PopupEditor(object sender, EventArgs e)
	{
		int selectedIndex = paletteSelectorIn.SelectedIndex;
		disabledPalette[selectedIndex] = !disabledPalette[selectedIndex];
		CreatePasteImage();
	}

	private void buttonOK_Click(object sender, EventArgs e)
	{
		OutputBitmap = GetOutputBitmap();
		if (cbUpdatePalette.Checked)
		{
			PastePalette = true;
		}
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void buttonCancel_Click(object sender, EventArgs e)
	{
		OutputBitmap = null;
		base.DialogResult = DialogResult.Cancel;
		Close();
	}

	private Bitmap GetOutputBitmap()
	{
		try
		{
			int num = tmpBmpOut.Width;
			int num2 = tmpBmpOut.Height;
			Bitmap bitmap = new Bitmap(num, num2, PixelFormat.Format8bppIndexed);
			Bytemap bytemap = new Bytemap(num, num2);
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num; j++)
				{
					Color pixel = tmpBmpOut.GetPixel(j, i);
					int num3 = -1;
					for (int k = 0; k < mColorList.Count; k++)
					{
						if (pixel == mColorList[k].Color)
						{
							num3 = mColorList[k].OutPaletteIndex;
							break;
						}
					}
					if (num3 >= 0 && num3 < 256)
					{
						bytemap.SetPixel(j, i, (byte)num3);
					}
				}
			}
			Color[] array = new Color[256];
			for (int l = 0; l < 256; l++)
			{
				if (l < mColorList.Count && mColorList[l].Enabled)
				{
					int outPaletteIndex = mColorList[l].OutPaletteIndex;
					array[outPaletteIndex] = mColorList[l].Color;
				}
			}
			bytemap.SetPalette(array);
			BytemapConvertor.UpdateBitmapPaletteFromBytemap(bitmap, bytemap);
			BytemapConvertor.UpdateBitmapFromBytemap(bitmap, bytemap);
			return bitmap;
		}
		catch
		{
			return null;
		}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlLib.ColorOptimizeForm));
		this.labelClipboard = new System.Windows.Forms.Label();
		this.labelPreview = new System.Windows.Forms.Label();
		this.cbClipImageColorReduce = new System.Windows.Forms.CheckBox();
		this.panel1 = new System.Windows.Forms.Panel();
		this.label1 = new System.Windows.Forms.Label();
		this.cbUpdatePalette = new System.Windows.Forms.CheckBox();
		this.labelHintOrderSource = new System.Windows.Forms.Label();
		this.labelHintReorder = new System.Windows.Forms.Label();
		this.cbListOrgPalette = new System.Windows.Forms.CheckBox();
		this.labelColorCount5 = new System.Windows.Forms.Label();
		this.labelColorCount4 = new System.Windows.Forms.Label();
		this.labelColorCount3 = new System.Windows.Forms.Label();
		this.labelColorCount2 = new System.Windows.Forms.Label();
		this.labelColorCount1 = new System.Windows.Forms.Label();
		this.tbCbBpp = new System.Windows.Forms.TrackBar();
		this.tbPalNum = new System.Windows.Forms.TrackBar();
		this.cbGroupMinorSimilerColor = new System.Windows.Forms.CheckBox();
		this.cbOrderByFreq = new System.Windows.Forms.CheckBox();
		this.cbSetPalNum = new System.Windows.Forms.CheckBox();
		this.buttonOK = new System.Windows.Forms.Button();
		this.buttonCancel = new System.Windows.Forms.Button();
		this.labelHintPalette = new System.Windows.Forms.Label();
		this.labelManualDisablePalette = new System.Windows.Forms.Label();
		this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
		this.panelImageOut = new ControlLib.PicturePanel();
		this.panelImageIn = new ControlLib.PicturePanel();
		this.paletteSelectorIn = new ControlLib.PaletteSelector();
		this.paletteSelectorOut = new ControlLib.PaletteSelector();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tbCbBpp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tbPalNum).BeginInit();
		base.SuspendLayout();
		resources.ApplyResources(this.labelClipboard, "labelClipboard");
		this.labelClipboard.Name = "labelClipboard";
		this.toolTip1.SetToolTip(this.labelClipboard, resources.GetString("labelClipboard.ToolTip"));
		resources.ApplyResources(this.labelPreview, "labelPreview");
		this.labelPreview.Name = "labelPreview";
		this.toolTip1.SetToolTip(this.labelPreview, resources.GetString("labelPreview.ToolTip"));
		resources.ApplyResources(this.cbClipImageColorReduce, "cbClipImageColorReduce");
		this.cbClipImageColorReduce.Checked = true;
		this.cbClipImageColorReduce.CheckState = System.Windows.Forms.CheckState.Checked;
		this.cbClipImageColorReduce.Name = "cbClipImageColorReduce";
		this.toolTip1.SetToolTip(this.cbClipImageColorReduce, resources.GetString("cbClipImageColorReduce.ToolTip"));
		this.cbClipImageColorReduce.UseVisualStyleBackColor = true;
		this.cbClipImageColorReduce.CheckedChanged += new System.EventHandler(ActionSetControlState);
		resources.ApplyResources(this.panel1, "panel1");
		this.panel1.Controls.Add(this.label1);
		this.panel1.Controls.Add(this.cbUpdatePalette);
		this.panel1.Controls.Add(this.labelHintOrderSource);
		this.panel1.Controls.Add(this.labelHintReorder);
		this.panel1.Controls.Add(this.cbListOrgPalette);
		this.panel1.Controls.Add(this.labelColorCount5);
		this.panel1.Controls.Add(this.labelColorCount4);
		this.panel1.Controls.Add(this.labelColorCount3);
		this.panel1.Controls.Add(this.labelColorCount2);
		this.panel1.Controls.Add(this.labelColorCount1);
		this.panel1.Controls.Add(this.tbCbBpp);
		this.panel1.Controls.Add(this.tbPalNum);
		this.panel1.Controls.Add(this.cbGroupMinorSimilerColor);
		this.panel1.Controls.Add(this.cbOrderByFreq);
		this.panel1.Controls.Add(this.cbSetPalNum);
		this.panel1.Controls.Add(this.cbClipImageColorReduce);
		this.panel1.Name = "panel1";
		this.toolTip1.SetToolTip(this.panel1, resources.GetString("panel1.ToolTip"));
		resources.ApplyResources(this.label1, "label1");
		this.label1.ForeColor = System.Drawing.SystemColors.GrayText;
		this.label1.Name = "label1";
		this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
		resources.ApplyResources(this.cbUpdatePalette, "cbUpdatePalette");
		this.cbUpdatePalette.Name = "cbUpdatePalette";
		this.toolTip1.SetToolTip(this.cbUpdatePalette, resources.GetString("cbUpdatePalette.ToolTip"));
		this.cbUpdatePalette.UseVisualStyleBackColor = true;
		resources.ApplyResources(this.labelHintOrderSource, "labelHintOrderSource");
		this.labelHintOrderSource.ForeColor = System.Drawing.SystemColors.GrayText;
		this.labelHintOrderSource.Name = "labelHintOrderSource";
		this.toolTip1.SetToolTip(this.labelHintOrderSource, resources.GetString("labelHintOrderSource.ToolTip"));
		resources.ApplyResources(this.labelHintReorder, "labelHintReorder");
		this.labelHintReorder.ForeColor = System.Drawing.SystemColors.GrayText;
		this.labelHintReorder.Name = "labelHintReorder";
		this.toolTip1.SetToolTip(this.labelHintReorder, resources.GetString("labelHintReorder.ToolTip"));
		resources.ApplyResources(this.cbListOrgPalette, "cbListOrgPalette");
		this.cbListOrgPalette.Checked = true;
		this.cbListOrgPalette.CheckState = System.Windows.Forms.CheckState.Checked;
		this.cbListOrgPalette.Name = "cbListOrgPalette";
		this.toolTip1.SetToolTip(this.cbListOrgPalette, resources.GetString("cbListOrgPalette.ToolTip"));
		this.cbListOrgPalette.UseVisualStyleBackColor = true;
		this.cbListOrgPalette.CheckedChanged += new System.EventHandler(ActionSetControlState);
		resources.ApplyResources(this.labelColorCount5, "labelColorCount5");
		this.labelColorCount5.ForeColor = System.Drawing.Color.Blue;
		this.labelColorCount5.Name = "labelColorCount5";
		this.toolTip1.SetToolTip(this.labelColorCount5, resources.GetString("labelColorCount5.ToolTip"));
		resources.ApplyResources(this.labelColorCount4, "labelColorCount4");
		this.labelColorCount4.ForeColor = System.Drawing.Color.Blue;
		this.labelColorCount4.Name = "labelColorCount4";
		this.toolTip1.SetToolTip(this.labelColorCount4, resources.GetString("labelColorCount4.ToolTip"));
		resources.ApplyResources(this.labelColorCount3, "labelColorCount3");
		this.labelColorCount3.ForeColor = System.Drawing.Color.Blue;
		this.labelColorCount3.Name = "labelColorCount3";
		this.toolTip1.SetToolTip(this.labelColorCount3, resources.GetString("labelColorCount3.ToolTip"));
		resources.ApplyResources(this.labelColorCount2, "labelColorCount2");
		this.labelColorCount2.ForeColor = System.Drawing.Color.Blue;
		this.labelColorCount2.Name = "labelColorCount2";
		this.toolTip1.SetToolTip(this.labelColorCount2, resources.GetString("labelColorCount2.ToolTip"));
		resources.ApplyResources(this.labelColorCount1, "labelColorCount1");
		this.labelColorCount1.ForeColor = System.Drawing.Color.Blue;
		this.labelColorCount1.Name = "labelColorCount1";
		this.toolTip1.SetToolTip(this.labelColorCount1, resources.GetString("labelColorCount1.ToolTip"));
		resources.ApplyResources(this.tbCbBpp, "tbCbBpp");
		this.tbCbBpp.LargeChange = 1;
		this.tbCbBpp.Maximum = 8;
		this.tbCbBpp.Minimum = 1;
		this.tbCbBpp.Name = "tbCbBpp";
		this.toolTip1.SetToolTip(this.tbCbBpp, resources.GetString("tbCbBpp.ToolTip"));
		this.tbCbBpp.Value = 3;
		this.tbCbBpp.ValueChanged += new System.EventHandler(ActionSourceColorBitChanged);
		resources.ApplyResources(this.tbPalNum, "tbPalNum");
		this.tbPalNum.LargeChange = 1;
		this.tbPalNum.Maximum = 8;
		this.tbPalNum.Minimum = 1;
		this.tbPalNum.Name = "tbPalNum";
		this.toolTip1.SetToolTip(this.tbPalNum, resources.GetString("tbPalNum.ToolTip"));
		this.tbPalNum.Value = 4;
		this.tbPalNum.ValueChanged += new System.EventHandler(tbPalNum_ValueChanged);
		resources.ApplyResources(this.cbGroupMinorSimilerColor, "cbGroupMinorSimilerColor");
		this.cbGroupMinorSimilerColor.Name = "cbGroupMinorSimilerColor";
		this.toolTip1.SetToolTip(this.cbGroupMinorSimilerColor, resources.GetString("cbGroupMinorSimilerColor.ToolTip"));
		this.cbGroupMinorSimilerColor.UseVisualStyleBackColor = true;
		this.cbGroupMinorSimilerColor.CheckedChanged += new System.EventHandler(ActionSetControlState);
		resources.ApplyResources(this.cbOrderByFreq, "cbOrderByFreq");
		this.cbOrderByFreq.Checked = true;
		this.cbOrderByFreq.CheckState = System.Windows.Forms.CheckState.Checked;
		this.cbOrderByFreq.Name = "cbOrderByFreq";
		this.toolTip1.SetToolTip(this.cbOrderByFreq, resources.GetString("cbOrderByFreq.ToolTip"));
		this.cbOrderByFreq.UseVisualStyleBackColor = true;
		this.cbOrderByFreq.CheckedChanged += new System.EventHandler(ActionSetControlState);
		resources.ApplyResources(this.cbSetPalNum, "cbSetPalNum");
		this.cbSetPalNum.Checked = true;
		this.cbSetPalNum.CheckState = System.Windows.Forms.CheckState.Checked;
		this.cbSetPalNum.Name = "cbSetPalNum";
		this.toolTip1.SetToolTip(this.cbSetPalNum, resources.GetString("cbSetPalNum.ToolTip"));
		this.cbSetPalNum.UseVisualStyleBackColor = true;
		this.cbSetPalNum.CheckedChanged += new System.EventHandler(ActionSetControlState);
		resources.ApplyResources(this.buttonOK, "buttonOK");
		this.buttonOK.Name = "buttonOK";
		this.toolTip1.SetToolTip(this.buttonOK, resources.GetString("buttonOK.ToolTip"));
		this.buttonOK.UseVisualStyleBackColor = true;
		this.buttonOK.Click += new System.EventHandler(buttonOK_Click);
		resources.ApplyResources(this.buttonCancel, "buttonCancel");
		this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.buttonCancel.Name = "buttonCancel";
		this.toolTip1.SetToolTip(this.buttonCancel, resources.GetString("buttonCancel.ToolTip"));
		this.buttonCancel.UseVisualStyleBackColor = true;
		this.buttonCancel.Click += new System.EventHandler(buttonCancel_Click);
		resources.ApplyResources(this.labelHintPalette, "labelHintPalette");
		this.labelHintPalette.Name = "labelHintPalette";
		this.toolTip1.SetToolTip(this.labelHintPalette, resources.GetString("labelHintPalette.ToolTip"));
		resources.ApplyResources(this.labelManualDisablePalette, "labelManualDisablePalette");
		this.labelManualDisablePalette.Name = "labelManualDisablePalette";
		this.toolTip1.SetToolTip(this.labelManualDisablePalette, resources.GetString("labelManualDisablePalette.ToolTip"));
		resources.ApplyResources(this.panelImageOut, "panelImageOut");
		this.panelImageOut.BackColor = System.Drawing.SystemColors.Control;
		this.panelImageOut.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.panelImageOut.Name = "panelImageOut";
		this.toolTip1.SetToolTip(this.panelImageOut, resources.GetString("panelImageOut.ToolTip"));
		this.panelImageOut.Paint += new System.Windows.Forms.PaintEventHandler(panelImage_Paint);
		resources.ApplyResources(this.panelImageIn, "panelImageIn");
		this.panelImageIn.BackColor = System.Drawing.SystemColors.Control;
		this.panelImageIn.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.panelImageIn.Name = "panelImageIn";
		this.toolTip1.SetToolTip(this.panelImageIn, resources.GetString("panelImageIn.ToolTip"));
		this.panelImageIn.Paint += new System.Windows.Forms.PaintEventHandler(panelImage_Paint);
		resources.ApplyResources(this.paletteSelectorIn, "paletteSelectorIn");
		this.paletteSelectorIn.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.paletteSelectorIn.CellColumnCount = 16;
		this.paletteSelectorIn.CellColumnView = 16;
		this.paletteSelectorIn.CellHeight = 16;
		this.paletteSelectorIn.CellRowCount = 16;
		this.paletteSelectorIn.CellRowView = 2;
		this.paletteSelectorIn.CellWidth = 16;
		this.paletteSelectorIn.ColorCount = 256;
		this.paletteSelectorIn.DrawMultiSelect = false;
		this.paletteSelectorIn.EnableMultiSelect = false;
		this.paletteSelectorIn.EnableSelectM = false;
		this.paletteSelectorIn.EnableSelectR = true;
		this.paletteSelectorIn.EnableSetUnknownMarkR = false;
		this.paletteSelectorIn.Label = new string[256];
		this.paletteSelectorIn.LabelItem = ControlLib.LabelItem.LabelsProperty;
		this.paletteSelectorIn.LabelStyle = ControlLib.LabelStyle.All;
		this.paletteSelectorIn.Name = "paletteSelectorIn";
		this.paletteSelectorIn.Palette = new System.Drawing.Color[256]
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
		this.paletteSelectorIn.PaletteFlags = new byte[256];
		this.paletteSelectorIn.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.paletteSelectorIn.SelectedIndex = 0;
		this.paletteSelectorIn.SelectEndIndex = 1;
		this.paletteSelectorIn.SetSize = 0;
		this.paletteSelectorIn.ShowSetRect = true;
		this.toolTip1.SetToolTip(this.paletteSelectorIn, resources.GetString("paletteSelectorIn.ToolTip"));
		this.paletteSelectorIn.PopupEditor += new System.EventHandler(paletteSelectorIn_PopupEditor);
		resources.ApplyResources(this.paletteSelectorOut, "paletteSelectorOut");
		this.paletteSelectorOut.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.paletteSelectorOut.CellColumnCount = 16;
		this.paletteSelectorOut.CellColumnView = 16;
		this.paletteSelectorOut.CellHeight = 16;
		this.paletteSelectorOut.CellRowCount = 16;
		this.paletteSelectorOut.CellRowView = 2;
		this.paletteSelectorOut.CellWidth = 16;
		this.paletteSelectorOut.ColorCount = 256;
		this.paletteSelectorOut.DrawMultiSelect = false;
		this.paletteSelectorOut.EnableMultiSelect = false;
		this.paletteSelectorOut.EnableSelectM = false;
		this.paletteSelectorOut.EnableSelectR = true;
		this.paletteSelectorOut.EnableSetUnknownMarkR = false;
		this.paletteSelectorOut.Label = new string[256];
		this.paletteSelectorOut.LabelItem = ControlLib.LabelItem.Index;
		this.paletteSelectorOut.LabelStyle = ControlLib.LabelStyle.None;
		this.paletteSelectorOut.Name = "paletteSelectorOut";
		this.paletteSelectorOut.Palette = new System.Drawing.Color[256]
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
		this.paletteSelectorOut.PaletteFlags = new byte[256];
		this.paletteSelectorOut.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.paletteSelectorOut.SelectedIndex = 0;
		this.paletteSelectorOut.SelectEndIndex = 1;
		this.paletteSelectorOut.SetSize = 0;
		this.paletteSelectorOut.ShowSetRect = true;
		this.toolTip1.SetToolTip(this.paletteSelectorOut, resources.GetString("paletteSelectorOut.ToolTip"));
		base.AcceptButton = this.buttonOK;
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
		base.CancelButton = this.buttonCancel;
		base.Controls.Add(this.labelManualDisablePalette);
		base.Controls.Add(this.panelImageOut);
		base.Controls.Add(this.panelImageIn);
		base.Controls.Add(this.paletteSelectorIn);
		base.Controls.Add(this.labelHintPalette);
		base.Controls.Add(this.buttonCancel);
		base.Controls.Add(this.buttonOK);
		base.Controls.Add(this.panel1);
		base.Controls.Add(this.labelPreview);
		base.Controls.Add(this.labelClipboard);
		base.Controls.Add(this.paletteSelectorOut);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "ColorOptimizeForm";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.tbCbBpp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tbPalNum).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
