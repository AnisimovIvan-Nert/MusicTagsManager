using MusicTagsManager.Resource;

namespace MusicTagsManager.Implementation.Resource;

public delegate bool FileFilter(FileInfo file);

public class ResourceRepository(
    string directoryPath,
    string searchPattern = "*",
    FileFilter? fileFilter = null,
    SearchOption searchOption = SearchOption.AllDirectories,
    IResourceFactory? resourceFactory = null)
    : IResourceRepository
{
    private readonly IResourceFactory _resourceFactory = resourceFactory ?? new ResourceFactory();

    public IEnumerable<IResource> GetAll()
    {
        var directory = new DirectoryInfo(directoryPath);
        var files = directory.EnumerateFiles(searchPattern, searchOption);
        if (fileFilter != null)
            files = files.Where(fileFilter.Invoke);

        return files.Select(file => _resourceFactory.Create(file.FullName, file.Name, file.FullName));
    }

    public IResource Get(IResourceIdentifier identifier)
    {
        if (Contains(identifier) == false)
            throw new ArgumentException("Don't belong to this repository", nameof(identifier));

        return (Resource)identifier;
    }

    public IResourceStreamAccess GetStreamAccess(IResourceIdentifier identifier)
    {
        if (Contains(identifier) == false)
            throw new ArgumentException("Don't belong to this repository", nameof(identifier));

        return new ResourceStreamAccess(identifier.Identifier);
    }

    public bool Contains(IResourceIdentifier identifier)
    {
        return identifier.Identifier.Contains(directoryPath);
    }
}