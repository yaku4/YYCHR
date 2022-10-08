using System;
using System.ComponentModel;
using System.Windows.Forms;
using CharactorLib.Common;

namespace PaletteEditor;

public class SettingForm : Form
{
	private Settings mSettings;

	private Settings mSettingsForEdit;

	private IContainer components;

	private Button buttonCancel;

	private Button buttonOK;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem miReset;

	private PropertyGrid propertyGridSetting;

	public SettingForm()
	{
		InitializeComponent();
		ResourceUtility.UpdateTextIfLngEnabled(this, null);
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!base.DesignMode)
		{
			mSettings = Settings.GetInstance();
			mSettingsForEdit = mSettings.Clone() as Settings;
			propertyGridSetting.SelectedObject = mSettingsForEdit;
		}
	}

	private void buttonOK_Click(object sender, EventArgs e)
	{
		mSettings.CopyFrom(mSettingsForEdit);
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void buttonCancel_Click(object sender, EventArgs e)
	{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PaletteEditor.SettingForm));
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.miReset = new System.Windows.Forms.ToolStripMenuItem();
		this.buttonCancel = new System.Windows.Forms.Button();
		this.buttonOK = new System.Windows.Forms.Button();
		this.propertyGridSetting = new System.Windows.Forms.PropertyGrid();
		this.contextMenuStrip1.SuspendLayout();
		base.SuspendLayout();
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.miReset });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
		this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(contextMenuStrip1_Opening);
		resources.ApplyResources(this.miReset, "miReset");
		this.miReset.Name = "miReset";
		this.miReset.Click += new System.EventHandler(miReset_Click);
		resources.ApplyResources(this.buttonCancel, "buttonCancel");
		this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.buttonCancel.Name = "buttonCancel";
		this.buttonCancel.UseVisualStyleBackColor = true;
		this.buttonCancel.Click += new System.EventHandler(buttonCancel_Click);
		resources.ApplyResources(this.buttonOK, "buttonOK");
		this.buttonOK.Name = "buttonOK";
		this.buttonOK.UseVisualStyleBackColor = true;
		this.buttonOK.Click += new System.EventHandler(buttonOK_Click);
		resources.ApplyResources(this.propertyGridSetting, "propertyGridSetting");
		this.propertyGridSetting.ContextMenuStrip = this.contextMenuStrip1;
		this.propertyGridSetting.Name = "propertyGridSetting";
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.buttonCancel;
		base.Controls.Add(this.propertyGridSetting);
		base.Controls.Add(this.buttonOK);
		base.Controls.Add(this.buttonCancel);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "SettingForm";
		base.ShowInTaskbar = false;
		this.contextMenuStrip1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
