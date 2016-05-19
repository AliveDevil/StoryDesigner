/* Copyright: Jöran Malek */
namespace StoryDesigner.Model
{
	/// <summary>
	/// Class containing model-information for conditions.
	/// </summary>
	public class StoryCondition
	{
		/// <summary>
		/// Condition Name.
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Target node.
		/// </summary>
		public StoryNode Node { get; set; }
	}
}
