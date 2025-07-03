namespace Manager.Desktop.Save;

public class MusicSaveOperation(MusicManager musicManager, MusicSaveSettings saveSettings)
{
    public async Task ExecuteAsync(string destination)
    {
        var artists = musicManager.LoadArtists();
        var musics = artists
            .SelectMany(artist => artist.Albums)
            .SelectMany(album => album.Musics);

        foreach (var music in musics)
        {
            var fileExtension = musicManager.GetMusicExtension(music);
            var relativeFilePath = saveSettings.GetFilePath(music);
            relativeFilePath += fileExtension;
            var path = Path.Combine(destination, relativeFilePath);
            var fileInfo = new FileInfo(path);
            
            var directory = fileInfo.Directory;

            if (directory == null)
                throw new InvalidOperationException();
            
            directory.Create();

            await using var fileStream = fileInfo.Create();
            await using var musicStream = musicManager.GetReadStream(music);
            await musicStream.CopyToAsync(fileStream);
        }
    }
}