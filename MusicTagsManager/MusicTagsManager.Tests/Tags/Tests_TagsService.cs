using MusicTagsManager.Implementation.Resource;
using MusicTagsManager.Implementation.Tags;
using MusicTagsManager.Tests.Fakes.Resource.InMemory;

namespace MusicTagsManager.Tests.Tags;

public class Tests_TagsService
{
    private const string MusicFileName = "file.mp3";

    private readonly Fake_ResourceService _resourceService;

    public Tests_TagsService()
    {
        var factory = new ResourceFactory();
        var resource = factory.Create(MusicFileName, MusicFileName, MusicFileName);
        var repository = new Fake_ResourceRepository(resource);
        _resourceService = new Fake_ResourceService(repository);
    }

    [Fact]
    public void Load_WithInMemoryResource_ReturnEmptyTags()
    {
        var resource = _resourceService.GetAll().First();
        var tagsService = new TagsService(_resourceService);

        var tags = tagsService.Load(resource);

        Assert.Null(tags.Title);
        Assert.Null(tags.Album);
        Assert.Null(tags.AlbumArtist);
        Assert.Null(tags.Artist);
    }

    [Fact]
    public void LoadEditable_WithInMemoryResource_ReturnEmptyEditableTags()
    {
        var resource = _resourceService.GetAll().First();
        var tagsService = new TagsService(_resourceService);

        var tags = tagsService.LoadEditable(resource);

        Assert.Null(tags.Title);
        Assert.Null(tags.Album);
        Assert.Null(tags.AlbumArtist);
        Assert.Null(tags.Artist);
    }

    [Fact]
    public void Update_WithInMemoryResource_UpdateResourceTags()
    {
        var resource = _resourceService.GetAll().First();
        var tagsService = new TagsService(_resourceService);

        var editableTags = tagsService.LoadEditable(resource);
        editableTags.Title = nameof(editableTags.Title);
        editableTags.Album = nameof(editableTags.Album);
        editableTags.AlbumArtist = nameof(editableTags.AlbumArtist);
        editableTags.Artist = nameof(editableTags.Artist);
        tagsService.Update(resource, editableTags);
        var savedTags = tagsService.Load(resource);

        Assert.Equal(editableTags.Title, savedTags.Title);
        Assert.Equal(editableTags.Album, savedTags.Album);
        Assert.Equal(editableTags.AlbumArtist, savedTags.AlbumArtist);
        Assert.Equal(editableTags.Artist, savedTags.Artist);
    }
}