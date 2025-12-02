using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs;
using static Cafeteria.Management.Components.Shared.Toast;
using Cafeteria.Management.Components.Pages.FoodType;
using Cafeteria.Management.Services;

namespace Cafeteria.Management.Components.Pages.FoodOption;

public partial class FoodOption : ComponentBase
{
    [Inject]
    private IFoodOptionVM FoodOptionVM { get; set; } = default!;

    [Inject]
    private IFoodTypeVM FoodTypeVM { get; set; } = default!;

    [Inject]
    private IOptionOptionTypeVM OptionOptionTypeVM { get; set; } = default!;

    [Inject]
    private IOptionOptionTypeService OptionOptionTypeService { get; set; } = default!;

    public bool IsInitialized { get; set; } = false;

    private bool ShowOptionModal { get; set; } = false;
    private bool ShowTypeModal { get; set; } = false;
    private bool ShowDeleteOptionModal { get; set; } = false;
    private bool ShowDeleteTypeModal { get; set; } = false;
    private bool ShowRemoveFromTypeModal { get; set; } = false;

    private FoodOptionDto? SelectedFoodOption { get; set; }
    private FoodOptionTypeDto? SelectedFoodType { get; set; }
    private FoodOptionDto? FoodOptionToDelete { get; set; }
    private FoodOptionTypeDto? FoodTypeToDelete { get; set; }
    private int CurrentFoodTypeId { get; set; } = 0;
    private int OptionToRemoveId { get; set; } = 0;
    private int TypeToRemoveFromId { get; set; } = 0;

    private bool showToast = false;
    private string toastMessage = string.Empty;
    private ToastType toastType = ToastType.Success;

    private string FilterText { get; set; } = string.Empty;
    private string OptionFilterText { get; set; } = string.Empty;
    private HashSet<int> expandedTypes = new HashSet<int>();
    private HashSet<int> expandedOptions = new HashSet<int>();

    private List<FoodOptionTypeDto> FilteredFoodTypes =>
        string.IsNullOrWhiteSpace(FilterText)
            ? FoodTypeVM.FoodTypes ?? new List<FoodOptionTypeDto>()
            : FoodTypeVM.FoodTypes?.Where(ft =>
                ft.FoodOptionTypeName.Contains(FilterText, StringComparison.OrdinalIgnoreCase))
                .ToList() ?? new List<FoodOptionTypeDto>();

    private List<FoodOptionDto> FilteredFoodOptions =>
        string.IsNullOrWhiteSpace(OptionFilterText)
            ? FoodOptionVM.FoodOptions ?? new List<FoodOptionDto>()
            : FoodOptionVM.FoodOptions?.Where(fo =>
                fo.FoodOptionName.Contains(OptionFilterText, StringComparison.OrdinalIgnoreCase))
                .ToList() ?? new List<FoodOptionDto>();

    private string DeleteOptionMessage =>
        FoodOptionToDelete != null
            ? $"Are you sure you want to delete '{FoodOptionToDelete.FoodOptionName}'? This action cannot be undone."
            : string.Empty;

