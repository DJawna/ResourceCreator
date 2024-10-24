using Terminal.Gui;

namespace Resourcecreator;

internal class MainWindow : View{

    public void SetCurrentFile(string newValue) {
        currentFileLabel.Text = newValue;

    }
    private readonly Label currentFileLabel;

    public MainWindow () {
        //Title = "Resource editor";

        var currentFileView = new FrameView("CurrentFile:", new Border{ BorderStyle = BorderStyle.Single }){
            Width=Dim.Fill(),
            Height = Dim.Percent(10f)
        };

        currentFileLabel = new Label(){
        };
        currentFileView.Add(currentFileLabel);
        var resourceEditView = new FrameView("ResourceContent:", new Border{ BorderStyle = BorderStyle.Single }){
            Y = Pos.Bottom(currentFileView),
            Width=Dim.Fill(),
            Height = Dim.Percent(90f)
        };

        //var label = new Label("tltest");


        Add(currentFileView, resourceEditView);
    }

}