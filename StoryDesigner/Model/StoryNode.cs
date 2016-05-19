/* Copyright: Jöran Malek */
using System;
using System.Collections.ObjectModel;
using StoryDesigner.Collections;
using StoryDesigner.ViewModel;

namespace StoryDesigner.Model
{
	/// <summary>
	/// Class containing model-information for nodes.
	/// </summary>
	public class StoryNode
	{
		/// <summary>
		/// Node ID
		/// </summary>
		public Guid Id { get; set; }
		/// <summary>
		/// Header of the node.
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Description of the node.
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Every condition for this node.
		/// </summary>
		public ImpObservableCollection<StoryCondition> Conditions { get; } = new ImpObservableCollection<StoryCondition>();
	}
}
