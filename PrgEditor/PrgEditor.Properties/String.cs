using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace PrgEditor.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class String
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
				resourceMan = new ResourceManager("PrgEditor.Properties.String", typeof(String).Assembly);
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

	internal static string CaptionConfirm => ResourceManager.GetString("CaptionConfirm", resourceCulture);

	internal static string CaptionError => ResourceManager.GetString("CaptionError", resourceCulture);

	internal static string CaptionInfo => ResourceManager.GetString("CaptionInfo", resourceCulture);

	internal static string CaptionWarn => ResourceManager.GetString("CaptionWarn", resourceCulture);

	internal static string MsgAutoLoadChr => ResourceManager.GetString("MsgAutoLoadChr", resourceCulture);

	internal static string MsgNotFound => ResourceManager.GetString("MsgNotFound", resourceCulture);

	internal static string MsgReloadChr => ResourceManager.GetString("MsgReloadChr", resourceCulture);

	internal static string MsgReloadPrg => ResourceManager.GetString("MsgReloadPrg", resourceCulture);

	internal String()
	{
	}
}
