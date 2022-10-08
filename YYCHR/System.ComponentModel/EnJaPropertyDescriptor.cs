namespace System.ComponentModel;

public class EnJaPropertyDescriptor : PropertyDescriptor
{
	public static bool EnJaConvert;

	private readonly PropertyDescriptor original;

	public override string Description
	{
		get
		{
			if (EnJaConvert)
			{
				DescriptionJaAttribute descriptionJaAttribute = (DescriptionJaAttribute)original.Attributes[typeof(DescriptionJaAttribute)];
				if (descriptionJaAttribute != null)
				{
					string text = descriptionJaAttribute.Description;
					DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)original.Attributes[typeof(DefaultValueAttribute)];
					if (defaultValueAttribute != null)
					{
						object value = defaultValueAttribute.Value;
						text = ((value == null) ? (text + " (デフォルト値：null)") : (text + " (デフォルト値：" + value.ToString() + ")"));
					}
					return text;
				}
				return original.Description;
			}
			DescriptionAttribute descriptionAttribute = (DescriptionAttribute)original.Attributes[typeof(DescriptionAttribute)];
			if (descriptionAttribute != null)
			{
				string text2 = descriptionAttribute.Description;
				DefaultValueAttribute defaultValueAttribute2 = (DefaultValueAttribute)original.Attributes[typeof(DefaultValueAttribute)];
				if (defaultValueAttribute2 != null)
				{
					object value2 = defaultValueAttribute2.Value;
					text2 = ((value2 == null) ? (text2 + " (default : null)") : (text2 + " (default : " + value2.ToString() + ")"));
				}
				return text2;
			}
			return original.Description;
		}
	}

	public override string Category => original.Category;

	public override string DisplayName => original.DisplayName;

	public override Type ComponentType => original.ComponentType;

	public override bool IsReadOnly => original.IsReadOnly;

	public override Type PropertyType => original.PropertyType;

	public EnJaPropertyDescriptor(PropertyDescriptor original)
		: base(original)
	{
		this.original = original;
	}

	public override bool CanResetValue(object component)
	{
		return original.CanResetValue(component);
	}

	public override object GetValue(object component)
	{
		return original.GetValue(component);
	}

	public override void ResetValue(object component)
	{
		original.ResetValue(component);
	}

	public override bool ShouldSerializeValue(object component)
	{
		return original.ShouldSerializeValue(component);
	}

	public override void SetValue(object component, object value)
	{
		original.SetValue(component, value);
	}
}
