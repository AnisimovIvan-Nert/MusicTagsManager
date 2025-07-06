using MusicTagsManager.Resource;

namespace MusicTagsManager.Implementation.Resource;

internal class Resource(string identifier, string name, string location) : IResource
{
    public string Identifier { get; } = identifier;
    public string Name { get; } = name;
    public string Location { get; } = location;
}