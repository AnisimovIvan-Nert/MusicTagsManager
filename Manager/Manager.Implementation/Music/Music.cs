using Manager.Music;
using Manager.Resource;
using Manager.Tags;

namespace Manager.Implementation.Music;

internal class Music(IResourceIdentifier identifier, ITags tags) : IMusic
{
    public IResourceIdentifier Identifier { get; } = identifier;
    public string Title { get; } = tags.Title ?? "";
    public string Artist { get; } = tags.Artist ?? "";
    public string Album { get; } = tags.Album ?? "";
    public string AlbumArtist { get; } = tags.AlbumArtist ?? "";
}