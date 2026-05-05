namespace Shiny.Blazor.Controls;

/// <summary>
/// A TextEntry tool that increments or decrements the numeric value in the entry.
/// If Text is not set, displays the step value with +/- prefix (e.g. "+1" or "-5").
/// </summary>
public class TextEntryStepperTool : TextEntryTool
{
    /// <summary>
    /// The amount to increment (positive) or decrement (negative) the entry value.
    /// </summary>
    public double Step { get; set; } = 1;

    string DefaultDisplayText => Step >= 0 ? $"+{Step:G}" : $"{Step:G}";

    public override void OnAttached(TextEntryContext context)
    {
        if (string.IsNullOrEmpty(Text))
            Text = DefaultDisplayText;
    }

    protected override void OnClick()
    {
        var currentText = GetEntryText();
        double currentValue = 0;

        if (!string.IsNullOrEmpty(currentText) && double.TryParse(currentText, out var parsed))
            currentValue = parsed;

        var newValue = currentValue + Step;
        SetEntryText(newValue.ToString("G"));
    }
}
