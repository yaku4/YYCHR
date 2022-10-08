using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace CharactorLib.Common;

public static class ResourceUtility
{
	private static string mLngFile = "";

	public static int ResourceReadCount { get; private set; } = 0;


	public static bool LoadFromLngFile { get; set; } = false;


	private static List<ResourceKeyValue> ResourceKeyValueList { get; set; } = new List<ResourceKeyValue>();


	public static void CreateNewLngList()
	{
		ResourceKeyValueList.Clear();
	}

	public static bool LoadLngFile(string lngFilename)
	{
		if (File.Exists(lngFilename))
		{
			ResourceKeyValueList.Clear();
			string[] array = File.ReadAllLines(lngFilename);
			for (int i = 0; i < array.Length; i++)
			{
				ResourceKeyValue resourceKeyValue = ResourceKeyValue.FromLngLine(array[i]);
				if (resourceKeyValue != null)
				{
					ResourceKeyValueList.Add(resourceKeyValue);
				}
			}
			mLngFile = lngFilename;
			return true;
		}
		mLngFile = "";
		return false;
	}

	public static void SaveLngFile(string lngFilename)
	{
		List<string> list = new List<string>();
		foreach (ResourceKeyValue resourceKeyValue in ResourceKeyValueList)
		{
			string item = resourceKeyValue.ToString();
			_ = resourceKeyValue.Source;
			string key = resourceKeyValue.Key;
			_ = resourceKeyValue.Value;
			if (!key.StartsWith(">>"))
			{
				list.Add(item);
			}
		}
		list.Sort();
		List<string> list2 = new List<string>();
		list2.Add("; YY-CHR.net language file");
		list2.Add(";   Format:");
		list2.Add(";     source@@key=value");
		list2.Add(";       modify value for localized text");
		list2.Add(";       line start with ';' or '#' = comment line");
		list2.Add(";       cannot use '@', use '[at]'");
		list2.Add(";       cannot use '=', use '[equal]'");
		list2.Add("; ");
		list2.Add("; LNG setting");
		list2.Add(";   LngLanguage=Lng Sample");
		list2.Add(";   LngFileAuther=Translator name");
		list2.Add(";   AllowedAttachToYyChr=true");
		list2.Add("; ");
		list2.Add("; * LoadResourceText function is not implemented yet. ");
		list2.Add("; ");
		list.InsertRange(0, list2);
		list.Add("");
		string contents = string.Join("\r\n", list);
		File.WriteAllText(lngFilename, contents);
		mLngFile = lngFilename;
	}

	public static string GetValue(string source, string key)
	{
		string result = null;
		ResourceKeyValue resourceInfo = GetResourceInfo(source, key);
		if (resourceInfo != null)
		{
			result = resourceInfo.Value;
		}
		return result;
	}

	private static ResourceKeyValue GetResourceInfo(string source, string key)
	{
		foreach (ResourceKeyValue resourceKeyValue in ResourceKeyValueList)
		{
			if (source == resourceKeyValue.Source && key == resourceKeyValue.Key)
			{
				return resourceKeyValue;
			}
		}
		return null;
	}

	public static void SetValue(string source, string key, string value)
	{
		ResourceKeyValue resourceInfo = GetResourceInfo(source, key);
		if (resourceInfo != null)
		{
			resourceInfo.Value = value;
		}
		else
		{
			AddValue(source, key, value);
		}
	}

	private static void AddValue(string source, string key, string value)
	{
		ResourceKeyValue resourceKeyValue = new ResourceKeyValue();
		resourceKeyValue.Source = source;
		resourceKeyValue.Key = key;
		resourceKeyValue.Value = value;
		ResourceKeyValueList.Add(resourceKeyValue);
	}

