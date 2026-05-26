using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Shiny.Maui.Controls.Tree;

/// <summary>
/// Load state of a tree node's children for lazy loading scenarios.
/// </summary>
public enum TreeLoadState
{
    NotLoaded,
    Loading,
    Loaded,
    Error
}

/// <summary>
/// Internal wrapper around a user item that tracks visual tree state (expansion,
/// loading, children cache, depth). Exposed publicly so event args can carry it.
/// </summary>
public class TreeNode : INotifyPropertyChanged
{
    bool isExpanded;
    bool isSelected;
    TreeLoadState loadState;
    Exception? loadError;
    ObservableCollection<TreeNode>? children;

    internal TreeNode(object item, TreeNode? parent, int depth)
    {
        Item = item;
        Parent = parent;
        Depth = depth;
    }

    public object Item { get; }
    public TreeNode? Parent { get; }
    public int Depth { get; }

    public bool IsExpanded
    {
        get => isExpanded;
        internal set => SetField(ref isExpanded, value);
    }

    public bool IsSelected
    {
        get => isSelected;
        internal set => SetField(ref isSelected, value);
    }

    public TreeLoadState LoadState
    {
        get => loadState;
        internal set => SetField(ref loadState, value);
    }

    public Exception? LoadError
    {
        get => loadError;
        internal set => SetField(ref loadError, value);
    }

    public ObservableCollection<TreeNode>? Children
    {
        get => children;
        internal set => SetField(ref children, value);
    }

    public bool HasLoadedChildren => children != null && children.Count > 0;

    public event PropertyChangedEventHandler? PropertyChanged;

    void SetField<T>(ref T storage, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
            return;
        storage = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
