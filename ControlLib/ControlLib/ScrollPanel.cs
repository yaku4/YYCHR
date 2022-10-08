using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ControlLib;

public class ScrollPanel : Panel
{
	public enum ScrollBarTypes
	{
		Vertical = 0,
		Horizontal = 1,
		None = 3
	}

	private ScrollBar mScrollBar;

	private ScrollBarTypes mScrollBarType;

	private int mLrChange = 1;

	private int mWheelRate = 4;

	private IContainer components;

	private VScrollBar vScrollBar1;

	private HScrollBar hScrollBar1;

	public ScrollBarTypes ScrollBarType
	{
		get
		{
			return mScrollBarType;
		}
		set
		{
			mScrollBarType = value;
			vScrollBar1.Visible = mScrollBarType == ScrollBarTypes.Vertical;
			hScrollBar1.Visible = mScrollBarType == ScrollBarTypes.Horizontal;
			ScrollBar scrollBar = mScrollBar;
			if (vScrollBar1.Visible)
			{
				mScrollBar = vScrollBar1;
			}
			if (hScrollBar1.Visible)
			{
				mScrollBar = hScrollBar1;
			}
			mScrollBar.Minimum = scrollBar.Minimum;
			mScrollBar.Maximum = scrollBar.Maximum;
			mScrollBar.Value = scrollBar.Value;
			mScrollBar.SmallChange = scrollBar.SmallChange;
			mScrollBar.LargeChange = scrollBar.LargeChange;
		}
	}

	public Size ClientAreaSize
	{
		get
		{
			int num = base.Width - base.Margin.Horizontal + 2;
			int num2 = base.Height - base.Margin.Vertical + 2;
			if (mScrollBar == vScrollBar1)
			{
				num -= mScrollBar.Width;
			}
			if (mScrollBar == hScrollBar1)
			{
				num2 -= mScrollBar.Height;
			}
			return new Size(num, num2);
		}
		set
		{
			int num = value.Width + base.Margin.Horizontal - 2;
			int num2 = value.Height + base.Margin.Vertical - 2;
			if (mScrollBar == vScrollBar1)
			{
				num += mScrollBar.Width;
			}
			if (mScrollBar == hScrollBar1)
			{
				num2 += mScrollBar.Height;
			}
			base.Size = new Size(num, num2);
		}
	}

	public int Minimum
	{
		get
		{
			return vScrollBar1.Minimum;
		}
		set
		{
			if (Value < value)
			{
				Value = value;
			}
			mScrollBar.Minimum = value;
			CheckScrollBarEnable();
		}
	}

	public int Maximum
	{
		get
		{
			return mScrollBar.Maximum;
		}
		set
		{
			if (value < Minimum)
			{
				value = Minimum;
			}
			if (Value > value + 1 - LargeChange)
			{
				Value = value + 1 - LargeChange;
			}
			mScrollBar.Maximum = value;
			CheckScrollBarEnable();
		}
	}

	public int SmallChange
	{
		get
		{
			return mScrollBar.SmallChange;
		}
		set
		{
			if (value < 1)
			{
				value = 1;
			}
			mScrollBar.SmallChange = value;
		}
	}

	public int LargeChange
	{
		get
		{
			return mScrollBar.LargeChange;
		}
		set
		{
			if (value < 1)
			{
				value = 1;
			}
			mScrollBar.LargeChange = value;
			CheckScrollBarEnable();
		}
	}

	public int LrChange
	{
		get
		{
			return mLrChange;
		}
		set
		{
			if (value < 1)
			{
				value = 1;
			}
			mLrChange = value;
			CheckScrollBarEnable();
		}
	}

	public int Value
	{
		get
		{
			return mScrollBar.Value;
		}
		set
		{
			if (value < Minimum)
			{
				value = Minimum;
			}
			if (value > Maximum + 1 - LargeChange)
			{
				value = Maximum + 1 - LargeChange;
			}
			mScrollBar.Value = value;
		}
	}

	public int WheelRate
	{
		get
		{
			return mWheelRate;
		}
		set
		{
			mWheelRate = value;
		}
	}

	[Browsable(true)]
	public event EventHandler Scrolled;

	[Browsable(true)]
	public new event KeyEventHandler KeyDown;

	[Browsable(true)]
	public event MouseEventHandler WheelSizeChange;

	public ScrollPanel()
	{
		InitializeComponent();
		mScrollBar = vScrollBar1;
		SetStyle(ControlStyles.Selectable, value: true);
		SetStyle(ControlStyles.ContainerControl, value: true);
	}

	private void CheckScrollBarEnable()
	{
		if (Maximum - Minimum > LargeChange)
		{
			mScrollBar.Enabled = true;
		}
		else
		{
			mScrollBar.Enabled = false;
		}
	}

