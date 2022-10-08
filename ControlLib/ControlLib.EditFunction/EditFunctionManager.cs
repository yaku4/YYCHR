using System.Collections.Generic;

namespace ControlLib.EditFunction;

public class EditFunctionManager
{
	private static EditFunctionManager sInstance;

	private List<EditFunctionBase> mFunctionList = new List<EditFunctionBase>();

	private int mSelectedIndex;

	public List<EditFunctionBase> FunctionList => mFunctionList;

	public int SelectedIndex
	{
		get
		{
			return mSelectedIndex;
		}
		set
		{
			if (value >= mFunctionList.Count)
			{
				value = mFunctionList.Count - 1;
			}
			if (value < 0)
			{
				value = 0;
			}
			mSelectedIndex = value;
		}
	}

	public EditFunctionBase SelectedFunction
	{
		get
		{
			return mFunctionList[mSelectedIndex];
		}
		set
		{
			try
			{
				mSelectedIndex = mFunctionList.IndexOf(value);
			}
			catch
			{
				mSelectedIndex = 0;
			}
		}
	}

	public FillPaint FillPaintPen { get; private set; }

	public ColSet ColSetPen { get; private set; }

	public static EditFunctionManager GetInstance()
	{
		if (sInstance == null)
		{
			sInstance = new EditFunctionManager();
		}
		return sInstance;
	}

	private EditFunctionManager()
	{
		InitFunctionList();
	}

	public void InitFunctionList()
	{
		ColSetPen = null;
		FunctionList.Clear();
		Add(new DrawPenLine());
		Add(new PatternPen());
		Add(new DrawLine());
		Add(new DrawRect());
		Add(new FillRect());
		Add(new PatternRect());
		Add(new DrawEllipse());
		Add(new FillEllipse());
		FillPaintPen = new FillPaint();
		Add(FillPaintPen);
		Add(new Stamp());
		ColSetPen = new ColSet();
		Add(ColSetPen);
		int num = SelectedIndex;
		int num2 = 0;
		int num3 = mFunctionList.Count - 1;
		if (num > num3)
		{
			num = num3;
		}
		if (num < num2)
		{
			num = num2;
		}
		SelectedIndex = num;
	}

	public void Add(EditFunctionBase function)
	{
		FunctionList.Add(function);
	}

	public EditFunctionBase SelectFunctionAdd(int delta, bool enableColSet)
	{
		int selectedIndex = SelectedIndex;
		int num = FunctionList.Count;
		if (!enableColSet)
		{
			num--;
		}
		selectedIndex = (selectedIndex + delta * -1 + num) % num;
		if (selectedIndex >= 0 && selectedIndex < num)
		{
			SelectedIndex = selectedIndex;
		}
		return SelectedFunction;
	}
}
