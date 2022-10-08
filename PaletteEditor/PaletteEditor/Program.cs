using System;
using System.Windows.Forms;

namespace PaletteEditor;

internal static class Program
{
	[STAThread]
	private static void Main()
	{
		Settings.GetInstance().Load();
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(defaultValue: false);
		Application.Run(new PaletteEditorMainForm());
	}
}
