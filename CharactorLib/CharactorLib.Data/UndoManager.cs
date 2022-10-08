using System.Collections.Generic;

namespace CharactorLib.Data;

public class UndoManager
{
	public class UndoInfo
	{
		private int mAddress;

		private byte[] mBuffer;

		public int Address
		{
			get
			{
				return mAddress;
			}
			set
			{
				mAddress = value;
			}
		}

		public byte[] Buffer => mBuffer;

		public UndoInfo(byte[] data)
			: this(data, 0, data.Length)
		{
		}

		public UndoInfo(byte[] data, int addr, int size)
		{
			mAddress = addr;
			mBuffer = new byte[size];
			for (int i = 0; i < size && i < data.Length; i++)
			{
				mBuffer[i] = data[addr + i];
			}
		}
	}

	private List<UndoInfo> mUndoBuffer = new List<UndoInfo>();

	private int mIndex = -1;

	public bool CanUndo
	{
		get
		{
			if (mIndex > 0)
			{
				return mUndoBuffer.Count > 1;
			}
			return false;
		}
	}

	public bool CanRedo => mIndex < mUndoBuffer.Count - 1;

	public void ClearUndoBuffer()
	{
		mUndoBuffer.Clear();
		mIndex = -1;
	}

	public void CreateUndoBuffer(byte[] data)
	{
		if (mUndoBuffer.Count >= 1 && mIndex >= 0)
		{
			for (int num = mUndoBuffer.Count - 1; num > mIndex; num--)
			{
				mUndoBuffer.RemoveAt(num);
			}
		}
		UndoInfo item = new UndoInfo(data);
		mUndoBuffer.Add(item);
		mIndex = mUndoBuffer.Count - 1;
	}

	public UndoInfo Undo()
	{
		int num = mIndex - 1;
		if (num >= 0 && num < mUndoBuffer.Count)
		{
			mIndex = num;
			return mUndoBuffer[mIndex];
		}
		return null;
	}

	public UndoInfo Redo()
	{
		int num = mIndex + 1;
		if (num >= 0 && num < mUndoBuffer.Count)
		{
			mIndex = num;
			return mUndoBuffer[mIndex];
		}
		return null;
	}
}
