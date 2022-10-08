using System.Drawing;

namespace CharactorLib.Common;

public class PatternManager
{
	private Size mSize = new Size(1, 1);

	private RectCopyInfo[] mInfos;

	public Size Size
	{
		get
		{
			return mSize;
		}
		set
		{
			int num = value.Width;
			int num2 = value.Height;
			if (num < 1)
			{
				num = 1;
			}
			if (num2 < 1)
			{
				num2 = 1;
			}
			UpdateSize(new Size(num, num2));
		}
	}

	public int W
	{
		get
		{
			return Size.Width;
		}
		set
		{
			Size = new Size(value, H);
		}
	}

	public int H
	{
		get
		{
			return Size.Height;
		}
		set
		{
			Size = new Size(W, value);
		}
	}

	public RectCopyInfo[] Infos
	{
		get
		{
			if (mInfos == null)
			{
				mInfos = CreateInfo(mInfos, Size, Size);
			}
			return mInfos;
		}
	}

	private void UpdateSize(Size newSize)
	{
		Size size = Size;
		mSize = newSize;
		mInfos = CreateInfo(mInfos, size, newSize);
	}

	public PatternManager()
	{
		UpdateSize(new Size(32, 30));
	}

	private RectCopyInfo[] CreateInfo(RectCopyInfo[] orgData, Size orgSize, Size newSize)
	{
		RectCopyInfo[] array = new RectCopyInfo[W * H];
		for (int i = 0; i < W * H; i++)
		{
			if (array[i] == null)
			{
				array[i] = new RectCopyInfo();
			}
		}
		return array;
	}

	public void CreateInfoFromByteData(byte[] prgData, int topAddress, Size chrSize, bool x8y16, bool x16y16, bool swapXY, int skipByte)
	{
		int width = chrSize.Width;
		int height = chrSize.Height;
		for (int i = 0; i < H; i++)
		{
			for (int j = 0; j < W; j++)
			{
				RectCopyInfo rectCopyInfo = new RectCopyInfo();
				int dataAddr = GetDataAddr(j, i, W, H, x8y16, x16y16, swapXY);
				int num = topAddress + dataAddr * (skipByte + 1);
				if (num >= 0 && num < prgData.Length)
				{
					byte num2 = prgData[num];
					rectCopyInfo.Enabled = true;
					int x = (int)num2 % 16 * width;
					int y = (int)num2 / 16 * height;
					int x2 = j * width;
					int y2 = i * height;
					rectCopyInfo.Address = num;
					rectCopyInfo.Size = new Size(width, height);
					rectCopyInfo.SrcPoint = new Point(x, y);
					rectCopyInfo.DestPoint = new Point(x2, y2);
				}
				else
				{
					rectCopyInfo.Enabled = false;
				}
				int num3 = i * W + j;
				if (num3 >= 0 && num3 < Infos.Length)
				{
					Infos[num3].CopyFrom(rectCopyInfo);
				}
			}
		}
	}

	private int GetDataAddr(int x, int y, int w, int h, bool x8y16, bool x16y16, bool swapXY)
	{
		if (swapXY)
		{
			int num = y;
			y = x;
			x = num;
			int num2 = h;
			h = w;
			w = num2;
		}
		int num3 = 0;
		if (x8y16)
		{
			int num4 = y % 2;
			int num5 = x * 2;
			return y / 2 * (w * 2) + num5 + num4;
		}
		if (x16y16)
		{
			int num6 = x % 2;
			int num7 = y % 2 * 2;
			int num8 = x / 2 * 4;
			return y / 2 * (w * 2) + num8 + num7 + num6;
		}
		return y * w + x;
	}
}
