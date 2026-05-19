using Foundation;
using UIKit;

namespace Shiny.Maui.Controls.Chat;

public partial class ChatView
{
    NSObject? keyboardWillChangeFrameToken;
    NSObject? keyboardWillHideToken;
    Thickness paddingBeforeKeyboard;
    bool keyboardPaddingApplied;

    partial void HookKeyboard()
    {
        keyboardWillChangeFrameToken ??= UIKeyboard.Notifications.ObserveWillChangeFrame(OnKeyboardWillChangeFrame);
        keyboardWillHideToken ??= UIKeyboard.Notifications.ObserveWillHide(OnKeyboardWillHide);
    }

    partial void UnhookKeyboard()
    {
        keyboardWillChangeFrameToken?.Dispose();
        keyboardWillHideToken?.Dispose();
        keyboardWillChangeFrameToken = null;
        keyboardWillHideToken = null;

        if (keyboardPaddingApplied)
        {
            Padding = paddingBeforeKeyboard;
            keyboardPaddingApplied = false;
        }
    }

    void OnKeyboardWillChangeFrame(object? sender, UIKeyboardEventArgs e)
    {
        if (!AdjustForKeyboard || Handler?.PlatformView is not UIView platformView || platformView.Window is null)
            return;

        var viewFrameInWindow = platformView.ConvertRectToView(platformView.Bounds, null);
        var viewBottom = viewFrameInWindow.Y + viewFrameInWindow.Height;
        var overlap = Math.Max(0, viewBottom - e.FrameEnd.Y);

        if (overlap <= 0)
        {
            RestoreOriginalPadding(e.AnimationDuration);
            return;
        }

        if (!keyboardPaddingApplied)
        {
            paddingBeforeKeyboard = Padding;
            keyboardPaddingApplied = true;
        }

        var target = new Thickness(
            paddingBeforeKeyboard.Left,
            paddingBeforeKeyboard.Top,
            paddingBeforeKeyboard.Right,
            paddingBeforeKeyboard.Bottom + overlap);

        UIView.Animate(e.AnimationDuration, () => Padding = target);
    }

    void OnKeyboardWillHide(object? sender, UIKeyboardEventArgs e)
    {
        if (!AdjustForKeyboard)
            return;

        RestoreOriginalPadding(e.AnimationDuration);
    }

    void RestoreOriginalPadding(double duration)
    {
        if (!keyboardPaddingApplied)
            return;

        var target = paddingBeforeKeyboard;
        keyboardPaddingApplied = false;
        UIView.Animate(duration, () => Padding = target);
    }
}
