using System;
using System.ComponentModel;
using System.Windows.Forms;
using CharactorLib.Common;
using YYCHR.Properties;

namespace YYCHR.Forms;

public class InputTextForm : Form
{
	private static Type ResourceType = typeof(Resources);

	private string mTextName = "Text";

	private string mTextValue = "";

	private bool mFirstShow = true;

	private IContainer components;

	private Button buttonOk;

	private Button buttonCancel;

	private Label labelDescription;

	private TextBox textBox;

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

	public string TextValue
	{
		get
		{
			return mTextValue;
		}
		set
		{
			mTextValue = value;
		}
	}

	public InputTextForm()
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
			textBox.Text = TextValue;
		}
		base.OnShown(e);
	}

	private void buttonOk_Click(object sender, EventArgs e)
	{
		string textValue = textBox.Text;
		if (CheckInvalidChar(textValue, "=") && CheckInvalidChar(textValue, ","))
		{
			TextValue = textValue;
			base.DialogResult = DialogResult.OK;
		}
	}

	private bool CheckInvalidChar(string text, string invalidText)
	{
		if (text.Contains(invalidText))
		{
			string resourceString = ResourceUtility.GetResourceString(ResourceType, "Resources.TitleWarning");
			string text2 = ResourceUtility.GetResourceString(ResourceType, "Resources.MessageInvalidTextUsed") + " '" + invalidText + "'";
			MsgBox.Show(this, text2, resourceString, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return false;
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
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(YYCHR.Forms.InputTextForm));
		this.buttonOk = new System.Windows.Forms.Button();
		this.buttonCancel = new System.Windows.Forms.Button();
		this.labelDescription = new System.Windows.Forms.Label();
		this.textBox = new System.Windows.Forms.TextBox();
		base.SuspendLayout();
		componentResourceManager.ApplyResources(this.buttonOk, "buttonOk");
		this.buttonOk.Name = "buttonOk";
		this.buttonOk.UseVisualStyleBackColor = true;
		this.buttonOk.Click += new System.EventHandler(buttonOk_Click);
		componentResourceManager.ApplyResources(this.buttonCancel, "buttonCancel");
		this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.buttonCancel.Name = "buttonCancel";
		this.buttonCancel.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this.labelDescription, "labelDescription");
		this.labelDescription.Name = "labelDescription";
		componentResourceManager.ApplyResources(this.textBox, "textBox");
		this.textBox.Name = "textBox";
		base.AcceptButton = this.buttonOk;
		componentResourceManager.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.buttonCancel;
		base.Controls.Add(this.textBox);
		base.Controls.Add(this.labelDescription);
		base.Controls.Add(this.buttonCancel);
		base.Controls.Add(this.buttonOk);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "InputTextForm";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
