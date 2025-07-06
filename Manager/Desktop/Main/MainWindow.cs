using System;
using Desktop.CallTraceLogger;
using Desktop.Widgets;
using Desktop.Widgets.Extensions;
using Gdk;
using Gtk;
using Manager.Desktop;
using Manager.Desktop.Save;
using Window = Gtk.Window;
using WindowType = Gtk.WindowType;

namespace Desktop.Main;

[ConsoleCallTraceLogger]
public class MainWindow : Window
{
    private const string OpenFolderIcon = "folder-open-symbolic";
    private const string SaveFolderIcon = "document-save-symbolic";

    private readonly Paned _windowContent;

    private Box? _mainViewContent;
    private MusicManager? _musicManager;

    public MainWindow() : base(WindowType.Toplevel)
    {
        var openButton = new Button();
        openButton.Image = Image.NewFromIconName(OpenFolderIcon, IconSize.Button);
        openButton.Clicked += OpenButton_Clicked;

        var saveButton = new Button();
        saveButton.Image = Image.NewFromIconName(SaveFolderIcon, IconSize.Button);
        saveButton.Clicked += SaveButton_Clicked;

        var headerBar = new HeaderBar();
        headerBar.ShowCloseButton = true;
        headerBar.Title = Title;
        headerBar.PackStart(openButton);
        headerBar.PackStart(saveButton);
        Titlebar = headerBar;

        _windowContent = new Paned(Orientation.Horizontal);
        _windowContent.Position = 200;
        Child = _windowContent;

        Destroyed += (_, _) => Application.Quit();
    }

    private MusicManager MusicManager => _musicManager
                                         ?? throw new InvalidOperationException();

    private void DisplayFolder(string folderPath)
    {
        if (_windowContent.Child1 != null)
            _windowContent.Remove(_windowContent.Child1);
        if (_windowContent.Child2 != null)
            _windowContent.Remove(_windowContent.Child2);

        _musicManager = new MusicManager(folderPath);

        var tree = CreateTreeView();
        _windowContent.Pack1(tree, false, true);

        var mainScroll = CreateMainView();
        _windowContent.Pack2(mainScroll, true, true);

        ShowAll();
    }

    private ScrolledWindow CreateMainView()
    {
        var mainScroll = new ScrolledWindow();
        _mainViewContent = new Box(Orientation.Vertical, 0);
        _mainViewContent.Margin = 8;
        mainScroll.Child = _mainViewContent;
        return mainScroll;
    }

    private ScrolledWindow CreateTreeView()
    {
        var artists = MusicManager.LoadArtists();
        var musicTree = new MusicTree(artists, MusicManager);
        musicTree.SelectionChanged += TreeSelectionChanged;
        var treeScroll = new ScrolledWindow();
        treeScroll.Add(musicTree.TreeView);
        return treeScroll;
    }

    private void TreeSelectionChanged(Widget widget)
    {
        if (_mainViewContent == null)
            return;

        while (_mainViewContent.Children.Length > 0)
            _mainViewContent.Remove(_mainViewContent.Children[0]);

        _mainViewContent.PackStart(widget, true, true, 0);
        _mainViewContent.ShowAll();
    }

    private void OpenButton_Clicked(object? _, EventArgs __)
    {
        var response = RunFolderChooserDialog("Open Folder", Stock.Open, out var folder);
        if (response == ResponseType.Ok && folder != null)
            DisplayFolder(folder);
    }

    private void SaveButton_Clicked(object? _, EventArgs __)
    {
        if (_musicManager == null)
            return;

        var fileResponse = RunFolderChooserDialog("Select Folder", Stock.Save, out var folder);
        if (fileResponse == ResponseType.Cancel || folder == null)
            return;

        var settingsResponse = RunSaveSettingDialog(out var settings);

        if (settingsResponse == ResponseType.Cancel)
            return;

        var saveOperation = new MusicSaveOperation(_musicManager, settings);

        saveOperation.Execute(folder);
    }

    private ResponseType RunFolderChooserDialog(string title, string okButton, out string? folder)
    {
        folder = null;

        using var fileChooserDialog = new FileChooserDialog(title, this, FileChooserAction.SelectFolder);
        fileChooserDialog.AddButton(Stock.Cancel, ResponseType.Cancel);
        fileChooserDialog.AddButton(okButton, ResponseType.Ok);
        fileChooserDialog.DefaultResponse = ResponseType.Ok;
        fileChooserDialog.SelectMultiple = false;

        var response = (ResponseType)fileChooserDialog.Run();
        if (response == ResponseType.Ok)
            folder = fileChooserDialog.Filename;

        return response;
    }

    private ResponseType RunSaveSettingDialog(out MusicSaveSettings musicSaveSettings)
    {
        using var settingsDialog = new Dialog("Save Settings", this, DialogFlags.Modal);
        settingsDialog.AddButton(Stock.Cancel, ResponseType.Cancel);
        settingsDialog.AddButton(Stock.Ok, ResponseType.Ok);
        settingsDialog.DefaultSize = new Size(300, 200);

        var saveSettings = new SaveSettings();
        var dialogContent = (Box)settingsDialog.Child;
        dialogContent.PackStart(saveSettings);
        dialogContent.ShowAll();

        var settingsResponse = (ResponseType)settingsDialog.Run();
        musicSaveSettings = saveSettings.GetSettings();

        return settingsResponse;
    }
}