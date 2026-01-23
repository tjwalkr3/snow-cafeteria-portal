using Cafeteria.Customer.Components.Pages.Stations.Configuration;

namespace Cafeteria.Customer.Components.Pages.Stations.Strategies;

public interface ISelectionStrategyFactory
{
    ISelectionStrategy CreateStrategy(StationType stationType);
}
