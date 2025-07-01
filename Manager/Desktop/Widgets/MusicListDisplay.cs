using System;
using System.Collections.Generic;
using System.Linq;
using Desktop.View;
using Gtk;

namespace Desktop.Widgets;

public class MusicListDisplay : Box
{
    private readonly Dictionary<CheckButton, Column> _buttonToColumnEnum;
    private readonly Dictionary<Column, TreeViewColumn> _columnEnumToTreeColumn;
    private readonly MusicView[] _musics;
    private readonly TreeView _tree;
    private ListStore? _model;

    public MusicListDisplay(IEnumerable<MusicView> musics)
        : base(Orientation.Vertical, 3)
    {
        _musics = musics.ToArray();
        _buttonToColumnEnum = new Dictionary<CheckButton, Column>();
        _columnEnumToTreeColumn = new Dictionary<Column, TreeViewColumn>();

        var columnToggleBox = CreateColumnToggleBox();
        PackStart(columnToggleBox, false, true, 0);

        CreateModel();

        var treeScroll = new ScrolledWindow
        {
            Expand = true
        };
        _tree = new TreeView(_model);
        AddColumns();
        treeScroll.Add(_tree);

        PackStart(treeScroll, true, true, 0);
    }

    private Box CreateColumnToggleBox()
    {
        _buttonToColumnEnum.Clear();
        var columnToggleBox = new Box(Orientation.Horizontal, 0);
        foreach (var column in Enum.GetValues<Column>())
        {
            var columnName = column.ToString();
            var checkButton = new CheckButton(columnName);
            checkButton.Active = true;
            checkButton.Toggled += ColumnCheckButton_Toggled;
            columnToggleBox.PackStart(checkButton, false, true, 0);
            _buttonToColumnEnum[checkButton] = column;
        }

        return columnToggleBox;
    }

    private void CreateModel()
    {
        var columnCount = Enum.GetNames<Column>().Length;
        var columnTypes = Enumerable.Repeat(typeof(string), columnCount);
        _model = new ListStore(columnTypes.ToArray());

        foreach (var music in _musics)
        {
            var iter = _model.Append();
            _model.SetValue(iter, (int)Column.Title, music.Title);
            _model.SetValue(iter, (int)Column.Artist, music.Artist);
            _model.SetValue(iter, (int)Column.Album, music.Album);
            _model.SetValue(iter, (int)Column.ArtistAlbum, music.AlbumArtist);
            _model.SetValue(iter, (int)Column.Name, music.Resource.Name);
            _model.SetValue(iter, (int)Column.Location, music.Resource.Location);
        }
    }

    private void AddColumns()
    {
        _columnEnumToTreeColumn.Clear();
        foreach (var columnEnum in Enum.GetValues<Column>())
        {
            var renderer = new CellRendererText();
            var column = new TreeViewColumn(columnEnum.ToString(), renderer, "text", columnEnum, null);
            column.SortColumnId = (int)columnEnum;
            _tree.AppendColumn(column);
            _columnEnumToTreeColumn[columnEnum] = column;
        }
    }

    private void ColumnCheckButton_Toggled(object? sender, EventArgs e)
    {
        if (sender == null)
            throw new InvalidOperationException();

        var checkButton = (CheckButton)sender;
        var columnEnum = _buttonToColumnEnum[checkButton];
        var treeColumn = _columnEnumToTreeColumn[columnEnum];
        treeColumn.Visible = checkButton.Active;
    }

    private enum Column
    {
        Title,
        Artist,
        Album,
        ArtistAlbum,
        Name,
        Location
    }
}