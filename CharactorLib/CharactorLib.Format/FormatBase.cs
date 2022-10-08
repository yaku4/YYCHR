using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CharactorLib.Common;
using CharactorLib.Data;

namespace CharactorLib.Format;

public class FormatBase
{
	private RotateType mRotateType;

	private MirrorType mMirrorType;

	private AdfInfo mAdfPattern;

	protected Form mForm;

	protected static byte[] _DmyData = new byte[65536];

	public string Name { get; protected set; }

	public string Extension { get; protected set; }

	public string Author { get; protected set; }

	public string Url { get; protected set; }

	public string FormatText { get; protected set; }

	public bool IsCompressed { get; protected set; }

	public bool Readonly { get; protected set; }

	public int ColorBit { get; protected set; }

	public int ColorNum { get; set; }

	public int Width { get; set; }

	public int Height { get; set; }

	public int CharWidth { get; protected set; }

	public int CharHeight { get; protected set; }

	public Size CharSize => new Size(CharWidth, CharHeight);

	public int ColumnCount
	{
		get
		{
			if (CharWidth > 0)
			{
				return Width / CharWidth;
			}
			return 0;
		}
	}

	public int RowsCount
	{
		get
		{
			if (CharHeight > 0)
			{
				return Height / CharHeight;
			}
			return 0;
		}
	}

	public bool IsSupportMirror { get; protected set; }

	public bool IsSupportRotate { get; protected set; }

	public RotateType RotateType
	{
		get
		{
			return mRotateType;
		}
		set
		{
			mRotateType = value;
		}
	}

	public MirrorType MirrorType
	{
		get
		{
			return mMirrorType;
		}
		set
		{
			mMirrorType = value;
		}
	}

	public bool EnableAdf { get; protected set; }

	[Browsable(false)]
	public AdfInfo AdfPattern
	{
		get
		{
			return mAdfPattern;
		}
		set
		{
			mAdfPattern = value;
		}
	}

	public override string ToString()
	{
		string name = Name;
		if (string.IsNullOrEmpty(name))
		{
			string fullName = GetType().FullName;
			try
			{
				return fullName.Split('.')[0];
			}
			catch
			{
				return fullName;
			}
		}
		return name;
	}

	public virtual string GetFormatInfo()
	{
		return "Name\t\t: " + Name + "\nExtension\t\t: " + Extension + "\n\nColorBit\t\t: " + ColorBit + "\nColorNum\t\t: " + ColorNum + "\nWidth\t\t: " + Width + "\nHeight\t\t: " + Height + "\nCharWidth\t\t: " + CharWidth + "\nCharHeight\t: " + CharHeight + "\n\nReadonly\t\t: " + Readonly + "\nIsSupportMirror\t: " + IsSupportMirror + "\nIsSupportRotate\t: " + IsSupportRotate + "\nIsCompressed\t: " + IsCompressed + "\nEnableAdf\t: " + EnableAdf + "\n\nPlugin Author\t: " + Author.ToString() + "\nPlugin URL\t: " + Url.ToString();
	}

	public int GetCharactorByteSize()
	{
		return CharWidth * CharHeight * ColorBit / 8;
	}

	public int GetBankByteSize()
	{
		return Width * Height * ColorBit / 8;
	}

	public virtual void LoadFormat(Form form)
	{
		mForm = form;
	}

	public virtual void UnloadFormat()
	{
		mForm = null;
	}

	public virtual void ConvertAllMemToChr(byte[] data, int addr, Bytemap bytemap, AdfInfo adfInfo)
	{
		bool flag = adfInfo?.IsDisableFF ?? false;
		byte[] array = adfInfo?.Pattern;
		bool flag2 = false;
		int bankByteSize = GetBankByteSize();
		if (addr + bankByteSize > data.Length)
		{
			return;
		}
		int charactorByteSize = GetCharactorByteSize();
		for (int i = 0; i < RowsCount; i++)
		{
			int py = i * CharHeight;
			for (int j = 0; j < ColumnCount; j++)
			{
				byte b = (byte)(i * ColumnCount + j);
				if (array != null)
				{
					b = array[b];
					flag2 = flag && b == byte.MaxValue;
				}
				if (!flag2)
				{
					int addr2 = b * charactorByteSize + addr;
					int px = j * CharWidth;
					ConvertMemToChr(data, addr2, bytemap, px, py);
				}
				else
				{
					int px2 = j * CharWidth;
					ConvertMemToChr(_DmyData, 0, bytemap, px2, py);
				}
			}
		}
	}

