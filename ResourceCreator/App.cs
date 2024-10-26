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
                new MenuItem("_New", "create new resource file", () => {
                    var filePicker = new SaveDialog(title: "New resource", message: "Create a new file for the Resource", allowedTypes: new List<string>{".resx"});
                    Application.Run(filePicker);
                    if(!filePicker.Canceled) {
                        persistence.CurrentlySelectedResource = (string) filePicker.FilePath;
                        mainWindow.CurrentFile =(string)filePicker.FilePath;
                        mainWindow.SetDataTable(new Dictionary<string, string>{
                            {"example1", "Value one"},
                        });
                    }
                    
                }),
                new MenuItem("_Open", "Open new resource file", () => {
                    var filePicker = new OpenDialog(title: "Open resource", message: "Pick an existing resource file", allowedTypes: new List<string>{".resx"});
                    Application.Run(filePicker);
                    if(!filePicker.Canceled) {
                        try {
                            using var fs = new FileStream(path: (string) filePicker.FilePath, mode: FileMode.Open, access: FileAccess.Read);
                            var resEditor = new ResXLib.ResourceEditor();
                            mainWindow.SetDataTable(resEditor.ReadResxToDictionary(fs));
                            mainWindow.CurrentFile =(string)filePicker.FilePath;
                            persistence.CurrentlySelectedResource = (string) filePicker.FilePath;
                        } catch(Exception ex) {
                            MessageBox.ErrorQuery("Error",$"Error during loading of the Resource: {ex.Message}", "Ok");
                        }
                    }
                }),
                new MenuItem("_Save", "Save the current data", ()=> {
                    if (mainWindow.Dirty) {
                        SaveCurrentFile(mainWindow, persistence);
                    }
                }),
                new MenuItem("_Quit", "Stop the app", () => {
                    if (mainWindow.Dirty) {
                        var choice = MessageBox.Query("Unsaved changes", "Do you want to save the current file?", "SaveAndQuit", "DiscardAndQuit", "Cancel");
                        if(choice == 0) {
                            SaveCurrentFile(mainWindow, persistence);
                            if (!mainWindow.Dirty) {
                                Application.RequestStop();
                            }
                        } else if (choice == 1) {
                            Application.RequestStop();
                        }
                    } else {
                        Application.RequestStop ();
                    }
                })
            })
        });

        mainWindow.CurrentFile =(persistence.CurrentlySelectedResource ?? "<None>");

        Application.Top.Add(menu, mainWindow);
        Application.Run();

        Application.Shutdown();
    }


    public static void SaveCurrentFile(MainWindow mainWindow, IPersistence persistence) {
        try {
            var currentData= mainWindow.GetFromDataTable();
            var resEditor = new ResXLib.ResourceEditor();
            using var fs = new FileStream(path: persistence.CurrentlySelectedResource, mode: FileMode.OpenOrCreate, access: FileAccess.Write);
            var result = resEditor.WriteResxToDictionary(currentData);
            result.CopyTo(fs);
            fs.Close();
            mainWindow.Dirty = false;
        } catch (Exception ex) {
            MessageBox.ErrorQuery("Error",$"Error during saving process: {ex.Message}", "Ok");
        }
    }
}