using System;
using System.IO;
using System.Threading;

namespace CharactorLib.Common;

public static class Utility
{
	public const string EXT_PAL = "pal";

	public const string EXT_DAT = "dat";

	public const string EXT_ADF = "adf";

	public const string EXT_COL = "col";

	public const string EXT_NAM = "nam";

	public const string EXT_CHR = "chr";

	public const string EXT_PRG = "prg";

	public const string EXT_BMP = "bmp";

	public const string EXT_BMP2 = "chr.bmp";

	public static string DefaultPal => GetExeDataFilename("pal");

	public static string DefaultDat => GetExeDataFilename("dat");

	public static string DefaultAdf => GetExeDataFilename("adf");

	public static string DefaultBmp => GetExeDataFilename("bmp");

	public static string GetExeFilename()
	{
		return Environment.GetCommandLineArgs()[0].Replace(".vshost", "");
	}

	public static string GetExeDirectory()
	{
		return Path.GetDirectoryName(GetExeFilename());
	}

	public static string GetExeDataFilename(string ext)
	{
		return GetDataFilenameOld(GetExeFilename(), ext);
	}

	public static string GetDataFilenameOld(string filename, string ext)
	{
		if (ext == "adf")
		{
			string text = "";
			try
			{
				text = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
			}
			catch (Exception)
			{
			}
			string text2 = Path.ChangeExtension(filename, text + "." + ext);
			if (File.Exists(text2))
			{
				return text2;
			}
		}
		return Path.ChangeExtension(filename, ext);
	}

	public static string GetDataFilename(string filename, string ext)
	{
		if (ext == "adf")
		{
			string text = "";
			try
			{
				text = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
			}
			catch (Exception)
			{
			}
			string text2 = filename + "." + text + "." + ext;
			if (File.Exists(text2))
			{
				return text2;
			}
		}
		return filename + "." + ext;
	}

	public static string GetDataFilenameName(string filename, string ext)
	{
		return Path.GetFileName(GetDataFilename(filename, ext));
	}
}
