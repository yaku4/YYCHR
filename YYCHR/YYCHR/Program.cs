using System;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace YYCHR;

internal static class Program
{
	[STAThread]
	private static void Main()
	{
		Settings instance = Settings.GetInstance();
		instance.Load();
		if (instance.CheckFont)
		{
			Font[] array = new Font[3]
			{
				SystemFonts.DialogFont,
				SystemFonts.MenuFont,
				SystemFonts.StatusFont
			};
			int num = 0;
			using (InstalledFontCollection installedFontCollection =
				new InstalledFontCollection())
			{
				FontFamily[] families = installedFontCollection.Families;
				Font[] array2 = array;
				foreach (Font font in array2)
				{
					FontFamily[] array3 = families;
					foreach (FontFamily fontFamily in array3)
					{
						if (font.Name == fontFamily.Name)
						{
							num++;
							break;
						}
					}
				}
			}
			if (num < array.Length)
				Thread.CurrentThread.CurrentUICulture =
					CultureInfo.GetCultureInfoByIetfLanguageTag("iv");
		}
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(defaultValue: false);
		using MainForm mainForm = new MainForm();
		Application.Run(mainForm);
	}
}
