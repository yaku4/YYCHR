using System;

namespace CharactorLib.Common;

public class NesHeader
{
	public enum Flags
	{
		MirrorVertical = 1,
		Sram = 2,
		Trainer = 4,
		FourScreen = 8
	}

	public enum Mirroring
	{
		Horizontal,
		Vertical
	}

	private byte[] mData;

	public byte[] Data => mData;

	public bool IsNesHeader
	{
		get
		{
			bool result = false;
			if (Data != null && Data[0] == 78 && Data[1] == 69 && Data[2] == 83 && Data[3] == 26)
			{
				result = true;
			}
			return result;
		}
	}

	public byte PrgRom
	{
		get
		{
			return Data[4];
		}
		set
		{
			Data[4] = value;
		}
	}

	public byte ChrRom
	{
		get
		{
			return Data[5];
		}
		set
		{
			Data[5] = value;
		}
	}

	public byte Mapper
	{
		get
		{
			int num = (Data[6] & 0xF0) >> 4;
			return (byte)((Data[7] & 0xF0u) | (uint)num);
		}
		set
		{
			byte b = (byte)(value & 0xFu);
			byte b2 = (byte)(value & 0xF0u);
			byte b3 = (byte)(Data[6] & 0xFu);
			Data[6] = (byte)((b << 4) | b3);
			Data[7] = b2;
		}
	}

	public Mirroring Mirror
	{
		get
		{
			if ((Data[6] & 1) == 1)
			{
				return Mirroring.Horizontal;
			}
			return Mirroring.Vertical;
		}
		set
		{
			Data[6] = (byte)(Data[6] | 1u);
		}
	}

	public bool IsSRam
	{
		get
		{
			return (Data[6] & 2) == 2;
		}
		set
		{
			Data[6] = (byte)(Data[6] | 2u);
		}
	}

	public bool IsTrainer
	{
		get
		{
			return (Data[6] & 4) == 4;
		}
		set
		{
			Data[6] = (byte)(Data[6] | 4u);
		}
	}

	public bool Is4Screen
	{
		get
		{
			return (Data[6] & 8) == 8;
		}
		set
		{
			Data[6] = (byte)(Data[6] | 8u);
		}
	}

	public NesHeader(byte[] data)
	{
		mData = data;
		if (mData == null)
		{
			throw new ArgumentException("null data input.");
		}
	}
}
