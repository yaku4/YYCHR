using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using CharactorLib.Common;
using CharactorLib.Data;
using ControlLib;
using Controls;

namespace YYCHR.Forms;

public class PatternEditorForm : Form
{
	private enum SelectStep
	{
		None,
		Selecting,
		Selected,
		Moving,
		Moved
	}

	private AdfPattern mAdfPattern;

	private byte[] mPatternData;

	private int mAdfIndex;

	private bool mNewPatternSelected;

	private Bitmap mChrBitmap = new Bitmap(128, 128, PixelFormat.Format8bppIndexed);

	private Bitmap mPtnBitmap = new Bitmap(128, 128, PixelFormat.Format8bppIndexed);

	private Bytemap mChrBytemapOrg;

	private Bytemap mChrBytemap;

	private Bytemap mPtnBytemap = new Bytemap(128, 128);

	private Bytemap mBlankBytemap = new Bytemap(128, 128);

	private bool mNumberd = true;

	private bool mShowChr = true;

	private bool mNumberHalf;

	private bool mHalfBright = true;

	private Brush mHalfBrush = new SolidBrush(Color.FromArgb(128, 255, 255, 255));

	private Brush mBrush = Brushes.White;

	private Font mFont = new Font("Lucida Console", 8f);

	private StringFormat sFormat = new StringFormat(StringFormatFlags.NoWrap);

	private string NEW_PATTERN = "--- New Pattern ---";

	private Point mPtDown = new Point(0, 0);

	private Point mPtMove = new Point(0, 0);

	private Point mPtUp = new Point(0, 0);

	private Rectangle mSelRect;

	private MouseButtons mMouseButtonForMove;

	private byte[] mBasePattern;

	private SelectStep dbgPaintMode;

	private IContainer components;

	private GroupBox groupBox1;

	private Button buttonListMoveLower;

	private Button buttonListMoveUpper;

	private GroupBox groupBoxCreateCharPattern;

	private Button buttonListAdd;

	private Button buttonListDelete;

	private Button buttonClose;

	private CellSelector cellSelectorPtn;

	private CellSelector cellSelectorChr;

	private Label label1;

	private Label label2;

	private NumericUpDown numericUpDownCreateH;

	private NumericUpDown numericUpDownCreateW;

	private ListBox listBoxPattern;

	private Label labelCharDirection;

	private Label labelCharHeight;

	private Label labelCharWidth;

	private Button buttonSave;

	private TextBox textBoxAdfName;

	private Label labelAdfName;

	private ComboBoxEx comboBoxCreateDirection;

	private CheckBox checkBoxHalfBright;

	private CheckBox checkBoxShowNumber;

	private CheckBox checkBoxNumberHalf;

	private CheckBox checkBoxShowChr;

	private CheckBox checkBoxHideFF;

	private Label labelHintRightDragMove;

	public PatternEditorForm()
	{
		InitializeComponent();
		if (!base.DesignMode)
		{
			throw new NotSupportedException();
		}
	}

	public PatternEditorForm(AdfPattern adfPattern, Bytemap chrBytemap)
	{
		InitializeComponent();
		ResourceUtility.UpdateTextIfLngEnabled(this, null);
		DoubleBuffered = true;
		comboBoxCreateDirection.SelectedIndex = 0;
		mAdfPattern = adfPattern;
		mChrBytemapOrg = chrBytemap;
		mChrBytemap = mChrBytemapOrg.Clone();
		sFormat.Trimming = StringTrimming.None;
		sFormat.Alignment = StringAlignment.Near;
		sFormat.LineAlignment = StringAlignment.Center;
		UpdateList();
		UpdateControlValue();
		cellSelectorChr.Image = mChrBitmap;
		cellSelectorPtn.Image = mPtnBitmap;
		DrawPatterns();
	}

	private void actionClose(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
	}

	private void actionListMoveUpper(object sender, EventArgs e)
	{
		int num = 0;
		if (mAdfIndex > num)
		{
			int toIndex = mAdfIndex - 1;
			mAdfPattern.Move(mAdfIndex, toIndex);
			mAdfIndex = toIndex;
			UpdateList();
			DrawPatterns();
		}
	}

	private void actionListMoveLower(object sender, EventArgs e)
	{
		int num = mAdfPattern.AdfPatterns.Length - 1;
		if (mAdfIndex < num)
		{
			int toIndex = mAdfIndex + 1;
			mAdfPattern.Move(mAdfIndex, toIndex);
			mAdfIndex = toIndex;
			UpdateList();
			DrawPatterns();
		}
	}

	private void actionListDelete(object sender, EventArgs e)
	{
		if (mAdfPattern.AdfPatterns.Length > 1)
		{
			mAdfPattern.Remove(mAdfIndex);
			int num = mAdfPattern.AdfPatterns.Length - 1;
			if (mAdfIndex >= num)
			{
				mAdfIndex = num;
			}
			UpdateList();
			DrawPatterns();
		}
	}

