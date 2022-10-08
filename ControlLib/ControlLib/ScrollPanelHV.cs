using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ControlLib;

public class ScrollPanelHV : Panel
{
	private IContainer components;

	private VScrollBar vScrollBar1;

	private HScrollBar hScrollBar1;

	public int WheelRate { get; set; } = 4;


	public int SmallChange { get; set; } = 8;


	public int LargeChange { get; set; } = 32;


	public new ScrollBar HScroll => hScrollBar1;

	public new ScrollBar VScroll => vScrollBar1;

	public Size ClientAreaSize
	{
		get
		{
			int num = base.Width - base.Margin.Horizontal;
			int num2 = base.Height - base.Margin.Vertical;
			if (vScrollBar1.Visible)
			{
				num -= vScrollBar1.Width;
			}
			if (hScrollBar1.Visible)
			{
				num2 -= hScrollBar1.Height;
			}
			return new Size(num, num2);
		}
		set
		{
			int num = value.Width + base.Margin.Horizontal;
			int num2 = value.Height + base.Margin.Vertical;
			if (vScrollBar1.Visible)
			{
				num += vScrollBar1.Width;
			}
			if (hScrollBar1.Visible)
			{
				num2 += hScrollBar1.Height;
			}
			base.Size = new Size(num, num2);
		}
	}

	public event EventHandler Scrolled;

	[Browsable(true)]
	public event MouseEventHandler WheelSizeChange;

	[Browsable(true)]
	public new event KeyEventHandler KeyDown;

	private void Call_Scrolled()
	{
		if (this.Scrolled != null)
		{
			this.Scrolled(this, EventArgs.Empty);
		}
	}

	public ScrollPanelHV()
	{
		InitializeComponent();
		SetStyle(ControlStyles.Selectable, value: true);
		SetStyle(ControlStyles.ContainerControl, value: true);
	}

	protected override void OnCreateControl()
	{
		base.OnCreateControl();
		ScrollBar_Init();
	}

	protected override void OnResize(EventArgs eventargs)
	{
		base.OnResize(eventargs);
		ScrollBar_SetBounds();
		ScrollBar_CheckEnable();
	}

	protected override void OnControlAdded(ControlEventArgs ev)
	{
		ev.Control.MouseDown += child_MouseDown;
		base.OnControlAdded(ev);
	}

	private void child_MouseDown(object sender, MouseEventArgs ev)
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

	private void ScrollBar_Init()
	{
		ScrollBar_SetBounds();
		ScrollBar_SetParam(vScrollBar1, visible: true, 0, 255, 0);
		ScrollBar_SetParam(hScrollBar1, visible: true, 0, 255, 0);
	}

	private void ScrollBar_SetBounds()
	{
		Size clientAreaSize = ClientAreaSize;
		vScrollBar1.Height = base.Height - base.Margin.Vertical - hScrollBar1.Height;
		vScrollBar1.Left = clientAreaSize.Width;
		hScrollBar1.Width = base.Width - base.Margin.Horizontal - vScrollBar1.Width;
		hScrollBar1.Top = clientAreaSize.Height;
		ScrollBar_UpdateLargeChange();
	}

	private void ScrollBar_UpdateLargeChange()
	{
		int largeChange = vScrollBar1.Height / 2;
		vScrollBar1.LargeChange = largeChange;
		int largeChange2 = hScrollBar1.Width / 2;
		hScrollBar1.LargeChange = largeChange2;
	}

	private void ScrollBar_SetParam(ScrollBar scrollBar, bool visible, int min, int max, int value)
	{
		scrollBar.Visible = visible;
		ScrollBar_SetChange(scrollBar);
		ScrollBar_SetMinimum(scrollBar, min);
		ScrollBar_SetMaximum(scrollBar, max);
		ScrollBar_SetValue(scrollBar, value);
		ScrollBar_CheckEnable(scrollBar);
	}

	private void ScrollBar_SetChange(ScrollBar scrollBar)
	{
		int num = SmallChange;
		if (num < 1)
		{
			num = 1;
		}
		if (scrollBar.SmallChange != num)
		{
			scrollBar.SmallChange = num;
		}
		ScrollBar_CheckEnable(scrollBar);
	}

	public void ScrollBar_SetMinimum(ScrollBar scrollBar, int newMin)
	{
		if (newMin > scrollBar.Maximum)
		{
			newMin = scrollBar.Maximum;
		}
		if (scrollBar.Value < newMin)
		{
			scrollBar.Value = newMin;
		}
		if (scrollBar.Minimum != newMin)
		{
			scrollBar.Minimum = newMin;
		}
		ScrollBar_CheckEnable(scrollBar);
	}

