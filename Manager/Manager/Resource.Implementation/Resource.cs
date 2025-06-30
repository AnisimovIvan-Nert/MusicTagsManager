namespace Manager.Resource.Implementation;

internal class Resource(string identifier, string name) : IResource
{
    public string Identifier { get; } = identifier;
    public string Name { get; } = name;
}