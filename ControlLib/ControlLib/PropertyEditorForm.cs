using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using CharactorLib.Common;
using ControlLib.Properties;
using CustumPropertyGrid;

namespace ControlLib;

public class PropertyEditorForm : Form
{
	private List<TreeNode> mAllTreeNode = new List<TreeNode>();

	private List<TreeNode> mNoParentNode = new List<TreeNode>();

	private TreeNode mControlNode;

	private TreeNode mItemNode;

	private TreeNode mOtherNode;

	private IContainer components;

	private PropertyGrid propertyGrid;

	private SplitContainer splitContainer1;

	private TreeView treeViewTarget;

	private ImageList imageList;

	private ContextMenuStrip contextMenuPropertyGrid;

	private ToolStripMenuItem miAddSelectedMemberToTree;

	private ToolStripMenuItem miReset;

	private ToolStrip toolStrip;

	private ToolStripButton tbSetting;

	private ToolStripButton tbResetTree;

	private ToolStripButton tbEditSelectedTreeNodeTag;

	private ToolStripButton tbEditSelectedPropertyGridItem;

	private ToolStripSeparator tbSepTree;

	private ToolStripSeparator tbSepPropertyGrid;

	[Browsable(false)]
	public object EditObject { get; set; }

	[Browsable(false)]
	public object EditingObject { get; set; }

	public bool FlagPublic { get; set; } = true;


	public bool FlagNonPublic { get; set; } = true;


	public bool FlagInstance { get; set; } = true;


	public bool FlagStatic { get; set; }

	public bool FlagDeclaredOnly { get; set; } = true;


	public PropertyEditorForm()
	{
		InitializeComponent();
		AttributeReloadPropertyGridTab.SetAttributeReloadPropertyGridTab(propertyGrid, hideToolStripItemsForTab: true);
		ToolStrip toolStrip = AttributeReloadPropertyGridTab.GetToolStrip(propertyGrid);
		if (toolStrip != null)
		{
			this.toolStrip.Items.Remove(tbSepPropertyGrid);
			toolStrip.Items.Add(tbSepPropertyGrid);
			this.toolStrip.Items.Remove(tbEditSelectedPropertyGridItem);
			toolStrip.Items.Add(tbEditSelectedPropertyGridItem);
		}
		ResourceUtility.UpdateTextIfLngEnabled(this, null);
	}

	private void PropertyEditorForm_Load(object sender, EventArgs e)
	{
		imageList.Images.Clear();
		imageList.Images.Add(Resources.IconForm);
		imageList.Images.Add(Resources.IconControl);
		imageList.Images.Add(Resources.IconDocument);
		imageList.Images.Add(Resources.IconFolder);
		if (EditObject == null)
		{
			EditObject = this;
		}
	}

