using Microsoft.AspNetCore.Components;

namespace Shiny.Blazor.Controls;

public partial class LoadingOverlay
{
    [Parameter] public bool IsShown { get; set; }
    [Parameter] public EventCallback<bool> IsShownChanged { get; set; }

    [Parameter] public string OverlayColor { get; set; } = "rgba(0, 0, 0, 0.5)";
    [Parameter] public double BlurRadius { get; set; }

    [Parameter] public bool IsIndeterminate { get; set; } = true;
    [Parameter] public double Progress { get; set; }

    [Parameter] public string? Message { get; set; }
    [Parameter] public string MessageColor { get; set; } = "#FFFFFF";

    [Parameter] public string SpinnerColor { get; set; } = "#FFFFFF";
    [Parameter] public double SpinnerSize { get; set; } = 48;

    [Parameter] public string ProgressBarColor { get; set; } = "#FFFFFF";
    [Parameter] public string ProgressTrackColor { get; set; } = "rgba(255, 255, 255, 0.2)";

    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string? CssClass { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? AdditionalAttributes { get; set; }

    string SpinnerStyle => $"width: {SpinnerSize}px; height: {SpinnerSize}px; border-color: {SpinnerColor} transparent transparent transparent;";
}
