using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CharactorLib.Common;
using ControlLib.Properties;

namespace ControlLib.EditFunction;

public class Stamp : EditFunctionBase
{
	private enum StampStep
	{
		None,
		SelectMouseDown,
		SelectMouseMove,
		SelectMouseUp,
		Selected,
		MoveMouseDown,
		MoveMouseMove,
		MoveMouseUp
	}

	private Rectangle _MovedRect = new Rectangle(0, 0, 0, 0);

	private StampStep _Step { get; set; }

	private Point _Select1 { get; set; } = new Point(0, 0);


	private Point _Select2 { get; set; } = new Point(0, 0);


	private Rectangle _SelectedRect { get; set; } = new Rectangle(0, 0, 0, 0);


	private Point _Move1 { get; set; } = new Point(0, 0);


	private Point _Move2 { get; set; } = new Point(0, 0);


	private Point _MoveShift { get; set; } = new Point(0, 0);


	public Stamp()
	{
		base.Name = ResourceUtility.GetResourceString(EditFunctionBase.ResourceType, "Resources.PenNameStamp");
		base.Icon = Resources.PenIconStamp;
		base.RectType = true;
	}

	public override string[] GetNaviText()
	{
		string empty = string.Empty;
		empty = ((_Step >= StampStep.Selected) ? ResourceUtility.GetResourceString(EditFunctionBase.ResourceType, "Resources.NaviEditStampL2") : ResourceUtility.GetResourceString(EditFunctionBase.ResourceType, "Resources.NaviEditStampL1"));
		string resourceString = ResourceUtility.GetResourceString(EditFunctionBase.ResourceType, "Resources.NaviEditR");
		return new string[2] { empty, resourceString };
	}

	public override void OnChrLoad()
	{
		ResetStep();
	}

	public override void OnEnter()
	{
		ResetStep();
	}

	public override void OnLeave()
	{
		ResetStep();
	}

	private void ResetStep()
	{
		_Step = StampStep.None;
		_SelectedRect = new Rectangle(0, 0, 0, 0);
		_MovedRect = new Rectangle(0, 0, 0, 0);
	}

	public override void MouseDown(Bytemap bytemap, Point mousePoint, MouseButtons mouseButtons)
	{
		base.MouseDown(bytemap, mousePoint, mouseButtons);
		if ((mMouseButtons & MouseButtons.Left) == MouseButtons.Left)
		{
			if (_Step >= StampStep.Selected && !_SelectedRect.Contains(mousePoint))
			{
				ResetStep();
			}
			if (_Step == StampStep.None)
			{
				base.RectType = true;
				_Step = StampStep.SelectMouseDown;
				_Select1 = new Point(mousePoint.X, mousePoint.Y);
			}
			else if (_Step == StampStep.Selected)
			{
				base.RectType = false;
				_Step = StampStep.MoveMouseDown;
				_Move1 = new Point(mousePoint.X, mousePoint.Y);
			}
		}
		else
		{
			_Step = StampStep.None;
		}
		MouseMove(bytemap, mousePoint, mouseButtons);
	}

