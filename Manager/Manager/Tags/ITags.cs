namespace Manager.Tags;

public interface ITags
{
    string? Title { get; }
    string? Artist { get; }
    string? Album { get; }
    string? AlbumArtist { get; }
}