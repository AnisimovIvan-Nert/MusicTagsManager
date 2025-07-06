namespace MusicTagsManager.Resource;

public interface IResource : IResourceIdentifier
{
    string Name { get; }
    string Location { get; }
}