namespace Cafeteria.Management.Components.Pages.LocationAndStation;
using Microsoft.AspNetCore.Components;
using Cafeteria.Management.Services;

public partial class LocationAndStation : ComponentBase
{
    [Inject]
    public ILocationAndStationVM ViewModel { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    /* Start */
    // This is a proof-of-concept. Delete it when you begin implementation. */
    [Inject]
    private IHttpClientAuth HttpClientAuth { get; set; } = default!;
    
    private string _authResult = "";

    private async Task CallAuthenticatedEndpoint()
    {
        try
        {
            var result = await HttpClientAuth.GetAsync<AuthResponse>("api/Location/authenticated");
            if (result != null)
            {
                _authResult = $"Authenticated as: {result.Username}";
            }
        }
        catch (Exception ex)
        {
            _authResult = $"Error: {ex.Message}";
        }
    }

    private class AuthResponse
    {
        public string Username { get; set; } = "";
    }
    /* End */
}