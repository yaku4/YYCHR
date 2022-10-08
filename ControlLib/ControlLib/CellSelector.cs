using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using CharactorLib.Common;

namespace ControlLib;

public class CellSelector : PictureBox
{
	private bool mEnableRightDragSelect;

	private bool mEnableMiddleClickSelect;

	private int mCellWidth = 8;

	private int mCellHeight = 8;

	private Size mDefaultSelectSize = new Size(16, 16);

	private int mZoomRate = 4;

	private bool mGridVisible = true;

	private GridStyle mGridStyle = GridStyle.Line;

	private Color mGridColor1 = Color.White;

	private Color mGridColor2 = Color.White;

	private bool mSelectorVisible = true;

	private Color mSelectedColor1 = Color.Aqua;

	private Color mSelectedColor2 = Color.Aqua;

	private bool mSelectedColorNegative;

	private static ImageAttributes mImageAttributes = new ImageAttributes();

	private MouseButtons mMouseButtons;

	private Point mMouseDownPoint = new Point(0, 0);

	private Point mMouseMovePoint = new Point(0, 0);

	private Point mMouseUpPoint = new Point(0, 0);

	private object lockMouseDownNew = "lockMouseDownNew";

	[DefaultValue(false)]
	public bool EnableRightDragSelect
	{
		get
		{
			return mEnableRightDragSelect;
		}
		set
		{
			mEnableRightDragSelect = value;
		}
	}

	[DefaultValue(false)]
	public bool EnableMiddleClickSelect
	{
		get
		{
			return mEnableMiddleClickSelect;
		}
		set
		{
			mEnableMiddleClickSelect = value;
		}
	}

	public Rectangle SelectedRect { get; set; }

	public Rectangle SelectedRectCell => new Rectangle(SelectedRect.X / CellWidth, SelectedRect.Y / CellHeight, SelectedRect.Width / CellWidth, SelectedRect.Height / CellHeight);

	public int SelectedIndex
	{
		get
		{
			int num = DefaultSelectSize.Width;
			int num2 = DefaultSelectSize.Height;
			int num3 = base.Width / (num * ZoomRate);
			int num4 = SelectedRect.Left / num;
			return SelectedRect.Top / num2 * num3 + num4;
		}
		set
		{
			int num = DefaultSelectSize.Width;
			int num2 = DefaultSelectSize.Height;
			int num3 = base.Width / (num * ZoomRate);
			int num4 = value % num3 * num;
			int num5 = value / num3 * num2;
			Rectangle rectangle2 = (SelectedRect = new Rectangle(num4, num5, num, num2));
		}
	}

	[DefaultValue(8)]
	public int CellWidth
	{
		get
		{
			return mCellWidth;
		}
		set
		{
			mCellWidth = value;
		}
	}

	[DefaultValue(8)]
	public int CellHeight
	{
		get
		{
			return mCellHeight;
		}
		set
		{
			mCellHeight = value;
		}
	}

	public bool FreeSelect { get; set; }

	public bool PixelSelect { get; set; }

	public Size DefaultSelectSize
	{
		get
		{
			return mDefaultSelectSize;
		}
		set
		{
			if (Image != null)
			{
				Point location = SelectedRect.Location;
				if (value.Width + location.X > Image.Width)
				{
					location.X = Image.Width - value.Width;
				}
				if (value.Height + location.Y > Image.Height)
				{
					location.Y = Image.Height - value.Height;
				}
				SelectedRect = new Rectangle(location, value);
			}
			mDefaultSelectSize = value;
		}
	}

	public new Bitmap Image { get; set; }

	public int ZoomRate
	{
		get
		{
			if (mZoomRate == 0)
			{
				mZoomRate = 1;
			}
			return mZoomRate;
		}
		set
		{
			if (value == 0)
			{
				value = 1;
			}
			mZoomRate = value;
		}
	}

	[DefaultValue(true)]
	public bool GridVisible
	{
		get
		{
			return mGridVisible;
		}
		set
		{
			mGridVisible = value;
		}
	}

	[DefaultValue(GridStyle.Line)]
	public GridStyle GridStyle
	{
		get
		{
			return mGridStyle;
		}
		set
		{
			mGridStyle = value;
		}
	}

	public Color GridColor1
	{
		get
		{
			return mGridColor1;
		}
		set
		{
			mGridColor1 = value;
		}
	}

	public Color GridColor2
	{
		get
		{
			return mGridColor2;
		}
		set
		{
			mGridColor2 = value;
		}
	}

	[DefaultValue(true)]
	public bool SelectorVisible
	{
		get
		{
			return mSelectorVisible;
		}
		set
		{
			mSelectorVisible = value;
		}
	}

