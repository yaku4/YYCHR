using System.Windows.Forms;

namespace YYCHR.Forms;

public static class MsgBox
{
	private static MsgBoxDialog instance = new MsgBoxDialog();

	public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
	{
		return instance.ShowDialog(owner, text, caption, buttons, icon);
	}

	public static DialogResult Show(IWin32Window owner, string text, string caption)
	{
		return Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
	}
}