	public override void MouseMove(Bytemap bytemap, Point mousePoint, MouseButtons mouseButtons)
	{
		base.MouseMove(bytemap, mousePoint, mouseButtons);
		if ((mMouseButtons & MouseButtons.Left) == MouseButtons.Left)
		{
			if (_Step == StampStep.SelectMouseDown)
			{
				_Step = StampStep.SelectMouseMove;
			}
			else if (_Step == StampStep.MoveMouseDown)
			{
				_Step = StampStep.MoveMouseMove;
			}
			if (_Step == StampStep.SelectMouseMove)
			{
				_Select2 = new Point(mousePoint.X, mousePoint.Y);
				TempSelect(_Select1, _Select2);
			}
			else if (_Step == StampStep.MoveMouseMove)
			{
				_Move2 = new Point(mousePoint.X, mousePoint.Y);
				TempMove(_Move1, _Move2);
			}
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
			base.EditedFlag = true;
			if (_Step == StampStep.SelectMouseMove)
			{
				_Step = StampStep.SelectMouseUp;
				_Select2 = new Point(mousePoint.X, mousePoint.Y);
				TempSelect(_Select1, _Select2);
				_Step = StampStep.Selected;
			}
			else if (_Step == StampStep.MoveMouseMove)
			{
				_Step = StampStep.MoveMouseUp;
				_Move2 = new Point(mousePoint.X, mousePoint.Y);
				TempMove(_Move1, _Move2);
				Rectangle movedRect = _MovedRect;
				Rectangle editingRect = base.EditingRect;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				if (movedRect.Left < editingRect.Left)
				{
					num = editingRect.Left - movedRect.Left;
				}
				if (movedRect.Top < editingRect.Top)
				{
					num2 = editingRect.Top - movedRect.Top;
				}
				if (movedRect.Right >= editingRect.Right)
				{
					num3 = movedRect.Right - editingRect.Right;
				}
				if (movedRect.Bottom >= editingRect.Bottom)
				{
					num4 = movedRect.Bottom - editingRect.Bottom;
				}
				int left = movedRect.Left + num;
				int top = movedRect.Top + num2;
				int right = movedRect.Right - num3;
				int bottom = movedRect.Bottom - num4;
				Rectangle rectangle = Rectangle.FromLTRB(left, top, right, bottom);
				int left2 = _SelectedRect.Left + num;
				int top2 = _SelectedRect.Top + num2;
				int right2 = _SelectedRect.Right - num3;
				int bottom2 = _SelectedRect.Bottom - num4;
				Rectangle srcRect = Rectangle.FromLTRB(left2, top2, right2, bottom2);
				bytemap.CopyRect(rectangle, srcRect);
				_SelectedRect = rectangle;
				_MovedRect = rectangle;
				_Step = StampStep.Selected;
			}
		}
		base.MouseUp(bytemap, mousePoint, mouseButtons);
	}

	private void TempSelect(Point _Select1, Point _Select2)
	{
		int left = Math.Min(_Select1.X, _Select2.X);
		int num = Math.Max(_Select1.X, _Select2.X);
		int top = Math.Min(_Select1.Y, _Select2.Y);
		int num2 = Math.Max(_Select1.Y, _Select2.Y);
		_SelectedRect = Rectangle.FromLTRB(left, top, num + 1, num2 + 1);
	}

	private void TempMove(Point _Move1, Point _Move2)
	{
		int x = _Move2.X - _Move1.X;
		int y = _Move2.Y - _Move1.Y;
		_MoveShift = new Point(x, y);
		_MovedRect = new Rectangle(_SelectedRect.Left, _SelectedRect.Top, _SelectedRect.Width, _SelectedRect.Height);
		_MovedRect.Offset(_MoveShift);
	}

	public override void PaintEx(object sender, PaintEventArgs e)
	{
		base.PaintEx(sender, e);
		Graphics graphics = e.Graphics;
		Control control = sender as Control;
		if (control is PictureBox)
		{
			control = control.Parent;
		}
		if (!(control is EditPanel) || !(control is EditPanel editPanel))
		{
			return;
		}
		int num = 1;
		try
		{
			int val = editPanel.ClientSize.Width / editPanel.SourceRect.Width;
			int val2 = editPanel.ClientSize.Height / editPanel.SourceRect.Height;
			num = Math.Min(val, val2);
		}
		catch
		{
			return;
		}
		if (_Step >= StampStep.Selected)
		{
			if (_SelectedRect.Width == 0 || _SelectedRect.Height == 0)
			{
				return;
			}
			try
			{
				int num2 = _SelectedRect.X - editPanel.SourceRect.X;
				int num3 = _SelectedRect.Y - editPanel.SourceRect.Y;
				Rectangle rect = new Rectangle(num2 * num, num3 * num, _SelectedRect.Width * num - 1, _SelectedRect.Height * num - 1);
				using Pen pen = new Pen(editPanel.EditingRectColor);
				graphics.DrawRectangle(pen, rect);
			}
			catch
			{
			}
		}
		if (_Step < StampStep.MoveMouseMove || _MovedRect.Width == 0 || _MovedRect.Height == 0)
		{
			return;
		}
		try
		{
			int num4 = _MovedRect.X - editPanel.SourceRect.X;
			int num5 = _MovedRect.Y - editPanel.SourceRect.Y;
			Rectangle rect2 = new Rectangle(num4 * num, num5 * num, _MovedRect.Width * num - 1, _MovedRect.Height * num - 1);
			using Pen pen2 = new Pen(Color.Yellow);
			pen2.DashStyle = DashStyle.Dash;
			graphics.DrawRectangle(pen2, rect2);
		}
		catch
		{
		}
	}
}
