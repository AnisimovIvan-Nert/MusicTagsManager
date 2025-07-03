using System;
using System.Collections.Generic;
using System.Linq;
using Desktop.View;
using Desktop.Widgets.Extensions;
using Gtk;
using Action = System.Action;

namespace Desktop.Widgets;

public class MusicEditor : Box
{
    private const string EditIconName = "document-edit-symbolic";
    private const string ConfirmIconName = "emblem-ok-symbolic";
    private const string CancelIconName = "application-exit-symbolic";
    private readonly Box _content;
    private readonly Widget _musicDisplay;
    private readonly MusicServiceView _musicService;

    private readonly MusicView[] _musicViews;

    private TagsEditor? _tagsEditor;

    public MusicEditor(IEnumerable<MusicView> musics, Widget musicDisplay, MusicServiceView musicService)
        : base(Orientation.Vertical, 0)
    {
        _musicViews = musics.ToArray();
        _musicService = musicService;
        _musicDisplay = musicDisplay;

        _content = new Box(Orientation.Vertical, 0);
        _content.Margin = 8;
        Child = _content;

        RedrawInDisplayMode();
    }

    public event Action? MusicUpdated;

    private void RedrawInDisplayMode()
    {
        ClearContent();

        PackEditButtonsBox();
        PackMusicDisplay();

        ShowAll();
    }

    private void RedrawInEditMode()
    {
        ClearContent();

        PackSaveButtonsBox();
        PackTagsEditor();

        ShowAll();
    }

    private void EditButton_Clicked(object? _, EventArgs __)
    {
        RedrawInEditMode();
    }

    private void SaveButton_Clicked(object? _, EventArgs __)
    {
        if (GetConfirmation())
            SaveChanges();
        RedrawInDisplayMode();
    }

    private void CancelButton_Clicked(object? _, EventArgs __)
    {
        RedrawInDisplayMode();
    }

    private void PackEditButtonsBox()
    {
        var editButton = CreateButton(EditIconName, EditButton_Clicked);

        var editButtonBox = new Box(Orientation.Horizontal, 0);
        editButtonBox.PackStart(editButton);
        _content.PackStart(editButtonBox);
    }

    private void PackSaveButtonsBox()
    {
        var saveButton = CreateButton(ConfirmIconName, SaveButton_Clicked);
        var cancelButton = CreateButton(CancelIconName, CancelButton_Clicked);

        var saveButtonsBox = new Box(Orientation.Horizontal, 0);
        saveButtonsBox.PackStart(saveButton);
        saveButtonsBox.PackStart(cancelButton);
        _content.PackStart(saveButtonsBox);
    }

    private void PackMusicDisplay()
    {
        _content.PackStart(_musicDisplay);
    }

    private void PackTagsEditor()
    {
        _tagsEditor = new TagsEditor(_musicViews, _musicService);
        _content.PackStart(_tagsEditor);
    }

    private bool GetConfirmation()
    {
        var parentWindow = this.GetParentWindow();
        var confirmationDialog = new MessageDialog(parentWindow, DialogFlags.Modal,
            MessageType.Question, ButtonsType.YesNo, "");
        confirmationDialog.Text = "Save changes?";
        var response = (ResponseType)confirmationDialog.Run();
        confirmationDialog.Destroy();
        return response == ResponseType.Yes;
    }

    private void SaveChanges()
    {
        if (_tagsEditor == null)
            throw new InvalidOperationException();

        _tagsEditor.Save();
        MusicUpdated?.Invoke();
    }

    private void ClearContent()
    {
        while (_content.Children.Length > 0)
            _content.Remove(_content.Children.First());
    }

    private static Button CreateButton(string iconName, EventHandler clickHandler)
    {
        var button = new Button();
        button.Image = Image.NewFromIconName(iconName, IconSize.Button);
        button.Clicked += clickHandler;
        return button;
    }
}