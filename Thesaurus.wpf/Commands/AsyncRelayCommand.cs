using System.Windows.Input;

namespace Thesaurus.wpf.Commands
{
    public sealed class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _execute; // The asynchronous operation to execute
        private readonly Func<bool>? _can; // Optional function to determine if the command can execute
        private bool _isExecuting; // Tracks whether the command is currently executing

        // Constructor to initialize the command with the execute function and optional can-execute function
        public AsyncRelayCommand(Func<Task> execute, Func<bool>? can = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _can = can;
        }

        // Determines whether the command can execute based on the _isExecuting flag and the optional _can function
        public bool CanExecute(object? parameter) => !_isExecuting && (_can?.Invoke() ?? true);

        // Executes the asynchronous operation, ensuring it cannot be executed concurrently
        public async void Execute(object? parameter)
        {
            if (!CanExecute(parameter)) return; // Prevent execution if the command cannot execute

            _isExecuting = true; // Mark the command as executing
            RaiseCanExecuteChanged(); // Notify that CanExecute state has changed

            try
            {
                await _execute(); // Execute the asynchronous operation
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                _isExecuting = false; // Mark the command as not executing
                RaiseCanExecuteChanged(); // Notify that CanExecute state has changed
            }
        }

        // Event to notify when the CanExecute state changes
        public event EventHandler? CanExecuteChanged;

        // Raises the CanExecuteChanged event
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
