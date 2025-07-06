namespace MusicTagsManager.Music;

public interface IAlbum
{
    string Title { get; }
    string Artist { get; }
    IEnumerable<IMusic> Musics { get; }
}