    private string RemoveFromTypeMessage
    {
        get
        {
            if (OptionToRemoveId == 0 || TypeToRemoveFromId == 0)
                return string.Empty;

            var option = FoodOptionVM.FoodOptions?.FirstOrDefault(o => o.Id == OptionToRemoveId);
            var type = FoodTypeVM.FoodTypes?.FirstOrDefault(t => t.Id == TypeToRemoveFromId);

            if (option == null || type == null)
                return string.Empty;

            return $"Remove '{option.FoodOptionName}' from '{type.FoodOptionTypeName}'?";
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await FoodOptionVM.InitializeFoodOptionsAsync();
        await FoodTypeVM.InitializeFoodTypesAsync();
        await OptionOptionTypeVM.InitializeOptionOptionTypesAsync();
        IsInitialized = true;
    }

    private void ToggleTypeExpansion(int typeId)
    {
        if (expandedTypes.Contains(typeId))
            expandedTypes.Remove(typeId);
        else
            expandedTypes.Add(typeId);
    }

    private void ToggleOptionExpansion(int optionId)
    {
        if (expandedOptions.Contains(optionId))
            expandedOptions.Remove(optionId);
        else
            expandedOptions.Add(optionId);
    }

    private string GetTypeIcon(FoodOptionTypeDto foodType)
    {
        return foodType.FoodOptionTypeName switch
        {
            var name when name.Contains("Bread", StringComparison.OrdinalIgnoreCase) => "🍞",
            var name when name.Contains("Meat", StringComparison.OrdinalIgnoreCase) => "🥩",
            var name when name.Contains("Cheese", StringComparison.OrdinalIgnoreCase) => "🧀",
            var name when name.Contains("Topping", StringComparison.OrdinalIgnoreCase) => "🥗",
            var name when name.Contains("Dressing", StringComparison.OrdinalIgnoreCase) => "🥫",
            var name when name.Contains("Plate", StringComparison.OrdinalIgnoreCase) => "🍽️",
            _ => "📋"
        };
    }

    private List<FoodOptionDto> GetOptionsForType(int foodTypeId)
    {
        if (OptionOptionTypeVM.OptionOptionTypes == null || FoodOptionVM.FoodOptions == null)
            return new List<FoodOptionDto>();

        var optionIds = OptionOptionTypeVM.OptionOptionTypes
            .Where(oot => oot.FoodOptionTypeId == foodTypeId)
            .Select(oot => oot.FoodOptionId)
            .ToList();

        return FoodOptionVM.FoodOptions
            .Where(fo => optionIds.Contains(fo.Id))
            .ToList();
    }

    private List<FoodOptionTypeDto> GetTypesForOption(int optionId)
    {
        if (OptionOptionTypeVM.OptionOptionTypes == null || FoodTypeVM.FoodTypes == null)
            return new List<FoodOptionTypeDto>();

        var typeIds = OptionOptionTypeVM.OptionOptionTypes
            .Where(oot => oot.FoodOptionId == optionId)
            .Select(oot => oot.FoodOptionTypeId)
            .ToList();

        return FoodTypeVM.FoodTypes
            .Where(ft => typeIds.Contains(ft.Id))
            .ToList();
    }

    private void ShowCreateTypeModal()
    {
        SelectedFoodType = new FoodOptionTypeDto();
        ShowTypeModal = true;
    }

    private void ShowEditTypeModal(FoodOptionTypeDto foodType)
    {
        SelectedFoodType = new FoodOptionTypeDto
        {
            Id = foodType.Id,
            FoodOptionTypeName = foodType.FoodOptionTypeName,
            NumIncluded = foodType.NumIncluded,
            MaxAmount = foodType.MaxAmount,
            FoodOptionPrice = foodType.FoodOptionPrice,
            EntreeId = foodType.EntreeId,
            SideId = foodType.SideId
        };
        ShowTypeModal = true;
    }

    private void ShowDeleteTypeConfirmation(FoodOptionTypeDto foodType)
    {
        FoodTypeToDelete = foodType;
        ShowDeleteTypeModal = true;
    }

    private void ShowCreateOptionModal(int foodTypeId)
    {
        CurrentFoodTypeId = foodTypeId;
        SelectedFoodOption = new FoodOptionDto();
        ShowOptionModal = true;
    }

    private void ShowEditOptionModal(FoodOptionDto foodOption, int foodTypeId)
    {
        CurrentFoodTypeId = foodTypeId;
        SelectedFoodOption = new FoodOptionDto
        {
            Id = foodOption.Id,
            FoodOptionName = foodOption.FoodOptionName,
            InStock = foodOption.InStock,
            ImageUrl = foodOption.ImageUrl
        };
        ShowOptionModal = true;
    }

    private void ShowCreateStandaloneOptionModal()
    {
        CurrentFoodTypeId = 0;
        SelectedFoodOption = new FoodOptionDto();
        ShowOptionModal = true;
    }

    private void ShowEditStandaloneOptionModal(FoodOptionDto foodOption)
    {
        CurrentFoodTypeId = 0;
        SelectedFoodOption = new FoodOptionDto
        {
            Id = foodOption.Id,
            FoodOptionName = foodOption.FoodOptionName,
            InStock = foodOption.InStock,
            ImageUrl = foodOption.ImageUrl
        };
        ShowOptionModal = true;
    }

    private void ShowDeleteOptionConfirmation(FoodOptionDto foodOption)
    {
        FoodOptionToDelete = foodOption;
        ShowDeleteOptionModal = true;
    }

    private void ShowRemoveOptionFromType(FoodOptionDto foodOption, int foodTypeId)
    {
        OptionToRemoveId = foodOption.Id;
        TypeToRemoveFromId = foodTypeId;
        ShowRemoveFromTypeModal = true;
    }

    private async Task HandleOptionSave()
    {
        try
        {
            var isEdit = SelectedFoodOption?.Id > 0;
            var foodOptionName = SelectedFoodOption?.FoodOptionName ?? "Food Option";

            if (!isEdit && CurrentFoodTypeId > 0 && SelectedFoodOption != null)
            {
                await FoodOptionVM.InitializeFoodOptionsAsync();
            }

            await FoodOptionVM.InitializeFoodOptionsAsync();
            await OptionOptionTypeVM.InitializeOptionOptionTypesAsync();

            toastMessage = isEdit
                ? $"'{foodOptionName}' has been updated successfully."
                : $"'{foodOptionName}' has been created successfully.";
            toastType = ToastType.Success;
            showToast = true;

            ShowOptionModal = false;
            SelectedFoodOption = null;
            CurrentFoodTypeId = 0;
            StateHasChanged();
        }
        catch
        {
            ShowOptionModal = false;
            SelectedFoodOption = null;
            CurrentFoodTypeId = 0;
        }
    }

    private void HandleOptionCancel()
    {
        ShowOptionModal = false;
        SelectedFoodOption = null;
        CurrentFoodTypeId = 0;
    }

    private async Task HandleTypeSave()
    {
        try
        {
            var isEdit = SelectedFoodType?.Id > 0;
            var typeName = SelectedFoodType?.FoodOptionTypeName ?? "Food Type";

            await FoodTypeVM.InitializeFoodTypesAsync();

            toastMessage = isEdit
                ? $"'{typeName}' has been updated successfully."
                : $"'{typeName}' has been created successfully.";
            toastType = ToastType.Success;
            showToast = true;

            ShowTypeModal = false;
            SelectedFoodType = null;
            StateHasChanged();
        }
        catch
        {
            ShowTypeModal = false;
            SelectedFoodType = null;
        }
    }

    private void HandleTypeCancel()
    {
        ShowTypeModal = false;
        SelectedFoodType = null;
    }
    private async Task ConfirmDeleteOption()
    {
        ShowDeleteOptionModal = false;

        if (FoodOptionToDelete != null)
        {
            try
            {
                var foodOptionName = FoodOptionToDelete.FoodOptionName;

                if (await FoodOptionVM.DeleteFoodOptionAsync(FoodOptionToDelete.Id))
                {
                    await FoodOptionVM.InitializeFoodOptionsAsync();
                    await OptionOptionTypeVM.InitializeOptionOptionTypesAsync();

                    toastMessage = $"'{foodOptionName}' has been deleted successfully.";
                    toastType = ToastType.Success;
                    showToast = true;

                    StateHasChanged();
                }
            }
            catch
            {
            }
            finally
            {
                FoodOptionToDelete = null;
            }
        }
    }

    private void CancelDeleteOption()
    {
        ShowDeleteOptionModal = false;
        FoodOptionToDelete = null;
    }

    private async Task ConfirmDeleteType()
    {
        ShowDeleteTypeModal = false;

        if (FoodTypeToDelete != null)
        {
            try
            {
                var typeName = FoodTypeToDelete.FoodOptionTypeName;

                if (await FoodTypeVM.DeleteFoodTypeAsync(FoodTypeToDelete.Id))
                {
                    await FoodTypeVM.InitializeFoodTypesAsync();
                    await OptionOptionTypeVM.InitializeOptionOptionTypesAsync();

                    toastMessage = $"'{typeName}' has been deleted successfully.";
                    toastType = ToastType.Success;
                    showToast = true;

                    StateHasChanged();
                }
            }
            catch
            {
            }
            finally
            {
                FoodTypeToDelete = null;
            }
        }
    }

    private void CancelDeleteType()
    {
        ShowDeleteTypeModal = false;
        FoodTypeToDelete = null;
    }

    private async Task ConfirmRemoveFromType()
    {
        ShowRemoveFromTypeModal = false;

        if (OptionToRemoveId > 0 && TypeToRemoveFromId > 0)
        {
            try
            {
                var mapping = OptionOptionTypeVM.OptionOptionTypes?
                    .FirstOrDefault(oot => oot.FoodOptionId == OptionToRemoveId &&
                                          oot.FoodOptionTypeId == TypeToRemoveFromId);

                if (mapping != null)
                {
                    await OptionOptionTypeVM.DeleteOptionOptionTypeAsync(mapping.Id);
                    await OptionOptionTypeVM.InitializeOptionOptionTypesAsync();

                    toastMessage = "Food option removed from type successfully.";
                    toastType = ToastType.Success;
                    showToast = true;

                    StateHasChanged();
                }
            }
            catch
            {
            }
            finally
            {
                OptionToRemoveId = 0;
                TypeToRemoveFromId = 0;
            }
        }
    }

    private void CancelRemoveFromType()
    {
        ShowRemoveFromTypeModal = false;
        OptionToRemoveId = 0;
        TypeToRemoveFromId = 0;
    }
}
