using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using CharactorLib.Common;
using ControlLib;

namespace PrgEditor.Forms;

public class SearchForm : Form
{
	public enum FindDirection
	{
		None,
		Previous,
		Next
	}

	public enum FindType
	{
		Text,
		Graphic
	}

	private byte[] mData = new byte[4];

	private Bitmap mBitmapChr = new Bitmap(128, 128, PixelFormat.Format8bppIndexed);

	private Bitmap mBitmapFind1 = new Bitmap(64, 16, PixelFormat.Format8bppIndexed);

	private Bitmap mBitmapFind2 = new Bitmap(32, 32, PixelFormat.Format8bppIndexed);

	private Bytemap mBytemapFind1 = new Bytemap(64, 16);

	private Bytemap mBytemapFind2 = new Bytemap(32, 32);

	private IContainer components;

	private CellSelector cellSelectorFind1;

	private CellSelector cellSelectorFind2;

	private CellSelector cellSelectorChr;

	private RadioButton radioButton1;

	private RadioButton radioButton2;

	private Button buttonFindNext;

	private Button buttonCancel;

	private Button buttonFindPrevious;

	private Label labelHint;

	private Panel panel1;

	private Panel panel2;

	private Panel panel3;

	public FindDirection Direction { get; set; }

	public FindType Type { get; set; }

	public byte[] Data
	{
		get
		{
			return mData;
		}
		set
		{
			if (value != null)
			{
				int num = Math.Min(mData.Length, value.Length);
				for (int i = 0; i < num; i++)
				{
					mData[i] = value[i];
				}
			}
		}
	}

	public Bytemap BytemapChr { get; set; }

	public SearchForm()
	{
		InitializeComponent();
		Direction = FindDirection.None;
		Type = FindType.Text;
	}

	public SearchForm(Bytemap bytemap, byte[] data)
	{
		InitializeComponent();
		Direction = FindDirection.None;
		BytemapChr = bytemap;
		BytemapConvertor.UpdateBitmapPaletteFromBytemap(mBitmapChr, BytemapChr);
		BytemapConvertor.UpdateBitmapFromBytemap(mBitmapChr, BytemapChr);
		cellSelectorChr.Image = mBitmapChr;
		mBytemapFind1.SetPalette(BytemapChr.Palette);
		BytemapConvertor.UpdateBitmapPaletteFromBytemap(mBitmapFind1, mBytemapFind1);
		BytemapConvertor.UpdateBitmapFromBytemap(mBitmapFind1, mBytemapFind1);
		cellSelectorFind1.Image = mBitmapFind1;
		mBytemapFind2.SetPalette(BytemapChr.Palette);
		BytemapConvertor.UpdateBitmapPaletteFromBytemap(mBitmapFind2, mBytemapFind2);
		BytemapConvertor.UpdateBitmapFromBytemap(mBitmapFind2, mBytemapFind2);
		cellSelectorFind2.Image = mBitmapFind2;
		Data = data;
		SetChrSelection(Data[0]);
		UpdateFindDataBytemap();
	}

	private void buttonFindPrevious_Click(object sender, EventArgs e)
	{
		Type = ((!radioButton1.Checked) ? FindType.Graphic : FindType.Text);
		Direction = FindDirection.Previous;
		base.DialogResult = DialogResult.OK;
	}

	private void buttonFindNext_Click(object sender, EventArgs e)
	{
		Type = ((!radioButton1.Checked) ? FindType.Graphic : FindType.Text);
		Direction = FindDirection.Next;
		base.DialogResult = DialogResult.OK;
	}

	private void buttonCancel_Click(object sender, EventArgs e)
	{
		Direction = FindDirection.None;
		base.DialogResult = DialogResult.Cancel;
	}

	private void cellSelectorFind1_Selected(object sender, MouseEventArgs e)
	{
		int num = cellSelectorFind1.SelectedRect.X / 8;
		_ = cellSelectorFind1.SelectedRect.Y / 8;
		int num2 = num;
		byte chrSelection = mData[num2];
		SetChrSelection(chrSelection);
		radioButton1.Checked = true;
		UpdateFindDataBytemap();
	}

	private void cellSelectorFind2_Selected(object sender, MouseEventArgs e)
	{
		int num = cellSelectorFind2.SelectedRect.X / 8;
		int num2 = cellSelectorFind2.SelectedRect.Y / 8 * 2 + num;
		byte chrSelection = mData[num2];
		SetChrSelection(chrSelection);
		radioButton2.Checked = true;
		UpdateFindDataBytemap();
	}

	private void cellSelectorChr_Selected(object sender, MouseEventArgs e)
	{
		byte b = 0;
		int num = cellSelectorChr.SelectedRect.X / 8;
		b = (byte)(cellSelectorChr.SelectedRect.Y / 8 * 16 + num);
		int num3;
		if (radioButton1.Checked)
		{
			int num2 = cellSelectorFind1.SelectedRect.X / 8;
			_ = cellSelectorFind1.SelectedRect.Y / 8;
			num3 = num2;
		}
		else
		{
			int num4 = cellSelectorFind2.SelectedRect.X / 8;
			num3 = cellSelectorFind2.SelectedRect.Y / 8 * 2 + num4;
		}
		if (e.Button == MouseButtons.Left)
		{
			mData[num3] = b;
		}
		UpdateFindDataBytemap();
	}

