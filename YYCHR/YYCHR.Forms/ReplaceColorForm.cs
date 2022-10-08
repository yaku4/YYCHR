using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using CharactorLib.Common;
using CharactorLib.Data;
using ControlLib;

namespace YYCHR.Forms;

public class ReplaceColorForm : Form
{
	private DatInfo mDatInfo;

	private int mFromPaletteSet;

	private int mFromSize = 4;

	private byte[] mToPalette = new byte[256];

	private bool first = true;

	private IContainer components;

	private PaletteSelector paletteSelectorFrom;

	private PaletteSelector paletteSelectorTo;

	private Button buttonOK;

	private Button buttonCancel;

	private Label labelFromColor;

	private Label labelToColor;

	private Label labelHint;

	private PicturePanel picturePanelSource;

	private PicturePanel picturePanelPreview;

	private Label labelNoteReplaceChange;

	public DatInfo DatInfo
	{
		get
		{
			return mDatInfo;
		}
		set
		{
			mDatInfo = value;
		}
	}

	public int FromPaletteSet
	{
		get
		{
			return mFromPaletteSet;
		}
		set
		{
			mFromPaletteSet = value;
		}
	}

	public int FromSize
	{
		get
		{
			return mFromSize;
		}
		set
		{
			if (value > 256)
			{
				value = 256;
			}
			mFromSize = value;
		}
	}

	public byte[] ToPalette => mToPalette;

	public Bytemap Bytemap { get; internal set; }

	public Rectangle SelectedRect { get; internal set; } = new Rectangle(0, 0, 8, 8);


	private Bytemap BytemapPreview { get; set; }

	public ReplaceColorForm()
	{
		InitializeComponent();
		ResourceUtility.UpdateTextIfLngEnabled(this, null);
	}

	private void ReplaceColorForm_Shown(object sender, EventArgs e)
	{
		if (first)
		{
			first = false;
			for (int i = 0; i < ToPalette.Length; i++)
			{
				ToPalette[i] = (byte)i;
			}
			string text = "X";
			if (FromSize > 16)
			{
				text = "X2";
			}
			for (int j = 0; j < FromSize; j++)
			{
				paletteSelectorFrom.Label[j] = j.ToString(text);
			}
			for (int k = 0; k < FromSize; k++)
			{
				paletteSelectorTo.Label[k] = k.ToString(text);
			}
			paletteSelectorFrom.SelectedIndex = 0;
			paletteSelectorTo.SelectedIndex = 0;
			paletteSelectorFrom.CellColumnView = FromSize;
			paletteSelectorTo.CellColumnView = FromSize;
			ResetPaletteSelectorColor();
			int num = FromSize / 16;
			if (num < 1)
			{
				num = 1;
			}
			paletteSelectorFrom.CellRowView = num;
			paletteSelectorTo.CellRowView = num;
			int num2 = 10;
			picturePanelSource.Location = new Point(paletteSelectorFrom.Left, paletteSelectorFrom.Bottom + num2);
			picturePanelPreview.Location = new Point(paletteSelectorTo.Left, paletteSelectorTo.Bottom + num2);
			base.Height = picturePanelPreview.Bottom + 80;
			paletteSelectorFrom.Refresh();
			paletteSelectorTo.Refresh();
			BytemapPreview = Bytemap.Clone();
		}
	}

	private void ResetPaletteSelectorColor()
	{
		for (int i = 0; i < FromSize; i++)
		{
			int num = FromPaletteSet * FromSize + i;
			int num2 = FromPaletteSet * FromSize + ToPalette[i];
			paletteSelectorFrom.Palette[i] = DatInfo.Colors[num];
			paletteSelectorTo.Palette[i] = DatInfo.Colors[num2];
		}
	}

	private void paletteSelectorFrom_OnPaletteSelect(object sender, EventArgs e)
	{
		int selectedIndex = paletteSelectorTo.SelectedIndex;
		byte value = (byte)paletteSelectorFrom.SelectedIndex;
		SetNewValue(selectedIndex, value);
		paletteSelectorFrom.Refresh();
		paletteSelectorTo.Refresh();
	}

	private void paletteSelectorTo_OnPaletteSelect(object sender, EventArgs e)
	{
		int selectedIndex = paletteSelectorTo.SelectedIndex;
		byte selectedIndex2 = ToPalette[selectedIndex];
		paletteSelectorFrom.SelectedIndex = selectedIndex2;
		paletteSelectorFrom.Refresh();
		paletteSelectorTo.Refresh();
	}

