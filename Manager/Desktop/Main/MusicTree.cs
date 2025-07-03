using System;
using System.Collections.Generic;
using System.Linq;
using Desktop.Widgets;
using Gtk;
using Manager.Desktop;
using Manager.Desktop.Views;

namespace Desktop.Main;

public class MusicTree
{
    private readonly Dictionary<TreeIter, AlbumView> _albums;
    private readonly Dictionary<TreeIter, ArtistView> _artists;
    private readonly Dictionary<TreeIter, MusicView> _musics;
    private readonly MusicManager _musicManager;
    private TreeIter _selectAll;

    public MusicTree(IEnumerable<ArtistView> artists, MusicManager musicManager)
    {
        _artists = new Dictionary<TreeIter, ArtistView>();
        _albums = new Dictionary<TreeIter, AlbumView>();
        _musics = new Dictionary<TreeIter, MusicView>();
        _musicManager = musicManager;

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

    private void Selection_Changed(object? _, EventArgs __)
    {
        if (TreeView.Selection.GetSelected(out var iter) == false)
            return;

        var musicDisplay = TryGetMusicDisplay(iter, out var displayedMusic);

        if (musicDisplay == null)
            return;

        var musicEditor = new MusicEditor(displayedMusic, musicDisplay, _musicManager);
        musicEditor.MusicUpdated += MusicEditor_MusicUpdated;
        SelectionChanged?.Invoke(musicEditor);
    }

    private Widget? TryGetMusicDisplay(TreeIter iter, out List<MusicView> displayedMusic)
    {
        displayedMusic = [];

        if (_musics.TryGetValue(iter, out var music))
        {
            displayedMusic.Add(music);
            return new MusicDisplay(music);
        }

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

        displayedMusic.AddRange(musics);
        var musicListDisplay = new MusicListDisplay(displayedMusic);
        return musicListDisplay;
    }

    private void MusicEditor_MusicUpdated()
    {
        var artists = _musicManager.LoadArtists();
        var store = CreateStore(artists);
        TreeView.Model = store;
        TreeView.Selection.SelectIter(_selectAll);
    }
}