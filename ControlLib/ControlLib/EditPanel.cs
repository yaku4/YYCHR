using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using CharactorLib.Common;

namespace ControlLib;

public class EditPanel : Panel
{
	private const string CATEGORY_GRID = "Grid";

	private const string CATEGORY_VIEW = "View";

	private const string CATEGORY_OPTION = "Option";

	private Bytemap mSourceBytemap;

	private int mZoomRate = 1;

	private Bitmap mViewBitmap;

	private Point oldPoint = new Point(-1, -1);

	private MouseButtons mMouseButtons;

	[Browsable(false)]
	public Bytemap SourceBytemap
	{
		get
		{
			return mSourceBytemap;
		}
		set
		{
			mSourceBytemap = value;
		}
	}

	[Browsable(false)]
	public Bitmap SourceBitmap { get; set; }

	[Browsable(false)]
	public Rectangle SourceRect { get; set; } = new Rectangle(0, 0, 8, 8);


	[Category("View")]
	[DefaultValue(false)]
	public bool ShowClipboard { get; set; }

	[Browsable(false)]
	public Bitmap ClipboardBitmap { get; set; }

	[Category("View")]
	[DefaultValue(1)]
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

	[Browsable(false)]
	public EditFunctionBase EditFunction { get; set; }

	[Category("Grid")]
	[DefaultValue(true)]
	public bool GridVisible { get; set; } = true;


	[Category("Grid")]
	[DefaultValue(GridStyle.Dot)]
	public GridStyle GridStyle { get; set; } = GridStyle.Dot;


	[Category("Grid")]
	[DefaultValue(typeof(Color), "White")]
	public Color GridColor1 { get; set; } = Color.White;


	[Category("Grid")]
	[DefaultValue(typeof(Color), "Black")]
	public Color GridColor2 { get; set; } = Color.Black;


	[Category("Grid")]
	[DefaultValue(typeof(Color), "Aqua")]
	public Color EditingRectColor { get; set; } = Color.Aqua;


	[Browsable(false)]
	public byte SelectedPalette
	{
		get
		{
			if (EditFunction != null)
			{
				try
				{
					return EditFunction.SelectedPalette;
				}
				catch
				{
					return 0;
				}
			}
			return 0;
		}
		set
		{
			if (EditFunction != null)
			{
				try
				{
					EditFunction.SelectedPalette = value;
				}
				catch
				{
				}
			}
		}
	}

	[Category("Option")]
	[DefaultValue(false)]
	public bool ShowDebugInfo { get; set; }

	public event EventHandler OnEdited;

	public event EventHandler OnPicked;

	public event EventHandler OnNaviUpdated;

	private void CallEdited()
	{
		EditFunction.EditedFlag = false;
		if (this.OnEdited != null)
		{
			this.OnEdited(this, EventArgs.Empty);
		}
	}

	private void CallPicked()
	{
		EditFunction.ColorPicked = false;
		if (this.OnPicked != null)
		{
			this.OnPicked(this, EventArgs.Empty);
		}
	}

	private void CallOnNaviUpdated()
	{
		if (this.OnNaviUpdated != null)
		{
			this.OnNaviUpdated(this, EventArgs.Empty);
		}
	}

	public EditPanel()
	{
		SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
		SetStyle(ControlStyles.Selectable, value: false);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		try
		{
			Graphics graphics = e.Graphics;
			GraphicsEx.InitGraphics(graphics);
			if (!ShowClipboard)
			{
				DrawGraphic(graphics);
				DrawGrid(graphics);
				if (EditFunction != null)
				{
					if (EditFunction.EditingFlag && !EditFunction.EditedFlag)
					{
						DrawEditingRect(graphics);
					}
					EditFunction.PaintEx(this, e);
				}
				if (ShowDebugInfo && EditFunction != null)
				{
					Font font = Font;
					graphics.DrawString(EditFunction.MouseDownPoint.ToString(), font, Brushes.Lime, new PointF(5f, 10f));
					graphics.DrawString(EditFunction.MouseMovePoint.ToString(), font, Brushes.Lime, new PointF(5f, 30f));
				}
			}
			else
			{
				DrawClipboard(graphics);
			}
		}
		catch
		{
		}
	}

