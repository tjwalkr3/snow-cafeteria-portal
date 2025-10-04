﻿using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.ViewModelInterfaces;

public interface ILocationSelectVM
{
    List<LocationDto> Locations { get; }
    Task InitializeLocationsAsync();
    void OnLocationSelected(LocationDto location);
}