/* Copyright: Ashley Davis (http://www.codeproject.com/Articles/182683/NetworkView-A-WPF-custom-control-for-visualizing-a) */
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StoryDesigner.Network
{
	public class ConnectorItem : ContentControl
	{
		public static readonly DependencyProperty DragEnabledProperty = DependencyProperty.Register("DragEnabled", typeof(bool), typeof(ConnectorItem), new PropertyMetadata(false));
		public static readonly DependencyProperty HotspotProperty = DependencyProperty.Register("Hotspot", typeof(Point), typeof(ConnectorItem));
		internal static readonly RoutedEvent ConnectorDragCompletedEvent = EventManager.RegisterRoutedEvent("ConnectorDragCompleted", RoutingStrategy.Bubble, typeof(ConnectorItemDragCompletedEventHandler), typeof(ConnectorItem));
		internal static readonly RoutedEvent ConnectorDraggingEvent = EventManager.RegisterRoutedEvent("ConnectorDragging", RoutingStrategy.Bubble, typeof(ConnectorItemDraggingEventHandler), typeof(ConnectorItem));
		internal static readonly RoutedEvent ConnectorDragStartedEvent = EventManager.RegisterRoutedEvent("ConnectorDragStarted", RoutingStrategy.Bubble, typeof(ConnectorItemDragStartedEventHandler), typeof(ConnectorItem));
		internal static readonly DependencyProperty ParentNetworkViewProperty = DependencyProperty.Register("ParentNetworkView", typeof(NetworkView), typeof(ConnectorItem), new FrameworkPropertyMetadata(ParentNetworkView_PropertyChanged));
		internal static readonly DependencyProperty ParentNodeItemProperty = DependencyProperty.Register("ParentNodeItem", typeof(NodeItem), typeof(ConnectorItem));
		private static readonly double DragThreshold = 2;

		private bool isDragging = false;
		private bool isLeftMouseDown = false;
		private Point lastMousePoint;

		public bool DragEnabled
		{
			get { return (bool)GetValue(DragEnabledProperty); }
			set { SetValue(DragEnabledProperty, value); }
		}

		public Point Hotspot
		{
			get { return (Point)GetValue(HotspotProperty); }
			set { SetValue(HotspotProperty, value); }
		}

		internal NetworkView ParentNetworkView
		{
			get { return (NetworkView)GetValue(ParentNetworkViewProperty); }
			set { SetValue(ParentNetworkViewProperty, value); }
		}

		internal NodeItem ParentNodeItem
		{
			get { return (NodeItem)GetValue(ParentNodeItemProperty); }
			set { SetValue(ParentNodeItemProperty, value); }
		}

		static ConnectorItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ConnectorItem), new FrameworkPropertyMetadata(typeof(ConnectorItem)));
		}

		public ConnectorItem()
		{
			Focusable = false;

			this.LayoutUpdated += new EventHandler(ConnectorItem_LayoutUpdated);
		}

		internal void CancelConnectionDragging()
		{
			if (isLeftMouseDown)
			{
				RaiseEvent(new ConnectorItemDragCompletedEventArgs(ConnectorDragCompletedEvent, null));

				isLeftMouseDown = false;
				this.ReleaseMouseCapture();
			}
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);

			if (this.ParentNodeItem != null)
			{
				this.ParentNodeItem.BringToFront();
			}

			if (this.ParentNetworkView != null)
			{
				this.ParentNetworkView.Focus();
			}

			if (e.ChangedButton == MouseButton.Left)
			{
				if (this.ParentNodeItem != null)
				{
					this.ParentNodeItem.LeftMouseDownSelectionLogic();
				}

				lastMousePoint = e.GetPosition(this.ParentNetworkView);
				isLeftMouseDown = true;
				e.Handled = true;
			}
			else if (e.ChangedButton == MouseButton.Right)
			{
				if (this.ParentNodeItem != null)
				{
					this.ParentNodeItem.RightMouseDownSelectionLogic();
				}
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (isDragging)
			{
				Point curMousePoint = e.GetPosition(this.ParentNetworkView);
				Vector offset = curMousePoint - lastMousePoint;
				if (offset.X != 0.0 &&
					offset.Y != 0.0)
				{
					lastMousePoint = curMousePoint;

					RaiseEvent(new ConnectorItemDraggingEventArgs(ConnectorDraggingEvent, this, offset.X, offset.Y));
				}

				e.Handled = true;
			}
			else if (isLeftMouseDown)
			{
				if (this.DragEnabled &&
					this.ParentNetworkView?.EnableConnectionDragging == true)
				{
					Point curMousePoint = e.GetPosition(this.ParentNetworkView);
					var dragDelta = curMousePoint - lastMousePoint;
					double dragDistance = Math.Abs(dragDelta.Length);
					if (dragDistance > DragThreshold)
					{
						var eventArgs = new ConnectorItemDragStartedEventArgs(ConnectorDragStartedEvent, this);
						RaiseEvent(eventArgs);

						if (eventArgs.Cancel)
						{
							isLeftMouseDown = false;
							return;
						}

						isDragging = true;
						this.CaptureMouse();
						e.Handled = true;
					}
				}
			}
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);

			if (e.ChangedButton == MouseButton.Left)
			{
				if (isLeftMouseDown)
				{
					if (isDragging)
					{
						RaiseEvent(new ConnectorItemDragCompletedEventArgs(ConnectorDragCompletedEvent, this));

						this.ReleaseMouseCapture();

						isDragging = false;
					}
					else
					{
						if (this.ParentNodeItem != null)
						{
							this.ParentNodeItem.LeftMouseUpSelectionLogic();
						}
					}

					isLeftMouseDown = false;

					e.Handled = true;
				}
			}
		}

		private static void ParentNetworkView_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ConnectorItem c = (ConnectorItem)d;
			c.UpdateHotspot();
		}

		private void ConnectorItem_LayoutUpdated(object sender, EventArgs e)
		{
			UpdateHotspot();
		}

		private void UpdateHotspot()
		{
			if (this.ParentNetworkView == null)
			{
				return;
			}

			if (!this.ParentNetworkView.IsAncestorOf(this))
			{
				this.ParentNetworkView = null;
				return;
			}

			var centerPoint = new Point(this.ActualWidth / 2, this.ActualHeight / 2);

			this.Hotspot = this.TransformToAncestor(this.ParentNetworkView).Transform(centerPoint);
		}
	}
}
