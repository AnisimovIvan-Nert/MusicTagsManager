namespace Manager.Resource.Implementation;

public delegate bool FileFilter(FileInfo file);

public class ResourceRepository(
    string directoryPath,
    string searchPattern = "*",
    FileFilter? fileFilter = null,
    SearchOption searchOption = SearchOption.AllDirectories)
    : IResourceRepository
{
    public IEnumerable<IResource> GetAll()
    {
        var directory = new DirectoryInfo(directoryPath);
        var files = directory.EnumerateFiles(searchPattern, searchOption);
        if (fileFilter != null)
            files = files.Where(fileFilter.Invoke);

        return files.Select(file => new Resource(file.FullName, file.Name));
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