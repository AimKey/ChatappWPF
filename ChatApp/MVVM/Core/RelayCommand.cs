using System.Windows.Input;

namespace ChatApp.MVVM.Core
{
    // This class is just handle binding stuff from UI
    // And yes I don't really know too much detail about it, maybe GPT 
    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;
        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;

            // Subscribe to CommandManager.RequerySuggested event
            CommandManager.RequerySuggested += (sender, e) => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }

}
