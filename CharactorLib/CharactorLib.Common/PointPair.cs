using System.Drawing;

namespace CharactorLib.Common;

internal class PointPair
{
	public Point Left { get; set; }

	public Point Right { get; set; }

	public PointPair(Point left, Point right)
	{
		Left = left;
		Right = right;
	}
}
