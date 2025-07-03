using System;
using Desktop.View;
using Gtk;

namespace Desktop.Main;

public class MainWindow : Window
{
    private const string OpenFolderIcon = "folder-open-symbolic";

    private Box? _boxContent;
    private MusicServiceView? _musicService;

    public MainWindow() : base(WindowType.Toplevel)
    {
        var openButton = new Button();
        openButton.Image = Image.NewFromIconName(OpenFolderIcon, IconSize.Button);
        openButton.Clicked += OpenButton_Clicked;

        var headerBar = new HeaderBar();
        headerBar.ShowCloseButton = true;
        headerBar.Title = Title;
        headerBar.PackStart(openButton);
        Titlebar = headerBar;

        Destroyed += (_, _) => Application.Quit();
    }

    private MusicServiceView MusicService => _musicService
                                             ?? throw new InvalidOperationException();

    private void DisplayFolder(string folderPath)
    {
        _musicService = new MusicServiceView(folderPath);

        Remove(Child);

        var horizontalPaned = new Paned(Orientation.Horizontal);
        horizontalPaned.Position = 200;

        var tree = CreateTreeView();
        horizontalPaned.Pack1(tree, false, true);

        var mainScroll = CreateMainView();
        horizontalPaned.Pack2(mainScroll, true, true);

        Child = horizontalPaned;

        ShowAll();
    }

    private ScrolledWindow CreateMainView()
    {
        var mainScroll = new ScrolledWindow();
        _boxContent = new Box(Orientation.Vertical, 0);
        _boxContent.Margin = 8;
        mainScroll.Child = _boxContent;
        return mainScroll;
    }

    private ScrolledWindow CreateTreeView()
    {
        var artists = MusicService.LoadArtists();
        var musicTree = new MusicTree(artists, MusicService);
        musicTree.SelectionChanged += TreeSelectionChanged;
        var treeScroll = new ScrolledWindow();
        treeScroll.Add(musicTree.TreeView);
        return treeScroll;
    }

    private void TreeSelectionChanged(Widget widget)
    {
        if (_boxContent == null)
            return;

        while (_boxContent.Children.Length > 0)
            _boxContent.Remove(_boxContent.Children[0]);

        _boxContent.PackStart(widget, true, true, 0);
        _boxContent.ShowAll();
    }

    private void OpenButton_Clicked(object? _, EventArgs __)
    {
        var fileChooserDialog = new FileChooserDialog("Open Folder", this, FileChooserAction.SelectFolder);
        fileChooserDialog.AddButton(Stock.Cancel, ResponseType.Cancel);
        fileChooserDialog.AddButton(Stock.Open, ResponseType.Ok);
        fileChooserDialog.DefaultResponse = ResponseType.Ok;
        fileChooserDialog.SelectMultiple = false;

        var response = (ResponseType)fileChooserDialog.Run();
        if (response == ResponseType.Ok)
            DisplayFolder(fileChooserDialog.Filename);
        fileChooserDialog.Destroy();
    }
}