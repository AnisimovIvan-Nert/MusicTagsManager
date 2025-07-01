using Desktop.View;
using Gtk;

namespace Desktop.Widgets.MusicDisplay;

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

    private void AddLabel(string title, string text)
    {
        var label = new Label(text);
        label.Xalign = -1;
        AddItem(title, label);
    }
}