/* Copyright: Jöran Malek */

using System;
using System.Windows;
using StoryDesigner.Collections;
using StoryDesigner.Model;

namespace StoryDesigner.ViewModel
{
	public class StoryNodeViewModel : ViewModelBase
	{
		public event EventHandler<EventArgs> HotspotUpdated;

		public event EventHandler<EventArgs> SizeChanged;

		private RelayCommand createConditionCommand;
		private Point hotspot;
		private bool isSelected = false;
		private StoryNode node;
		private RelayCommand removeNodeCommand;
		private Size size = Size.Empty;
		private StoryViewModel story;
		private double x = 0;
		private double y = 0;
		private int zIndex = 0;

		/// <summary>
		/// Every condition in this node.
		/// </summary>
		public ImpObservableCollection<StoryConditionViewModel> Conditions { get; } = new ImpObservableCollection<StoryConditionViewModel>();

		/// <summary>
		/// Create a new condition.
		/// </summary>
		public RelayCommand CreateConditionCommand
		{
			get
			{
				return createConditionCommand ?? (createConditionCommand = new RelayCommand(() =>
				{
					StoryCondition condition = new StoryCondition();
					condition.Name = "Test";
					node.Conditions.Add(condition);
					Conditions.Add(new StoryConditionViewModel(story, this, condition));
				}));
			}
		}

		/// <summary>
		/// Description
		/// </summary>
		public string Description
		{
			get
			{
				return node.Description;
			}
			set
			{
				node.Description = value;
				RaisePropertyChanged(nameof(Description));
			}
		}

		/// <summary>
		/// Current location
		/// </summary>
		public Point Hotspot
		{
			get
			{
				return hotspot;
			}
			set
			{
				if (hotspot == value) return;
				hotspot = value;
				OnHotspotUpdated();
			}
		}

		/// <summary>
		/// Id
		/// </summary>
		public Guid Id
		{
			get { return node.Id; }
			set { node.Id = value; RaisePropertyChanged(nameof(Id)); }
		}

		/// <summary>
		/// Is this node selected
		/// </summary>
		public bool IsSelected
		{
			get
			{
				return isSelected;
			}
			set
			{
				if (isSelected == value) return;
				isSelected = value;
				RaisePropertyChanged(nameof(IsSelected));
			}
		}

		/// <summary>
		/// Header.
		/// </summary>
		public string Name
		{
			get
			{
				return node.Name;
			}
			set
			{
				node.Name = value;
				RaisePropertyChanged(nameof(Name));
			}
		}

		/// <summary>
		/// Remove this node.
		/// </summary>
		public RelayCommand RemoveNodeCommand
		{
			get
			{
				return removeNodeCommand ?? (removeNodeCommand = new RelayCommand(() =>
				{
					story.DeleteNode(this);
				}));
			}
		}

		/// <summary>
		/// Size
		/// </summary>
		public Size Size
		{
			get
			{
				return size;
			}
			set
			{
				if (size == value) return;
				size = value;
				SizeChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// X-Position on view.
		/// </summary>
		public double X
		{
			get
			{
				return x;
			}
			set
			{
				if (x == value) return;
				x = value;
				RaisePropertyChanged(nameof(X));
			}
		}

		/// <summary>
		/// Y-Position on view.
		/// </summary>
		public double Y
		{
			get
			{
				return y;
			}
			set
			{
				if (y == value) return;
				y = value;
				RaisePropertyChanged(nameof(Y));
			}
		}

		/// <summary>
		/// ZIndex.
		/// </summary>
		public int ZIndex
		{
			get
			{
				return zIndex;
			}
			set
			{
				if (zIndex == value) return;
				zIndex = value;
				RaisePropertyChanged(nameof(zIndex));
			}
		}

		public StoryNodeViewModel(StoryViewModel story, StoryNode node)
		{
			this.story = story;
			this.node = node;
			foreach (var condition in node.Conditions)
			{
				Conditions.Add(new StoryConditionViewModel(story, this, condition));
			}
		}

		/// <summary>
		/// Creates a new condition.
		/// </summary>
		public void CreateCondition()
		{
			StoryCondition condition = new StoryCondition();
			node.Conditions.Add(condition);
			Conditions.Add(new StoryConditionViewModel(story, this, condition));
		}

		/// <summary>
		/// Removes a condition.
		/// </summary>
		/// <param name="condition"></param>
		public void RemoveCondition(StoryConditionViewModel condition)
		{
			Conditions.Remove(condition);
			node.Conditions.Remove(condition);
			story.Connections.Remove(condition.Connection);
		}

		public static explicit operator StoryNode(StoryNodeViewModel vm)
		{
			return vm.node;
		}

		private void OnHotspotUpdated()
		{
			RaisePropertyChanged(nameof(Hotspot));
			HotspotUpdated?.Invoke(this, EventArgs.Empty);
		}
	}
}
