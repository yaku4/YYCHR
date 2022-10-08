using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace CharactorLib.Format;

public class FormatManager
{
	public static FormatManager sFormatManager = null;

	private static List<FormatBase> mFormatList = new List<FormatBase>();

	public static FormatManager GetInstance()
	{
		if (sFormatManager == null)
		{
			sFormatManager = new FormatManager();
		}
		return sFormatManager;
	}

	private FormatManager()
	{
		mFormatList.Add(new _1bpp8());
		mFormatList.Add(new _2bppNES());
		mFormatList.Add(new _2bppGB());
		mFormatList.Add(new _2bppNGP());
		mFormatList.Add(new _2bppVB());
		mFormatList.Add(new _2bppMSX());
		mFormatList.Add(new _3bppSNES());
		mFormatList.Add(new _3bpp8());
		mFormatList.Add(new _4bppSNES());
		mFormatList.Add(new _4bppGBA());
		mFormatList.Add(new _4bppSMS());
		mFormatList.Add(new _4bppMSX());
		mFormatList.Add(new _4bppPCE_CG2());
		mFormatList.Add(new _4bppPCE_SG());
		mFormatList.Add(new _8bppSNES_M7());
		mFormatList.Add(new _2bppMSX_S2());
		mFormatList.Add(new _1bpp16());
		mFormatList.Add(new _2bpp16());
		mFormatList.Add(new _8bppSNES());
		mFormatList.Add(new _1bpp12());
		mFormatList.Add(new _1bpp11());
		LoadPluginFormat();
	}

	private void LoadPluginFormat()
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
						if (type.IsSubclassOf(typeof(FormatBase)))
						{
							FormatBase item = (FormatBase)type.InvokeMember(null, BindingFlags.CreateInstance, null, null, null);
							mFormatList.Add(item);
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

	public FormatBase[] GetFormats()
	{
		return mFormatList.ToArray();
	}

	public string GetExtension()
	{
		char[] trimChars = new char[3] { ' ', '\t', ',' };
		StringBuilder stringBuilder = new StringBuilder();
		foreach (FormatBase mFormat in mFormatList)
		{
			string extension = mFormat.Extension;
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

	public FormatBase GetFormatByFilename(string filename)
	{
		char[] trimChars = new char[3] { ' ', '\t', '.' };
		string extension = Path.GetExtension(filename);
		extension = extension.Trim(trimChars);
		return GetFormatByExtension(extension);
	}

	public FormatBase GetFormatByExtension(string targetExt)
	{
		FormatBase result = null;
		targetExt = targetExt.ToLower();
		char[] trimChars = new char[3] { ' ', '\t', ',' };
		foreach (FormatBase mFormat in mFormatList)
		{
			string extension = mFormat.Extension;
			if (!string.IsNullOrEmpty(extension))
			{
				extension = extension.Trim(trimChars);
				extension = extension.ToLower();
				if (!string.IsNullOrEmpty(extension) && extension.Contains(targetExt))
				{
					return mFormat;
				}
			}
		}
		return result;
	}

	public FormatBase GetFormat(string name)
	{
		name = name.ToLower();
		foreach (FormatBase mFormat in mFormatList)
		{
			if (!string.IsNullOrEmpty(mFormat.Name) && name == mFormat.Name.ToLower())
			{
				return mFormat;
			}
		}
		return mFormatList[0];
	}
}
