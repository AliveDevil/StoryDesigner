/* Copyright: Jöran Malek */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using Microsoft.Win32;
using StoryDesigner.Network;

namespace StoryDesigner.ViewModel
{
	public class MainViewModel : ViewModelBase
	{
		private RelayCommand<IInputElement> createNodeCommand;
		private RelayCommand deleteNodeCommand;
		private string filename;
		private RelayCommand openCommand;
		private RelayCommand saveAsCommand;
		private RelayCommand saveCommand;

		/// <summary>
		/// Command executed on creating a new node.
		/// </summary>
		public RelayCommand<IInputElement> CreateNodeCommand
		{
			get
			{
				return createNodeCommand ?? (createNodeCommand = new RelayCommand<IInputElement>(ui =>
				{
					Story.CreateNode("New Node", "New Description", (Point)(Mouse.GetPosition(ui) - ((NetworkView)ui).Camera), true);
				}));
			}
		}

		/// <summary>
		/// Deletes a node.
		/// </summary>
		public RelayCommand DeleteNodeCommand
		{
			get
			{
				return deleteNodeCommand ?? (deleteNodeCommand = new RelayCommand(() =>
				  {
					  var nodesCopy = this.Story.Nodes.ToArray();
					  foreach (var node in nodesCopy)
					  {
						  if (node.IsSelected)
						  {
							  Story.DeleteNode(node);
						  }
					  }
				  }));
			}
		}

		/// <summary>
		/// Opens a StoryDesigner-Project
		/// </summary>
		public RelayCommand OpenCommand
		{
			get
			{
				return openCommand ?? (openCommand = new RelayCommand(() =>
				{
					OpenFileDialog dialog = new OpenFileDialog()
					{
						AddExtension = true,
						CheckFileExists = true,
						CheckPathExists = true,
						DereferenceLinks = true,
						DefaultExt = ".sdp",
						Filter = "StoryDesigner (.sdp)|*.sdp",
						InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
						Multiselect = false,
						ReadOnlyChecked = false,
						RestoreDirectory = true,
						ShowReadOnly = false,
						Title = "Choose file"
					};
					if (dialog.ShowDialog() == true && File.Exists(dialog.FileName))
					{
						LoadStoryDesigner(filename = dialog.FileName);
					}
				}));
			}
		}

		/// <summary>
		/// Saves everything.
		/// </summary>
		public RelayCommand SaveAsCommand
		{
			get
			{
				return saveAsCommand ?? (saveAsCommand = new RelayCommand(() =>
				{
					string saveFilename = ShowSaveDialog("StoryDesigner File (.sdp)|*.sdp|Story File (.story)|*.story");
					switch (Path.GetExtension(saveFilename))
					{
						case ".sdp":
							SaveStoryDesigner(saveFilename);
							break;

						case ".story":
							SaveStory(saveFilename);
							break;
					}
				}));
			}
		}

		/// <summary>
		/// Saves everything as StoryDesigner-Project
		/// </summary>
		public RelayCommand SaveCommand
		{
			get
			{
				return saveCommand ?? (saveCommand = new RelayCommand(() =>
				{
					if (string.IsNullOrEmpty(filename))
					{
						string saveFilename = ShowSaveDialog("StoryDesigner File (.sdp)|*.sdp");
						filename = string.IsNullOrEmpty(saveFilename) ? filename : saveFilename;
					}
					SaveStoryDesigner(filename);
				}));
			}
		}

		/// <summary>
		/// Current story.
		/// </summary>
		public StoryViewModel Story { get; } = new StoryViewModel();

		/// <summary>
		/// Default constructor.
		/// </summary>
		public MainViewModel()
		{
			Story.CreateNode("Node", "Description", new Point(), false);
		}