	private void actionCreateNewPattern(object sender, EventArgs e)
	{
		CreateNewPattern();
	}

	private void CreateNewPattern()
	{
		if (mNewPatternSelected && mPatternData != null)
		{
			int maxX = 16;
			int maxY = 16;
			int num = (int)numericUpDownCreateW.Value;
			int num2 = (int)numericUpDownCreateH.Value;
			bool flag = comboBoxCreateDirection.SelectedIndex == 1;
			string text = ((!flag) ? "H" : "V");
			textBoxAdfName.Text = num + "," + num2 + text;
			MakeAdf(num, num2, maxX, maxY, flag);
			cellSelectorPtn.SelectedRect = new Rectangle(0, 0, num * 8, num2 * 8);
			DrawPatterns();
		}
	}

	private void MakeAdf(int charX, int charY, int maxX, int maxY, bool vertical)
	{
		for (int i = 0; i < 256; i++)
		{
			mPatternData[i] = 0;
		}
		int num = maxX / charX;
		int num2 = maxY / charY;
		int num3 = 0;
		for (int j = 0; j < num2; j++)
		{
			for (int k = 0; k < num; k++)
			{
				if (vertical)
				{
					for (int l = 0; l < charX; l++)
					{
						for (int m = 0; m < charY; m++)
						{
							int num4 = k * charX + l;
							int num5 = (j * charY + m) * 16 + num4;
							mPatternData[num5] = (byte)num3++;
						}
					}
					continue;
				}
				for (int n = 0; n < charY; n++)
				{
					for (int num6 = 0; num6 < charX; num6++)
					{
						int num4 = k * charX + num6;
						int num5 = (j * charY + n) * 16 + num4;
						mPatternData[num5] = (byte)num3++;
					}
				}
			}
		}
	}

	private void actionListSelectedIndexChanged(object sender, EventArgs e)
	{
		int num = 0;
		int num2 = mAdfPattern.AdfPatterns.Length - 1;
		mAdfIndex = listBoxPattern.SelectedIndex;
		if (mAdfIndex >= num2)
		{
			mAdfIndex = num2;
		}
		if (mAdfIndex < num)
		{
			mAdfIndex = num;
		}
		mNewPatternSelected = mAdfIndex != listBoxPattern.SelectedIndex;
		buttonListMoveUpper.Enabled = mAdfIndex != num && !mNewPatternSelected;
		buttonListMoveLower.Enabled = mAdfIndex != num2 && !mNewPatternSelected;
		buttonListDelete.Enabled = mAdfPattern.AdfPatterns.Length >= 2 && !mNewPatternSelected;
		if (mNewPatternSelected)
		{
			CreateNewPattern();
		}
		else
		{
			LoadSelectedPatternData();
		}
		groupBoxCreateCharPattern.Enabled = mNewPatternSelected;
	}

	private void actionSave(object sender, EventArgs e)
	{
		if (mNewPatternSelected)
		{
			mAdfPattern.Add();
			int num = (mAdfIndex = mAdfPattern.AdfPatterns.Length - 1);
		}
		SaveSelectedPatternData();
	}

	private void UpdateControlValue()
	{
		if (checkBoxShowNumber.Checked != mNumberd)
		{
			checkBoxShowNumber.CheckedChanged -= actionShowNumberChanged;
			checkBoxShowNumber.Checked = mNumberd;
			checkBoxShowNumber.CheckedChanged += actionShowNumberChanged;
		}
		if (checkBoxShowChr.Checked != mShowChr)
		{
			checkBoxShowChr.CheckedChanged -= checkBoxShowChr_CheckedChanged;
			checkBoxShowChr.Checked = mShowChr;
			checkBoxShowChr.CheckedChanged += checkBoxShowChr_CheckedChanged;
		}
		if (checkBoxHalfBright.Checked != mHalfBright)
		{
			checkBoxHalfBright.CheckedChanged -= actionHalfBrightChanged;
			checkBoxHalfBright.Checked = mHalfBright;
			checkBoxHalfBright.CheckedChanged += actionHalfBrightChanged;
		}
		if (checkBoxNumberHalf.Checked != mNumberHalf)
		{
			checkBoxNumberHalf.CheckedChanged -= actionHalfBrightChanged;
			checkBoxNumberHalf.Checked = mNumberHalf;
			checkBoxNumberHalf.CheckedChanged += actionHalfBrightChanged;
		}
		checkBoxNumberHalf.Enabled = mNumberd;
		checkBoxHalfBright.Enabled = mShowChr;
	}

	private void actionShowNumberChanged(object sender, EventArgs e)
	{
		mNumberd = !mNumberd;
		if (!mNumberd && !mShowChr)
		{
			mShowChr = true;
		}
		UpdateControlValue();
		DrawPatterns();
	}

