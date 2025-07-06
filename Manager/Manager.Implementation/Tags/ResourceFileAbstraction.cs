using Manager.Resource;
using File = TagLib.File;

namespace Manager.Implementation.Tags;

public class ResourceFileAbstraction(
    IResourceService resourceService,
    IResourceIdentifier resource)
    : File.IFileAbstraction
{
    public string Name => resource.Identifier;
    public Stream ReadStream => resourceService.OpenReadStream(resource);
    public Stream WriteStream => resourceService.OpenWriteStream(resource);

    public void CloseStream(Stream stream)
    {
        stream.Dispose();
    }
}