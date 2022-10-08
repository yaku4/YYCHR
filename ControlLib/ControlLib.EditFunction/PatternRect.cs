using System;
using System.Drawing;
using System.Windows.Forms;
using CharactorLib.Common;
using ControlLib.Properties;

namespace ControlLib.EditFunction;

public class PatternRect : EditFunctionBase
{
	public PatternRect()
	{
		base.Name = ResourceUtility.GetResourceString(EditFunctionBase.ResourceType, "Resources.PenNamePatternRect");
		base.Icon = Resources.PenIconPatternRect;
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
			DrawPatternRect(editingBytemap, mMouseDownPoint, mousePoint);
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
			DrawPatternRect(bytemap, mMouseDownPoint, mouseUpPoint);
			base.EditedFlag = true;
		}
		base.MouseUp(bytemap, mouseUpPoint, mouseButtons);
	}

	private void DrawPatternRect(Bytemap bytemap, Point mMouseDownPoint, Point mousePoint)
	{
		Point mouseDownPoint = base.MouseDownPoint;
		int num = Math.Min(mMouseDownPoint.X, mousePoint.X);
		int num2 = Math.Min(mMouseDownPoint.Y, mousePoint.Y);
		int num3 = Math.Max(mMouseDownPoint.X, mousePoint.X);
		int num4 = Math.Max(mMouseDownPoint.Y, mousePoint.Y);
		for (int i = num2; i <= num4; i++)
		{
			for (int j = num; j <= num3; j++)
			{
				if (((j - mouseDownPoint.X + (i - mouseDownPoint.Y)) & 1) == 0)
				{
					bytemap.SetPixel(j, i, mSelectedPalette, clip: true);
				}
			}
		}
	}
}
