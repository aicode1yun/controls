using Shiny.Blazor.Controls;

namespace Shiny.Blazor.Controls.SpeechAddins;

/// <summary>
/// A TextEntry tool that uses the Web Speech API (via JS interop) to listen for speech
/// and backfill the parent entry text. Toggles listening state with visual feedback.
/// Requires the shinySpeechToText.js module to be loaded.
/// </summary>
public class SpeechToTextTool : TextEntryTool
{
    bool isListening;
    string? originalText;
    string? originalColor;

    public SpeechToTextTool()
    {
        Text = "\uD83C\uDF99"; // microphone emoji
        ToolColor = "#4CAF50";
    }

    /// <summary>Text shown while listening.</summary>
    public string ListeningText { get; set; } = "\u23F9"; // stop icon

    /// <summary>Color while listening.</summary>
    public string ListeningColor { get; set; } = "#F44336";

    /// <summary>BCP 47 language tag (e.g. "en-US"). Null uses browser default.</summary>
    public string? Culture { get; set; }

    /// <summary>Whether speech recognition should continue until explicitly stopped.</summary>
    public bool Continuous { get; set; }

    /// <summary>Whether the tool is currently listening.</summary>
    public bool IsListening => isListening;

    /// <summary>
    /// Called by the TextEntry when this tool is clicked.
    /// The actual JS interop start/stop must be handled by the hosting page
    /// via the OnListeningChanged callback, since tools don't have direct JS access.
    /// </summary>
    public Action<SpeechToTextTool, bool>? OnListeningChanged { get; set; }

    protected override void OnClick()
    {
        isListening = !isListening;

        if (isListening)
        {
            originalText = Text;
            originalColor = ToolColor;
            Text = ListeningText;
            ToolColor = ListeningColor;
        }
        else
        {
            Text = originalText;
            ToolColor = originalColor ?? "#4CAF50";
        }

        OnListeningChanged?.Invoke(this, isListening);
    }

    /// <summary>
    /// Call this from the JS interop callback when speech result is received.
    /// Appends the transcript to the parent entry text.
    /// </summary>
    public void ReceiveTranscript(string transcript)
    {
        if (string.IsNullOrEmpty(transcript)) return;

        var existing = GetEntryText()?.Trim();
        var newText = string.IsNullOrEmpty(existing)
            ? transcript
            : $"{existing} {transcript}";

        SetEntryText(newText);
    }

    /// <summary>
    /// Call this from the JS interop callback when speech recognition ends.
    /// Resets the tool appearance.
    /// </summary>
    public void StopListening()
    {
        if (!isListening) return;
        isListening = false;
        Text = originalText;
        ToolColor = originalColor ?? "#4CAF50";
    }
}