	protected override void OnControlAdded(ControlEventArgs ev)
	{
		ev.Control.MouseDown += this_Click;
		base.OnControlAdded(ev);
	}

	private void this_Click(object sender, MouseEventArgs ev)
	{
		if (!Focused && base.CanSelect)
		{
			Focus();
		}
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		if (!Focused && base.CanSelect)
		{
			Focus();
		}
		base.OnMouseClick(e);
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

	protected override void OnMouseWheel(MouseEventArgs ev)
	{
		if (KeyState.Control)
		{
			if (this.WheelSizeChange != null)
			{
				this.WheelSizeChange(this, ev);
			}
			return;
		}
		int num = mScrollBar.Value;
		int num2 = SmallChange * WheelRate;
		if (ev.Delta > 0)
		{
			num -= num2;
		}
		if (ev.Delta < 0)
		{
			num += num2;
		}
		if (num < Minimum)
		{
			num = Minimum;
		}
		if (num > Maximum + 1 - LargeChange)
		{
			num = Maximum + 1 - LargeChange;
		}
		mScrollBar.Value = num;
	}

	private void vScrollBar1_Scroll(object sender, ScrollEventArgs ev)
	{
		if (!Focused && base.CanSelect)
		{
			Focus();
		}
		if (ev.Type == ScrollEventType.ThumbTrack || ev.Type == ScrollEventType.ThumbPosition)
		{
			int num = ev.OldValue % SmallChange;
			int num2 = ev.NewValue / SmallChange * SmallChange + num;
			if (num2 < Minimum)
			{
				num2 = Minimum;
			}
			if (num2 > Maximum + 1 - LargeChange)
			{
				num2 = Maximum + 1 - LargeChange;
			}
			ev.NewValue = num2;
		}
	}

	private void vScrollBar1_ValueChanged(object sender, EventArgs ev)
	{
		if (this.Scrolled != null)
		{
			this.Scrolled(this, EventArgs.Empty);
		}
	}

	protected override bool IsInputKey(Keys keyData)
	{
		bool flag;
		switch (keyData)
		{
		case Keys.Prior:
		case Keys.Next:
		case Keys.End:
		case Keys.Home:
		case Keys.Left:
		case Keys.Up:
		case Keys.Right:
		case Keys.Down:
		case Keys.Oemplus:
		case Keys.OemMinus:
		case Keys.Left | Keys.Shift:
		case Keys.Up | Keys.Shift:
		case Keys.Right | Keys.Shift:
		case Keys.Down | Keys.Shift:
			flag = true;
			break;
		default:
			flag = false;
			break;
		}
		if (flag)
		{
			return true;
		}
		return base.IsInputKey(keyData);
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (this.KeyDown != null)
		{
			this.KeyDown(this, e);
			return;
		}
		switch (e.KeyData)
		{
		case Keys.Home:
			Value = Minimum;
			break;
		case Keys.Prior:
			Value -= LargeChange;
			break;
		case Keys.Up:
			Value -= SmallChange;
			break;
		case Keys.Left:
			Value -= LrChange;
			break;
		case Keys.OemMinus:
			Value--;
			break;
		case Keys.Oemplus:
			Value++;
			break;
		case Keys.Right:
			Value += LrChange;
			break;
		case Keys.Down:
			Value += SmallChange;
			break;
		case Keys.Next:
			Value += LargeChange;
			break;
		case Keys.End:
			Value = Maximum + 1 - LargeChange;
			break;
		default:
			base.OnKeyDown(e);
			break;
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
		this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
		this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
		base.SuspendLayout();
		this.vScrollBar1.Dock = System.Windows.Forms.DockStyle.Right;
		this.vScrollBar1.Location = new System.Drawing.Point(357, 0);
		this.vScrollBar1.Name = "vScrollBar1";
		this.vScrollBar1.Size = new System.Drawing.Size(16, 248);
		this.vScrollBar1.TabIndex = 0;
		this.vScrollBar1.ValueChanged += new System.EventHandler(vScrollBar1_ValueChanged);
		this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(vScrollBar1_Scroll);
		this.hScrollBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.hScrollBar1.Location = new System.Drawing.Point(0, 357);
		this.hScrollBar1.Name = "hScrollBar1";
		this.hScrollBar1.Size = new System.Drawing.Size(248, 16);
		this.hScrollBar1.TabIndex = 1;
		this.hScrollBar1.ValueChanged += new System.EventHandler(vScrollBar1_ValueChanged);
		this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(vScrollBar1_Scroll);
		this.hScrollBar1.Visible = false;
		base.Controls.Add(this.vScrollBar1);
		base.Controls.Add(this.hScrollBar1);
		base.Name = "ScrollPanel";
		base.Size = new System.Drawing.Size(373, 248);
		base.ResumeLayout(false);
	}
}
