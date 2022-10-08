using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PrgEditor.Forms;

public class AddressInputForm : Form
{
	private IContainer components;

	private Button button1;

	private TextBox textBox1;

	public int Address { get; set; }

	public AddressInputForm()
	{
		InitializeComponent();
		Address = 0;
	}

	private void AddressInputForm_Shown(object sender, EventArgs e)
	{
		textBox1.Text = Address.ToString("X8");
		textBox1.SelectAll();
	}

	private void button1_Click(object sender, EventArgs e)
	{
		string value = textBox1.Text;
		int address = 0;
		try
		{
			address = Convert.ToInt32(value, 16);
		}
		catch
		{
		}
		Address = address;
		base.DialogResult = DialogResult.OK;
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
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(PrgEditor.Forms.AddressInputForm));
		this.button1 = new System.Windows.Forms.Button();
		this.textBox1 = new System.Windows.Forms.TextBox();
		base.SuspendLayout();
		componentResourceManager.ApplyResources(this.button1, "button1");
		this.button1.Name = "button1";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(button1_Click);
		componentResourceManager.ApplyResources(this.textBox1, "textBox1");
		this.textBox1.HideSelection = false;
		this.textBox1.Name = "textBox1";
		base.AcceptButton = this.button1;
		componentResourceManager.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.textBox1);
		base.Controls.Add(this.button1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "AddressInputForm";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.Shown += new System.EventHandler(AddressInputForm_Shown);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
