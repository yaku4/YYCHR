using System;
using System.Windows.Forms;

namespace ControlLib;

public class OneClickToolStrip : ToolStrip
{
	private const int WM_MOUSEACTIVATE = 33;

	private const int MA_ACTIVATE = 1;

	protected override void WndProc(ref Message m)
	{
		if (m.Msg == 33)
		{
			m.Result = new IntPtr(1);
		}
		else
		{
			base.WndProc(ref m);
		}
	}
}
