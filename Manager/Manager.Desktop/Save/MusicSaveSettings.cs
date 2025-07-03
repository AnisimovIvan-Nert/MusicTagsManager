using Manager.Desktop.Views;

namespace Manager.Desktop;

public delegate string FileNameParser(MusicView music);

public enum Hierarchy
{
    None,
    Album,
    Artist,
    ArtistAlbum
}

public class MusicSaveSettings(Hierarchy hierarchy, FileNameParser fileNameParser)
{
    public string GetFilePath(MusicView music)
    {
        var name = fileNameParser(music);
        var path = name;

        switch (hierarchy)
        {
            case Hierarchy.None:
                break;
            case Hierarchy.Album:
                path = Path.Combine(music.Album, path);
                break;
            case Hierarchy.Artist:
                path = Path.Combine(music.Artist, path);
                break;
            case Hierarchy.ArtistAlbum:
                path = Path.Combine(music.Artist, music.Album, path);
                break;
        }

        return path;
    }
}