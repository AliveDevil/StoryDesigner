/* Copyright: Ashley Davis (http://www.codeproject.com/Articles/182683/NetworkView-A-WPF-custom-control-for-visualizing-a) */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using StoryDesigner.Collections;

namespace StoryDesigner.Network
{
	public partial class NetworkView : Control
	{
		public static readonly RoutedCommand CancelConnectionDraggingCommand;
		public static readonly RoutedEvent ConnectionDragCompletedEvent;
		public static readonly RoutedEvent ConnectionDraggingEvent;
		public static readonly RoutedEvent ConnectionDragStartedEvent;
		public static readonly DependencyProperty ConnectionItemContainerStyleProperty;
		public static readonly DependencyProperty ConnectionItemTemplateProperty;
		public static readonly DependencyProperty ConnectionItemTemplateSelectorProperty;
		public static readonly DependencyProperty ConnectionsProperty;
		public static readonly DependencyProperty ConnectionsSourceProperty;
		public static readonly DependencyProperty EnableConnectionDraggingProperty;
		public static readonly DependencyProperty EnableNodeDraggingProperty;
		public static readonly RoutedCommand InvertSelectionCommand;
		public static readonly DependencyProperty IsClearSelectionOnEmptySpaceClickEnabledProperty;
		public static readonly DependencyProperty IsDraggingConnectionProperty;
		public static readonly DependencyProperty IsDraggingNodeProperty;
		public static readonly DependencyProperty IsDraggingProperty;
		public static readonly DependencyProperty IsNotDraggingConnectionProperty;
		public static readonly DependencyProperty IsNotDraggingNodeProperty;
		public static readonly DependencyProperty IsNotDraggingProperty;
		public static readonly RoutedEvent NodeDragCompletedEvent;
		public static readonly RoutedEvent NodeDraggingEvent;
		public static readonly RoutedEvent NodeDragStartedEvent;
		public static readonly DependencyProperty NodeItemContainerStyleProperty;
		public static readonly DependencyProperty NodeItemTemplateProperty;
		public static readonly DependencyProperty NodeItemTemplateSelectorProperty;
		public static readonly DependencyProperty NodesProperty;
		public static readonly DependencyProperty NodesSourceProperty;
		public static readonly RoutedEvent QueryConnectionFeedbackEvent;
		public static readonly RoutedCommand SelectAllCommand;
		public static readonly RoutedCommand SelectNoneCommand;
		private static readonly DependencyPropertyKey ConnectionsPropertyKey;
		private static readonly DependencyPropertyKey IsDraggingConnectionPropertyKey;
		private static readonly DependencyPropertyKey IsDraggingNodePropertyKey;
		private static readonly DependencyPropertyKey IsDraggingPropertyKey;
		private static readonly DependencyPropertyKey IsNotDraggingConnectionPropertyKey;
		private static readonly DependencyPropertyKey IsNotDraggingNodePropertyKey;
		private static readonly DependencyPropertyKey IsNotDraggingPropertyKey;
		private static readonly DependencyPropertyKey NodesPropertyKey;
		private ItemsControl connectionItemsControl;
		private List<object> initialSelectedNodes;
		private NodeItemsControl nodeItemsControl;

		public event ConnectionDragCompletedEventHandler ConnectionDragCompleted
		{
			add { AddHandler(ConnectionDragCompletedEvent, value); }
			remove { RemoveHandler(ConnectionDragCompletedEvent, value); }
		}

		public event ConnectionDraggingEventHandler ConnectionDragging
		{
			add { AddHandler(ConnectionDraggingEvent, value); }
			remove { RemoveHandler(ConnectionDraggingEvent, value); }
		}

		public event ConnectionDragStartedEventHandler ConnectionDragStarted
		{
			add { AddHandler(ConnectionDragStartedEvent, value); }
			remove { RemoveHandler(ConnectionDragStartedEvent, value); }
		}

		public event NodeDragCompletedEventHandler NodeDragCompleted
		{
			add { AddHandler(NodeDragCompletedEvent, value); }
			remove { RemoveHandler(NodeDragCompletedEvent, value); }
		}

		public event NodeDraggingEventHandler NodeDragging
		{
			add { AddHandler(NodeDraggingEvent, value); }
			remove { RemoveHandler(NodeDraggingEvent, value); }
		}

