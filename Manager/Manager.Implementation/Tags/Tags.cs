using Manager.Tags;

namespace Manager.Implementation.Tags;

internal class Tags(string? title, string? artist, string? album, string? albumArtist) : IEditableTags
{
    public string? Title { get; set; } = title;
    public string? Artist { get; set; } = artist;
    public string? Album { get; set; } = album;
    public string? AlbumArtist { get; set; } = albumArtist;
}