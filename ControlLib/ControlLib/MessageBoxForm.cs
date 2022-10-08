using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ControlLib;

public class MessageBoxForm : Form
{
	public MessageBoxIcon IconType;

	public MessageBoxButtons Buttons;

	private const int IMAGE_W = 32;

	private const int IMAGE_H = 32;

	private IContainer components;

	private TextBoxEx textBoxMessage;

	private Button buttonOK;

	private Button buttonCancel;

	private PictureBox pictureBoxIcon;

	public ScrollBars TextBoxScrollBars
	{
		get
		{
			return textBoxMessage.ScrollBars;
		}
		set
		{
			textBoxMessage.ScrollBars = value;
		}
	}

	public static DialogResult Show(string text, string title)
	{
		return Show(text, title, MessageBoxIcon.None, MessageBoxButtons.OK, new Size(600, 300));
	}

	public static DialogResult Show(string text, string title, MessageBoxIcon icon, MessageBoxButtons buttons)
	{
		return Show(text, title, icon, buttons, new Size(600, 300));
	}

	public static DialogResult Show(string text, string title, MessageBoxIcon icon, MessageBoxButtons buttons, Size size)
	{
		return new MessageBoxForm(text, title, icon, buttons)
		{
			TextBoxScrollBars = ScrollBars.Vertical,
			Size = size
		}.ShowDialog(GetOwner());
	}

	public static DialogResult ShowSizable(string text, string title, MessageBoxIcon icon, MessageBoxButtons buttons)
	{
		return ShowSizable(text, title, icon, buttons, new Size(600, 300));
	}

	public static DialogResult ShowSizable(string text, string title, MessageBoxIcon icon, MessageBoxButtons buttons, Size size)
	{
		return new MessageBoxForm(text, title, icon, buttons)
		{
			FormBorderStyle = FormBorderStyle.Sizable,
			TextBoxScrollBars = ScrollBars.Vertical,
			Size = size
		}.ShowDialog(GetOwner());
	}

	private static IWin32Window GetOwner()
	{
		if (Application.OpenForms.Count > 0)
		{
			return Application.OpenForms[0];
		}
		return null;
	}

	public MessageBoxForm()
	{
		InitializeComponent();
	}

	public MessageBoxForm(string text, string title, MessageBoxIcon icon, MessageBoxButtons buttons)
	{
		InitializeComponent();
		textBoxMessage.Text = text;
		Text = title;
		IconType = icon;
		Buttons = buttons;
		if (IconType == MessageBoxIcon.None)
		{
			pictureBoxIcon.Visible = false;
			textBoxMessage.Left -= pictureBoxIcon.Width / 2;
		}
		else
		{
			pictureBoxIcon.Visible = true;
			textBoxMessage.Left = pictureBoxIcon.Right + 10;
		}
		MessageBoxButtons buttons2 = Buttons;
		if (buttons2 != 0 && buttons2 == MessageBoxButtons.OKCancel)
		{
			base.CancelButton = buttonCancel;
			buttonCancel.Visible = true;
		}
		else
		{
			base.CancelButton = buttonOK;
			buttonCancel.Visible = false;
		}
		CalcButtonPos();
	}