	private void actionNumberHalfChecked(object sender, EventArgs e)
	{
		mNumberHalf = !mNumberHalf;
		DrawPatterns();
	}

	private void checkBoxShowChr_CheckedChanged(object sender, EventArgs e)
	{
		mShowChr = !mShowChr;
		if (!mShowChr && !mNumberd)
		{
			mNumberd = true;
		}
		UpdateControlValue();
		DrawPatterns();
	}

	private void actionHalfBrightChanged(object sender, EventArgs e)
	{
		mHalfBright = !mHalfBright;
		DrawPatterns();
	}

	private void DrawPatterns()
	{
		mChrBytemap.SetPalette(mChrBytemapOrg.Palette, mHalfBright);
		mPtnBytemap.SetPalette(mChrBytemap.Palette);
		BytemapConvertor.UpdateBitmapPaletteFromBytemap(mChrBitmap, mChrBytemap);
		BytemapConvertor.UpdateBitmapPaletteFromBytemap(mPtnBitmap, mChrBytemap);
		if (mNumberHalf)
		{
			mBrush = mHalfBrush;
		}
		else
		{
			mBrush = Brushes.White;
		}
		DrawChr();
		DrawPtn();
	}

	private void actionPatternSelected(object sender, MouseEventArgs e)
	{
		byte b = (byte)cellSelectorPtn.SelectedIndex;
		byte selectedIndex = mPatternData[b];
		cellSelectorChr.SelectedIndex = selectedIndex;
		cellSelectorChr.Refresh();
		cellSelectorPtn.Refresh();
	}

	private void actionChrSelected(object sender, MouseEventArgs e)
	{
		byte b = (byte)cellSelectorPtn.SelectedIndex;
		byte b2 = (byte)cellSelectorChr.SelectedIndex;
		mPatternData[b] = b2;
		cellSelectorChr.Refresh();
		DrawPtn();
	}

	private void UpdateList()
	{
		NEW_PATTERN = "( " + groupBoxCreateCharPattern.Text + " )";
		listBoxPattern.Items.Clear();
		ListBox.ObjectCollection items = listBoxPattern.Items;
		object[] names = mAdfPattern.GetNames();
		items.AddRange(names);
		listBoxPattern.Items.Add(NEW_PATTERN);
		listBoxPattern.SelectedIndex = mAdfIndex;
		listBoxPattern.Refresh();
	}

	private void listBoxPattern_DrawItem(object sender, DrawItemEventArgs e)
	{
		if (e.Index < listBoxPattern.Items.Count)
		{
			string text = listBoxPattern.Items[e.Index].ToString();
			Color foreColor = (((e.State & DrawItemState.Selected) == DrawItemState.Selected) ? SystemColors.HighlightText : ((!(text == NEW_PATTERN)) ? SystemColors.ControlText : Color.Gray));
			e.DrawBackground();
			e.DrawFocusRectangle();
			TextRenderer.DrawText(e.Graphics, text, e.Font, e.Bounds, foreColor, TextFormatFlags.Default);
		}
	}

	private void LoadSelectedPatternData()
	{
		textBoxAdfName.Text = mAdfPattern.AdfPatterns[mAdfIndex].Name;
		checkBoxHideFF.Checked = mAdfPattern.AdfPatterns[mAdfIndex].IsDisableFF;
		mPatternData = mAdfPattern.AdfPatterns[mAdfIndex].Pattern.Clone() as byte[];
		cellSelectorPtn.SelectedRect = new Rectangle(0, 0, 8, 8);
		byte b = (byte)cellSelectorPtn.SelectedIndex;
		byte selectedIndex = mPatternData[b];
		cellSelectorChr.SelectedIndex = selectedIndex;
		DrawPtn();
	}

	private void SaveSelectedPatternData()
	{
		for (int i = 0; i < 256; i++)
		{
			mAdfPattern.AdfPatterns[mAdfIndex].Pattern[i] = mPatternData[i];
		}
		string text = textBoxAdfName.Text;
		mAdfPattern.AdfPatterns[mAdfIndex].Name = text;
		mAdfPattern.AdfPatterns[mAdfIndex].IsDisableFF = checkBoxHideFF.Checked;
		listBoxPattern.Items[mAdfIndex] = text;
		UpdateList();
	}

	private void DrawChr()
	{
		if (mShowChr)
		{
			BytemapConvertor.UpdateBitmapFromBytemap(mChrBitmap, mChrBytemap);
		}
		else
		{
			BytemapConvertor.UpdateBitmapFromBytemap(mChrBitmap, mBlankBytemap);
		}
		cellSelectorChr.Refresh();
	}

