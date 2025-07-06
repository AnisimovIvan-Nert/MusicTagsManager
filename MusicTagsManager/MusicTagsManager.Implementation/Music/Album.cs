using MusicTagsManager.Music;

namespace MusicTagsManager.Implementation.Music;

internal class Album(string title, string artist, IEnumerable<IMusic> musics) : IAlbum
{
    public string Title { get; } = title;
    public string Artist { get; } = artist;
    public IEnumerable<IMusic> Musics { get; } = musics;
}