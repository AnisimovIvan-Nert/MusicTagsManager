using MusicTagsManager.Resource;

namespace MusicTagsManager.Tags;

public interface ITagsService
{
    ITags Load(IResourceIdentifier resource);
    IEditableTags LoadEditable(IResourceIdentifier resource);
    void Update(IResourceIdentifier resource, IEditableTags tags);
}