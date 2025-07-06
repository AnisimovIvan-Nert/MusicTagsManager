using MusicTagsManager.Desktop.Views;

namespace MusicTagsManager.Desktop.Save;

public class MusicSaveOperation(MusicManager musicManager, MusicSaveSettings saveSettings)
{
    public void Execute(string destination)
    {
        foreach (var targetDestination in GetTargetDestinationPairs(destination))
        {
            Copy(targetDestination.taget, targetDestination.destination);
        }
    }

    public async ValueTask ExecuteAsync(string destination)
    {
        foreach (var targetDestination in GetTargetDestinationPairs(destination))
        {
            await CopyAsync(targetDestination.taget, targetDestination.destination);
        }
    }

    private IEnumerable<(MusicView taget, FileInfo destination)> GetTargetDestinationPairs(string destinationPath)
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
            var path = Path.Combine(destinationPath, relativeFilePath);
            var fileInfo = new FileInfo(path);

            var directory = fileInfo.Directory;

            if (directory == null)
                throw new InvalidOperationException();

            directory.Create();

            yield return (music, fileInfo);
        }
    }

    private void Copy(MusicView taget, FileInfo destination)
    {
        using var fileStream = destination.Create();
        using var musicStream = musicManager.GetReadStream(taget);
        musicStream.CopyTo(fileStream);
    }

    private async ValueTask CopyAsync(MusicView taget, FileInfo destination)
    {
        await using var fileStream = destination.Create();
        await using var musicStream = musicManager.GetReadStream(taget);
        await musicStream.CopyToAsync(fileStream);
    }
}