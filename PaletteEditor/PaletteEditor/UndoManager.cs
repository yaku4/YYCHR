using System.Collections.Generic;

namespace PaletteEditor;

internal class UndoManager
{
	internal class UndoInfo
	{
		private int mAddress;

		private byte[] mUndoBuffer;

		private byte[] mRedoBuffer;

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

		public byte[] UndoBuffer => mUndoBuffer;

		public byte[] RedoBuffer => mRedoBuffer;

		public UndoInfo(byte[] data, int addr, byte[] newData)
		{
			mAddress = addr;
			int num = newData.Length;
			if (addr + num >= data.Length)
			{
				num = data.Length - addr;
			}
			mUndoBuffer = new byte[num];
			mRedoBuffer = new byte[num];
			for (int i = 0; i < num; i++)
			{
				int num2 = addr + i;
				if (num2 >= 0 && num2 < data.Length)
				{
					mUndoBuffer[i] = data[num2];
				}
				mRedoBuffer[i] = newData[i];
			}
		}
	}

	private List<UndoInfo> mUndoBuffer = new List<UndoInfo>();

	private int mIndex;

	public bool CanUndo
	{
		get
		{
			if (mUndoBuffer.Count >= 1)
			{
				return mIndex > 0;
			}
			return false;
		}
	}

	public bool CanRedo
	{
		get
		{
			if (mUndoBuffer.Count >= 1)
			{
				return mIndex < mUndoBuffer.Count;
			}
			return false;
		}
	}

	public void ClearUndoBuffer()
	{
		mUndoBuffer.Clear();
		mIndex = 0;
	}

	private void AddBuffer(byte[] editData, int editDataAddr, byte[] newData)
	{
		if (mUndoBuffer.Count >= 1 && mIndex >= 0)
		{
			for (int num = mUndoBuffer.Count - 1; num >= mIndex; num--)
			{
				mUndoBuffer.RemoveAt(num);
			}
		}
		UndoInfo item = new UndoInfo(editData, editDataAddr, newData);
		mUndoBuffer.Add(item);
		mIndex = mUndoBuffer.Count;
	}

	private UndoInfo GetBuffer(int index)
	{
		if (index >= 0 && index < mUndoBuffer.Count)
		{
			return mUndoBuffer[index];
		}
		return null;
	}

	private UndoInfo Undo()
	{
		int num = mIndex - 1;
		if (num >= 0 && num <= mUndoBuffer.Count)
		{
			mIndex = num;
		}
		return GetBuffer(mIndex);
	}

	private UndoInfo Redo()
	{
		int num = mIndex + 1;
		if (num >= 0 && num <= mUndoBuffer.Count)
		{
			mIndex = num;
		}
		return GetBuffer(mIndex - 1);
	}

	public void WriteNewDataAndCreateUndoBuffer(byte[] editData, int editDataAddr, byte[] writeData)
	{
		AddBuffer(editData, editDataAddr, writeData);
		WriteData(editData, editDataAddr, writeData);
	}

	public void WriteUndoData(byte[] editData)
	{
		UndoInfo undoInfo = Undo();
		if (undoInfo != null)
		{
			int address = undoInfo.Address;
			byte[] undoBuffer = undoInfo.UndoBuffer;
			WriteData(editData, address, undoBuffer);
		}
	}

	public void WriteRedoData(byte[] editData)
	{
		UndoInfo undoInfo = Redo();
		if (undoInfo != null)
		{
			int address = undoInfo.Address;
			byte[] redoBuffer = undoInfo.RedoBuffer;
			WriteData(editData, address, redoBuffer);
		}
	}

	private void WriteData(byte[] editData, int editDataAddr, byte[] writeData)
	{
		if (editData == null || writeData == null)
		{
			return;
		}
		for (int i = 0; i < writeData.Length; i++)
		{
			int num = editDataAddr + i;
			if (num >= 0 && num < editData.Length)
			{
				editData[num] = writeData[i];
			}
		}
	}
}
