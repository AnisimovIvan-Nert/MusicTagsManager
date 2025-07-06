using System;
using System.Collections.Generic;
using System.Linq;
using Gtk;
using MusicTagsManager.Desktop.Linux.CallTraceLogger;
using MusicTagsManager.Desktop.Linux.Widgets.Extensions;
using MusicTagsManager.Desktop.Views;
using Action = System.Action;

namespace MusicTagsManager.Desktop.Linux.Widgets;

[ConsoleCallTraceLogger]
public class MusicEditor : Box
{
    private const string EditIconName = "document-edit-symbolic";
    private const string ConfirmIconName = "emblem-ok-symbolic";
    private const string CancelIconName = "application-exit-symbolic";
    private readonly Box _content;
    private readonly Widget _musicDisplay;
    private readonly MusicManager _musicManager;
    private readonly MusicView[] _musicViews;
    private readonly IMusicSelection? _musicSelection;

    private TagsEditor? _tagsEditor;

    public MusicEditor(IEnumerable<MusicView> musics, Widget musicDisplay,
        MusicManager musicManager, IMusicSelection? musicSelection = null)
        : base(Orientation.Vertical, 0)
    {
        _musicViews = musics.ToArray();
        _musicManager = musicManager;
        _musicDisplay = musicDisplay;
        _musicSelection = musicSelection;

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
        var selectedMusic = _musicSelection?.GetSelectedMusic().ToArray();
        if (selectedMusic == null || selectedMusic.Length == 0)
            selectedMusic = _musicViews;

        _tagsEditor = new TagsEditor(selectedMusic, _musicManager);
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