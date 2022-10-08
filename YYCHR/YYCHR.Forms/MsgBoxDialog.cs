using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CharactorLib.Common;

namespace YYCHR.Forms;

public class MsgBoxDialog : Form
{
	private const int BTN_MARGIN = 8;

	private IContainer components;

	private TextBox textBoxMessage;

	private PictureBox pictureBoxIcon;

	private Button buttonOK;

	private Button buttonCancel;

	private Button buttonYes;

	private Button buttonNo;

	public MsgBoxDialog()
	{
		InitializeComponent();
		ResourceUtility.UpdateTextIfLngEnabled(this, null);
	}

	public DialogResult ShowDialog(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
	{
		Text = caption;
		SetupText(text);
		SetupButton(buttons);
		SetupIcon(icon);
		int num = textBoxMessage.Top + textBoxMessage.Height / 2 - pictureBoxIcon.Height / 2;
		pictureBoxIcon.Location = new Point(pictureBoxIcon.Left, num);
		if (base.AcceptButton != null)
		{
			base.ActiveControl = (Button)base.AcceptButton;
		}
		return ShowDialog(owner);
	}

	private void SetupText(string text)
	{
		string text2 = text;
		text2 = text2.Replace("\\r", "\r");
		text2 = text2.Replace("\\n", "\n");
		text2 = text2.Replace("\r\n", "\n");
		text2 = text2.Replace("\n", "\r\n");
		textBoxMessage.Text = text2;
		Size proposedSize = new Size(1, 1);
		Size size = TextRenderer.MeasureText(text2, Font, proposedSize, TextFormatFlags.TextBoxControl);
		Size size2 = TextRenderer.MeasureText("\r\n\r\n", Font, proposedSize, TextFormatFlags.TextBoxControl);
		int num = size.Width;
		int num2 = size.Height;
		int num3 = Screen.PrimaryScreen.Bounds.Width * 3 / 4;
		int num4 = Screen.PrimaryScreen.Bounds.Height * 3 / 4;
		int num5 = 200;
		int num6 = size2.Height;
		if (num >= num3)
		{
			num = num3;
		}
		if (num2 >= num4)
		{
			num2 = num4;
		}
		if (num < num5)
		{
			num = num5;
		}
		if (num2 < num6)
		{
			num2 = num6;
		}
		num += 16;
		num2 += 8;
		if (text.Contains("\t"))
		{
			num += 48;
		}
		int num7 = base.Width - textBoxMessage.Width + num;
		int num8 = base.Height - textBoxMessage.Height + num2;
		base.Size = new Size(num7, num8);
		textBoxMessage.SelectionStart = textBoxMessage.Text.Length;
		textBoxMessage.SelectionLength = 0;
	}

	private void SetupIcon(MessageBoxIcon icon)
	{
		Bitmap image = new Bitmap(32, 32);
		Graphics graphics = Graphics.FromImage(image);
		switch (icon)
		{
		case MessageBoxIcon.Asterisk:
			graphics.DrawIcon(SystemIcons.Asterisk, 0, 0);
			break;
		case MessageBoxIcon.Hand:
			graphics.DrawIcon(SystemIcons.Hand, 0, 0);
			break;
		case MessageBoxIcon.Question:
			graphics.DrawIcon(SystemIcons.Question, 0, 0);
			break;
		case MessageBoxIcon.Exclamation:
			graphics.DrawIcon(SystemIcons.Warning, 0, 0);
			break;
		}
		graphics.Dispose();
		pictureBoxIcon.Image = image;
	}

	private void SetupButton(MessageBoxButtons buttons)
	{
		buttonOK.Visible = false;
		buttonCancel.Visible = false;
		buttonYes.Visible = false;
		buttonNo.Visible = false;
		List<Button> list = new List<Button>();
		switch (buttons)
		{
		case MessageBoxButtons.OKCancel:
			list.Add(buttonOK);
			list.Add(buttonCancel);
			base.AcceptButton = buttonOK;
			base.CancelButton = buttonCancel;
			break;
		case MessageBoxButtons.YesNo:
			list.Add(buttonYes);
			list.Add(buttonNo);
			base.AcceptButton = null;
			base.CancelButton = null;
			break;
		case MessageBoxButtons.YesNoCancel:
			list.Add(buttonYes);
			list.Add(buttonNo);
			list.Add(buttonCancel);
			base.AcceptButton = null;
			base.CancelButton = buttonCancel;
			break;
		default:
			list.Add(buttonOK);
			base.AcceptButton = buttonOK;
			base.CancelButton = buttonOK;
			break;
		}
		int num = 0;
		foreach (Button item in list)
		{
			if (num > 0)
			{
				num += 8;
			}
			num += item.Width;
		}
		int num2 = base.Width / 2 - num / 2;
		foreach (Button item2 in list)
		{
			item2.Left = num2;
			if (num2 > 0)
			{
				num2 += 8;
			}
			num2 += item2.Width;
		}
		foreach (Button item3 in list)
		{
			item3.Visible = true;
		}
	}

	private void buttonOK_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
	}

	private void buttonCancel_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
	}

	private void buttonYes_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Yes;
	}

	private void buttonNo_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.No;
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
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(YYCHR.Forms.MsgBoxDialog));
		this.textBoxMessage = new System.Windows.Forms.TextBox();
		this.buttonOK = new System.Windows.Forms.Button();
		this.buttonCancel = new System.Windows.Forms.Button();
		this.buttonYes = new System.Windows.Forms.Button();
		this.buttonNo = new System.Windows.Forms.Button();
		this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
		((System.ComponentModel.ISupportInitialize)this.pictureBoxIcon).BeginInit();
		base.SuspendLayout();
		componentResourceManager.ApplyResources(this.textBoxMessage, "textBoxMessage");
		this.textBoxMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.textBoxMessage.Name = "textBoxMessage";
		this.textBoxMessage.ReadOnly = true;
		this.textBoxMessage.TabStop = false;
		componentResourceManager.ApplyResources(this.buttonOK, "buttonOK");
		this.buttonOK.Name = "buttonOK";
		this.buttonOK.UseVisualStyleBackColor = true;
		this.buttonOK.Click += new System.EventHandler(buttonOK_Click);
		componentResourceManager.ApplyResources(this.buttonCancel, "buttonCancel");
		this.buttonCancel.Name = "buttonCancel";
		this.buttonCancel.UseVisualStyleBackColor = true;
		this.buttonCancel.Click += new System.EventHandler(buttonCancel_Click);
		componentResourceManager.ApplyResources(this.buttonYes, "buttonYes");
		this.buttonYes.Name = "buttonYes";
		this.buttonYes.UseVisualStyleBackColor = true;
		this.buttonYes.Click += new System.EventHandler(buttonYes_Click);
		componentResourceManager.ApplyResources(this.buttonNo, "buttonNo");
		this.buttonNo.Name = "buttonNo";
		this.buttonNo.UseVisualStyleBackColor = true;
		this.buttonNo.Click += new System.EventHandler(buttonNo_Click);
		componentResourceManager.ApplyResources(this.pictureBoxIcon, "pictureBoxIcon");
		this.pictureBoxIcon.Name = "pictureBoxIcon";
		this.pictureBoxIcon.TabStop = false;
		componentResourceManager.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.buttonNo);
		base.Controls.Add(this.buttonYes);
		base.Controls.Add(this.buttonCancel);
		base.Controls.Add(this.buttonOK);
		base.Controls.Add(this.pictureBoxIcon);
		base.Controls.Add(this.textBoxMessage);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "MsgBoxDialog";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
		((System.ComponentModel.ISupportInitialize)this.pictureBoxIcon).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
