/* Copyright: Ashley Davis (http://www.codeproject.com/Articles/182683/NetworkView-A-WPF-custom-control-for-visualizing-a) */
using System.Collections;
using System.Windows;

namespace StoryDesigner.Network
{
	public delegate void NodeDragCompletedEventHandler(object sender, NodeDragCompletedEventArgs e);

	public delegate void NodeDragEventHandler(object sender, NodeDragEventArgs e);

	public delegate void NodeDraggingEventHandler(object sender, NodeDraggingEventArgs e);

	public delegate void NodeDragStartedEventHandler(object sender, NodeDragStartedEventArgs e);

	public class NodeDragCompletedEventArgs : NodeDragEventArgs
	{
		public NodeDragCompletedEventArgs(RoutedEvent routedEvent, object source, ICollection nodes) :
			base(routedEvent, source, nodes)
		{
		}
	}

	public class NodeDragEventArgs : RoutedEventArgs
	{
		public ICollection Nodes { get; }

		protected NodeDragEventArgs(RoutedEvent routedEvent, object source, ICollection nodes) :
					base(routedEvent, source)
		{
			this.Nodes = nodes;
		}
	}

	public class NodeDraggingEventArgs : NodeDragEventArgs
	{
		public double HorizontalChange { get; }

		public double VerticalChange { get; }

		internal NodeDraggingEventArgs(RoutedEvent routedEvent, object source, ICollection nodes, double horizontalChange, double verticalChange) :
							base(routedEvent, source, nodes)
		{
			this.HorizontalChange = horizontalChange;
			this.VerticalChange = verticalChange;
		}
	}

	public class NodeDragStartedEventArgs : NodeDragEventArgs
	{
		public bool Cancel { get; set; }

		internal NodeDragStartedEventArgs(RoutedEvent routedEvent, object source, ICollection nodes) :
					base(routedEvent, source, nodes)
		{
		}
	}
}
