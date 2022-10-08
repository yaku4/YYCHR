using System.Windows.Forms;

namespace ControlLib;

public class PicturePanel : Panel
{
	public PicturePanel()
	{
		SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
	}
}
