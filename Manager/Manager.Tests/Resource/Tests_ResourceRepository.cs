using System.Text;
using Manager.Resource;
using Manager.Resource.Implementation;

namespace Manager.Tests.Resource;

public class Tests_ResourceRepository : TestsWithTemporaryFolder
{
    private const string TestFileName = "test";
    private const int TestFileCount = 10;
    
    private const string EmptyExtension = "";
    private const string Mp3Extension = ".mp3";
    private const string OggExtension = ".ogg";
    private readonly string[] _testFileExtensions = [EmptyExtension, Mp3Extension, OggExtension];
    
    private readonly string[] _testFilePaths;
    
    public Tests_ResourceRepository()
    {
        var filePaths = GenerateFiles(TestFileName, 
            TestFileCount, _testFileExtensions);
        _testFilePaths = filePaths.ToArray();
    }
    
    private IEnumerable<string> GenerateFiles(string nameBase, int count, string[] extensions)
    {
        foreach (var extension in extensions)
        {
            for (var i = 0; i < count; i++)
            {
                yield return CreateFile(nameBase + i + extension);
            }
        }
    }

    [Fact]
    public void GetAll_WithDefaultRepository_ReturnAllResources()
    {
        var repository = new ResourceRepository(TestDirectory.FullName);

        var resources = repository.GetAll().ToList();
        
        Assert.Equal(_testFilePaths.Length, resources.Count);
        foreach (var path in _testFilePaths)
        {
            var name = Path.GetFileName(path);
            Assert.Contains(resources, resource => resource.Name == name 
                                                       && resource.Identifier == path);
        }
    }
    
    [Fact]
    public void GetAll_WithMp3SearchPattern_ReturnOnlyMp3Resources()
    {
        var searchPattern = "*" + Mp3Extension;
        var repository = new ResourceRepository(TestDirectory.FullName, searchPattern);
        
        var resources = repository.GetAll().ToList();
        
        Assert.Equal(TestFileCount, resources.Count);
        foreach (var resource in resources)
        {
            var extension = Path.GetExtension(resource.Name);
            Assert.Equal(Mp3Extension, extension);
            Assert.Contains(_testFilePaths, path => path == resource.Identifier);
        }
    }

    [Fact]
    public void GetAll_WithFileFilter_ReturnOnlyFilteredResources()
    {
        var filterCondition = (TestFileCount / 2).ToString();
        var repository = new ResourceRepository(TestDirectory.FullName, fileFilter: FileFilter);
        
        var resources = repository.GetAll().ToList();
        
        Assert.Equal(_testFileExtensions.Length, resources.Count);
        foreach (var resource in resources)
        {
            var name = resource.Name;
            var extension = Path.GetExtension(name);
            if (string.IsNullOrEmpty(extension) == false)
                name = name.Replace(extension, EmptyExtension);
            Assert.Contains(filterCondition, name);
            Assert.Contains(_testFilePaths, path => path == resource.Identifier);
        }
        return;
        
        bool FileFilter(FileInfo fileInfo)
        {
            var name = fileInfo.Name;
            var extension = fileInfo.Extension;
            if (string.IsNullOrEmpty(extension) == false)
                name = name.Replace(extension, EmptyExtension);
            return name.Contains(filterCondition);
        }
    }
    
    [Fact]
    public void Contains_WithValidIdentifier_ReturnTrue()
    {
        var repository = new ResourceRepository(TestDirectory.FullName);
        var resources = repository.GetAll().First();
        IResourceIdentifier identifier = resources;
        
        var contains = repository.Contains(identifier);
        
        Assert.True(contains);
    }

    [Fact]
    public void Contains_WithInvalidIdentifier_ReturnFalse()
    {
        const string identifier = "identifier";
        const string name = "name";
        
        var resourceFactory = new ResourceFactory();
        var repository = new ResourceRepository(TestDirectory.FullName);
        var resources = resourceFactory.Create(identifier, name);
        IResourceIdentifier resourceIdentifier = resources;
        
        var contains = repository.Contains(resourceIdentifier);
        
        Assert.False(contains);
    }

    [Fact]
    public void GetStreamAccess_WithValidIdentifier_ReturnReadWriteStreamAccess()
    {
        const string textData = "some text";
        
        var repository = new ResourceRepository(TestDirectory.FullName);
        var resources = repository.GetAll().First();
        IResourceIdentifier identifier = resources;
        var byteData = Encoding.UTF8.GetBytes(textData);
        
        
        var streamAccess = repository.GetStreamAccess(identifier);
        using (var writeStream = streamAccess.OpenWrite())
            writeStream.Write(byteData, 0, byteData.Length);
        
        var buffer = new byte[byteData.Length];
        using (var readStream = streamAccess.OpenRead())
            readStream.ReadExactly(buffer, 0, buffer.Length);
        
        
        var textFromStream =  Encoding.UTF8.GetString(buffer);
        Assert.Equal(textData, textFromStream);
    }
}