		/// <summary>
		/// On Connection Drag Completed
		/// </summary>
		/// <param name="newConnection"></param>
		/// <param name="connectorDraggedOut"></param>
		/// <param name="connectorDraggedOver"></param>
		public void ConnectionDragCompleted(StoryConnectionViewModel newConnection, StoryConditionViewModel connectorDraggedOut, StoryNodeViewModel connectorDraggedOver)
		{
			if (connectorDraggedOver == null || connectorDraggedOut.Parent == connectorDraggedOver)
			{
				this.Story.Connections.Remove(newConnection);
				connectorDraggedOut.Connection = null;
				return;
			}

			newConnection.Node = connectorDraggedOver;
		}

		/// <summary>
		/// Update positions while dragging.
		/// </summary>
		/// <param name="curDragPoint"></param>
		/// <param name="connection"></param>
		public void ConnectionDragging(Point curDragPoint, StoryConnectionViewModel connection)
		{
			connection.NodeConnectorHotspot = curDragPoint;
		}

		/// <summary>
		/// Started drag.
		/// </summary>
		/// <param name="conditionConnector"></param>
		/// <param name="curDragPoint"></param>
		/// <returns></returns>
		public StoryConnectionViewModel ConnectionDragStarted(StoryConditionViewModel conditionConnector, Point curDragPoint)
		{
			var connection = conditionConnector.Connection ?? (conditionConnector.Connection = CreateConnection());
			connection.Condition = conditionConnector;
			connection.Node = null;
			connection.NodeConnectorHotspot = curDragPoint;
			return connection;
		}

		private StoryConnectionViewModel CreateConnection()
		{
			StoryConnectionViewModel connection = new StoryConnectionViewModel();
			this.Story.Connections.Add(connection);
			return connection;
		}

		private void LoadStoryDesigner(string filename)
		{
			if (string.IsNullOrEmpty(filename))
				return;
			Story.Connections.Clear();
			Story.Nodes.Clear();
			FileInfo file = new FileInfo(filename);
			using (var stream = file.OpenRead())
			{
				XmlSerializer serializer = new XmlSerializer(typeof(StoryDesignerRoot));
				StoryDesignerRoot root = (StoryDesignerRoot)serializer.Deserialize(stream);
				Dictionary<Guid, StoryNodeViewModel> tempNodes = new Dictionary<Guid, StoryNodeViewModel>();

				foreach (var node in root.Nodes)
				{
					StoryNodeViewModel nodeView = new StoryNodeViewModel(Story, new Model.StoryNode() { Id = node.Id, Name = node.Name, Description = node.Description }) { X = node.X, Y = node.Y };
					tempNodes.Add(node.Id, nodeView);
					Story.Nodes.Add(nodeView);
				}
				foreach (var node in root.Nodes)
				{
					foreach (var condition in node.Conditions)
					{
						StoryNodeViewModel targetNode = condition.Node != Guid.Empty ? tempNodes[condition.Node] : null;
						tempNodes[node.Id].Conditions.Add(new StoryConditionViewModel(Story, tempNodes[node.Id], new Model.StoryCondition() { Name = condition.Name, Node = (Model.StoryNode)targetNode }) { Target = targetNode });
					}
				}
				foreach (var node in Story.Nodes)
				{
					foreach (var condition in node.Conditions)
					{
						if (condition.Target == null)
							continue;
						StoryConnectionViewModel connection = CreateConnection();
						connection.Condition = condition;
						connection.Node = condition.Target;
						condition.Connection = connection;
					}
				}
			}
		}

		private void SaveStory(string filename)
		{
			if (string.IsNullOrEmpty(filename))
				return;
			StoryRoot root = new StoryRoot() { Nodes = new Collection<StoryNode>() };
			foreach (var node in Story.Nodes)
			{
				StoryNode storyNode = new StoryNode() { Id = node.Id, Name = node.Name, Description = node.Description, Conditions = new Collection<StoryNodeCondition>() };
				foreach (var condition in node.Conditions)
				{
					storyNode.Conditions.Add(new StoryNodeCondition() { Name = condition.Name, Node = (condition.Connection?.Node?.Id ?? Guid.Empty) });
				}
				root.Nodes.Add(storyNode);
			}
			FileInfo file = new FileInfo(filename);
			using (var stream = file.Create())
			{
				XmlSerializer serializer = new XmlSerializer(typeof(StoryRoot));
				serializer.Serialize(stream, root);
			}
		}

