using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Management.Services.SchedulingExceptions;
using static Cafeteria.Management.Components.Shared.Toast;

namespace Cafeteria.Management.Components.Pages.LocationAndStation;

public partial class SchedulingExceptionsEditor : ComponentBase
{
    [Inject]
    public ISchedulingExceptionsService SchedulingExceptionsService { get; set; } = default!;

    [Parameter, EditorRequired]
    public int EntityId { get; set; }

    [Parameter]
    public bool IsStation { get; set; }

    [Parameter]
    public EventCallback OnExceptionsChanged { get; set; }

    private List<(int Id, DateTime StartDateTime, DateTime EndDateTime)> Exceptions { get; set; } = [];

    private bool ShowModal { get; set; }
    private bool IsEditing { get; set; }
    private int EditingExceptionId { get; set; }
    private DateTime EditStartDateTime { get; set; }
    private DateTime EditEndDateTime { get; set; }

    private bool ShowDeleteConfirmation { get; set; }
    private (int Id, DateTime Start, DateTime End)? DeletingException { get; set; }

    private bool ShowToast { get; set; }
    private string ToastMessage { get; set; } = string.Empty;
    private ToastType CurrentToastType { get; set; } = ToastType.Success;

    protected override async Task OnInitializedAsync()
    {
        await LoadExceptions();
    }

    private async Task LoadExceptions()
    {
        try
        {
            if (IsStation)
            {
                var stationExceptions = await SchedulingExceptionsService.GetStationExceptions(EntityId);
                Exceptions = stationExceptions
                    .OrderByDescending(e => e.StartExceptionDateTime)
                    .Select(e => (e.Id, e.StartExceptionDateTime, e.EndExceptionDateTime))
                    .ToList();
            }
            else
            {
                var locationExceptions = await SchedulingExceptionsService.GetLocationExceptions(EntityId);
                Exceptions = locationExceptions
                    .OrderByDescending(e => e.StartExceptionDateTime)
                    .Select(e => (e.Id, e.StartExceptionDateTime, e.EndExceptionDateTime))
                    .ToList();
            }
        }
        catch (Exception ex)
        {
            ShowToastMessage($"Error loading exceptions: {ex.Message}", ToastType.Error);
        }
    }

    private void HandleAdd()
    {
        IsEditing = false;
        EditStartDateTime = DateTime.Now;
        EditEndDateTime = DateTime.Now.AddHours(1);
        ShowModal = true;
    }

    private void HandleEdit((int Id, DateTime StartDateTime, DateTime EndDateTime) exception)
    {
        IsEditing = true;
        EditingExceptionId = exception.Id;
        EditStartDateTime = exception.StartDateTime;
        EditEndDateTime = exception.EndDateTime;
        ShowModal = true;
    }

    private async Task SaveException()
    {
        if (EditEndDateTime <= EditStartDateTime)
        {
            ShowToastMessage("End time must be after start time.", ToastType.Error);
            return;
        }

        try
        {
            if (IsEditing)
            {
                if (IsStation)
                    await SchedulingExceptionsService.UpdateStationException(EditingExceptionId, EditStartDateTime, EditEndDateTime);
                else
                    await SchedulingExceptionsService.UpdateLocationException(EditingExceptionId, EditStartDateTime, EditEndDateTime);

                ShowToastMessage("Exception updated successfully.", ToastType.Success);
            }
            else
            {
                if (IsStation)
                    await SchedulingExceptionsService.AddStationException(EntityId, EditStartDateTime, EditEndDateTime);
                else
                    await SchedulingExceptionsService.AddLocationException(EntityId, EditStartDateTime, EditEndDateTime);

                ShowToastMessage("Exception added successfully.", ToastType.Success);
            }

            ShowModal = false;
            await LoadExceptions();
            await OnExceptionsChanged.InvokeAsync();
        }
        catch (Exception ex)
        {
            ShowToastMessage($"Error saving exception: {ex.Message}", ToastType.Error);
        }
    }

    private void HandleDelete((int Id, DateTime StartDateTime, DateTime EndDateTime) exception)
    {
        DeletingException = exception;
        ShowDeleteConfirmation = true;
    }

    private async Task ConfirmDelete()
    {
        if (DeletingException is null) return;

        try
        {
            if (IsStation)
                await SchedulingExceptionsService.DeleteStationException(DeletingException.Value.Id);
            else
                await SchedulingExceptionsService.DeleteLocationException(DeletingException.Value.Id);

            ShowToastMessage("Exception deleted.", ToastType.Success);
            await LoadExceptions();
            await OnExceptionsChanged.InvokeAsync();
        }
        catch (Exception ex)
        {
            ShowToastMessage($"Error deleting exception: {ex.Message}", ToastType.Error);
        }

        ShowDeleteConfirmation = false;
        DeletingException = null;
    }

    private void CancelDelete()
    {
        ShowDeleteConfirmation = false;
        DeletingException = null;
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
