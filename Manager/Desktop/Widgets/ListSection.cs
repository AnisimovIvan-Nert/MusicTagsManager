using Gtk;

namespace Desktop.Widgets;

public abstract class ListSection : Box
{
    private readonly Grid _grid;
    private int _position;

    protected ListSection()
        : base(Orientation.Vertical, 0)
    {
        _position = 0;
        _grid = new Grid
        {
            RowSpacing = 6,
            ColumnSpacing = 6
        };
        PackStart(_grid, false, true, 0);
    }

    protected void AddItem(string labelText, Widget widget)
    {
        var label = new Label(labelText)
        {
            Hexpand = true,
            Halign = Align.Start
        };
        _grid.Attach(label, 0, _position, 1, 1);

        var horizontalBox = new Box(Orientation.Horizontal, 0);
        var verticalBox = new Box(Orientation.Vertical, 0);
        horizontalBox.PackStart(verticalBox, true, true, 0);
        horizontalBox.PackStart(widget, false, true, 0);

        _grid.Attach(horizontalBox, 1, _position, 1, 1);
        _position++;
    }
}