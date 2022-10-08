using System.Windows.Forms;

namespace Controls;

public class ComboBoxEx : ComboBox
{
	protected override void OnMouseWheel(MouseEventArgs e)
	{
		bool flag = false;
		try
		{
			if (base.Items.Count > 0)
			{
				int num = 0;
				if (e.Delta < 0)
				{
					num = 1;
				}
				if (e.Delta > 0)
				{
					num = -1;
				}
				int num2 = SelectedIndex;
				int num3 = 0;
				int num4 = base.Items.Count - 1;
				if (num4 < num3)
				{
					num4 = num3;
				}
				num2 += num;
				if (num2 <= num3)
				{
					num2 = num3;
				}
				if (num2 >= num4)
				{
					num2 = num4;
				}
				if (SelectedIndex != num2)
				{
					SelectedIndex = num2;
				}
				flag = ((e as HandledMouseEventArgs).Handled = true);
			}
		}
		catch
		{
		}
		if (!flag)
		{
			base.OnMouseWheel(e);
		}
	}
}
