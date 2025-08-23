using System.Windows.Input;

namespace Thesaurus.wpf.Commands
{
    public sealed class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool>? _can;

        public AsyncRelayCommand(Func<Task> execute, Func<bool>? can = null)
        {
            _execute = execute;
            _can = can;
        }

        public bool CanExecute(object? parameter) => _can?.Invoke() ?? true;

        public async void Execute(object? parameter)
        {
            try { await _execute(); }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        public event EventHandler? CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
