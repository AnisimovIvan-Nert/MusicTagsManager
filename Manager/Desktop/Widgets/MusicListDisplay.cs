using System;
using System.Collections.Generic;
using System.Linq;
using Desktop.Widgets.Extensions;
using Gtk;
using Manager.Desktop.Views;

namespace Desktop.Widgets;

public class MusicListDisplay : Box
{
    private readonly Dictionary<Column, ListColumn> _columns;
    private readonly MusicView[] _musics;
    private ListStore? _model;
    private TreeView? _tree;

    public MusicListDisplay(IEnumerable<MusicView> musics)
        : base(Orientation.Vertical, 3)
    {
        _musics = musics.ToArray();
        _columns = new Dictionary<Column, ListColumn>();

        PackColumnToggleBox();
        PackTreeView();
    }

    private void PackColumnToggleBox()
    {
        _columns.Clear();

        var columnToggleBox = new Box(Orientation.Horizontal, 0);
        foreach (var column in Enum.GetValues<Column>())
        {
            var columnName = column.ToString();

            var listColumn = new ListColumn();
            _columns[column] = listColumn;

            var checkButton = new CheckButton(columnName);
            checkButton.Active = true;
            checkButton.Toggled += listColumn.ColumnCheckButton_Toggled;

            columnToggleBox.PackStart(checkButton);
        }

        this.PackStart(columnToggleBox);
    }

    private void PackTreeView()
    {
        GenerateModel();

        var treeScroll = new ScrolledWindow
        {
            Expand = true
        };
        _tree = new TreeView(_model);

        AppendColumns();

        treeScroll.Add(_tree);
        _tree.Selection.Mode = SelectionMode.Multiple;

        this.PackStart(treeScroll);
    }

    private void GenerateModel()
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

    private void AppendColumns()
    {
        if (_tree == null)
            throw new InvalidOperationException();

        foreach (var columnEnum in Enum.GetValues<Column>())
        {
            var title = columnEnum.ToString();
            var renderer = new CellRendererText();
            var column = new TreeViewColumn(title, renderer, "text", columnEnum, null);
            column.SortColumnId = (int)columnEnum;
            _tree.AppendColumn(column);
            _columns[columnEnum].AssignTreViewColum(column);
        }
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

    private class ListColumn
    {
        private TreeViewColumn? _treeViewColumn;

        public void AssignTreViewColum(TreeViewColumn treeViewColumn)
        {
            _treeViewColumn = treeViewColumn;
        }

        public void ColumnCheckButton_Toggled(object? sender, EventArgs _)
        {
            if (_treeViewColumn == null)
                return;

            if (sender == null)
                throw new InvalidOperationException();

            var checkButton = (CheckButton)sender;
            _treeViewColumn.Visible = checkButton.Active;
        }
    }
}