		private void SaveStoryDesigner(string filename)
		{
			if (string.IsNullOrEmpty(filename))
				return;
			StoryDesignerRoot root = new StoryDesignerRoot() { Nodes = new Collection<StoryDesignerNode>() };
			foreach (var node in Story.Nodes)
			{
				StoryDesignerNode storyNode = new StoryDesignerNode() { Id = node.Id, Name = node.Name, Description = node.Description, X = node.X, Y = node.Y, Conditions = new Collection<StoryDesignerNodeCondition>() };
				foreach (var condition in node.Conditions)
				{
					storyNode.Conditions.Add(new StoryDesignerNodeCondition() { Name = condition.Name, Node = (condition.Connection?.Node?.Id ?? Guid.Empty) });
				}
				root.Nodes.Add(storyNode);
			}
			FileInfo file = new FileInfo(filename);
			using (var stream = file.Create())
			{
				XmlSerializer serializer = new XmlSerializer(typeof(StoryDesignerRoot));
				serializer.Serialize(stream, root);
			}
		}

		private string ShowSaveDialog(string filter)
		{
			SaveFileDialog dialog = new SaveFileDialog()
			{
				AddExtension = true,
				CheckPathExists = true,
				CreatePrompt = false,
				DefaultExt = ".sdp",
				DereferenceLinks = true,
				Filter = filter,
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
				OverwritePrompt = true,
				RestoreDirectory = true,
				Title = "Select save path",
				ValidateNames = true
			};
			bool? result = dialog.ShowDialog();
			return result.HasValue && result.Value ? dialog.FileName : string.Empty;
		}

		/// <summary>
		/// Project Node
		/// </summary>
		public struct StoryDesignerNode
		{
			[XmlArray("Conditions", Namespace = "app://storydesigner/")]
			[XmlArrayItem("Condition", Namespace = "app://storydesigner/")]
			public Collection<StoryDesignerNodeCondition> Conditions;
			public string Description;
			[XmlAttribute]
			public Guid Id;
			[XmlAttribute]
			public string Name;
			[XmlAttribute]
			public double X;
			[XmlAttribute]
			public double Y;
		}

		/// <summary>
		/// Project Condition. Save/Load-Dummies
		/// </summary>
		public struct StoryDesignerNodeCondition
		{
			[XmlAttribute]
			public string Name;
			[XmlAttribute]
			public Guid Node;
		}

		/// <summary>
		/// Project. Save/Load-Dummies
		/// </summary>
		[XmlRoot("Story", Namespace = "app://storydesigner/")]
		public struct StoryDesignerRoot
		{
			[XmlArray("Nodes", Namespace = "app://storydesigner/")]
			[XmlArrayItem("Node", Namespace = "app://storydesigner/")]
			public Collection<StoryDesignerNode> Nodes;
		}

		/// <summary>
		/// Story Node. Save/Load-Dummies
		/// </summary>
		public struct StoryNode
		{
			[XmlArray("Conditions", Namespace = "app://storydesigner/")]
			[XmlArrayItem("Condition", Namespace = "app://storydesigner/")]
			public Collection<StoryNodeCondition> Conditions;
			public string Description;
			[XmlAttribute]
			public Guid Id;
			[XmlAttribute]
			public string Name;
		}

		/// <summary>
		/// Story Condition. Save/Load-Dummies
		/// </summary>
		public struct StoryNodeCondition
		{
			[XmlAttribute]
			public string Name;
			[XmlAttribute]
			public Guid Node;
		}

		/// <summary>
		/// Story Root. Save/Load-Dummies
		/// </summary>
		[XmlRoot("Story", Namespace = "app://storydesigner/")]
		public struct StoryRoot
		{
			[XmlArray("Nodes", Namespace = "app://storydesigner/")]
			[XmlArrayItem("Node", Namespace = "app://storydesigner/")]
			public Collection<StoryNode> Nodes;
		}
	}
}
