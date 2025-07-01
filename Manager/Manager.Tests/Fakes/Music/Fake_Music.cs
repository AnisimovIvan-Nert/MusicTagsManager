using Manager.Music;
using Manager.Resource;

namespace Manager.Tests.Fakes.Music;

public class Fake_Music(
    string? title,
    string? artist,
    string? album,
    string? albumArtist)
    : IMusic
{
    public IResourceIdentifier Identifier => null!;
    public string? Title { get; } = title;
    public string? Artist { get; } = artist;
    public string? Album { get; } = album;
    public string? AlbumArtist { get; } = albumArtist;
}