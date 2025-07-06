using MusicTagsManager.Implementation.Music;
using MusicTagsManager.Tests.Fakes.Music;

namespace MusicTagsManager.Tests.Music;

public class Tests_MusicTreeGenerator
{
    private static readonly string[] Titles = ["", "title"];
    private static readonly int ArtistTitlesCount = Titles.Length;
    private static readonly int AlbumTitlesCount = Titles.Length;
    private static readonly int AlbumArtistTitlesCount = Titles.Length;
    private static readonly int MusicTitlesCount = Titles.Length;

    [Fact]
    public void Generate_ReturnValidMusicTree()
    {
        var fakeMusics =
            from artist in Titles
            from album in Titles
            from albumArtist in Titles
            from title in Titles
            select new Fake_Music(title, artist, album, albumArtist);
        var generator = new MusicTreeGenerator(fakeMusics);


        var artists = generator.Generate().ToArray();


        Assert.Equal(ArtistTitlesCount, artists.Length);
        foreach (var title in Titles)
            Assert.NotNull(artists.SingleOrDefault(artist => artist.Name == title));

        var albums = artists.SelectMany(artist => artist.Albums).ToArray();
        var albumCount = ArtistTitlesCount * AlbumTitlesCount * AlbumArtistTitlesCount;
        Assert.Equal(albumCount, albums.Length);
        foreach (var albumName in Titles)
        foreach (var albumArtist in Titles)
            Assert.Equal(ArtistTitlesCount, albums.Count(album =>
                album.Title == albumName
                && album.Artist == albumArtist));

        var musics = albums.SelectMany(album => album.Musics).ToArray();
        var musicCount = albumCount * MusicTitlesCount;
        Assert.Equal(musicCount, musics.Length);
        foreach (var musicTitle in Titles) Assert.Equal(albumCount, musics.Count(music => music.Title == musicTitle));
    }
}