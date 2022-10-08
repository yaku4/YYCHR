using System.Text;

namespace CharactorLib.Data;

public class AdfInfo
{
	public const int PATTERN_SIZE = 288;

	private string mName = string.Empty;

	private byte[] mPattern = new byte[256];

	public string Name
	{
		get
		{
			return mName;
		}
		set
		{
			mName = value;
		}
	}

	public bool IsDisableFF { get; set; }

	public byte[] Pattern => mPattern;

	public AdfInfo(byte[] data, int address)
	{
		bool isDisableFF = false;
		byte[] array = new byte[32];
		for (int i = 0; i < 32; i++)
		{
			int num = address + i;
			if (num < data.Length)
			{
				if (num == address + 31 && data[num] == byte.MaxValue)
				{
					isDisableFF = true;
				}
				else
				{
					array[i] = data[num];
				}
			}
		}
		mName = Encoding.GetEncoding("Shift_JIS").GetString(array);
		IsDisableFF = isDisableFF;
		for (int j = 0; j < 256; j++)
		{
			int num2 = address + 32 + j;
			if (num2 < data.Length)
			{
				mPattern[j] = data[num2];
			}
		}
	}

	public byte[] GetAdfBytes()
	{
		byte[] array = new byte[288];
		byte[] bytes = Encoding.GetEncoding("Shift_JIS").GetBytes(mName);
		for (int i = 0; i < 32; i++)
		{
			if (i < bytes.Length)
			{
				array[i] = bytes[i];
			}
		}
		if (IsDisableFF)
		{
			array[31] = byte.MaxValue;
		}
		for (int j = 0; j < 256; j++)
		{
			array[32 + j] = mPattern[j];
		}
		return array;
	}

	public override string ToString()
	{
		return Name;
	}
}
