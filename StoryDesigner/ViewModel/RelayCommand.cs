/* Copyright: Jöran Malek */
using System;
using System.Windows.Input;

namespace StoryDesigner.ViewModel
{
	/// <summary>
	/// Simple relaycommand.
	/// </summary>
	public class RelayCommand : ICommand
	{
		/// <summary>
		/// Enable Requery in CanExecuteChanged.
		/// </summary>
		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		private Func<bool> canExecute;
		private Action execute;

		/// <summary>
		/// Simple constructor for a command alway returning true on CanExecute.
		/// </summary>
		/// <param name="executeAction"></param>
		public RelayCommand(Action executeAction) : this(executeAction, null)
		{
		}

		/// <summary>
		/// Constructor for creating a relaycommand with given action and check.
		/// </summary>
		/// <param name="executeAction"></param>
		/// <param name="canExecuteFunc"></param>
		public RelayCommand(Action executeAction, Func<bool> canExecuteFunc)
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
			return canExecute?.Invoke() ?? true;
		}

		/// <summary>
		/// Execute this command.
		/// </summary>
		/// <param name="parameter"></param>
		public void Execute(object parameter)
		{
			execute();
		}
	}
}
