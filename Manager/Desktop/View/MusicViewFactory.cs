using System.Linq;
using Manager.Music;
using Manager.Resource;

namespace Desktop.View;

public static class MusicViewFactory
{
    public static MusicView CreateMusic(IMusic music, IResourceService resourceService)
    {
        var resource = resourceService.Get(music.Identifier);
        var resourceView = new ResourceView(resource.Name, resource.Location, resource);
        return new MusicView(music.Title, music.Artist, music.Album, music.AlbumArtist, resourceView);
    }

    public static AlbumView CreateAlbum(IAlbum album, IResourceService resourceService)
    {
        var musicViews = album.Musics
            .Select(music => CreateMusic(music, resourceService)).ToArray();

        return new AlbumView(album.Title, album.Artist, musicViews);
    }

    public static ArtistView CreateArtist(IArtist artist, IResourceService resourceService)
    {
        var albumViews = artist.Albums
            .Select(album => CreateAlbum(album, resourceService)).ToArray();

        return new ArtistView(artist.Name, albumViews);
    }
}