	private void DrawClipboard(Graphics g)
	{
		int num = base.ClientRectangle.Width;
		int num2 = base.ClientRectangle.Height;
		g.Clear(Color.Gray);
		if (ClipboardBitmap != null && ClipboardBitmap.PixelFormat != 0)
		{
			int zoomRate = ZoomRate;
			Rectangle srcRect = new Rectangle(new Point(0, 0), ClipboardBitmap.Size);
			if (srcRect.Width >= num / zoomRate)
			{
				srcRect.Width = num / zoomRate;
			}
			if (srcRect.Height >= num2 / zoomRate)
			{
				srcRect.Height = num2 / zoomRate;
			}
			Rectangle rectangle = new Rectangle(0, 0, srcRect.Width * zoomRate, srcRect.Height * zoomRate);
			GraphicsEx.DrawImage(g, ClipboardBitmap, rectangle, srcRect);
			using Pen pen = new Pen(GridColor1);
			g.DrawRectangle(pen, rectangle);
		}
	}

	private void DrawGraphic(Graphics g)
	{
		if (EditFunction == null)
		{
			return;
		}
		Bytemap bytemap = null;
		if (EditFunction.EditingFlag)
		{
			bytemap = EditFunction.ViewBytemap;
		}
		else if (SourceBitmap != null)
		{
			bytemap = mSourceBytemap;
		}
		if (bytemap != null)
		{
			Bitmap bitmap = null;
			if (mViewBitmap == null || mViewBitmap.Size != bytemap.Size)
			{
				mViewBitmap = new Bitmap(bytemap.Width, bytemap.Height, PixelFormat.Format8bppIndexed);
			}
			BytemapConvertor.UpdateBitmapPaletteFromBytemap(mViewBitmap, bytemap);
			BytemapConvertor.UpdateBitmapFromBytemap(mViewBitmap, bytemap);
			bitmap = mViewBitmap;
			Rectangle clientRectangle = base.ClientRectangle;
			GraphicsEx.DrawImage(g, bitmap, clientRectangle, SourceRect);
		}
	}

