namespace Manager.Resource;

public interface IResourceRepository
{
    IEnumerable<IResource> GetAll();
    IResource Get(IResourceIdentifier identifier);
    IResourceStreamAccess GetStreamAccess(IResourceIdentifier identifier);
    bool Contains(IResourceIdentifier identifier);
}