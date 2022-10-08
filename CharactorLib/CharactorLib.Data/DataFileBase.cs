using System;
using System.IO;
using CharactorLib.Common;

namespace CharactorLib.Data;

public class DataFileBase
{
	protected byte[] mData;

	protected string mFileName = string.Empty;

	protected string mFileNameName = string.Empty;

	protected bool mModified;

	private long mFileSize;

	public DataFileManager DataFileManager { get; internal set; }

	public string Name { get; set; } = string.Empty;


	public string DataName { get; set; } = string.Empty;


	public string Extension { get; set; } = string.Empty;


	public string Author { get; set; } = string.Empty;


	public string Url { get; set; } = string.Empty;


	public bool IsAutoLoad { get; set; }

	public bool IsAutoSave { get; set; }

	public bool InitializeIfNotFound { get; set; }

	public int FixedByteSize { get; protected set; }

	public byte[] Data => mData;

	public string FileName
	{
		get
		{
			return mFileName;
		}
		set
		{
			mFileName = value;
			mFileNameName = Path.GetFileName(value);
		}
	}

	public string FileNameName => mFileNameName;

	public DateTime FileDate { get; protected set; } = DateTime.MinValue;


	public bool Modified
	{
		get
		{
			return mModified;
		}
		set
		{
			mModified = value;
			if (value)
			{
				OnDataModified();
			}
		}
	}

	public long FileSize
	{
		get
		{
			return mFileSize;
		}
		set
		{
			if (value < 0)
			{
				value = 0L;
			}
			mFileSize = value;
		}
	}

	public bool Exist { get; private set; }

	public bool ReadOnly { get; private set; }

	public event EventHandler DataLoaded;

	public event EventHandler DataModified;

	protected void OnDataLoaded()
	{
		Modified = false;
		if (this.DataLoaded != null)
		{
			this.DataLoaded(this, EventArgs.Empty);
		}
	}

	protected void OnDataModified()
	{
		if (this.DataModified != null)
		{
			this.DataModified(this, EventArgs.Empty);
		}
	}

	public string GetDataFileName(string filename)
	{
		return Utility.GetDataFilename(filename, Extension);
	}

	public virtual bool CheckExtSupported(string ext)
	{
		Extension.Contains(ext);
		return false;
	}

	protected static void CopyData(byte[] dstData, int dstAddress, byte[] srcData, int srcAddress, int size)
	{
		for (int i = 0; i < size; i++)
		{
			int num = dstAddress + i;
			if (num < dstData.Length)
			{
				int num2 = srcAddress + i;
				if (num2 < srcData.Length)
				{
					dstData[num] = srcData[num2];
				}
				else
				{
					dstData[num] = 0;
				}
			}
		}
	}

	protected virtual void ClearData()
	{
		for (int i = 0; i < mData.Length; i++)
		{
			mData[i] = 0;
		}
		Modified = true;
	}

	public virtual void Initialize(string filename)
	{
		mFileName = GetDataFileName(filename);
	}

	public virtual void CreateNew(int size)
	{
		mData = new byte[size];
		mFileName = "NewFile" + Extension;
		LoadFileDate();
		LoadFileInfo();
		ClearData();
		OnDataLoaded();
	}

	public virtual void LoadFromFile(string filename)
	{
		int fixedByteSize = FixedByteSize;
		if (fixedByteSize > 0 && File.Exists(filename))
		{
			if (mData != null && fixedByteSize > 0 && mData.Length != fixedByteSize)
			{
				mData = null;
			}
			if (mData == null)
			{
				CreateNew(fixedByteSize);
			}
			byte[] srcData = File.ReadAllBytes(filename);
			CopyData(mData, 0, srcData, 0, fixedByteSize);
		}
		else
		{
			mData = File.ReadAllBytes(filename);
		}
		mFileName = filename;
		LoadFileDate();
		LoadFileInfo();
		OnDataLoaded();
	}

	public virtual void LoadFromMem(byte[] mem, int memAddress, int dataAddress, int size)
	{
		if (mData == null)
		{
			mData = new byte[size];
		}
		CopyData(mData, dataAddress, mem, memAddress, size);
		mFileName = "NewFile" + Extension;
		FileDate = DateTime.MinValue;
		OnDataLoaded();
	}

	public virtual void Load()
	{
		if (!string.IsNullOrEmpty(FileName))
		{
			LoadFromFile(FileName);
		}
	}

	public virtual void SaveToFile(string filename)
	{
		int fixedByteSize = FixedByteSize;
		if (fixedByteSize > 0 && File.Exists(filename))
		{
			byte[] array = File.ReadAllBytes(filename);
			CopyData(array, 0, mData, 0, fixedByteSize);
			File.WriteAllBytes(filename, array);
		}
		else
		{
			File.WriteAllBytes(filename, mData);
		}
		mFileName = filename;
		LoadFileDate();
		LoadFileInfo();
		OnDataLoaded();
	}

	public virtual void SaveToFile(string filename, long fileSize)
	{
		if (fileSize > 0)
		{
			byte[] array = new byte[fileSize];
			long num = Math.Min(fileSize, Data.Length);
			int size = CharactorCommon.NormalizeValue(0, int.MaxValue, (int)num);
			CopyData(array, 0, mData, 0, size);
			File.WriteAllBytes(filename, array);
		}
		else
		{
			File.WriteAllBytes(filename, mData);
		}
		mFileName = filename;
		LoadFileDate();
		LoadFileInfo();
		OnDataLoaded();
	}

	public virtual void SaveToMem(byte[] mem, int memAddress, int dataAddress, int size)
	{
		CopyData(mem, memAddress, mData, dataAddress, size);
		OnDataLoaded();
	}

	public virtual void Save()
	{
		if (!string.IsNullOrEmpty(FileName))
		{
			SaveToFile(FileName);
		}
	}

	protected void LoadFileInfo()
	{
		try
		{
			if (File.Exists(FileName))
			{
				FileInfo fileInfo = new FileInfo(FileName);
				FileSize = fileInfo.Length;
				Exist = true;
				ReadOnly = fileInfo.IsReadOnly;
			}
			else
			{
				FileSize = Data.Length;
				Exist = false;
				ReadOnly = false;
			}
		}
		catch
		{
			FileSize = 0L;
		}
	}

	public void LoadFileDate()
	{
		try
		{
			if (File.Exists(FileName))
			{
				FileDate = File.GetLastWriteTime(FileName);
			}
			else
			{
				FileDate = DateTime.MinValue;
			}
		}
		catch
		{
			FileDate = DateTime.MinValue;
		}
	}

	public virtual bool CheckFileDateChanged()
	{
		bool result = false;
		if (!string.IsNullOrWhiteSpace(FileName))
		{
			DateTime dateTime;
			try
			{
				FileInfo fileInfo = new FileInfo(FileName);
				dateTime = fileInfo.LastWriteTime;
				if (!fileInfo.Exists)
				{
					dateTime = DateTime.MinValue;
				}
			}
			catch (Exception)
			{
				dateTime = DateTime.MinValue;
			}
			if (dateTime != FileDate)
			{
				result = true;
			}
		}
		return result;
	}

	public virtual bool CheckAutoLoad(string filename)
	{
		return true;
	}

	public override string ToString()
	{
		return Name.ToString();
	}
}
