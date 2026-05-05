namespace Shiny.Blazor.Controls;

static class TextEntryMaskHelper
{
    const char DefaultMaskChar = '#';

    /// <summary>
    /// Applies a mask to raw text (digits only).
    /// Example: ApplyMask("5551234567", "(###) ###-####") → "(555) 123-4567"
    /// </summary>
    public static string ApplyMask(string? rawText, string mask, char maskChar = DefaultMaskChar)
    {
        if (string.IsNullOrEmpty(rawText) || string.IsNullOrEmpty(mask))
            return string.Empty;

        var result = new char[mask.Length];
        var rawIndex = 0;

        for (var i = 0; i < mask.Length; i++)
        {
            if (rawIndex >= rawText.Length)
                break;

            if (mask[i] == maskChar)
            {
                result[i] = rawText[rawIndex++];
            }
            else
            {
                result[i] = mask[i];
            }
        }

        // Find the position after the last raw char was placed
        var lastFilledMaskPos = -1;
        var rawCount = 0;
        for (var i = 0; i < mask.Length; i++)
        {
            if (mask[i] == maskChar)
            {
                rawCount++;
                if (rawCount <= rawText.Length)
                    lastFilledMaskPos = i;
            }
        }

        // Include trailing literals immediately after last filled position
        var length = 0;
        if (lastFilledMaskPos >= 0)
        {
            length = lastFilledMaskPos + 1;
            for (var i = lastFilledMaskPos + 1; i < mask.Length; i++)
            {
                if (mask[i] == maskChar)
                    break;
                result[i] = mask[i];
                length = i + 1;
            }
        }

        return new string(result, 0, length);
    }

    /// <summary>
    /// Strips a formatted string back to raw digits only.
    /// Example: StripMask("(555) 123-4567", "(###) ###-####") → "5551234567"
    /// </summary>
    public static string StripMask(string? formattedText, string mask, char maskChar = DefaultMaskChar)
    {
        if (string.IsNullOrEmpty(formattedText))
            return string.Empty;

        var maxRaw = CalculateRawMaxLength(mask, maskChar);
        var result = new char[maxRaw];
        var count = 0;

        foreach (var ch in formattedText)
        {
            if (count >= maxRaw)
                break;

            if (char.IsDigit(ch))
                result[count++] = ch;
        }

        return new string(result, 0, count);
    }

    /// <summary>
    /// Returns the number of fillable slots in the mask.
    /// </summary>
    public static int CalculateRawMaxLength(string mask, char maskChar = DefaultMaskChar)
    {
        var count = 0;
        foreach (var ch in mask)
        {
            if (ch == maskChar)
                count++;
        }
        return count;
    }

    /// <summary>
    /// Calculates the cursor position in formatted text given a raw text cursor index.
    /// </summary>
    public static int CalculateCursorPosition(int rawCursorIndex, string mask, char maskChar = DefaultMaskChar)
    {
        if (rawCursorIndex <= 0)
            return 0;

        var rawCount = 0;
        for (var i = 0; i < mask.Length; i++)
        {
            if (mask[i] == maskChar)
            {
                rawCount++;
                if (rawCount >= rawCursorIndex)
                    return i + 1;
            }
        }

        return mask.Length;
    }
}
