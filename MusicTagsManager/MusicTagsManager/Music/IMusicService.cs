namespace MusicTagsManager.Music;

public interface IMusicService
{
    public IEnumerable<IMusic> LoadAll();
    public IEnumerable<IArtist> LoadArtists();
    public void Update(IMusic music);
}