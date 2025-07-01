using Manager.Music;

namespace Manager.Implementation.Music;

internal class Artist(string name, IEnumerable<IAlbum> albums) : IArtist
{
    public string Name { get; } = name;
    public IEnumerable<IAlbum> Albums { get; } = albums;
}