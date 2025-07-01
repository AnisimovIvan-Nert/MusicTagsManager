namespace Manager.Resource;

public interface IResourceService
{
    IEnumerable<IResource> GetAll();
    IResource Get(IResourceIdentifier identifier);
    Stream OpenReadStream(IResourceIdentifier identifier);
    Stream OpenWriteStream(IResourceIdentifier identifier);
}