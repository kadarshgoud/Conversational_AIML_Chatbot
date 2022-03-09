using System;
using System.Windows.Input;

namespace ChatBotDemo.Utilities
{
    public sealed class DelegateCommand : ICommand
    {
        private readonly Action _command;
        private object para;

        public DelegateCommand(Action command)
        {
            _command = command;
        }

        public void Execute(object parameter)
        {
            para = parameter;
            _command();
        }
        
        public object getParameter()
        {
            return para;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { }
            remove { }
        }
    }
}
