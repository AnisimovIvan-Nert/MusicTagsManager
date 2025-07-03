using Manager.Desktop.Views;
using Manager.Implementation.Music;
using Manager.Implementation.Resource;
using Manager.Implementation.Tags;

namespace Manager.Desktop;

public class MusicManager
{
    private readonly MusicService _musicService;
    private readonly ResourceService _resourceService;
    private readonly TagsService _tagsService;

    public MusicManager(string folderPath)
    {
        var resourceRepository = new ResourceRepository(folderPath, fileFilter: FileFilter);
        _resourceService = new ResourceService(resourceRepository);
        _tagsService = new TagsService(_resourceService);
        _musicService = new MusicService(_resourceService, _tagsService);
    }

    public IEnumerable<ArtistView> LoadArtists()
    {
        var artists = _musicService.LoadArtists();
        return artists.Select(artist => MusicViewFactory.CreateArtist(artist, _resourceService));
    }
    
    public TagsView LoadEditableTags(MusicView music)
    {
        var identifier = music.Resource.Identifier;
        var tags = _tagsService.LoadEditable(identifier);
        return new TagsView(
            tags.Title ?? "", 
            tags.Artist ?? "", 
            tags.Album ?? "", 
            tags.AlbumArtist ?? "");
    }

    public void UpdateTags(MusicView music, TagsView tagsVew)
    {
        var identifier = music.Resource.Identifier;
        var tags = _tagsService.LoadEditable(identifier);
        tags.Title = tagsVew.Title;
        tags.Artist = tagsVew.Artist;
        tags.Album = tagsVew.Album;
        tags.AlbumArtist = tagsVew.AlbumArtist;
        _tagsService.Update(identifier, tags);
    }

    public string GetMusicExtension(MusicView music)
    {
        var identifier = music.Resource.Identifier;
        return _resourceService.GetResourceExtension(identifier);
    }

    public Stream GetReadStream(MusicView music)
    {
        var identifier = music.Resource.Identifier;
        return _resourceService.OpenReadStream(identifier);
    }

    private static bool FileFilter(FileInfo fileInfo)
    {
        var supportedFormats = TagsService.GetSupportedAudioFormats.ToArray();
        return supportedFormats.Contains(fileInfo.Extension);
    }
}