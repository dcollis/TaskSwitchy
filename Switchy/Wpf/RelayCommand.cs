using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Switchy.Wpf
{
    internal class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        internal RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        internal RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        internal RelayCommand(Action execute, Func<bool> canExecute) : this(o => execute(), o => canExecute())
        {
        
        }

        internal RelayCommand(Action execute) : this(execute, () => true)
        {

        }
        

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
