namespace Shiny.Maui.Controls.Chat;

/// <summary>
/// A bubble tool that displays a popup grid of emoji reactions to choose from.
/// Tapping a glyph toggles the acknowledgement on the message.
/// </summary>
public class AcknowledgementSelectorBubbleTool : ChatBubbleTool
{
    static readonly string[] DefaultGlyphs =
    [
        "\ud83d\udc4d", // 👍
        "\ud83d\udc4e", // 👎
        "\u2764\ufe0f", // ❤️
        "\ud83d\ude02", // 😂
        "\ud83d\ude2e", // 😮
        "\ud83d\ude22", // 😢
        "\ud83d\ude21", // 😡
        "\ud83d\udd25", // 🔥
        "\ud83d\udc4f", // 👏
        "\ud83d\ude4f", // 🙏
        "\ud83d\udcaf", // 💯
        "\ud83c\udf89"  // 🎉
    ];

    public static readonly BindableProperty GlyphsProperty = BindableProperty.Create(
        nameof(Glyphs),
        typeof(string[]),
        typeof(AcknowledgementSelectorBubbleTool));

    /// <summary>
    /// The set of emoji glyphs to display in the selector.
    /// If not set, a default set of common reactions is used.
    /// </summary>
    public string[]? Glyphs
    {
        get => (string[]?)GetValue(GlyphsProperty);
        set => SetValue(GlyphsProperty, value);
    }

    public static readonly BindableProperty UserIdProperty = BindableProperty.Create(
        nameof(UserId),
        typeof(string),
        typeof(AcknowledgementSelectorBubbleTool),
        "me");

    /// <summary>
    /// The user ID to stamp on the acknowledgement. Defaults to "me".
    /// </summary>
    public string UserId
    {
        get => (string)GetValue(UserIdProperty);
        set => SetValue(UserIdProperty, value);
    }

    public AcknowledgementSelectorBubbleTool()
    {
        Text = "\u2764\ufe0f";
        FabBackgroundColor = Color.FromArgb("#F3E5F5");
        Clicked += OnClicked;
    }

    async void OnClicked(object? sender, EventArgs e)
    {
        if (Message is null)
            return;

        var glyphs = Glyphs ?? DefaultGlyphs;
        var selected = await ShowSelectorAsync(glyphs);

        if (selected is null)
            return;

        Message.Acknowledgements ??= [];

        var existing = Message.Acknowledgements
            .FirstOrDefault(a => a.Glyph == selected && a.UserId == UserId);

        if (existing is not null)
            Message.Acknowledgements.Remove(existing);
        else
            Message.Acknowledgements.Add(new Acknowledgement
            {
                Glyph = selected,
                UserId = UserId,
                Timestamp = DateTime.Now
            });

        var context = new AcknowledgementChangedContext(Message, selected);
        if (Command?.CanExecute(context) == true)
            Command.Execute(context);

        RequestRefresh();
    }

    async Task<string?> ShowSelectorAsync(string[] glyphs)
    {
        var page = Application.Current?.Windows.FirstOrDefault()?.Page;
        if (page is null)
            return null;

        var result = await page.DisplayActionSheet(
            "React",
            "Cancel",
            null,
            glyphs);

        if (result is null or "Cancel")
            return null;

        return result;
    }
}
