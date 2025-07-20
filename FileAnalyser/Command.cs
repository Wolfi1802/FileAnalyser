using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileAnalyser
{
    public class Command : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool>? canExecute;

        public Command(Action execute, Func<bool>? canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => this.canExecute?.Invoke() ?? true;
        public void Execute(object? parameter) => this.execute();

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value!;
            remove => CommandManager.RequerySuggested -= value!;
        }
    }
}
