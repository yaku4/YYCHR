using System.Drawing;
using System.Windows.Forms;
using CharactorLib.Common;
using ControlLib.Properties;

namespace ControlLib.EditFunction;

public class DrawLine : EditFunctionBase
{
	public DrawLine()
	{
		base.Name = ResourceUtility.GetResourceString(EditFunctionBase.ResourceType, "Resources.PenNameLine");
		base.Icon = Resources.PenIconLine;
		base.RectType = false;
	}

	public override void MouseDown(Bytemap bytemap, Point mousePoint, MouseButtons mouseButtons)
	{
		base.MouseDown(bytemap, mousePoint, mouseButtons);
	}

	public override void MouseMove(Bytemap bytemap, Point mousePoint, MouseButtons mouseButtons)
	{
		base.MouseMove(bytemap, mousePoint, mouseButtons);
		if ((mMouseButtons & MouseButtons.Left) == MouseButtons.Left)
		{
			editingBytemap.DrawLine(mMouseDownPoint, mMouseMovePoint, mSelectedPalette);
		}
		if (mMouseButtons == MouseButtons.Right)
		{
			mSelectedPalette = bytemap.GetPixel(mousePoint);
			base.ColorPicked = true;
		}
	}

	public override void MouseUp(Bytemap bytemap, Point mouseUpPoint, MouseButtons mouseButtons)
	{
		if ((mMouseButtons & MouseButtons.Left) == MouseButtons.Left && mouseButtons == MouseButtons.Left)
		{
			bytemap.DrawLine(mMouseDownPoint, mouseUpPoint, mSelectedPalette);
			base.EditedFlag = true;
		}
		base.MouseUp(bytemap, mouseUpPoint, mouseButtons);
	}
}
