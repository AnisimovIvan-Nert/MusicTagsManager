using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Desktop.View;
using Gtk;
using Manager.Implementation.Music;
using Manager.Implementation.Resource;
using Manager.Implementation.Tags;

namespace Desktop.Main;

public class MainWindow : Window
{
    private const string OpenFolderIcon = "folder-open-symbolic";

    private Box? _boxContent;
    private MusicService? _musicService;
    private ResourceService? _resourceService;
    private TagsService? _tagsService;

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

    private void DisplayFolder(string folderPath)
    {
        Remove(Child);

        var horizontalPaned = new Paned(Orientation.Horizontal);
        horizontalPaned.Position = 200;

        var tree = CreateTreeView(folderPath);
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

    private ScrolledWindow CreateTreeView(string folderPath)
    {
        var artists = LoadArtists(folderPath);
        var musicTree = new MusicTree(artists);
        musicTree.SelectionChanged += TreeSelectionChanged;
        var treeScroll = new ScrolledWindow();
        treeScroll.Add(musicTree.TreeView);
        return treeScroll;
    }

    private IEnumerable<ArtistView> LoadArtists(string folderPath)
    {
        var supportedFormats = TagsService.GetSupportedAudioFormats.ToArray();
        var resourceRepository = new ResourceRepository(folderPath, fileFilter: FileFilter);
        _resourceService = new ResourceService(resourceRepository);
        _tagsService = new TagsService(_resourceService);
        _musicService = new MusicService(_resourceService, _tagsService);
        var artists = _musicService.LoadArtists();
        return artists.Select(artist => MusicViewFactory.CreateArtist(artist, _resourceService));

        bool FileFilter(FileInfo fileInfo)
        {
            return supportedFormats.Contains(fileInfo.Extension);
        }
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

    private void OpenButton_Clicked(object? sender, EventArgs e)
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