namespace Shiny.Maui.Controls.Collections;

public class CollectionItemEventArgs : EventArgs
{
    public CollectionItemEventArgs(object item, int index)
    {
        Item = item;
        Index = index;
    }

    public object Item { get; }
    public int Index { get; }
}
