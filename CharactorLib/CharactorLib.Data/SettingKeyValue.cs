namespace CharactorLib.Data;

public class SettingKeyValue
{
	private static char SettingSplitterC = '=';

	private static string SettingSplitterS = "=";

	public string Key;

	public string Value;

	public bool Valid;

	public SettingKeyValue(string key, string value)
	{
		Key = key;
		Value = value;
		Valid = true;
	}

	public SettingKeyValue(string line)
	{
		string key = string.Empty;
		string value = string.Empty;
		bool valid = false;
		string[] array = line.Split(SettingSplitterC);
		if (array.Length >= 2)
		{
			key = array[0];
			value = array[1];
			valid = true;
		}
		Key = key;
		Value = value;
		Valid = valid;
	}

	public override string ToString()
	{
		return Key + SettingSplitterS + Value;
	}
}
