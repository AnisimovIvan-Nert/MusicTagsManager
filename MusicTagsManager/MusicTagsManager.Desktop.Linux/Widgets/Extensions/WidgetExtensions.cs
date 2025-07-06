using System;
using Gtk;

namespace MusicTagsManager.Desktop.Linux.Widgets.Extensions;

public static class WidgetExtensions
{
    public static Window GetParentWindow(this Widget widget)
    {
        var parent = widget.Parent;
        while (parent != null && parent is not Window)
            parent = parent.Parent;

        if (parent == null)
            throw new InvalidOperationException();

        return (Window)parent;
    }
}