using Cafeteria.Shared.Services.Auth;
using Cafeteria.Shared.Services.Portal;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Cafeteria.Shared.Components.Pages.SignIn;

public partial class SignIn : ComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IAuthService AuthenticationService { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    private IPortalSettings PortalSettings { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "returnUrl")]
    public string? ReturnUrl { get; set; }

    private string Username { get; set; } = string.Empty;
    private string Password { get; set; } = string.Empty;
    private bool IsLoading { get; set; }
    private bool HasError { get; set; }
    private string ErrorMessage { get; set; } = string.Empty;

    private async Task HandleSubmitAsync()
    {
        if (!ValidateInputs())
        {
            return;
        }

        await PerformSignInAsync();
    }

    private bool ValidateInputs()
    {
        ClearError();

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ShowError("Please enter both username and password");
            return false;
        }

        return true;
    }

    private async Task PerformSignInAsync()
    {
        StartLoading();

        var result = await AuthenticationService.ValidateCredentialsAsync(Username, Password);

        if (result.Success && !string.IsNullOrEmpty(result.SessionToken))
        {
            await NavigateToSessionEndpoint(result.SessionToken);
            return;
        }

        StopLoading();
        ShowError(result.ErrorMessage ?? "Sign in failed");
    }

    private async Task NavigateToSessionEndpoint(string sessionToken)
    {
        try
        {
            var destination = string.IsNullOrWhiteSpace(ReturnUrl) ? "/" : ReturnUrl;
            var url = $"/auth/signin?token={Uri.EscapeDataString(sessionToken)}&returnUrl={Uri.EscapeDataString(destination)}";
            await JSRuntime.InvokeVoidAsync("window.location.assign", url);
        }
        catch (JSDisconnectedException)
        {
        }
        catch (TaskCanceledException)
        {
        }
    }

    private void StartLoading()
    {
        IsLoading = true;
    }

    private void StopLoading()
    {
        IsLoading = false;
    }

    private void ShowError(string message)
    {
        HasError = true;
        ErrorMessage = message;
    }

    private void ClearError()
    {
        HasError = false;
        ErrorMessage = string.Empty;
    }
}
