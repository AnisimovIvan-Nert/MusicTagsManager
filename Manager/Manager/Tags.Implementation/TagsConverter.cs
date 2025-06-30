using TagLib;

namespace Manager.Tags.Implementation;

public static class TagsConverter
{
    public static void ConvertToTagLib(IEditableTags source, Tag destination)
    {
        destination.Title = source.Title;
        destination.Album = source.Album;
        destination.Performers = destination.Performers.DeterminateNewValue(source.Artist);
        destination.AlbumArtists = destination.Performers.DeterminateNewValue(source.AlbumArtist);
    }

    public static IEditableTags ConvertFromTagLib(Tag tags)
    {
        var title = tags.Title;
        var artist = tags.FirstPerformer;
        var album = tags.Album;
        var albumArtist = tags.FirstAlbumArtist;
        return new Tags(title, artist, album, albumArtist);
    }
}