using System;
using Desktop.Main;
using Gdk;
using GLib;
using Application = Gtk.Application;

namespace Desktop;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Application.Init();

        var app = new Application("Music Manager", ApplicationFlags.None);
        app.Register(Cancellable.Current);

        var win = new MainWindow();
        win.DefaultSize = new Size(800, 600);
        app.AddWindow(win);

        win.ShowAll();
        Application.Run();
    }
}