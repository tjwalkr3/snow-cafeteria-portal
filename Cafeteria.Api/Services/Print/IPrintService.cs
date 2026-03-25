using Cafeteria.Shared.DTOs.Order;

namespace Cafeteria.Api.Services.Print;

public interface IPrintService
{
    Task PrintOrder(BrowserOrder browserOrder, int orderId);
}