	protected override void OnShown(EventArgs e)
	{
		base.OnShown(e);
		textBoxMessage.Select(0, 0);
		base.ActiveControl = buttonOK;
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

	private void pictureBox1_Paint(object sender, PaintEventArgs e)
	{
		int num = e.ClipRectangle.Width / 2 - 16;
		int num2 = e.ClipRectangle.Height / 2 - 16;
		Graphics graphics = e.Graphics;
		DrawIcon(graphics, num, num2);
	}

	private void DrawIcon(Graphics g, int x, int y)
	{
		Icon icon = IconType switch
		{
			MessageBoxIcon.Exclamation => SystemIcons.Warning, 
			MessageBoxIcon.Hand => SystemIcons.Error, 
			MessageBoxIcon.Asterisk => SystemIcons.Information, 
			MessageBoxIcon.Question => SystemIcons.Question, 
			MessageBoxIcon.None => null, 
			_ => SystemIcons.Application, 
		};
		if (icon != null)
		{
			g.DrawIcon(icon, x, y);
		}
	}

	private void MessageBoxForm_Resize(object sender, EventArgs e)
	{
		pictureBoxIcon.Invalidate();
		CalcButtonPos();
	}

	private void CalcButtonPos()
	{
		int num = base.Width / 2;
		if (buttonCancel.Visible)
		{
			buttonOK.Left = num - 20 - buttonOK.Width / 2;
			buttonCancel.Left = num + 20;
		}
		else
		{
			buttonOK.Left = num - buttonOK.Width / 2;
		}
	}

	private void MessageBoxForm_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Control && e.KeyCode == Keys.A)
		{
			textBoxMessage.SelectAll();
			textBoxMessage.Focus();
		}
		if (e.Control && e.KeyCode == Keys.C)
		{
			base.ActiveControl = textBoxMessage;
			if (textBoxMessage.SelectionLength == 0)
			{
				textBoxMessage.SelectAll();
				textBoxMessage.Focus();
			}
			textBoxMessage.Copy();
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
		this.buttonOK = new System.Windows.Forms.Button();
		this.buttonCancel = new System.Windows.Forms.Button();
		this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
		this.textBoxMessage = new ControlLib.TextBoxEx();
		((System.ComponentModel.ISupportInitialize)this.pictureBoxIcon).BeginInit();
		base.SuspendLayout();
		this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.buttonOK.Location = new System.Drawing.Point(197, 119);
		this.buttonOK.Name = "buttonOK";
		this.buttonOK.Size = new System.Drawing.Size(100, 32);
		this.buttonOK.TabIndex = 1;
		this.buttonOK.Text = "OK";
		this.buttonOK.UseVisualStyleBackColor = true;
		this.buttonOK.Click += new System.EventHandler(buttonOK_Click);
		this.buttonOK.KeyDown += new System.Windows.Forms.KeyEventHandler(MessageBoxForm_KeyDown);
		this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.buttonCancel.Location = new System.Drawing.Point(320, 119);
		this.buttonCancel.Name = "buttonCancel";
		this.buttonCancel.Size = new System.Drawing.Size(100, 32);
		this.buttonCancel.TabIndex = 3;
		this.buttonCancel.Text = "Cancel";
		this.buttonCancel.UseVisualStyleBackColor = true;
		this.buttonCancel.Click += new System.EventHandler(buttonCancel_Click);
		this.buttonCancel.KeyDown += new System.Windows.Forms.KeyEventHandler(MessageBoxForm_KeyDown);
		this.pictureBoxIcon.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.pictureBoxIcon.Location = new System.Drawing.Point(12, 12);
		this.pictureBoxIcon.Name = "pictureBoxIcon";
		this.pictureBoxIcon.Size = new System.Drawing.Size(40, 97);
		this.pictureBoxIcon.TabIndex = 4;
		this.pictureBoxIcon.TabStop = false;
		this.pictureBoxIcon.Paint += new System.Windows.Forms.PaintEventHandler(pictureBox1_Paint);
		this.textBoxMessage.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.textBoxMessage.BackColor = System.Drawing.SystemColors.Control;
		this.textBoxMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.textBoxMessage.Font = new System.Drawing.Font("Consolas", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.textBoxMessage.Location = new System.Drawing.Point(58, 12);
		this.textBoxMessage.Multiline = true;
		this.textBoxMessage.Name = "textBoxMessage";
		this.textBoxMessage.ReadOnly = true;
		this.textBoxMessage.Size = new System.Drawing.Size(434, 97);
		this.textBoxMessage.TabIndex = 0;
		this.textBoxMessage.Text = "Message";
		this.textBoxMessage.WordWrap = false;
		base.AcceptButton = this.buttonOK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.buttonCancel;
		base.ClientSize = new System.Drawing.Size(504, 159);
		base.Controls.Add(this.pictureBoxIcon);
		base.Controls.Add(this.buttonCancel);
		base.Controls.Add(this.buttonOK);
		base.Controls.Add(this.textBoxMessage);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "MessageBoxForm";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Message Box";
		base.KeyDown += new System.Windows.Forms.KeyEventHandler(MessageBoxForm_KeyDown);
		base.Resize += new System.EventHandler(MessageBoxForm_Resize);
		((System.ComponentModel.ISupportInitialize)this.pictureBoxIcon).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
