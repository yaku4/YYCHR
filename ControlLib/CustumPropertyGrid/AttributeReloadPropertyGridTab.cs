using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.PropertyGridInternal;

namespace CustumPropertyGrid;

public class AttributeReloadPropertyGridTab : PropertyTab
{
	private class AttributeReloadPropertyDescripter : PropertyDescriptor
	{
		private PropertyDescriptor innerDescriptor;

		public override Type ComponentType => innerDescriptor.ComponentType;

		public override bool IsReadOnly => innerDescriptor.IsReadOnly;

		public override Type PropertyType => innerDescriptor.PropertyType;

		public AttributeReloadPropertyDescripter(PropertyDescriptor pd)
			: base(pd)
		{
			innerDescriptor = pd;
			UpdateAttribute();
		}

		private void UpdateAttribute()
		{
			PropertyInfo property = ComponentType.GetProperty(Name, BindingFlags.Instance | BindingFlags.Public);
			if (!(property != null))
			{
				return;
			}
			List<Type> list = new List<Type>();
			Attribute[] attributeArray = AttributeArray;
			foreach (Attribute attribute in attributeArray)
			{
				list.Add(attribute.GetType());
			}
			attributeArray = Attribute.GetCustomAttributes(property, inherit: true);
			foreach (Attribute attribute2 in attributeArray)
			{
				int num = list.IndexOf(attribute2.GetType());
				if (num >= 0)
				{
					AttributeArray[num] = attribute2;
				}
			}
		}

		public override bool CanResetValue(object component)
		{
			return innerDescriptor.CanResetValue(component);
		}

		public override object GetValue(object component)
		{
			return innerDescriptor.GetValue(component);
		}

		public override void ResetValue(object component)
		{
			innerDescriptor.ResetValue(component);
		}

		public override void SetValue(object component, object value)
		{
			innerDescriptor.SetValue(component, value);
		}

		public override bool ShouldSerializeValue(object component)
		{
			return innerDescriptor.ShouldSerializeValue(component);
		}
	}

	private PropertiesTab defatultTab = new PropertiesTab();

	public override Bitmap Bitmap => new Bitmap(16, 16);

	public override string TabName => DefaultTabName;

	private static string DefaultTabName => "#DumyTabName";

	public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
	{
		PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null, readOnly: false);
		foreach (PropertyDescriptor property in defatultTab.GetProperties(component))
		{
			propertyDescriptorCollection.Add(new AttributeReloadPropertyDescripter(property));
		}
		return propertyDescriptorCollection;
	}

	public static void SetAttributeReloadPropertyGridTab(PropertyGrid grid, bool hideToolStripItemsForTab)
	{
		foreach (PropertiesTab propertyTab in grid.PropertyTabs)
		{
			if (propertyTab.GetType() == typeof(AttributeReloadPropertyGridTab))
			{
				return;
			}
		}
		grid.PropertyTabs.AddTabType(typeof(AttributeReloadPropertyGridTab), PropertyTabScope.Global);
		string tabName = grid.PropertyTabs[0].TabName;
		string defaultTabName = DefaultTabName;
		ToolStripItem toolStripItem = null;
		ToolStripItem toolStripItem2 = null;
		bool flag = false;
		ToolStrip toolStrip = GetToolStrip(grid);
		if (toolStrip != null)
		{
			foreach (ToolStripItem item in toolStrip.Items)
			{
				if (item.Text == tabName)
				{
					toolStripItem = item;
				}
				else if (item.Text == defaultTabName)
				{
					toolStripItem2 = item;
				}
				else if (item is ToolStripSeparator && !flag)
				{
					flag = true;
				}
				if (hideToolStripItemsForTab && flag)
				{
					item.AutoSize = false;
					item.Size = new Size(0, 0);
				}
			}
		}
		if (toolStripItem != null && toolStripItem2 != null)
		{
			toolStrip.Items.Remove(toolStripItem);
			toolStripItem2.Image = toolStripItem.Image;
			toolStripItem2.Text = toolStripItem.Text;
			toolStripItem2.ToolTipText = toolStripItem.ToolTipText;
			toolStripItem2.PerformClick();
			return;
		}
		throw new ApplicationException("PropertyGridの構成が変更されているようです");
	}

	public static ToolStrip GetToolStrip(Control ctl)
	{
		ToolStrip toolStrip = ctl as ToolStrip;
		if (toolStrip == null)
		{
			foreach (Control control in ctl.Controls)
			{
				toolStrip = GetToolStrip(control);
				if (toolStrip != null)
				{
					return toolStrip;
				}
			}
			return toolStrip;
		}
		return toolStrip;
	}
}
