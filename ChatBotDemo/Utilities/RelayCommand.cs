using System;
using System.Windows.Input;

namespace ChatBotDemo.Utilities
{
    public class RelayCommand : ICommand
    {
        Predicate<object> canExecuteDelegate;
        Action executeDelegate;

        public RelayCommand(Action tuesDelegate, Predicate<object> kannDelegate)
        {
            executeDelegate = tuesDelegate;
            canExecuteDelegate = kannDelegate;
        }

        public bool CanExecute(object parameter)
        {
            return canExecuteDelegate(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            executeDelegate();
        }
    }
}
