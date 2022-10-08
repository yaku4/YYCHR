using System;
using CharactorLib.Common;

namespace CharactorLib.Data;

public class PatternInfo
{
	private const int csvValueCount = 7;

	public int Address { get; set; }

	public int Format { get; set; }

	public int PalSet { get; set; }

	public MirrorType Mirror { get; set; }

	public RotateType Rotate { get; set; }

	public int X { get; set; }

	public int Y { get; set; }

	public bool Selected { get; set; }

	public PatternInfo()
	{
		Address = 0;
		Format = 0;
		PalSet = 0;
		Mirror = MirrorType.None;
		Rotate = RotateType.None;
		X = 0;
		Y = 0;
		Selected = false;
	}

	public static PatternInfo FromString(string s)
	{
		string[] array = s.Split(',');
		if (array == null || array.Length != 7)
		{
			return null;
		}
		try
		{
			PatternInfo patternInfo = new PatternInfo();
			patternInfo.Address = Convert.ToInt32(array[0], 16);
			patternInfo.Format = Convert.ToInt32(array[1], 16);
			patternInfo.PalSet = Convert.ToInt32(array[2], 16);
			patternInfo.Mirror = (MirrorType)Convert.ToInt32(array[3]);
			patternInfo.Rotate = (RotateType)Convert.ToInt32(array[4]);
			patternInfo.X = Convert.ToInt32(array[5], 16);
			patternInfo.Y = Convert.ToInt32(array[6], 16);
			return patternInfo;
		}
		catch
		{
			return null;
		}
	}

	public override string ToString()
	{
		return string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Empty + Address.ToString("X") + ",", Format.ToString("X"), ","), PalSet.ToString("X"), ","), ((int)Mirror).ToString(), ","), ((int)Rotate).ToString(), ","), X.ToString("X"), ","), Y.ToString("X"));
	}
}
