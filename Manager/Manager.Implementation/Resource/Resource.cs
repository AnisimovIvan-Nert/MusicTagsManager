using Manager.Resource;

namespace Manager.Implementation.Resource;

internal class Resource(string identifier, string name) : IResource
{
    public string Identifier { get; } = identifier;
    public string Name { get; } = name;
}