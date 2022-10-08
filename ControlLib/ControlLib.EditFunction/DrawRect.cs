using System;
using System.Drawing;
using System.Windows.Forms;
using CharactorLib.Common;
using ControlLib.Properties;

namespace ControlLib.EditFunction;

public class DrawRect : EditFunctionBase
{
	public DrawRect()
	{
		base.Name = ResourceUtility.GetResourceString(EditFunctionBase.ResourceType, "Resources.PenNameDrawRect");
		base.Icon = Resources.PenIconDrawRect;
		base.RectType = true;
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
			int l = Math.Min(mMouseDownPoint.X, mMouseMovePoint.X);
			int t = Math.Min(mMouseDownPoint.Y, mMouseMovePoint.Y);
			int num = Math.Abs(mMouseDownPoint.X - mMouseMovePoint.X);
			int num2 = Math.Abs(mMouseDownPoint.Y - mMouseMovePoint.Y);
			editingBytemap.DrawRect(l, t, num + 1, num2 + 1, mSelectedPalette);
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
			int l = Math.Min(mMouseDownPoint.X, mouseUpPoint.X);
			int t = Math.Min(mMouseDownPoint.Y, mouseUpPoint.Y);
			int num = Math.Abs(mMouseDownPoint.X - mouseUpPoint.X);
			int num2 = Math.Abs(mMouseDownPoint.Y - mouseUpPoint.Y);
			bytemap.DrawRect(l, t, num + 1, num2 + 1, mSelectedPalette);
			base.EditedFlag = true;
		}
		base.MouseUp(bytemap, mouseUpPoint, mouseButtons);
	}
}
