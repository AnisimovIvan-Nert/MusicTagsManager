namespace Manager.Music;

public interface IArtist
{
    string Name { get; }
    IEnumerable<IAlbum> Albums { get; }
}