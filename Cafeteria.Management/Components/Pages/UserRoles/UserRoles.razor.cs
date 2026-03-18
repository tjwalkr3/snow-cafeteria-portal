using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs.Customer;
using Cafeteria.Management.Services.Customers;
using static Cafeteria.Management.Components.Shared.Toast;

namespace Cafeteria.Management.Components.Pages.UserRoles;

public partial class UserRoles : ComponentBase, IDisposable
{
    [Inject]
    private ICustomerService CustomerService { get; set; } = default!;

    private List<CustomerRoleDto> customers = [];
    private bool isLoading = false;
    private string searchText = string.Empty;
    private bool hasSearch => !string.IsNullOrWhiteSpace(searchText);
    private HashSet<string> togglingEmail = [];
    private string toastMessage = "";
    private ToastType toastType = ToastType.Success;
    private bool showToast = false;

    private CancellationTokenSource? _debounceCts;

    private async Task LoadCustomers(string? search = null)
    {
        isLoading = true;
        StateHasChanged();
        try
        {
            var results = await CustomerService.GetAllCustomersWithRoles(search);
            customers = results.Take(5).ToList();
        }
        catch (Exception ex)
        {
            ShowToast($"Error loading users: {ex.Message}", ToastType.Error);
            customers = [];
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task OnSearchInput(ChangeEventArgs e)
    {
        searchText = e.Value?.ToString() ?? string.Empty;

        _debounceCts?.Cancel();
        _debounceCts?.Dispose();
        _debounceCts = new CancellationTokenSource();

        try
        {
            await Task.Delay(350, _debounceCts.Token);
            if (string.IsNullOrWhiteSpace(searchText))
            {
                customers = [];
                StateHasChanged();
                return;
            }
            await LoadCustomers(searchText);
        }
        catch (TaskCanceledException)
        {
            // debounce cancelled — a newer keystroke will handle it
        }
    }

    private async Task ToggleRole(CustomerRoleDto customer)
    {
        if (togglingEmail.Contains(customer.Email))
            return;

        togglingEmail.Add(customer.Email);
        StateHasChanged();

        try
        {
            var success = await CustomerService.ToggleFoodServiceRole(customer.Email);
            if (success)
            {
                customer.UserRole = string.Equals(customer.UserRole, "food-service", StringComparison.OrdinalIgnoreCase)
                    ? null
                    : "food-service";
                ShowToast($"Role updated for {customer.CustName}.", ToastType.Success);
            }
            else
            {
                ShowToast($"Could not update role for {customer.CustName}.", ToastType.Error);
            }
        }
        catch (Exception ex)
        {
            ShowToast($"Error: {ex.Message}", ToastType.Error);
        }
        finally
        {
            togglingEmail.Remove(customer.Email);
            StateHasChanged();
        }
    }

    private void ShowToast(string message, ToastType type)
    {
        toastMessage = message;
        toastType = type;
        showToast = true;
    }

    public void Dispose()
    {
        _debounceCts?.Cancel();
        _debounceCts?.Dispose();
    }
}
