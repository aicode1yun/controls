using System.Collections.Specialized;
using Shiny.Maui.Controls.Chat.Internal;

namespace Shiny.Maui.Controls.Chat;

public partial class ChatView : ContentView
{
    public event EventHandler<ChatMessage>? MessageTapped;

    readonly CollectionView collectionView;
    readonly ChatInputBar inputBar;
    readonly VerticalStackLayout typingBubbleHost;
    readonly Border toastPill;
    readonly Label toastNewMessagesLabel;
    readonly Label toastTypingLabel;
    readonly Grid messageArea;
    readonly Grid rootGrid;
    readonly FabMenu toolsMenu;
    readonly FabMenu bubbleToolsMenu;
    ChatMessage? activeBubbleToolMessage;

    INotifyCollectionChanged? observedCollection;
    INotifyCollectionChanged? observedTypingCollection;
    INotifyCollectionChanged? observedToolItems;
    bool isLoadingMore;
    bool isNearBottom = true;
    int unreadCount;

    public ChatView()
    {
        collectionView = new CollectionView
        {
            ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
            {
                ItemSpacing = 0
            },
            ItemTemplate = new ChatBubbleTemplateSelector(this),
            RemainingItemsThreshold = 5
        };
        collectionView.RemainingItemsThresholdReached += OnRemainingItemsThresholdReached;
        collectionView.Scrolled += OnCollectionViewScrolled;

        // Shared toast pill — shows new messages on top, typing below
        toastNewMessagesLabel = new Label
        {
            TextColor = Colors.White,
            FontSize = 13,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            IsVisible = false
        };

        toastTypingLabel = new Label
        {
            TextColor = Color.FromArgb("#D0E8FF"),
            FontSize = 12,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            IsVisible = false
        };

        toastPill = new Border
        {
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
            BackgroundColor = Color.FromArgb("#007AFF"),
            Padding = new Thickness(16, 8),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 0, 0, 12),
            IsVisible = false,
            Content = new VerticalStackLayout
            {
                Spacing = 2,
                Children = { toastNewMessagesLabel, toastTypingLabel }
            }
        };

        var pillTap = new TapGestureRecognizer();
        pillTap.Tapped += OnToastPillTapped;
        toastPill.GestureRecognizers.Add(pillTap);

        // Messages area: CollectionView + toast pill overlay
        messageArea = new Grid
        {
            IsClippedToBounds = true,
            Children = { collectionView, toastPill }
        };

        // Typing bubble host — sits between messages and input bar, outside the CollectionView
        typingBubbleHost = new VerticalStackLayout
        {
            IsVisible = false,
            Spacing = 0
        };

        // Input bar: always visible, pinned at bottom
        inputBar = new ChatInputBar();
        inputBar.SendRequested += OnSendRequested;
        inputBar.AttachRequested += OnAttachRequested;
        inputBar.ToolsRequested += OnToolsButtonRequested;

        // Tools FabMenu overlay — spans full chat area for proper backdrop/item expansion
        toolsMenu = new FabMenu
        {
            IsVisible = false,
            FabSize = 40,
            HasShadow = false,
            HasBackdrop = true,
            CloseOnBackdropTap = true,
            CloseOnItemTap = true,
            MenuAlignment = LayoutOptions.Start,
            FabBackgroundColor = Color.FromArgb("#007AFF"),
            Margin = new Thickness(8, 0, 0, 6)
        };
        toolsMenu.ItemTapped += OnToolItemTapped;
        toolsMenu.PropertyChanged += OnToolsMenuPropertyChanged;

        // Bubble tools FabMenu overlay — shared across all bubbles, populated dynamically
        bubbleToolsMenu = new FabMenu
        {
            IsVisible = false,
            FabSize = 36,
            HasShadow = false,
            HasBackdrop = true,
            CloseOnBackdropTap = true,
            CloseOnItemTap = true,
            FabBackgroundColor = Color.FromArgb("#007AFF"),
            Text = "\u22ee"
        };
        bubbleToolsMenu.ItemTapped += OnBubbleToolItemTapped;
        bubbleToolsMenu.PropertyChanged += OnBubbleToolsMenuPropertyChanged;

