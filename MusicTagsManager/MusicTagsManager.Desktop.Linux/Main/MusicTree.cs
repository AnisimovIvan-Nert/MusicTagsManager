using System;
using System.Collections.Generic;
using System.Linq;
using Gtk;
using MusicTagsManager.Desktop.Linux.CallTraceLogger;
using MusicTagsManager.Desktop.Linux.Widgets;
using MusicTagsManager.Desktop.Views;

namespace MusicTagsManager.Desktop.Linux.Main;

[ConsoleCallTraceLogger]
public class MusicTree
{
    private readonly Dictionary<TreeIter, AlbumView> _albums;
    private readonly Dictionary<TreeIter, ArtistView> _artists;
    private readonly Dictionary<TreeIter, MusicView> _musics;
    private readonly MusicManager _musicManager;

    private TreeStore? _treeStore;
    private TreeIter _selectAll;
    private object? _lastSelection;

    public TreeView TreeView { get; }

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
        _treeStore = new TreeStore(typeof(string));
        TreeView.Model = _treeStore;
        _selectAll = _treeStore.AppendValues("[Select all]");

        _artists.Clear();
        _albums.Clear();
        _musics.Clear();
        foreach (var artist in artists)
        {
            var artistTreeIter = _treeStore.AppendValues(artist.Name);
            _artists[artistTreeIter] = artist;
            foreach (var album in artist.Albums)
            {
                var albumTreeIter = _treeStore.AppendValues(artistTreeIter, album.Title);
                _albums[albumTreeIter] = album;
                foreach (var music in album.Musics)
                {
                    var musicTreeIter = _treeStore.AppendValues(albumTreeIter, music.Title);
                    _musics[musicTreeIter] = music;
                }
            }
        }

        return _treeStore;
    }

    private void Selection_Changed(object? _, EventArgs __)
    {
        if (TreeView.Selection.GetSelected(out var iter) == false)
            return;

        var musicDisplay = TryGetMusicDisplay(iter, out var displayedMusic, out var selection);

        if (musicDisplay == null)
            return;

        var musicEditor = new MusicEditor(displayedMusic, musicDisplay, _musicManager, selection);
        musicEditor.MusicUpdated += MusicEditor_MusicUpdated;
        SelectionChanged?.Invoke(musicEditor);
    }

    private Widget? TryGetMusicDisplay(TreeIter iter, out List<MusicView> displayedMusic,
        out IMusicSelection? musicSelection)
    {
        _lastSelection = null;
        displayedMusic = [];
        musicSelection = null;

        if (_musics.TryGetValue(iter, out var music))
        {
            displayedMusic.Add(music);
            _lastSelection = music;
            return new MusicDisplay(music);
        }

        IEnumerable<MusicView> musics;

        if (_albums.TryGetValue(iter, out var album))
        {
            musics = album.Musics;
            _lastSelection = album;
        }
        else if (_artists.TryGetValue(iter, out var artist))
        {
            musics = artist.Albums
                .SelectMany(musicAlbum => musicAlbum.Musics);
            _lastSelection = artist;
        }
        else if (iter.Equals(_selectAll))
        {
            musics = _artists.Values
                .SelectMany(musicArtist => musicArtist.Albums)
                .SelectMany(musicAlbum => musicAlbum.Musics);
        }
        else
            return null;

        displayedMusic.AddRange(musics);
        var musicListDisplay = new MusicListDisplay(displayedMusic);
        musicSelection = musicListDisplay;
        return musicListDisplay;
    }

    private void MusicEditor_MusicUpdated()
    {
        var artists = _musicManager.LoadArtists();
        _treeStore = CreateStore(artists);
        TreeView.Model = _treeStore;

        var selection = TryFindLastSelection();
        TreeView.Selection.SelectIter(selection);
    }

    private TreeIter TryFindLastSelection()
    {
        var defaultSelection = _selectAll;

        switch (_lastSelection)
        {
            case ArtistView artist:
            {
                var newArtist = _artists.Values
                    .FirstOrDefault(value => value.Name == artist.Name);
                return newArtist == null
                    ? defaultSelection
                    : _artists.First(pair => pair.Value == newArtist).Key;
            }
            case AlbumView album:
            {
                var newAlbum = _albums.Values
                    .FirstOrDefault(value => value.Title == album.Title
                                             && value.Artist == album.Artist);
                return newAlbum == null
                    ? defaultSelection
                    : _albums.First(pair => pair.Value == newAlbum).Key;
            }
            case MusicView music:
            {
                var newMusic = _musics.Values
                    .FirstOrDefault(value => value.Title == music.Title
                                             && value.Artist == music.Artist);
                return newMusic == null
                    ? defaultSelection
                    : _musics.First(pair => pair.Value == newMusic).Key;
            }
            default:
                return defaultSelection;
        }
    }
}