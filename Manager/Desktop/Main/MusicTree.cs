using System;
using System.Collections.Generic;
using System.Linq;
using Desktop.View;
using Desktop.Widgets;
using Desktop.Widgets.MusicDisplay;
using Gtk;

namespace Desktop.Main;

public class MusicTree
{
    private readonly Dictionary<TreeIter, AlbumView> _albums;
    private readonly Dictionary<TreeIter, ArtistView> _artists;
    private readonly Dictionary<TreeIter, MusicView> _musics;
    private TreeIter? _selectAll;

    public MusicTree(IEnumerable<ArtistView> artists)
    {
        _artists = new Dictionary<TreeIter, ArtistView>();
        _albums = new Dictionary<TreeIter, AlbumView>();
        _musics = new Dictionary<TreeIter, MusicView>();

        TreeView = new TreeView();
        TreeView.HeadersVisible = false;
        SetUpTreeView(artists);
        TreeView.Selection.Changed += Selection_Changed;
    }

    public TreeView TreeView { get; }

    public event Action<Widget>? SelectionChanged;

    private void SetUpTreeView(IEnumerable<ArtistView> artists)
    {
        var cellName = new CellRendererText();
        var columSections = new TreeViewColumn();
        columSections.Title = "Sections";
        columSections.PackStart(cellName, true);
        columSections.AddAttribute(cellName, "text", 0);
        TreeView.AppendColumn(columSections);

        var store = CreateStore(artists);
        TreeView.Model = store;
    }

    private TreeStore CreateStore(IEnumerable<ArtistView> artists)
    {
        var store = new TreeStore(typeof(string));
        TreeView.Model = store;
        _selectAll = store.AppendValues("[Select all]");

        _artists.Clear();
        _albums.Clear();
        _musics.Clear();
        foreach (var artist in artists)
        {
            var artistTreeIter = store.AppendValues(artist.Name);
            _artists[artistTreeIter] = artist;
            foreach (var album in artist.Albums)
            {
                var albumTreeIter = store.AppendValues(artistTreeIter, album.Title);
                _albums[albumTreeIter] = album;
                foreach (var music in album.Musics)
                {
                    var musicTreeIter = store.AppendValues(albumTreeIter, music.Title);
                    _musics[musicTreeIter] = music;
                }
            }
        }

        return store;
    }

    private void Selection_Changed(object? sender, EventArgs e)
    {
        if (TreeView.Selection.GetSelected(out var iter) == false)
            return;

        var display = TryGetDisplay(iter);
        if (display != null)
            SelectionChanged?.Invoke(display);
    }

    private Widget? TryGetDisplay(TreeIter iter)
    {
        if (_musics.TryGetValue(iter, out var music))
            return new MusicDisplay(music);

        IEnumerable<MusicView> musics;

        if (_albums.TryGetValue(iter, out var album))
            musics = album.Musics;
        else if (_artists.TryGetValue(iter, out var artist))
            musics = artist.Albums
                .SelectMany(musicAlbum => musicAlbum.Musics);
        else if (iter.Equals(_selectAll))
            musics = _artists.Values
                .SelectMany(musicArtist => musicArtist.Albums)
                .SelectMany(musicAlbum => musicAlbum.Musics);
        else
            return null;

        var musicListDisplay = new MusicListDisplay(musics);
        return musicListDisplay;
    }
}