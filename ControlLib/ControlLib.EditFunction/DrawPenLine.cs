using System.Drawing;
using System.Windows.Forms;
using CharactorLib.Common;
using ControlLib.Properties;

namespace ControlLib.EditFunction;

public class DrawPenLine : EditFunctionBase
{
	private bool oldEnable;

	private Point oldPoint = new Point(-1, -1);

	public DrawPenLine()
	{
		base.Name = ResourceUtility.GetResourceString(EditFunctionBase.ResourceType, "Resources.PenNamePen");
		base.Icon = Resources.PenIconPen;
		base.CopyOriginal = false;
		base.RectType = false;
	}

	public override void MouseDown(Bytemap bytemap, Point mousePoint, MouseButtons mouseButtons)
	{
		if (bytemap != null)
		{
			base.MouseDown(bytemap, mousePoint, mouseButtons);
			ResetPoint();
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
			DrawLine(editingBytemap, mousePoint);
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
			DrawLine(editingBytemap, mouseUpPoint);
			bytemap.CopyRect(base.EditingRect, editingBytemap, base.EditingRect);
			base.EditedFlag = true;
		}
		base.MouseUp(bytemap, mouseUpPoint, mouseButtons);
		ResetPoint();
	}

	private void ResetPoint()
	{
		oldEnable = false;
	}

	private void DrawLine(Bytemap bytemap, Point mousePoint)
	{
		if (bytemap != null)
		{
			if (oldEnable)
			{
				bytemap.DrawLine(oldPoint, mousePoint, mSelectedPalette);
			}
			else
			{
				bytemap.SetPixel(mousePoint, mSelectedPalette);
				oldEnable = true;
			}
			oldPoint = mousePoint;
		}
	}
}
