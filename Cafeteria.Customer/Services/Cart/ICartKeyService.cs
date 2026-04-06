namespace Cafeteria.Customer.Services.Cart;

public interface ICartKeyService
{
    Task<string> GetCartKey();
}
