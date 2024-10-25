using System.Data;
using Terminal.Gui;

namespace Resourcecreator;

internal class MainWindow : View{

    public string CurrentFile  
    {   get {
            return (string) currentFileLabel.Text;
        } 
        set {
            currentFileLabel.Text = value;
        }
    }

    private readonly Label currentFileLabel;
    private readonly DataTable data;


    public void SetDataTable(Dictionary<string,string> resourceData) {
        foreach((var key, var value) in resourceData) {
            var row = data.NewRow();
            row["Key"] = key; 
            row["Value"] = value;
            data.Rows.Add(row);
        }
        placeHolder.Height = Dim.Percent(0f);
        resourceTable.Height = Dim.Percent(100f);
    }

    public bool Dirty {get;set;}

    public Dictionary<string,string> GetFromDataTable() {
        var result = new Dictionary<string,string>();

        foreach(DataRow row in data.Rows) {
            string key = (string) row["Key"];
            string value = (string) row["Value"];
            result[key] = value;
        }
        return result;

    }

    private readonly Label placeHolder;
    private readonly TableView resourceTable;

    public MainWindow () {
        Dirty = false;
        data = new DataTable();
        data.Columns.Add(new DataColumn("Key"));
        data.Columns.Add(new DataColumn("Value"));
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
        placeHolder= new Label(text: "No Data loaded!", autosize: true){
            Width=Dim.Fill(),
            Height = Dim.Percent(100f)
        };

        resourceTable = new TableView(data){
            Width=Dim.Fill(),
            Height = Dim.Percent(0f)
        };

        resourceTable.CellActivated += EditCurrentCell;
        resourceEditView.Add(placeHolder, resourceTable);
        Add(currentFileView, resourceEditView);
    }


		private void EditCurrentCell (TableView.CellActivatedEventArgs e)
		{
			if (e.Table == null)
				return;

			var oldValue = e.Table.Rows [e.Row] [e.Col].ToString ();

			if (GetText ("Enter new value", e.Table.Columns [e.Col].ColumnName, oldValue, out string newText)) {
				try {
					e.Table.Rows [e.Row] [e.Col] = string.IsNullOrWhiteSpace (newText) ? DBNull.Value : (object)newText;
                    Dirty = true;
				} catch (Exception ex) {
					MessageBox.ErrorQuery (60, 20, "Failed to set text", ex.Message, "Ok");
				}

				resourceTable.Update ();
			}
		}
    
    		private bool GetText (string title, string label, string initialText, out string enteredText)
		{
			bool okPressed = false;

			var ok = new Button ("Ok", is_default: true);
			ok.Clicked += () => { okPressed = true; Application.RequestStop (); };
			var cancel = new Button ("Cancel");
			cancel.Clicked += () => { Application.RequestStop (); };
			var d = new Dialog (title, 60, 20, ok, cancel);

			var lbl = new Label () {
				X = 0,
				Y = 1,
				Text = label
			};

			var tf = new TextField () {
				Text = initialText,
				X = 0,
				Y = 2,
				Width = Dim.Fill ()
			};

			d.Add (lbl, tf);
			tf.SetFocus ();

			Application.Run (d);

			enteredText = okPressed ? tf.Text.ToString () : null;
			return okPressed;
		}
}