		public event NodeDragStartedEventHandler NodeDragStarted
		{
			add { AddHandler(NodeDragStartedEvent, value); }
			remove { RemoveHandler(NodeDragStartedEvent, value); }
		}

		public event QueryConnectionFeedbackEventHandler QueryConnectionFeedback
		{
			add { AddHandler(QueryConnectionFeedbackEvent, value); }
			remove { RemoveHandler(QueryConnectionFeedbackEvent, value); }
		}

		public event SelectionChangedEventHandler SelectionChanged;

		public Style ConnectionItemContainerStyle
		{
			get { return (Style)GetValue(ConnectionItemContainerStyleProperty); }
			set { SetValue(ConnectionItemContainerStyleProperty, value); }
		}

		public DataTemplate ConnectionItemTemplate
		{
			get { return (DataTemplate)GetValue(ConnectionItemTemplateProperty); }
			set { SetValue(ConnectionItemTemplateProperty, value); }
		}

		public DataTemplateSelector ConnectionItemTemplateSelector
		{
			get { return (DataTemplateSelector)GetValue(ConnectionItemTemplateSelectorProperty); }
			set { SetValue(ConnectionItemTemplateSelectorProperty, value); }
		}

		public ImpObservableCollection<object> Connections
		{
			get { return (ImpObservableCollection<object>)GetValue(ConnectionsProperty); }
			private set { SetValue(ConnectionsPropertyKey, value); }
		}

		public IEnumerable ConnectionsSource
		{
			get { return (IEnumerable)GetValue(ConnectionsSourceProperty); }
			set { SetValue(ConnectionsSourceProperty, value); }
		}

		public bool EnableConnectionDragging
		{
			get { return (bool)GetValue(EnableConnectionDraggingProperty); }
			set { SetValue(EnableConnectionDraggingProperty, value); }
		}

		public bool EnableNodeDragging
		{
			get { return (bool)GetValue(EnableNodeDraggingProperty); }
			set { SetValue(EnableNodeDraggingProperty, value); }
		}

		public bool IsClearSelectionOnEmptySpaceClickEnabled
		{
			get { return (bool)GetValue(IsClearSelectionOnEmptySpaceClickEnabledProperty); }
			set { SetValue(IsClearSelectionOnEmptySpaceClickEnabledProperty, value); }
		}

		public bool IsDragging
		{
			get { return (bool)GetValue(IsDraggingProperty); }
			private set { SetValue(IsDraggingPropertyKey, value); }
		}

		public bool IsDraggingConnection
		{
			get { return (bool)GetValue(IsDraggingConnectionProperty); }
			private set { SetValue(IsDraggingConnectionPropertyKey, value); }
		}

		public bool IsDraggingNode
		{
			get { return (bool)GetValue(IsDraggingNodeProperty); }
			private set { SetValue(IsDraggingNodePropertyKey, value); }
		}

		public bool IsNotDragging
		{
			get { return (bool)GetValue(IsNotDraggingProperty); }
			private set { SetValue(IsNotDraggingPropertyKey, value); }
		}

		public bool IsNotDraggingConnection
		{
			get { return (bool)GetValue(IsNotDraggingConnectionProperty); }
			private set { SetValue(IsNotDraggingConnectionPropertyKey, value); }
		}

		public bool IsNotDraggingNode
		{
			get { return (bool)GetValue(IsNotDraggingNodeProperty); }
			private set { SetValue(IsNotDraggingNodePropertyKey, value); }
		}

		public Style NodeItemContainerStyle
		{
			get { return (Style)GetValue(NodeItemContainerStyleProperty); }
			set { SetValue(NodeItemContainerStyleProperty, value); }
		}

		public DataTemplate NodeItemTemplate
		{
			get { return (DataTemplate)GetValue(NodeItemTemplateProperty); }
			set { SetValue(NodeItemTemplateProperty, value); }
		}

		public DataTemplateSelector NodeItemTemplateSelector
		{
			get { return (DataTemplateSelector)GetValue(NodeItemTemplateSelectorProperty); }
			set { SetValue(NodeItemTemplateSelectorProperty, value); }
		}

		public ImpObservableCollection<object> Nodes
		{
			get { return (ImpObservableCollection<object>)GetValue(NodesProperty); }
			private set { SetValue(NodesPropertyKey, value); }
		}

		public IEnumerable NodesSource
		{
			get { return (IEnumerable)GetValue(NodesSourceProperty); }
			set { SetValue(NodesSourceProperty, value); }
		}

