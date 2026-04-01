using Cafeteria.Customer.Components.Pages.Stations.Domain;

namespace Cafeteria.UnitTests.Customer.Domain;

public class CardOrderAutoSyncCoordinatorTests
{
    [Fact]
    public async Task Schedule_DebouncesRapidCalls_ToSingleExecution()
    {
        var executionCount = 0;
        using var coordinator = new CardOrderAutoSyncCoordinator(
            () =>
            {
                Interlocked.Increment(ref executionCount);
                return Task.CompletedTask;
            },
            debounceMs: 25);

        coordinator.Schedule();
        coordinator.Schedule();
        coordinator.Schedule();

        await Task.Delay(120);

        Assert.Equal(1, executionCount);
    }

    [Fact]
    public async Task FlushAsync_CancelsPendingDebounce_AndExecutesOnce()
    {
        var executionCount = 0;
        using var coordinator = new CardOrderAutoSyncCoordinator(
            () =>
            {
                Interlocked.Increment(ref executionCount);
                return Task.CompletedTask;
            },
            debounceMs: 100);

        coordinator.Schedule();
        await coordinator.FlushAsync();
        await Task.Delay(140);

        Assert.Equal(1, executionCount);
    }

    [Fact]
    public async Task FlushAsync_SerializesConcurrentExecutions()
    {
        var current = 0;
        var maxConcurrent = 0;
        var executionCount = 0;

        using var coordinator = new CardOrderAutoSyncCoordinator(async () =>
        {
            var now = Interlocked.Increment(ref current);
            maxConcurrent = Math.Max(maxConcurrent, now);
            Interlocked.Increment(ref executionCount);
            await Task.Delay(40);
            Interlocked.Decrement(ref current);
        });

        await Task.WhenAll(
            coordinator.FlushAsync(),
            coordinator.FlushAsync());

        Assert.Equal(2, executionCount);
        Assert.Equal(1, maxConcurrent);
    }

    [Fact]
    public async Task FlushAndDisposeAsync_FlushesPendingDebounceBeforeDispose()
    {
        var executionCount = 0;
        var coordinator = new CardOrderAutoSyncCoordinator(
            () =>
            {
                Interlocked.Increment(ref executionCount);
                return Task.CompletedTask;
            },
            debounceMs: 100);

        coordinator.Schedule();
        await coordinator.FlushAndDisposeAsync();

        Assert.Equal(1, executionCount);
    }

    [Fact]
    public async Task Schedule_AfterDispose_ThrowsObjectDisposedException()
    {
        var coordinator = new CardOrderAutoSyncCoordinator(() => Task.CompletedTask);
        await coordinator.FlushAndDisposeAsync();

        Assert.Throws<ObjectDisposedException>(() => coordinator.Schedule());
    }
}