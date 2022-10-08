using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ControlLib;

public class BytemapSelectorBmp : UserControl
{
	private IContainer components;

	private ScrollPanelHV scrollPanelHV1;

	private CellSelector cellSelector1;

	public BytemapSelectorBmp()
	{
		InitializeComponent();
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
		this.scrollPanelHV1 = new ControlLib.ScrollPanelHV();
		this.cellSelector1 = new ControlLib.CellSelector();
		this.scrollPanelHV1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cellSelector1).BeginInit();
		base.SuspendLayout();
		this.scrollPanelHV1.ClientAreaSize = new System.Drawing.Size(512, 512);
		this.scrollPanelHV1.Controls.Add(this.cellSelector1);
		this.scrollPanelHV1.LargeChange = 32;
		this.scrollPanelHV1.Location = new System.Drawing.Point(0, 0);
		this.scrollPanelHV1.Margin = new System.Windows.Forms.Padding(0);
		this.scrollPanelHV1.Name = "scrollPanelHV1";
		this.scrollPanelHV1.Size = new System.Drawing.Size(528, 528);
		this.scrollPanelHV1.SmallChange = 8;
		this.scrollPanelHV1.TabIndex = 0;
		this.scrollPanelHV1.WheelRate = 4;
		this.cellSelector1.DefaultSelectSize = new System.Drawing.Size(32, 32);
		this.cellSelector1.EnableRightDragSelect = true;
		this.cellSelector1.FreeSelect = false;
		this.cellSelector1.GridColor1 = System.Drawing.Color.FromArgb(128, 255, 255, 255);
		this.cellSelector1.GridColor2 = System.Drawing.Color.FromArgb(64, 255, 255, 255);
		this.cellSelector1.GridStyle = ControlLib.GridStyle.Dot;
		this.cellSelector1.Image = null;
		this.cellSelector1.Location = new System.Drawing.Point(0, 0);
		this.cellSelector1.Margin = new System.Windows.Forms.Padding(0);
		this.cellSelector1.Name = "cellSelector1";
		this.cellSelector1.PixelSelect = false;
		this.cellSelector1.SelectedColor1 = System.Drawing.Color.White;
		this.cellSelector1.SelectedColor2 = System.Drawing.Color.Aqua;
		this.cellSelector1.SelectedIndex = 0;
		this.cellSelector1.SelectedRect = new System.Drawing.Rectangle(0, 0, 32, 32);
		this.cellSelector1.Size = new System.Drawing.Size(512, 512);
		this.cellSelector1.TabIndex = 2;
		this.cellSelector1.TabStop = false;
		this.cellSelector1.ZoomRate = 1;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.scrollPanelHV1);
		base.Margin = new System.Windows.Forms.Padding(0);
		base.Name = "BytemapSelectorBmp";
		base.Size = new System.Drawing.Size(534, 534);
		this.scrollPanelHV1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cellSelector1).EndInit();
		base.ResumeLayout(false);
	}
}
