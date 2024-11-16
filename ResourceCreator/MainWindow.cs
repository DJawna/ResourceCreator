using System.Data;
using Terminal.Gui;

namespace Resourcecreator;

internal class MainWindow : View
{

    public string CurrentFile
    {
        get
        {
            return currentFileLabel.Text;
        }
        set
        {
            currentFileLabel.Text = value;
        }
    }

    private readonly Label currentFileLabel;
    private readonly DataTable data;

    public void UnsetData()
    {
        Dirty = false;
        data.Rows.Clear();
        placeHolder.Visible = true;
        addRowButton.Visible = false;
        removeSelectedRowButton.Visible = false;
        resourceTable.Visible = false;
        CurrentFile = "<None>";

    }
    public void SetDataTable(Dictionary<string, string> resourceData)
    {
        foreach ((var key, var value) in resourceData)
        {
            var row = data.NewRow();
            row["Key"] = key;
            row["Value"] = value;
            data.Rows.Add(row);
        }
        placeHolder.Visible = false;
        addRowButton.Visible = true;
        resourceTable.Visible = true;
        removeSelectedRowButton.Visible = true;
    }

    public bool Dirty { get; set; }

    public Dictionary<string, string> GetFromDataTable()
    {
        var result = new Dictionary<string, string>();

        foreach (DataRow row in data.Rows)
        {
            string key = (string)row["Key"];
            string value = (string)row["Value"];
            result[key] = value;
        }
        return result;

    }

    private readonly Label placeHolder;
    private readonly TableView resourceTable;
    private readonly Button addRowButton;
    private readonly Button removeSelectedRowButton;

    public MainWindow()
    {
        Dirty = false;
        data = new DataTable();
        data.Columns.Add(new DataColumn("Key"));
        data.Columns.Add(new DataColumn("Value"));
        var currentFileView = new FrameView()
        {
            Title = "CurrentFile:",
            BorderStyle = LineStyle.Single,
            Width = Dim.Fill(),
            Height = Dim.Auto()

        };

        currentFileLabel = new Label()
        {
            Height = Dim.Auto(),
            Width = Dim.Fill(),
            Text = "<None>"
        };
        currentFileView.Add(currentFileLabel);

        var resourceEditView = new FrameView
        {
            Title = "ResourceContent:",
            BorderStyle = LineStyle.Single,
            Y = Pos.Bottom(currentFileView),
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        placeHolder = new Label
        {
            Text = "No Data loaded!",
            Width = Dim.Fill(),
            Height = Dim.Percent(100)
        };
        addRowButton = new Button
        {
            Text = "add Row",
            Width = Dim.Percent(50),
            Height = Dim.Percent(10),
            Visible = false
        };
        addRowButton.MouseClick += (sender, mouseEventArgs) =>
        {
            bool textEntered = GetText("Add Row", "Pick a new Keyname...", "<exampleKey>", out string enteredText);
            if (textEntered)
            {
                var row = data.NewRow();
                row["Key"] = enteredText;
                data.Rows.Add(row);
            }
            Dirty = true;
            resourceTable?.Update();
        };

        removeSelectedRowButton = new Button{
            Text = "remove selected row",
            Width= Dim.Percent(50),
            Height = Dim.Percent(10),
            Visible = false,
            X = Pos.Right(addRowButton),
            Y = 0
        };
        removeSelectedRowButton.MouseClick += RemoveSelcectedRow;

        resourceTable = new TableView
        {
            Table = new DataTableSource(data),
            Y = Pos.Bottom(addRowButton),
            Width = Dim.Fill(),
            Height = Dim.Percent(90),
            Visible = false
        };

        resourceTable.CellActivated += EditCurrentCell;
        resourceEditView.Add(
            placeHolder,
            addRowButton,
            removeSelectedRowButton,
            resourceTable
        );
        
        Add(currentFileView, resourceEditView);
    }

    private void RemoveSelcectedRow(object? sender, MouseEventEventArgs keyevent)
    {
        var rowToDelete = resourceTable.SelectedRow;
        data.Rows.RemoveAt(rowToDelete);
        Dirty = true;
        resourceTable?.Update();
    }


    private void EditCurrentCell(object? sender, CellActivatedEventArgs e)
    {
        if (e.Table == null)
            return;

        var oldValue = e.Table[e.Row, e.Col].ToString();

        if (GetText("Enter new value", e.Table.ColumnNames[e.Col], oldValue, out string newText))
        {
            try
            {
                data.Rows[e.Row][e.Col] = string.IsNullOrWhiteSpace(newText) ? DBNull.Value : (object)newText;
                Dirty = true;
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery(60, 20, "Failed to set text", ex.Message, "Ok");
            }

            resourceTable.Update();
        }
    }

    private bool GetText(string title, string label, string initialText, out string enteredText)
    {
        bool okPressed = false;

        var lbl = new Label()
        {
            Text = label,
            Width = Dim.Fill()
        };

        var tf = new TextField()
        {
            Text = initialText,
            X = Pos.Bottom(lbl),
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        var ok = new Button
        {
            Text = "Ok",
            IsDefault = true
        };

        ok.Accept += (sender, eventargs) => { okPressed = true; Application.RequestStop(); };

        var cancel = new Button
        {
            Text = "Cancel"
        };


        cancel.Accept += (sender, eventargs) => { Application.RequestStop(); };

        var dialog = new Dialog
        {
            Title = title,
            //X = 60, Y = 20 
            Buttons = [ok, cancel],
        };
        dialog.Add(lbl, tf);
        tf.SetFocus();

        Application.Run(dialog);

        enteredText = okPressed ? tf.Text.ToString() : null;
        return okPressed;
    }
}