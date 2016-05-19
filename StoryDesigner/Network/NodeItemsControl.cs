/* Copyright: Ashley Davis (http://www.codeproject.com/Articles/182683/NetworkView-A-WPF-custom-control-for-visualizing-a) */
using System.Windows;
using System.Windows.Controls;

namespace StoryDesigner.Network
{
	internal class NodeItemsControl : ListBox
	{
		public NodeItemsControl()
		{
			Focusable = false;
		}

		internal NodeItem FindAssociatedNodeItem(object nodeDataContext)
		{
			return (NodeItem)this.ItemContainerGenerator.ContainerFromItem(nodeDataContext);
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new NodeItem();
		}

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is NodeItem;
		}
	}
}
