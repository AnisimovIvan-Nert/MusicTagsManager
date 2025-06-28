namespace Manager.Tests;

public abstract class TestsWithTemporaryFolder : IDisposable
{
    protected readonly DirectoryInfo TestDirectory;
    
    protected TestsWithTemporaryFolder()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        TestDirectory = Directory.CreateDirectory(Path.Join(currentDirectory, "__TEST__" + this.GetType().Name));
    }

    public virtual void Dispose()
    {
        TestDirectory.Delete(true);
        GC.SuppressFinalize(this);
    }
}