using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs.Swipe;
using Cafeteria.Management.Services.Customers;
using static Cafeteria.Management.Components.Shared.Toast;

namespace Cafeteria.Management.Components.Pages.Customers;

public partial class Customer : ComponentBase
{
    [Inject]
    private ICustomerService CustomerService { get; set; } = default!;

    private List<CustomerSwipeDto>? allCustomers;
    private bool isLoading = true;
    private string toastMessage = "";
    private ToastType toastType = ToastType.Success;
    private bool showToast = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadCustomers();
    }

    private async Task LoadCustomers()
    {
        try
        {
            isLoading = true;
            allCustomers = await CustomerService.GetAllCustomers();
        }
        catch (Exception ex)
        {
            ShowToast($"Error loading customers: {ex.Message}", ToastType.Error);
            allCustomers = [];
        }
        finally
        {
            isLoading = false;
        }
    }

    private void ShowToast(string message, ToastType type)
    {
        toastMessage = message;
        toastType = type;
        showToast = true;
    }
}
