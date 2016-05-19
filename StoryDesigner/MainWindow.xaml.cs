using System.Windows;
using System.Windows.Input;
using StoryDesigner.Network;
using StoryDesigner.ViewModel;

namespace StoryDesigner
{
	public partial class MainWindow : Window
	{
		protected MainViewModel ViewModel => (MainViewModel)DataContext;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void networkControl_ConnectionDragStarted(object sender, ConnectionDragStartedEventArgs e)
		{
			var draggedOutConnector = (StoryConditionViewModel)e.DraggedOutConnector;
			var curDragPoint = Mouse.GetPosition(networkControl);
			var connection = this.ViewModel.ConnectionDragStarted(draggedOutConnector, curDragPoint);
			e.Connection = connection;
		}
		private void networkControl_ConnectionDragging(object sender, ConnectionDraggingEventArgs e)
		{
			Point curDragPoint = Mouse.GetPosition(networkControl);
			var connection = (StoryConnectionViewModel)e.Connection;
			this.ViewModel.ConnectionDragging(curDragPoint, connection);
		}
		private void networkControl_ConnectionDragCompleted(object sender, ConnectionDragCompletedEventArgs e)
		{
			var connectorDraggedOut = (StoryConditionViewModel)e.DraggedOutConnector;
			var connectorDraggedOver = (StoryNodeViewModel)e.DraggedOverConnector;
			var newConnection = (StoryConnectionViewModel)e.Connection;
			this.ViewModel.ConnectionDragCompleted(newConnection, connectorDraggedOut, connectorDraggedOver);
		}

		private void Node_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var element = (FrameworkElement)sender;
			var node = (StoryNodeViewModel)element.DataContext;
			node.Size = new Size(element.ActualWidth, element.ActualHeight);
		}
	}
}
