using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.Enums;
using Cafeteria.Management.Services.Locations;
using Cafeteria.Management.Services.Stations;
using static Cafeteria.Management.Components.Shared.Toast;

namespace Cafeteria.Management.Components.Pages.LocationAndStation;

public partial class BusinessHoursEditor : ComponentBase
{
    [Inject]
    public ILocationService LocationService { get; set; } = default!;

    [Inject]
    public IStationService StationService { get; set; } = default!;

    [Parameter, EditorRequired]
    public int EntityId { get; set; }

    [Parameter]
    public bool IsStation { get; set; }

    [Parameter]
    public EventCallback OnHoursChanged { get; set; }

    private record HoursEntry(int Id, int WeekdayId, TimeOnly OpenTime, TimeOnly CloseTime);

    private List<HoursEntry> Hours { get; set; } = [];

    // Modal state
    private bool ShowModal { get; set; }
    private bool IsEditing { get; set; }
    private int EditingHoursId { get; set; }
    private int EditingWeekdayId { get; set; }
    private TimeOnly EditOpenTime { get; set; }
    private TimeOnly EditCloseTime { get; set; }

    // Delete confirmation state
    private bool ShowDeleteConfirmation { get; set; }
    private HoursEntry? DeletingHours { get; set; }

    // Toast state
    private bool ShowToast { get; set; }
    private string ToastMessage { get; set; } = string.Empty;
    private ToastType CurrentToastType { get; set; } = ToastType.Success;

    protected override async Task OnInitializedAsync()
    {
        await LoadHours();
    }

    private async Task LoadHours()
    {
        if (IsStation)
        {
            var stationHours = await StationService.GetStationBusinessHours(EntityId);
            Hours = stationHours.Select(h => new HoursEntry(h.Id, h.WeekdayId, h.OpenTime, h.CloseTime)).ToList();
        }
        else
        {
            var locationHours = await LocationService.GetLocationBusinessHours(EntityId);
            Hours = locationHours.Select(h => new HoursEntry(h.Id, h.WeekdayId, h.OpenTime, h.CloseTime)).ToList();
        }
    }

    private HoursEntry? GetHoursForDay(WeekDay day)
    {
        return Hours.FirstOrDefault(h => h.WeekdayId == (int)day);
    }

    private void HandleAdd(WeekDay day)
    {
        IsEditing = false;
        EditingWeekdayId = (int)day;
        EditOpenTime = new TimeOnly(8, 0);
        EditCloseTime = new TimeOnly(17, 0);
        ShowModal = true;
    }

    private void HandleEdit(HoursEntry entry)
    {
        IsEditing = true;
        EditingHoursId = entry.Id;
        EditingWeekdayId = entry.WeekdayId;
        EditOpenTime = entry.OpenTime;
        EditCloseTime = entry.CloseTime;
        ShowModal = true;
    }

    private async Task SaveHours()
    {
        if (EditCloseTime <= EditOpenTime)
        {
            ShowToastMessage("Close time must be after open time.", ToastType.Error);
            return;
        }

        try
        {
            if (IsEditing)
            {
                if (IsStation)
                    await StationService.UpdateStationBusinessHours(EditingHoursId, EditOpenTime, EditCloseTime, EditingWeekdayId);
                else
                    await LocationService.UpdateLocationBusinessHours(EditingHoursId, EditOpenTime, EditCloseTime, EditingWeekdayId);

                ShowToastMessage("Business hours updated successfully.", ToastType.Success);
            }
            else
            {
                if (IsStation)
                    await StationService.AddStationBusinessHours(EntityId, EditOpenTime, EditCloseTime, EditingWeekdayId);
                else
                    await LocationService.AddLocationBusinessHours(EntityId, EditOpenTime, EditCloseTime, EditingWeekdayId);

                ShowToastMessage("Business hours added successfully.", ToastType.Success);
            }

            ShowModal = false;
            await LoadHours();
            await OnHoursChanged.InvokeAsync();
        }
        catch (Exception)
        {
            ShowToastMessage("Failed to save business hours.", ToastType.Error);
        }
    }

    private void HandleDelete(HoursEntry entry)
    {
        DeletingHours = entry;
        ShowDeleteConfirmation = true;
    }

    private async Task ConfirmDelete()
    {
        if (DeletingHours is null) return;

        try
        {
            bool success;
            if (IsStation)
                success = await StationService.DeleteStationBusinessHours(DeletingHours.Id);
            else
                success = await LocationService.DeleteLocationBusinessHours(DeletingHours.Id);

            if (success)
            {
                ShowToastMessage("Business hours deleted.", ToastType.Success);
                await LoadHours();
                await OnHoursChanged.InvokeAsync();
            }
            else
            {
                ShowToastMessage("Failed to delete business hours.", ToastType.Error);
            }
        }
        catch (Exception)
        {
            ShowToastMessage("Failed to delete business hours.", ToastType.Error);
        }

        ShowDeleteConfirmation = false;
        DeletingHours = null;
    }

    private void CancelDelete()
    {
        ShowDeleteConfirmation = false;
        DeletingHours = null;
    }

    private void CloseModal()
    {
        ShowModal = false;
    }

    private void ShowToastMessage(string message, ToastType type)
    {
        ToastMessage = message;
        CurrentToastType = type;
        ShowToast = true;
    }
}
