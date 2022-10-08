namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.All)]
public class DescriptionJaAttribute : Attribute
{
	public static readonly DescriptionAttribute Default;

	public virtual string Description => DescriptionValue;

	protected string DescriptionValue { get; set; }

	public DescriptionJaAttribute()
	{
		DescriptionValue = "";
	}

	public DescriptionJaAttribute(string description)
	{
		DescriptionValue = description;
	}

	public override bool Equals(object obj)
	{
		string descriptionValue = DescriptionValue;
		_ = ((DescriptionJaAttribute)obj).DescriptionValue;
		if (string.IsNullOrWhiteSpace(descriptionValue))
		{
			if (string.IsNullOrWhiteSpace(descriptionValue))
			{
				return true;
			}
			return false;
		}
		if (string.IsNullOrWhiteSpace(descriptionValue))
		{
			return false;
		}
		return DescriptionValue.Equals(obj);
	}

	public override int GetHashCode()
	{
		return DescriptionValue.GetHashCode();
	}

	public override bool IsDefaultAttribute()
	{
		return false;
	}
}
