using System.Collections.Generic;
using System.IO;

namespace CharactorLib.Data;

public class AdfPattern : DataFileBase
{
	private AdfInfo[] mAdfPatterns;

	public AdfInfo[] AdfPatterns => mAdfPatterns;

	public AdfPattern()
	{
		CreateNew(288);
		base.DataName = "ADF Pattern";
		base.Extension = "adf";
		base.IsAutoLoad = true;
		base.IsAutoSave = false;
		base.FixedByteSize = 0;
		base.Author = "-";
		base.Url = "-";
		base.Name = GetType().Name;
	}

	public override void CreateNew(int size)
	{
		mData = new byte[size];
		mFileName = "NewFile" + base.Extension;
		LoadFileDate();
		ClearData();
		LoadAdfInfos();
		OnDataLoaded();
	}

	public override void LoadFromFile(string filename)
	{
		mData = File.ReadAllBytes(filename);
		mFileName = filename;
		LoadAdfInfos();
		OnDataLoaded();
	}

	public override void LoadFromMem(byte[] mem, int memAddress, int dataAddress, int size)
	{
		for (int i = 0; i < size; i++)
		{
			int num = memAddress + i;
			mData[i] = mem[num];
		}
		mFileName = "NewFile" + base.Extension;
		LoadAdfInfos();
		OnDataLoaded();
	}

	private void LoadAdfInfos()
	{
		int count = GetCount();
		mAdfPatterns = new AdfInfo[count];
		for (int i = 0; i < count; i++)
		{
			mAdfPatterns[i] = new AdfInfo(base.Data, i * 288);
		}
	}

	public override void SaveToFile(string filename)
	{
		SaveAdfInfos();
		base.SaveToFile(filename);
	}

	public override void SaveToMem(byte[] mem, int memAddress, int dataAddress, int size)
	{
		SaveAdfInfos();
		base.SaveToMem(mem, memAddress, dataAddress, size);
	}

	private void SaveAdfInfos()
	{
		int num = mAdfPatterns.Length;
		int num2 = num * 288;
		mData = new byte[num2];
		for (int i = 0; i < num; i++)
		{
			int num3 = i * 288;
			byte[] adfBytes = mAdfPatterns[i].GetAdfBytes();
			for (int j = 0; j < 288; j++)
			{
				int num4 = num3 + j;
				base.Data[num4] = adfBytes[j];
			}
		}
	}

	public int GetCount()
	{
		return base.Data.Length / 288;
	}

	public string[] GetNames()
	{
		List<string> list = new List<string>();
		AdfInfo[] adfPatterns = AdfPatterns;
		foreach (AdfInfo adfInfo in adfPatterns)
		{
			list.Add(adfInfo.Name);
		}
		return list.ToArray();
	}

	public void UpdateAdf()
	{
		OnDataLoaded();
	}

	public void Add(int index, AdfInfo adfInfo)
	{
		List<AdfInfo> list = new List<AdfInfo>();
		list.AddRange(mAdfPatterns);
		list.Insert(index, adfInfo);
		mAdfPatterns = list.ToArray();
	}

	public void Add(int index)
	{
		byte[] data = new byte[288];
		Add(index, new AdfInfo(data, 0));
	}

	public void Add()
	{
		AdfInfo item = new AdfInfo(new byte[288], 0);
		List<AdfInfo> list = new List<AdfInfo>();
		list.AddRange(mAdfPatterns);
		list.Add(item);
		mAdfPatterns = list.ToArray();
	}

	public void Remove(int index)
	{
		List<AdfInfo> list = new List<AdfInfo>();
		list.AddRange(mAdfPatterns);
		list.RemoveAt(index);
		mAdfPatterns = list.ToArray();
	}

	public void Move(int fromIndex, int toIndex)
	{
		List<AdfInfo> list = new List<AdfInfo>();
		list.AddRange(mAdfPatterns);
		int index;
		int index2;
		if (fromIndex < toIndex)
		{
			index = fromIndex;
			index2 = toIndex;
		}
		else
		{
			index = toIndex;
			index2 = fromIndex;
		}
		AdfInfo item = list[index2];
		list.RemoveAt(index2);
		list.Insert(index, item);
		mAdfPatterns = list.ToArray();
	}
}