	private void cellSelectorChr_MouseUp(object sender, MouseEventArgs e)
	{
		if (radioButton1.Checked)
		{
			if (e.Button == MouseButtons.Left)
			{
				cellSelectorFind1.SelectedRect = new Rectangle((cellSelectorFind1.SelectedRect.X + 8) % 32, 0, 8, 8);
			}
			else
			{
				cellSelectorFind1.SelectedRect = new Rectangle((cellSelectorFind1.SelectedRect.X + 24) % 32, 0, 8, 8);
			}
		}
		UpdateFindDataBytemap();
	}

	private void SetChrSelection(byte data)
	{
		int num = (int)data / 16;
		int num2 = (int)data % 16;
		cellSelectorChr.SelectedRect = new Rectangle(num2 * 8, num * 8, 8, 8);
	}

	private void UpdateFindDataBytemap()
	{
		DraeFindData(mBytemapFind1, mData, 4, 1);
		DraeFindData(mBytemapFind2, mData, 2, 2);
		BytemapConvertor.UpdateBitmapFromBytemap(mBitmapFind1, mBytemapFind1);
		BytemapConvertor.UpdateBitmapFromBytemap(mBitmapFind2, mBytemapFind2);
		cellSelectorChr.Refresh();
		cellSelectorFind1.Refresh();
		cellSelectorFind2.Refresh();
	}

