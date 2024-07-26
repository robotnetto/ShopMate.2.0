using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

public class DebounceCommand : ICommand
{
    private readonly ICommand _innerCommand;
    private readonly TimeSpan _delay;
    private CancellationTokenSource _cancellationTokenSource;

    public DebounceCommand(ICommand innerCommand, TimeSpan delay)
    {
        _innerCommand = innerCommand ?? throw new ArgumentNullException(nameof(innerCommand));
        _delay = delay;
    }

    public bool CanExecute(object parameter)
    {
        return _innerCommand.CanExecute(parameter);
    }

    public async void Execute(object parameter)
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            await Task.Delay(_delay, _cancellationTokenSource.Token);
            _innerCommand.Execute(parameter);
        }
        catch (TaskCanceledException)
        {
            // Ignore the exception
        }
    }

    public event EventHandler CanExecuteChanged
    {
        add => _innerCommand.CanExecuteChanged += value;
        remove => _innerCommand.CanExecuteChanged -= value;
    }
}