	private void PropertyEditorForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (base.Modal)
		{
			e.Cancel = false;
		}
		else
		{
			e.Cancel = false;
		}
	}

	private void PropertyEditorForm_VisibleChanged(object sender, EventArgs e)
	{
		if (base.Visible)
		{
			EditingObject = EditObject;
			ResetTreeNodes();
		}
	}

	private void ResetTreeNodes()
	{
		TreeView treeView = treeViewTarget;
		object obj = EditingObject;
		if (obj == null)
		{
			obj = this;
		}
		Type type = obj.GetType();
		string text = (type.IsSubclassOf(typeof(Form)) ? ((Control)obj).Name : ((!type.IsSubclassOf(typeof(ToolStripItem))) ? obj.ToString() : ((ToolStripItem)obj).Name));
		string name = type.Name;
		string obj2 = text + " <" + name + ">";
		treeView.Nodes.Clear();
		mAllTreeNode.Clear();
		mNoParentNode.Clear();
		TreeNode treeNode = new TreeNode(obj2);
		treeNode.Tag = obj;
		treeNode.ImageIndex = 0;
		treeView.Nodes.Add(treeNode);
		mAllTreeNode.Add(treeNode);
		TreeNode treeNode2 = new TreeNode("@NoParent");
		treeNode2.ImageIndex = 3;
		treeNode.Nodes.Add(treeNode2);
		mAllTreeNode.Add(treeNode2);
		mControlNode = treeNode2;
		TreeNode treeNode3 = new TreeNode("@ToolStripItem");
		treeNode3.ImageIndex = 3;
		treeNode.Nodes.Add(treeNode3);
		mAllTreeNode.Add(treeNode3);
		mItemNode = treeNode3;
		TreeNode treeNode4 = new TreeNode("@Other");
		treeNode4.ImageIndex = 3;
		treeNode.Nodes.Add(treeNode4);
		mAllTreeNode.Add(treeNode4);
		mOtherNode = treeNode4;
		AddTargetChildNodes(treeNode, obj);
		treeNode.Expand();
	}

	private void AddTargetChildNodes(TreeNode parentNode, object target)
	{
		Type type = target.GetType();
		BindingFlags bindingFlags = BindingFlags.Default;
		if (FlagPublic)
		{
			bindingFlags |= BindingFlags.Public;
		}
		if (FlagNonPublic)
		{
			bindingFlags |= BindingFlags.NonPublic;
		}
		if (FlagInstance)
		{
			bindingFlags |= BindingFlags.Instance;
		}
		if (FlagStatic)
		{
			bindingFlags |= BindingFlags.Static;
		}
		if (FlagDeclaredOnly)
		{
			bindingFlags |= BindingFlags.DeclaredOnly;
		}
		FieldInfo[] fields = type.GetFields(bindingFlags);
		foreach (FieldInfo fieldInfo in fields)
		{
			object value = fieldInfo.GetValue(target);
			if (value == null || CheckRegisted(value))
			{
				continue;
			}
			Type type2 = value.GetType();
			if (type2.IsPrimitive || !type2.IsClass)
			{
				continue;
			}
			string name = fieldInfo.Name;
			string name2 = type2.Name;
			string text = name + " <" + name2 + ">";
			if (type2.IsSubclassOf(typeof(Form)))
			{
				Form control = (Form)value;
				TreeNode treeNode = new TreeNode(text);
				treeNode.Tag = value;
				treeNode.ImageIndex = 0;
				AddChildNode(treeNode, addNoParentNodeToRoot: false);
				AddSubControl(treeNode, control);
			}
			else if (type2.IsSubclassOf(typeof(Control)))
			{
				Control control2 = (Control)value;
				if (control2.Parent == target)
				{
					TreeNode treeNode2 = new TreeNode(text);
					treeNode2.Tag = value;
					treeNode2.ImageIndex = 1;
					AddChildNode(treeNode2, addNoParentNodeToRoot: false);
					AddSubControl(treeNode2, control2);
				}
				else
				{
					TreeNode treeNode3 = new TreeNode(text);
					treeNode3.Tag = value;
					treeNode3.ImageIndex = 1;
					AddChildNode(treeNode3, addNoParentNodeToRoot: false);
					AddSubControl(treeNode3, control2);
				}
			}
			else if (type2.IsSubclassOf(typeof(ToolStripItem)))
			{
				_ = (ToolStripItem)value;
				TreeNode treeNode4 = new TreeNode(text);
				treeNode4.Tag = value;
				treeNode4.ImageIndex = 2;
				AddChildNode(treeNode4, addNoParentNodeToRoot: false);
			}
			else
			{
				TreeNode treeNode5 = new TreeNode(text);
				treeNode5.Tag = value;
				treeNode5.ImageIndex = 2;
				AddChildNode(treeNode5, addNoParentNodeToRoot: false);
			}
		}
		foreach (TreeNode item in mNoParentNode)
		{
			AddChildNode(item, addNoParentNodeToRoot: true);
		}
	}

	private bool CheckRegisted(object obj)
	{
		bool result = false;
		foreach (TreeNode item in mAllTreeNode)
		{
			if (item.Tag == obj)
			{
				return true;
			}
		}
		return result;
	}

	private void AddChildNode(TreeNode childTreeNode, bool addNoParentNodeToRoot)
	{
		object obj = null;
		object tag = childTreeNode.Tag;
		Type type = tag.GetType();
		if (type.IsSubclassOf(typeof(Control)))
		{
			obj = (tag as Control).Parent;
		}
		else if (type.IsSubclassOf(typeof(ToolStripItem)))
		{
			ToolStripItem toolStripItem = tag as ToolStripItem;
			obj = toolStripItem.OwnerItem;
			if (obj == null)
			{
				obj = toolStripItem.Owner;
			}
		}
		else
		{
			obj = null;
		}
		TreeNode treeNode = null;
		if (obj != null)
		{
			foreach (TreeNode item in mAllTreeNode)
			{
				if (item.Tag == obj)
				{
					treeNode = item;
					break;
				}
			}
		}
		if (treeNode != null)
		{
			treeNode.Nodes.Add(childTreeNode);
			mAllTreeNode.Add(childTreeNode);
		}
		else if (addNoParentNodeToRoot)
		{
			AddNoParentTreeNode(childTreeNode);
		}
		else
		{
			mNoParentNode.Add(childTreeNode);
			mAllTreeNode.Add(childTreeNode);
		}
	}

	private void AddNoParentTreeNode(TreeNode childTreeNode)
	{
		object tag = childTreeNode.Tag;
		if (tag != null)
		{
			Type type = tag.GetType();
			if (type.IsSubclassOf(typeof(Form)))
			{
				mControlNode.Nodes.Add(childTreeNode);
			}
			else if (type.IsSubclassOf(typeof(Control)))
			{
				mAllTreeNode[0].Nodes.Add(childTreeNode);
			}
			else if (type.IsSubclassOf(typeof(ToolStripItem)))
			{
				mItemNode.Nodes.Add(childTreeNode);
			}
			else
			{
				mOtherNode.Nodes.Add(childTreeNode);
			}
		}
		else
		{
			mOtherNode.Nodes.Add(childTreeNode);
		}
	}

	private void AddSubControl(TreeNode treeNode, Control control)
	{
		foreach (Control control2 in control.Controls)
		{
			object obj = control2;
			if (CheckRegisted(obj))
			{
				continue;
			}
			Type type = obj.GetType();
			string name = control2.Name;
			string name2 = type.Name;
			TreeNode treeNode2 = new TreeNode(name + " <" + name2 + ">");
			treeNode2.Tag = control2;
			treeNode2.ImageIndex = 1;
			treeNode.Nodes.Add(treeNode2);
			mAllTreeNode.Add(treeNode2);
			AddSubControl(treeNode2, control2);
			if (control is ToolStrip)
			{
				ToolStrip toolStrip = (ToolStrip)control;
				if (toolStrip.Items != null)
				{
					AddSubItem(treeNode2, toolStrip.Items);
				}
			}
			if (control is ToolStripPanel)
			{
				treeNode.Collapse();
			}
			else
			{
				treeNode.Expand();
			}
		}
	}

	private void AddSubItem(TreeNode treeNode, ToolStripItemCollection items)
	{
		foreach (ToolStripItem item in items)
		{
			object obj = item;
			if (!CheckRegisted(obj))
			{
				Type type = obj.GetType();
				string name = item.Name;
				string name2 = type.Name;
				TreeNode treeNode2 = new TreeNode(name + " <" + name2 + ">");
				treeNode2.Tag = treeNode2;
				treeNode2.ImageIndex = 1;
				treeNode.Nodes.Add(treeNode2);
				mAllTreeNode.Add(treeNode2);
				treeNode.Collapse();
			}
		}
	}

	private void objectTreeView_AfterSelect(object sender, TreeViewEventArgs e)
	{
		TreeNode node = e.Node;
		node.SelectedImageIndex = node.ImageIndex;
		try
		{
			if (node != null)
			{
				object tag = node.Tag;
				if (tag != null)
				{
					propertyGrid.SelectedObject = tag;
				}
			}
		}
		catch (Exception)
		{
		}
	}

	private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
	{
		bool enabled = false;
		bool enabled2 = false;
		object selectedObject = propertyGrid.SelectedObject;
		GridItem selectedGridItem = propertyGrid.SelectedGridItem;
		if (selectedGridItem != null && selectedGridItem.GridItemType == GridItemType.Property && selectedGridItem.PropertyDescriptor != null)
		{
			enabled = selectedGridItem.PropertyDescriptor.CanResetValue(selectedObject);
			enabled2 = selectedGridItem.PropertyDescriptor.GetType().IsClass;
		}
		miReset.Enabled = enabled;
		miAddSelectedMemberToTree.Enabled = enabled2;
	}

	private void miReset_Click(object sender, EventArgs e)
	{
		try
		{
			propertyGrid.ResetSelectedProperty();
		}
		catch
		{
		}
	}

	private void tbSetting_Click(object sender, EventArgs e)
	{
		propertyGrid.SelectedObject = this;
	}

	private void tbResetTree_Click(object sender, EventArgs e)
	{
		try
		{
			EditingObject = EditObject;
			ResetTreeNodes();
		}
		catch
		{
		}
	}

	private void tbEditSelectedTreeNodeTag_Click(object sender, EventArgs e)
	{
		try
		{
			if (treeViewTarget != null && treeViewTarget.SelectedNode != null && treeViewTarget.SelectedNode.Tag != null)
			{
				EditingObject = treeViewTarget.SelectedNode.Tag;
			}
			ResetTreeNodes();
		}
		catch
		{
		}
	}

	private void tbEditSelectedPropertyGridItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (propertyGrid != null && propertyGrid.SelectedGridItem != null && propertyGrid.SelectedGridItem.Value != null)
			{
				EditingObject = propertyGrid.SelectedGridItem.Value;
			}
			ResetTreeNodes();
		}
		catch
		{
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlLib.PropertyEditorForm));
		this.splitContainer1 = new System.Windows.Forms.SplitContainer();
		this.treeViewTarget = new System.Windows.Forms.TreeView();
		this.imageList = new System.Windows.Forms.ImageList(this.components);
		this.toolStrip = new System.Windows.Forms.ToolStrip();
		this.tbSetting = new System.Windows.Forms.ToolStripButton();
		this.tbResetTree = new System.Windows.Forms.ToolStripButton();
		this.tbSepTree = new System.Windows.Forms.ToolStripSeparator();
		this.tbEditSelectedTreeNodeTag = new System.Windows.Forms.ToolStripButton();
		this.tbSepPropertyGrid = new System.Windows.Forms.ToolStripSeparator();
		this.tbEditSelectedPropertyGridItem = new System.Windows.Forms.ToolStripButton();
		this.propertyGrid = new System.Windows.Forms.PropertyGrid();
		this.contextMenuPropertyGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.miReset = new System.Windows.Forms.ToolStripMenuItem();
		this.miAddSelectedMemberToTree = new System.Windows.Forms.ToolStripMenuItem();
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).BeginInit();
		this.splitContainer1.Panel1.SuspendLayout();
		this.splitContainer1.Panel2.SuspendLayout();
		this.splitContainer1.SuspendLayout();
		this.toolStrip.SuspendLayout();
		this.contextMenuPropertyGrid.SuspendLayout();
		base.SuspendLayout();
		resources.ApplyResources(this.splitContainer1, "splitContainer1");
		this.splitContainer1.Name = "splitContainer1";
		this.splitContainer1.Panel1.Controls.Add(this.treeViewTarget);
		this.splitContainer1.Panel1.Controls.Add(this.toolStrip);
		this.splitContainer1.Panel2.Controls.Add(this.propertyGrid);
		resources.ApplyResources(this.treeViewTarget, "treeViewTarget");
		this.treeViewTarget.HideSelection = false;
		this.treeViewTarget.ImageList = this.imageList;
		this.treeViewTarget.Name = "treeViewTarget";
		this.treeViewTarget.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(objectTreeView_AfterSelect);
		this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
		resources.ApplyResources(this.imageList, "imageList");
		this.imageList.TransparentColor = System.Drawing.Color.Fuchsia;
		this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[6] { this.tbSetting, this.tbResetTree, this.tbSepTree, this.tbEditSelectedTreeNodeTag, this.tbSepPropertyGrid, this.tbEditSelectedPropertyGridItem });
		resources.ApplyResources(this.toolStrip, "toolStrip");
		this.toolStrip.Name = "toolStrip";
		this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		this.tbSetting.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbSetting.Image = ControlLib.Properties.Resources.IconOptionSetting;
		resources.ApplyResources(this.tbSetting, "tbSetting");
		this.tbSetting.Name = "tbSetting";
		this.tbSetting.Click += new System.EventHandler(tbSetting_Click);
		this.tbResetTree.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbResetTree.Image = ControlLib.Properties.Resources.IconFindPrev;
		resources.ApplyResources(this.tbResetTree, "tbResetTree");
		this.tbResetTree.Name = "tbResetTree";
		this.tbResetTree.Click += new System.EventHandler(tbResetTree_Click);
		this.tbSepTree.Name = "tbSepTree";
		resources.ApplyResources(this.tbSepTree, "tbSepTree");
		this.tbEditSelectedTreeNodeTag.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbEditSelectedTreeNodeTag.Image = ControlLib.Properties.Resources.IconFindNext;
		resources.ApplyResources(this.tbEditSelectedTreeNodeTag, "tbEditSelectedTreeNodeTag");
		this.tbEditSelectedTreeNodeTag.Name = "tbEditSelectedTreeNodeTag";
		this.tbEditSelectedTreeNodeTag.Click += new System.EventHandler(tbEditSelectedTreeNodeTag_Click);
		this.tbSepPropertyGrid.Name = "tbSepPropertyGrid";
		resources.ApplyResources(this.tbSepPropertyGrid, "tbSepPropertyGrid");
		this.tbEditSelectedPropertyGridItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tbEditSelectedPropertyGridItem.Image = ControlLib.Properties.Resources.IconFindNext;
		resources.ApplyResources(this.tbEditSelectedPropertyGridItem, "tbEditSelectedPropertyGridItem");
		this.tbEditSelectedPropertyGridItem.Name = "tbEditSelectedPropertyGridItem";
		this.tbEditSelectedPropertyGridItem.Click += new System.EventHandler(tbEditSelectedPropertyGridItem_Click);
		this.propertyGrid.ContextMenuStrip = this.contextMenuPropertyGrid;
		resources.ApplyResources(this.propertyGrid, "propertyGrid");
		this.propertyGrid.Name = "propertyGrid";
		this.contextMenuPropertyGrid.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.miReset, this.miAddSelectedMemberToTree });
		this.contextMenuPropertyGrid.Name = "contextMenuStrip1";
		resources.ApplyResources(this.contextMenuPropertyGrid, "contextMenuPropertyGrid");
		this.contextMenuPropertyGrid.Opening += new System.ComponentModel.CancelEventHandler(contextMenuStrip1_Opening);
		this.miReset.Name = "miReset";
		resources.ApplyResources(this.miReset, "miReset");
		this.miReset.Click += new System.EventHandler(miReset_Click);
		this.miAddSelectedMemberToTree.Name = "miAddSelectedMemberToTree";
		resources.ApplyResources(this.miAddSelectedMemberToTree, "miAddSelectedMemberToTree");
		this.miAddSelectedMemberToTree.Click += new System.EventHandler(tbEditSelectedPropertyGridItem_Click);
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.splitContainer1);
		base.Name = "PropertyEditorForm";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(PropertyEditorForm_FormClosing);
		base.Load += new System.EventHandler(PropertyEditorForm_Load);
		base.VisibleChanged += new System.EventHandler(PropertyEditorForm_VisibleChanged);
		this.splitContainer1.Panel1.ResumeLayout(false);
		this.splitContainer1.Panel1.PerformLayout();
		this.splitContainer1.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).EndInit();
		this.splitContainer1.ResumeLayout(false);
		this.toolStrip.ResumeLayout(false);
		this.toolStrip.PerformLayout();
		this.contextMenuPropertyGrid.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
