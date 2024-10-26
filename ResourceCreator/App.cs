using Resourcecreator.Services.Api;
using Resourcecreator.Services.Impl;
using Terminal.Gui;

namespace Resourcecreator;

internal class App
{

    public static void Start(IPersistence persistence, string? resourceFile) {
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

        var menu = new MenuBar() {
            Menus = new MenuBarItem[]{
            new MenuBarItem ("_File", new MenuItem []{
                new MenuItem("_New", "create new resource file", () => {
                    var filePicker = new SaveDialog(){
                        Title = "Pick file for New Resource",
                        AllowedTypes = new List<IAllowedType>{new AllowedType("Microsoft resource file", [".resx"])},
                        Path = persistence.CurrentWorkFolder
                    };
                    
                    Application.Run(filePicker);
                    if(!filePicker.Canceled) {
                        persistence.CurrentWorkFolder = Path.GetDirectoryName(filePicker.Path) ?? "./";
                        mainWindow.CurrentFile =filePicker.FileName;
                        mainWindow.SetDataTable(new Dictionary<string, string>{
                            {"example1", "Value one"},
                        });

                    }
                    
                }),
                new MenuItem("_Open", "Open new resource file", () => {
                    var filePicker = new OpenDialog() {
                        Title = "Open resource: Pick an existing resource file", 
                        AllowedTypes = new List<IAllowedType>{ new AllowedType(description: "Microsoft resource files", [".resx"])},
                        Path = persistence.CurrentWorkFolder,
                        AllowsMultipleSelection = false
                    };
                    Application.Run(filePicker);
                    if(!filePicker.Canceled) {
                        LoadFile(filePicker.FilePaths.First(), mainWindow, persistence);
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
        }
        };

        mainWindow.CurrentFile ="<None>";

        var topLevel = new Toplevel();
        topLevel.Add(menu, mainWindow);

        if (resourceFile!= null) {
            LoadFile(resourceFile, mainWindow, persistence);
        }
        Application.Run(topLevel);

        Application.Shutdown();
    }

    public static void LoadFile(string resourceFile, MainWindow mainWindow, IPersistence persistence) {

        try {
            using var fs = new FileStream(path: resourceFile, mode: FileMode.Open, access: FileAccess.Read);
            var resEditor = new ResXLib.ResourceEditor();
            mainWindow.SetDataTable(resEditor.ReadResxToDictionary(fs));
            mainWindow.CurrentFile = resourceFile;
            persistence.CurrentWorkFolder = Path.GetDirectoryName(resourceFile) ?? "./";
        } catch(Exception ex) {
            MessageBox.ErrorQuery("Error",$"Error during loading of the Resource: {ex.Message}", "Ok");
        }
    }


    public static void SaveCurrentFile(MainWindow mainWindow, IPersistence persistence) {
        try {
            var currentData= mainWindow.GetFromDataTable();
            var resEditor = new ResXLib.ResourceEditor();
            using var fs = new FileStream(path: mainWindow.CurrentFile, mode: FileMode.OpenOrCreate, access: FileAccess.Write);
            var result = resEditor.WriteResxToDictionary(currentData);
            result.CopyTo(fs);
            fs.Close();
            mainWindow.Dirty = false;
        } catch (Exception ex) {
            MessageBox.ErrorQuery("Error",$"Error during saving process: {ex.Message}", "Ok");
        }
    }
}