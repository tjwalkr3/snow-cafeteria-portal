using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Management.Services.OptionOptionTypes;
using Cafeteria.Management.Components.Pages.FoodOption;

namespace Cafeteria.Management.Components.Pages.FoodOptionType;

public partial class AddFoodOptionToTypeModal : ComponentBase
{
    [Inject]
    private IOptionOptionTypeService OptionOptionTypeService { get; set; } = default!;

    [Inject]
    private IOptionOptionTypeVM OptionOptionTypeVM { get; set; } = default!;

    [Parameter]
    public int FoodTypeId { get; set; }

    [Parameter]
    public List<FoodOptionDto> AllOptions { get; set; } = new();

    [Parameter]
    public List<FoodOptionDto> AlreadyAssignedOptions { get; set; } = new();

    [Parameter]
    public EventCallback OnSave { get; set; }

    [Parameter]
    public EventCallback OnCancel { get; set; }

    private string SearchText { get; set; } = string.Empty;
    private bool IsSaving { get; set; } = false;
    private string? ErrorMessage { get; set; }
    private List<FoodOptionDto> AssignedOptions { get; set; } = new();
    private bool HasChanges { get; set; } = false;

    private List<FoodOptionDto> AvailableOptions =>
        AllOptions.Where(o => !AssignedOptions.Any(a => a.Id == o.Id)).ToList();

    private List<FoodOptionDto> FilteredAvailableOptions =>
        string.IsNullOrWhiteSpace(SearchText)
            ? AvailableOptions
            : AvailableOptions.Where(o => 
                o.FoodOptionName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();

    protected override void OnInitialized()
    {
        AssignedOptions = new List<FoodOptionDto>(AlreadyAssignedOptions);
    }

    private async Task HandleAdd(int foodOptionId)
    {
        if (foodOptionId == 0)
        {
            ErrorMessage = "Please select a food option.";
            return;
        }

        IsSaving = true;
        ErrorMessage = null;
        StateHasChanged();

        try
        {
            var mapping = new OptionOptionTypeDto
            {
                FoodOptionId = foodOptionId,
                FoodOptionTypeId = FoodTypeId
            };

            await OptionOptionTypeService.CreateOptionOptionType(mapping);
            
            // Add to assigned options list for display
            var selectedOption = AllOptions.FirstOrDefault(o => o.Id == foodOptionId);
            if (selectedOption != null)
            {
                AssignedOptions.Add(selectedOption);
                HasChanges = true;
            }
            
            SearchText = string.Empty; // Clear search
            IsSaving = false;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error adding food option: {ex.Message}";
            IsSaving = false;
            StateHasChanged();
        }
    }

    private async Task RemoveOption(int foodOptionId)
    {
        try
        {
            // Find the OptionOptionType record
            var mapping = OptionOptionTypeVM.OptionOptionTypes?.FirstOrDefault(oot => 
                oot.FoodOptionId == foodOptionId && oot.FoodOptionTypeId == FoodTypeId);

            if (mapping != null)
            {
                await OptionOptionTypeService.DeleteOptionOptionTypeById(mapping.Id);
                AssignedOptions.RemoveAll(o => o.Id == foodOptionId);
                HasChanges = true;
                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error removing food option: {ex.Message}";
            StateHasChanged();
        }
    }

    private async Task Cancel()
    {
        // Call OnSave if there were changes, otherwise OnCancel
        if (HasChanges)
        {
            await OnSave.InvokeAsync();
        }
        else
        {
            await OnCancel.InvokeAsync();
        }
    }
}
