namespace Shiny.Maui.Controls.Chat;

/// <summary>
/// A bubble tool that toggles a single acknowledgement glyph on a message.
/// Tapping adds the reaction; tapping again removes it.
/// </summary>
public class AcknowledgementBubbleTool : ChatBubbleTool
{
    public static readonly BindableProperty GlyphProperty = BindableProperty.Create(
        nameof(Glyph),
        typeof(string),
        typeof(AcknowledgementBubbleTool),
        "\ud83d\udc4d",
        propertyChanged: (b, _, n) => ((AcknowledgementBubbleTool)b).Text = (string)n);

    /// <summary>
    /// The emoji/glyph to toggle on the message (e.g. "👍", "👎", "❤️").
    /// Also used as the tool's display text.
    /// </summary>
    public string Glyph
    {
        get => (string)GetValue(GlyphProperty);
        set => SetValue(GlyphProperty, value);
    }

    public static readonly BindableProperty UserIdProperty = BindableProperty.Create(
        nameof(UserId),
        typeof(string),
        typeof(AcknowledgementBubbleTool),
        "me");

    /// <summary>
    /// The user ID to stamp on the acknowledgement. Defaults to "me".
    /// </summary>
    public string UserId
    {
        get => (string)GetValue(UserIdProperty);
        set => SetValue(UserIdProperty, value);
    }

    public AcknowledgementBubbleTool()
    {
        Text = Glyph;
        FabBackgroundColor = Color.FromArgb("#E5E7EB");
        Clicked += OnClicked;
    }

    void OnClicked(object? sender, EventArgs e)
    {
        if (Message is null)
            return;

        Message.Acknowledgements ??= [];

        var existing = Message.Acknowledgements
            .FirstOrDefault(a => a.Glyph == Glyph && a.UserId == UserId);

        if (existing is not null)
            Message.Acknowledgements.Remove(existing);
        else
            Message.Acknowledgements.Add(new Acknowledgement
            {
                Glyph = Glyph,
                UserId = UserId,
                Timestamp = DateTime.Now
            });

        var context = new AcknowledgementChangedContext(Message, Glyph);
        if (Command?.CanExecute(context) == true)
            Command.Execute(context);

        RequestRefresh();
    }
}
