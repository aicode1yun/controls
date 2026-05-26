namespace Shiny.Maui.Controls.Tree;

public class TreeItemEventArgs : EventArgs
{
    public TreeItemEventArgs(TreeNode node)
    {
        Node = node;
    }

    public TreeNode Node { get; }
    public object Item => Node.Item;
}

public class TreeLoadFailedEventArgs : TreeItemEventArgs
{
    public TreeLoadFailedEventArgs(TreeNode node, Exception exception) : base(node)
    {
        Exception = exception;
    }

    public Exception Exception { get; }
}

public enum TreeDropPosition
{
    Above,
    Below,
    Into
}

public class TreeItemDroppedEventArgs : EventArgs
{
    public TreeItemDroppedEventArgs(TreeNode source, TreeNode target, TreeDropPosition position)
    {
        Source = source;
        Target = target;
        Position = position;
    }

    public TreeNode Source { get; }
    public TreeNode Target { get; }
    public TreeDropPosition Position { get; }

    public object SourceItem => Source.Item;
    public object TargetItem => Target.Item;
}
