using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PharmacyInventory.Commands
{
    public sealed class AsyncRelayCommand : ICommand
    {
        private readonly Func<object?, Task> _execute;
        private readonly Func<object?, bool>? _canExecute;
        private bool _isExecuting;

        public event EventHandler? CanExecuteChanged;

        // Supports: new AsyncRelayCommand(async () => ...)
        public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
        {
            if (execute is null) throw new ArgumentNullException(nameof(execute));

            _execute = _ => execute();
            _canExecute = canExecute is null ? null : (_ => canExecute());
        }

        // Supports: new AsyncRelayCommand(async _ => ...)
        public AsyncRelayCommand(Func<object?, Task> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
            => !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);

        public async void Execute(object? parameter)
        {
            await ExecuteAsync(parameter);
        }

        // ✅ Fixes your SettingsView.xaml.cs error: ExecuteAsync not found
        public async Task ExecuteAsync(object? parameter = null)
        {
            if (!CanExecute(parameter)) return;

            try
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();

                // IMPORTANT: no ConfigureAwait(false) here
                await _execute(parameter);
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            var dispatcher = Application.Current?.Dispatcher;

            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                dispatcher.BeginInvoke(new Action(() =>
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty)));
            }
            else
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