	public static string GetResourceString(Type resourceType, string keyPath)
	{
		ResourceReadCount++;
		string text = "!!!" + keyPath + "!!!";
		string[] array = keyPath.Split('.');
		if (array.Length < 2 || array[0] != "Resources")
		{
			return text;
		}
		string text2 = array[1];
		if (LoadFromLngFile)
		{
			Assembly assembly = resourceType.Assembly;
			if (!(assembly == null))
			{
				string value = GetValue(Path.GetFileName(assembly.Location), text2);
				if (!string.IsNullOrWhiteSpace(value))
				{
					text = value;
				}
			}
		}
		else
		{
			ResourceManager resourceManager = null;
			PropertyInfo property = resourceType.GetProperty("ResourceManager", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (property != null)
			{
				resourceManager = property.GetValue(null, null) as ResourceManager;
			}
			if (resourceManager != null)
			{
				CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
				string @string = resourceManager.GetString(text2, currentUICulture);
				if (!string.IsNullOrWhiteSpace(@string))
				{
					text = @string;
				}
			}
		}
		return GetUnEscapedText(text);
	}

	private static string GetUnEscapedText(string value)
	{
		return value.Replace("\\r", "\r").Replace("\\n", "\n").Replace("\r\n", "\n")
			.Replace("\n", "\r\n");
	}

	public static void ChangeLanguage(Control resxOwner, ToolTip resxOwnersToolTip, CultureInfo culture)
	{
		SetResxText(resxOwner, resxOwnersToolTip, culture);
		UpdateTextIfLngEnabled(resxOwner, resxOwnersToolTip);
	}

	private static void SetKeyPathValue(string keyPath, string value, Control resxOwner, ToolTip resxOwnersToolTip)
	{
		string[] array = keyPath.Split('.');
		object obj = resxOwner;
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i];
			if (text == "$this")
			{
				continue;
			}
			Type type = obj.GetType();
			PropertyInfo property = type.GetProperty(text, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty);
			if (property != null)
			{
				if (i == array.Length - 1)
				{
					if (property.CanWrite)
					{
						property.SetValue(obj, value, null);
					}
				}
				else
				{
					obj = property.GetValue(obj, null);
				}
				continue;
			}
			FieldInfo field = type.GetField(text, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField);
			if (field != null)
			{
				obj = field.GetValue(obj);
				continue;
			}
			if (text == "ToolTip" && obj is Control)
			{
				Control control = obj as Control;
				resxOwnersToolTip.SetToolTip(control, value);
			}
			break;
		}
	}

	public static void SetResxText(Control resxOwner, ToolTip resxOwnersToolTip, CultureInfo culture)
	{
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(resxOwner.GetType());
		if (componentResourceManager == null)
		{
			return;
		}
		ResourceSet resourceSet = componentResourceManager.GetResourceSet(culture, createIfNotExists: true, tryParents: false);
		if (resourceSet == null)
		{
			return;
		}
		IDictionaryEnumerator enumerator = resourceSet.GetEnumerator();
		if (enumerator == null)
		{
			return;
		}
		while (enumerator.MoveNext())
		{
			if (enumerator.Value is string)
			{
				string text = (string)enumerator.Key;
				if (!text.StartsWith(">>"))
				{
					string value = (string)enumerator.Value;
					value = GetUnEscapedText(value);
					SetKeyPathValue(text, value, resxOwner, resxOwnersToolTip);
				}
			}
		}
	}

	public static void UpdateTextIfLngEnabled(Control resxOwner, ToolTip resxOwnersToolTip)
	{
		if (LoadFromLngFile)
		{
			try
			{
				SetLngText(resxOwner, resxOwnersToolTip);
			}
			catch
			{
			}
		}
	}

	public static void SetLngText(Control resxOwner, ToolTip toolTip)
	{
		string name = resxOwner.GetType().Name;
		foreach (ResourceKeyValue resourceKeyValue in ResourceKeyValueList)
		{
			if (resourceKeyValue.Source == name)
			{
				string key = resourceKeyValue.Key;
				string value = resourceKeyValue.Value;
				value = GetUnEscapedText(value);
				SetKeyPathValue(key, value, resxOwner, toolTip);
			}
		}
	}
}
