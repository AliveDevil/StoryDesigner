/* Copyright: Jöran Malek */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using StoryDesigner.Collections;
using StoryDesigner.Model;

namespace StoryDesigner.ViewModel
{
	public class StoryViewModel : ViewModelBase
	{
		public ImpObservableCollection<StoryNodeViewModel> Nodes { get; } = new ImpObservableCollection<StoryNodeViewModel>();
		public ImpObservableCollection<StoryConnectionViewModel> Connections { get; } = new ImpObservableCollection<StoryConnectionViewModel>();
		public StoryViewModel()
		{
			Connections.ItemsRemoved += Connections_ItemsRemoved;
		}

		private void Connections_ItemsRemoved(object sender, CollectionItemsChangedEventArgs e)
		{

		}
		public StoryNodeViewModel CreateNode(string name, string description, Point nodeLocation, bool centerNode)
		{
			var node = new StoryNodeViewModel(this, new StoryNode() { Name = name, Description = description, Id = Guid.NewGuid() });
			node.X = nodeLocation.X;
			node.Y = nodeLocation.Y;

			node.Conditions.Add(new StoryConditionViewModel(this, node, new StoryCondition() { Name = "Condition" }));

			if (centerNode)
			{
				EventHandler<EventArgs> sizeChangedEventHandler = null;
				sizeChangedEventHandler =
					delegate (object sender, EventArgs e)
					{
						node.X -= node.Size.Width / 2;
						node.Y -= node.Size.Height / 2;

						node.SizeChanged -= sizeChangedEventHandler;
					};

				node.SizeChanged += sizeChangedEventHandler;
			}

			Nodes.Add(node);

			return node;
		}

		public void DeleteNode(StoryNodeViewModel node)
		{
			ICollection<StoryConnectionViewModel> connections = new Collection<StoryConnectionViewModel>();
			foreach (var item in Connections)
			{
				if (item.Node == node || item.Condition.Parent == node)
				{
					connections.Add(item);
					item.Condition.Connection = null;
				}
			}
			Connections.RemoveRange(connections);

			Nodes.Remove(node);
		}
	}
}
