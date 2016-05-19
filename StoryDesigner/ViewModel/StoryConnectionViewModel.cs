/* Copyright: Jöran Malek */

using System;
using System.Windows;
using System.Windows.Media;

namespace StoryDesigner.ViewModel
{
	public class StoryConnectionViewModel : ViewModelBase
	{
		public event EventHandler<EventArgs> ConnectionChanged;

		private StoryConditionViewModel condition;
		private Point conditionConnectorHotspot;
		private StoryNodeViewModel node;
		private Point nodeConnectorHotspot;
		private PointCollection points;

		/// <summary>
		/// Connection from this condition.
		/// </summary>
		public StoryConditionViewModel Condition
		{
			get
			{
				return condition;
			}
			set
			{
				if (condition != null)
				{
					condition.HotspotUpdated -= Condition_HotspotUpdated;
				}
				condition = value;
				if (condition != null)
				{
					condition.HotspotUpdated += Condition_HotspotUpdated;
					ConditionConnectorHotspot = condition.Hotspot;
				}
				RaisePropertyChanged(nameof(Condition));
				OnConnectionChanged();
			}
		}

		/// <summary>
		/// Condition hotspot.
		/// </summary>
		public Point ConditionConnectorHotspot
		{
			get
			{
				return conditionConnectorHotspot;
			}
			set
			{
				conditionConnectorHotspot = value;
				ComputeConnectionPoints();
				RaisePropertyChanged(nameof(ConditionConnectorHotspot));
			}
		}

		/// <summary>
		/// Connection to this node.
		/// </summary>
		public StoryNodeViewModel Node
		{
			get
			{
				return node;
			}
			set
			{
				if (node != null)
				{
					node.HotspotUpdated -= Node_HotspotUpdated;
				}

				node = value;
				if (node != null)
				{
					node.HotspotUpdated += Node_HotspotUpdated;
					NodeConnectorHotspot = node.Hotspot;
				}

				RaisePropertyChanged(nameof(Node));
				OnConnectionChanged();
			}
		}

		/// <summary>
		/// Point of node hotspot.
		/// </summary>
		public Point NodeConnectorHotspot
		{
			get
			{
				return nodeConnectorHotspot;
			}
			set
			{
				nodeConnectorHotspot = value;
				ComputeConnectionPoints();
				RaisePropertyChanged(nameof(NodeConnectorHotspot));
			}
		}

		/// <summary>
		/// Some points that are drawn.
		/// </summary>
		public PointCollection Points
		{
			get
			{
				return points;
			}
			set
			{
				points = value;
				RaisePropertyChanged(nameof(Points));
			}
		}

		private void ComputeConnectionPoints()
		{
			PointCollection computedPoints = new PointCollection();
			computedPoints.Add(this.ConditionConnectorHotspot);
			Point conditionConnectorMinusOne = new Point(ConditionConnectorHotspot.X + 32, ConditionConnectorHotspot.Y);
			computedPoints.Add(conditionConnectorMinusOne);
			Point nodeConnectorPlusOne = new Point(NodeConnectorHotspot.X - 32, NodeConnectorHotspot.Y);

			double deltaXS = nodeConnectorPlusOne.X - conditionConnectorMinusOne.X;
			double deltaX = Math.Abs(nodeConnectorPlusOne.X - conditionConnectorMinusOne.X);
			double deltaY = Math.Abs(nodeConnectorPlusOne.Y - conditionConnectorMinusOne.Y);
			if (deltaX > deltaY && deltaXS > 0)
			{
				double midPointX = conditionConnectorMinusOne.X + ((nodeConnectorPlusOne.X - conditionConnectorMinusOne.X) / 2);
				computedPoints.Add(new Point(midPointX, conditionConnectorMinusOne.Y));
				computedPoints.Add(new Point(midPointX, nodeConnectorPlusOne.Y));
			}
			else
			{
				double midPointY = conditionConnectorMinusOne.Y + ((nodeConnectorPlusOne.Y - conditionConnectorMinusOne.Y) / 2);
				computedPoints.Add(new Point(conditionConnectorMinusOne.X, midPointY));
				computedPoints.Add(new Point(nodeConnectorPlusOne.X, midPointY));
			}

			computedPoints.Add(nodeConnectorPlusOne);
			computedPoints.Add(this.NodeConnectorHotspot);
			computedPoints.Freeze();

			this.Points = computedPoints;
		}

		private void Condition_HotspotUpdated(object sender, EventArgs e)
		{
			ConditionConnectorHotspot = Condition.Hotspot;
		}

		private void Node_HotspotUpdated(object sender, EventArgs e)
		{
			NodeConnectorHotspot = Node.Hotspot;
		}

		private void OnConnectionChanged()
		{
			ConnectionChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
