using System;
using System.Diagnostics;
using Desktop.Widgets;
using Desktop.Widgets.Extensions;
using Gdk;
using Gtk;
using Manager.Desktop;
using Manager.Desktop.Save;
using Window = Gtk.Window;
using WindowType = Gtk.WindowType;

namespace Desktop.Main;

public class MainWindow : Window
{
    private const string OpenFolderIcon = "folder-open-symbolic";
    private const string SaveFolderIcon = "document-save-symbolic";

    private Box? _boxContent;
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

        Destroyed += (_, _) => Application.Quit();
    }

    private MusicManager MusicManager => _musicManager
                                             ?? throw new InvalidOperationException();

    private void DisplayFolder(string folderPath)
    {
        _musicManager = new MusicManager(folderPath);

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
        var artists = MusicManager.LoadArtists();
        var musicTree = new MusicTree(artists, MusicManager);
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
        var response = RunFolderChooserDialog("Open Folder", Stock.Open, out var folder);
        if (response == ResponseType.Ok && folder != null)
            DisplayFolder(folder);
    }

    private async void SaveButton_Clicked(object? _, EventArgs __)
    {
        try
        {
            if (_musicManager == null)
                return;
        
            var fileResponse = RunFolderChooserDialog("Select Folder", Stock.Save, out var folder);
            if (fileResponse == ResponseType.Cancel || folder == null)
                return;

            var settingsResponse = RusSaveSettingDialog(out var settings);
        
            if (settingsResponse == ResponseType.Cancel)
                return;
            
            var saveOperation = new MusicSaveOperation(_musicManager, settings);
        
            await saveOperation.ExecuteAsync(folder);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }

    private ResponseType RunFolderChooserDialog(string title, string okButton, out string? folder)
    {
        folder = null;

        var fileChooserDialog = new FileChooserDialog(title, this, FileChooserAction.SelectFolder);
        fileChooserDialog.AddButton(Stock.Cancel, ResponseType.Cancel);
        fileChooserDialog.AddButton(okButton, ResponseType.Ok);
        fileChooserDialog.DefaultResponse = ResponseType.Ok;
        fileChooserDialog.SelectMultiple = false;

        var response = (ResponseType)fileChooserDialog.Run();
        if (response == ResponseType.Ok)
            folder = fileChooserDialog.Filename;
        fileChooserDialog.Destroy();
        fileChooserDialog.Dispose();
        return response;
    }

    private ResponseType RusSaveSettingDialog(out MusicSaveSettings musicSaveSettings)
    {
        var settingsDialog = new Dialog("Save Settings", this, DialogFlags.Modal);
        settingsDialog.AddButton(Stock.Cancel, ResponseType.Cancel);
        settingsDialog.AddButton(Stock.Ok, ResponseType.Ok);
        settingsDialog.DefaultSize = new Size(300, 200);
        
        var saveSettings = new SaveSettings();
        var dialogContent = (Box)settingsDialog.Child;
        dialogContent.PackStart(saveSettings);
        dialogContent.ShowAll();
        
        var settingsResponse = (ResponseType)settingsDialog.Run();
        musicSaveSettings = saveSettings.GetSettings();
        settingsDialog.Destroy();
        settingsDialog.Dispose();

        return settingsResponse;
    }
}