	private void DrawPtn()
	{
		if (mChrBytemap == null || mPatternData == null)
		{
			return;
		}
		int num = 8;
		int num2 = 8;
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				int num3 = i * 16 + j;
				byte num4 = mPatternData[num3];
				int num5 = (int)num4 % 16;
				int num6 = (int)num4 / 16;
				Point destPoint = new Point(j * num, i * num2);
				Rectangle srcRect = new Rectangle(num5 * num, num6 * num2, num, num2);
				mPtnBytemap.CopyRect(destPoint, mChrBytemap, srcRect);
			}
		}
		if (mShowChr)
		{
			BytemapConvertor.UpdateBitmapFromBytemap(mPtnBitmap, mPtnBytemap);
		}
		else
		{
			BytemapConvertor.UpdateBitmapFromBytemap(mPtnBitmap, mBlankBytemap);
		}
		cellSelectorPtn.Refresh();
	}

	private void cellSelectorChr_Paint(object sender, PaintEventArgs e)
	{
		if (!mNumberd)
		{
			return;
		}
		Graphics graphics = e.Graphics;
		int num = cellSelectorChr.ZoomRate * cellSelectorChr.DefaultSelectSize.Width;
		int num2 = cellSelectorChr.ZoomRate * cellSelectorChr.DefaultSelectSize.Height;
		int num3 = 0;
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				Rectangle rectangle = new Rectangle(j * num, i * num2, num, num2);
				string s = num3.ToString("X2");
				graphics.DrawString(s, mFont, mBrush, rectangle, sFormat);
				num3++;
			}
		}
	}

	private void cellSelectorPtn_Paint(object sender, PaintEventArgs e)
	{
		Graphics graphics = e.Graphics;
		_ = cellSelectorPtn.CellWidth;
		_ = cellSelectorChr.ZoomRate;
		_ = cellSelectorPtn.CellHeight;
		_ = cellSelectorChr.ZoomRate;
		if (mNumberd)
		{
			int num = cellSelectorChr.ZoomRate * cellSelectorChr.DefaultSelectSize.Width;
			int num2 = cellSelectorChr.ZoomRate * cellSelectorChr.DefaultSelectSize.Height;
			int num3 = 0;
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					Rectangle rectangle = new Rectangle(j * num, i * num2, num, num2);
					string s = mPatternData[num3].ToString("X2");
					graphics.DrawString(s, mFont, mBrush, rectangle, sFormat);
					num3++;
				}
			}
		}
		int zoomRate = cellSelectorChr.ZoomRate;
		Rectangle rectangle2 = mSelRect;
		if (dbgPaintMode >= SelectStep.Selecting)
		{
			Rectangle rect = Rectangle.FromLTRB(rectangle2.Left * zoomRate, rectangle2.Top * zoomRate, rectangle2.Right * zoomRate, rectangle2.Bottom * zoomRate);
			graphics.DrawRectangle(Pens.Aqua, rect);
		}
		if (dbgPaintMode >= SelectStep.Moving)
		{
			int num4 = (mPtMove.X - mPtDown.X) * cellSelectorPtn.CellWidth;
			int num5 = (mPtMove.Y - mPtDown.Y) * cellSelectorPtn.CellHeight;
			Rectangle rect2 = Rectangle.FromLTRB((rectangle2.Left + num4) * zoomRate, (rectangle2.Top + num5) * zoomRate, (rectangle2.Right + num4) * zoomRate, (rectangle2.Bottom + num5) * zoomRate);
			graphics.DrawRectangle(Pens.Yellow, rect2);
		}
	}

	private void SetMouseStep(SelectStep step)
	{
		dbgPaintMode = step;
		cellSelectorPtn.SelectorVisible = dbgPaintMode == SelectStep.None;
		cellSelectorChr.SelectorVisible = dbgPaintMode == SelectStep.None;
	}

	private bool MouseButtonDownForMove(MouseButtons mouseButton)
	{
		return (mouseButton & MouseButtons.Right) == MouseButtons.Right;
	}

	private void cellSelectorPtn_MouseDown(object sender, MouseEventArgs e)
	{
		if (MouseButtonDownForMove(e.Button))
		{
			int num = cellSelectorPtn.CellWidth * cellSelectorPtn.ZoomRate;
			int num2 = cellSelectorPtn.CellHeight * cellSelectorPtn.ZoomRate;
			int num3 = e.X / num;
			int num4 = e.Y / num2;
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num4 < 0)
			{
				num4 = 0;
			}
			mPtDown = new Point(num3, num4);
			mMouseButtonForMove = e.Button;
			if (mSelRect.Contains(e.X / cellSelectorPtn.ZoomRate, e.Y / cellSelectorPtn.ZoomRate))
			{
				if (mBasePattern != null)
				{
					mBasePattern = null;
				}
				mBasePattern = mPatternData.Clone() as byte[];
				SetMouseStep(SelectStep.Moving);
			}
			else
			{
				mBasePattern = null;
				SetMouseStep(SelectStep.Selecting);
			}
			cellSelectorPtn_MouseMove(sender, e);
		}
		else if (dbgPaintMode != SelectStep.Selecting && dbgPaintMode != SelectStep.Moving)
		{
			mBasePattern = null;
			SetMouseStep(SelectStep.None);
			mSelRect = new Rectangle(0, 0, 1, 1);
			DrawPtn();
			DrawChr();
		}
	}

	private void cellSelectorPtn_MouseMove(object sender, MouseEventArgs e)
	{
		if (MouseButtonDownForMove(mMouseButtonForMove))
		{
			int num = cellSelectorPtn.CellWidth * cellSelectorPtn.ZoomRate;
			int num2 = cellSelectorPtn.CellHeight * cellSelectorPtn.ZoomRate;
			int num3 = e.X / num;
			int num4 = e.Y / num2;
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num4 < 0)
			{
				num4 = 0;
			}
			mPtMove = new Point(num3, num4);
			if (dbgPaintMode == SelectStep.Selecting)
			{
				int num5 = Math.Min(mPtDown.X, mPtMove.X);
				int num6 = Math.Max(mPtDown.X, mPtMove.X);
				int num7 = Math.Min(mPtDown.Y, mPtMove.Y);
				int num8 = Math.Max(mPtDown.Y, mPtMove.Y);
				int num9 = num5 * cellSelectorPtn.CellWidth;
				int num10 = num7 * cellSelectorPtn.CellHeight;
				int num11 = (num6 - num5 + 1) * cellSelectorPtn.CellWidth;
				int num12 = (num8 - num7 + 1) * cellSelectorPtn.CellHeight;
				mSelRect = new Rectangle(num9, num10, num11, num12);
				cellSelectorPtn.SelectedRect = mSelRect;
			}
			DrawPtn();
			DrawChr();
		}
	}

	private void cellSelectorPtn_MouseUp(object sender, MouseEventArgs e)
	{
		bool flag = false;
		if (MouseButtonDownForMove(e.Button))
		{
			flag = true;
			mMouseButtonForMove = MouseButtons.None;
		}
		if (flag && mBasePattern != null)
		{
			int num = cellSelectorPtn.CellWidth * cellSelectorPtn.ZoomRate;
			int num2 = cellSelectorPtn.CellHeight * cellSelectorPtn.ZoomRate;
			int num3 = e.X / num;
			int num4 = e.Y / num2;
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num4 < 0)
			{
				num4 = 0;
			}
			mPtUp = new Point(num3, num4);
			if (mPtDown != mPtUp)
			{
				int num5 = mPtUp.X - mPtDown.X;
				int num6 = mPtUp.Y - mPtDown.Y;
				int num7 = mSelRect.Width / cellSelectorPtn.CellWidth;
				int num8 = mSelRect.Height / cellSelectorPtn.CellHeight;
				for (int i = 0; i < num8; i++)
				{
					for (int j = 0; j < num7; j++)
					{
						int num9 = j + mSelRect.X / cellSelectorPtn.CellWidth;
						int num10 = i + mSelRect.Y / cellSelectorPtn.CellHeight;
						int num11 = j + mSelRect.X / cellSelectorPtn.CellWidth + num5;
						int num12 = (i + mSelRect.Y / cellSelectorPtn.CellHeight + num6) * 16 + num11;
						int num13 = num10 * 16 + num9;
						if (0 <= num13 && num13 <= 255 && 0 <= num12 && num12 <= 255)
						{
							byte b = mPatternData[num12];
							mPatternData[num12] = mPatternData[num13];
							mPatternData[num13] = b;
						}
					}
				}
				SetMouseStep(SelectStep.Moved);
				Rectangle rectangle = mSelRect;
				int num14 = rectangle.Left + num5 * cellSelectorPtn.CellWidth;
				int num15 = rectangle.Top + num6 * cellSelectorPtn.CellHeight;
				mSelRect = new Rectangle(num14, num15, rectangle.Width, rectangle.Height);
			}
		}
		if (MouseButtonDownForMove(e.Button))
		{
			if (dbgPaintMode == SelectStep.Selecting)
			{
				SetMouseStep(SelectStep.Selected);
			}
			else if (dbgPaintMode == SelectStep.Moved)
			{
				SetMouseStep(SelectStep.Selected);
			}
			else
			{
				mBasePattern = null;
				mSelRect = new Rectangle(0, 0, 1, 1);
				cellSelectorPtn.SelectedRect = mSelRect;
				SetMouseStep(SelectStep.None);
			}
		}
		DrawPtn();
		DrawChr();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(YYCHR.Forms.PatternEditorForm));
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.listBoxPattern = new System.Windows.Forms.ListBox();
		this.buttonListDelete = new System.Windows.Forms.Button();
		this.buttonListMoveLower = new System.Windows.Forms.Button();
		this.buttonListMoveUpper = new System.Windows.Forms.Button();
		this.groupBoxCreateCharPattern = new System.Windows.Forms.GroupBox();
		this.labelCharDirection = new System.Windows.Forms.Label();
		this.comboBoxCreateDirection = new Controls.ComboBoxEx();
		this.labelCharHeight = new System.Windows.Forms.Label();
		this.numericUpDownCreateH = new System.Windows.Forms.NumericUpDown();
		this.labelCharWidth = new System.Windows.Forms.Label();
		this.numericUpDownCreateW = new System.Windows.Forms.NumericUpDown();
		this.buttonListAdd = new System.Windows.Forms.Button();
		this.buttonClose = new System.Windows.Forms.Button();
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.buttonSave = new System.Windows.Forms.Button();
		this.textBoxAdfName = new System.Windows.Forms.TextBox();
		this.labelAdfName = new System.Windows.Forms.Label();
		this.checkBoxHalfBright = new System.Windows.Forms.CheckBox();
		this.checkBoxShowNumber = new System.Windows.Forms.CheckBox();
		this.checkBoxNumberHalf = new System.Windows.Forms.CheckBox();
		this.checkBoxShowChr = new System.Windows.Forms.CheckBox();
		this.checkBoxHideFF = new System.Windows.Forms.CheckBox();
		this.cellSelectorChr = new ControlLib.CellSelector();
		this.cellSelectorPtn = new ControlLib.CellSelector();
		this.labelHintRightDragMove = new System.Windows.Forms.Label();
		this.groupBox1.SuspendLayout();
		this.groupBoxCreateCharPattern.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.numericUpDownCreateH).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.numericUpDownCreateW).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cellSelectorChr).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cellSelectorPtn).BeginInit();
		base.SuspendLayout();
		this.groupBox1.Controls.Add(this.listBoxPattern);
		this.groupBox1.Controls.Add(this.buttonListDelete);
		this.groupBox1.Controls.Add(this.buttonListMoveLower);
		this.groupBox1.Controls.Add(this.buttonListMoveUpper);
		this.groupBox1.Controls.Add(this.groupBoxCreateCharPattern);
		resources.ApplyResources(this.groupBox1, "groupBox1");
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.TabStop = false;
		this.listBoxPattern.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
		resources.ApplyResources(this.listBoxPattern, "listBoxPattern");
		this.listBoxPattern.Name = "listBoxPattern";
		this.listBoxPattern.DrawItem += new System.Windows.Forms.DrawItemEventHandler(listBoxPattern_DrawItem);
		this.listBoxPattern.SelectedIndexChanged += new System.EventHandler(actionListSelectedIndexChanged);
		resources.ApplyResources(this.buttonListDelete, "buttonListDelete");
		this.buttonListDelete.Name = "buttonListDelete";
		this.buttonListDelete.UseVisualStyleBackColor = true;
		this.buttonListDelete.Click += new System.EventHandler(actionListDelete);
		resources.ApplyResources(this.buttonListMoveLower, "buttonListMoveLower");
		this.buttonListMoveLower.Name = "buttonListMoveLower";
		this.buttonListMoveLower.UseVisualStyleBackColor = true;
		this.buttonListMoveLower.Click += new System.EventHandler(actionListMoveLower);
		resources.ApplyResources(this.buttonListMoveUpper, "buttonListMoveUpper");
		this.buttonListMoveUpper.Name = "buttonListMoveUpper";
		this.buttonListMoveUpper.UseVisualStyleBackColor = true;
		this.buttonListMoveUpper.Click += new System.EventHandler(actionListMoveUpper);
		this.groupBoxCreateCharPattern.Controls.Add(this.labelCharDirection);
		this.groupBoxCreateCharPattern.Controls.Add(this.comboBoxCreateDirection);
		this.groupBoxCreateCharPattern.Controls.Add(this.labelCharHeight);
		this.groupBoxCreateCharPattern.Controls.Add(this.numericUpDownCreateH);
		this.groupBoxCreateCharPattern.Controls.Add(this.labelCharWidth);
		this.groupBoxCreateCharPattern.Controls.Add(this.numericUpDownCreateW);
		this.groupBoxCreateCharPattern.Controls.Add(this.buttonListAdd);
		resources.ApplyResources(this.groupBoxCreateCharPattern, "groupBoxCreateCharPattern");
		this.groupBoxCreateCharPattern.Name = "groupBoxCreateCharPattern";
		this.groupBoxCreateCharPattern.TabStop = false;
		resources.ApplyResources(this.labelCharDirection, "labelCharDirection");
		this.labelCharDirection.Name = "labelCharDirection";
		this.comboBoxCreateDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxCreateDirection.FormattingEnabled = true;
		this.comboBoxCreateDirection.Items.AddRange(new object[2]
		{
			resources.GetString("comboBoxCreateDirection.Items"),
			resources.GetString("comboBoxCreateDirection.Items1")
		});
		resources.ApplyResources(this.comboBoxCreateDirection, "comboBoxCreateDirection");
		this.comboBoxCreateDirection.Name = "comboBoxCreateDirection";
		this.comboBoxCreateDirection.SelectedIndexChanged += new System.EventHandler(actionCreateNewPattern);
		resources.ApplyResources(this.labelCharHeight, "labelCharHeight");
		this.labelCharHeight.Name = "labelCharHeight";
		resources.ApplyResources(this.numericUpDownCreateH, "numericUpDownCreateH");
		this.numericUpDownCreateH.Maximum = new decimal(new int[4] { 16, 0, 0, 0 });
		this.numericUpDownCreateH.Minimum = new decimal(new int[4] { 1, 0, 0, 0 });
		this.numericUpDownCreateH.Name = "numericUpDownCreateH";
		this.numericUpDownCreateH.Value = new decimal(new int[4] { 2, 0, 0, 0 });
		this.numericUpDownCreateH.ValueChanged += new System.EventHandler(actionCreateNewPattern);
		resources.ApplyResources(this.labelCharWidth, "labelCharWidth");
		this.labelCharWidth.Name = "labelCharWidth";
		resources.ApplyResources(this.numericUpDownCreateW, "numericUpDownCreateW");
		this.numericUpDownCreateW.Maximum = new decimal(new int[4] { 16, 0, 0, 0 });
		this.numericUpDownCreateW.Minimum = new decimal(new int[4] { 1, 0, 0, 0 });
		this.numericUpDownCreateW.Name = "numericUpDownCreateW";
		this.numericUpDownCreateW.Value = new decimal(new int[4] { 2, 0, 0, 0 });
		this.numericUpDownCreateW.ValueChanged += new System.EventHandler(actionCreateNewPattern);
		resources.ApplyResources(this.buttonListAdd, "buttonListAdd");
		this.buttonListAdd.Name = "buttonListAdd";
		this.buttonListAdd.UseVisualStyleBackColor = true;
		this.buttonListAdd.Click += new System.EventHandler(actionSave);
		this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		resources.ApplyResources(this.buttonClose, "buttonClose");
		this.buttonClose.Name = "buttonClose";
		this.buttonClose.UseVisualStyleBackColor = true;
		this.buttonClose.Click += new System.EventHandler(actionClose);
		resources.ApplyResources(this.label1, "label1");
		this.label1.Name = "label1";
		resources.ApplyResources(this.label2, "label2");
		this.label2.Name = "label2";
		resources.ApplyResources(this.buttonSave, "buttonSave");
		this.buttonSave.Name = "buttonSave";
		this.buttonSave.UseVisualStyleBackColor = true;
		this.buttonSave.Click += new System.EventHandler(actionSave);
		resources.ApplyResources(this.textBoxAdfName, "textBoxAdfName");
		this.textBoxAdfName.Name = "textBoxAdfName";
		resources.ApplyResources(this.labelAdfName, "labelAdfName");
		this.labelAdfName.Name = "labelAdfName";
		resources.ApplyResources(this.checkBoxHalfBright, "checkBoxHalfBright");
		this.checkBoxHalfBright.Name = "checkBoxHalfBright";
		this.checkBoxHalfBright.UseVisualStyleBackColor = true;
		this.checkBoxHalfBright.CheckedChanged += new System.EventHandler(actionHalfBrightChanged);
		resources.ApplyResources(this.checkBoxShowNumber, "checkBoxShowNumber");
		this.checkBoxShowNumber.Name = "checkBoxShowNumber";
		this.checkBoxShowNumber.UseVisualStyleBackColor = true;
		this.checkBoxShowNumber.CheckedChanged += new System.EventHandler(actionShowNumberChanged);
		resources.ApplyResources(this.checkBoxNumberHalf, "checkBoxNumberHalf");
		this.checkBoxNumberHalf.Name = "checkBoxNumberHalf";
		this.checkBoxNumberHalf.UseVisualStyleBackColor = true;
		this.checkBoxNumberHalf.CheckedChanged += new System.EventHandler(actionNumberHalfChecked);
		resources.ApplyResources(this.checkBoxShowChr, "checkBoxShowChr");
		this.checkBoxShowChr.Name = "checkBoxShowChr";
		this.checkBoxShowChr.UseVisualStyleBackColor = true;
		this.checkBoxShowChr.CheckedChanged += new System.EventHandler(checkBoxShowChr_CheckedChanged);
		resources.ApplyResources(this.checkBoxHideFF, "checkBoxHideFF");
		this.checkBoxHideFF.Name = "checkBoxHideFF";
		this.checkBoxHideFF.UseVisualStyleBackColor = true;
		this.cellSelectorChr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.cellSelectorChr.DefaultSelectSize = new System.Drawing.Size(8, 8);
		this.cellSelectorChr.EnableRightDragSelect = true;
		this.cellSelectorChr.FreeSelect = false;
		this.cellSelectorChr.GridColor1 = System.Drawing.Color.White;
		this.cellSelectorChr.GridColor2 = System.Drawing.Color.Gray;
		this.cellSelectorChr.GridStyle = ControlLib.GridStyle.Dot;
		this.cellSelectorChr.Image = null;
		resources.ApplyResources(this.cellSelectorChr, "cellSelectorChr");
		this.cellSelectorChr.MouseDownNew = true;
		this.cellSelectorChr.Name = "cellSelectorChr";
		this.cellSelectorChr.PixelSelect = false;
		this.cellSelectorChr.SelectedColor1 = System.Drawing.Color.White;
		this.cellSelectorChr.SelectedColor2 = System.Drawing.Color.Aqua;
		this.cellSelectorChr.SelectedIndex = 0;
		this.cellSelectorChr.SelectedRect = new System.Drawing.Rectangle(0, 0, 8, 8);
		this.cellSelectorChr.TabStop = false;
		this.cellSelectorChr.ZoomRate = 2;
		this.cellSelectorChr.Selected += new System.Windows.Forms.MouseEventHandler(actionChrSelected);
		this.cellSelectorChr.Paint += new System.Windows.Forms.PaintEventHandler(cellSelectorChr_Paint);
		this.cellSelectorPtn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.cellSelectorPtn.DefaultSelectSize = new System.Drawing.Size(8, 8);
		this.cellSelectorPtn.EnableMiddleClickSelect = true;
		this.cellSelectorPtn.EnableRightDragSelect = true;
		this.cellSelectorPtn.FreeSelect = false;
		this.cellSelectorPtn.GridColor1 = System.Drawing.Color.White;
		this.cellSelectorPtn.GridColor2 = System.Drawing.Color.Gray;
		this.cellSelectorPtn.GridStyle = ControlLib.GridStyle.Dot;
		this.cellSelectorPtn.Image = null;
		resources.ApplyResources(this.cellSelectorPtn, "cellSelectorPtn");
		this.cellSelectorPtn.MouseDownNew = true;
		this.cellSelectorPtn.Name = "cellSelectorPtn";
		this.cellSelectorPtn.PixelSelect = false;
		this.cellSelectorPtn.SelectedColor1 = System.Drawing.Color.White;
		this.cellSelectorPtn.SelectedColor2 = System.Drawing.Color.Aqua;
		this.cellSelectorPtn.SelectedIndex = 0;
		this.cellSelectorPtn.SelectedRect = new System.Drawing.Rectangle(0, 0, 8, 8);
		this.cellSelectorPtn.TabStop = false;
		this.cellSelectorPtn.ZoomRate = 2;
		this.cellSelectorPtn.Selected += new System.Windows.Forms.MouseEventHandler(actionPatternSelected);
		this.cellSelectorPtn.Paint += new System.Windows.Forms.PaintEventHandler(cellSelectorPtn_Paint);
		this.cellSelectorPtn.MouseDown += new System.Windows.Forms.MouseEventHandler(cellSelectorPtn_MouseDown);
		this.cellSelectorPtn.MouseMove += new System.Windows.Forms.MouseEventHandler(cellSelectorPtn_MouseMove);
		this.cellSelectorPtn.MouseUp += new System.Windows.Forms.MouseEventHandler(cellSelectorPtn_MouseUp);
		resources.ApplyResources(this.labelHintRightDragMove, "labelHintRightDragMove");
		this.labelHintRightDragMove.Name = "labelHintRightDragMove";
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
		base.CancelButton = this.buttonClose;
		resources.ApplyResources(this, "$this");
		base.Controls.Add(this.labelHintRightDragMove);
		base.Controls.Add(this.checkBoxHideFF);
		base.Controls.Add(this.checkBoxShowChr);
		base.Controls.Add(this.checkBoxNumberHalf);
		base.Controls.Add(this.checkBoxShowNumber);
		base.Controls.Add(this.checkBoxHalfBright);
		base.Controls.Add(this.labelAdfName);
		base.Controls.Add(this.textBoxAdfName);
		base.Controls.Add(this.buttonSave);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.cellSelectorChr);
		base.Controls.Add(this.cellSelectorPtn);
		base.Controls.Add(this.buttonClose);
		base.Controls.Add(this.groupBox1);
		this.DoubleBuffered = true;
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "PatternEditorForm";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.groupBox1.ResumeLayout(false);
		this.groupBoxCreateCharPattern.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.numericUpDownCreateH).EndInit();
		((System.ComponentModel.ISupportInitialize)this.numericUpDownCreateW).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cellSelectorChr).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cellSelectorPtn).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
