namespace Cafeteria.Customer.Services;

public class CartNotificationService
{
    public event Action? OnCartChanged;

    public void NotifyCartChanged()
    {
        OnCartChanged?.Invoke();
    }
}
