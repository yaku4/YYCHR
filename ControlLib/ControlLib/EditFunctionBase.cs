using System;
using System.Drawing;
using System.Windows.Forms;
using CharactorLib.Common;
using ControlLib.Properties;

namespace ControlLib;

public class EditFunctionBase
{
	protected static Type ResourceType = typeof(Resources);

	protected byte mSelectedPalette;

	protected byte mSelectedPaletteSet;

	private bool mEditingFlag;

	private bool mEditedFlag;

	private bool mColorPicked;

	private Rectangle mEditingRect = new Rectangle(0, 0, 8, 8);

	protected Bytemap editingBytemap;

	protected Bytemap mViewBytemap;

	protected MouseButtons mMouseButtons;

	protected Point mMouseDownPoint = new Point(0, 0);

	protected Point mMouseMovePoint = new Point(0, 0);

	public string Name { get; protected set; } = string.Empty;


	public Image Icon { get; protected set; }

	public bool RectType { get; protected set; }

	public bool CopyOriginal { get; protected set; } = true;


	public byte SelectedPalette
	{
		get
		{
			return mSelectedPalette;
		}
		set
		{
			mSelectedPalette = value;
		}
	}

	public byte SelectedPaletteSet
	{
		get
		{
			return mSelectedPaletteSet;
		}
		set
		{
			mSelectedPaletteSet = value;
		}
	}

	public bool EditingFlag
	{
		get
		{
			return mEditingFlag;
		}
		set
		{
			mEditingFlag = value;
		}
	}

	public bool EditedFlag
	{
		get
		{
			return mEditedFlag;
		}
		set
		{
			mEditedFlag = value;
		}
	}

	public bool ColorPicked
	{
		get
		{
			return mColorPicked;
		}
		set
		{
			mColorPicked = value;
		}
	}

	public Rectangle EditingRect
	{
		get
		{
			return mEditingRect;
		}
		set
		{
			mEditingRect = value;
		}
	}

	public Bytemap ViewBytemap => mViewBytemap;

	public MouseButtons MouseButtons => mMouseButtons;

	public Point MouseDownPoint => mMouseDownPoint;

	public Point MouseMovePoint => mMouseMovePoint;

	public virtual string[] GetNaviText()
	{
		string text = ResourceUtility.GetResourceString(ResourceType, "Resources.NaviEditL").Replace("%PEN%", Name);
		string resourceString = ResourceUtility.GetResourceString(ResourceType, "Resources.NaviEditR");
		return new string[2] { text, resourceString };
	}

	public virtual void OnChrLoad()
	{
	}

	public virtual void OnEnter()
	{
	}

	public virtual void OnLeave()
	{
	}

	public virtual void MouseDown(Bytemap bytemap, Point mousePoint, MouseButtons mouseButtons)
	{
		if ((mouseButtons & MouseButtons.Left) == MouseButtons.Left)
		{
			mEditingFlag = true;
			if (bytemap != null)
			{
				editingBytemap = bytemap.Clone();
				mViewBytemap = editingBytemap;
			}
		}
		if ((mMouseButtons & MouseButtons.Left) != MouseButtons.Left && (mouseButtons & MouseButtons.Left) == MouseButtons.Left)
		{
			mMouseDownPoint = mousePoint;
		}
		mMouseButtons |= mouseButtons;
	}

	public virtual void MouseMove(Bytemap bytemap, Point mousePoint, MouseButtons mouseButtons)
	{
		mMouseMovePoint = mousePoint;
		if ((mMouseButtons & MouseButtons.Left) == MouseButtons.Left && CopyOriginal && bytemap != null)
		{
			editingBytemap.FillRect(new Rectangle(0, 0, editingBytemap.Width, editingBytemap.Height), SelectedPalette);
			editingBytemap.CopyRect(EditingRect, bytemap, EditingRect);
		}
	}

	public virtual void MouseUp(Bytemap bytemap, Point mousePoint, MouseButtons mouseButtons)
	{
		if (mouseButtons == MouseButtons.Left)
		{
			mEditingFlag = false;
			mViewBytemap = null;
		}
		if ((mMouseButtons & mouseButtons) == mouseButtons)
		{
			mMouseButtons ^= mouseButtons;
		}
	}

	public virtual void PaintEx(object sender, PaintEventArgs e)
	{
	}
}
