namespace Cafeteria.Customer.Components.Pages.Stations.Domain;

public class TabDefinition
{
    public string Id { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public bool IsDefault { get; init; }

    public TabDefinition() { }

    public TabDefinition(string id, string displayName, bool isDefault = false)
    {
        Id = id;
        DisplayName = displayName;
        IsDefault = isDefault;
    }
}
