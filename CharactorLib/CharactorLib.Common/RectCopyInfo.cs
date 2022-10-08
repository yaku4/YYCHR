using System.Drawing;

namespace CharactorLib.Common;

public class RectCopyInfo
{
	public bool Enabled { get; set; }

	public bool Selected { get; set; }

	public int Address { get; set; }

	public Point SrcPoint { get; set; }

	public Point DestPoint { get; set; }

	public Size Size { get; set; }

	public MirrorType Mirror { get; set; }

	public RotateType Rotate { get; set; }

	public int SX
	{
		get
		{
			return SrcPoint.X;
		}
		set
		{
			SrcPoint = new Point(value, SY);
		}
	}

	public int SY
	{
		get
		{
			return SrcPoint.Y;
		}
		set
		{
			SrcPoint = new Point(SX, value);
		}
	}

	public int DX
	{
		get
		{
			return DestPoint.X;
		}
		set
		{
			DestPoint = new Point(value, DY);
		}
	}

	public int DY
	{
		get
		{
			return DestPoint.Y;
		}
		set
		{
			DestPoint = new Point(DX, value);
		}
	}

	public int W
	{
		get
		{
			return Size.Width;
		}
		set
		{
			Size = new Size(value, H);
		}
	}

	public int H
	{
		get
		{
			return Size.Height;
		}
		set
		{
			Size = new Size(W, value);
		}
	}

	public Rectangle SrcRect => new Rectangle(SrcPoint, Size);

	public RectCopyInfo()
	{
		Enabled = true;
		Selected = false;
		Address = 0;
		SrcPoint = new Point(0, 0);
		DestPoint = new Point(0, 0);
		Size = new Size(8, 8);
		Mirror = MirrorType.None;
		Rotate = RotateType.None;
	}

	public void CopyFrom(RectCopyInfo src)
	{
		Enabled = src.Enabled;
		Selected = src.Selected;
		Address = src.Address;
		SrcPoint = src.SrcPoint;
		DestPoint = src.DestPoint;
		Size = src.Size;
		Mirror = src.Mirror;
		Rotate = src.Rotate;
	}
}
