using System.Windows.Input;

namespace Shiny.Maui.Controls.Chat;

public partial class ChatView
{
    // Data
    public static readonly BindableProperty MessagesProperty = BindableProperty.Create(
        nameof(Messages),
        typeof(IList<ChatMessage>),
        typeof(ChatView),
        null,
        propertyChanged: (b, _, _) => ((ChatView)b).OnMessagesChanged());

    public IList<ChatMessage>? Messages
    {
        get => (IList<ChatMessage>?)GetValue(MessagesProperty);
        set => SetValue(MessagesProperty, value);
    }

    public static readonly BindableProperty ParticipantsProperty = BindableProperty.Create(
        nameof(Participants),
        typeof(IList<ChatParticipant>),
        typeof(ChatView));

    public IList<ChatParticipant>? Participants
    {
        get => (IList<ChatParticipant>?)GetValue(ParticipantsProperty);
        set => SetValue(ParticipantsProperty, value);
    }

    public static readonly BindableProperty IsMultiPersonProperty = BindableProperty.Create(
        nameof(IsMultiPerson),
        typeof(bool),
        typeof(ChatView),
        false,
        propertyChanged: (b, _, _) => ((ChatView)b).RefreshBubbles());

    public bool IsMultiPerson
    {
        get => (bool)GetValue(IsMultiPersonProperty);
        set => SetValue(IsMultiPersonProperty, value);
    }

    public static readonly BindableProperty ShowAvatarsInSingleChatProperty = BindableProperty.Create(
        nameof(ShowAvatarsInSingleChat),
        typeof(bool),
        typeof(ChatView),
        false,
        propertyChanged: (b, _, _) => ((ChatView)b).RefreshBubbles());

    public bool ShowAvatarsInSingleChat
    {
        get => (bool)GetValue(ShowAvatarsInSingleChatProperty);
        set => SetValue(ShowAvatarsInSingleChatProperty, value);
    }

    // Bubble colors
    public static readonly BindableProperty MyBubbleColorProperty = BindableProperty.Create(
        nameof(MyBubbleColor),
        typeof(Color),
        typeof(ChatView),
        Color.FromArgb("#DCF8C6"));

    public Color MyBubbleColor
    {
        get => (Color)GetValue(MyBubbleColorProperty);
        set => SetValue(MyBubbleColorProperty, value);
    }

    public static readonly BindableProperty MyTextColorProperty = BindableProperty.Create(
        nameof(MyTextColor),
        typeof(Color),
        typeof(ChatView),
        Colors.Black);

    public Color MyTextColor
    {
        get => (Color)GetValue(MyTextColorProperty);
        set => SetValue(MyTextColorProperty, value);
    }

    public static readonly BindableProperty OtherBubbleColorProperty = BindableProperty.Create(
        nameof(OtherBubbleColor),
        typeof(Color),
        typeof(ChatView),
        Colors.White);

    public Color OtherBubbleColor
    {
        get => (Color)GetValue(OtherBubbleColorProperty);
        set => SetValue(OtherBubbleColorProperty, value);
    }

    public static readonly BindableProperty OtherTextColorProperty = BindableProperty.Create(
        nameof(OtherTextColor),
        typeof(Color),
        typeof(ChatView),
        Colors.Black);

    public Color OtherTextColor
    {
        get => (Color)GetValue(OtherTextColorProperty);
        set => SetValue(OtherTextColorProperty, value);
    }

    // Input bar
    public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(
        nameof(PlaceholderText),
        typeof(string),
        typeof(ChatView),
        "Type a message...",
        propertyChanged: (b, _, n) => ((ChatView)b).inputBar.PlaceholderText = (string)n);

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public static readonly BindableProperty SendButtonTextProperty = BindableProperty.Create(
        nameof(SendButtonText),
        typeof(string),
        typeof(ChatView),
        "Send",
        propertyChanged: (b, _, n) => ((ChatView)b).inputBar.SendButtonText = (string)n);

    public string SendButtonText
    {
        get => (string)GetValue(SendButtonTextProperty);
        set => SetValue(SendButtonTextProperty, value);
    }

    public static readonly BindableProperty IsInputBarVisibleProperty = BindableProperty.Create(
        nameof(IsInputBarVisible),
        typeof(bool),
        typeof(ChatView),
        true,
        propertyChanged: (b, _, n) => ((ChatView)b).inputBar.IsVisible = (bool)n);

