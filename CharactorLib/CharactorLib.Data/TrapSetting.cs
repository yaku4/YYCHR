namespace CharactorLib.Data;

public class TrapSetting
{
	public int Address { get; set; }

	public string Command { get; set; } = "";


	public TrapSetting(int address, string command)
	{
		Address = address;
		Command = command;
	}
}
