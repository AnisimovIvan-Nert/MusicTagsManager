namespace Manager.Resource;

public interface IResourceService
{
    IEnumerable<IResource> GetAll();
    
    Stream OpenReadStream(IResourceIdentifier identifier);
    Stream OpenWriteStream(IResourceIdentifier identifier);
}