    public bool IsInputBarVisible
    {
        get => (bool)GetValue(IsInputBarVisibleProperty);
        set => SetValue(IsInputBarVisibleProperty, value);
    }

    // Typing
    public static readonly BindableProperty ShowTypingIndicatorProperty = BindableProperty.Create(
        nameof(ShowTypingIndicator),
        typeof(bool),
        typeof(ChatView),
        true,
        propertyChanged: (b, _, _) => ((ChatView)b).SyncTypingBubbles());

    public bool ShowTypingIndicator
    {
        get => (bool)GetValue(ShowTypingIndicatorProperty);
        set => SetValue(ShowTypingIndicatorProperty, value);
    }

    public static readonly BindableProperty TypingParticipantsProperty = BindableProperty.Create(
        nameof(TypingParticipants),
        typeof(IList<ChatParticipant>),
        typeof(ChatView),
        null,
        propertyChanged: (b, _, _) => ((ChatView)b).OnTypingParticipantsChanged());

    public IList<ChatParticipant>? TypingParticipants
    {
        get => (IList<ChatParticipant>?)GetValue(TypingParticipantsProperty);
        set => SetValue(TypingParticipantsProperty, value);
    }

    // Commands
    public static readonly BindableProperty LoadMoreCommandProperty = BindableProperty.Create(
        nameof(LoadMoreCommand),
        typeof(ICommand),
        typeof(ChatView));

    public ICommand? LoadMoreCommand
    {
        get => (ICommand?)GetValue(LoadMoreCommandProperty);
        set => SetValue(LoadMoreCommandProperty, value);
    }

    public static readonly BindableProperty SendCommandProperty = BindableProperty.Create(
        nameof(SendCommand),
        typeof(ICommand),
        typeof(ChatView));

    public ICommand? SendCommand
    {
        get => (ICommand?)GetValue(SendCommandProperty);
        set => SetValue(SendCommandProperty, value);
    }

    public static readonly BindableProperty AttachImageCommandProperty = BindableProperty.Create(
        nameof(AttachImageCommand),
        typeof(ICommand),
        typeof(ChatView),
        null,
        propertyChanged: (b, _, n) => ((ChatView)b).inputBar.ShowAttachButton = n is not null);

    public ICommand? AttachImageCommand
    {
        get => (ICommand?)GetValue(AttachImageCommandProperty);
        set => SetValue(AttachImageCommandProperty, value);
    }

    public static readonly BindableProperty MessageTappedCommandProperty = BindableProperty.Create(
        nameof(MessageTappedCommand),
        typeof(ICommand),
        typeof(ChatView));

    public ICommand? MessageTappedCommand
    {
        get => (ICommand?)GetValue(MessageTappedCommandProperty);
        set => SetValue(MessageTappedCommandProperty, value);
    }

    // Scroll behavior
    public static readonly BindableProperty ScrollToFirstUnreadProperty = BindableProperty.Create(
        nameof(ScrollToFirstUnread),
        typeof(bool),
        typeof(ChatView),
        false);

    public bool ScrollToFirstUnread
    {
        get => (bool)GetValue(ScrollToFirstUnreadProperty);
        set => SetValue(ScrollToFirstUnreadProperty, value);
    }

    public static readonly BindableProperty FirstUnreadMessageIdProperty = BindableProperty.Create(
        nameof(FirstUnreadMessageId),
        typeof(string),
        typeof(ChatView));

    public string? FirstUnreadMessageId
    {
        get => (string?)GetValue(FirstUnreadMessageIdProperty);
        set => SetValue(FirstUnreadMessageIdProperty, value);
    }

    // Message template
    public static readonly BindableProperty MessageTemplateProperty = BindableProperty.Create(
        nameof(MessageTemplate),
        typeof(DataTemplate),
        typeof(ChatView),
        null,
        propertyChanged: (b, _, _) => ((ChatView)b).RefreshBubbles());

    public DataTemplate? MessageTemplate
    {
        get => (DataTemplate?)GetValue(MessageTemplateProperty);
        set => SetValue(MessageTemplateProperty, value);
    }

    public static readonly BindableProperty MessageTemplateSelectorProperty = BindableProperty.Create(
        nameof(MessageTemplateSelector),
        typeof(DataTemplateSelector),
        typeof(ChatView),
        null,
        propertyChanged: (b, _, _) => ((ChatView)b).RefreshBubbles());

