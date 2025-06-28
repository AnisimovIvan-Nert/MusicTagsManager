using TagLib;

namespace Manager.Tests;

public class Tests_TagsPrototyping : TestsWithTemporaryFolder
{
    private const string MusicFolderVariable = "MUSIC_FOLDER_PATH";
    private const string MusicFileNameVariable = "MUSIC_FILE_NAME";
    private const string MusicArtistVariable = "MUSIC_ARTIST";
    
    [Fact]
    public void WriteTag()
    {
        const string newTitle = "newTitle";
        
        var musicDirectory = GetMusicDirectory();
        var musicFilePath = Path.Combine(musicDirectory, GetMusicFileName());
        var musicFile = new FileInfo(musicFilePath);
        var copy = musicFile.CopyTo(Path.Join(TestDirectory.FullName, musicFile.Name));
        
        var tag = TagLib.File.Create(copy.FullName);
        tag.Tag.Title = newTitle;
        tag.Save();
        tag.Dispose();
        
        tag = TagLib.File.Create(copy.FullName);
        Assert.Equal(newTitle, tag.Tag.Title);
    }
    
    [Fact]
    public void ReadTag()
    {
        var musicDirectory = GetMusicDirectory();
        var musicFilePath = Path.Combine(musicDirectory, GetMusicFileName());
        var musicFile = new FileInfo(musicFilePath);
        
        var copy = musicFile.CopyTo(Path.Join(TestDirectory.FullName, musicFile.Name));
        
        var tag = TagLib.File.Create(copy.FullName);
        Assert.Equal(GetMusicArtist(), tag.Tag.FirstPerformer ?? tag.Tag.FirstAlbumArtist);
    }

    //[Fact]
    public void CreateFolderHierarchy()
    {
        var musicDirectory = GetMusicDirectory();

        var musics = new DirectoryInfo(musicDirectory).GetFiles();

        foreach (var file in musics)
        {
            var tag = TagLib.File.Create(file.FullName, ReadStyle.PictureLazy);
            var artist = tag.Tag.FirstPerformer ?? tag.Tag.FirstAlbumArtist;
            var album = tag.Tag.Album;
            var title = tag.Tag.Title;

            var path = TestDirectory.FullName;
            
            if (string.IsNullOrEmpty(artist) == false)
                path = Path.Combine(path, artist);
            if (string.IsNullOrEmpty(album) == false)
                path = Path.Combine(path, album);

            if (string.IsNullOrEmpty(title) || title.Contains("/") || title.Contains(@"\"))
                title = file.Name;
            
            Directory.CreateDirectory(path);
            path = Path.Combine(path, title + file.Extension);
            file.CopyTo(path);
        }
    }
    private string GetMusicDirectory()
    {
        var musicFolderPath = GetEnvironmentVariable(MusicFolderVariable);
        return Path.Combine(GetUserDirectory(), musicFolderPath);
    }
    
    private string GetUserDirectory()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    }

    private string GetMusicFileName()
    {
        return GetEnvironmentVariable(MusicFileNameVariable);
    }

    private string GetMusicArtist()
    {
        return GetEnvironmentVariable(MusicArtistVariable);
    }

    private string GetEnvironmentVariable(string variable)
    {
        return Environment.GetEnvironmentVariable(variable)
            ?? throw new NullReferenceException(variable + " environment variable is missing.");
    }
}