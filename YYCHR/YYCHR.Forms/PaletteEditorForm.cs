using System;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CharactorLib;
using CharactorLib.Common;
using ControlLib;
using Controls;

namespace YYCHR.Forms;

public class PaletteEditorForm : Form
{
	private bool firstVisibleProcessed;

	private PaletteType[] mComboboxPaletteType = new PaletteType[5]
	{
		PaletteType.R8G8B8,
		PaletteType.R5G6B5,
		PaletteType.R5G5B5,
		PaletteType.R4G4B4,
		PaletteType.R3G3B3
	};

	private bool DisableTextBoxChanged;

	private IContainer components;

	private ComboBoxEx comboBoxBit;

	private Button buttonOK;

	private RGBEditor rgbEditor1;

	private PicturePanel panelPreview;

	private Button buttonReset;

	private TextBox textBoxHex;

	private Button buttonCancel;

	private Panel panelBase;

	private TextBox textBoxHexBit;

	private ToolTip toolTip1;

	public int Index { get; set; }

	public ColorBit ColorBit { get; set; } = new ColorBit(PaletteType.R8G8B8, byte.MaxValue, 0, 0, 0);


	public bool ShowReset { get; set; } = true;


	public ColorBit ResetColorBit { get; set; } = new ColorBit(PaletteType.R8G8B8, byte.MaxValue, 0, 0, 0);


	private bool UpdateResetColorForBitChange { get; set; } = true;


	private Color ResetColorForBitChange { get; set; } = Color.Black;


	public bool ReadOnly { get; set; }

	public ToolTip ToolTip => toolTip1;

	public PaletteEditorForm()
	{
		InitializeComponent();
		ResourceUtility.UpdateTextIfLngEnabled(this, toolTip1);
	}

	private void PaletteEditorForm_VisibleChanged(object sender, EventArgs e)
	{
		if (base.Visible)
		{
			if (!firstVisibleProcessed)
			{
				FirstVisible();
				firstVisibleProcessed = true;
			}
			ColorBit colorBit = ColorBit;
			ResetColorBit.CopyAllFrom(colorBit);
			ResetColorForBitChange = colorBit.Color;
			bool enabled = !ReadOnly;
			bool readOnly = ReadOnly;
			textBoxHex.ReadOnly = readOnly;
			rgbEditor1.ReadOnly = readOnly;
			comboBoxBit.Enabled = enabled;
			buttonReset.Enabled = enabled;
			buttonOK.Enabled = enabled;
			int comboboxIndexFromPaletteType = GetComboboxIndexFromPaletteType(colorBit.PalType);
			if (comboBoxBit.SelectedIndex != comboboxIndexFromPaletteType)
			{
				comboBoxBit.SelectedIndexChanged -= comboBoxBit_SelectedIndexChanged;
				comboBoxBit.SelectedIndex = comboboxIndexFromPaletteType;
				comboBoxBit.SelectedIndexChanged += comboBoxBit_SelectedIndexChanged;
			}
			comboBoxBit.Enabled = false;
			rgbEditor1.SetColorBitFrom(colorBit);
			UpdateControls();
		}
		_ = base.Visible;
	}

	private void FirstVisible()
	{
		CreateComboBoxItem();
		if (buttonReset.Visible != ShowReset)
		{
			buttonReset.Visible = ShowReset;
		}
	}

	private void PaletteEditorForm_Deactivate(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		HideDialog();
	}

	private void HideDialog()
	{
		if (base.Visible)
		{
			base.Visible = false;
		}
	}

	private void CreateComboBoxItem()
	{
		comboBoxBit.Items.Clear();
		PaletteType[] array = mComboboxPaletteType;
		foreach (PaletteType paletteType in array)
		{
			string name = Enum.GetName(paletteType.GetType(), paletteType);
			comboBoxBit.Items.Add(name);
		}
		comboBoxBit.SelectedIndex = 0;
	}

	private int GetComboboxIndexFromPaletteType(PaletteType palType)
	{
		int result = 0;
		for (int i = 0; i < mComboboxPaletteType.Length; i++)
		{
			if (palType == mComboboxPaletteType[i])
			{
				result = i;
				break;
			}
		}
		return result;
	}