    public DataTemplateSelector? MessageTemplateSelector
    {
        get => (DataTemplateSelector?)GetValue(MessageTemplateSelectorProperty);
        set => SetValue(MessageTemplateSelectorProperty, value);
    }

    // Tools
    public static readonly BindableProperty ToolItemsProperty = BindableProperty.Create(
        nameof(ToolItems),
        typeof(IList<ChatEntryTool>),
        typeof(ChatView),
        null,
        propertyChanged: (b, o, n) => ((ChatView)b).OnToolItemsChanged(o as IList<ChatEntryTool>, n as IList<ChatEntryTool>));

    public IList<ChatEntryTool>? ToolItems
    {
        get => (IList<ChatEntryTool>?)GetValue(ToolItemsProperty);
        set => SetValue(ToolItemsProperty, value);
    }

    public static readonly BindableProperty ToolsIconProperty = BindableProperty.Create(
        nameof(ToolsIcon),
        typeof(ImageSource),
        typeof(ChatView),
        null,
        propertyChanged: (b, _, n) =>
        {
            var cv = (ChatView)b;
            cv.toolsMenu.Icon = n as ImageSource;
            cv.inputBar.ToolsButtonIcon = n as ImageSource;
        });

    public ImageSource? ToolsIcon
    {
        get => (ImageSource?)GetValue(ToolsIconProperty);
        set => SetValue(ToolsIconProperty, value);
    }

    public static readonly BindableProperty ToolsTextProperty = BindableProperty.Create(
        nameof(ToolsText),
        typeof(string),
        typeof(ChatView),
        null,
        propertyChanged: (b, _, n) =>
        {
            var cv = (ChatView)b;
            cv.toolsMenu.Text = n as string;
            cv.inputBar.ToolsButtonText = n as string;
        });

    public string? ToolsText
    {
        get => (string?)GetValue(ToolsTextProperty);
        set => SetValue(ToolsTextProperty, value);
    }

    public static readonly BindableProperty ToolsFabBackgroundColorProperty = BindableProperty.Create(
        nameof(ToolsFabBackgroundColor),
        typeof(Color),
        typeof(ChatView),
        Color.FromArgb("#007AFF"),
        propertyChanged: (b, _, n) =>
        {
            if (n is Color c)
            {
                var cv = (ChatView)b;
                cv.toolsMenu.FabBackgroundColor = c;
                cv.inputBar.ToolsButtonBackgroundColor = c;
            }
        });

    public Color? ToolsFabBackgroundColor
    {
        get => (Color?)GetValue(ToolsFabBackgroundColorProperty);
        set => SetValue(ToolsFabBackgroundColorProperty, value);
    }

    // Bubble Tools
    public static readonly BindableProperty BubbleToolItemsProperty = BindableProperty.Create(
        nameof(BubbleToolItems),
        typeof(IList<ChatBubbleTool>),
        typeof(ChatView),
        null,
        propertyChanged: (b, _, _) => ((ChatView)b).SyncBubbleToolsButtonVisibility());

    /// <summary>
    /// Bubble tools shown on received (other user) messages.
    /// </summary>
    public IList<ChatBubbleTool>? BubbleToolItems
    {
        get => (IList<ChatBubbleTool>?)GetValue(BubbleToolItemsProperty);
        set => SetValue(BubbleToolItemsProperty, value);
    }

    public static readonly BindableProperty MyBubbleToolItemsProperty = BindableProperty.Create(
        nameof(MyBubbleToolItems),
        typeof(IList<ChatBubbleTool>),
        typeof(ChatView),
        null,
        propertyChanged: (b, _, _) => ((ChatView)b).SyncBubbleToolsButtonVisibility());

    /// <summary>
    /// Bubble tools shown on the local user's own messages.
    /// </summary>
    public IList<ChatBubbleTool>? MyBubbleToolItems
    {
        get => (IList<ChatBubbleTool>?)GetValue(MyBubbleToolItemsProperty);
        set => SetValue(MyBubbleToolItemsProperty, value);
    }

    // Chat background
    public static readonly BindableProperty ChatBackgroundColorProperty = BindableProperty.Create(
        nameof(ChatBackgroundColor),
        typeof(Color),
        typeof(ChatView),
        null,
        propertyChanged: (b, _, n) => ((ChatView)b).OnChatBackgroundColorChanged(n as Color));

    public Color? ChatBackgroundColor
    {
        get => (Color?)GetValue(ChatBackgroundColorProperty);
        set => SetValue(ChatBackgroundColorProperty, value);
    }

