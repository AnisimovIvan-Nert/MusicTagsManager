using Manager.Resource;

namespace Manager.Music;

public interface IMusic
{
    IResourceIdentifier Identifier { get; }
    
    string? Title { get; }
    string? Artist { get; }
    string? Album { get; }
    string? AlbumArtist { get; }
}