using Microsoft.AspNetCore.Mvc;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Gdn.Web.Fluentblazor.Extensions;

public static class DialogServiceExtensions
{
    public static async Task ShowResponseErrorAsync(this IDialogService dialogService, HttpResponseMessage responseMessage)
    {
        if (responseMessage.IsSuccessStatusCode)
            return;

        var problemDetails = await responseMessage.Content.ReadFromJsonAsync<ProblemDetails>();
        if (problemDetails is null)
            return;

        string errorMessage = problemDetails?.Detail ?? "Si è verificato un errore non previsto.";
        await dialogService.ShowErrorAsync(errorMessage, "Errore");
    }
}
