namespace Shiny.Maui.Controls.Chat;

/// <summary>
/// A ChatView entry tool that opens the device camera via MAUI MediaPicker
/// and fires the ChatView's AttachImageCommand with the captured photo path.
/// </summary>
public class TakePhotoEntryTool : ChatEntryTool
{
    public TakePhotoEntryTool()
    {
        Text = "Camera";
        FabBackgroundColor = Color.FromArgb("#FF9800");
        Clicked += OnClicked;
    }

    async void OnClicked(object? sender, EventArgs e)
    {
        if (ChatView is null)
            return;

        if (!MediaPicker.Default.IsCaptureSupported)
        {
            System.Diagnostics.Debug.WriteLine("TakePhotoEntryTool: Capture not supported on this device.");
            return;
        }

        try
        {
            var result = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
            {
                Title = PickerTitle
            });

            if (result is null)
                return;

            var path = result.FullPath;
            if (ChatView.AttachImageCommand?.CanExecute(path) == true)
                ChatView.AttachImageCommand.Execute(path);
        }
        catch (PermissionException)
        {
            System.Diagnostics.Debug.WriteLine("TakePhotoEntryTool: Permission denied.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"TakePhotoEntryTool: {ex.Message}");
        }
    }

    // ------- Configuration -------

    public static readonly BindableProperty PickerTitleProperty = BindableProperty.Create(
        nameof(PickerTitle), typeof(string), typeof(TakePhotoEntryTool), "Take a photo");

    /// <summary>
    /// Title displayed on the media picker dialog.
    /// </summary>
    public string PickerTitle
    {
        get => (string)GetValue(PickerTitleProperty);
        set => SetValue(PickerTitleProperty, value);
    }
}
