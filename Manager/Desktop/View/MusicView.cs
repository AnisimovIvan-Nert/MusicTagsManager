using System.Collections.Generic;

namespace Desktop.View;

public record MusicView(string Title, string Artist, string Album, string AlbumArtist, ResourceView Resource);

public record AlbumView(string Title, string Artist, IEnumerable<MusicView> Musics);

public record ArtistView(string Name, IEnumerable<AlbumView> Albums);