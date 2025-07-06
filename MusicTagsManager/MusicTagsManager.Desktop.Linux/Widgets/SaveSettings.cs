using System;
using Gtk;
using MusicTagsManager.Desktop.Linux.CallTraceLogger;
using MusicTagsManager.Desktop.Linux.Widgets.Extensions;
using MusicTagsManager.Desktop.Save;
using MusicTagsManager.Desktop.Views;

namespace MusicTagsManager.Desktop.Linux.Widgets;

[ConsoleCallTraceLogger]
public class SaveSettings : Box
{
    private CheckButton? _artistButton;
    private CheckButton? _albumButton;
    private Entry? _fileNameFormatEntry;

    private CheckButton ArtistButton => _artistButton
                                        ?? throw new InvalidOperationException();

    private CheckButton AlbumButton => _albumButton
                                       ?? throw new InvalidOperationException();

    private Entry FileNameFormatEntry => _fileNameFormatEntry
                                         ?? throw new InvalidOperationException();

    public SaveSettings() : base(Orientation.Vertical, 3)
    {
        PackHierarchyBox();
        PackFileFormatEntry();
    }

    public MusicSaveSettings GetSettings()
    {
        var artistChecked = ArtistButton.Active;
        var albumChecked = AlbumButton is { Sensitive: true, Active: true };
        var formatText = FileNameFormatEntry.Text;

        var hierarchy = artistChecked
            ? albumChecked
                ? Hierarchy.ArtistAlbum
                : Hierarchy.Artist
            : albumChecked
                ? Hierarchy.Album
                : Hierarchy.None;

        return new MusicSaveSettings(hierarchy, FileNameParser);

        string FileNameParser(MusicView music)
        {
            var name = formatText;
            name = name.Replace($"[{nameof(MusicVariable.Title)}]", $"{music.Title}");
            name = name.Replace($"[{nameof(MusicVariable.Artist)}]", $"{music.Artist}");
            name = name.Replace($"[{nameof(MusicVariable.Album)}]", $"{music.Album}");
            name = name.Replace($"[{nameof(MusicVariable.AlbumArtist)}]", $"{music.AlbumArtist}");
            return name;
        }
    }

    private void ArtistCheckButton_Toggled(object? _, EventArgs __)
    {
        AlbumButton.Sensitive = ArtistButton.Active;
    }

    private void PackHierarchyBox()
    {
        var hierarchyLabel = new Label("File hierarchy: ");
        this.PackStart(hierarchyLabel);


        var hierarchyBox = new Box(Orientation.Horizontal, 0);

        _artistButton = new CheckButton("Artist");
        _artistButton.Toggled += ArtistCheckButton_Toggled;
        hierarchyBox.PackStart(_artistButton);

        _albumButton = new CheckButton("Artist");
        _albumButton.Sensitive = false;
        hierarchyBox.PackStart(_albumButton);

        this.PackStart(hierarchyBox);
    }

    private void PackFileFormatEntry()
    {
        var fileNameFormatLabel = new Label("File names format:");
        this.PackStart(fileNameFormatLabel);


        _fileNameFormatEntry = new Entry("");
        _fileNameFormatEntry.IsEditable = true;
        _fileNameFormatEntry.PlaceholderText = $"Example: [{MusicVariable.Artist}] -- [{MusicVariable.Title}]";

        var tooltipText = "Available variables: ";
        foreach (var musicVariable in Enum.GetNames<MusicVariable>())
        {
            tooltipText += $"[{musicVariable}] ";
        }

        _fileNameFormatEntry.TooltipText = tooltipText;

        this.PackStart(_fileNameFormatEntry);
    }

    private enum MusicVariable
    {
        Title,
        Artist,
        Album,
        AlbumArtist
    }
}