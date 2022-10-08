using System.Collections.Generic;

namespace CharactorLib.Data;

public class RomDataFile : DataFileBase
{
	private class UndoInfo
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

	private int mIndex;

	public bool CanUndo => mIndex > 0;

	public bool CanRedo => mIndex < mUndoBuffer.Count - 1;

	public override void CreateNew(int size)
	{
		base.CreateNew(size);
		ClearUndoBuffer();
	}

	public override void LoadFromFile(string filename)
	{
		LoadFromFile(filename, clearUndoBuffer: true);
	}

	public void LoadFromFile(string filename, bool clearUndoBuffer)
	{
		base.LoadFromFile(filename);
		if (clearUndoBuffer)
		{
			ClearUndoBuffer();
		}
	}

	public override void SaveToFile(string filename)
	{
		base.SaveToFile(filename);
	}

	public override void SaveToFile(string filename, long fileSize)
	{
		base.SaveToFile(filename, fileSize);
	}

	public void SetMinLength(int minLength)
	{
		byte[] array = mData;
		mData = new byte[minLength];
		DataFileBase.CopyData(mData, 0, array, 0, array.Length);
	}

	public void ClearUndoBuffer()
	{
		mUndoBuffer.Clear();
		mIndex = 0;
	}

	public void CreateUndoBuffer(int addr, int size)
	{
		for (int num = mUndoBuffer.Count - 1; num >= mIndex; num--)
		{
			mUndoBuffer.RemoveAt(num);
		}
		UndoInfo item = new UndoInfo(base.Data, addr, size);
		mUndoBuffer.Add(item);
		mIndex = mUndoBuffer.Count;
	}

	public void Undo()
	{
		if (mIndex > 0)
		{
			int num = mIndex;
			if (num == mUndoBuffer.Count)
			{
				UndoInfo undoInfo = mUndoBuffer[mIndex - 1];
				int address = undoInfo.Address;
				int size = undoInfo.Buffer.Length;
				CreateUndoBuffer(address, size);
			}
			mIndex = num - 1;
			UndoInfo undoInfo2 = mUndoBuffer[mIndex];
			for (int i = 0; i < undoInfo2.Buffer.Length; i++)
			{
				int num2 = undoInfo2.Address + i;
				base.Data[num2] = undoInfo2.Buffer[i];
			}
		}
	}

	public void Redo()
	{
		if (mIndex < mUndoBuffer.Count)
		{
			mIndex++;
			UndoInfo undoInfo = mUndoBuffer[mIndex];
			for (int i = 0; i < undoInfo.Buffer.Length; i++)
			{
				int num = undoInfo.Address + i;
				base.Data[num] = undoInfo.Buffer[i];
			}
		}
	}
}
