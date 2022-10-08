using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace ControlLib.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Resources
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				resourceMan = new ResourceManager("ControlLib.Properties.Resources", typeof(Resources).Assembly);
			}
			return resourceMan;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	internal static Bitmap IconControl => (Bitmap)ResourceManager.GetObject("IconControl", resourceCulture);

	internal static Bitmap IconDocument => (Bitmap)ResourceManager.GetObject("IconDocument", resourceCulture);

	internal static Bitmap IconFind => (Bitmap)ResourceManager.GetObject("IconFind", resourceCulture);

	internal static Bitmap IconFindNext => (Bitmap)ResourceManager.GetObject("IconFindNext", resourceCulture);

	internal static Bitmap IconFindPrev => (Bitmap)ResourceManager.GetObject("IconFindPrev", resourceCulture);

	internal static Bitmap IconFolder => (Bitmap)ResourceManager.GetObject("IconFolder", resourceCulture);

	internal static Bitmap IconForm => (Bitmap)ResourceManager.GetObject("IconForm", resourceCulture);

	internal static Bitmap IconOptionSetting => (Bitmap)ResourceManager.GetObject("IconOptionSetting", resourceCulture);

	internal static string LabelReadOnly => ResourceManager.GetString("LabelReadOnly", resourceCulture);

	internal static string NaviEditL => ResourceManager.GetString("NaviEditL", resourceCulture);

	internal static string NaviEditR => ResourceManager.GetString("NaviEditR", resourceCulture);

	internal static string NaviEditStampL1 => ResourceManager.GetString("NaviEditStampL1", resourceCulture);

	internal static string NaviEditStampL2 => ResourceManager.GetString("NaviEditStampL2", resourceCulture);

	internal static Bitmap PenIconDrawEllipse => (Bitmap)ResourceManager.GetObject("PenIconDrawEllipse", resourceCulture);

	internal static Bitmap PenIconDrawRect => (Bitmap)ResourceManager.GetObject("PenIconDrawRect", resourceCulture);

	internal static Bitmap PenIconFillEllipse => (Bitmap)ResourceManager.GetObject("PenIconFillEllipse", resourceCulture);

	internal static Bitmap PenIconFillRect => (Bitmap)ResourceManager.GetObject("PenIconFillRect", resourceCulture);

	internal static Bitmap PenIconLine => (Bitmap)ResourceManager.GetObject("PenIconLine", resourceCulture);

	internal static Bitmap PenIconPaint => (Bitmap)ResourceManager.GetObject("PenIconPaint", resourceCulture);

	internal static Bitmap PenIconPatternPen => (Bitmap)ResourceManager.GetObject("PenIconPatternPen", resourceCulture);

	internal static Bitmap PenIconPatternRect => (Bitmap)ResourceManager.GetObject("PenIconPatternRect", resourceCulture);

	internal static Bitmap PenIconPen => (Bitmap)ResourceManager.GetObject("PenIconPen", resourceCulture);

	internal static Bitmap PenIconSetPalette => (Bitmap)ResourceManager.GetObject("PenIconSetPalette", resourceCulture);

	internal static Bitmap PenIconStamp => (Bitmap)ResourceManager.GetObject("PenIconStamp", resourceCulture);

	internal static string PenNameDrawEllipse => ResourceManager.GetString("PenNameDrawEllipse", resourceCulture);

	internal static string PenNameDrawRect => ResourceManager.GetString("PenNameDrawRect", resourceCulture);

	internal static string PenNameFillEllipse => ResourceManager.GetString("PenNameFillEllipse", resourceCulture);

	internal static string PenNameFillRect => ResourceManager.GetString("PenNameFillRect", resourceCulture);

	internal static string PenNameLine => ResourceManager.GetString("PenNameLine", resourceCulture);

	internal static string PenNamePaint => ResourceManager.GetString("PenNamePaint", resourceCulture);

	internal static string PenNamePatternEllipse => ResourceManager.GetString("PenNamePatternEllipse", resourceCulture);

	internal static string PenNamePatternLine => ResourceManager.GetString("PenNamePatternLine", resourceCulture);

	internal static string PenNamePatternPen => ResourceManager.GetString("PenNamePatternPen", resourceCulture);

	internal static string PenNamePatternRect => ResourceManager.GetString("PenNamePatternRect", resourceCulture);

	internal static string PenNamePen => ResourceManager.GetString("PenNamePen", resourceCulture);

	internal static string PenNameSetPalette => ResourceManager.GetString("PenNameSetPalette", resourceCulture);

	internal static string PenNameStamp => ResourceManager.GetString("PenNameStamp", resourceCulture);

	internal Resources()
	{
	}
}