    // Send button styling
    public static readonly BindableProperty SendButtonBackgroundColorProperty = BindableProperty.Create(
        nameof(SendButtonBackgroundColor),
        typeof(Color),
        typeof(ChatView),
        Color.FromArgb("#007AFF"),
        propertyChanged: (b, _, n) => ((ChatView)b).inputBar.SendButtonBackgroundColor = (Color)n);

    public Color SendButtonBackgroundColor
    {
        get => (Color)GetValue(SendButtonBackgroundColorProperty);
        set => SetValue(SendButtonBackgroundColorProperty, value);
    }

    public static readonly BindableProperty SendButtonTextColorProperty = BindableProperty.Create(
        nameof(SendButtonTextColor),
        typeof(Color),
        typeof(ChatView),
        Colors.White,
        propertyChanged: (b, _, n) => ((ChatView)b).inputBar.SendButtonTextColor = (Color)n);

    public Color SendButtonTextColor
    {
        get => (Color)GetValue(SendButtonTextColorProperty);
        set => SetValue(SendButtonTextColorProperty, value);
    }

    // Input bar styling
    public static readonly BindableProperty InputBarBackgroundColorProperty = BindableProperty.Create(
        nameof(InputBarBackgroundColor),
        typeof(Color),
        typeof(ChatView),
        Color.FromArgb("#F5F5F5"),
        propertyChanged: (b, _, n) => ((ChatView)b).inputBar.BarBackgroundColor = (Color)n);

    public Color InputBarBackgroundColor
    {
        get => (Color)GetValue(InputBarBackgroundColorProperty);
        set => SetValue(InputBarBackgroundColorProperty, value);
    }

    public static readonly BindableProperty InputBarBorderColorProperty = BindableProperty.Create(
        nameof(InputBarBorderColor),
        typeof(Color),
        typeof(ChatView),
        Color.FromArgb("#E0E0E0"),
        propertyChanged: (b, _, n) => ((ChatView)b).inputBar.BarBorderColor = (Color)n);

    public Color InputBarBorderColor
    {
        get => (Color)GetValue(InputBarBorderColorProperty);
        set => SetValue(InputBarBorderColorProperty, value);
    }

    // Font properties
    public static readonly BindableProperty BubbleFontSizeProperty = BindableProperty.Create(
        nameof(BubbleFontSize),
        typeof(double),
        typeof(ChatView),
        15.0,
        propertyChanged: (b, _, _) => ((ChatView)b).RefreshBubbles());

    public double BubbleFontSize
    {
        get => (double)GetValue(BubbleFontSizeProperty);
        set => SetValue(BubbleFontSizeProperty, value);
    }

    public static readonly BindableProperty BubbleFontFamilyProperty = BindableProperty.Create(
        nameof(BubbleFontFamily),
        typeof(string),
        typeof(ChatView),
        null,
        propertyChanged: (b, _, _) => ((ChatView)b).RefreshBubbles());

    public string? BubbleFontFamily
    {
        get => (string?)GetValue(BubbleFontFamilyProperty);
        set => SetValue(BubbleFontFamilyProperty, value);
    }

    public static readonly BindableProperty TimestampFontSizeProperty = BindableProperty.Create(
        nameof(TimestampFontSize),
        typeof(double),
        typeof(ChatView),
        11.0,
        propertyChanged: (b, _, _) => ((ChatView)b).RefreshBubbles());

    public double TimestampFontSize
    {
        get => (double)GetValue(TimestampFontSizeProperty);
        set => SetValue(TimestampFontSizeProperty, value);
    }

    // Bubble corner radius
    public static readonly BindableProperty BubbleCornerRadiusProperty = BindableProperty.Create(
        nameof(BubbleCornerRadius),
        typeof(double),
        typeof(ChatView),
        18.0,
        propertyChanged: (b, _, _) => ((ChatView)b).RefreshBubbles());

    public double BubbleCornerRadius
    {
        get => (double)GetValue(BubbleCornerRadiusProperty);
        set => SetValue(BubbleCornerRadiusProperty, value);
    }

    // Haptic
    public static readonly BindableProperty UseFeedbackProperty = BindableProperty.Create(
        nameof(UseFeedback),
        typeof(bool),
        typeof(ChatView),
        true);

    public bool UseFeedback
    {
        get => (bool)GetValue(UseFeedbackProperty);
        set => SetValue(UseFeedbackProperty, value);
    }
}
