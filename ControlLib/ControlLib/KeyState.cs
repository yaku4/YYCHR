namespace ControlLib;

public static class KeyState
{
	public static bool Control { get; set; }

	public static bool Shift { get; set; }

	public static bool Alt { get; set; }

	public static byte State
	{
		get
		{
			byte b = 0;
			if (Control)
			{
				b = (byte)(b | 1u);
			}
			if (Shift)
			{
				b = (byte)(b | 2u);
			}
			if (Alt)
			{
				b = (byte)(b | 4u);
			}
			return b;
		}
	}
}