	private void panelPreview_Paint(object sender, PaintEventArgs e)
	{
		Color color = rgbEditor1.ColorBit.Color;
		Graphics graphics = e.Graphics;
		graphics.Clear(Color.Black);
		graphics.Clear(color);
		string text = "Pal " + Index.ToString("X2");
		Color[] textColorFromBackColor = GraphicsEx.GetTextColorFromBackColor(color);
		Color color2 = textColorFromBackColor[0];
		Color color3 = textColorFromBackColor[1];
		using Brush brushOuter = new SolidBrush(color2);
		using Brush brushInner = new SolidBrush(color3);
		using StringFormat stringFormat = new StringFormat();
		stringFormat.Alignment = StringAlignment.Center;
		stringFormat.LineAlignment = StringAlignment.Center;
		GraphicsEx.DrawString4(graphics, text, rgbEditor1.Font, brushOuter, brushInner, panelPreview.ClientRectangle, stringFormat);
	}

	private void UpdateControls()
	{
		ColorBit colorBit = rgbEditor1.ColorBit;
		string text = colorBit.ToColorString();
		SetTextBoxWithoutChangedEvent(textBoxHex, text);
		Array array = mComboboxPaletteType;
		int num = comboBoxBit.SelectedIndex;
		if (num >= array.Length)
		{
			num = array.Length - 1;
		}
		if (num <= 0)
		{
			num = 0;
		}
		PaletteType palType = (PaletteType)array.GetValue(num);
		textBoxHexBit.Text = colorBit.GetByteDataText(palType);
		Refresh();
	}

