using Manager.Resource;
using Manager.Tags;
using TagLib;
using File = TagLib.File;

namespace Manager.Implementation.Tags;

public class TagsService(IResourceService resourceService) : ITagsService
{
    public ITags Load(IResourceIdentifier resource)
        => LoadEditable(resource);
    
    public IEditableTags LoadEditable(IResourceIdentifier resource)
    {
        var fileAbstraction = new ResourceFileAbstraction(resourceService, resource);
        using var tagFile = File.Create(fileAbstraction, ReadStyle.PictureLazy);
        return TagsConverter.ConvertFromTagLib(tagFile.Tag);
    }

    public void Update(IResourceIdentifier resource, IEditableTags tags)
    {
        var fileAbstraction = new ResourceFileAbstraction(resourceService, resource);
        using var tagFile = File.Create(fileAbstraction, ReadStyle.PictureLazy);
        TagsConverter.ConvertToTagLib(tags, tagFile.Tag);
        tagFile.Save();
    }
}