	public Color SelectedColor1
	{
		get
		{
			return mSelectedColor1;
		}
		set
		{
			mSelectedColor1 = value;
		}
	}

	public Color SelectedColor2
	{
		get
		{
			return mSelectedColor2;
		}
		set
		{
			mSelectedColor2 = value;
		}
	}

	[DefaultValue(false)]
	public bool SelectedColorNegative
	{
		get
		{
			return mSelectedColorNegative;
		}
		set
		{
			mSelectedColorNegative = value;
		}
	}

	public bool MouseDownNew { get; set; } = true;


	public event MouseEventHandler Selected;

	public CellSelector()
	{
		Image = null;
		EnableRightDragSelect = true;
		EnableMiddleClickSelect = false;
		GridVisible = true;
		GridStyle = GridStyle.Dot;
		GridColor1 = Color.White;
		GridColor2 = Color.Gray;
		SelectorVisible = true;
		SelectedColor1 = Color.White;
		SelectedColor2 = Color.Aqua;
		SelectedRect = new Rectangle(0, 0, 16, 16);
		ZoomRate = 1;
		FreeSelect = false;
		ColorMatrix colorMatrix = new ColorMatrix(new float[5][]
		{
			new float[5] { -1f, 0f, 0f, 0f, 0f },
			new float[5] { 0f, -1f, 0f, 0f, 0f },
			new float[5] { 0f, 0f, -1f, 0f, 0f },
			new float[5] { 0f, 0f, 0f, 1f, 0f },
			new float[5] { 1f, 1f, 1f, 0f, 1f }
		});
		mImageAttributes.SetColorMatrix(colorMatrix);
		base.Paint += this_Paint;
		base.MouseDown += this_MouseDown;
		base.MouseMove += this_MouseMove;
		base.MouseUp += this_MouseUp;
	}

	private void this_Paint(object sender, PaintEventArgs e)
	{
		try
		{
			Graphics graphics = e.Graphics;
			GraphicsEx.InitGraphics(graphics);
			DrawImage(graphics);
			if (SelectedColorNegative)
			{
				DrawNegativeSelection(graphics);
				DrawGrid(graphics);
			}
			else
			{
				DrawGrid(graphics);
				DrawSelection(graphics);
			}
		}
		catch
		{
		}
	}

	private void DrawImage(Graphics g)
	{
		if (Image != null)
		{
			GraphicsEx.DrawImage(srcRect: new Rectangle(0, 0, Image.Width, Image.Height), destRect: new Rectangle(0, 0, Image.Width * ZoomRate, Image.Height * ZoomRate), g: g, image: Image);
		}
	}

	private void DrawGrid(Graphics g)
	{
		if (!GridVisible)
		{
			return;
		}
		int num = CellWidth * ZoomRate;
		int num2 = CellHeight * ZoomRate;
		if (GridStyle == GridStyle.Point)
		{
			using Brush brush = new SolidBrush(GridColor1);
			DrawPointGrid(g, brush, num, num2);
		}
		if (GridStyle != GridStyle.Dot && GridStyle != GridStyle.Dash && GridStyle != GridStyle.Line)
		{
			return;
		}
		DashStyle dashStyle = DashStyle.Custom;
		switch (GridStyle)
		{
		case GridStyle.Dot:
			dashStyle = DashStyle.Dot;
			break;
		case GridStyle.Dash:
			dashStyle = DashStyle.Dash;
			break;
		case GridStyle.Line:
			dashStyle = DashStyle.Solid;
			break;
		}
		using (Pen pen = new Pen(GridColor2))
		{
			pen.DashStyle = dashStyle;
			DrawPenGrid(g, pen, num, num2);
		}
		using Pen pen2 = new Pen(GridColor1);
		pen2.DashStyle = dashStyle;
		DrawPenGrid(g, pen2, num * 8, num2 * 8);
	}

	private void DrawPointGrid(Graphics g, Brush brush, int chrWidth, int chrHeight)
	{
		for (int i = chrHeight; i < base.Height; i += chrHeight)
		{
			for (int j = chrWidth; j < base.Width; j += chrWidth)
			{
				Rectangle rect = new Rectangle(j, i, 1, 1);
				g.FillRectangle(brush, rect);
			}
		}
	}

	private void DrawPenGrid(Graphics g, Pen pen, int chrWidth, int chrHeight)
	{
		_ = base.Height / chrHeight;
		_ = base.Width / chrWidth;
		for (int i = chrWidth; i < base.Width; i += chrWidth)
		{
			Point pt = new Point(i, 0);
			Point pt2 = new Point(i, base.Height);
			g.DrawLine(pen, pt, pt2);
		}
		for (int j = chrHeight; j < base.Height; j += chrHeight)
		{
			Point pt3 = new Point(0, j);
			Point pt4 = new Point(base.Width, j);
			g.DrawLine(pen, pt3, pt4);
		}
	}

