namespace CharactorLib.Common;

internal class ResourceKeyValue
{
	public string Source { get; set; } = "";


	public string Key { get; set; } = "";


	public string Value { get; set; } = "";


	public static ResourceKeyValue FromLngLine(string line)
	{
		if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#") || line.StartsWith("#"))
		{
			return null;
		}
		line = line.Replace("@@", "@");
		char[] separator = new char[2] { '@', '=' };
		string[] array = line.Split(separator);
		if (array.Length == 3)
		{
			ResourceKeyValue resourceKeyValue = new ResourceKeyValue();
			string text = array[0];
			string text2 = array[1];
			string text3 = array[2];
			text = text.Replace("[at]", "@");
			text = text.Replace("[equal]", "=");
			text2 = text2.Replace("[at]", "@");
			text2 = text2.Replace("[equal]", "=");
			text3 = text3.Replace("[at]", "@");
			text3 = text3.Replace("[equal]", "=");
			text3 = text3.Replace("\\r", "\r");
			text3 = text3.Replace("\\n", "\n");
			text3 = text3.Replace("\\t", "\t");
			resourceKeyValue.Source = text;
			resourceKeyValue.Key = text2;
			resourceKeyValue.Value = text3;
			return resourceKeyValue;
		}
		return null;
	}

	public override string ToString()
	{
		string source = Source;
		string key = Key;
		string value = Value;
		source = source.Replace("@", "[at]");
		source = source.Replace("=", "[equal]");
		key = key.Replace("@", "[at]");
		key = key.Replace("=", "[equal]");
		value = value.Replace("@", "[at]");
		value = value.Replace("=", "[equal]");
		value = value.Replace("\r", "\\r");
		value = value.Replace("\n", "\\n");
		value = value.Replace("\t", "\\t");
		return (source + "@" + key + "=" + value).Replace("@", "@@");
	}
}
