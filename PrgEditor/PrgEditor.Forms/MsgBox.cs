using System.Windows.Forms;

namespace PrgEditor.Forms;

public static class MsgBox
{
	private static MsgBoxDialog dialog = new MsgBoxDialog();

	public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
	{
		if (dialog.Visible)
		{
			return DialogResult.Cancel;
		}
		return dialog.ShowDialog(owner, text, caption, buttons, icon);
	}

	public static DialogResult Show(IWin32Window owner, string text, string caption)
	{
		return Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
	}
}
