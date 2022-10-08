using System;
using System.Drawing;

namespace ControlLib;

public class ColorCount : IComparable
{
	public Color Color { get; set; }

	public int Count { get; set; }

	public bool Enabled { get; set; } = true;


	public bool ManualDisabled { get; set; }

	public int OutPaletteIndex { get; set; } = 255;


	public ColorCount(Color color)
	{
		Color = color;
		Count = 1;
	}

	public int CountUp()
	{
		Count++;
		return Count;
	}

	public int CompareTo(object obj)
	{
		return (obj as ColorCount).Count - Count;
	}
}
