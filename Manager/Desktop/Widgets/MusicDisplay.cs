using Gtk;
using Manager.Desktop.Views;

namespace Desktop.Widgets;

public class MusicDisplay : ListSection
{
    public MusicDisplay(MusicView musicView)
    {
        AddLabel(nameof(musicView.Title), musicView.Title);
        AddLabel(nameof(musicView.Artist), musicView.Artist);
        AddLabel(nameof(musicView.Album), musicView.Album);
        AddLabel(nameof(musicView.AlbumArtist), musicView.AlbumArtist);

        AddLabel(nameof(musicView.Resource.Name), musicView.Resource.Name);
        AddLabel(nameof(musicView.Resource.Location), musicView.Resource.Location);
    }

    private void AddLabel(string name, string value)
    {
        var label = new Label(value);
        AddItem(name, label);
    }
}