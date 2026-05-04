using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Shiny.Blazor.Controls;

public partial class TextEntry : IDisposable
{
    ElementReference inputRef;
    bool IsFocused;
    TextEntryContext? context;
    bool IsPlaceholderUp => IsFocused || !string.IsNullOrEmpty(Text);

    // Parameters
    [Parameter] public string Text { get; set; } = "";
    [Parameter] public EventCallback<string> TextChanged { get; set; }
    [Parameter] public string Placeholder { get; set; } = "";
    [Parameter] public string PlaceholderColor { get; set; } = "#9CA3AF";
    [Parameter] public string FocusedPlaceholderColor { get; set; } = "#007AFF";
    [Parameter] public string BorderColor { get; set; } = "#CCCCCC";
    [Parameter] public string FocusedBorderColor { get; set; } = "#007AFF";
    [Parameter] public double BorderThickness { get; set; } = 1;
    [Parameter] public double FocusedBorderThickness { get; set; } = 2;
    [Parameter] public string CornerRadius { get; set; } = "8px";
    [Parameter] public string EntryBackgroundColor { get; set; } = "transparent";
    [Parameter] public double FontSize { get; set; } = 15;
    [Parameter] public string FontFamily { get; set; } = "inherit";
    [Parameter] public string TextColor { get; set; } = "inherit";
    [Parameter] public bool IsReadOnly { get; set; }
    [Parameter] public bool IsPassword { get; set; }
    [Parameter] public int MaxLength { get; set; }
    [Parameter] public string? HintText { get; set; }
    [Parameter] public string HintColor { get; set; } = "#9CA3AF";
    [Parameter] public bool HasError { get; set; }
    [Parameter] public string ErrorColor { get; set; } = "#DC3545";
    [Parameter] public bool ShowCharacterCount { get; set; }
    [Parameter] public List<TextEntryTool>? LeftTools { get; set; }
    [Parameter] public List<TextEntryTool>? RightTools { get; set; }
    [Parameter] public string? CssClass { get; set; }
    [Parameter] public EventCallback Completed { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? AdditionalAttributes { get; set; }

    string InputType => IsPassword ? "password" : "text";

    string HintDisplay
    {
        get
        {
            if (HasError && !string.IsNullOrEmpty(HintText))
                return HintText;
            if (!string.IsNullOrEmpty(HintText))
                return HintText;
            if (ShowCharacterCount && MaxLength > 0)
                return $"{(Text?.Length ?? 0)}/{MaxLength}";
            return "";
        }
    }

    string RootStyle => "";

    string BorderStyle
    {
        get
        {
            var color = HasError ? ErrorColor : (IsFocused ? FocusedBorderColor : BorderColor);
            var thickness = IsFocused ? FocusedBorderThickness : BorderThickness;
            return $"border: {thickness}px solid {color}; border-radius: {CornerRadius}; background: {EntryBackgroundColor};";
        }
    }

    string PlaceholderStyle
    {
        get
        {
            var color = HasError ? ErrorColor : (IsPlaceholderUp ? FocusedPlaceholderColor : PlaceholderColor);
            return $"color: {color};";
        }
    }

    string InputStyle => $"font-size: {FontSize}px; font-family: {FontFamily}; color: {TextColor};";

    string HintStyle
    {
        get
        {
            var color = HasError ? ErrorColor : HintColor;
            return $"color: {color};";
        }
    }

    protected override void OnInitialized()
    {
        context = new TextEntryContext(() => Text, SetTextFromTool);
        AttachTools(LeftTools);
        AttachTools(RightTools);
    }

    protected override void OnParametersSet()
    {
        // Re-attach if tool lists changed
        AttachTools(LeftTools);
        AttachTools(RightTools);
    }

    async Task OnInput(ChangeEventArgs e)
    {
        Text = e.Value?.ToString() ?? "";
        await TextChanged.InvokeAsync(Text);
        NotifyToolsTextChanged();
    }

    void OnFocusIn()
    {
        IsFocused = true;
    }

    async Task OnFocusOut()
    {
        IsFocused = false;
        StateHasChanged();
        await Task.CompletedTask;
    }

    async Task OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
            await Completed.InvokeAsync();
    }

    async Task FocusInput()
    {
        try { await inputRef.FocusAsync(); } catch { }
    }

    void OnToolClicked(TextEntryTool tool)
    {
        tool.InternalClick();
        StateHasChanged();
    }

    async void SetTextFromTool(string text)
    {
        Text = text;
        await TextChanged.InvokeAsync(text);
        NotifyToolsTextChanged();
        StateHasChanged();
    }

    void NotifyToolsTextChanged()
    {
        NotifyToolsInList(LeftTools);
        NotifyToolsInList(RightTools);
    }

    void NotifyToolsInList(List<TextEntryTool>? tools)
    {
        if (tools is null) return;
        foreach (var tool in tools)
            tool.OnTextChanged(Text);
    }

    void AttachTools(List<TextEntryTool>? tools)
    {
        if (tools is null || context is null) return;
        foreach (var tool in tools)
        {
            if (tool._context is null)
                tool.InternalAttach(context);
        }
    }

    void DetachTools(List<TextEntryTool>? tools)
    {
        if (tools is null) return;
        foreach (var tool in tools)
            tool.InternalDetach();
    }

    public void Dispose()
    {
        DetachTools(LeftTools);
        DetachTools(RightTools);
    }
}
