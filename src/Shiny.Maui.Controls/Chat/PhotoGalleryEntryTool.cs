namespace Shiny.Maui.Controls.Chat;

/// <summary>
/// A ChatView entry tool that opens the device photo gallery via MAUI MediaPicker
/// and fires the ChatView's AttachImageCommand with the selected file path.
/// </summary>
public class PhotoGalleryEntryTool : ChatEntryTool
{
    public PhotoGalleryEntryTool()
    {
        Text = "Gallery";
        FabBackgroundColor = Color.FromArgb("#2196F3");
        Clicked += OnClicked;
    }

    async void OnClicked(object? sender, EventArgs e)
    {
        if (ChatView is null)
            return;

        try
        {
            var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
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
            System.Diagnostics.Debug.WriteLine("PhotoGalleryEntryTool: Permission denied.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"PhotoGalleryEntryTool: {ex.Message}");
        }
    }

    // ------- Configuration -------

    public static readonly BindableProperty PickerTitleProperty = BindableProperty.Create(
        nameof(PickerTitle), typeof(string), typeof(PhotoGalleryEntryTool), "Select a photo");

    /// <summary>
    /// Title displayed on the media picker dialog.
    /// </summary>
    public string PickerTitle
    {
        get => (string)GetValue(PickerTitleProperty);
        set => SetValue(PickerTitleProperty, value);
    }
}
