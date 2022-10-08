using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using CharactorLib.Common;
using CharactorLib.Data;
using CharactorLib.Format;

namespace ControlLib;

public class WorkSpaceSelector : CellSelector
{
	private WsInfo mWsInfo = new WsInfo();

	private FormatManager mFormatManager = FormatManager.GetInstance();

	private string DebugInfo = string.Empty;

	private Bitmap mBitmap;

	private Bytemap mBytemap = new Bytemap(512, 512);

	private MouseButtons mMouseButtons;

	private Point mMouseDownPoint = new Point(0, 0);

	private Point mMouseMovePoint = new Point(0, 0);

	private Point mMouseUpPoint = new Point(0, 0);

	private bool mCtrl;

	private List<PatternInfo> mMovingPatternList = new List<PatternInfo>();

	public bool ForceDraw { get; set; }

	[Browsable(false)]
	public byte[] Data { get; set; }

	public WsInfo WsInfo => mWsInfo;

	public Color BackGroundColor { get; set; } = Color.Black;


	[Browsable(false)]
	public Bitmap Bitmap => mBitmap;

	[Browsable(false)]
	public Bytemap Bytemap => mBytemap;

	[Browsable(false)]
	public Color[] Palette => mBytemap.Palette;

	public void SetPalette(Color[] palette)
	{
		if (palette != null)
		{
			for (int i = 0; i < palette.Length && i < 256; i++)
			{
				mBytemap.Palette[i] = palette[i];
			}
			mBytemap.CanUpdatePalette = true;
			if (mBitmap != null)
			{
				BytemapConvertor.UpdateBitmapPaletteFromBytemap(mBitmap, mBytemap);
			}
		}
	}

	public WorkSpaceSelector()
	{
		SetStyle(ControlStyles.ResizeRedraw, value: true);
		base.DoubleBuffered = true;
		BackGroundColor = Color.Green;
		GetBitmap(base.ClientSize);
	}

	private Bitmap GetBitmap(Size size)
	{
		if (mBitmap == null || mBitmap.Size != size)
		{
			if (mBitmap != null)
			{
				mBitmap.Dispose();
				mBitmap = null;
			}
			mBitmap = new Bitmap(base.Size.Width, base.Size.Height, PixelFormat.Format8bppIndexed);
		}
		return mBitmap;
	}

	private Bytemap GetBytemap(Size size)
	{
		if (mBytemap == null || mBytemap.Size != size)
		{
			if (mBytemap != null)
			{
				mBytemap = null;
			}
			mBytemap = new Bytemap(base.Size);
		}
		return mBytemap;
	}

	protected override void OnPaint(PaintEventArgs pe)
	{
		if (base.Image != mBitmap || base.Image == null)
		{
			base.Image = mBitmap;
		}
		if (ForceDraw || (WsInfo != null && WsInfo.Modified))
		{
			DrawWorkSpaceGraphics();
			ForceDraw = false;
			WsInfo.Modified = false;
		}
		base.OnPaint(pe);
		if (!string.IsNullOrEmpty(DebugInfo))
		{
			pe.Graphics.DrawString(DebugInfo, SystemFonts.DialogFont, SystemBrushes.ActiveCaptionText, new PointF(0f, 0f));
		}
		foreach (PatternInfo pattern in WsInfo.PatternList)
		{
			if (pattern.Selected && pattern.Format < WsInfo.FormatList.Count)
			{
				FormatBase formatBase = WsInfo.FormatList[pattern.Format];
				if (formatBase != null)
				{
					Rectangle rect = new Rectangle(pattern.X * base.ZoomRate, pattern.Y * base.ZoomRate, formatBase.CharWidth * base.ZoomRate, formatBase.CharHeight * base.ZoomRate);
					pe.Graphics.DrawRectangle(Pens.Aqua, rect);
				}
			}
		}
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
		Bytemap bytemap = GetBytemap(base.ClientSize);
		GetBitmap(bytemap.Size);
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		mCtrl = e.Control;
		base.OnKeyDown(e);
	}

