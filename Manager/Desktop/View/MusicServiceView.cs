using System.Collections.Generic;
using System.IO;
using System.Linq;
using Manager.Implementation.Music;
using Manager.Implementation.Resource;
using Manager.Implementation.Tags;
using Manager.Resource;
using Manager.Tags;

namespace Desktop.View;

public class MusicServiceView
{
    private readonly MusicService _musicService;
    private readonly ResourceService _resourceService;
    private readonly TagsService _tagsService;

    public MusicServiceView(string folderPath)
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

    private static bool FileFilter(FileInfo fileInfo)
    {
        var supportedFormats = TagsService.GetSupportedAudioFormats.ToArray();
        return supportedFormats.Contains(fileInfo.Extension);
    }

    public IEditableTags LoadEditableTags(IResourceIdentifier identifier)
    {
        return _tagsService.LoadEditable(identifier);
    }

    public void UpdateTags(IResourceIdentifier identifier, IEditableTags tags)
    {
        _tagsService.Update(identifier, tags);
    }
}