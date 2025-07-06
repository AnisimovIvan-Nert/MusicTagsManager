using Gtk;

namespace MusicTagsManager.Desktop.Linux.Widgets.Extensions;

public static class BoxExtensions
{
    public static void PackStart(
        this Box box,
        Widget child,
        uint padding = 0,
        bool expand = false,
        bool fill = true)
    {
        box.PackStart(child, expand, fill, padding);
    }
}