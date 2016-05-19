/* Copyright: Jöran Malek */
using System;
using System.Windows.Input;

namespace StoryDesigner.ViewModel
{
	/// <summary>
	/// Generic relaycommand.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class RelayCommand<T> : ICommand
	{
		/// <summary>
		/// Enable Requery in CanExecuteChanged.
		/// </summary>
		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		private Func<T, bool> canExecute;
		private Action<T> execute;

		/// <summary>
		/// Simple constructor for a command alway returning true on CanExecute.
		/// </summary>
		/// <param name="executeAction"></param>
		public RelayCommand(Action<T> executeAction) : this(executeAction, null)
		{
		}

		/// <summary>
		/// Constructor for creating a relaycommand with given action and check.
		/// </summary>
		/// <param name="executeAction"></param>
		/// <param name="canExecuteFunc"></param>
		public RelayCommand(Action<T> executeAction, Func<T, bool> canExecuteFunc)
		{
			execute = executeAction;
			canExecute = canExecuteFunc;
		}

		/// <summary>
		/// May this command execute?
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public bool CanExecute(object parameter)
		{
			if (parameter != null && !(parameter is T))
				throw new ArgumentException();
			return canExecute?.Invoke((T)parameter) ?? true;
		}

		/// <summary>
		/// Execute this command.
		/// </summary>
		/// <param name="parameter"></param>
		public void Execute(object parameter)
		{
			if (parameter != null && !(parameter is T))
				throw new ArgumentException();
			execute((T)parameter);
		}
	}
}
