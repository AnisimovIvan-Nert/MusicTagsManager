namespace MusicTagsManager.Tests;

public abstract class TestsWithTemporaryFolder : IDisposable
{
    protected readonly DirectoryInfo TestDirectory;
    private List<DirectoryInfo>? _extraDirectories;

    protected TestsWithTemporaryFolder()
    {
        var name = GetTypeSpecificDirectoryName();
        TestDirectory = CreateDirectory(name);
    }

    public virtual void Dispose()
    {
        _extraDirectories?.ForEach(directory => directory.Delete(true));
        TestDirectory.Delete(true);
        GC.SuppressFinalize(this);
    }

    protected IEnumerable<DirectoryInfo> CreateExtraDirectories(int count = 1)
    {
        _extraDirectories ??= [];

        var start = _extraDirectories.Count;
        for (var i = start; i < count + start; i++)
        {
            var name = GetTypeSpecificDirectoryName() + "__EXTRA__" + i;
            var directory = CreateDirectory(name);
            _extraDirectories.Add(directory);
            yield return directory;
        }
    }

    protected string CreateFile(string fileName,
        string? folder = null, DirectoryInfo? root = null)
    {
        root ??= TestDirectory;

        var directory = root;
        if (folder != null)
            directory = directory.CreateSubdirectory(folder);

        var file = File.Create(Path.Combine(directory.FullName, fileName));
        var path = file.Name;
        file.Dispose();

        return path;
    }

    protected IEnumerable<FileInfo> GetFiles(DirectoryInfo? root = null)
    {
        root ??= TestDirectory;
        return root.EnumerateFiles("*", SearchOption.AllDirectories);
    }

    private DirectoryInfo CreateDirectory(string name)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        return Directory.CreateDirectory(Path.Join(currentDirectory, name));
    }

    private string GetTypeSpecificDirectoryName()
    {
        return "__TEST__" + GetType().Name;
    }
}