	public virtual void ConvertAllMemToChr(byte[] data, int addr, Bytemap bytemap)
	{
		if (EnableAdf && AdfPattern != null)
		{
			ConvertAllMemToChr(data, addr, bytemap, AdfPattern);
		}
		else
		{
			ConvertAllMemToChr(data, addr, bytemap, null);
		}
	}

	public virtual void ConvertAllChrToMem(byte[] data, int addr, Bytemap bytemap, AdfInfo adfInfo)
	{
		bool flag = adfInfo?.IsDisableFF ?? false;
		byte[] array = adfInfo?.Pattern;
		bool flag2 = false;
		int bankByteSize = GetBankByteSize();
		if (addr + bankByteSize > data.Length)
		{
			return;
		}
		int charactorByteSize = GetCharactorByteSize();
		for (int i = 0; i < RowsCount; i++)
		{
			int py = i * CharHeight;
			for (int j = 0; j < ColumnCount; j++)
			{
				byte b = (byte)(i * ColumnCount + j);
				if (array != null)
				{
					b = array[b];
					flag2 = flag && b == byte.MaxValue;
				}
				if (!flag2)
				{
					int addr2 = b * charactorByteSize + addr;
					int px = j * CharWidth;
					ConvertChrToMem(data, addr2, bytemap, px, py);
				}
			}
		}
	}

	public virtual void ConvertAllChrToMem(byte[] data, int addr, Bytemap bytemap)
	{
		if (EnableAdf && AdfPattern != null)
		{
			ConvertAllChrToMem(data, addr, bytemap, AdfPattern);
		}
		else
		{
			ConvertAllChrToMem(data, addr, bytemap, null);
		}
	}

	public virtual void ConvertMemToChr(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
	}

	public virtual void ConvertMemToChr(byte[] data, int addr, Bytemap bytemap, Point pt)
	{
		ConvertMemToChr(data, addr, bytemap, pt.X, pt.Y);
	}

	public virtual void ConvertChrToMem(byte[] data, int addr, Bytemap bytemap, int px, int py)
	{
	}

	public virtual void ConvertChrToMem(byte[] data, int addr, Bytemap bytemap, Point pt)
	{
		ConvertChrToMem(data, addr, bytemap, pt.X, pt.Y);
	}

	protected virtual Point GetAdvancePixelPoint(int x, int y)
	{
		return GetAdvancePixelPoint(x, y, CharWidth, CharHeight, RotateType, MirrorType);
	}

	protected static Point GetAdvancePixelPoint(int ox, int oy, int w, int h, RotateType rotate, MirrorType mirror)
	{
		int num = ox / w * w;
		int num2 = oy / h * h;
		int num3 = ox % w;
		int num4 = oy % h;
		if (mirror == MirrorType.Horizontal || mirror == MirrorType.Both)
		{
			num3 = w - 1 - num3;
		}
		if (mirror == MirrorType.Vertical || mirror == MirrorType.Both)
		{
			num4 = h - 1 - num4;
		}
		if (rotate == RotateType.Left)
		{
			int num5 = num4;
			num4 = w - 1 - num3;
			num3 = num5;
		}
		if (rotate == RotateType.Right)
		{
			int num6 = h - 1 - num4;
			num4 = num3;
			num3 = num6;
		}
		if (rotate == RotateType.Turn)
		{
			num4 = h - 1 - num4;
			num3 = w - 1 - num3;
		}
		return new Point(num + num3, num2 + num4);
	}

	public virtual int GetDataAddress(byte[] mData)
	{
		return 0;
	}

	public virtual int GetAddress(int addr, int size, AddressChange addressChange)
	{
		int num = addr;
		int charactorByteSize = GetCharactorByteSize();
		int num2 = charactorByteSize * ColumnCount;
		int bankByteSize = GetBankByteSize();
		switch (addressChange)
		{
		case AddressChange.Begin:
			num = 0;
			break;
		case AddressChange.BlockM100:
			num = addr - bankByteSize;
			break;
		case AddressChange.BlockM10:
			num = addr - num2;
			break;
		case AddressChange.BlockM1:
			num = addr - charactorByteSize;
			break;
		case AddressChange.ByteM1:
			num = addr - 1;
			break;
		case AddressChange.ByteP1:
			num = addr + 1;
			break;
		case AddressChange.BlockP1:
			num = addr + charactorByteSize;
			break;
		case AddressChange.BlockP10:
			num = addr + num2;
			break;
		case AddressChange.BlockP100:
			num = addr + bankByteSize;
			break;
		case AddressChange.End:
			num = size - bankByteSize;
			break;
		}
		if (num < 0)
		{
			num = 0;
		}
		if (num > size - bankByteSize)
		{
			num = size - bankByteSize;
		}
		return num;
	}
}
