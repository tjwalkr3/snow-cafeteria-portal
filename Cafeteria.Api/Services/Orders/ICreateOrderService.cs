using Cafeteria.Shared.DTOs.Order;

namespace Cafeteria.Api.Services.Orders;

public interface ICreateOrderService
{
    Task<OrderDto> CreateOrder(BrowserOrder browserOrder, string customerEmail);
}
