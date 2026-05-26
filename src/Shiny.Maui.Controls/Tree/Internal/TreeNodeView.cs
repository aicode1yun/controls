namespace Shiny.Maui.Controls.Tree.Internal;

internal class TreeNodeView : Grid
{
    readonly TreeView owner;
    readonly Grid chevronHost;
    readonly ContentView contentHost;
    readonly Border background;
    readonly Grid dropIndicatorAbove;
    readonly Grid dropIndicatorBelow;
    readonly Grid dropIndicatorInto;

    public TreeNodeView(TreeView owner, TreeNode node)
    {
        this.owner = owner;
        Node = node;

        BindingContext = node.Item;
        Padding = 0;
        ColumnSpacing = 0;
        RowSpacing = 0;

        // Background sits behind content and handles selection highlight + tap.
        background = new Border
        {
            StrokeThickness = 0,
            BackgroundColor = owner.RowBackgroundColor,
            Padding = owner.RowPadding,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        // Indent + chevron + content layout
        var inner = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto), // indent + guide lines
                new ColumnDefinition(GridLength.Auto), // chevron
                new ColumnDefinition(GridLength.Star)  // user content
            },
            ColumnSpacing = 4,
            VerticalOptions = LayoutOptions.Center
        };

        // Indent column
        var indent = BuildIndent();
        inner.Add(indent, 0, 0);

        // Chevron
        chevronHost = new Grid
        {
            WidthRequest = owner.ChevronSize + 6,
            HeightRequest = owner.ChevronSize + 6,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            BackgroundColor = Colors.Transparent
        };
        var chevronTap = new TapGestureRecognizer();
        chevronTap.Tapped += OnChevronTapped;
        chevronHost.GestureRecognizers.Add(chevronTap);
        inner.Add(chevronHost, 1, 0);

        // Content host
        contentHost = new ContentView
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Fill
        };

        var template = owner.ItemTemplate;
        if (template is DataTemplateSelector selector)
            template = selector.SelectTemplate(node.Item, owner);

        if (template != null && template.CreateContent() is View userView)
        {
            userView.BindingContext = node.Item;
            contentHost.Content = userView;
        }
        else
        {
            contentHost.Content = new Label
            {
                Text = node.Item?.ToString() ?? string.Empty,
                VerticalTextAlignment = TextAlignment.Center
            };
        }
        inner.Add(contentHost, 2, 0);

        background.Content = inner;

        // Row tap = selection
        var rowTap = new TapGestureRecognizer();
        rowTap.Tapped += OnRowTapped;
        background.GestureRecognizers.Add(rowTap);

        Add(background);

        // Drop indicators (always added; opacity toggled during drag)
        dropIndicatorAbove = MakeDropIndicator(LayoutOptions.Start);
        dropIndicatorBelow = MakeDropIndicator(LayoutOptions.End);
        dropIndicatorInto = new Grid
        {
            BackgroundColor = Color.FromArgb("#330078D7"),
            IsVisible = false
        };
        Add(dropIndicatorInto);
        Add(dropIndicatorAbove);
        Add(dropIndicatorBelow);

        if (owner.EnableDragDrop)
            WireDragDrop();

        RefreshChevron();
        RefreshSelection();
        HookNodeChanges();
    }

    public TreeNode Node { get; }

    Grid BuildIndent()
    {
        var width = owner.IndentSize * Node.Depth;
        var host = new Grid
        {
            WidthRequest = width,
            HorizontalOptions = LayoutOptions.Start
        };

        if (!owner.ShowGuideLines || Node.Depth == 0)
            return host;

        for (var d = 0; d < Node.Depth; d++)
        {
            host.Add(new BoxView
            {
                WidthRequest = 1,
                Color = owner.GuideLineColor,
                HorizontalOptions = LayoutOptions.Start,
                Margin = new Thickness((d * owner.IndentSize) + (owner.IndentSize / 2), 0, 0, 0)
            });
        }
        return host;
    }

    static Grid MakeDropIndicator(LayoutOptions vertical) =>
        new Grid
        {
            HeightRequest = 2,
            BackgroundColor = Color.FromArgb("#0078D7"),
            VerticalOptions = vertical,
            HorizontalOptions = LayoutOptions.Fill,
            IsVisible = false
        };

    public void RefreshChevron()
    {
        chevronHost.Children.Clear();

        var hasChildren = owner.HasChildren(Node.Item);
        var canExpand = owner.CanExpand(Node.Item);

        if (!hasChildren)
            return;

        if (Node.LoadState == TreeLoadState.Loading)
        {
            chevronHost.Children.Add(new ActivityIndicator
            {
                IsRunning = true,
                Color = owner.ChevronColor,
                WidthRequest = owner.ChevronSize,
                HeightRequest = owner.ChevronSize,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            });
            return;
        }

        if (Node.LoadState == TreeLoadState.Error)
        {
            chevronHost.Children.Add(BuildChevronImage(owner.RetryIcon, "↻"));
            return;
        }

        var icon = Node.IsExpanded ? owner.ExpandedIcon : owner.CollapsedIcon;
        var glyph = Node.IsExpanded ? "▼" : "▶";
        var view = BuildChevronImage(icon, glyph);
        view.Opacity = canExpand ? 1.0 : 0.35;
        chevronHost.Children.Add(view);
    }

    View BuildChevronImage(ImageSource? icon, string fallbackGlyph)
    {
        if (icon != null)
        {
            return new Image
            {
                Source = icon,
                WidthRequest = owner.ChevronSize,
                HeightRequest = owner.ChevronSize,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Aspect = Aspect.AspectFit
            };
        }

        return new Label
        {
            Text = fallbackGlyph,
            FontSize = owner.ChevronSize * 0.75,
            TextColor = owner.ChevronColor,
            VerticalTextAlignment = TextAlignment.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };
    }

    public void RefreshSelection()
    {
        background.BackgroundColor = Node.IsSelected
            ? owner.SelectedBackgroundColor
            : owner.RowBackgroundColor;
    }

    void OnChevronTapped(object? sender, EventArgs e)
    {
        if (Node.LoadState == TreeLoadState.Error)
        {
            owner.RetryLoad(Node);
            return;
        }
        if (!owner.HasChildren(Node.Item) || !owner.CanExpand(Node.Item))
            return;
        owner.ToggleExpand(Node);
    }

    void OnRowTapped(object? sender, EventArgs e)
    {
        owner.HandleRowTapped(Node);
    }

    void HookNodeChanges()
    {
        Node.PropertyChanged += (_, args) =>
        {
            switch (args.PropertyName)
            {
                case nameof(TreeNode.IsExpanded):
                case nameof(TreeNode.LoadState):
                    RefreshChevron();
                    break;
                case nameof(TreeNode.IsSelected):
                    RefreshSelection();
                    break;
            }
        };
    }

    // ------------- Drag/drop -------------
    void WireDragDrop()
    {
        var drag = new DragGestureRecognizer { CanDrag = true };
        drag.DragStarting += (s, e) =>
        {
            e.Data.Properties["TreeNodeView"] = this;
        };
        background.GestureRecognizers.Add(drag);

        var drop = new DropGestureRecognizer { AllowDrop = true };
        drop.DragOver += OnDragOver;
        drop.DragLeave += OnDragLeave;
        drop.Drop += OnDrop;
        background.GestureRecognizers.Add(drop);
    }

    TreeDropPosition currentZone = TreeDropPosition.Above;

    void OnDragOver(object? sender, DragEventArgs e)
    {
        // We can't read pointer position reliably from DragEventArgs across all platforms,
        // so we expose three equal vertical thirds via separate sub-targets is overkill —
        // instead we cycle: tap-over toggles by relative time. Default to "below" for the
        // simplest case (reorder as next sibling). The user can re-arrange by dropping
        // again. Visual indicator below shows the chosen zone.
        // For now, show "below" indicator; cross-parent "into" handled by HasChildren.
        currentZone = TreeDropPosition.Below;
        dropIndicatorAbove.IsVisible = false;
        dropIndicatorBelow.IsVisible = true;
        dropIndicatorInto.IsVisible = false;
        e.AcceptedOperation = DataPackageOperation.Copy;
    }

    void OnDragLeave(object? sender, DragEventArgs e)
    {
        dropIndicatorAbove.IsVisible = false;
        dropIndicatorBelow.IsVisible = false;
        dropIndicatorInto.IsVisible = false;
    }

    void OnDrop(object? sender, DropEventArgs e)
    {
        dropIndicatorAbove.IsVisible = false;
        dropIndicatorBelow.IsVisible = false;
        dropIndicatorInto.IsVisible = false;

        if (e.Data?.Properties != null &&
            e.Data.Properties.TryGetValue("TreeNodeView", out var src) &&
            src is TreeNodeView srcView &&
            !ReferenceEquals(srcView, this))
        {
            owner.HandleDrop(srcView.Node, Node, currentZone);
        }
    }
}
