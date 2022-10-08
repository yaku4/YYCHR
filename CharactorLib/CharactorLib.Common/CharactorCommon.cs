using System.Drawing;

namespace CharactorLib.Common;

public class CharactorCommon
{
	public static int[] SortValue(int val1, int val2)
	{
		int[] array = new int[2];
		if (val1 <= val2)
		{
			array[0] = val1;
			array[1] = val2;
		}
		else
		{
			array[0] = val2;
			array[1] = val1;
		}
		return array;
	}

	public static int NormalizeValue(int min, int max, int value)
	{
		if (value < min)
		{
			value = min;
		}
		if (value > max)
		{
			value = max;
		}
		return value;
	}

	public static Point ClipPoint(Rectangle rect, Point point)
	{
		int x = NormalizeValue(rect.Left, rect.Right, point.X);
		int y = NormalizeValue(rect.Top, rect.Bottom, point.Y);
		return new Point(x, y);
	}

	public static Rectangle GetClipedRect(Rectangle rect, Bitmap bitmap)
	{
		return new Rectangle(NormalizeValue(0, bitmap.Width, rect.Left), NormalizeValue(0, bitmap.Height, rect.Top), NormalizeValue(0, bitmap.Width, rect.Right), NormalizeValue(0, bitmap.Height, rect.Bottom));
	}

	public static Rectangle GetValidRectangleFrom2Point(Point p1, Point p2)
	{
		return GetValidRectangleFrom2Point(p1.X, p1.Y, p2.X, p2.Y);
	}

	public static Rectangle GetValidRectangleFrom2Point(int x1, int y1, int x2, int y2)
	{
		int[] array = SortValue(x1, x2);
		int[] array2 = SortValue(y1, y2);
		return Rectangle.FromLTRB(array[0], array2[0], array[1], array2[1]);
	}
}
