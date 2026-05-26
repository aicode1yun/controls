namespace Shiny.Maui.Controls.Tree;

public partial class TreeView
{
    public event EventHandler<TreeItemEventArgs>? ItemSelected;
    public event EventHandler<TreeItemEventArgs>? ItemExpanded;
    public event EventHandler<TreeItemEventArgs>? ItemCollapsed;
    public event EventHandler<TreeLoadFailedEventArgs>? LoadFailed;
    public event EventHandler<TreeItemDroppedEventArgs>? ItemDropped;

    void RaiseSelected(TreeNode node)
    {
        var args = new TreeItemEventArgs(node);
        ItemSelected?.Invoke(this, args);
        if (ItemSelectedCommand?.CanExecute(args) == true)
            ItemSelectedCommand.Execute(args);
    }

    void RaiseExpanded(TreeNode node)
    {
        var args = new TreeItemEventArgs(node);
        ItemExpanded?.Invoke(this, args);
        if (ItemExpandedCommand?.CanExecute(args) == true)
            ItemExpandedCommand.Execute(args);
    }

    void RaiseCollapsed(TreeNode node)
    {
        var args = new TreeItemEventArgs(node);
        ItemCollapsed?.Invoke(this, args);
        if (ItemCollapsedCommand?.CanExecute(args) == true)
            ItemCollapsedCommand.Execute(args);
    }

    void RaiseLoadFailed(TreeNode node, Exception exception)
    {
        var args = new TreeLoadFailedEventArgs(node, exception);
        LoadFailed?.Invoke(this, args);
        if (LoadFailedCommand?.CanExecute(args) == true)
            LoadFailedCommand.Execute(args);
    }

    void RaiseDropped(TreeItemDroppedEventArgs args)
    {
        ItemDropped?.Invoke(this, args);
        if (ItemDroppedCommand?.CanExecute(args) == true)
            ItemDroppedCommand.Execute(args);
    }
}
