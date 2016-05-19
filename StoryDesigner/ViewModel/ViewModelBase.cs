/* Copyright: Jöran Malek */
using System.ComponentModel;

namespace StoryDesigner.ViewModel
{
	/// <summary>
	/// Base class for viewmodel.
	/// </summary>
	public class ViewModelBase : INotifyPropertyChanged
	{
		/// <summary>
		/// Property changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raise property changed event.
		/// </summary>
		/// <param name="propertyName"></param>
		protected void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Legacy.
		/// </summary>
		/// <param name="propertyName"></param>
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