	private void buttonOK_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
	}

	private void buttonCancel_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
	}

	private void ReplaceColorForm_KeyDown(object sender, KeyEventArgs e)
	{
		Keys keyCode = e.KeyCode;
		bool flag = false;
		byte b = 0;
		if (keyCode >= Keys.D0 && keyCode <= Keys.D9)
		{
			b = (byte)(keyCode - 48);
			flag = true;
		}
		if (keyCode >= Keys.NumPad0 && keyCode <= Keys.NumPad9)
		{
			b = (byte)(keyCode - 96);
			flag = true;
		}
		if (keyCode >= Keys.A && keyCode <= Keys.F)
		{
			b = (byte)(keyCode - 65 + 10);
			flag = true;
		}
		if (flag && b < FromSize)
		{
			int selectedIndex = paletteSelectorTo.SelectedIndex;
			SetNewValue(selectedIndex, b);
			paletteSelectorTo.SelectedIndex = (paletteSelectorTo.SelectedIndex + 1) % FromSize;
			paletteSelectorFrom.Refresh();
			paletteSelectorTo.Refresh();
		}
	}

	private void SetNewValue(int index, byte value)
	{
		string text = "X";
		if (FromSize > 16)
		{
			text = "X2";
		}
		ToPalette[index] = value;
		paletteSelectorTo.Label[index] = value.ToString(text);
		ResetPaletteSelectorColor();
		picturePanelPreview.Refresh();
	}

	private void picturePanelSource_Paint(object sender, PaintEventArgs e)
	{
		Bytemap bytemap = Bytemap;
		if (bytemap != null)
		{
			Graphics graphics = e.Graphics;
			Rectangle selectedRect = SelectedRect;
			PicturePanel panel = sender as PicturePanel;
			DrawBytemap(graphics, panel, bytemap, selectedRect);
		}
	}

	private void picturePanelPreview_Paint(object sender, PaintEventArgs e)
	{
		Bytemap bytemapPreview = BytemapPreview;
		if (bytemapPreview != null)
		{
			Graphics graphics = e.Graphics;
			Rectangle selectedRect = SelectedRect;
			PicturePanel panel = sender as PicturePanel;
			Rectangle rectangle = new Rectangle(0, 0, bytemapPreview.Width, bytemapPreview.Height);
			bytemapPreview.CopyRect(rectangle, Bytemap, rectangle);
			bytemapPreview.ReplaceColor(selectedRect, ToPalette);
			DrawBytemap(graphics, panel, bytemapPreview, selectedRect);
		}
	}

	private void DrawBytemap(Graphics g, PicturePanel panel, Bytemap bytemap, Rectangle rect)
	{
		GraphicsEx.InitGraphics(g);
		using (Bitmap bitmap = new Bitmap(bytemap.Width, bytemap.Height, PixelFormat.Format8bppIndexed))
		{
			BytemapConvertor.UpdateBitmapAllFromBytemap(bitmap, bytemap);
			Rectangle destRect = new Rectangle(0, 0, panel.ClientSize.Width, panel.ClientSize.Height);
			GraphicsEx.DrawImage(srcRect: new Rectangle(0, 0, bitmap.Width, bitmap.Height), g: g, image: bitmap, destRect: destRect);
		}
		float num = panel.Width / bytemap.Width;
		g.DrawRectangle(rect: new Rectangle((int)((float)rect.Left * num), (int)((float)rect.Top * num), (int)((float)rect.Width * num) - 1, (int)((float)rect.Height * num) - 1), pen: Pens.White);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(YYCHR.Forms.ReplaceColorForm));
		this.buttonOK = new System.Windows.Forms.Button();
		this.buttonCancel = new System.Windows.Forms.Button();
		this.labelFromColor = new System.Windows.Forms.Label();
		this.labelToColor = new System.Windows.Forms.Label();
		this.labelHint = new System.Windows.Forms.Label();
		this.picturePanelPreview = new ControlLib.PicturePanel();
		this.picturePanelSource = new ControlLib.PicturePanel();
		this.paletteSelectorTo = new ControlLib.PaletteSelector();
		this.paletteSelectorFrom = new ControlLib.PaletteSelector();
		this.labelNoteReplaceChange = new System.Windows.Forms.Label();
		base.SuspendLayout();
		resources.ApplyResources(this.buttonOK, "buttonOK");
		this.buttonOK.Name = "buttonOK";
		this.buttonOK.UseVisualStyleBackColor = true;
		this.buttonOK.Click += new System.EventHandler(buttonOK_Click);
		resources.ApplyResources(this.buttonCancel, "buttonCancel");
		this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.buttonCancel.Name = "buttonCancel";
		this.buttonCancel.UseVisualStyleBackColor = true;
		this.buttonCancel.Click += new System.EventHandler(buttonCancel_Click);
		resources.ApplyResources(this.labelFromColor, "labelFromColor");
		this.labelFromColor.Name = "labelFromColor";
		resources.ApplyResources(this.labelToColor, "labelToColor");
		this.labelToColor.Name = "labelToColor";
		resources.ApplyResources(this.labelHint, "labelHint");
		this.labelHint.Name = "labelHint";
		resources.ApplyResources(this.picturePanelPreview, "picturePanelPreview");
		this.picturePanelPreview.BackColor = System.Drawing.Color.Black;
		this.picturePanelPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.picturePanelPreview.Name = "picturePanelPreview";
		this.picturePanelPreview.Paint += new System.Windows.Forms.PaintEventHandler(picturePanelPreview_Paint);
		resources.ApplyResources(this.picturePanelSource, "picturePanelSource");
		this.picturePanelSource.BackColor = System.Drawing.Color.Black;
		this.picturePanelSource.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.picturePanelSource.Name = "picturePanelSource";
		this.picturePanelSource.Paint += new System.Windows.Forms.PaintEventHandler(picturePanelSource_Paint);
		resources.ApplyResources(this.paletteSelectorTo, "paletteSelectorTo");
		this.paletteSelectorTo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.paletteSelectorTo.CellColumnCount = 16;
		this.paletteSelectorTo.CellColumnView = 16;
		this.paletteSelectorTo.CellHeight = 16;
		this.paletteSelectorTo.CellRowCount = 16;
		this.paletteSelectorTo.CellRowView = 16;
		this.paletteSelectorTo.CellWidth = 16;
		this.paletteSelectorTo.ColorCount = 256;
		this.paletteSelectorTo.LabelItem = ControlLib.LabelItem.LabelsProperty;
		this.paletteSelectorTo.LabelStyle = ControlLib.LabelStyle.All;
		this.paletteSelectorTo.Name = "paletteSelectorTo";
		this.paletteSelectorTo.Palette = new System.Drawing.Color[256]
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
		this.paletteSelectorTo.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.paletteSelectorTo.SelectedIndex = 0;
		this.paletteSelectorTo.SetSize = 0;
		this.paletteSelectorTo.ShowSetRect = true;
		this.paletteSelectorTo.OnPaletteSelect += new ControlLib.PaletteSelector.OnPaletteSelectHandler(paletteSelectorTo_OnPaletteSelect);
		resources.ApplyResources(this.paletteSelectorFrom, "paletteSelectorFrom");
		this.paletteSelectorFrom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.paletteSelectorFrom.CellColumnCount = 16;
		this.paletteSelectorFrom.CellColumnView = 16;
		this.paletteSelectorFrom.CellHeight = 16;
		this.paletteSelectorFrom.CellRowCount = 16;
		this.paletteSelectorFrom.CellRowView = 16;
		this.paletteSelectorFrom.CellWidth = 16;
		this.paletteSelectorFrom.ColorCount = 256;
		this.paletteSelectorFrom.LabelItem = ControlLib.LabelItem.LabelsProperty;
		this.paletteSelectorFrom.LabelStyle = ControlLib.LabelStyle.All;
		this.paletteSelectorFrom.Name = "paletteSelectorFrom";
		this.paletteSelectorFrom.Palette = new System.Drawing.Color[256]
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
		this.paletteSelectorFrom.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.paletteSelectorFrom.SelectedIndex = 0;
		this.paletteSelectorFrom.SetSize = 0;
		this.paletteSelectorFrom.ShowSetRect = true;
		this.paletteSelectorFrom.OnPaletteSelect += new ControlLib.PaletteSelector.OnPaletteSelectHandler(paletteSelectorFrom_OnPaletteSelect);
		resources.ApplyResources(this.labelNoteReplaceChange, "labelNoteReplaceChange");
		this.labelNoteReplaceChange.Name = "labelNoteReplaceChange";
		base.AcceptButton = this.buttonOK;
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
		base.CancelButton = this.buttonCancel;
		base.Controls.Add(this.labelNoteReplaceChange);
		base.Controls.Add(this.picturePanelPreview);
		base.Controls.Add(this.picturePanelSource);
		base.Controls.Add(this.labelHint);
		base.Controls.Add(this.labelToColor);
		base.Controls.Add(this.labelFromColor);
		base.Controls.Add(this.buttonCancel);
		base.Controls.Add(this.buttonOK);
		base.Controls.Add(this.paletteSelectorTo);
		base.Controls.Add(this.paletteSelectorFrom);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.KeyPreview = true;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "ReplaceColorForm";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.Shown += new System.EventHandler(ReplaceColorForm_Shown);
		base.KeyDown += new System.Windows.Forms.KeyEventHandler(ReplaceColorForm_KeyDown);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
