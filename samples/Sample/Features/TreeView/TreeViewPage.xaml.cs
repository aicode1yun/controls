using Shiny;
using Shiny.Maui.Controls.Tree;

namespace Sample.Features.TreeView;

[ShellMap<TreeViewPage>(registerRoute: false)]
public partial class TreeViewPage : ContentPage
{
    readonly List<FileNode> data;

    public TreeViewPage()
    {
        InitializeComponent();
        data = FileNode.SampleData().ToList();

        Tree.ItemsSource = data;
        Tree.ChildrenSelector = item => (item is FileNode { LazyLoad: false } f) ? f.Children : null;
        Tree.HasChildrenSelector = item => item is FileNode f && f.IsFolder;
        Tree.CanSelectSelector = item => item is FileNode f && !f.IsLocked;
        Tree.CanExpandSelector = item => item is FileNode f && f.IsFolder;
        Tree.ChildrenLoader = LoadCloudChildrenAsync;
    }

    async Task<IEnumerable<object>> LoadCloudChildrenAsync(object parent)
    {
        // Only the "Cloud" branch is lazy. Other folders use the sync selector.
        if (parent is FileNode { LazyLoad: true })
        {
            await Task.Delay(900); // simulate network
            return new object[]
            {
                new FileNode { Name = "remote-backup.zip", Icon = "💾" },
                new FileNode { Name = "shared", Icon = "📁", IsFolder = true, LazyLoad = true },
                new FileNode { Name = "presentation.pptx", Icon = "📊" }
            };
        }
        // Fallback for any unexpected branch
        return Array.Empty<object>();
    }

    void OnExpandAllClicked(object? sender, EventArgs e) => Tree.ExpandAll();

    void OnCollapseAllClicked(object? sender, EventArgs e) => Tree.CollapseAll();

    void OnRefreshCloudClicked(object? sender, EventArgs e)
    {
        var cloud = data.FirstOrDefault(d => d.LazyLoad);
        if (cloud != null)
            Tree.Refresh(cloud);
    }

    void OnMultiSelectToggled(object? sender, ToggledEventArgs e)
    {
        Tree.SelectionMode = e.Value ? TreeSelectionMode.Multiple : TreeSelectionMode.Single;
        StatusLabel.Text = $"Selection mode: {Tree.SelectionMode}";
    }

    void OnItemSelected(object? sender, TreeItemEventArgs e)
    {
        if (e.Item is FileNode f)
            StatusLabel.Text = $"Selected: {f.Name}";
    }

    void OnItemExpanded(object? sender, TreeItemEventArgs e)
    {
        if (e.Item is FileNode f)
            StatusLabel.Text = $"Expanded: {f.Name}";
    }

    void OnItemCollapsed(object? sender, TreeItemEventArgs e)
    {
        if (e.Item is FileNode f)
            StatusLabel.Text = $"Collapsed: {f.Name}";
    }

    void OnLoadFailed(object? sender, TreeLoadFailedEventArgs e)
    {
        StatusLabel.Text = $"Load failed: {e.Exception.Message}";
    }

    void OnItemDropped(object? sender, TreeItemDroppedEventArgs e)
    {
        if (e.SourceItem is not FileNode src || e.TargetItem is not FileNode tgt)
            return;

        // Pop the source from its current parent collection
        var sourceList = FindParentList(src);
        var targetList = FindParentList(tgt);
        if (sourceList == null || targetList == null)
            return;

        sourceList.Remove(src);
        var targetIndex = targetList.IndexOf(tgt);
        targetList.Insert(targetIndex + 1, src);

        // Re-bind so the tree re-flattens with the new order
        Tree.ItemsSource = null;
        Tree.ItemsSource = data;

        StatusLabel.Text = $"Moved {src.Name} after {tgt.Name}";
    }

    List<FileNode>? FindParentList(FileNode item)
    {
        if (data.Contains(item))
            return data;
        return FindParentListIn(data, item);
    }

    static List<FileNode>? FindParentListIn(List<FileNode> nodes, FileNode item)
    {
        foreach (var n in nodes)
        {
            if (n.Children == null)
                continue;
            if (n.Children.Contains(item))
                return n.Children;
            var nested = FindParentListIn(n.Children, item);
            if (nested != null)
                return nested;
        }
        return null;
    }
}
