namespace Shiny.Maui.Controls;

public partial class Overlay : ContentView
{
    readonly Grid rootGrid;
    readonly BoxView backdrop;
    readonly ContentView overlayContainer;

    public Overlay()
    {
        backdrop = new BoxView
        {
            Color = OverlayColor,
            Opacity = 0,
            IsVisible = false,
            InputTransparent = true
        };

        overlayContainer = new ContentView
        {
            IsVisible = false,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };

        rootGrid = new Grid();
        Content = rootGrid;
    }

    protected override void OnChildAdded(Element child)
    {
        base.OnChildAdded(child);
        RebuildLayout();
    }

    protected override void OnChildRemoved(Element child, int oldLogicalIndex)
    {
        base.OnChildRemoved(child, oldLogicalIndex);
        RebuildLayout();
    }

    void RebuildLayout()
    {
        // Ensure backdrop and overlay container are always the last children (on top)
        if (!rootGrid.Children.Contains(backdrop))
            rootGrid.Children.Add(backdrop);
        if (!rootGrid.Children.Contains(overlayContainer))
            rootGrid.Children.Add(overlayContainer);
    }

    void UpdateOverlayContent()
    {
        if (OverlayContentTemplate == null)
        {
            overlayContainer.Content = null;
            return;
        }

        var content = OverlayContentTemplate.CreateContent() as View;
        overlayContainer.Content = content;
    }

    async void OnIsShownChanged(bool shown)
    {
        if (shown)
        {
            backdrop.InputTransparent = false;
            backdrop.IsVisible = true;
            overlayContainer.IsVisible = true;

            await backdrop.FadeToAsync(OverlayOpacity, AnimationDuration);
        }
        else
        {
            await backdrop.FadeToAsync(0, AnimationDuration);
            backdrop.IsVisible = false;
            backdrop.InputTransparent = true;
            overlayContainer.IsVisible = false;
        }
    }
}