        // Root: messages fill space, typing bubbles below, input bar at bottom
        rootGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            }
        };
        rootGrid.Add(messageArea, 0, 0);
        rootGrid.Add(typingBubbleHost, 0, 1);
        rootGrid.Add(inputBar, 0, 2);

        // FabMenu overlay spans all rows, anchored to bottom-left
        rootGrid.Add(toolsMenu, 0, 0);
        Grid.SetRowSpan(toolsMenu, 3);

        // Bubble tools FabMenu overlay spans all rows
        rootGrid.Add(bubbleToolsMenu, 0, 0);
        Grid.SetRowSpan(bubbleToolsMenu, 3);

        Content = rootGrid;

        // Initialize collections so XAML source generator can Add() items directly
        ToolItems = new System.Collections.ObjectModel.ObservableCollection<ChatEntryTool>();
        BubbleToolItems = new System.Collections.ObjectModel.ObservableCollection<ChatBubbleTool>();
        MyBubbleToolItems = new System.Collections.ObjectModel.ObservableCollection<ChatBubbleTool>();
    }

    /// <summary>
    /// Gets or sets the current text in the input bar entry field.
    /// </summary>
    public string EntryText
    {
        get => inputBar.EntryText;
        set => inputBar.EntryText = value;
    }

    /// <summary>
    /// Programmatically submits the current entry text as if the user pressed Send.
    /// </summary>
    public void SubmitEntry()
    {
        var text = inputBar.EntryText?.Trim();
        if (string.IsNullOrEmpty(text))
            return;

        inputBar.ClearText();
        OnSendRequested(text);
    }

    /// <summary>
    /// Displays a centered grid of tappable glyphs over the chat area and returns the
    /// glyph the user selects, or null if the backdrop is tapped to cancel.
    /// </summary>
    internal Task<string?> ShowGlyphPickerAsync(IReadOnlyList<string> glyphs, int columns = 6)
    {
        var tcs = new TaskCompletionSource<string?>();

        var backdrop = new BoxView
        {
            BackgroundColor = Color.FromRgba(0, 0, 0, 0.4),
            Opacity = 0
        };
        var backdropTap = new TapGestureRecognizer();
        backdrop.GestureRecognizers.Add(backdropTap);

        var pickerGrid = new Grid
        {
            ColumnSpacing = 4,
            RowSpacing = 4,
            Padding = new Thickness(8)
        };
        for (var c = 0; c < columns; c++)
            pickerGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        var rowCount = (int)Math.Ceiling(glyphs.Count / (double)columns);
        for (var r = 0; r < rowCount; r++)
            pickerGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        var container = new Border
        {
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
            BackgroundColor = Colors.White,
            Padding = 4,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Opacity = 0,
            Scale = 0.85,
            Content = pickerGrid
        };

        void Close(string? result)
        {
            backdrop.GestureRecognizers.Remove(backdropTap);
            if (rootGrid.Children.Contains(backdrop))
                rootGrid.Children.Remove(backdrop);
            if (rootGrid.Children.Contains(container))
                rootGrid.Children.Remove(container);
            tcs.TrySetResult(result);
        }

        backdropTap.Tapped += (_, _) => Close(null);

        for (var i = 0; i < glyphs.Count; i++)
        {
            var glyph = glyphs[i];
            var label = new Label
            {
                Text = glyph,
                FontSize = 28,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            var cell = new Border
            {
                StrokeThickness = 0,
                BackgroundColor = Colors.Transparent,
                Padding = new Thickness(8, 6),
                Content = label
            };
            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) => Close(glyph);
            cell.GestureRecognizers.Add(tap);

            pickerGrid.Add(cell, i % columns, i / columns);
        }

        rootGrid.Add(backdrop, 0, 0);
        Grid.SetRowSpan(backdrop, 3);
        rootGrid.Add(container, 0, 0);
        Grid.SetRowSpan(container, 3);

        _ = Task.WhenAll(
            backdrop.FadeToAsync(1, 150u),
            container.FadeToAsync(1, 150u),
            container.ScaleToAsync(1, 150u, Easing.SpringOut));

        return tcs.Task;
    }

    partial void HookKeyboard();
    partial void UnhookKeyboard();

    protected override void OnHandlerChanging(HandlerChangingEventArgs args)
    {
        base.OnHandlerChanging(args);
        if (args.OldHandler != null)
            UnhookKeyboard();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (Handler != null)
            HookKeyboard();
    }
}
