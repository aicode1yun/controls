using System.Collections.Specialized;
using System.Windows.Input;
using Shiny.Maui.Controls.Chat.Internal;
using Shiny.Maui.Controls.Infrastructure;

namespace Shiny.Maui.Controls.Chat;

public partial class ChatView
{
    void OnMessagesChanged()
    {
        if (observedCollection is not null)
        {
            observedCollection.CollectionChanged -= OnMessagesCollectionChanged;
            observedCollection = null;
        }

        collectionView.ItemsSource = Messages;

        if (Messages is INotifyCollectionChanged ncc)
        {
            ncc.CollectionChanged += OnMessagesCollectionChanged;
            observedCollection = ncc;
        }

        isNearBottom = true;
        unreadCount = 0;
        UpdateToastPill();
        PerformInitialScroll();
    }

    void OnMessagesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action != NotifyCollectionChangedAction.Add)
            return;

        if (e.NewStartingIndex < 0 || Messages is not { Count: > 0 } || e.NewStartingIndex < Messages.Count - 1)
            return;

        var newMessage = Messages[e.NewStartingIndex];

        if (UseFeedback)
        {
            var eventName = newMessage.IsFromMe ? "MessageSent" : "MessageReceived";
            FeedbackHelper.Execute(this, eventName, newMessage);
        }

        if (newMessage.IsFromMe || isNearBottom)
        {
            unreadCount = 0;
            UpdateToastPill();
            Dispatcher.Dispatch(() => ScrollToEnd(animate: true));
        }
        else
        {
            unreadCount++;
            UpdateToastPill();
        }
    }

    void OnCollectionViewScrolled(object? sender, ItemsViewScrolledEventArgs e)
    {
        var lastIndex = (Messages?.Count ?? 0) - 1;
        if (lastIndex < 0)
        {
            isNearBottom = true;
            return;
        }

        var wasNearBottom = isNearBottom;
        isNearBottom = e.LastVisibleItemIndex >= lastIndex - 1;

        if (isNearBottom && unreadCount > 0)
        {
            unreadCount = 0;
            UpdateToastPill();
        }
        else if (wasNearBottom != isNearBottom)
        {
            // Toggle typing bubble visibility based on scroll position
            typingBubbleHost.IsVisible = isNearBottom && typingBubbleHost.Children.Count > 0;
            UpdateToastPill();
        }
    }

    void UpdateToastPill()
    {
        var hasUnread = unreadCount > 0;
        var hasTyping = !isNearBottom && ShowTypingIndicator && TypingParticipants is { Count: > 0 };

        if (!hasUnread && !hasTyping)
        {
            toastPill.IsVisible = false;
            toastNewMessagesLabel.IsVisible = false;
            toastTypingLabel.IsVisible = false;
            return;
        }

        if (hasUnread)
        {
            toastNewMessagesLabel.Text = unreadCount == 1
                ? "1 New Message"
                : $"{unreadCount} New Messages";
            toastNewMessagesLabel.IsVisible = true;
        }
        else
        {
            toastNewMessagesLabel.IsVisible = false;
        }

        if (hasTyping)
        {
            toastTypingLabel.Text = GetTypingText();
            toastTypingLabel.IsVisible = true;
        }
        else
        {
            toastTypingLabel.IsVisible = false;
        }

        toastPill.IsVisible = true;
    }

    void OnToastPillTapped(object? sender, TappedEventArgs e)
    {
        if (unreadCount <= 0)
            return;

        unreadCount = 0;
        UpdateToastPill();
        ScrollToEnd(animate: true);
    }

    void OnRemainingItemsThresholdReached(object? sender, EventArgs e)
    {
        if (isLoadingMore)
            return;

        if (LoadMoreCommand?.CanExecute(null) != true)
            return;

        isLoadingMore = true;
        Dispatcher.Dispatch(() =>
        {
            try
            {
                LoadMoreCommand?.Execute(null);
            }
            finally
            {
                Dispatcher.Dispatch(() => isLoadingMore = false);
            }
        });
    }

    void OnSendRequested(string text)
    {
        if (UseFeedback)
            FeedbackHelper.Execute(this, "MessageSent");

        if (SendCommand?.CanExecute(text) == true)
            SendCommand.Execute(text);
    }

    void OnAttachRequested()
    {
        if (UseFeedback)
            FeedbackHelper.Execute(this, "AttachImage");

        if (AttachImageCommand?.CanExecute(null) == true)
            AttachImageCommand.Execute(null);
    }

    internal void OnMessageTapped(ChatMessage message)
    {
        if (UseFeedback)
            FeedbackHelper.Execute(this, "MessageTapped", message);

        MessageTapped?.Invoke(this, message);

        if (MessageTappedCommand?.CanExecute(message) == true)
            MessageTappedCommand.Execute(message);
    }

    void PerformInitialScroll()
    {
        if (Messages is not { Count: > 0 })
            return;

        Dispatcher.Dispatch(() =>
        {
            Dispatcher.Dispatch(() =>
            {
                if (Messages is not { Count: > 0 })
                    return;

                if (ScrollToFirstUnread && FirstUnreadMessageId is not null)
                    ScrollToMessage(FirstUnreadMessageId);
                else
                    ScrollToEnd();
            });
        });
    }

    public void ScrollToEnd(bool animate = false)
    {
        if (Messages is { Count: > 0 })
            collectionView.ScrollTo(Messages.Count - 1, position: ScrollToPosition.End, animate: animate);
    }

    public void ScrollToMessage(string messageId, bool animate = true)
    {
        if (Messages is null)
            return;

        for (var i = 0; i < Messages.Count; i++)
        {
            if (Messages[i].Id == messageId)
            {
                collectionView.ScrollTo(i, position: ScrollToPosition.Start, animate: animate);
                return;
            }
        }

        ScrollToEnd(animate);
    }

    void RefreshBubbles()
    {
        collectionView.ItemsSource = null;
        Dispatcher.Dispatch(() => collectionView.ItemsSource = Messages);
    }

    void OnTypingParticipantsChanged()
    {
        if (observedTypingCollection is not null)
        {
            observedTypingCollection.CollectionChanged -= OnTypingCollectionChanged;
            observedTypingCollection = null;
        }

        if (TypingParticipants is INotifyCollectionChanged ncc)
        {
            ncc.CollectionChanged += OnTypingCollectionChanged;
            observedTypingCollection = ncc;
        }

        SyncTypingBubbles();
    }

    void OnTypingCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SyncTypingBubbles();
    }

    string GetTypingText()
    {
        var participants = TypingParticipants!;
        return participants.Count switch
        {
            1 => $"{participants[0].DisplayName} is typing\u2026",
            2 => $"{participants[0].DisplayName}, {participants[1].DisplayName} are typing\u2026",
            3 => $"{participants[0].DisplayName}, {participants[1].DisplayName}, {participants[2].DisplayName} are typing\u2026",
            _ => "Multiple users are typing\u2026"
        };
    }

    void SyncTypingBubbles()
    {
        typingBubbleHost.Children.Clear();

        if (ShowTypingIndicator && TypingParticipants is { Count: > 0 })
        {
            foreach (var participant in TypingParticipants)
            {
                var msg = new ChatMessage
                {
                    SenderId = participant.Id,
                    IsFromMe = false,
                    IsTypingIndicator = true
                };
                var bubble = new ChatTypingBubbleView(this);
                bubble.BindingContext = msg;
                typingBubbleHost.Children.Add(bubble);
            }

            // Only show inline bubbles if user is near the bottom
            typingBubbleHost.IsVisible = isNearBottom;
        }
        else
        {
            typingBubbleHost.IsVisible = false;
        }

        UpdateToastPill();
    }

    // ------- Tools -------

    void OnToolItemsChanged(IList<FabMenuItem>? oldItems, IList<FabMenuItem>? newItems)
    {
        if (observedToolItems is not null)
        {
            observedToolItems.CollectionChanged -= OnToolItemsCollectionChanged;
            observedToolItems = null;
        }

        DetachChatEntryTools(oldItems);
        toolsMenu.Items = newItems ?? new System.Collections.ObjectModel.ObservableCollection<FabMenuItem>();
        AttachChatEntryTools(newItems);

        if (newItems is INotifyCollectionChanged ncc)
        {
            ncc.CollectionChanged += OnToolItemsCollectionChanged;
            observedToolItems = ncc;
        }

        SyncToolsVisibility();
    }

    void OnToolItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => SyncToolsVisibility();

    void SyncToolsVisibility()
    {
        var hasTools = ToolItems is { Count: > 0 };
        toolsMenu.IsVisible = hasTools;
        inputBar.ShowToolsSpacer = hasTools;
    }

    void OnToolItemTapped(object? sender, FabMenuItem item)
    {
        if (UseFeedback)
            FeedbackHelper.Execute(this, "ToolItemTapped", item);
    }

    // ------- Bubble Tools -------

    internal bool HasBubbleTools(ChatMessage message)
        => message.ToolItems is { Count: > 0 } || BubbleToolItems is { Count: > 0 };

    internal void ShowBubbleTools(ChatMessage message)
    {
        // Per-message tools take priority, fall back to ChatView-level defaults
        var tools = message.ToolItems is { Count: > 0 }
            ? message.ToolItems
            : BubbleToolItems;

        if (tools is not { Count: > 0 })
            return;

        activeBubbleToolMessage = message;

        // Set CommandParameter to the ChatMessage so individual item Commands get context
        foreach (var tool in tools)
            tool.CommandParameter = message;

        bubbleToolsMenu.Items = new System.Collections.ObjectModel.ObservableCollection<FabMenuItem>(tools);
        bubbleToolsMenu.IsVisible = true;
        bubbleToolsMenu.Open();
    }

    void SyncBubbleToolsButtonVisibility() => RefreshBubbles();

    void OnBubbleToolItemTapped(object? sender, FabMenuItem item)
    {
        var message = activeBubbleToolMessage;
        activeBubbleToolMessage = null;
        bubbleToolsMenu.IsVisible = false;

        if (message is null)
            return;

        var context = new ChatBubbleToolContext(message, item);

        if (UseFeedback)
            FeedbackHelper.Execute(this, "BubbleToolItemTapped", context);

        BubbleToolItemTapped?.Invoke(this, context);

        if (BubbleToolItemTappedCommand?.CanExecute(context) == true)
            BubbleToolItemTappedCommand.Execute(context);
    }

    // ------- IChatEntryTool -------

    void ScanChatEntryTools(IList<FabMenuItem>? items, bool attach)
    {
        if (items is null) return;
        foreach (var item in items)
        {
            if (item is IChatEntryTool tool)
            {
                if (attach)
                    tool.Attach(this);
                else
                    tool.Detach();
            }
        }
    }

    void DetachChatEntryTools(IList<FabMenuItem>? items) => ScanChatEntryTools(items, false);
    void AttachChatEntryTools(IList<FabMenuItem>? items) => ScanChatEntryTools(items, true);
}
