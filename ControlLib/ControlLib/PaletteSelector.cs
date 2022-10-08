using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ControlLib;

public class PaletteSelector : UserControl
{
	public delegate void OnPaletteSelectHandler(object sender, EventArgs e);

	public delegate void OnPaletteSelectedHandler(object sender, EventArgs e);

	public delegate void OnPaletteSetChangedHandler(object sender, EventArgs e);

	private const int PAL_NUM = 256;

	private Color[] mPalette = new Color[256];

	private string[] mLabel = new string[256];

	private int mColorCount = 256;

	private int mCellColumnCount = 16;

	private int mCellRowCount = 16;

	private int mCellColumnView = 16;

	private int mCellRowView = 16;

	private int mCellWidth = 16;

	private int mCellHeight = 16;

	private ScrollBars mScrollBars = ScrollBars.Vertical;

	private int mSelectedIndex;

	private int setSize;

	private bool mSelectedSetChanged;

	private LabelItem mLabelItem;

	private LabelStyle mLabelStyle = LabelStyle.SelectedAll;

	private MouseButtons mMouseButtons;

	private IContainer components;

	private PictureBox pictureBox;

	private HScrollBar hScrollBar;

	private VScrollBar vScrollBar;

	public Color[] Palette
	{
		get
		{
			return mPalette;
		}
		set
		{
			int num = value.Length;
			if (num >= mPalette.Length)
			{
				num = mPalette.Length;
			}
			for (int i = 0; i < num; i++)
			{
				mPalette[i] = value[i];
			}
		}
	}

	[Browsable(false)]
	public string[] Label
	{
		get
		{
			return mLabel;
		}
		set
		{
			int num = value.Length;
			if (num >= mLabel.Length)
			{
				num = mLabel.Length;
			}
			for (int i = 0; i < num; i++)
			{
				mLabel[i] = value[i];
			}
			RedrawImage();
		}
	}

	[Browsable(false)]
	public byte[] PaletteFlags { get; set; } = new byte[256];


	public int ColorCount
	{
		get
		{
			return mColorCount;
		}
		set
		{
			mColorCount = value;
			RedrawImage();
		}
	}

	public int CellColumnCount
	{
		get
		{
			return mCellColumnCount;
		}
		set
		{
			mCellColumnCount = value;
			UpdateScrollBars();
			UpdateControlSize();
			RedrawControl();
		}
	}

	public int CellRowCount
	{
		get
		{
			return mCellRowCount;
		}
		set
		{
			mCellRowCount = value;
			UpdateScrollBars();
			UpdateControlSize();
			RedrawControl();
		}
	}

	public int CellColumnView
	{
		get
		{
			return mCellColumnView;
		}
		set
		{
			mCellColumnView = value;
			UpdateScrollBars();
			UpdateControlSize();
			RedrawControl();
		}
	}

	public int CellRowView
	{
		get
		{
			return mCellRowView;
		}
		set
		{
			mCellRowView = value;
			UpdateScrollBars();
			UpdateControlSize();
			RedrawControl();
		}
	}

	public int CellViewNum => CellColumnView * CellRowView;

	public int CellWidth
	{
		get
		{
			return mCellWidth;
		}
		set
		{
			mCellWidth = value;
			UpdateControlSize();
			RedrawControl();
		}
	}

	public int CellHeight
	{
		get
		{
			return mCellHeight;
		}
		set
		{
			mCellHeight = value;
			UpdateControlSize();
			RedrawControl();
		}
	}

	public ScrollBars ScrollBars
	{
		get
		{
			return mScrollBars;
		}
		set
		{
			mScrollBars = value;
			hScrollBar.Visible = mScrollBars == ScrollBars.Horizontal || mScrollBars == ScrollBars.Both;
			vScrollBar.Visible = mScrollBars == ScrollBars.Vertical || mScrollBars == ScrollBars.Both;
			UpdateControlSize();
			RedrawControl();
		}
	}

	public int SelectedIndex
	{
		get
		{
			return mSelectedIndex;
		}
		set
		{
			mSelectedIndex = value;
		}
	}

	[Browsable(false)]
	public Color SelectedColor => mPalette[mSelectedIndex];

	public int SetSize
	{
		get
		{
			return setSize;
		}
		set
		{
			setSize = value;
			RedrawImage();
			if (this.OnPaletteSetChanged != null)
			{
				this.OnPaletteSetChanged(this, EventArgs.Empty);
			}
		}
	}

