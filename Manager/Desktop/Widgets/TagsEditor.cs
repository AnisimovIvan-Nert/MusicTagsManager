using System.Collections.Generic;
using System.Linq;
using Desktop.Widgets.Extensions;
using Gtk;
using Manager.Desktop;
using Manager.Desktop.Views;

namespace Desktop.Widgets;

public class TagsEditor : Box
{
    private const string TitleEntryLabel = "Title";
    private const string ArtistEntryLabel = "Artist";
    private const string AlbumEntryLabel = "Album";
    private const string AlbumArtistEntryLabel = "AlbumArtist";

    private readonly MusicView[] _musics;
    private readonly MusicManager _musicManager;
    private readonly TagsEntries _tagsEntries;

    public TagsEditor(IEnumerable<MusicView> musics, MusicManager musicManager)
        : base(Orientation.Vertical, 0)
    {
        _musics = musics.ToArray();
        _musicManager = musicManager;
        
        var tags = _musics.Select(_musicManager.LoadEditableTags).ToArray();

        _tagsEntries = new TagsEntries();

        var titleText = tags.FirstOrDefault()?.Title ?? "";
        var titleSensitive = AllowEditTitle();
        _tagsEntries.Add(TitleEntryLabel, titleSensitive, titleText);

        var artists = tags.Select(tag => tag.Artist);
        var artistOptions = new HashSet<string>(artists);
        _tagsEntries.AddComboBox(ArtistEntryLabel, artistOptions);

        var albums = tags.Select(tag => tag.Album);
        var albumOptions = new HashSet<string>(albums);
        _tagsEntries.AddComboBox(AlbumEntryLabel, albumOptions);

        var albumArtists = tags.Select(tag => tag.AlbumArtist);
        var albumArtistOptions = new HashSet<string>(albumArtists);
        _tagsEntries.AddComboBox(AlbumArtistEntryLabel, albumArtistOptions);

        this.PackStart(_tagsEntries);
    }

    public void Save()
    {
        var title = _tagsEntries.GetValue(TitleEntryLabel);
        var artist = _tagsEntries.GetValue(ArtistEntryLabel);
        var album = _tagsEntries.GetValue(AlbumEntryLabel);
        var albumArtist = _tagsEntries.GetValue(AlbumArtistEntryLabel);

        foreach (var music in _musics)
        {
            var tags = _musicManager.LoadEditableTags(music);

            if (AllowEditTitle())
                tags.Title = title;

            tags.Artist = artist;
            tags.Album = album;
            tags.AlbumArtist = albumArtist;
            _musicManager.UpdateTags(music, tags);
        }
    }

    private bool AllowEditTitle()
    {
        return _musics.Length == 1;
    }

    private class TagsEntries : ListSection
    {
        private readonly Dictionary<string, Entry> _entries = new();

        public string GetValue(string label)
        {
            return _entries[label].Text;
        }

        public void Add(string label, bool sensitive, string defaultValue)
        {
            var entry = new Entry();
            entry.Sensitive = sensitive;
            entry.Text = defaultValue;
            AddItem(label, entry);
            _entries[label] = entry;
        }

        public void AddComboBox(string label, HashSet<string> options)
        {
            var comboBox = ComboBoxText.NewWithEntry();
            foreach (var option in options)
                comboBox.AppendText(option);
            if (options.Count == 1)
                comboBox.Active = 0;
            AddItem(label, comboBox);
            _entries[label] = comboBox.Entry;
        }
    }
}