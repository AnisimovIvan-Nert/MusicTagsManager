using System;
using Desktop.CallTraceLogger;
using Desktop.Main;
using Gdk;
using GLib;
using Application = Gtk.Application;

namespace Desktop;

[ConsoleCallTraceLogger]
public class Program
{
    private const string ApplicationId = "org.Desktop.Desktop";

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