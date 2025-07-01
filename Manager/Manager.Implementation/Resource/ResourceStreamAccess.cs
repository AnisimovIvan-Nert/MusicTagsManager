using Manager.Resource;

namespace Manager.Implementation.Resource;

internal class ResourceStreamAccess(string path) : IResourceStreamAccess
{
    public Stream OpenRead()
    {
        return File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public Stream OpenWrite()
    {
        return File.Open(path, FileMode.Open, FileAccess.ReadWrite);
    }
}