namespace Shiny.Maui.Controls;

/// <summary>
/// Visual style of <see cref="TextEntry"/>. Modeled after the two Bootstrap
/// input styles: <c>.form-control</c> (Classic) and <c>.form-floating</c> (Floating).
/// </summary>
public enum TextEntryVariant
{
    /// <summary>
    /// Static placeholder inside the field. Mirrors Bootstrap's <c>.form-control</c>.
    /// </summary>
    Classic,

    /// <summary>
    /// Placeholder animates up and shrinks to sit above the entered text.
    /// Mirrors Bootstrap's <c>.form-floating</c>.
    /// </summary>
    Floating
}
