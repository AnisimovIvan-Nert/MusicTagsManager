namespace Manager.Desktop.Views;

public class TagsView(string title, string artist, string album, string albumArtist)
{
    public string Title { get; set; } = title;
    public string Artist { get; set; } = artist;
    public string Album { get; set; } = album;
    public string AlbumArtist { get; set; } = albumArtist;
}