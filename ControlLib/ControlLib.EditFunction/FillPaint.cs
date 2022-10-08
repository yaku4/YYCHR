using System.Drawing;
using System.Windows.Forms;
using CharactorLib.Common;
using ControlLib.Properties;

namespace ControlLib.EditFunction;

public class FillPaint : EditFunctionBase
{
	public FillPaint()
	{
		base.Name = ResourceUtility.GetResourceString(EditFunctionBase.ResourceType, "Resources.PenNamePaint");
		base.Icon = Resources.PenIconPaint;
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
			editingBytemap.Fill(mousePoint, base.SelectedPalette);
		}
		if (mMouseButtons == MouseButtons.Right)
		{
			mSelectedPalette = bytemap.GetPixel(mousePoint);
			base.ColorPicked = true;
		}
	}

	public override void MouseUp(Bytemap bytemap, Point mousePoint, MouseButtons mouseButtons)
	{
		if ((mMouseButtons & MouseButtons.Left) == MouseButtons.Left && mouseButtons == MouseButtons.Left)
		{
			editingBytemap.FillRect(new Rectangle(0, 0, editingBytemap.Width, editingBytemap.Height), base.SelectedPalette);
			editingBytemap.CopyRect(base.EditingRect, bytemap, base.EditingRect);
			editingBytemap.Fill(mousePoint, base.SelectedPalette);
			bytemap.CopyRect(base.EditingRect, editingBytemap, base.EditingRect);
			base.EditedFlag = true;
		}
		base.MouseUp(bytemap, mousePoint, mouseButtons);
	}
}