	private void DraeFindData(Bytemap bytemap, byte[] data, int w, int h)
	{
		int num = 0;
		for (int i = 0; i < h; i++)
		{
			for (int j = 0; j < w; j++)
			{
				byte num2 = data[num];
				int num3 = (int)num2 % 16;
				int num4 = (int)num2 / 16;
				bytemap.CopyRect(new Point(j * 8, i * 8), BytemapChr, new Rectangle(num3 * 8, num4 * 8, 8, 8));
				num++;
			}
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
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(PrgEditor.Forms.SearchForm));
		this.radioButton1 = new System.Windows.Forms.RadioButton();
		this.radioButton2 = new System.Windows.Forms.RadioButton();
		this.buttonFindNext = new System.Windows.Forms.Button();
		this.buttonCancel = new System.Windows.Forms.Button();
		this.buttonFindPrevious = new System.Windows.Forms.Button();
		this.labelHint = new System.Windows.Forms.Label();
		this.panel1 = new System.Windows.Forms.Panel();
		this.cellSelectorFind2 = new ControlLib.CellSelector();
		this.panel2 = new System.Windows.Forms.Panel();
		this.cellSelectorFind1 = new ControlLib.CellSelector();
		this.panel3 = new System.Windows.Forms.Panel();
		this.cellSelectorChr = new ControlLib.CellSelector();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cellSelectorFind2).BeginInit();
		this.panel2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cellSelectorFind1).BeginInit();
		this.panel3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cellSelectorChr).BeginInit();
		base.SuspendLayout();
		componentResourceManager.ApplyResources(this.radioButton1, "radioButton1");
		this.radioButton1.Checked = true;
		this.radioButton1.Name = "radioButton1";
		this.radioButton1.TabStop = true;
		this.radioButton1.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this.radioButton2, "radioButton2");
		this.radioButton2.Name = "radioButton2";
		this.radioButton2.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this.buttonFindNext, "buttonFindNext");
		this.buttonFindNext.Name = "buttonFindNext";
		this.buttonFindNext.UseVisualStyleBackColor = true;
		this.buttonFindNext.Click += new System.EventHandler(buttonFindNext_Click);
		this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		componentResourceManager.ApplyResources(this.buttonCancel, "buttonCancel");
		this.buttonCancel.Name = "buttonCancel";
		this.buttonCancel.UseVisualStyleBackColor = true;
		this.buttonCancel.Click += new System.EventHandler(buttonCancel_Click);
		componentResourceManager.ApplyResources(this.buttonFindPrevious, "buttonFindPrevious");
		this.buttonFindPrevious.Name = "buttonFindPrevious";
		this.buttonFindPrevious.UseVisualStyleBackColor = true;
		this.buttonFindPrevious.Click += new System.EventHandler(buttonFindPrevious_Click);
		componentResourceManager.ApplyResources(this.labelHint, "labelHint");
		this.labelHint.Name = "labelHint";
		this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.panel1.Controls.Add(this.cellSelectorFind2);
		componentResourceManager.ApplyResources(this.panel1, "panel1");
		this.panel1.Name = "panel1";
		this.cellSelectorFind2.DefaultSelectSize = new System.Drawing.Size(8, 8);
		this.cellSelectorFind2.EnableRightDragSelect = false;
		this.cellSelectorFind2.FreeSelect = false;
		this.cellSelectorFind2.GridColor1 = System.Drawing.Color.White;
		this.cellSelectorFind2.GridColor2 = System.Drawing.Color.Gray;
		this.cellSelectorFind2.GridStyle = ControlLib.GridStyle.Dot;
		this.cellSelectorFind2.GridVisible = true;
		this.cellSelectorFind2.Image = null;
		componentResourceManager.ApplyResources(this.cellSelectorFind2, "cellSelectorFind2");
		this.cellSelectorFind2.Name = "cellSelectorFind2";
		this.cellSelectorFind2.PixelSelect = false;
		this.cellSelectorFind2.SelectedColor1 = System.Drawing.Color.White;
		this.cellSelectorFind2.SelectedColor2 = System.Drawing.Color.Aqua;
		this.cellSelectorFind2.SelectedRect = new System.Drawing.Rectangle(0, 0, 8, 8);
		this.cellSelectorFind2.SelectorVisible = true;
		this.cellSelectorFind2.TabStop = false;
		this.cellSelectorFind2.ZoomRate = 2;
		this.cellSelectorFind2.Selected += new System.Windows.Forms.MouseEventHandler(cellSelectorFind2_Selected);
		this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.panel2.Controls.Add(this.cellSelectorFind1);
		componentResourceManager.ApplyResources(this.panel2, "panel2");
		this.panel2.Name = "panel2";
		this.cellSelectorFind1.DefaultSelectSize = new System.Drawing.Size(8, 8);
		this.cellSelectorFind1.EnableRightDragSelect = false;
		this.cellSelectorFind1.FreeSelect = false;
		this.cellSelectorFind1.GridColor1 = System.Drawing.Color.White;
		this.cellSelectorFind1.GridColor2 = System.Drawing.Color.Gray;
		this.cellSelectorFind1.GridStyle = ControlLib.GridStyle.Dot;
		this.cellSelectorFind1.GridVisible = true;
		this.cellSelectorFind1.Image = null;
		componentResourceManager.ApplyResources(this.cellSelectorFind1, "cellSelectorFind1");
		this.cellSelectorFind1.Name = "cellSelectorFind1";
		this.cellSelectorFind1.PixelSelect = false;
		this.cellSelectorFind1.SelectedColor1 = System.Drawing.Color.White;
		this.cellSelectorFind1.SelectedColor2 = System.Drawing.Color.Aqua;
		this.cellSelectorFind1.SelectedRect = new System.Drawing.Rectangle(0, 0, 8, 8);
		this.cellSelectorFind1.SelectorVisible = true;
		this.cellSelectorFind1.TabStop = false;
		this.cellSelectorFind1.ZoomRate = 2;
		this.cellSelectorFind1.Selected += new System.Windows.Forms.MouseEventHandler(cellSelectorFind1_Selected);
		this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.panel3.Controls.Add(this.cellSelectorChr);
		componentResourceManager.ApplyResources(this.panel3, "panel3");
		this.panel3.Name = "panel3";
		this.cellSelectorChr.DefaultSelectSize = new System.Drawing.Size(8, 8);
		this.cellSelectorChr.EnableRightDragSelect = false;
		this.cellSelectorChr.FreeSelect = false;
		this.cellSelectorChr.GridColor1 = System.Drawing.Color.White;
		this.cellSelectorChr.GridColor2 = System.Drawing.Color.Gray;
		this.cellSelectorChr.GridStyle = ControlLib.GridStyle.Dot;
		this.cellSelectorChr.GridVisible = true;
		this.cellSelectorChr.Image = null;
		componentResourceManager.ApplyResources(this.cellSelectorChr, "cellSelectorChr");
		this.cellSelectorChr.Name = "cellSelectorChr";
		this.cellSelectorChr.PixelSelect = false;
		this.cellSelectorChr.SelectedColor1 = System.Drawing.Color.White;
		this.cellSelectorChr.SelectedColor2 = System.Drawing.Color.Aqua;
		this.cellSelectorChr.SelectedRect = new System.Drawing.Rectangle(0, 0, 8, 8);
		this.cellSelectorChr.SelectorVisible = true;
		this.cellSelectorChr.TabStop = false;
		this.cellSelectorChr.ZoomRate = 2;
		this.cellSelectorChr.Selected += new System.Windows.Forms.MouseEventHandler(cellSelectorChr_Selected);
		this.cellSelectorChr.MouseUp += new System.Windows.Forms.MouseEventHandler(cellSelectorChr_MouseUp);
		base.AcceptButton = this.buttonFindNext;
		componentResourceManager.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.buttonCancel;
		base.Controls.Add(this.panel3);
		base.Controls.Add(this.panel2);
		base.Controls.Add(this.panel1);
		base.Controls.Add(this.labelHint);
		base.Controls.Add(this.buttonFindPrevious);
		base.Controls.Add(this.buttonCancel);
		base.Controls.Add(this.buttonFindNext);
		base.Controls.Add(this.radioButton2);
		base.Controls.Add(this.radioButton1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "SearchForm";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.panel1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cellSelectorFind2).EndInit();
		this.panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cellSelectorFind1).EndInit();
		this.panel3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cellSelectorChr).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
