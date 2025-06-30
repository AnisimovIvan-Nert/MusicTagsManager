namespace Manager.Resource.Implementation;

public class ResourceFactory
{
    public IResource Create(string identifier, string name)
    {
        return new Resource(identifier, name);
    }
}