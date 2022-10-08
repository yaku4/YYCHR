using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CharactorLib.Common;
using CharactorLib.Format;

namespace YYCHR.Forms;

public class SettingForm : Form
{
	private FormatManager mFormatManager;

	private Settings mSettings;

	private Settings mSettingsOrg;

	private IContainer components;

	private TabControl tabControl1;

	private TabPage tabPageSetting;

	private Button buttonCancel;

	private Button buttonOK;

	private TabPage tabPageFormat;

	private ListBox listBoxFormat;

	private PropertyGrid propertyGridSetting;

	private Button buttonFormatInfo;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem miReset;

	public SettingForm()
	{
		InitializeComponent();
		ResourceUtility.UpdateTextIfLngEnabled(this, null);
		mFormatManager = FormatManager.GetInstance();
		mSettings = Settings.GetInstance();
		mSettingsOrg = mSettings.Clone() as Settings;
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		propertyGridSetting.SelectedObject = mSettings;
		listBoxFormat.Items.Clear();
		FormatBase[] formats = mFormatManager.GetFormats();
		foreach (FormatBase item in formats)
		{
			listBoxFormat.Items.Add(item);
		}
	}

	private void buttonOK_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void buttonCancel_Click(object sender, EventArgs e)
	{
		mSettings.CopyFrom(mSettingsOrg);
		base.DialogResult = DialogResult.Cancel;
		Close();
	}

	private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
	{
		bool enabled = false;
		object selectedObject = propertyGridSetting.SelectedObject;
		GridItem selectedGridItem = propertyGridSetting.SelectedGridItem;
		if (selectedGridItem != null && selectedGridItem.GridItemType == GridItemType.Property && selectedGridItem.PropertyDescriptor != null)
		{
			enabled = selectedGridItem.PropertyDescriptor.CanResetValue(selectedObject);
		}
		miReset.Enabled = enabled;
	}

	private void miReset_Click(object sender, EventArgs e)
	{
		try
		{
			propertyGridSetting.ResetSelectedProperty();
		}
		catch
		{
		}
	}

	private void buttonFormatInfo_Click(object sender, EventArgs e)
	{
		ShowFormatInfo();
	}

	private void listBoxFormat_DoubleClick(object sender, EventArgs e)
	{
		ShowFormatInfo();
	}

	private void ShowFormatInfo()
	{
		try
		{
			FormatBase formatBase = (FormatBase)listBoxFormat.SelectedItem;
			if (formatBase != null)
			{
				MsgBox.Show(this, formatBase.GetFormatInfo(), formatBase.Name, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
		}
		catch
		{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(YYCHR.Forms.SettingForm));
		this.tabControl1 = new System.Windows.Forms.TabControl();
		this.tabPageSetting = new System.Windows.Forms.TabPage();
		this.propertyGridSetting = new System.Windows.Forms.PropertyGrid();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.miReset = new System.Windows.Forms.ToolStripMenuItem();
		this.tabPageFormat = new System.Windows.Forms.TabPage();
		this.buttonFormatInfo = new System.Windows.Forms.Button();
		this.listBoxFormat = new System.Windows.Forms.ListBox();
		this.buttonCancel = new System.Windows.Forms.Button();
		this.buttonOK = new System.Windows.Forms.Button();
		this.tabControl1.SuspendLayout();
		this.tabPageSetting.SuspendLayout();
		this.contextMenuStrip1.SuspendLayout();
		this.tabPageFormat.SuspendLayout();
		base.SuspendLayout();
		resources.ApplyResources(this.tabControl1, "tabControl1");
		this.tabControl1.Controls.Add(this.tabPageSetting);
		this.tabControl1.Controls.Add(this.tabPageFormat);
		this.tabControl1.Name = "tabControl1";
		this.tabControl1.SelectedIndex = 0;
		this.tabPageSetting.Controls.Add(this.propertyGridSetting);
		resources.ApplyResources(this.tabPageSetting, "tabPageSetting");
		this.tabPageSetting.Name = "tabPageSetting";
		this.propertyGridSetting.ContextMenuStrip = this.contextMenuStrip1;
		resources.ApplyResources(this.propertyGridSetting, "propertyGridSetting");
		this.propertyGridSetting.Name = "propertyGridSetting";
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.miReset });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
		this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(contextMenuStrip1_Opening);
		resources.ApplyResources(this.miReset, "miReset");
		this.miReset.Name = "miReset";
		this.miReset.Click += new System.EventHandler(miReset_Click);
		this.tabPageFormat.BackColor = System.Drawing.SystemColors.Control;
		this.tabPageFormat.Controls.Add(this.buttonFormatInfo);
		this.tabPageFormat.Controls.Add(this.listBoxFormat);
		resources.ApplyResources(this.tabPageFormat, "tabPageFormat");
		this.tabPageFormat.Name = "tabPageFormat";
		resources.ApplyResources(this.buttonFormatInfo, "buttonFormatInfo");
		this.buttonFormatInfo.Name = "buttonFormatInfo";
		this.buttonFormatInfo.UseVisualStyleBackColor = true;
		this.buttonFormatInfo.Click += new System.EventHandler(buttonFormatInfo_Click);
		resources.ApplyResources(this.listBoxFormat, "listBoxFormat");
		this.listBoxFormat.FormattingEnabled = true;
		this.listBoxFormat.Name = "listBoxFormat";
		this.listBoxFormat.DoubleClick += new System.EventHandler(listBoxFormat_DoubleClick);
		resources.ApplyResources(this.buttonCancel, "buttonCancel");
		this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.buttonCancel.Name = "buttonCancel";
		this.buttonCancel.UseVisualStyleBackColor = true;
		this.buttonCancel.Click += new System.EventHandler(buttonCancel_Click);
		resources.ApplyResources(this.buttonOK, "buttonOK");
		this.buttonOK.Name = "buttonOK";
		this.buttonOK.UseVisualStyleBackColor = true;
		this.buttonOK.Click += new System.EventHandler(buttonOK_Click);
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.buttonCancel;
		base.Controls.Add(this.buttonOK);
		base.Controls.Add(this.buttonCancel);
		base.Controls.Add(this.tabControl1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "SettingForm";
		base.ShowInTaskbar = false;
		this.tabControl1.ResumeLayout(false);
		this.tabPageSetting.ResumeLayout(false);
		this.contextMenuStrip1.ResumeLayout(false);
		this.tabPageFormat.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
