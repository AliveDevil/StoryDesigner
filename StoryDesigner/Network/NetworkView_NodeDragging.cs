/* Copyright: Ashley Davis (http://www.codeproject.com/Articles/182683/NetworkView-A-WPF-custom-control-for-visualizing-a) */
using System;
using System.Collections.Generic;

namespace StoryDesigner.Network
{
	public partial class NetworkView
	{
		private void NodeItem_DragCompleted(object source, NodeDragCompletedEventArgs e)
		{
			e.Handled = true;

			var eventArgs = new NodeDragCompletedEventArgs(NodeDragCompletedEvent, this, this.SelectedNodes);
			RaiseEvent(eventArgs);

			if (cachedSelectedNodeItems != null)
			{
				cachedSelectedNodeItems = null;
			}

			this.IsDragging = false;
			this.IsNotDragging = true;
			this.IsDraggingNode = false;
			this.IsNotDraggingNode = true;
		}

		private void NodeItem_Dragging(object source, NodeDraggingEventArgs e)
		{
			e.Handled = true;
			if (this.cachedSelectedNodeItems == null)
			{
				this.cachedSelectedNodeItems = new List<NodeItem>();

				foreach (var selectedNode in this.SelectedNodes)
				{
					NodeItem nodeItem = FindAssociatedNodeItem(selectedNode);
					if (nodeItem == null)
					{
						throw new ApplicationException("Unexpected code path!");
					}

					this.cachedSelectedNodeItems.Add(nodeItem);
				}
			}
			foreach (var nodeItem in this.cachedSelectedNodeItems)
			{
				nodeItem.X += e.HorizontalChange;
				nodeItem.Y += e.VerticalChange;
			}

			var eventArgs = new NodeDraggingEventArgs(NodeDraggingEvent, this, this.SelectedNodes, e.HorizontalChange, e.VerticalChange);
			RaiseEvent(eventArgs);
		}

		private void NodeItem_DragStarted(object source, NodeDragStartedEventArgs e)
		{
			e.Handled = true;

			this.IsDragging = true;
			this.IsNotDragging = false;
			this.IsDraggingNode = true;
			this.IsNotDraggingNode = false;

			var eventArgs = new NodeDragStartedEventArgs(NodeDragStartedEvent, this, this.SelectedNodes);
			RaiseEvent(eventArgs);

			e.Cancel = eventArgs.Cancel;
		}
	}
}
