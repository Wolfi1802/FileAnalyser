using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileAnalyser
{
    internal class AsyncCommand : ICommand
    {
        private readonly Func<Task> execute;
        private readonly Func<bool>? canExecute;

        public AsyncCommand(Func<Task> execute, Func<bool>? canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => this.canExecute?.Invoke() ?? true;
        public async void Execute(object? parameter)
        {
            await this.execute();
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value!;//[TS]value!, das Ausrufezeichen sagt aus: value darf hier nicht null sein, 
            remove => CommandManager.RequerySuggested -= value!;//[TS]value!, das Ausrufezeichen sagt aus: value darf hier nicht null sein, 
        }
    }
}
