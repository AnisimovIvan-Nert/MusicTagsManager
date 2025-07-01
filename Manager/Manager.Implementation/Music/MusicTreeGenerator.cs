using Manager.Music;

namespace Manager.Implementation.Music;

public class MusicTreeGenerator(IEnumerable<IMusic> musics)
{
    private Dictionary<IAlbum, List<IMusic>> _albumToMusicCollection = new();
    private Dictionary<IArtist, List<IAlbum>> _artistToAlbumCollection = new();
    private Dictionary<string, IArtist> _nameToArtist = new();

    public IEnumerable<IArtist> Generate()
    {
        _nameToArtist = new Dictionary<string, IArtist>();
        _artistToAlbumCollection = new Dictionary<IArtist, List<IAlbum>>();
        _albumToMusicCollection = new Dictionary<IAlbum, List<IMusic>>();

        foreach (var music in musics)
        {
            var artistName = music.Artist;
            var albumTitle = music.Album;
            var albumArtist = music.AlbumArtist;

            if (_nameToArtist.TryGetValue(artistName, out var artist) == false)
            {
                var emptyAlbum = CreateAlbum(albumTitle, albumArtist);
                artist = CreateArtist(artistName, emptyAlbum);
            }

            var albums = _artistToAlbumCollection[artist];
            var album = albums.FirstOrDefault(album => album.Title == albumTitle && album.Artist == albumArtist);

            if (album == null)
            {
                album = CreateAlbum(albumTitle, albumArtist);
                albums.Add(album);
            }

            var albumMusics = _albumToMusicCollection[album];
            albumMusics.Add(music);
        }

        return _nameToArtist.Values;
    }

    private Album CreateAlbum(string title, string artist)
    {
        var musicCollection = new List<IMusic>();
        var album = new Album(title, artist, musicCollection);
        _albumToMusicCollection.Add(album, musicCollection);
        return album;
    }

    private Artist CreateArtist(string name, params IAlbum[] albums)
    {
        var albumCollection = new List<IAlbum>(albums);
        var artist = new Artist(name, albumCollection);
        _artistToAlbumCollection.Add(artist, albumCollection);
        _nameToArtist.Add(name, artist);
        return artist;
    }
}