	protected override void OnKeyUp(KeyEventArgs e)
	{
		mCtrl = e.Control;
		base.OnKeyUp(e);
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		DebugInfo = string.Empty;
		if (e.Button == MouseButtons.Middle)
		{
			int num = CharactorCommon.NormalizeValue(0, base.Width - 1, e.X) / base.ZoomRate;
			int num2 = CharactorCommon.NormalizeValue(0, base.Height - 1, e.Y) / base.ZoomRate;
			mMouseButtons = e.Button;
			mMouseDownPoint = new Point(num, num2);
			int num3;
			int num4;
			if (base.PixelSelect)
			{
				num3 = 1;
				num4 = 1;
			}
			else
			{
				num3 = 8;
				num4 = 8;
			}
			int num5 = num / num3;
			DebugInfo = string.Concat(str3: (num2 / num4).ToString(), str0: "OnMouseDown ", str1: num5.ToString(), str2: ",");
			if (!mCtrl)
			{
				PatternInfo patternByPosition = WsInfo.GetPatternByPosition(num, num2);
				if (patternByPosition == null || !patternByPosition.Selected)
				{
					WsInfo.UnSelectAll();
				}
			}
			Refresh();
		}
		else
		{
			WsInfo.UnSelectAll();
			base.SelectorVisible = true;
			base.OnMouseDown(e);
		}
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		DebugInfo = string.Empty;
		if (base.Image == null)
		{
			return;
		}
		if (mMouseButtons == MouseButtons.Middle)
		{
			int num = CharactorCommon.NormalizeValue(0, base.Width - 1, e.X) / base.ZoomRate;
			int num2 = CharactorCommon.NormalizeValue(0, base.Height - 1, e.Y) / base.ZoomRate;
			mMouseMovePoint = new Point(num, num2);
			int num3;
			int num4;
			if (base.PixelSelect)
			{
				num3 = 1;
				num4 = 1;
			}
			else
			{
				num3 = 8;
				num4 = 8;
			}
			Rectangle selectedRect = base.SelectedRect;
			Size size = new Size(base.Image.Width, base.Image.Height);
			Size defaultSelectSize = base.DefaultSelectSize;
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
			int num7 = base.DefaultSelectSize.Width;
			int num8 = base.DefaultSelectSize.Height;
			base.FreeSelect = false;
			selectedRect = new Rectangle(num5 * num3, num6 * num4, num7, num8);
			DebugInfo = "OnMouseMove " + num5 + "," + num6;
			if (base.SelectedRect != selectedRect)
			{
				base.SelectedRect = selectedRect;
				Refresh();
			}
		}
		else
		{
			base.OnMouseMove(e);
		}
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		DebugInfo = string.Empty;
		if (e.Button == MouseButtons.Middle)
		{
			int num = CharactorCommon.NormalizeValue(0, base.Width - 1, e.X) / base.ZoomRate;
			int num2 = CharactorCommon.NormalizeValue(0, base.Height - 1, e.Y) / base.ZoomRate;
			mMouseUpPoint = new Point(num, num2);
			if ((mMouseButtons & e.Button) > MouseButtons.None)
			{
				mMouseButtons ^= e.Button;
			}
			int num3;
			int num4;
			if (base.PixelSelect)
			{
				num3 = 1;
				num4 = 1;
			}
			else
			{
				num3 = 8;
				num4 = 8;
			}
			_ = base.SelectedRect;
			Size size = new Size(base.Image.Width, base.Image.Height);
			Size defaultSelectSize = base.DefaultSelectSize;
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
			int num7 = base.DefaultSelectSize.Width;
			int num8 = base.DefaultSelectSize.Height;
			base.FreeSelect = false;
			new Rectangle(num5 * num3, num6 * num4, num7, num8);
			int num9 = mMouseDownPoint.X / num3;
			int num10 = mMouseDownPoint.Y / num4;
			int num11 = num5 - num9;
			int num12 = num6 - num10;
			if (num11 != 0 || num12 != 0)
			{
				WsInfo.MoveSelectedPattern(num11 * num3, num12 * num4);
			}
			else
			{
				if (!mCtrl)
				{
					WsInfo.UnSelectAll();
				}
				WsInfo.SelectPatternByPosition(num, num2);
			}
		}
		else
		{
			base.OnMouseUp(e);
		}
		UpdateSelectorVisibleBySelection();
		Refresh();
	}

	public void UpdateSelectorVisibleBySelection()
	{
		PatternInfo[] selectedPattern = WsInfo.GetSelectedPattern();
		base.SelectorVisible = selectedPattern == null || selectedPattern.Length == 0;
	}

	private void DrawWorkSpaceGraphics()
	{
		if (Data == null || WsInfo == null)
		{
			return;
		}
		mBytemap.FillRect(new Rectangle(new Point(0, 0), mBytemap.Size), 0);
		foreach (PatternInfo pattern in WsInfo.PatternList)
		{
			int format = pattern.Format;
			WsInfo.FormatList[format].ConvertMemToChr(Data, pattern.Address, mBytemap, pattern.X, pattern.Y);
		}
		mBytemap.CanUpdatePixel = true;
		BytemapConvertor.UpdateBitmapAllFromBytemap(mBitmap, mBytemap);
	}
}
