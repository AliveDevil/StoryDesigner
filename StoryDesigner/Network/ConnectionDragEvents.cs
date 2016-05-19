/* Copyright: Ashley Davis (http://www.codeproject.com/Articles/182683/NetworkView-A-WPF-custom-control-for-visualizing-a) */
using System.Windows;

namespace StoryDesigner.Network
{
	public delegate void ConnectionDragCompletedEventHandler(object sender, ConnectionDragCompletedEventArgs e);

	public delegate void ConnectionDraggingEventHandler(object sender, ConnectionDraggingEventArgs e);

	public delegate void ConnectionDragStartedEventHandler(object sender, ConnectionDragStartedEventArgs e);

	public delegate void QueryConnectionFeedbackEventHandler(object sender, QueryConnectionFeedbackEventArgs e);

	public class ConnectionDragCompletedEventArgs : ConnectionDragEventArgs
	{
		public new object Connection => base.Connection;

		public object DraggedOverConnector { get; }

		internal ConnectionDragCompletedEventArgs(RoutedEvent routedEvent, object source, object node, object connection, object connector, object connectorDraggedOver) :
			base(routedEvent, source, node, connection, connector)
		{
			this.DraggedOverConnector = connectorDraggedOver;
		}
	}

	public class ConnectionDragEventArgs : RoutedEventArgs
	{
		protected object Connection { get; set; }

		public object DraggedOutConnector { get; }

		public object Node { get; }

		protected ConnectionDragEventArgs(RoutedEvent routedEvent, object source, object node, object connection, object connector) :
			base(routedEvent, source)
		{
			this.Node = node;
			this.DraggedOutConnector = connector;
			this.Connection = connection;
		}
	}

	public class ConnectionDraggingEventArgs : ConnectionDragEventArgs
	{
		public new object Connection => base.Connection;

		internal ConnectionDraggingEventArgs(RoutedEvent routedEvent, object source,
				object node, object connection, object connector) :
			base(routedEvent, source, node, connection, connector)
		{
		}
	}

	public class ConnectionDragStartedEventArgs : ConnectionDragEventArgs
	{
		public new object Connection
		{
			get { return base.Connection; }
			set { base.Connection = value; }
		}

		internal ConnectionDragStartedEventArgs(RoutedEvent routedEvent, object source, object node, object connector) :
			base(routedEvent, source, node, null, connector)
		{
		}
	}

	public class QueryConnectionFeedbackEventArgs : ConnectionDragEventArgs
	{
		public new object Connection => base.Connection;

		public bool ConnectionOk { get; set; } = true;

		public object DraggedOverConnector { get; }

		public object FeedbackIndicator { get; set; }

		internal QueryConnectionFeedbackEventArgs(RoutedEvent routedEvent, object source, object node, object connection, object connector, object draggedOverConnector) : base(routedEvent, source, node, connection, connector)
		{
			this.DraggedOverConnector = draggedOverConnector;
		}
	}
}
