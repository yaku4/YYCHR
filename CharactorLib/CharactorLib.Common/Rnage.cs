namespace CharactorLib.Common;

public class Rnage
{
	public int L { get; set; }

	public int R { get; set; }

	public int Y { get; set; }

	public Rnage(int l, int r, int y)
	{
		L = l;
		R = r;
		Y = y;
	}

	public Rnage(int x1, int y1, int x2, int y2)
	{
		L = x1;
		R = x2;
		Y = y1;
	}
}
