using System;

namespace CharactorLib.Data;

public class JumpListInfo
{
	public string Name { get; set; } = "";


	public int Address { get; set; }

	public int Format { get; set; } = -1;


	public int Pattern { get; set; } = -1;


	public string NameGui
	{
		get
		{
			if (!string.IsNullOrEmpty(Name))
			{
				return Name;
			}
			return Address.ToString("X");
		}
	}

	public JumpListInfo()
	{
	}

	public JumpListInfo(string line)
	{
		if (line == null)
		{
			return;
		}
		string[] array = line.Split(',');
		if (array == null)
		{
			return;
		}
		if (array.Length >= 1)
		{
			Name = array[0];
		}
		if (array.Length >= 2)
		{
			string value = array[1];
			if (!string.IsNullOrWhiteSpace(value))
			{
				Address = Convert.ToInt32(value, 16);
			}
		}
		if (array.Length >= 3)
		{
			string value = array[2];
			if (!string.IsNullOrWhiteSpace(value))
			{
				Format = Convert.ToInt32(value, 16);
			}
		}
		if (array.Length >= 4)
		{
			string value = array[3];
			if (!string.IsNullOrWhiteSpace(value))
			{
				Pattern = Convert.ToInt32(value, 16);
			}
		}
	}

	public override string ToString()
	{
		string text = Name + "," + Address.ToString("X8");
		if (Format >= 0)
		{
			text = text + "," + Format.ToString("X");
		}
		if (Pattern >= 0)
		{
			text = text + "," + Pattern.ToString("X");
		}
		return text;
	}
}
