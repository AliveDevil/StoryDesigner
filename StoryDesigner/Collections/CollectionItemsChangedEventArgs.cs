/* Copyright: Ashley Davis (http://www.codeproject.com/Articles/182683/NetworkView-A-WPF-custom-control-for-visualizing-a) */
using System;
using System.Collections;

namespace StoryDesigner.Collections
{
	/// <summary>
	/// Event for changed items.
	/// </summary>
	public class CollectionItemsChangedEventArgs : EventArgs
	{
		private ICollection items = null;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="items">Changed items.</param>
		public CollectionItemsChangedEventArgs(ICollection items)
		{
			this.items = items;
		}

		/// <summary>
		/// Collection of changed items.
		/// </summary>
		public ICollection Items
		{
			get
			{
				return items;
			}
		}
	}
}
