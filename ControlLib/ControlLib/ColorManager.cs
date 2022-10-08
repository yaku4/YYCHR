using System.Collections.Generic;
using System.Drawing;

namespace ControlLib;

public static class ColorManager
{
	public static void AddListPaletteEntry(List<ColorCount> list, Bitmap orgBmp, int bitPer)
	{
		if (orgBmp == null || orgBmp.Palette == null || orgBmp.Palette.Entries == null || orgBmp.Palette.Entries.Length == 0)
		{
			return;
		}
		for (int i = 0; i < orgBmp.Palette.Entries.Length; i++)
		{
			Color color = BitOptColor(orgBmp.Palette.Entries[i], bitPer);
			bool flag = false;
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].Color == color)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.Add(new ColorCount(color));
			}
		}
	}

	public static void CountColorUsed(List<ColorCount> list, Bitmap bmp, int bitPer)
	{
		for (int i = 0; i < bmp.Height; i++)
		{
			for (int j = 0; j < bmp.Width; j++)
			{
				Color color = BitOptColor(bmp.GetPixel(j, i), bitPer);
				bool flag = false;
				for (int k = 0; k < list.Count; k++)
				{
					if (list[k].Color == color)
					{
						list[k].CountUp();
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list.Add(new ColorCount(color));
				}
			}
		}
	}

	public static Color BitOptColor(Color c, int bitPer)
	{
		int num = 8 - bitPer;
		int num2 = (1 << num) / 2;
		int num3 = (c.R >> num << num) + num2;
		int num4 = (c.G >> num << num) + num2;
		int num5 = (c.B >> num << num) + num2;
		return Color.FromArgb(255, (byte)num3, (byte)num4, (byte)num5);
	}
}
