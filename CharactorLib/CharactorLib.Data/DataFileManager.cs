using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using CharactorLib.Common;

namespace CharactorLib.Data;

public class DataFileManager
{
	public static DataFileManager sDataFileManager = null;

	private static bool isGettingInstance = false;

	private RomDataFile mEditFileData = new RomDataFile();

	private DataFileSetting mSettingInfo = new DataFileSetting();

	private JumpListFile mJumpListFile = new JumpListFile();

	private static PalInfo mPalInfoNes = new PalInfo();

	private static PalInfo mPalInfo256 = new PalInfo();

	private static PalInfo mPalInfoBmp = new PalInfo();

	private static DatInfo mDatInfoNes = new DatInfo();

	private static DatInfo mDatInfo256 = new DatInfo();

	private AdfPattern mAdfPattern = new AdfPattern();

	private DataFileState mDataFileCustom = new DataFileState();

	private ColSetData mColSetData = new ColSetData();

	private static List<DataFileBase> mDataFileList = new List<DataFileBase>();

	public RomDataFile RomData => mEditFileData;

	public int RomHeaderSize { get; set; }

	public DataFileSetting SettingInfo => mSettingInfo;

	public JumpListFile JumpListFile => mJumpListFile;

	public PaletteMode PaletteMode { get; set; }

	public PalInfo PalInfoNes => mPalInfoNes;

	public PalInfo PalInfo256 => mPalInfo256;

	public PalInfo PalInfoBmp => mPalInfoBmp;

	[Browsable(false)]
	public PalInfo PalInfo => PaletteMode switch
	{
		PaletteMode.Dat => PalInfoNes, 
		PaletteMode.Bmp => PalInfoBmp, 
		_ => PalInfo256, 
	};

	public DatInfo DatInfoNes => mDatInfoNes;

	public DatInfo DatInfo256 => mDatInfo256;

	[Browsable(false)]
	public DatInfo DatInfo => PaletteMode switch
	{
		PaletteMode.Dat => DatInfoNes, 
		_ => DatInfo256, 
	};

	public AdfPattern AdfPattern => mAdfPattern;

	public DataFileState DataFileCustom => mDataFileCustom;

	public ColSetData ColSetData => mColSetData;

	public static DataFileManager GetInstance()
	{
		if (sDataFileManager == null)
		{
			if (isGettingInstance)
			{
				throw new InvalidOperationException("DataFileManager.GetInstance()で無限ループが発生。実装ミスです。");
			}
			isGettingInstance = true;
			sDataFileManager = new DataFileManager();
		}
		return sDataFileManager;
	}

	private DataFileManager()
	{
		mDatInfo256.CreateNew(256);
		for (int i = 0; i < mDatInfo256.Data.Length; i++)
		{
			mDatInfo256.Data[i] = (byte)((uint)i & 0xFFu);
		}
		mDatInfo256.Extension = "dat256";
		mDataFileList.Add(mEditFileData);
		mDataFileList.Add(mSettingInfo);
		mDataFileList.Add(mPalInfoNes);
		mDataFileList.Add(mPalInfo256);
		mDataFileList.Add(mDatInfoNes);
		mDataFileList.Add(mDatInfo256);
		mDataFileList.Add(mAdfPattern);
		mDataFileList.Add(mDataFileCustom);
		mDataFileList.Add(mJumpListFile);
		mDataFileList.Add(mColSetData);
		mPalInfoNes.Name += "(PAL+SET)";
		mPalInfo256.Name += "(256)";
		mPalInfoBmp.Name += "(BMP)";
		mDatInfoNes.Name += "(PAL+SET)";
		mDatInfo256.Name += "(256)";
		LoadPluginDataFile();
		foreach (DataFileBase mDataFile in mDataFileList)
		{
			mDataFile.DataFileManager = this;
		}
	}

	private void LoadPluginDataFile()
	{
		try
		{
			string path = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + "\\plugins";
			if (!Directory.Exists(path))
			{
				return;
			}
			string[] files = Directory.GetFiles(path, "*.dll");
			foreach (string path2 in files)
			{
				try
				{
					Type[] types = Assembly.LoadFile(path2).GetTypes();
					foreach (Type type in types)
					{
						if (type.IsSubclassOf(typeof(DataFileBase)))
						{
							DataFileBase item = (DataFileBase)type.InvokeMember(null, BindingFlags.CreateInstance, null, null, null);
							mDataFileList.Add(item);
						}
					}
				}
				catch (Exception ex)
				{
					_ = ex.Message;
				}
			}
		}
		catch
		{
		}
	}

	public DataFileBase[] GetDataFiles()
	{
		return mDataFileList.ToArray();
	}

