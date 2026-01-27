namespace Cafeteria.Customer.Services.Cart;

public class CartNotificationService
{
    public event Action? OnCartChanged;

    public void NotifyCartChanged()
    {
        OnCartChanged?.Invoke();
    }
}
