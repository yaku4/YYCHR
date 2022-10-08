using System.Windows.Forms;

namespace ControlLib;

public class ButtonNoFocus : Button
{
	public ButtonNoFocus()
	{
		SetStyle(ControlStyles.Selectable, value: false);
	}
}
