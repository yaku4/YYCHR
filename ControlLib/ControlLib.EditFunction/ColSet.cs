using System.Drawing;
using System.Windows.Forms;
using CharactorLib.Common;
using CharactorLib.Data;
using ControlLib.Properties;

namespace ControlLib.EditFunction;

public class ColSet : EditFunctionBase
{
	public ColSetData ColSetData { get; set; }

	public AdfInfo AdfPattern { get; set; }

	public int ColSetDataAddr { get; set; }

	public ColSet()
	{
		base.Name = ResourceUtility.GetResourceString(EditFunctionBase.ResourceType, "Resources.PenNameSetPalette");
		base.Icon = Resources.PenIconSetPalette;
		base.CopyOriginal = false;
		base.RectType = false;
	}

	public override void MouseDown(Bytemap bytemap, Point mousePoint, MouseButtons mouseButtons)
	{
		base.MouseDown(bytemap, mousePoint, mouseButtons);
	}

	public override void MouseMove(Bytemap bytemap, Point mousePoint, MouseButtons mouseButtons)
	{
		base.MouseMove(bytemap, mousePoint, mouseButtons);
		if ((mMouseButtons & MouseButtons.Left) == MouseButtons.Left && ColSetData != null)
		{
			int x = mMouseMovePoint.X / 8;
			int y = mMouseMovePoint.Y / 8;
			SetColData(x, y);
			base.EditedFlag = true;
		}
		if (mMouseButtons == MouseButtons.Right)
		{
			mSelectedPalette = bytemap.GetPixel(mousePoint);
			base.ColorPicked = true;
		}
	}

	public override void MouseUp(Bytemap bytemap, Point mouseUpPoint, MouseButtons mouseButtons)
	{
		if ((mMouseButtons & MouseButtons.Left) == MouseButtons.Left && ColSetData != null)
		{
			int x = mouseUpPoint.X / 8;
			int y = mouseUpPoint.Y / 8;
			SetColData(x, y);
			base.EditedFlag = true;
		}
		base.MouseUp(bytemap, mouseUpPoint, mouseButtons);
	}

	private void SetColData(int x, int y)
	{
		if (AdfPattern == null || AdfPattern.Pattern == null || AdfPattern.Pattern.Length < 256)
		{
			return;
		}
		int num = y * 16 + x;
		int num2 = AdfPattern.Pattern[num];
		if (!AdfPattern.IsDisableFF || num2 != 255)
		{
			int num3 = ColSetDataAddr + num2;
			if (num3 < ColSetData.Data.Length)
			{
				byte b = mSelectedPaletteSet;
				ColSetData.Data[num3] = b;
			}
		}
	}
}
