using Resourcecreator.Services.Api;
using Resourcecreator.Services.Impl;
using Terminal.Gui;

namespace Resourcecreator;

internal class App
{

    public static void Start(IPersistence persistence) {
        /*
        var mainWindow = new MainWindow();
        mainWindow.Persistence = new HomeFolderPersistence();
        */

        Application.Init();
        var mainWindow = new MainWindow(){
            X= 0,
            Y= 1,
            Width= Dim.Fill(),
            Height = Dim.Fill() -1
        };

        var menu = new MenuBar(new MenuBarItem[]{
            new MenuBarItem ("_File", new MenuItem []{
                new MenuItem("_Open", "Open new resource file", () => {
                    var filePicker = new FileDialog(title: "Open resource", prompt: "Confirm", message: "Pick an existing resource file", allowedTypes: new List<string>{"*.resx"});
                    Application.Run(filePicker);
                    if(!filePicker.Canceled) {
                        persistence.CurrentlySelectedResource = (string) filePicker.FilePath;
                        mainWindow.SetCurrentFile((string)filePicker.FilePath);
                    }
                    
                }),
                new MenuItem("_Quit", "Stop the app", () => {
                    Application.RequestStop ();
                })
            })
        });

        mainWindow.SetCurrentFile(persistence.CurrentlySelectedResource ?? "<None>");

        Application.Top.Add(menu, mainWindow);
        Application.Run();

        Application.Shutdown();
    }
}