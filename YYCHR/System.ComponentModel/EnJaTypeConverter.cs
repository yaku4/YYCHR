using System.Linq;

namespace System.ComponentModel;

public class EnJaTypeConverter : TypeConverter
{
	public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
	{
		PropertyDescriptor[] properties = (from PropertyDescriptor original in TypeDescriptor.GetProperties(value, attributes, noCustomTypeDesc: true)
			select new EnJaPropertyDescriptor(original)).ToArray();
		return new PropertyDescriptorCollection(properties);
	}

	public override bool GetPropertiesSupported(ITypeDescriptorContext context)
	{
		return true;
	}
}