	public void ScrollBar_SetMaximum(ScrollBar scrollBar, int newMax)
	{
		if (newMax < scrollBar.Minimum)
		{
			newMax = scrollBar.Minimum;
		}
		if (scrollBar.Value > newMax + 1 - LargeChange)
		{
			scrollBar.Value = newMax + 1 - LargeChange;
		}
		if (scrollBar.Maximum != newMax)
		{
			scrollBar.Maximum = newMax;
		}
		ScrollBar_CheckEnable(scrollBar);
	}

	private bool ScrollBar_SetValue(ScrollBar scrollBar, int value)
	{
		bool result = false;
		if (value < scrollBar.Minimum)
		{
			value = scrollBar.Minimum;
		}
		if (value > scrollBar.Maximum + 1 - scrollBar.LargeChange)
		{
			value = scrollBar.Maximum + 1 - scrollBar.LargeChange;
		}
		if (scrollBar.Value != value)
		{
			scrollBar.Value = value;
			result = true;
		}
		return result;
	}

	private bool ScrollBar_Scroll(VScrollBar scrollBar, int change)
	{
		int value = scrollBar.Value + change;
		return ScrollBar_SetValue(scrollBar, value);
	}

	private void ScrollBar_CheckEnable()
	{
		ScrollBar_CheckEnable(vScrollBar1);
		ScrollBar_CheckEnable(hScrollBar1);
	}

	private void ScrollBar_CheckEnable(ScrollBar mScrollBar)
	{
		if (mScrollBar.Maximum - mScrollBar.Minimum > mScrollBar.LargeChange)
		{
			mScrollBar.Enabled = true;
		}
		else
		{
			mScrollBar.Enabled = false;
		}
	}

	private void scrollBar_Scroll(object sender, ScrollEventArgs ev)
	{
		if (!Focused && base.CanSelect)
		{
			Focus();
		}
		if (sender is ScrollBar scrollBar && (ev.Type == ScrollEventType.ThumbTrack || ev.Type == ScrollEventType.ThumbPosition))
		{
			int num = ev.OldValue % scrollBar.SmallChange;
			int num2 = ev.NewValue / scrollBar.SmallChange * scrollBar.SmallChange + num;
			if (num2 < scrollBar.Minimum)
			{
				num2 = scrollBar.Minimum;
			}
			if (num2 > scrollBar.Maximum + 1 - scrollBar.LargeChange)
			{
				num2 = scrollBar.Maximum + 1 - scrollBar.LargeChange;
			}
			if (ev.NewValue != num2)
			{
				ev.NewValue = num2;
			}
		}
	}

	private void vScrollBar1_ValueChanged(object sender, EventArgs ev)
	{
		Call_Scrolled();
	}

	protected override void OnMouseWheel(MouseEventArgs e)
	{
		if (!ActionWheel(e))
		{
			base.OnMouseWheel(e);
		}
	}

	private bool ActionWheel(MouseEventArgs ev)
	{
		if (KeyState.Control)
		{
			if (this.WheelSizeChange != null)
			{
				this.WheelSizeChange(this, ev);
			}
		}
		else
		{
			int num = SmallChange * WheelRate;
			int change = 0;
			if (ev.Delta > 0)
			{
				change = -num;
			}
			if (ev.Delta < 0)
			{
				change = num;
			}
			ScrollBar_Scroll(vScrollBar1, change);
		}
		return true;
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
		this.vScrollBar1.Dock = System.Windows.Forms.DockStyle.None;
		this.vScrollBar1.Location = new System.Drawing.Point(357, 0);
		this.vScrollBar1.Name = "vScrollBar1";
		this.vScrollBar1.Size = new System.Drawing.Size(16, 248);
		this.vScrollBar1.TabIndex = 0;
		this.vScrollBar1.ValueChanged += new System.EventHandler(vScrollBar1_ValueChanged);
		this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(scrollBar_Scroll);
		this.hScrollBar1.Dock = System.Windows.Forms.DockStyle.None;
		this.hScrollBar1.Location = new System.Drawing.Point(0, 357);
		this.hScrollBar1.Name = "hScrollBar1";
		this.hScrollBar1.Size = new System.Drawing.Size(248, 16);
		this.hScrollBar1.TabIndex = 1;
		this.hScrollBar1.ValueChanged += new System.EventHandler(vScrollBar1_ValueChanged);
		this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(scrollBar_Scroll);
		base.Controls.Add(this.vScrollBar1);
		base.Controls.Add(this.hScrollBar1);
		base.Name = "ScrollPanel";
		base.Size = new System.Drawing.Size(373, 248);
		base.ResumeLayout(false);
	}
}
