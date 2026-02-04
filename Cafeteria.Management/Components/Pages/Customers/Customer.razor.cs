using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs.Swipe;
using System.Net.Http.Json;

namespace Cafeteria.Management.Components.Pages.Customers;

public partial class Customer : ComponentBase
{
    [Inject]
    private HttpClient HttpClient { get; set; } = default!;

    private List<CustomerSwipeDto>? allCustomers;
    private bool isLoading = true;
    private string toastMessage = "";
    private string toastType = "";
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
            var response = await HttpClient.GetAsync("api/swipe/all-customers");
            
            if (response.IsSuccessStatusCode)
            {
                allCustomers = await response.Content.ReadFromJsonAsync<List<CustomerSwipeDto>>();
            }
            else
            {
                ShowToast("Failed to load customers", "error");
                allCustomers = new List<CustomerSwipeDto>();
            }
        }
        catch (Exception ex)
        {
            ShowToast($"Error loading customers: {ex.Message}", "error");
            allCustomers = new List<CustomerSwipeDto>();
        }
        finally
        {
            isLoading = false;
        }
    }

    private void ShowToast(string message, string type)
    {
        toastMessage = message;
        toastType = type;
        showToast = true;
    }
}
