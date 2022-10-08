using System.Drawing;
using System.Windows.Forms;
using CharactorLib.Common;
using ControlLib.Properties;

namespace ControlLib.EditFunction;

public class PatternPen : EditFunctionBase
{
	public PatternPen()
	{
		base.Name = ResourceUtility.GetResourceString(EditFunctionBase.ResourceType, "Resources.PenNamePatternPen");
		base.Icon = Resources.PenIconPatternPen;
		base.CopyOriginal = false;
		base.RectType = false;
	}

	public override void MouseDown(Bytemap bytemap, Point mousePoint, MouseButtons mouseButtons)
	{
		if (bytemap != null)
		{
			base.MouseDown(bytemap, mousePoint, mouseButtons);
			mMouseDownPoint = mousePoint;
			DrawPixel(editingBytemap, mousePoint);
		}
	}

	public override void MouseMove(Bytemap bytemap, Point mousePoint, MouseButtons mouseButtons)
	{
		if (bytemap == null)
		{
			return;
		}
		base.MouseMove(bytemap, mousePoint, mouseButtons);
		if ((mMouseButtons & MouseButtons.Left) == MouseButtons.Left)
		{
			if (editingBytemap == null)
			{
				return;
			}
			DrawPixel(editingBytemap, mousePoint);
		}
		if (mMouseButtons == MouseButtons.Right)
		{
			mSelectedPalette = bytemap.GetPixel(mousePoint);
			base.ColorPicked = true;
		}
	}

	public override void MouseUp(Bytemap bytemap, Point mouseUpPoint, MouseButtons mouseButtons)
	{
		if (bytemap == null)
		{
			return;
		}
		if ((mMouseButtons & MouseButtons.Left) == MouseButtons.Left && mouseButtons == MouseButtons.Left)
		{
			if (editingBytemap == null)
			{
				return;
			}
			DrawPixel(editingBytemap, mouseUpPoint);
			bytemap.CopyRect(base.EditingRect, editingBytemap, base.EditingRect);
			base.EditedFlag = true;
		}
		base.MouseUp(bytemap, mouseUpPoint, mouseButtons);
	}

	private void DrawPixel(Bytemap bytemap, Point mousePoint)
	{
		if (bytemap != null)
		{
			Point mouseDownPoint = base.MouseDownPoint;
			if (((mousePoint.X - mouseDownPoint.X + (mousePoint.Y - mouseDownPoint.Y)) & 1) == 0)
			{
				bytemap.SetPixel(mousePoint, mSelectedPalette);
			}
		}
	}
}