	private void rgbEditor1_Changed(object sender, EventArgs e)
	{
		UpdateControls();
		if (UpdateResetColorForBitChange)
		{
			ResetColorForBitChange = rgbEditor1.ColorBit.Color;
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

	private void comboBoxBit_SelectedIndexChanged(object sender, EventArgs e)
	{
		Array array = mComboboxPaletteType;
		int num = comboBoxBit.SelectedIndex;
		if (num >= array.Length)
		{
			num = array.Length - 1;
		}
		if (num <= 0)
		{
			num = 0;
		}
		PaletteType paletteType = mComboboxPaletteType[num];
		rgbEditor1.ColorBit.SetPaletteType(paletteType);
		UpdateResetColorForBitChange = false;
		rgbEditor1.SetColor(ResetColorForBitChange);
		UpdateResetColorForBitChange = true;
		Refresh();
	}

	private void buttonOK_Click(object sender, EventArgs e)
	{
		ColorBit.CopyAllFrom(rgbEditor1.ColorBit);
		base.DialogResult = DialogResult.OK;
		HideDialog();
	}

	private void buttonCancel_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		HideDialog();
	}

	private void buttonDefault_Click(object sender, EventArgs e)
	{
		rgbEditor1.SetColor(ResetColorBit.Color);
		UpdateControls();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(YYCHR.Forms.PaletteEditorForm));
		this.comboBoxBit = new Controls.ComboBoxEx();
		this.buttonOK = new System.Windows.Forms.Button();
		this.panelPreview = new ControlLib.PicturePanel();
		this.buttonReset = new System.Windows.Forms.Button();
		this.textBoxHex = new System.Windows.Forms.TextBox();
		this.buttonCancel = new System.Windows.Forms.Button();
		this.rgbEditor1 = new ControlLib.RGBEditor();
		this.panelBase = new System.Windows.Forms.Panel();
		this.textBoxHexBit = new System.Windows.Forms.TextBox();
		this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
		this.panelBase.SuspendLayout();
		base.SuspendLayout();
		this.comboBoxBit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxBit.FormattingEnabled = true;
		this.comboBoxBit.Items.AddRange(new object[3]
		{
			resources.GetString("comboBoxBit.Items"),
			resources.GetString("comboBoxBit.Items1"),
			resources.GetString("comboBoxBit.Items2")
		});
		resources.ApplyResources(this.comboBoxBit, "comboBoxBit");
		this.comboBoxBit.Name = "comboBoxBit";
		this.toolTip1.SetToolTip(this.comboBoxBit, resources.GetString("comboBoxBit.ToolTip"));
		this.comboBoxBit.SelectedIndexChanged += new System.EventHandler(comboBoxBit_SelectedIndexChanged);
		resources.ApplyResources(this.buttonOK, "buttonOK");
		this.buttonOK.Name = "buttonOK";
		this.toolTip1.SetToolTip(this.buttonOK, resources.GetString("buttonOK.ToolTip"));
		this.buttonOK.UseVisualStyleBackColor = true;
		this.buttonOK.Click += new System.EventHandler(buttonOK_Click);
		this.panelPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		resources.ApplyResources(this.panelPreview, "panelPreview");
		this.panelPreview.Name = "panelPreview";
		this.toolTip1.SetToolTip(this.panelPreview, resources.GetString("panelPreview.ToolTip"));
		this.panelPreview.Paint += new System.Windows.Forms.PaintEventHandler(panelPreview_Paint);
		resources.ApplyResources(this.buttonReset, "buttonReset");
		this.buttonReset.Name = "buttonReset";
		this.toolTip1.SetToolTip(this.buttonReset, resources.GetString("buttonReset.ToolTip"));
		this.buttonReset.UseVisualStyleBackColor = true;
		this.buttonReset.Click += new System.EventHandler(buttonDefault_Click);
		resources.ApplyResources(this.textBoxHex, "textBoxHex");
		this.textBoxHex.Name = "textBoxHex";
		this.toolTip1.SetToolTip(this.textBoxHex, resources.GetString("textBoxHex.ToolTip"));
		this.textBoxHex.TextChanged += new System.EventHandler(textBoxHex_TextChanged);
		this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		resources.ApplyResources(this.buttonCancel, "buttonCancel");
		this.buttonCancel.Name = "buttonCancel";
		this.toolTip1.SetToolTip(this.buttonCancel, resources.GetString("buttonCancel.ToolTip"));
		this.buttonCancel.UseVisualStyleBackColor = true;
		this.buttonCancel.Click += new System.EventHandler(buttonCancel_Click);
		this.rgbEditor1.BackColor = System.Drawing.Color.Black;
		this.rgbEditor1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		resources.ApplyResources(this.rgbEditor1, "rgbEditor1");
		this.rgbEditor1.Name = "rgbEditor1";
		this.rgbEditor1.ColorChanged += new System.EventHandler(rgbEditor1_Changed);
		this.panelBase.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.panelBase.Controls.Add(this.textBoxHexBit);
		this.panelBase.Controls.Add(this.panelPreview);
		this.panelBase.Controls.Add(this.comboBoxBit);
		this.panelBase.Controls.Add(this.textBoxHex);
		this.panelBase.Controls.Add(this.rgbEditor1);
		this.panelBase.Controls.Add(this.buttonReset);
		this.panelBase.Controls.Add(this.buttonOK);
		this.panelBase.Controls.Add(this.buttonCancel);
		resources.ApplyResources(this.panelBase, "panelBase");
		this.panelBase.Name = "panelBase";
		resources.ApplyResources(this.textBoxHexBit, "textBoxHexBit");
		this.textBoxHexBit.Name = "textBoxHexBit";
		this.textBoxHexBit.ReadOnly = true;
		this.toolTip1.SetToolTip(this.textBoxHexBit, resources.GetString("textBoxHexBit.ToolTip"));
		base.AcceptButton = this.buttonOK;
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
		base.CancelButton = this.buttonCancel;
		resources.ApplyResources(this, "$this");
		base.Controls.Add(this.panelBase);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		base.Name = "PaletteEditorForm";
		base.ShowInTaskbar = false;
		base.TopMost = true;
		base.Deactivate += new System.EventHandler(PaletteEditorForm_Deactivate);
		base.VisibleChanged += new System.EventHandler(PaletteEditorForm_VisibleChanged);
		this.panelBase.ResumeLayout(false);
		this.panelBase.PerformLayout();
		base.ResumeLayout(false);
	}
}
