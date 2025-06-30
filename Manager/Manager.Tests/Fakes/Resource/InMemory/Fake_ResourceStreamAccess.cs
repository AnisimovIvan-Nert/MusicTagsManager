using Manager.Resource;

namespace Manager.Tests.Fakes.Resource.InMemory;

public class Fake_ResourceStreamAccess : IResourceStreamAccess, IDisposable
{
    private MemoryStream _stream = new();

    public void Dispose()
    {
        _stream.Dispose();
    }

    public Stream OpenRead()
    {
        var isClosed = _stream.CanRead == false;
        if (isClosed)
            RecreateStream();

        return _stream;
    }
    public Stream OpenWrite()
    {
        var isClosed = _stream.CanRead == false;
        if (isClosed)
            RecreateStream();

        return _stream;
    }

    private void RecreateStream()
    {
        var data = _stream.ToArray();
        _stream = new MemoryStream();
        _stream.Write(data);
    }
}