using Desktop.Widgets.Extensions;
using Gtk;

namespace Desktop.Widgets;

public abstract class ListSection : Box
{
    private const int GridSpacing = 6;
    private const int ElementSpacing = 0;
    private const int ItemHeight = 1;
    private const int LabelWidth = 1;
    private const int WidgetWidth = 1;

    private readonly Grid _grid;
    private int _itemCount;

    protected ListSection()
        : base(Orientation.Vertical, 0)
    {
        _itemCount = 0;

        _grid = new Grid
        {
            RowSpacing = GridSpacing,
            ColumnSpacing = GridSpacing
        };
        this.PackStart(_grid, GridSpacing);
    }

    protected void AddItem(string labelText, Widget widget)
    {
        const int labelLeftPosition = 1;
        const int widgetLeftPosition = 2;

        var label = new Label(labelText)
        {
            Hexpand = true,
            Halign = Align.Start
        };
        var topPosition = _itemCount;
        _grid.Attach(label, labelLeftPosition, topPosition, LabelWidth, ItemHeight);

        var horizontalBox = new Box(Orientation.Horizontal, ElementSpacing);
        var verticalBox = new Box(Orientation.Vertical, ElementSpacing);
        horizontalBox.PackStart(verticalBox);
        horizontalBox.PackStart(widget);

        _grid.Attach(horizontalBox, widgetLeftPosition, topPosition, WidgetWidth, ItemHeight);
        _itemCount++;
    }
}