using Resourcecreator.Services.Impl;
using Terminal.Gui;

namespace Resourcecreator;

internal class App
{

    public static void Start() {
        /*
        var mainWindow = new MainWindow();
        mainWindow.Persistence = new HomeFolderPersistence();
        */

        Application.Init();

        var menu = new MenuBar(new MenuBarItem[]{
            new MenuBarItem ("_File", new MenuItem []{
                new MenuItem("_Quit", "Stop the app", () => {
                    Application.RequestStop ();
                })
            })
        });
        var mainWindow = new MainWindow(){
            X= 0,
            Y= 1,
            Width= Dim.Fill(),
            Height = Dim.Fill() -1
        };

        Application.Top.Add(menu, mainWindow);
        Application.Run();

        Application.Shutdown();
    }
}