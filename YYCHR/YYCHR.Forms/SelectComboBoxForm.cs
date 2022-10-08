using System;
using System.ComponentModel;
using System.Windows.Forms;
using CharactorLib.Common;
using Controls;

namespace YYCHR.Forms;

public class SelectComboBoxForm : Form
{
	private string mTextName = "";

	private bool mFirstShow = true;

	private IContainer components;

	private Button buttonOK;

	private Button buttonCancel;

	private ComboBoxEx comboBoxItem;

	private Label labelDescription;

	public string TextName
	{
		get
		{
			return mTextName;
		}
		set
		{
			mTextName = value;
		}
	}

	public ComboBox ComboBox => comboBoxItem;

	public SelectComboBoxForm()
	{
		InitializeComponent();
		ResourceUtility.UpdateTextIfLngEnabled(this, null);
	}

	protected override void OnShown(EventArgs e)
	{
		if (mFirstShow)
		{
			mFirstShow = false;
			Text = Text.Replace("@TEXT_NAME", TextName);
			labelDescription.Text = labelDescription.Text.Replace("@TEXT_NAME", TextName);
			if (comboBoxItem.Items != null && comboBoxItem.Items.Count > 0)
			{
				comboBoxItem.SelectedIndex = 0;
			}
		}
		base.OnShown(e);
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
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(YYCHR.Forms.SelectComboBoxForm));
		this.buttonOK = new System.Windows.Forms.Button();
		this.buttonCancel = new System.Windows.Forms.Button();
		this.comboBoxItem = new Controls.ComboBoxEx();
		this.labelDescription = new System.Windows.Forms.Label();
		base.SuspendLayout();
		componentResourceManager.ApplyResources(this.buttonOK, "buttonOK");
		this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.buttonOK.Name = "buttonOK";
		this.buttonOK.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this.buttonCancel, "buttonCancel");
		this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.buttonCancel.Name = "buttonCancel";
		this.buttonCancel.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this.comboBoxItem, "comboBoxItem");
		this.comboBoxItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxItem.FormattingEnabled = true;
		this.comboBoxItem.Name = "comboBoxItem";
		componentResourceManager.ApplyResources(this.labelDescription, "labelDescription");
		this.labelDescription.Name = "labelDescription";
		base.AcceptButton = this.buttonOK;
		componentResourceManager.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.buttonCancel;
		base.Controls.Add(this.labelDescription);
		base.Controls.Add(this.comboBoxItem);
		base.Controls.Add(this.buttonCancel);
		base.Controls.Add(this.buttonOK);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "SelectComboBoxForm";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
		base.ResumeLayout(false);
	}
}