	private void DrawGrid(Graphics g)
	{
		if (!GridVisible || SourceRect.Width == 0 || SourceRect.Height == 0)
		{
			return;
		}
		int num = base.ClientRectangle.Width;
		int num2 = base.ClientRectangle.Height;
		int num3 = SourceRect.Width;
		int num4 = num / num3;
		int num5 = SourceRect.Height;
		int num6 = num2 / num5;
		if (num4 == 0)
		{
			num4 = 1;
		}
		if (num6 == 0)
		{
			num6 = 1;
		}
		if (GridStyle == GridStyle.Point)
		{
			using Brush brush = new SolidBrush(GridColor1);
			using Brush brush2 = new SolidBrush(GridColor2);
			DrawPointGrid(g, brush, brush2, num4, num6);
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
		if (num4 >= 4 && num6 >= 4)
		{
			using Pen pen = new Pen(GridColor2);
			pen.DashStyle = dashStyle;
			DrawGrid(g, pen, num4, num6);
		}
		using Pen pen2 = new Pen(GridColor1);
		pen2.DashStyle = dashStyle;
		DrawGrid(g, pen2, num4 * 8, num6 * 8);
	}

	private void DrawPointGrid(Graphics g, Brush brush1, Brush brush2, int dotXSize, int dotYSize)
	{
		int num = base.ClientRectangle.Width;
		int num2 = base.ClientRectangle.Height;
		int num3 = num / dotXSize;
		Brush brush3;
		for (int i = 1; i < num3; i++)
		{
			int num4 = i * dotXSize;
			int num5 = num2 / dotYSize;
			for (int j = 1; j < num5; j++)
			{
				int num6 = j * dotYSize;
				Rectangle rect = new Rectangle(num4, num6, 1, 1);
				if (i % 8 == 0 || j % 8 == 0)
				{
					brush3 = brush1;
					g.FillRectangle(brush3, rect);
				}
				else if (dotXSize >= 4 && dotYSize >= 4)
				{
					brush3 = brush2;
					g.FillRectangle(brush3, rect);
				}
			}
		}
		brush3 = null;
	}

	private void DrawGrid(Graphics g, Pen pen, int dotXSize, int dotYSize)
	{
		int num = base.ClientRectangle.Width;
		int num2 = base.ClientRectangle.Height;
		int num3 = num / dotXSize;
		for (int i = 1; i < num3; i++)
		{
			int num4 = i * dotXSize;
			Point pt = new Point(num4, 0);
			Point pt2 = new Point(num4, num2);
			g.DrawLine(pen, pt, pt2);
		}
		int num5 = num2 / dotYSize;
		for (int j = 1; j < num5; j++)
		{
			int num6 = j * dotYSize;
			Point pt3 = new Point(0, num6);
			Point pt4 = new Point(num, num6);
			g.DrawLine(pen, pt3, pt4);
		}
	}

	private void DrawEditingRect(Graphics g)
	{
		if (EditFunction == null || SourceRect.Width == 0 || SourceRect.Height == 0)
		{
			return;
		}
		int num = base.ClientRectangle.Width;
		int num2 = base.ClientRectangle.Height;
		int num3 = SourceRect.Width;
		int num4 = num / num3;
		int num5 = SourceRect.Height;
		int num6 = num2 / num5;
		if (num4 == 0)
		{
			num4 = 1;
		}
		if (num6 == 0)
		{
			num6 = 1;
		}
		if (!EditFunction.RectType)
		{
			return;
		}
		DashStyle dashStyle = DashStyle.Dash;
		using Pen pen = new Pen(EditingRectColor);
		pen.DashStyle = dashStyle;
		int x = EditFunction.MouseDownPoint.X - SourceRect.Left;
		int y = EditFunction.MouseDownPoint.Y - SourceRect.Top;
		int x2 = EditFunction.MouseMovePoint.X - SourceRect.Left;
		int y2 = EditFunction.MouseMovePoint.Y - SourceRect.Top;
		Rectangle validRectangleFrom2Point = CharactorCommon.GetValidRectangleFrom2Point(x, y, x2, y2);
		Rectangle rect = Rectangle.FromLTRB(validRectangleFrom2Point.Left * num4, validRectangleFrom2Point.Top * num6, (validRectangleFrom2Point.Right + 1) * num4 - 1, (validRectangleFrom2Point.Bottom + 1) * num6 - 1);
		g.DrawRectangle(pen, rect);
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		base.OnMouseDown(e);
		int num = base.ClientRectangle.Width;
		int num2 = base.ClientRectangle.Height;
		if (ClipboardBitmap == null)
		{
			mMouseButtons |= e.Button;
			int num3 = CharactorCommon.NormalizeValue(0, num - 1, e.X);
			int num4 = CharactorCommon.NormalizeValue(0, num2 - 1, e.Y);
			int num5 = num3 * SourceRect.Width / num + SourceRect.Left;
			int num6 = num4 * SourceRect.Height / num2 + SourceRect.Top;
			EditFunction.EditingRect = SourceRect;
			Point mousePoint = new Point(num5, num6);
			EditFunction.MouseDown(mSourceBytemap, mousePoint, e.Button);
			oldPoint = new Point(-1, -1);
			OnMouseMove(e);
			if (EditFunction.ColorPicked)
			{
				CallPicked();
			}
			if (EditFunction.EditedFlag)
			{
				CallEdited();
			}
			if (EditFunction.EditedFlag || EditFunction.EditingFlag)
			{
				Refresh();
			}
			CallOnNaviUpdated();
		}
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		base.OnMouseMove(e);
		int num = base.ClientRectangle.Width;
		int num2 = base.ClientRectangle.Height;
		if (ClipboardBitmap != null)
		{
			return;
		}
		int num3 = CharactorCommon.NormalizeValue(0, num - 1, e.X);
		int num4 = CharactorCommon.NormalizeValue(0, num2 - 1, e.Y);
		int num5 = num3 * SourceRect.Width / num + SourceRect.Left;
		int num6 = num4 * SourceRect.Height / num2 + SourceRect.Top;
		Point point = new Point(num5, num6);
		if (oldPoint != point)
		{
			EditFunction.MouseMove(mSourceBytemap, point, e.Button);
			oldPoint = point;
			if (EditFunction.ColorPicked)
			{
				CallPicked();
			}
			if (EditFunction.EditedFlag)
			{
				CallEdited();
			}
			if (EditFunction.EditedFlag || EditFunction.EditingFlag)
			{
				Refresh();
			}
		}
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		base.OnMouseUp(e);
		int num = base.ClientRectangle.Width;
		int num2 = base.ClientRectangle.Height;
		if (ClipboardBitmap == null)
		{
			int num3 = CharactorCommon.NormalizeValue(0, num - 1, e.X);
			int num4 = CharactorCommon.NormalizeValue(0, num2 - 1, e.Y);
			int num5 = num3 * SourceRect.Width / num + SourceRect.Left;
			int num6 = num4 * SourceRect.Height / num2 + SourceRect.Top;
			Point mousePoint = new Point(num5, num6);
			EditFunction.MouseUp(mSourceBytemap, mousePoint, e.Button);
			oldPoint = new Point(-1, -1);
			if (EditFunction.ColorPicked)
			{
				CallPicked();
			}
			if (EditFunction.EditedFlag)
			{
				CallEdited();
			}
			if ((mMouseButtons & e.Button) == e.Button)
			{
				mMouseButtons ^= e.Button;
			}
			Refresh();
			CallOnNaviUpdated();
		}
	}
}
