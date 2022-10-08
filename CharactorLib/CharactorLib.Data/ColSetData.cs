namespace CharactorLib.Data;

public class ColSetData : DataFileBase
{
	private const int COL_HEAD_SIZE = 256;

	private const int COL_PATTERN_BANK_SIZE = 256;

	public bool IsColMode => mData != null;

	public ColSetData()
	{
		base.DataName = "Col SetData File";
		base.Extension = "col";
		base.IsAutoLoad = true;
		base.IsAutoSave = true;
		base.FixedByteSize = 0;
		base.Author = "-";
		base.Url = "-";
		base.Name = GetType().Name;
		base.InitializeIfNotFound = true;
	}

	public override void Initialize(string filename)
	{
		base.Initialize(filename);
		mFileName = string.Empty;
		mData = null;
	}

	public override void LoadFromFile(string filename)
	{
		base.LoadFromFile(filename);
		mFileName = filename;
		base.DataFileManager.DatInfoNes.LoadFromMem(mData, 128, 0, 128);
	}

	public override void SaveToFile(string filename)
	{
		base.SaveToFile(filename);
	}

	public int GetBankPaletteSetAddr(int romAddr)
	{
		return romAddr / 16 + 256;
	}

	public byte[] GetBankPaletteSet(int romAddr, byte[] adf)
	{
		int bankPaletteSetAddr = GetBankPaletteSetAddr(romAddr);
		byte[] array = new byte[256];
		for (int i = 0; i < 256; i++)
		{
			int num = bankPaletteSetAddr + adf[i];
			byte b = (array[i] = (byte)((num < mData.Length) ? mData[num] : 0));
		}
		return array;
	}
}
