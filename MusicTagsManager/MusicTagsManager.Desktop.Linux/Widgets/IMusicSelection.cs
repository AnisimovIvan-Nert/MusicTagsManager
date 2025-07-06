using System.Collections.Generic;
using MusicTagsManager.Desktop.Views;

namespace MusicTagsManager.Desktop.Linux.Widgets;

public interface IMusicSelection
{
    public IEnumerable<MusicView> GetSelectedMusic();
}