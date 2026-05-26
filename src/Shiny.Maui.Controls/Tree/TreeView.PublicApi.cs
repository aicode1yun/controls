namespace Shiny.Maui.Controls.Tree;

public partial class TreeView
{
    /// <summary>
    /// Expand every node, materializing children via <see cref="ChildrenSelector"/>.
    /// Nodes that require <see cref="ChildrenLoader"/> are skipped (call
    /// <see cref="ExpandAllAsync"/> for the async equivalent).
    /// </summary>
    public void ExpandAll()
    {
        foreach (var n in rootNodes)
            ExpandRecursiveSync(n);
        Rebuild();
    }

    void ExpandRecursiveSync(TreeNode node)
    {
        if (!HasChildren(node.Item) || !CanExpand(node.Item))
            return;
        if (node.Children == null)
        {
            if (ChildrenLoader != null)
                return; // skip lazy-loaded subtrees in sync expand
            var kids = ChildrenSelector?.Invoke(node.Item);
            node.Children = new System.Collections.ObjectModel.ObservableCollection<TreeNode>();
            if (kids != null)
            {
                foreach (var item in kids)
                {
                    if (item != null)
                        node.Children.Add(new TreeNode(item, node, node.Depth + 1));
                }
            }
            node.LoadState = TreeLoadState.Loaded;
        }
        node.IsExpanded = true;
        foreach (var c in node.Children!)
            ExpandRecursiveSync(c);
    }

    /// <summary>
    /// Expand every node, awaiting <see cref="ChildrenLoader"/> as needed.
    /// </summary>
    public async Task ExpandAllAsync()
    {
        foreach (var n in rootNodes)
            await ExpandRecursiveAsync(n);
        Rebuild();
    }

    async Task ExpandRecursiveAsync(TreeNode node)
    {
        if (!HasChildren(node.Item) || !CanExpand(node.Item))
            return;
        if (node.Children == null)
        {
            var syncKids = ChildrenSelector?.Invoke(node.Item);
            if (syncKids != null)
            {
                node.Children = new System.Collections.ObjectModel.ObservableCollection<TreeNode>();
                foreach (var item in syncKids)
                {
                    if (item != null)
                        node.Children.Add(new TreeNode(item, node, node.Depth + 1));
                }
                node.LoadState = TreeLoadState.Loaded;
            }
            else if (ChildrenLoader != null)
            {
                node.LoadState = TreeLoadState.Loading;
                try
                {
                    var children = await ChildrenLoader(node.Item);
                    node.Children = new System.Collections.ObjectModel.ObservableCollection<TreeNode>();
                    foreach (var item in children)
                    {
                        if (item != null)
                            node.Children.Add(new TreeNode(item, node, node.Depth + 1));
                    }
                    node.LoadState = TreeLoadState.Loaded;
                }
                catch (Exception ex)
                {
                    node.LoadError = ex;
                    node.LoadState = TreeLoadState.Error;
                    RaiseLoadFailed(node, ex);
                    return;
                }
            }
            else
            {
                node.Children = new System.Collections.ObjectModel.ObservableCollection<TreeNode>();
                node.LoadState = TreeLoadState.Loaded;
            }
        }
        node.IsExpanded = true;
        foreach (var c in node.Children!)
            await ExpandRecursiveAsync(c);
    }

    public void CollapseAll()
    {
        foreach (var n in rootNodes)
            CollapseRecursive(n);
        Rebuild();
    }

    void CollapseRecursive(TreeNode node)
    {
        node.IsExpanded = false;
        if (node.Children != null)
            foreach (var c in node.Children)
                CollapseRecursive(c);
    }

    /// <summary>
    /// Expand the node that wraps the given source item.
    /// </summary>
    public void Expand(object item)
    {
        var node = FindNode(item);
        if (node == null || node.IsExpanded)
            return;
        ToggleExpand(node);
    }

    public void Collapse(object item)
    {
        var node = FindNode(item);
        if (node == null || !node.IsExpanded)
            return;
        ToggleExpand(node);
    }

    /// <summary>
    /// Drop the cached children for the given item so the next expand re-runs the loader/selector.
    /// </summary>
    public void Refresh(object item)
    {
        var node = FindNode(item);
        if (node == null)
            return;
        var wasExpanded = node.IsExpanded;
        node.IsExpanded = false;
        node.Children = null;
        node.LoadState = TreeLoadState.NotLoaded;
        node.LoadError = null;
        Rebuild();
        if (wasExpanded)
            ToggleExpand(node);
    }

    /// <summary>
    /// Re-run the root loader (or re-bind the ItemsSource). Drops all children caches.
    /// </summary>
    public async Task ReloadAsync()
    {
        if (RootLoader != null)
        {
            rootLoaderInvoked = false;
            await EnsureRootLoadedAsync();
        }
        else
        {
            BuildRootNodes();
            Rebuild();
        }
    }

    public TreeNode? FindNode(object item)
    {
        foreach (var n in rootNodes)
        {
            var found = FindRecursive(n, item);
            if (found != null)
                return found;
        }
        return null;
    }

    static TreeNode? FindRecursive(TreeNode node, object item)
    {
        if (node.Item == item || Equals(node.Item, item))
            return node;
        if (node.Children == null)
            return null;
        foreach (var c in node.Children)
        {
            var found = FindRecursive(c, item);
            if (found != null)
                return found;
        }
        return null;
    }
}