	private void DrawSelection(Graphics g)
	{
		if (!SelectorVisible)
		{
			return;
		}
		Color color = ((!FreeSelect) ? SelectedColor1 : SelectedColor2);
		int zoomRate = ZoomRate;
		Rectangle rect = new Rectangle(SelectedRect.Left * zoomRate, SelectedRect.Top * zoomRate, SelectedRect.Width * zoomRate - 1, SelectedRect.Height * zoomRate - 1);
		using Pen pen = new Pen(color);
		g.DrawRectangle(pen, rect);
	}

	private void DrawNegativeSelection(Graphics g)
	{
		if (SelectorVisible)
		{
			int zoomRate = ZoomRate;
			Rectangle destRect = new Rectangle(SelectedRect.Left * zoomRate, SelectedRect.Top * zoomRate, SelectedRect.Width * zoomRate, SelectedRect.Height * zoomRate);
			GraphicsEx.DrawImage(g, Image, destRect, SelectedRect, mImageAttributes);
		}
	}

	private void this_MouseDown(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Right)
		{
			SetMouseDownNew(state: true);
		}
		int num = CharactorCommon.NormalizeValue(0, base.Width - 1, e.X) / ZoomRate;
		int num2 = CharactorCommon.NormalizeValue(0, base.Height - 1, e.Y) / ZoomRate;
		mMouseButtons = e.Button;
		mMouseDownPoint = new Point(num, num2);
		this_MouseMove(sender, e);
	}

	private void this_MouseMove(object sender, MouseEventArgs e)
	{
		if (Image == null)
		{
			return;
		}
		int num = CharactorCommon.NormalizeValue(0, base.Width - 1, e.X) / ZoomRate;
		int num2 = CharactorCommon.NormalizeValue(0, base.Height - 1, e.Y) / ZoomRate;
		mMouseMovePoint = new Point(num, num2);
		int num3;
		int num4;
		if (PixelSelect)
		{
			num3 = 1;
			num4 = 1;
		}
		else
		{
			num3 = CellWidth;
			num4 = CellHeight;
		}
		Rectangle rectangle = SelectedRect;
		if (mMouseButtons == MouseButtons.Left || (mMouseButtons == MouseButtons.Right && !EnableRightDragSelect) || (mMouseButtons == MouseButtons.Middle && EnableMiddleClickSelect))
		{
			Size size = new Size(Image.Width, Image.Height);
			Size defaultSelectSize = DefaultSelectSize;
			if (num >= size.Width - defaultSelectSize.Width)
			{
				num = size.Width - defaultSelectSize.Width;
			}
			if (num2 >= size.Width - defaultSelectSize.Height)
			{
				num2 = size.Width - defaultSelectSize.Height;
			}
			int num5 = num / num3;
			int num6 = num2 / num4;
			int num7 = DefaultSelectSize.Width;
			int num8 = DefaultSelectSize.Height;
			FreeSelect = false;
			rectangle = new Rectangle(num5 * num3, num6 * num4, num7, num8);
		}
		if (mMouseButtons == MouseButtons.Right && EnableRightDragSelect)
		{
			FreeSelect = true;
			int num9 = Math.Min(mMouseDownPoint.X / num3, mMouseMovePoint.X / num3);
			int num10 = Math.Min(mMouseDownPoint.Y / num4, mMouseMovePoint.Y / num4);
			int num11 = Math.Abs(mMouseDownPoint.X / num3 - mMouseMovePoint.X / num3);
			int num12 = Math.Abs(mMouseDownPoint.Y / num4 - mMouseMovePoint.Y / num4);
			rectangle = new Rectangle(num9 * num3, num10 * num4, (num11 + 1) * num3, (num12 + 1) * num4);
		}
		if (SelectedRect != rectangle)
		{
			SelectedRect = rectangle;
			Refresh();
			if (this.Selected != null)
			{
				this.Selected(this, e);
			}
		}
	}

	private void this_MouseUp(object sender, MouseEventArgs e)
	{
		int num = CharactorCommon.NormalizeValue(0, base.Width - 1, e.X) / ZoomRate;
		int num2 = CharactorCommon.NormalizeValue(0, base.Height - 1, e.Y) / ZoomRate;
		mMouseUpPoint = new Point(num, num2);
		if ((mMouseButtons & e.Button) > MouseButtons.None)
		{
			mMouseButtons ^= e.Button;
		}
	}

	public void SetMouseDownNew(bool state)
	{
		if (MouseDownNew != state)
		{
			lock (lockMouseDownNew)
			{
				MouseDownNew = state;
			}
		}
	}
}
