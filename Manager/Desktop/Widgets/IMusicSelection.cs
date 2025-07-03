using System.Collections.Generic;
using Manager.Desktop.Views;

namespace Desktop.Widgets;

public interface IMusicSelection
{
    public IEnumerable<MusicView> GetSelectedMusic();
}