	[Browsable(false)]
	public int SelectedSet
	{
		get
		{
			if (setSize == 0)
			{
				return 0;
			}
			return mSelectedIndex / setSize;
		}
	}

	[Browsable(false)]
	public bool PaletteSetChanged => mSelectedSetChanged;

	public LabelItem LabelItem
	{
		get
		{
			return mLabelItem;
		}
		set
		{
			mLabelItem = value;
			RedrawImage();
		}
	}

	public LabelStyle LabelStyle
	{
		get
		{
			return mLabelStyle;
		}
		set
		{
			mLabelStyle = value;
			RedrawImage();
		}
	}

	public bool ShowSetRect { get; set; } = true;


	public bool EnableSetUnknownMarkR { get; set; }

	public bool EnableSelectR { get; set; } = true;


	public bool EnableSelectM { get; set; }

	public bool EnableMultiSelect { get; set; }

	public bool DrawMultiSelect { get; set; }

	public int SelectEndIndex { get; set; } = 1;


	public int SelectedStart => Math.Min(SelectedIndex, SelectEndIndex);

	public int SelectedEnd => Math.Max(SelectedIndex, SelectEndIndex);

	public event OnPaletteSelectHandler OnPaletteSelect;

	public event OnPaletteSelectedHandler OnPaletteSelected;

	public event OnPaletteSetChangedHandler OnPaletteSetChanged;

	public event EventHandler PopupEditor;

	public PaletteSelector()
	{
		InitializeComponent();
		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
		SetStyle(ControlStyles.Selectable, value: false);
		if (!base.DesignMode)
		{
			ScrollBars = ScrollBars.Vertical;
			base.MouseWheel += PalettePanel_MouseWheel;
		}
	}

	private void UpdateControlSize()
	{
		int num = CellColumnView * CellWidth;
		int num2 = CellRowView * CellHeight;
		int num3 = (vScrollBar.Visible ? vScrollBar.Width : 0);
		int num4 = (hScrollBar.Visible ? hScrollBar.Height : 0);
		int num5 = 0;
		int num6 = 0;
		if (base.BorderStyle == BorderStyle.None)
		{
			num5 = 0;
			num6 = 0;
		}
		else
		{
			num5 = 4;
			num6 = 4;
		}
		pictureBox.Size = new Size(num, num2);
		base.Size = new Size(num + num3 + num5, num2 + num4 + num6);
	}

	private void UpdateScrollBars()
	{
		hScrollBar.LargeChange = CellColumnView;
		hScrollBar.Maximum = CellColumnCount - 1;
		hScrollBar.Enabled = UpdateScrollBarRange(hScrollBar);
		vScrollBar.LargeChange = CellRowView;
		vScrollBar.Maximum = CellRowCount - 1;
		vScrollBar.Enabled = UpdateScrollBarRange(vScrollBar);
	}

	private bool UpdateScrollBarRange(ScrollBar scrollBar)
	{
		int num = scrollBar.Value;
		if (num > scrollBar.Maximum - scrollBar.LargeChange)
		{
			num = scrollBar.Maximum - scrollBar.LargeChange;
		}
		if (num < 0)
		{
			num = 0;
		}
		if (num != scrollBar.Value)
		{
			scrollBar.Value = num;
		}
		if (scrollBar.LargeChange >= scrollBar.Maximum)
		{
			return false;
		}
		return true;
	}

	private void RedrawImage()
	{
		pictureBox.Refresh();
	}

	private void RedrawControl()
	{
		Refresh();
	}

	public void SelectPaletteWithScroll(byte newIndex)
	{
		int selectedIndex = SelectedIndex;
		SelectedIndex = newIndex;
		try
		{
			int cellColumnView = CellColumnView;
			int cellRowView = CellRowView;
			int num = cellColumnView * cellRowView;
			int num2 = vScrollBar.Value * cellColumnView;
			int num3 = num2 + num;
			int num4 = ((newIndex < selectedIndex) ? (-cellColumnView) : cellColumnView);
			for (int i = 0; i < 16; i++)
			{
				if (num2 <= newIndex && newIndex < num3)
				{
					vScrollBar.Value = num2 / cellColumnView;
					break;
				}
				num2 += num4;
				num3 = num2 + num;
				if (num2 < 0 || num2 >= 256)
				{
					break;
				}
			}
		}
		catch
		{
		}
	}

