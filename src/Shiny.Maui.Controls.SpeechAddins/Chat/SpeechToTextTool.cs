using System.Globalization;
using Shiny.Maui.Controls.Chat;
using Shiny.Speech;

namespace Shiny.Maui.Controls.SpeechAddins.Chat;

/// <summary>
/// A reusable input bar tool that listens for speech and backfills the chat entry.
/// Shows a listening icon while recording. Optionally auto-sends on completion.
/// </summary>
public class SpeechToTextTool : FabMenuItem, IChatEntryTool
{
    ChatView? chatView;
    CancellationTokenSource? cts;
    bool isListening;
    string? originalText;
    ImageSource? originalIcon;

    public SpeechToTextTool()
    {
        Text = "Voice Input";
        FabBackgroundColor = Color.FromArgb("#4CAF50");
        Clicked += OnClicked;
    }

    // ------- Configuration Properties -------

    public static readonly BindableProperty AutoSendProperty = BindableProperty.Create(
        nameof(AutoSend), typeof(bool), typeof(SpeechToTextTool), false);

    /// <summary>
    /// When true, automatically submits the entry text after speech recognition completes.
    /// </summary>
    public bool AutoSend
    {
        get => (bool)GetValue(AutoSendProperty);
        set => SetValue(AutoSendProperty, value);
    }

    public static readonly BindableProperty SilenceTimeoutProperty = BindableProperty.Create(
        nameof(SilenceTimeout), typeof(TimeSpan), typeof(SpeechToTextTool), TimeSpan.FromSeconds(2));

    /// <summary>
    /// Duration of silence before recognition is considered complete.
    /// </summary>
    public TimeSpan SilenceTimeout
    {
        get => (TimeSpan)GetValue(SilenceTimeoutProperty);
        set => SetValue(SilenceTimeoutProperty, value);
    }

    public static readonly BindableProperty CultureProperty = BindableProperty.Create(
        nameof(Culture), typeof(string), typeof(SpeechToTextTool));

    /// <summary>
    /// Culture code (e.g. "en-US") for speech recognition. If null, the system default is used.
    /// </summary>
    public string? Culture
    {
        get => (string?)GetValue(CultureProperty);
        set => SetValue(CultureProperty, value);
    }

    public static readonly BindableProperty PreferOnDeviceProperty = BindableProperty.Create(
        nameof(PreferOnDevice), typeof(bool), typeof(SpeechToTextTool), false);

    /// <summary>
    /// When true, prefers on-device speech recognition over cloud-based.
    /// </summary>
    public bool PreferOnDevice
    {
        get => (bool)GetValue(PreferOnDeviceProperty);
        set => SetValue(PreferOnDeviceProperty, value);
    }

    public static readonly BindableProperty ListeningIconProperty = BindableProperty.Create(
        nameof(ListeningIcon), typeof(ImageSource), typeof(SpeechToTextTool));

    /// <summary>
    /// Optional icon displayed while listening. If not set, the tool text changes to indicate listening state.
    /// </summary>
    public ImageSource? ListeningIcon
    {
        get => (ImageSource?)GetValue(ListeningIconProperty);
        set => SetValue(ListeningIconProperty, value);
    }

    public static readonly BindableProperty ListeningTextProperty = BindableProperty.Create(
        nameof(ListeningText), typeof(string), typeof(SpeechToTextTool), "Listening\u2026");

    /// <summary>
    /// Text displayed on the tool while actively listening.
    /// </summary>
    public string ListeningText
    {
        get => (string)GetValue(ListeningTextProperty);
        set => SetValue(ListeningTextProperty, value);
    }

    public static readonly BindableProperty ListeningFabBackgroundColorProperty = BindableProperty.Create(
        nameof(ListeningFabBackgroundColor), typeof(Color), typeof(SpeechToTextTool),
        Color.FromArgb("#F44336"));

    /// <summary>
    /// Background color of the tool FAB while listening.
    /// </summary>
    public Color ListeningFabBackgroundColor
    {
        get => (Color)GetValue(ListeningFabBackgroundColorProperty);
        set => SetValue(ListeningFabBackgroundColorProperty, value);
    }

    // ------- IChatEntryTool -------

    void IChatEntryTool.Attach(ChatView view) => chatView = view;
    void IChatEntryTool.Detach() => chatView = null;

    // ------- Core Logic -------

    async void OnClicked(object? sender, EventArgs e)
    {
        if (isListening)
        {
            StopListening();
            return;
        }

        if (chatView is null)
            return;

        var stt = ResolveService<ISpeechToTextService>();
        if (stt is null || !stt.IsSupported)
            return;

        var access = await stt.RequestAccess();
        if (access != AccessState.Available)
            return;

        isListening = true;
        cts = new CancellationTokenSource();
        SetListeningAppearance(true);

        try
        {
            var options = new SpeechRecognitionOptions
            {
                SilenceTimeout = SilenceTimeout,
                PreferOnDevice = PreferOnDevice,
                Culture = Culture is not null ? CultureInfo.GetCultureInfo(Culture) : null
            };

            var result = await stt.ListenUntilSilence(options, cts.Token);

            if (!string.IsNullOrEmpty(result))
            {
                // Append to existing text with a space if needed
                var existing = chatView.EntryText?.Trim();
                chatView.EntryText = string.IsNullOrEmpty(existing)
                    ? result
                    : $"{existing} {result}";

                if (AutoSend)
                    chatView.SubmitEntry();
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SpeechToTextTool: {ex.Message}");
        }
        finally
        {
            isListening = false;
            SetListeningAppearance(false);
        }
    }

    void StopListening()
    {
        cts?.Cancel();
        cts = null;
    }

    void SetListeningAppearance(bool listening)
    {
        if (listening)
        {
            originalText = Text;
            originalIcon = Icon;
            Text = ListeningText;
            FabBackgroundColor = ListeningFabBackgroundColor;

            if (ListeningIcon is not null)
                Icon = ListeningIcon;
        }
        else
        {
            Text = originalText;
            FabBackgroundColor = Color.FromArgb("#4CAF50");
            Icon = originalIcon;
        }
    }

    static T? ResolveService<T>() where T : class
        => Application.Current?.Handler?.MauiContext?.Services.GetService<T>();
}
