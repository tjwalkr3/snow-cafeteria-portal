namespace Cafeteria.Customer.Components.Pages.Stations.Domain;

public sealed class CardOrderAutoSyncCoordinator : IDisposable
{
    private readonly Func<Task> _syncAction;
    private readonly int _debounceMs;
    private CancellationTokenSource? _debounceCts;
    private readonly SemaphoreSlim _syncLock = new(1, 1);

    public CardOrderAutoSyncCoordinator(Func<Task> syncAction, int debounceMs = 250)
    {
        _syncAction = syncAction;
        _debounceMs = debounceMs;
    }

    public void Schedule()
    {
        _debounceCts?.Cancel();
        _debounceCts?.Dispose();
        _debounceCts = new CancellationTokenSource();
        _ = DebouncedSyncAsync(_debounceCts.Token);
    }

    public async Task FlushAsync()
    {
        _debounceCts?.Cancel();
        _debounceCts?.Dispose();
        _debounceCts = null;
        await ExecuteSyncAsync();
    }

    public void Dispose()
    {
        _debounceCts?.Cancel();
        _debounceCts?.Dispose();
        _syncLock.Dispose();
    }

    private async Task DebouncedSyncAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(_debounceMs, cancellationToken);
            await ExecuteSyncAsync();
        }
        catch (OperationCanceledException)
        {
            // A newer scheduled sync superseded this one.
        }
    }

    private async Task ExecuteSyncAsync()
    {
        await _syncLock.WaitAsync();
        try
        {
            await _syncAction();
        }
        finally
        {
            _syncLock.Release();
        }
    }
}