	public string GetExtension()
	{
		char[] trimChars = new char[3] { ' ', '\t', ',' };
		StringBuilder stringBuilder = new StringBuilder();
		foreach (DataFileBase mDataFile in mDataFileList)
		{
			string extension = mDataFile.Extension;
			if (string.IsNullOrEmpty(extension))
			{
				continue;
			}
			extension = extension.Trim(trimChars);
			if (string.IsNullOrEmpty(extension))
			{
				continue;
			}
			extension = extension.Replace(" ", "");
			extension = extension.Replace(",", ";*.");
			extension = "*." + extension;
			if (!stringBuilder.ToString().Contains(extension))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(";");
				}
				stringBuilder.Append(extension);
			}
		}
		return stringBuilder.ToString();
	}

	public DataFileBase GetFormatByFilename(string filename)
	{
		char[] trimChars = new char[3] { ' ', '\t', '.' };
		string extension = Path.GetExtension(filename);
		extension = extension.Trim(trimChars);
		return GetFormatByExtension(extension);
	}

	public DataFileBase GetFormatByExtension(string targetExt)
	{
		DataFileBase result = null;
		targetExt = targetExt.ToLower();
		char[] trimChars = new char[3] { ' ', '\t', ',' };
		foreach (DataFileBase mDataFile in mDataFileList)
		{
			string extension = mDataFile.Extension;
			if (!string.IsNullOrEmpty(extension))
			{
				extension = extension.Trim(trimChars);
				extension = extension.ToLower();
				if (!string.IsNullOrEmpty(extension) && extension.Contains(targetExt))
				{
					return mDataFile;
				}
			}
		}
		return result;
	}

	public void UnboundsFiles()
	{
		for (int i = 0; i < mDataFileList.Count; i++)
		{
			DataFileBase dataFileBase = mDataFileList[i];
			if (dataFileBase != null && !string.IsNullOrEmpty(dataFileBase.FileName) && dataFileBase is JumpListFile)
			{
				string extension = dataFileBase.Extension;
				Utility.GetDataFilename("New", extension);
				((JumpListFile)dataFileBase).CreateNew(0);
			}
		}
	}

	public void AutoLoadFiles(string filename)
	{
		for (int i = 0; i < mDataFileList.Count; i++)
		{
			DataFileBase dataFileBase = mDataFileList[i];
			if (!dataFileBase.IsAutoLoad)
			{
				continue;
			}
			string extension = dataFileBase.Extension;
			string dataFilename = Utility.GetDataFilename(filename, extension);
			if (File.Exists(dataFilename))
			{
				if (dataFileBase.CheckAutoLoad(dataFilename))
				{
					dataFileBase.LoadFromFile(dataFilename);
				}
				continue;
			}
			dataFilename = Utility.GetDataFilenameOld(filename, extension);
			if (File.Exists(dataFilename))
			{
				if (dataFileBase.CheckAutoLoad(dataFilename))
				{
					dataFileBase.LoadFromFile(dataFilename);
				}
			}
			else if (dataFileBase.InitializeIfNotFound)
			{
				dataFileBase.Initialize(filename);
			}
		}
	}

	public void AutoSaveFiles(string filename)
	{
		foreach (DataFileBase mDataFile in mDataFileList)
		{
			if ((mDataFile is PalInfo && mDataFile != PalInfo) || (mDataFile is DatInfo && (mDataFile != DatInfoNes || mDataFile != DatInfo)) || !mDataFile.IsAutoSave)
			{
				continue;
			}
			string extension = mDataFile.Extension;
			string dataFilename = Utility.GetDataFilename(filename, extension);
			if (File.Exists(dataFilename))
			{
				mDataFile.SaveToFile(dataFilename);
				continue;
			}
			dataFilename = Utility.GetDataFilenameOld(filename, extension);
			if (File.Exists(dataFilename))
			{
				mDataFile.SaveToFile(dataFilename);
			}
		}
	}

	public void LoadDataFromBinary(string ext, byte[] data, string command)
	{
		string[] array = command.Split('(');
		if (array.Length != 2 || string.IsNullOrEmpty(array[0]) || string.IsNullOrEmpty(array[1]))
		{
			return;
		}
		string dataType = array[0].ToUpper();
		string[] array2 = array[1].Trim(')').Split(',');
		if (string.IsNullOrEmpty(array2[0]))
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		try
		{
			num = Convert.ToInt32(array2[0], 16);
		}
		catch
		{
			return;
		}
		if (array2.Length == 3)
		{
			try
			{
				num2 = Convert.ToInt32(array2[1], 16);
			}
			catch
			{
				return;
			}
			try
			{
				num3 = Convert.ToInt32(array2[2], 16);
			}
			catch
			{
				return;
			}
		}
		else
		{
			if (array2.Length != 1)
			{
				return;
			}
			num2 = GetDataDefaultSize(dataType);
			num3 = 0;
		}
		LoadDataFromFile(dataType, data, num, num2, num3);
	}

	public int GetDataDefaultSize(string dataType)
	{
		int result = 0;
		switch (dataType)
		{
		case "PAL":
		case "PAL24":
			result = 768;
			break;
		case "PAL16":
		case "PAL15":
			result = 512;
			break;
		case "PAL9":
			result = 512;
			break;
		case "DAT":
		case "PTBL":
			result = 32;
			break;
		case "ADF":
			result = 256;
			break;
		}
		return result;
	}

	public void LoadDataFromFile(string dataType, byte[] data, int addr, int size, int dest)
	{
		try
		{
			switch (dataType)
			{
			case "PAL":
				PalInfo.LoadFromMem(data, addr, dest, size);
				break;
			case "PAL32":
				PalInfo256.LoadPal32FromMem(data, addr, dest, size);
				break;
			case "PAL24":
				PalInfo256.LoadPal24FromMem(data, addr, dest, size);
				break;
			case "PAL16":
			case "PAL15":
				PalInfo256.LoadPal16FromMem(data, addr, dest, size);
				break;
			case "PAL9":
				PalInfo256.LoadPal9FromMem(data, addr, dest, size);
				break;
			case "DAT":
			case "PTBL":
				DatInfoNes.LoadFromMem(data, addr, dest, size);
				break;
			case "ADF":
			{
				AdfPattern.CreateNew(288);
				AdfPattern.AdfPatterns[0].Name = ".setting";
				for (int i = 0; i < size; i++)
				{
					AdfPattern.AdfPatterns[0].Pattern[dest + i] = data[addr + i];
				}
				AdfPattern.UpdateAdf();
				break;
			}
			}
		}
		catch
		{
		}
	}
}
