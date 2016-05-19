/* Copyright: Ashley Davis (http://www.codeproject.com/Articles/182683/NetworkView-A-WPF-custom-control-for-visualizing-a) */
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
//using StoryDesigner.Adorner;

namespace StoryDesigner.Network
{
	public partial class NetworkView
	{
		private object draggedOutConnectorDataContext = null;
		private ConnectorItem draggedOutConnectorItem = null;
		private object draggedOutNodeDataContext = null;
		private object draggingConnectionDataContext = null;

		private void ConnectorItem_DragCompleted(object source, ConnectorItemDragCompletedEventArgs e)
		{
			e.Handled = true;

			Trace.Assert((ConnectorItem)e.OriginalSource == this.draggedOutConnectorItem);

			Point mousePoint = Mouse.GetPosition(this);
			ConnectorItem connectorDraggedOver = null;
			object connectorDataContextDraggedOver = null;
			DetermineConnectorItemDraggedOver(mousePoint, out connectorDraggedOver, out connectorDataContextDraggedOver);
			RaiseEvent(new ConnectionDragCompletedEventArgs(ConnectionDragCompletedEvent, this, this.draggedOutNodeDataContext, this.draggingConnectionDataContext, this.draggedOutConnectorDataContext, connectorDataContextDraggedOver));

			this.IsDragging = false;
			this.IsNotDragging = true;
			this.IsDraggingConnection = false;
			this.IsNotDraggingConnection = true;
			this.draggedOutConnectorDataContext = null;
			this.draggedOutNodeDataContext = null;
			this.draggedOutConnectorItem = null;
			this.draggingConnectionDataContext = null;
		}

		private void ConnectorItem_Dragging(object source, ConnectorItemDraggingEventArgs e)
		{
			e.Handled = true;

			Trace.Assert((ConnectorItem)e.OriginalSource == this.draggedOutConnectorItem);

			Point mousePoint = Mouse.GetPosition(this);
			var connectionDraggingEventArgs =
				new ConnectionDraggingEventArgs(ConnectionDraggingEvent, this,
						this.draggedOutNodeDataContext, this.draggingConnectionDataContext,
						this.draggedOutConnectorDataContext);

			RaiseEvent(connectionDraggingEventArgs);
		}

		private void ConnectorItem_DragStarted(object source, ConnectorItemDragStartedEventArgs e)
		{
			this.Focus();

			e.Handled = true;

			this.IsDragging = true;
			this.IsNotDragging = false;
			this.IsDraggingConnection = true;
			this.IsNotDraggingConnection = false;

			this.draggedOutConnectorItem = (ConnectorItem)e.OriginalSource;
			var nodeItem = this.draggedOutConnectorItem.ParentNodeItem;
			this.draggedOutNodeDataContext = nodeItem.DataContext != null ? nodeItem.DataContext : nodeItem;
			this.draggedOutConnectorDataContext = this.draggedOutConnectorItem.DataContext != null ? this.draggedOutConnectorItem.DataContext : this.draggedOutConnectorItem;
			ConnectionDragStartedEventArgs eventArgs = new ConnectionDragStartedEventArgs(ConnectionDragStartedEvent, this, this.draggedOutNodeDataContext, this.draggedOutConnectorDataContext);
			RaiseEvent(eventArgs);
			this.draggingConnectionDataContext = eventArgs.Connection;

			if (draggingConnectionDataContext == null)
			{
				e.Cancel = true;
				return;
			}
		}

		private bool DetermineConnectorItemDraggedOver(Point hitPoint, out ConnectorItem connectorItemDraggedOver, out object connectorDataContextDraggedOver)
		{
			connectorItemDraggedOver = null;
			connectorDataContextDraggedOver = null;
			HitTestResult result = null;
			VisualTreeHelper.HitTest(nodeItemsControl, null,
				delegate (HitTestResult hitTestResult)
				{
					result = hitTestResult;

					return HitTestResultBehavior.Stop;
				},
				new PointHitTestParameters(hitPoint));

			if (result == null || result.VisualHit == null)
			{
				return false;
			}
			var hitItem = result.VisualHit as FrameworkElement;
			if (hitItem == null)
			{
				return false;
			}
			var connectorItem = WpfUtils.FindVisualParentWithType<ConnectorItem>(hitItem);
			if (connectorItem == null)
			{
				return false;
			}

			var networkView = connectorItem.ParentNetworkView;
			if (networkView != this)
			{
				return false;
			}

			object connectorDataContext = connectorItem;
			if (connectorItem.DataContext != null)
			{
				connectorDataContext = connectorItem.DataContext;
			}

			connectorItemDraggedOver = connectorItem;
			connectorDataContextDraggedOver = connectorDataContext;

			return true;
		}
	}
}
