using File = System.IO.File;

namespace Manager.Tests;

public class Tests_Prototyping : TestsWithTemporaryFolder
{
    private const string TestFile = "test";
    
    [Fact]
    public void GetTestFile()
    {
        CreateFile(TestFile);

        var files = TestDirectory.GetFiles();
        
        Assert.Single(files);
        Assert.Equal(TestFile, files.Single().Name);
    }

    private const string Mp3Extension = ".mp3";
    private const string WavExtension = ".wav";
    
    [Fact]
    public void GetTestFileWithFormat()
    {
        CreateFile(TestFile + Mp3Extension);
        CreateFile(TestFile + WavExtension);
        
        var mp3 = new FileInfo(Path.Join(TestDirectory.FullName, TestFile + Mp3Extension));
        var wav = new FileInfo(Path.Join(TestDirectory.FullName, TestFile + WavExtension));
        
        Assert.True(mp3.Exists);
        Assert.True(wav.Exists);
    }
    
    [Fact]
    public void GetAllTestFilesWithFormat()
    {
        const int fileCount = 10;

        for (var i = 0; i < fileCount; i++)
        {
            CreateFile(TestFile + i + Mp3Extension);
            CreateFile(TestFile + i + WavExtension);
        }
        
        var files = TestDirectory.GetFiles("*" + Mp3Extension);
        
        Assert.Equal(fileCount, files.Length);
        for (var i = 0; i < fileCount; i++)
            Assert.Contains(TestFile + i + Mp3Extension, files.Select(f => f.Name));
    }

    [Fact]
    public void RenameTestFile()
    {
        var oldName = TestFile + Mp3Extension;
        var newName = TestFile + TestFile + Mp3Extension;
        CreateFile(oldName);
        
        File.Move(Path.Join(TestDirectory.FullName, oldName), Path.Join(TestDirectory.FullName, newName));
        
        var files = TestDirectory.GetFiles();
        
        Assert.Single(files);
        Assert.Equal(newName, files.Single().Name);
    }

    private void CreateFile(string fileName)
    {
        File.Create(Path.Join(TestDirectory.FullName, fileName));
    }
}