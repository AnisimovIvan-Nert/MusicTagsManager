namespace MusicTagsManager.Tags;

public interface IEditableTags : ITags
{
    new string? Title { get; set; }
    new string? Artist { get; set; }
    new string? Album { get; set; }
    new string? AlbumArtist { get; set; }
}