using System;
using System.ComponentModel;
using System.Windows.Forms;
using CharactorLib.Format;

namespace PrgEditor.Forms;

public class SettingForm : Form
{
	private Setting mSetting = new Setting();

	private FormatManager mFormatManager;

	private IContainer components;

	private TabControl tabControl1;

	private TabPage tabPageSetting;

	private Button buttonCancel;

	private Button buttonOK;

	private TabPage tabPageFormat;

	private ListBox listBoxFormat;

	private PropertyGrid propertyGridSetting;

	private Button buttonFormatInfo;

	public SettingForm()
	{
		InitializeComponent();
		mFormatManager = FormatManager.GetInstance();
		tabControl1.Controls.Remove(tabPageFormat);
	}

	private void buttonOK_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void buttonCancel_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
	}

	public void LoadSetting(Setting setting)
	{
		mSetting.CopyFromSetting(setting);
		propertyGridSetting.SelectedObject = mSetting;
		listBoxFormat.Items.Clear();
		FormatBase[] formats = mFormatManager.GetFormats();
		foreach (FormatBase item in formats)
		{
			listBoxFormat.Items.Add(item);
		}
	}

	public void SaveSetting(Setting setting)
	{
		setting.CopyFromSetting(mSetting);
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
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(PrgEditor.Forms.SettingForm));
		this.tabControl1 = new System.Windows.Forms.TabControl();
		this.tabPageSetting = new System.Windows.Forms.TabPage();
		this.propertyGridSetting = new System.Windows.Forms.PropertyGrid();
		this.tabPageFormat = new System.Windows.Forms.TabPage();
		this.buttonFormatInfo = new System.Windows.Forms.Button();
		this.listBoxFormat = new System.Windows.Forms.ListBox();
		this.buttonCancel = new System.Windows.Forms.Button();
		this.buttonOK = new System.Windows.Forms.Button();
		this.tabControl1.SuspendLayout();
		this.tabPageSetting.SuspendLayout();
		this.tabPageFormat.SuspendLayout();
		base.SuspendLayout();
		componentResourceManager.ApplyResources(this.tabControl1, "tabControl1");
		this.tabControl1.Controls.Add(this.tabPageSetting);
		this.tabControl1.Controls.Add(this.tabPageFormat);
		this.tabControl1.Name = "tabControl1";
		this.tabControl1.SelectedIndex = 0;
		this.tabPageSetting.Controls.Add(this.propertyGridSetting);
		componentResourceManager.ApplyResources(this.tabPageSetting, "tabPageSetting");
		this.tabPageSetting.Name = "tabPageSetting";
		this.tabPageSetting.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this.propertyGridSetting, "propertyGridSetting");
		this.propertyGridSetting.Name = "propertyGridSetting";
		this.tabPageFormat.Controls.Add(this.buttonFormatInfo);
		this.tabPageFormat.Controls.Add(this.listBoxFormat);
		componentResourceManager.ApplyResources(this.tabPageFormat, "tabPageFormat");
		this.tabPageFormat.Name = "tabPageFormat";
		this.tabPageFormat.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this.buttonFormatInfo, "buttonFormatInfo");
		this.buttonFormatInfo.Name = "buttonFormatInfo";
		this.buttonFormatInfo.UseVisualStyleBackColor = true;
		this.buttonFormatInfo.Click += new System.EventHandler(buttonFormatInfo_Click);
		componentResourceManager.ApplyResources(this.listBoxFormat, "listBoxFormat");
		this.listBoxFormat.FormattingEnabled = true;
		this.listBoxFormat.Name = "listBoxFormat";
		this.listBoxFormat.DoubleClick += new System.EventHandler(listBoxFormat_DoubleClick);
		componentResourceManager.ApplyResources(this.buttonCancel, "buttonCancel");
		this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.buttonCancel.Name = "buttonCancel";
		this.buttonCancel.UseVisualStyleBackColor = true;
		this.buttonCancel.Click += new System.EventHandler(buttonCancel_Click);
		componentResourceManager.ApplyResources(this.buttonOK, "buttonOK");
		this.buttonOK.Name = "buttonOK";
		this.buttonOK.UseVisualStyleBackColor = true;
		this.buttonOK.Click += new System.EventHandler(buttonOK_Click);
		base.AcceptButton = this.buttonOK;
		componentResourceManager.ApplyResources(this, "$this");
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
		this.tabPageFormat.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
