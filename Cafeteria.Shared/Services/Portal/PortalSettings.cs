namespace Cafeteria.Shared.Services.Portal;

public class PortalSettings : IPortalSettings
{
    public string PortalName { get; init; } = string.Empty;
    public string SignInSubtitle { get; init; } = string.Empty;
}
