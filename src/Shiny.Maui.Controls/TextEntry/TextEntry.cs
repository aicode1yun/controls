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
            Padding = new Thickness(0, 8),
            Children = { placeholderLabel, entry }
        };

        leftToolsLayout = new HorizontalStackLayout
        {
            Spacing = 2,
            VerticalOptions = LayoutOptions.Center
        };

        rightToolsLayout = new HorizontalStackLayout
        {
            Spacing = 2,
            VerticalOptions = LayoutOptions.Center
        };

        contentGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 4,
            Padding = new Thickness(12, 0)
        };
        contentGrid.Add(leftToolsLayout, 0, 0);
        contentGrid.Add(entryArea, 1, 0);
        contentGrid.Add(rightToolsLayout, 2, 0);

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
        Text = entry.Text ?? string.Empty;
        suppressTextChanged = false;

        InternalTextChanged?.Invoke(this, EventArgs.Empty);
        TextChanged?.Invoke(this, e);
        if (TextChangedCommand?.CanExecute(e.NewTextValue) == true)
            TextChangedCommand.Execute(e.NewTextValue);

        UpdateCharacterCount();
    }

    void OnEntryFocused(object? sender, FocusEventArgs e)
    {
        AnimatePlaceholder(true);
        outerBorder.Stroke = FocusedBorderColor;
        outerBorder.StrokeThickness = FocusedBorderThickness;

        if (HasError)
            outerBorder.Stroke = ErrorColor;
    }

    void OnEntryUnfocused(object? sender, FocusEventArgs e)
    {
        if (string.IsNullOrEmpty(entry.Text))
            AnimatePlaceholder(false);

        outerBorder.Stroke = HasError ? ErrorColor : BorderColor;
        outerBorder.StrokeThickness = BorderThickness;
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
        if (HasError && !string.IsNullOrEmpty(HintText))
        {
            hintLabel.Text = HintText;
            hintLabel.TextColor = ErrorColor;
            hintLabel.IsVisible = true;
            outerBorder.Stroke = ErrorColor;
        }
        else if (!string.IsNullOrEmpty(HintText))
        {
            hintLabel.Text = HintText;
            hintLabel.TextColor = HintColor;
            hintLabel.IsVisible = true;
            outerBorder.Stroke = entry.IsFocused ? FocusedBorderColor : BorderColor;
        }
        else if (ShowCharacterCount && MaxLength > 0)
        {
            UpdateCharacterCount();
            outerBorder.Stroke = entry.IsFocused ? FocusedBorderColor : BorderColor;
        }
        else
        {
            hintLabel.IsVisible = false;
            outerBorder.Stroke = entry.IsFocused ? FocusedBorderColor : BorderColor;
        }
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
        if (tools is null) return;
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

    // Public API
    public event EventHandler<TextChangedEventArgs>? TextChanged;
    public event EventHandler? Completed;

    public new bool Focus() => entry.Focus();
    public new void Unfocus() => entry.Unfocus();
}
