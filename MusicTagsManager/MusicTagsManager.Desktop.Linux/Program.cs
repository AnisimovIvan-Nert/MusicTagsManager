using System;
using Gdk;
using GLib;
using MusicTagsManager.Desktop.Linux.CallTraceLogger;
using MusicTagsManager.Desktop.Linux.Main;
using Application = Gtk.Application;

namespace MusicTagsManager.Desktop.Linux;

[ConsoleCallTraceLogger]
public class Program
{
    private const string ApplicationId = "org.MusicTagsManager.Desktop.Linux";

    [STAThread]
    public static void Main(string[] args)
    {
        Application.Init();

        var app = new Application(ApplicationId, ApplicationFlags.None);
        app.Register(Cancellable.Current);

        var win = new MainWindow();
        win.DefaultSize = new Size(800, 600);
        app.AddWindow(win);

        win.ShowAll();
        Application.Run();
    }
}