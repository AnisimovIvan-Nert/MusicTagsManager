namespace Manager.Music;

public interface IMusicService
{
    public IEnumerable<IMusic> LoadAll();
    public void Update(IMusic music);
}