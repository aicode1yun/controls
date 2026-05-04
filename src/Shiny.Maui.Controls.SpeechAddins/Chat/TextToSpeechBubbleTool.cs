using System.Globalization;
using Shiny.Maui.Controls.Chat;
using Shiny.Speech;

namespace Shiny.Maui.Controls.SpeechAddins.Chat;

/// <summary>
/// A reusable bubble tool that reads the chat message text aloud using ITextToSpeechService.
/// </summary>
public class TextToSpeechBubbleTool : FabMenuItem
{
    CancellationTokenSource? cts;

    public TextToSpeechBubbleTool()
    {
        Text = "Read Aloud";
        FabBackgroundColor = Color.FromArgb("#FF5722");
        Clicked += OnClicked;
    }

    public static readonly BindableProperty SpeechRateProperty = BindableProperty.Create(
        nameof(SpeechRate), typeof(float), typeof(TextToSpeechBubbleTool), 1.0f);

    public float SpeechRate
    {
        get => (float)GetValue(SpeechRateProperty);
        set => SetValue(SpeechRateProperty, value);
    }

    public static readonly BindableProperty PitchProperty = BindableProperty.Create(
        nameof(Pitch), typeof(float), typeof(TextToSpeechBubbleTool), 1.0f);

    public float Pitch
    {
        get => (float)GetValue(PitchProperty);
        set => SetValue(PitchProperty, value);
    }

    public static readonly BindableProperty VolumeProperty = BindableProperty.Create(
        nameof(Volume), typeof(float), typeof(TextToSpeechBubbleTool), 1.0f);

    public float Volume
    {
        get => (float)GetValue(VolumeProperty);
        set => SetValue(VolumeProperty, value);
    }

    public static readonly BindableProperty VoiceNameProperty = BindableProperty.Create(
        nameof(VoiceName), typeof(string), typeof(TextToSpeechBubbleTool));

    /// <summary>
    /// The name of the voice to use. If null, the system default is used.
    /// </summary>
    public string? VoiceName
    {
        get => (string?)GetValue(VoiceNameProperty);
        set => SetValue(VoiceNameProperty, value);
    }

    public static readonly BindableProperty CultureProperty = BindableProperty.Create(
        nameof(Culture), typeof(string), typeof(TextToSpeechBubbleTool));

    /// <summary>
    /// Culture code (e.g. "en-US") for voice selection. If null, the system default is used.
    /// </summary>
    public string? Culture
    {
        get => (string?)GetValue(CultureProperty);
        set => SetValue(CultureProperty, value);
    }

    async void OnClicked(object? sender, EventArgs e)
    {
        if (CommandParameter is not ChatMessage message)
            return;

        if (string.IsNullOrEmpty(message.Text))
            return;

        var tts = ResolveService<ITextToSpeechService>();
        if (tts is null || !tts.IsSupported)
            return;

        // Cancel any previous speech
        cts?.Cancel();
        cts = new CancellationTokenSource();

        try
        {
            var options = new TextToSpeechOptions
            {
                SpeechRate = SpeechRate,
                Pitch = Pitch,
                Volume = Volume,
                Culture = Culture is not null ? CultureInfo.GetCultureInfo(Culture) : null,
                Voice = await ResolveVoiceAsync(tts)
            };

            await tts.SpeakAsync(message.Text, options, cts.Token);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"TextToSpeechBubbleTool: {ex.Message}");
        }
    }

    async Task<VoiceInfo?> ResolveVoiceAsync(ITextToSpeechService tts)
    {
        if (string.IsNullOrEmpty(VoiceName))
            return null;

        try
        {
            var culture = Culture is not null ? CultureInfo.GetCultureInfo(Culture) : null;
            var voices = await tts.GetVoicesAsync(culture);
            return voices.FirstOrDefault(v =>
                v.Name.Equals(VoiceName, StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return null;
        }
    }

    static T? ResolveService<T>() where T : class
        => Application.Current?.Handler?.MauiContext?.Services.GetService<T>();
}