	private void PalettePanel_Load(object sender, EventArgs e)
	{
		UpdateScrollBars();
		UpdateControlSize();
		RedrawControl();
	}

	private void ScroolBarChanged(object sender, EventArgs e)
	{
		RedrawImage();
	}

	private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
	{
		ScrollBar scrollBar = (ScrollBar)sender;
		int num = scrollBar.Maximum + 1 - scrollBar.LargeChange;
		if (e.NewValue > num)
		{
			e.NewValue = num;
		}
	}

	protected override void OnEnter(EventArgs e)
	{
		base.OnEnter(e);
		BorderRenderer.NCPaint(this, focused: true);
	}

	protected override void OnLeave(EventArgs e)
	{
		BorderRenderer.NCPaint(this, focused: false);
		base.OnLeave(e);
	}

	protected override void WndProc(ref Message m)
	{
		base.WndProc(ref m);
		if (m.Msg == 133)
		{
			BorderRenderer.NCPaint(this, Focused);
		}
	}

	private void pictureBox_Paint(object sender, PaintEventArgs e)
	{
		Graphics graphics = e.Graphics;
		int num = mPalette.Length;
		if (num >= mColorCount)
		{
			num = mColorCount;
		}
		Color white = Color.White;
		Color color = Color.FromArgb(160, 0, 0, 0);
		Color color2 = Color.Black;
		Color color3 = Color.Red;
		if (!base.Enabled)
		{
			color2 = GetDisabledColor(color2);
			color3 = GetDisabledColor(color3);
		}
		using Brush brush2 = new SolidBrush(color2);
		using Pen pen5 = new Pen(color3);
		using Pen pen4 = new Pen(white);
		using Pen pen3 = new Pen(color);
		using Pen pen = new Pen(Color.Red);
		using Pen pen2 = new Pen(Color.Aqua);
		int num2 = SelectedSet * SetSize;
		int num3 = num2 + SetSize;
		_ = num2 % CellColumnCount;
		_ = num2 / CellColumnCount;
		_ = (num3 - 1) % CellColumnCount;
		_ = (num3 - 1) / CellColumnCount;
		int num4 = 0;
		int num5 = 0;
		int value = hScrollBar.Value;
		int value2 = vScrollBar.Value;
		for (int i = value2; i < value2 + CellRowView; i++)
		{
			for (int j = value; j < value + CellColumnView; j++)
			{
				num4 = i * CellColumnCount + j;
				if (num4 >= num)
				{
					break;
				}
				int num6 = num5 % CellColumnCount;
				int num7 = num5 / CellColumnCount;
				Rectangle rect = new Rectangle(num6 * CellWidth, num7 * CellHeight, CellWidth, CellHeight);
				bool flag = true;
				Color color4 = mPalette[num4];
				if (!base.Enabled)
				{
					color4 = GetDisabledColor(color4);
				}
				using (Brush brush = new SolidBrush(color4))
				{
					graphics.FillRectangle(brush, rect);
				}
				if (ShowSetRect && num4 >= num2 && num4 < num3)
				{
					bool flag2 = false;
					bool flag3 = false;
					bool flag4 = false;
					bool flag5 = false;
					if (num4 == num2 || num4 % CellColumnCount == 0)
					{
						flag2 = true;
					}
					if (num4 - CellColumnCount < num2)
					{
						flag4 = true;
					}
					if (num4 == num3 - 1 || num4 % CellColumnCount == CellColumnCount - 1)
					{
						flag3 = true;
					}
					if (num4 + CellColumnCount >= num3)
					{
						flag5 = true;
					}
					int left = rect.Left;
					int top = rect.Top;
					int num8 = rect.Right - 1;
					int num9 = rect.Bottom - 1;
					if (flag2)
					{
						graphics.DrawLine(pen, left, top, left, num9);
					}
					if (flag4)
					{
						graphics.DrawLine(pen, left, top, num8, top);
					}
					if (flag3)
					{
						graphics.DrawLine(pen, num8, top, num8, num9);
					}
					if (flag5)
					{
						graphics.DrawLine(pen, left, num9, num8, num9);
					}
				}
				if (DrawMultiSelect)
				{
					if (num4 >= SelectedStart && num4 <= SelectedEnd)
					{
						bool flag6 = false;
						bool flag7 = false;
						bool flag8 = false;
						bool flag9 = false;
						if (num4 == SelectedStart)
						{
							flag6 = true;
						}
						if (num4 == SelectedEnd)
						{
							flag7 = true;
						}
						if (num4 - CellColumnCount < SelectedStart)
						{
							flag8 = true;
						}
						if (num4 + CellColumnCount > SelectedEnd)
						{
							flag9 = true;
						}
						int left2 = rect.Left;
						int top2 = rect.Top;
						int num10 = rect.Right - 1;
						int num11 = rect.Bottom - 1;
						int num12 = left2 + 1;
						int num13 = top2 + 1;
						int num14 = num10 - 1;
						int num15 = num11 - 1;
						if (flag8)
						{
							graphics.DrawLine(pen2, left2, top2, num10, top2);
							graphics.DrawLine(pen3, left2, num13, num10, num13);
						}
						if (flag9)
						{
							graphics.DrawLine(pen2, left2, num11, num10, num11);
							graphics.DrawLine(pen3, left2, num15, num10, num15);
						}
						if (flag6)
						{
							graphics.DrawLine(pen2, left2, top2, left2, num11);
							graphics.DrawLine(pen3, num12, num13, num12, num15);
						}
						if (flag7)
						{
							graphics.DrawLine(pen2, num10, top2, num10, num11);
							graphics.DrawLine(pen3, num14, num13, num14, num15);
						}
					}
				}
				else if (mSelectedIndex == num4)
				{
					graphics.DrawRectangle(pen4, rect.Left, rect.Top, CellWidth - 1, CellHeight - 1);
					graphics.DrawRectangle(pen3, rect.Left + 1, rect.Top + 1, CellWidth - 3, CellHeight - 3);
				}
				if (num4 >= 0 && num4 < PaletteFlags.Length)
				{
					byte num16 = PaletteFlags[num4];
					if (((uint)num16 & (true ? 1u : 0u)) != 0)
					{
						Rectangle rect2 = new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width - 2, rect.Height - 2);
						graphics.FillRectangle(brush2, rect2);
						graphics.DrawLine(pen5, rect.Left, rect.Top, rect.Right, rect.Bottom);
						graphics.DrawLine(pen5, rect.Left, rect.Bottom, rect.Right, rect.Top);
					}
					if ((num16 & 2u) != 0)
					{
						DrawLabel(graphics, rect, "?");
						flag = false;
					}
					if ((num16 & 4u) != 0)
					{
						Rectangle rect3 = new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width - 2, rect.Height - 2);
						graphics.FillRectangle(brush2, rect3);
						int num17 = rect.Top + 1;
						int num18 = rect.Bottom - 2;
						int x = rect.Left + 1;
						int x2 = rect.Right - 2;
						int num19 = rect3.Width / 2 + rect.Left;
						graphics.DrawLine(pen5, x, num18, num19, num17);
						graphics.DrawLine(pen5, num19, num17, x2, num18);
						graphics.DrawLine(pen5, x, num18, x2, num18);
					}
				}
				bool flag10 = num4 == mSelectedIndex;
				bool flag11 = num2 <= num4 && num4 < num3;
				bool flag12 = false;
				if (LabelStyle == LabelStyle.All)
				{
					flag12 = true;
				}
				else if (LabelStyle == LabelStyle.SelectedAll && (flag10 || flag11))
				{
					flag12 = true;
				}
				else if (LabelStyle == LabelStyle.SelectedSet && flag11)
				{
					flag12 = true;
				}
				else if (LabelStyle == LabelStyle.Selected && flag10)
				{
					flag12 = true;
				}
				if (!flag)
				{
					flag12 = false;
				}
				if (flag12)
				{
					DrawLabel(graphics, rect, num4);
				}
				num5++;
			}
			if (num4 >= num)
			{
				break;
			}
		}
	}

	private Color GetDisabledColor(Color color)
	{
		return Color.FromArgb(color.A, (color.R * 2 + 256) / 4, (color.G * 2 + 256) / 4, (color.B * 2 + 256) / 4);
	}

	private void DrawLabel(Graphics g, Rectangle rect, int palIndex)
	{
		string text = palIndex.ToString("X2");
		if (LabelItem == LabelItem.LabelsProperty)
		{
			text = Label[palIndex];
		}
		DrawLabel(g, rect, text);
	}

	private void DrawLabel(Graphics g, Rectangle rect, string text)
	{
		Font font = Font;
		Color[] textColorFromBackColor = GraphicsEx.GetTextColorFromBackColor(Color.Black);
		Color color = textColorFromBackColor[0];
		Color color2 = textColorFromBackColor[1];
		using Brush brushOuter = new SolidBrush(color);
		using Brush brushInner = new SolidBrush(color2);
		using StringFormat stringFormat = new StringFormat();
		Rectangle rect2 = new Rectangle(rect.Left - 2, rect.Top, rect.Width + 4, rect.Height);
		stringFormat.Alignment = StringAlignment.Center;
		stringFormat.LineAlignment = StringAlignment.Center;
		stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit;
		GraphicsEx.DrawString8(g, text, font, brushOuter, brushInner, rect2, stringFormat);
	}

	private void PalettePanel_MouseWheel(object sender, MouseEventArgs e)
	{
		ScrollBar scrollBar = null;
		scrollBar = ((!vScrollBar.Visible) ? ((ScrollBar)hScrollBar) : ((ScrollBar)vScrollBar));
		int value = scrollBar.Value;
		int smallChange = scrollBar.SmallChange;
		int minimum = scrollBar.Minimum;
		int num = scrollBar.Maximum + 1 - scrollBar.LargeChange;
		value = ((e.Delta <= 0) ? (value + smallChange) : (value - smallChange));
		if (value < minimum)
		{
			value = minimum;
		}
		if (value > num)
		{
			value = num;
		}
		scrollBar.Value = value;
	}

	private void pictureBox_MouseDown(object sender, MouseEventArgs e)
	{
		if (!base.Enabled)
		{
			return;
		}
		if (base.CanSelect)
		{
			Focus();
		}
		mMouseButtons = e.Button;
		DrawMultiSelect = false;
		if (EnableMultiSelect && e.Button == MouseButtons.Right)
		{
			DrawMultiSelect = true;
			int paletteIndex = GetPaletteIndex(sender, e);
			if (SelectedIndex != paletteIndex)
			{
				SelectedIndex = paletteIndex;
				RedrawImage();
				if (this.OnPaletteSelect != null)
				{
					this.OnPaletteSelect(sender, EventArgs.Empty);
				}
			}
		}
		if (!EnableMultiSelect && EnableSetUnknownMarkR && e.Button == MouseButtons.Right)
		{
			int paletteIndex2 = GetPaletteIndex(sender, e);
			if (SelectedIndex != paletteIndex2)
			{
				SelectedIndex = paletteIndex2;
				if (this.OnPaletteSelect != null)
				{
					this.OnPaletteSelect(sender, EventArgs.Empty);
				}
			}
			byte b = PaletteFlags[paletteIndex2];
			b = (((b & 2) == 0) ? ((byte)(b | 2u)) : ((byte)(b ^ 2u)));
			if (PaletteFlags[paletteIndex2] != b)
			{
				PaletteFlags[paletteIndex2] = b;
			}
			RedrawImage();
		}
		OnMouseDown(e);
		pictureBox_MouseMove(sender, e);
	}

	private void pictureBox_MouseMove(object sender, MouseEventArgs e)
	{
		if (!base.Enabled)
		{
			return;
		}
		if (EnableMultiSelect && mMouseButtons == MouseButtons.Right)
		{
			int paletteIndex = GetPaletteIndex(sender, e);
			if (SelectEndIndex != paletteIndex)
			{
				SelectEndIndex = paletteIndex;
				RedrawImage();
				if (this.OnPaletteSelect != null)
				{
					this.OnPaletteSelect(sender, EventArgs.Empty);
				}
			}
		}
		else if (mMouseButtons == MouseButtons.Left || (EnableSelectR && mMouseButtons == MouseButtons.Right) || (EnableSelectM && mMouseButtons == MouseButtons.Middle))
		{
			int paletteIndex2 = GetPaletteIndex(sender, e);
			bool flag = false;
			if (mSelectedIndex != paletteIndex2)
			{
				flag = true;
				int selectedSet = SelectedSet;
				mSelectedIndex = paletteIndex2;
				SelectEndIndex = SelectedIndex;
				if (selectedSet != SelectedSet)
				{
					mSelectedSetChanged = true;
				}
			}
			if (flag || mSelectedSetChanged)
			{
				RedrawImage();
			}
			if (flag && this.OnPaletteSelect != null)
			{
				this.OnPaletteSelect(sender, EventArgs.Empty);
			}
			if (mSelectedSetChanged && this.OnPaletteSetChanged != null)
			{
				this.OnPaletteSetChanged(sender, EventArgs.Empty);
			}
		}
		base.OnMouseMove(e);
	}

	private void pictureBox_MouseUp(object sender, MouseEventArgs e)
	{
		if (base.Enabled)
		{
			if ((mMouseButtons & e.Button) > MouseButtons.None)
			{
				mMouseButtons ^= e.Button;
			}
			if (e.Button == MouseButtons.Right && this.PopupEditor != null)
			{
				this.PopupEditor(this, EventArgs.Empty);
			}
			if (e.Button == MouseButtons.Left && this.OnPaletteSelected != null)
			{
				this.OnPaletteSelected(sender, EventArgs.Empty);
			}
			base.OnMouseUp(e);
		}
	}

	private int GetPaletteIndex(object sender, MouseEventArgs e)
	{
		int num = e.X;
		int num2 = e.Y;
		_ = (PictureBox)sender;
		Rectangle rectangle = new Rectangle(0, 0, pictureBox.Width, pictureBox.Height);
		if (num < rectangle.Left)
		{
			num = rectangle.Left;
		}
		if (num2 < rectangle.Top)
		{
			num2 = rectangle.Top;
		}
		if (num >= rectangle.Right)
		{
			num = rectangle.Right - 1;
		}
		if (num2 >= rectangle.Bottom)
		{
			num2 = rectangle.Bottom - 1;
		}
		int num3 = num / mCellWidth + hScrollBar.Value;
		return (num2 / mCellHeight + vScrollBar.Value) * mCellColumnCount + num3;
	}

	private void pictureBox_MouseEnter(object sender, EventArgs e)
	{
		base.OnMouseEnter(e);
	}

	private void pictureBox_MouseLeave(object sender, EventArgs e)
	{
		base.OnMouseLeave(e);
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
		this.pictureBox = new System.Windows.Forms.PictureBox();
		this.hScrollBar = new System.Windows.Forms.HScrollBar();
		this.vScrollBar = new System.Windows.Forms.VScrollBar();
		((System.ComponentModel.ISupportInitialize)this.pictureBox).BeginInit();
		base.SuspendLayout();
		this.pictureBox.Location = new System.Drawing.Point(0, 0);
		this.pictureBox.MinimumSize = new System.Drawing.Size(8, 8);
		this.pictureBox.Name = "pictureBox";
		this.pictureBox.Size = new System.Drawing.Size(334, 145);
		this.pictureBox.TabIndex = 0;
		this.pictureBox.TabStop = false;
		this.pictureBox.Paint += new System.Windows.Forms.PaintEventHandler(pictureBox_Paint);
		this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(pictureBox_MouseDown);
		this.pictureBox.MouseEnter += new System.EventHandler(pictureBox_MouseEnter);
		this.pictureBox.MouseLeave += new System.EventHandler(pictureBox_MouseLeave);
		this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(pictureBox_MouseMove);
		this.pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(pictureBox_MouseUp);
		this.hScrollBar.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.hScrollBar.Location = new System.Drawing.Point(-2, 159);
		this.hScrollBar.Name = "hScrollBar";
		this.hScrollBar.Size = new System.Drawing.Size(344, 16);
		this.hScrollBar.TabIndex = 2;
		this.hScrollBar.ValueChanged += new System.EventHandler(ScroolBarChanged);
		this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
		this.vScrollBar.Location = new System.Drawing.Point(342, 0);
		this.vScrollBar.Name = "vScrollBar";
		this.vScrollBar.Size = new System.Drawing.Size(16, 175);
		this.vScrollBar.TabIndex = 1;
		this.vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(vScrollBar_Scroll);
		this.vScrollBar.ValueChanged += new System.EventHandler(ScroolBarChanged);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
		base.Controls.Add(this.vScrollBar);
		base.Controls.Add(this.hScrollBar);
		base.Controls.Add(this.pictureBox);
		base.Name = "PaletteSelector";
		base.Size = new System.Drawing.Size(358, 175);
		base.Load += new System.EventHandler(PalettePanel_Load);
		((System.ComponentModel.ISupportInitialize)this.pictureBox).EndInit();
		base.ResumeLayout(false);
	}
}
