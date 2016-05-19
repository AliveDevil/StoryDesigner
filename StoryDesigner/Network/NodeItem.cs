/* Copyright: Ashley Davis (http://www.codeproject.com/Articles/182683/NetworkView-A-WPF-custom-control-for-visualizing-a) */
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StoryDesigner.Network
{
	public class NodeItem : ListBoxItem
	{
		public static readonly DependencyProperty XProperty = DependencyProperty.Register("X", typeof(double), typeof(NodeItem), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static readonly DependencyProperty YProperty = DependencyProperty.Register("Y", typeof(double), typeof(NodeItem), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static readonly DependencyProperty ZIndexProperty = DependencyProperty.Register("ZIndex", typeof(int), typeof(NodeItem), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		internal static readonly RoutedEvent NodeDragCompletedEvent = EventManager.RegisterRoutedEvent("NodeDragCompleted", RoutingStrategy.Bubble, typeof(NodeDragCompletedEventHandler), typeof(NodeItem));
		internal static readonly RoutedEvent NodeDraggingEvent = EventManager.RegisterRoutedEvent("NodeDragging", RoutingStrategy.Bubble, typeof(NodeDraggingEventHandler), typeof(NodeItem));
		internal static readonly RoutedEvent NodeDragStartedEvent = EventManager.RegisterRoutedEvent("NodeDragStarted", RoutingStrategy.Bubble, typeof(NodeDragStartedEventHandler), typeof(NodeItem));
		internal static readonly DependencyProperty ParentNetworkViewProperty = DependencyProperty.Register("ParentNetworkView", typeof(NetworkView), typeof(NodeItem), new FrameworkPropertyMetadata(ParentNetworkView_PropertyChanged));
		private static readonly double DragThreshold = 5;
		private bool isDragging = false;
		private bool isLeftMouseAndControlDown = false;
		private bool isLeftMouseDown = false;
		private Point lastMousePoint;

		public double X
		{
			get { return (double)GetValue(XProperty); }
			set { SetValue(XProperty, value); }
		}

		public double Y
		{
			get { return (double)GetValue(YProperty); }
			set { SetValue(YProperty, value); }
		}

		public int ZIndex
		{
			get { return (int)GetValue(ZIndexProperty); }
			set { SetValue(ZIndexProperty, value); }
		}

		internal NetworkView ParentNetworkView
		{
			get { return (NetworkView)GetValue(ParentNetworkViewProperty); }
			set { SetValue(ParentNetworkViewProperty, value); }
		}

		static NodeItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeItem), new FrameworkPropertyMetadata(typeof(NodeItem)));
		}

		public NodeItem()
		{
			Focusable = false;
		}

		internal void BringToFront()
		{
			if (this.ParentNetworkView == null)
			{
				return;
			}

			int maxZ = this.ParentNetworkView.FindMaxZIndex();
			this.ZIndex = maxZ + 1;
		}

		internal void LeftMouseDownSelectionLogic()
		{
			if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
			{
				isLeftMouseAndControlDown = true;
			}
			else
			{
				isLeftMouseAndControlDown = false;

				if (this.ParentNetworkView.SelectedNodes.Count == 0)
				{
					this.IsSelected = true;
				}
				else if (this.ParentNetworkView.SelectedNodes.Contains(this) ||
						 this.ParentNetworkView.SelectedNodes.Contains(this.DataContext))
				{
				}
				else
				{
					this.ParentNetworkView.SelectedNodes.Clear();
					this.IsSelected = true;
				}
			}
		}

		internal void LeftMouseUpSelectionLogic()
		{
			if (isLeftMouseAndControlDown)
			{
				this.IsSelected = !this.IsSelected;
			}
			else
			{
				if (this.ParentNetworkView.SelectedNodes.Count == 1 &&
				 (this.ParentNetworkView.SelectedNode == this ||
				  this.ParentNetworkView.SelectedNode == this.DataContext))
				{
				}
				else
				{
					this.ParentNetworkView.SelectedNodes.Clear();
					this.IsSelected = true;
				}
			}

			isLeftMouseAndControlDown = false;
		}

		internal void RightMouseDownSelectionLogic()
		{
			if (this.ParentNetworkView.SelectedNodes.Count == 0)
			{
				this.IsSelected = true;
			}
			else if (this.ParentNetworkView.SelectedNodes.Contains(this) ||
					 this.ParentNetworkView.SelectedNodes.Contains(this.DataContext))
			{
			}
			else
			{
				this.ParentNetworkView.SelectedNodes.Clear();
				this.IsSelected = true;
			}
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);

			BringToFront();

			if (this.ParentNetworkView != null)
			{
				this.ParentNetworkView.Focus();
			}

			if (e.ChangedButton == MouseButton.Left && this.ParentNetworkView != null)
			{
				lastMousePoint = e.GetPosition(this.ParentNetworkView);
				isLeftMouseDown = true;

				LeftMouseDownSelectionLogic();

				e.Handled = true;
			}
			else if (e.ChangedButton == MouseButton.Right && this.ParentNetworkView != null)
			{
				RightMouseDownSelectionLogic();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (isDragging)
			{
				Point curMousePoint = e.GetPosition(this.ParentNetworkView);

				object item = this;
				if (DataContext != null)
				{
					item = DataContext;
				}

				Vector offset = curMousePoint - lastMousePoint;
				if (offset.X != 0.0 ||
					offset.Y != 0.0)
				{
					lastMousePoint = curMousePoint;

					RaiseEvent(new NodeDraggingEventArgs(NodeDraggingEvent, this, new object[] { item }, offset.X, offset.Y));
				}
			}
			else if (isLeftMouseDown && this.ParentNetworkView.EnableNodeDragging)
			{
				Point curMousePoint = e.GetPosition(this.ParentNetworkView);
				var dragDelta = curMousePoint - lastMousePoint;
				double dragDistance = Math.Abs(dragDelta.Length);
				if (dragDistance > DragThreshold)
				{
					NodeDragStartedEventArgs eventArgs = new NodeDragStartedEventArgs(NodeDragStartedEvent, this, new NodeItem[] { this });
					RaiseEvent(eventArgs);

					if (eventArgs.Cancel)
					{
						isLeftMouseDown = false;
						isLeftMouseAndControlDown = false;
						return;
					}

					isDragging = true;
					this.CaptureMouse();
					e.Handled = true;
				}
			}
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);

			if (isLeftMouseDown)
			{
				if (isDragging)
				{
					RaiseEvent(new NodeDragCompletedEventArgs(NodeDragCompletedEvent, this, new NodeItem[] { this }));

					this.ReleaseMouseCapture();

					isDragging = false;
				}
				else
				{
					LeftMouseUpSelectionLogic();
				}

				isLeftMouseDown = false;
				isLeftMouseAndControlDown = false;

				e.Handled = true;
			}
		}

		private static void ParentNetworkView_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var nodeItem = (NodeItem)o;
			nodeItem.BringToFront();
		}
	}
}
