namespace Shiny.Maui.Controls;

public partial class Overlay
{
    public static readonly BindableProperty IsShownProperty = BindableProperty.Create(
        nameof(IsShown), typeof(bool), typeof(Overlay), false,
        BindingMode.TwoWay,
        propertyChanged: (b, _, n) => ((Overlay)b).OnIsShownChanged((bool)n));
    public bool IsShown { get => (bool)GetValue(IsShownProperty); set => SetValue(IsShownProperty, value); }

    public static readonly BindableProperty OverlayColorProperty = BindableProperty.Create(
        nameof(OverlayColor), typeof(Color), typeof(Overlay), Colors.Black,
        propertyChanged: (b, _, n) => ((Overlay)b).backdrop.Color = (Color)n);
    public Color OverlayColor { get => (Color)GetValue(OverlayColorProperty); set => SetValue(OverlayColorProperty, value); }

    public static readonly BindableProperty OverlayOpacityProperty = BindableProperty.Create(
        nameof(OverlayOpacity), typeof(double), typeof(Overlay), 0.5);
    public double OverlayOpacity { get => (double)GetValue(OverlayOpacityProperty); set => SetValue(OverlayOpacityProperty, value); }

    public static readonly BindableProperty AnimationDurationProperty = BindableProperty.Create(
        nameof(AnimationDuration), typeof(uint), typeof(Overlay), (uint)250);
    public uint AnimationDuration { get => (uint)GetValue(AnimationDurationProperty); set => SetValue(AnimationDurationProperty, value); }

    public static readonly BindableProperty OverlayContentTemplateProperty = BindableProperty.Create(
        nameof(OverlayContentTemplate), typeof(DataTemplate), typeof(Overlay),
        propertyChanged: (b, _, _) => ((Overlay)b).UpdateOverlayContent());
    public DataTemplate? OverlayContentTemplate { get => (DataTemplate?)GetValue(OverlayContentTemplateProperty); set => SetValue(OverlayContentTemplateProperty, value); }
}
