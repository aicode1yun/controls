using Microsoft.AspNetCore.Components;

namespace Shiny.Blazor.Controls;

public partial class Overlay
{
    bool isVisible;

    [Parameter] public bool IsShown { get; set; }
    [Parameter] public EventCallback<bool> IsShownChanged { get; set; }

    [Parameter] public string OverlayColor { get; set; } = "rgba(0, 0, 0, 0.5)";
    [Parameter] public double OverlayOpacity { get; set; } = 1.0;
    [Parameter] public double BlurRadius { get; set; }

    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public RenderFragment? OverlayContent { get; set; }

    [Parameter] public string? CssClass { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? AdditionalAttributes { get; set; }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        var wasShown = IsShown;
        await base.SetParametersAsync(parameters);

        if (IsShown && !wasShown)
        {
            // Small delay for CSS transition to kick in
            _ = Task.Run(async () =>
            {
                await Task.Delay(16);
                isVisible = true;
                await InvokeAsync(StateHasChanged);
            });
        }
        else if (!IsShown && wasShown)
        {
            isVisible = false;
        }
        else if (IsShown)
        {
            isVisible = true;
        }
    }
}
