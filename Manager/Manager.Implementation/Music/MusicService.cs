using Manager.Music;
using Manager.Resource;
using Manager.Tags;

namespace Manager.Implementation.Music;

public class MusicService(
    IResourceService resourceService,
    ITagsService tagsService)
    : IMusicService
{
    public IEnumerable<IMusic> LoadAll()
    {
        var musicResources = resourceService.GetAll();
        foreach (var musicResource in musicResources)
        {
            var tags = tagsService.Load(musicResource);
            yield return new Music(musicResource, tags);
        }
    }

    public IEnumerable<IArtist> LoadArtists()
    {
        var musics = LoadAll();
        var treeGenerator = new MusicTreeGenerator(musics);
        return treeGenerator.Generate();
    }

    public void Update(IMusic music)
    {
        var editableTags = tagsService.LoadEditable(music.Identifier);
        editableTags.Title = music.Title;
        editableTags.Artist = music.Artist;
        editableTags.Album = music.Album;
        editableTags.AlbumArtist = music.AlbumArtist;
        tagsService.Update(music.Identifier, editableTags);
    }
}