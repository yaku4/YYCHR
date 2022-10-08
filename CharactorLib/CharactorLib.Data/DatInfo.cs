using System.Drawing;
using System.IO;

namespace CharactorLib.Data;

public class DatInfo : DataFileBase
{
	private const int DAT_SIZE = 256;

	private PalInfo mPalInfo;

	public PalInfo PalInfo
	{
		get
		{
			return mPalInfo;
		}
		set
		{
			mPalInfo = value;
		}
	}

	public Color[] Colors
	{
		get
		{
			Color[] array = new Color[256];
			if (mPalInfo != null)
			{
				Color[] colors = mPalInfo.Colors;
				for (int i = 0; i < 256; i++)
				{
					if (i < base.Data.Length)
					{
						byte b = base.Data[i];
						array[i] = colors[b];
					}
					else
					{
						array[i] = Color.Black;
					}
				}
			}
			return array;
		}
	}

	public DatInfo()
	{
		CreateNew(256);
		base.DataName = "Palette Table";
		base.Extension = "dat";
		base.IsAutoLoad = true;
		base.IsAutoSave = true;
		base.FixedByteSize = 256;
		base.Author = "-";
		base.Url = "-";
		base.Name = GetType().Name;
	}

	public override bool CheckAutoLoad(string filename)
	{
		bool result = true;
		if (new FileInfo(filename).Length >= 512)
		{
			result = false;
		}
		return result;
	}
}