		public object SelectedNode
		{
			get
			{
				if (nodeItemsControl != null)
				{
					return nodeItemsControl.SelectedItem;
				}
				else
				{
					if (initialSelectedNodes == null)
					{
						return null;
					}

					if (initialSelectedNodes.Count != 1)
					{
						return null;
					}

					return initialSelectedNodes[0];
				}
			}
			set
			{
				if (nodeItemsControl != null)
				{
					nodeItemsControl.SelectedItem = value;
				}
				else
				{
					if (initialSelectedNodes == null)
					{
						initialSelectedNodes = new List<object>();
					}

					initialSelectedNodes.Clear();
					initialSelectedNodes.Add(value);
				}
			}
		}

		public IList SelectedNodes
		{
			get
			{
				if (nodeItemsControl != null)
				{
					return nodeItemsControl.SelectedItems;
				}
				else
				{
					if (initialSelectedNodes == null)
					{
						initialSelectedNodes = new List<object>();
					}

					return initialSelectedNodes;
				}
			}
		}

		static NetworkView()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NetworkView), new FrameworkPropertyMetadata(typeof(NetworkView)));

			ConnectionsPropertyKey = DependencyProperty.RegisterReadOnly("Connections", typeof(ImpObservableCollection<object>), typeof(NetworkView), new FrameworkPropertyMetadata());
			IsDraggingConnectionPropertyKey = DependencyProperty.RegisterReadOnly("IsDraggingConnection", typeof(bool), typeof(NetworkView), new FrameworkPropertyMetadata(false));
			IsDraggingNodePropertyKey = DependencyProperty.RegisterReadOnly("IsDraggingNode", typeof(bool), typeof(NetworkView), new FrameworkPropertyMetadata(false));
			IsDraggingPropertyKey = DependencyProperty.RegisterReadOnly("IsDragging", typeof(bool), typeof(NetworkView), new FrameworkPropertyMetadata(false));
			IsNotDraggingConnectionPropertyKey = DependencyProperty.RegisterReadOnly("IsNotDraggingConnection", typeof(bool), typeof(NetworkView), new FrameworkPropertyMetadata(true));
			IsNotDraggingNodePropertyKey = DependencyProperty.RegisterReadOnly("IsNotDraggingNode", typeof(bool), typeof(NetworkView), new FrameworkPropertyMetadata(true));
			IsNotDraggingPropertyKey = DependencyProperty.RegisterReadOnly("IsNotDragging", typeof(bool), typeof(NetworkView), new FrameworkPropertyMetadata(true));
			NodesPropertyKey = DependencyProperty.RegisterReadOnly("Nodes", typeof(ImpObservableCollection<object>), typeof(NetworkView), new FrameworkPropertyMetadata());

			CancelConnectionDraggingCommand = null;
			InvertSelectionCommand = null;
			SelectAllCommand = null;
			SelectNoneCommand = null;

			ConnectionsProperty = ConnectionsPropertyKey.DependencyProperty;
			IsDraggingConnectionProperty = IsDraggingConnectionPropertyKey.DependencyProperty;
			IsDraggingNodeProperty = IsDraggingNodePropertyKey.DependencyProperty;
			IsDraggingProperty = IsDraggingPropertyKey.DependencyProperty;
			IsNotDraggingConnectionProperty = IsNotDraggingConnectionPropertyKey.DependencyProperty;
			IsNotDraggingNodeProperty = IsDraggingNodePropertyKey.DependencyProperty;
			IsNotDraggingProperty = IsNotDraggingPropertyKey.DependencyProperty;
			NodesProperty = NodesPropertyKey.DependencyProperty;

			ConnectionDragCompletedEvent = EventManager.RegisterRoutedEvent("ConnectionDragCompleted", RoutingStrategy.Bubble, typeof(ConnectionDragCompletedEventHandler), typeof(NetworkView));
			ConnectionDraggingEvent = EventManager.RegisterRoutedEvent("ConnectionDragging", RoutingStrategy.Bubble, typeof(ConnectionDraggingEventHandler), typeof(NetworkView));
			ConnectionDragStartedEvent = EventManager.RegisterRoutedEvent("ConnectionDragStarted", RoutingStrategy.Bubble, typeof(ConnectionDragStartedEventHandler), typeof(NetworkView));
			ConnectionItemContainerStyleProperty = DependencyProperty.Register("ConnectionItemContainerStyle", typeof(Style), typeof(NetworkView));
			ConnectionItemTemplateProperty = DependencyProperty.Register("ConnectionItemTemplate", typeof(DataTemplate), typeof(NetworkView));
			ConnectionItemTemplateSelectorProperty = DependencyProperty.Register("ConnectionItemTemplateSelector", typeof(DataTemplateSelector), typeof(NetworkView));
			ConnectionsSourceProperty = DependencyProperty.Register("ConnectionsSource", typeof(IEnumerable), typeof(NetworkView), new FrameworkPropertyMetadata(ConnectionsSource_PropertyChanged));
			EnableConnectionDraggingProperty = DependencyProperty.Register("EnableConnectionDragging", typeof(bool), typeof(NetworkView), new FrameworkPropertyMetadata(true));
			EnableNodeDraggingProperty = DependencyProperty.Register("EnableNodeDragging", typeof(bool), typeof(NetworkView), new FrameworkPropertyMetadata(true));
			IsClearSelectionOnEmptySpaceClickEnabledProperty = DependencyProperty.Register("IsClearSelectionOnEmptySpaceClickEnabled", typeof(bool), typeof(NetworkView), new FrameworkPropertyMetadata(true));
			NodeDragCompletedEvent = EventManager.RegisterRoutedEvent("NodeDragCompleted", RoutingStrategy.Bubble, typeof(NodeDragCompletedEventHandler), typeof(NetworkView));
			NodeDraggingEvent = EventManager.RegisterRoutedEvent("NodeDragging", RoutingStrategy.Bubble, typeof(NodeDraggingEventHandler), typeof(NetworkView));
			NodeDragStartedEvent = EventManager.RegisterRoutedEvent("NodeDragStarted", RoutingStrategy.Bubble, typeof(NodeDragStartedEventHandler), typeof(NetworkView));
			NodeItemContainerStyleProperty = DependencyProperty.Register("NodeItemContainerStyle", typeof(Style), typeof(NetworkView));
			NodeItemTemplateProperty = DependencyProperty.Register("NodeItemTemplate", typeof(DataTemplate), typeof(NetworkView));
			NodeItemTemplateSelectorProperty = DependencyProperty.Register("NodeItemTemplateSelector", typeof(DataTemplateSelector), typeof(NetworkView));
			NodesSourceProperty = DependencyProperty.Register("NodesSource", typeof(IEnumerable), typeof(NetworkView), new FrameworkPropertyMetadata(NodesSource_PropertyChanged));
			QueryConnectionFeedbackEvent = EventManager.RegisterRoutedEvent("QueryConnectionFeedback", RoutingStrategy.Bubble, typeof(QueryConnectionFeedbackEventHandler), typeof(NetworkView));

			InputGestureCollection inputs = new InputGestureCollection();
			inputs.Add(new KeyGesture(Key.A, ModifierKeys.Control));
			SelectAllCommand = new RoutedCommand("SelectAll", typeof(NetworkView), inputs);

			inputs = new InputGestureCollection();
			inputs.Add(new KeyGesture(Key.Escape));
			SelectNoneCommand = new RoutedCommand("SelectNone", typeof(NetworkView), inputs);

			inputs = new InputGestureCollection();
			inputs.Add(new KeyGesture(Key.I, ModifierKeys.Control));
			InvertSelectionCommand = new RoutedCommand("InvertSelection", typeof(NetworkView), inputs);

			CancelConnectionDraggingCommand = new RoutedCommand("CancelConnectionDragging", typeof(NetworkView));

			CommandBinding binding = new CommandBinding();
			binding.Command = SelectAllCommand;
			binding.Executed += new ExecutedRoutedEventHandler(SelectAll_Executed);
			CommandManager.RegisterClassCommandBinding(typeof(NetworkView), binding);

			binding = new CommandBinding();
			binding.Command = SelectNoneCommand;
			binding.Executed += new ExecutedRoutedEventHandler(SelectNone_Executed);
			CommandManager.RegisterClassCommandBinding(typeof(NetworkView), binding);

			binding = new CommandBinding();
			binding.Command = InvertSelectionCommand;
			binding.Executed += new ExecutedRoutedEventHandler(InvertSelection_Executed);
			CommandManager.RegisterClassCommandBinding(typeof(NetworkView), binding);

			binding = new CommandBinding();
			binding.Command = CancelConnectionDraggingCommand;
			binding.Executed += new ExecutedRoutedEventHandler(CancelConnectionDragging_Executed);
			CommandManager.RegisterClassCommandBinding(typeof(NetworkView), binding);
		}

		public NetworkView()
		{
			this.Nodes = new ImpObservableCollection<object>();

			this.Connections = new ImpObservableCollection<object>();

			this.Background = Brushes.White;

			AddHandler(NodeItem.NodeDragStartedEvent, new NodeDragStartedEventHandler(NodeItem_DragStarted));
			AddHandler(NodeItem.NodeDraggingEvent, new NodeDraggingEventHandler(NodeItem_Dragging));
			AddHandler(NodeItem.NodeDragCompletedEvent, new NodeDragCompletedEventHandler(NodeItem_DragCompleted));
			AddHandler(ConnectorItem.ConnectorDragStartedEvent, new ConnectorItemDragStartedEventHandler(ConnectorItem_DragStarted));
			AddHandler(ConnectorItem.ConnectorDraggingEvent, new ConnectorItemDraggingEventHandler(ConnectorItem_Dragging));
			AddHandler(ConnectorItem.ConnectorDragCompletedEvent, new ConnectorItemDragCompletedEventHandler(ConnectorItem_DragCompleted));
		}

		public void BringNodesIntoView(ICollection nodes)
		{
			if (nodes == null)
			{
				throw new ArgumentNullException("'nodes' argument shouldn't be null.");
			}

			if (nodes.Count == 0)
			{
				return;
			}

			Rect rect = Rect.Empty;

			foreach (var node in nodes)
			{
				NodeItem nodeItem = FindAssociatedNodeItem(node);
				Rect nodeRect = new Rect(nodeItem.X, nodeItem.Y, nodeItem.ActualWidth, nodeItem.ActualHeight);

				if (rect == Rect.Empty)
				{
					rect = nodeRect;
				}
				else
				{
					rect.Intersect(nodeRect);
				}
			}

			this.BringIntoView(rect);
		}

		public void BringSelectedNodesIntoView()
		{
			BringNodesIntoView(SelectedNodes);
		}

		public void CancelConnectionDragging()
		{
			if (!this.IsDraggingConnection)
			{
				return;
			}

			//ClearFeedbackAdorner();

			draggedOutConnectorItem.CancelConnectionDragging();

			this.IsDragging = false;
			this.IsNotDragging = true;
			this.IsDraggingConnection = false;
			this.IsNotDraggingConnection = true;
			this.draggedOutConnectorItem = null;
			this.draggedOutNodeDataContext = null;
			this.draggedOutConnectorDataContext = null;
			this.draggingConnectionDataContext = null;
		}

		public void InvertSelection()
		{
			var selectedNodesCopy = new ArrayList(this.SelectedNodes);
			this.SelectedNodes.Clear();

			foreach (var node in this.Nodes)
			{
				if (!selectedNodesCopy.Contains(node))
				{
					this.SelectedNodes.Add(node);
				}
			}
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.nodeItemsControl = (NodeItemsControl)this.Template.FindName("PART_NodeItemsControl", this);
			if (this.nodeItemsControl == null)
			{
				throw new ApplicationException("Failed to find 'PART_NodeItemsControl' in the visual tree for 'NetworkView'.");
			}

			if (this.initialSelectedNodes != null && this.initialSelectedNodes.Count > 0)
			{
				foreach (var node in this.initialSelectedNodes)
				{
					this.nodeItemsControl.SelectedItems.Add(node);
				}
			}

			this.initialSelectedNodes = null; // Don't need this any more.

			this.nodeItemsControl.SelectionChanged += new SelectionChangedEventHandler(nodeItemsControl_SelectionChanged);

			this.connectionItemsControl = (ItemsControl)this.Template.FindName("PART_ConnectionItemsControl", this);
			if (this.connectionItemsControl == null)
			{
				throw new ApplicationException("Failed to find 'PART_ConnectionItemsControl' in the visual tree for 'NetworkView'.");
			}
		}

		public void SelectAll()
		{
			if (this.SelectedNodes.Count != this.Nodes.Count)
			{
				this.SelectedNodes.Clear();
				foreach (var node in this.Nodes)
				{
					this.SelectedNodes.Add(node);
				}
			}
		}

		public Point Camera
		{
			get { return (Point)GetValue(CameraProperty); }
			set { SetValue(CameraProperty, value); }
		}

		public static readonly DependencyProperty CameraProperty = DependencyProperty.Register("Camera", typeof(Point), typeof(NetworkView), new FrameworkPropertyMetadata(new Point(), FrameworkPropertyMetadataOptions.AffectsRender));

		public void SelectNone()
		{
			this.SelectedNodes.Clear();
		}

		internal NodeItem FindAssociatedNodeItem(object node)
		{
			NodeItem nodeItem = node as NodeItem;
			if (nodeItem == null)
			{
				nodeItem = nodeItemsControl.FindAssociatedNodeItem(node);
			}
			return nodeItem;
		}

		internal int FindMaxZIndex()
		{
			if (this.nodeItemsControl == null)
			{
				return 0;
			}

			int maxZ = 0;

			for (int nodeIndex = 0; ; ++nodeIndex)
			{
				NodeItem nodeItem = (NodeItem)this.nodeItemsControl.ItemContainerGenerator.ContainerFromIndex(nodeIndex);
				if (nodeItem == null)
				{
					break;
				}

				if (nodeItem.ZIndex > maxZ)
				{
					maxZ = nodeItem.ZIndex;
				}
			}

			return maxZ;
		}

		private static void CancelConnectionDragging_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			NetworkView c = (NetworkView)sender;
			c.CancelConnectionDragging();
		}

		private static void ConnectionsSource_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			NetworkView c = (NetworkView)d;

			c.Connections.Clear();

			if (e.OldValue != null)
			{
				INotifyCollectionChanged notifyCollectionChanged = e.NewValue as INotifyCollectionChanged;
				if (notifyCollectionChanged != null)
				{
					notifyCollectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(c.ConnectionsSource_CollectionChanged);
				}
			}

			if (e.NewValue != null)
			{
				IEnumerable enumerable = e.NewValue as IEnumerable;
				if (enumerable != null)
				{
					foreach (object obj in enumerable)
					{
						c.Connections.Add(obj);
					}
				}

				INotifyCollectionChanged notifyCollectionChanged = e.NewValue as INotifyCollectionChanged;
				if (notifyCollectionChanged != null)
				{
					notifyCollectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(c.ConnectionsSource_CollectionChanged);
				}
			}
		}

		private static void InvertSelection_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			NetworkView c = (NetworkView)sender;
			c.InvertSelection();
		}

		private static void NodesSource_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			NetworkView c = (NetworkView)d;

			c.Nodes.Clear();

			if (e.OldValue != null)
			{
				var notifyCollectionChanged = e.OldValue as INotifyCollectionChanged;
				if (notifyCollectionChanged != null)
				{
					notifyCollectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(c.NodesSource_CollectionChanged);
				}
			}

			if (e.NewValue != null)
			{
				var enumerable = e.NewValue as IEnumerable;
				if (enumerable != null)
				{
					foreach (object obj in enumerable)
					{
						c.Nodes.Add(obj);
					}
				}

				var notifyCollectionChanged = e.NewValue as INotifyCollectionChanged;
				if (notifyCollectionChanged != null)
				{
					notifyCollectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(c.NodesSource_CollectionChanged);
				}
			}
		}

		private static void SelectAll_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			NetworkView c = (NetworkView)sender;
			c.SelectAll();
		}

		private static void SelectNone_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			NetworkView c = (NetworkView)sender;
			c.SelectNone();
		}

		private void ConnectionsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				Connections.Clear();
			}
			else
			{
				if (e.OldItems != null)
				{
					foreach (object obj in e.OldItems)
					{
						Connections.Remove(obj);
					}
				}

				if (e.NewItems != null)
				{
					foreach (object obj in e.NewItems)
					{
						Connections.Add(obj);
					}
				}
			}
		}

		private void nodeItemsControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.SelectionChanged != null)
			{
				this.SelectionChanged(this, new SelectionChangedEventArgs(ListBox.SelectionChangedEvent, e.RemovedItems, e.AddedItems));
			}
		}

		private void NodesSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				Nodes.Clear();
			}
			else
			{
				if (e.OldItems != null)
				{
					foreach (object obj in e.OldItems)
					{
						Nodes.Remove(obj);
					}
				}

				if (e.NewItems != null)
				{
					foreach (object obj in e.NewItems)
					{
						Nodes.Add(obj);
					}
				}
			}
		}
	}
}
