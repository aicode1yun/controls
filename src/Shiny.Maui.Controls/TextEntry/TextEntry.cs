using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace Shiny.Maui.Controls;

[ContentProperty(nameof(RightTools))]
public partial class TextEntry : ContentView
{
    const double PlaceholderTranslationY = -16;
    const double PlaceholderScaledSize = 0.8;
    const uint AnimationDuration = 150;

    readonly Border outerBorder;
    readonly Microsoft.Maui.Controls.Shapes.RoundRectangle borderShape;
    readonly Grid contentGrid;
    readonly HorizontalStackLayout leftToolsLayout;
    readonly HorizontalStackLayout rightToolsLayout;
    readonly BoxView leftSeparator;
    readonly BoxView rightSeparator;
    readonly Label placeholderLabel;
    readonly BorderlessEntry entry;
    readonly Label hintLabel;
    readonly Grid rootGrid;

    bool suppressTextChanged;
    bool isPlaceholderUp;

    // Internal event for tools (like ClearButtonTool) to observe text changes
    internal event EventHandler? InternalTextChanged;

    public TextEntry()
    {
        placeholderLabel = new Label
        {
            FontSize = 15,
            TextColor = Colors.Grey,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
            InputTransparent = true,
            AnchorX = 0 // Scale from left edge
        };

        entry = new BorderlessEntry
        {
            FontSize = 15,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Fill,
            BackgroundColor = Colors.Transparent
        };
        entry.TextChanged += OnEntryTextChanged;
        entry.Focused += OnEntryFocused;
        entry.Unfocused += OnEntryUnfocused;
        entry.Completed += OnEntryCompleted;

        var entryArea = new Grid
        {
            Padding = new Thickness(12, 8),
            Children = { placeholderLabel, entry }
        };

        leftToolsLayout = new HorizontalStackLayout
        {
            Spacing = 0,
            VerticalOptions = LayoutOptions.Fill,
            BackgroundColor = Color.FromArgb("#F8F9FA")
        };

        rightToolsLayout = new HorizontalStackLayout
        {
            Spacing = 0,
            VerticalOptions = LayoutOptions.Fill,
            BackgroundColor = Color.FromArgb("#F8F9FA")
        };

        leftSeparator = new BoxView
        {
            WidthRequest = 1,
            VerticalOptions = LayoutOptions.Fill,
            Color = Color.FromArgb("#CCCCCC"),
            IsVisible = false
        };

        rightSeparator = new BoxView
        {
            WidthRequest = 1,
            VerticalOptions = LayoutOptions.Fill,
            Color = Color.FromArgb("#CCCCCC"),
            IsVisible = false
        };

        contentGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),  // left tools
                new ColumnDefinition(GridLength.Auto),  // left separator
                new ColumnDefinition(GridLength.Star),  // entry area
                new ColumnDefinition(GridLength.Auto),  // right separator
                new ColumnDefinition(GridLength.Auto)   // right tools
            },
            ColumnSpacing = 0,
            Padding = 0
        };
        contentGrid.Add(leftToolsLayout, 0, 0);
        contentGrid.Add(leftSeparator, 1, 0);
        contentGrid.Add(entryArea, 2, 0);
        contentGrid.Add(rightSeparator, 3, 0);
        contentGrid.Add(rightToolsLayout, 4, 0);

        borderShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 };
        outerBorder = new Border
        {
            StrokeShape = borderShape,
            Stroke = Color.FromArgb("#CCCCCC"),
            StrokeThickness = 1,
            BackgroundColor = Colors.Transparent,
            Padding = 0,
            Content = contentGrid,
            MinimumHeightRequest = 48
        };

        hintLabel = new Label
        {
            FontSize = 12,
            TextColor = Colors.Grey,
            Margin = new Thickness(12, 2, 12, 0),
            IsVisible = false
        };

        rootGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            }
        };
        rootGrid.Add(outerBorder, 0, 0);
        rootGrid.Add(hintLabel, 0, 1);

        Content = rootGrid;

        // Initialize tool collections
        LeftTools = new ObservableCollection<TextEntryTool>();
        RightTools = new ObservableCollection<TextEntryTool>();
    }

    void OnEntryTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (suppressTextChanged) return;

        suppressTextChanged = true;

        if (!string.IsNullOrEmpty(Mask))
        {
            var rawText = TextEntryMaskHelper.StripMask(entry.Text, Mask);
            var maxRaw = TextEntryMaskHelper.CalculateRawMaxLength(Mask);
            if (rawText.Length > maxRaw)
                rawText = rawText[..maxRaw];

            Text = rawText;
            var formatted = TextEntryMaskHelper.ApplyMask(rawText, Mask);
            FormattedText = formatted;
            entry.Text = formatted;

            // Set cursor position after formatting
            var cursorPos = TextEntryMaskHelper.CalculateCursorPosition(rawText.Length, Mask);
            Dispatcher.Dispatch(() => entry.CursorPosition = Math.Min(cursorPos, formatted.Length));
        }
        else
        {
            Text = entry.Text ?? string.Empty;
        }

        suppressTextChanged = false;

        InternalTextChanged?.Invoke(this, EventArgs.Empty);
        TextChanged?.Invoke(this, new TextChangedEventArgs(e.OldTextValue, Text));
        if (TextChangedCommand?.CanExecute(Text) == true)
            TextChangedCommand.Execute(Text);

        UpdateCharacterCount();
    }

    void OnEntryFocused(object? sender, FocusEventArgs e)
    {
        AnimatePlaceholder(true);
        var color = HasError ? ErrorColor : FocusedBorderColor;
        outerBorder.Stroke = color;
        outerBorder.StrokeThickness = FocusedBorderThickness;
        UpdateSeparatorColors(color);
    }

    void OnEntryUnfocused(object? sender, FocusEventArgs e)
    {
        if (string.IsNullOrEmpty(entry.Text))
            AnimatePlaceholder(false);

        var color = HasError ? ErrorColor : BorderColor;
        outerBorder.Stroke = color;
        outerBorder.StrokeThickness = BorderThickness;
        UpdateSeparatorColors(color);
    }

    void UpdateSeparatorColors(Color color)
    {
        leftSeparator.Color = color;
        rightSeparator.Color = color;
    }

    void OnEntryCompleted(object? sender, EventArgs e)
    {
        Completed?.Invoke(this, e);
        if (CompletedCommand?.CanExecute(Text) == true)
            CompletedCommand.Execute(Text);
    }

    async void AnimatePlaceholder(bool up)
    {
        if (up == isPlaceholderUp) return;
        isPlaceholderUp = up;

        if (up)
        {
            await Task.WhenAll(
                placeholderLabel.TranslateToAsync(0, PlaceholderTranslationY, AnimationDuration, Easing.CubicOut),
                placeholderLabel.ScaleToAsync(PlaceholderScaledSize, AnimationDuration, Easing.CubicOut)
            );
            placeholderLabel.TextColor = HasError ? ErrorColor : FocusedPlaceholderColor;
        }
        else
        {
            await Task.WhenAll(
                placeholderLabel.TranslateToAsync(0, 0, AnimationDuration, Easing.CubicOut),
                placeholderLabel.ScaleToAsync(1, AnimationDuration, Easing.CubicOut)
            );
            placeholderLabel.TextColor = PlaceholderColor;
        }
    }

    void UpdateCharacterCount()
    {
        if (!ShowCharacterCount || MaxLength <= 0) return;
        var count = entry.Text?.Length ?? 0;
        // Show in hint when no error
        if (!HasError)
        {
            hintLabel.Text = $"{count}/{MaxLength}";
            hintLabel.TextColor = count >= MaxLength ? ErrorColor : HintColor;
            hintLabel.IsVisible = true;
        }
    }

    void SyncHint()
    {
        Color borderColor;
        if (HasError && !string.IsNullOrEmpty(HintText))
        {
            hintLabel.Text = HintText;
            hintLabel.TextColor = ErrorColor;
            hintLabel.IsVisible = true;
            borderColor = ErrorColor;
        }
        else if (!string.IsNullOrEmpty(HintText))
        {
            hintLabel.Text = HintText;
            hintLabel.TextColor = HintColor;
            hintLabel.IsVisible = true;
            borderColor = entry.IsFocused ? FocusedBorderColor : BorderColor;
        }
        else if (ShowCharacterCount && MaxLength > 0)
        {
            UpdateCharacterCount();
            borderColor = entry.IsFocused ? FocusedBorderColor : BorderColor;
        }
        else
        {
            hintLabel.IsVisible = false;
            borderColor = entry.IsFocused ? FocusedBorderColor : BorderColor;
        }

        outerBorder.Stroke = borderColor;
        UpdateSeparatorColors(borderColor);
    }

    // Tool collection management
    void OnToolsChanged(IList<TextEntryTool>? oldTools, IList<TextEntryTool>? newTools, HorizontalStackLayout layout)
    {
        if (oldTools is INotifyCollectionChanged oldNcc)
            oldNcc.CollectionChanged -= (_, _) => RebuildTools(newTools, layout);

        DetachTools(oldTools);
        RebuildTools(newTools, layout);
        AttachTools(newTools);

        if (newTools is INotifyCollectionChanged ncc)
            ncc.CollectionChanged += (_, _) =>
            {
                DetachTools(newTools);
                RebuildTools(newTools, layout);
                AttachTools(newTools);
            };
    }

    void RebuildTools(IList<TextEntryTool>? tools, HorizontalStackLayout layout)
    {
        layout.Children.Clear();
        if (tools is null || tools.Count == 0)
        {
            layout.IsVisible = false;
            if (layout == leftToolsLayout)
                leftSeparator.IsVisible = false;
            else
                rightSeparator.IsVisible = false;
            return;
        }

        layout.IsVisible = true;
        if (layout == leftToolsLayout)
            leftSeparator.IsVisible = true;
        else
            rightSeparator.IsVisible = true;

        foreach (var tool in tools)
        {
            tool.ParentEntry = this;
            layout.Children.Add(tool);
        }
    }

    void AttachTools(IList<TextEntryTool>? tools)
    {
        if (tools is null) return;
        foreach (var tool in tools)
        {
            if (tool is ITextEntryAwareTool aware)
                aware.Attach(this);
        }
    }

    void DetachTools(IList<TextEntryTool>? tools)
    {
        if (tools is null) return;
        foreach (var tool in tools)
        {
            if (tool is ITextEntryAwareTool aware)
                aware.Detach();
        }
    }

    void OnMaskChanged()
    {
        if (!string.IsNullOrEmpty(Mask))
        {
            entry.Keyboard = Keyboard.Numeric;
            entry.MaxLength = Mask.Length;

            // Reformat existing text
            if (!string.IsNullOrEmpty(Text))
            {
                suppressTextChanged = true;
                var formatted = TextEntryMaskHelper.ApplyMask(Text, Mask);
                FormattedText = formatted;
                entry.Text = formatted;
                suppressTextChanged = false;
            }
        }
        else
        {
            // Mask removed - restore raw text to entry
            entry.MaxLength = MaxLength;
            suppressTextChanged = true;
            entry.Text = Text;
            FormattedText = string.Empty;
            suppressTextChanged = false;
        }
    }

    void ApplyMaskToEntry()
    {
        if (string.IsNullOrEmpty(Mask)) return;

        var formatted = TextEntryMaskHelper.ApplyMask(Text, Mask);
        FormattedText = formatted;

        suppressTextChanged = true;
        entry.Text = formatted;
        suppressTextChanged = false;

        if (!string.IsNullOrEmpty(formatted) && !isPlaceholderUp)
            AnimatePlaceholder(true);
        else if (string.IsNullOrEmpty(formatted) && !entry.IsFocused && isPlaceholderUp)
            AnimatePlaceholder(false);
    }

    // Public API
    public event EventHandler<TextChangedEventArgs>? TextChanged;
    public event EventHandler? Completed;

    public new bool Focus() => entry.Focus();
    public new void Unfocus() => entry.Unfocus();
}
