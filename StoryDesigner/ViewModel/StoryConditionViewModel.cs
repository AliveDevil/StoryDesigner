/* Copyright: Jöran Malek */
using System;
using System.Windows;
using StoryDesigner.Model;

namespace StoryDesigner.ViewModel
{
	public class StoryConditionViewModel : ViewModelBase
	{
		private StoryCondition condition;
		private StoryNodeViewModel node;
		private StoryViewModel story;
		private Point hotspot;
		private StoryConnectionViewModel connection;
		private RelayCommand deleteConditionCommand;

		public event EventHandler<EventArgs> HotspotUpdated;

		/// <summary>
		/// Current position.
		/// </summary>
		public Point Hotspot
		{
			get
			{
				return hotspot;
			}
			set
			{
				if (hotspot == value)
				{
					return;
				}

				hotspot = value;

				OnHotspotUpdated();
			}
		}

		/// <summary>
		/// Which node belonds to this condition.
		/// </summary>
		public StoryNodeViewModel Parent { get; }

		/// <summary>
		/// Header.
		/// </summary>
		public string Name
		{
			get { return condition.Name; }
			set
			{
				condition.Name = value;
				RaisePropertyChanged(nameof(Name));
			}
		}

		/// <summary>
		/// Attached connection.
		/// </summary>
		public StoryConnectionViewModel Connection
		{
			get { return connection; }
			set
			{
				if (connection != null) connection.ConnectionChanged -= Connection_ConnectionChanged;
				connection = value;
				if (connection != null) connection.ConnectionChanged += Connection_ConnectionChanged;
				RaisePropertyChanged(nameof(Connection));
				RaisePropertyChanged(nameof(IsConnectionAttached));
				RaisePropertyChanged(nameof(IsConnected));
			}
		}

		/// <summary>
		/// Delete this condition.
		/// </summary>
		public RelayCommand DeleteConditionCommand
		{
			get
			{
				return deleteConditionCommand ?? (deleteConditionCommand = new RelayCommand(() =>
				{
					Parent.RemoveCondition(this);
				}));
			}
		}

		/// <summary>
		/// Is this connection connected to a node
		/// </summary>
		public bool IsConnected => Connection?.Node != null;

		/// <summary>
		/// Is there a connection?
		/// </summary>
		public bool IsConnectionAttached => Connection != null;

		/// <summary>
		/// Target.
		/// </summary>
		public StoryNodeViewModel Target { get; set; }

		public StoryConditionViewModel(StoryViewModel story, StoryNodeViewModel parent, StoryCondition condition)
		{
			this.story = story;
			Parent = parent;
			this.condition = condition;
		}

		private void OnHotspotUpdated()
		{
			RaisePropertyChanged(nameof(Hotspot));
			HotspotUpdated?.Invoke(this, EventArgs.Empty);
		}

		private void Connection_ConnectionChanged(object sender, EventArgs e)
		{
			RaisePropertyChanged(nameof(IsConnectionAttached));
			RaisePropertyChanged(nameof(IsConnected));
		}

		public static implicit operator StoryCondition(StoryConditionViewModel sc)
		{
			return sc.condition;
		}
	}
}
