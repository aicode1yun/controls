namespace Shiny.Maui.Controls.Chat;

/// <summary>
/// Interface for tools that need access to the ChatView for entry manipulation.
/// ChatView automatically calls Attach when the tool is added to ToolItems or BubbleToolItems.
/// </summary>
public interface IChatEntryTool
{
    void Attach(ChatView chatView);
    void Detach();
}
