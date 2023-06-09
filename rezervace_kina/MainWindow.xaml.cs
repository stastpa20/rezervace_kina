using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using SQLite;
using SQLitePCL;
using System.Windows.Media.Media3D;
using System.Xml.Linq;


namespace rezervace_kina
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //values
        int rowValue;
        int columnValue;
        string uuidValue;
        string movieNameValue;
        string cinemaNameValue;
        string stateValue;

        //filepaths
        string DataSource = "seats.db";
        private string jsonpath = "filmy.json";

        private Button sub;
        private Grid subGrid;

        private List<List<(Button, seat)>> subButtons = new List<List<(Button, seat)>>();

        List<List<(Button, seat)>> buttons = new List<List<(Button, seat)>>();

        List<seat> reservations = new List<seat>();

        private List<Projection> projections;

        TextBox subNameBox;
        TextBox subEmailBox;

        private static List<string> jsonlist = new List<string>();

        private int rows = 7;
        private int columns = 22;

        //brushes
        private Brush unavailable = Brushes.DarkGray;
        private Brush sold = Brushes.Crimson;
        private Brush reserved = Brushes.DarkOrange;
        Brush unoccupied = Brushes.LightGray;


        public MainWindow()
        {
            InitializeComponent();
            InitialiseConnection();
            Title = "Seznam Projekcí";
            ResizeMode = ResizeMode.NoResize;
            ReadFile();
            Content = listvju();
        }

        // init
        public void InitialiseConnection()
        {            
            SQLiteConnectionString options = new SQLiteConnectionString(DataSource, false);

            SQLiteConnection conn = new SQLiteConnection(options);
            try
            {
                string query = $"SELECT * FROM seats";
                conn.Query<seat>(query);
            }
            catch
            {
                conn.CreateTable<seat>();
            }
        }

        // select and return all seats
        public List<seat> GetRecords()
        {
            var options = new SQLiteConnectionString(DataSource, false);
            var conn = new SQLiteConnection(options);

            var results = conn.Table<seat>().ToList();

            return results;            
        }

        // insert new record into table
        public void InsertRecord()
        {
            var options = new SQLiteConnectionString(DataSource, false);
            var conn = new SQLiteConnection(options);

            var record = new seat { id = Guid.NewGuid(), row = rowValue, column = columnValue, uuid = uuidValue, cinemaName = cinemaNameValue, movieName = movieNameValue, state = stateValue };

            var results = conn.Insert(record);

            conn.Close();
        }

        // update record in table
        public void UpdateRecord(Guid id)
        {
            var options = new SQLiteConnectionString(DataSource, false);
            var conn = new SQLiteConnection(options);

            var record = new seat { id = id, row = rowValue, column = columnValue, uuid = uuidValue, cinemaName = cinemaNameValue, movieName = movieNameValue, state = stateValue };

            var results = conn.Update(record);

            conn.Close();
        }

        // delete record from table
        public void DeleteRecord(string id)
        {
            var options = new SQLiteConnectionString(DataSource, false);
            var conn = new SQLiteConnection(options);

            var result = conn.Delete<seat>(id);
            conn.Close();
        }

        // create new listview
        ListView listvju()
        {
            ListView listView = new ListView();
            for (int i = 0; i < projections.Count; i++)
            {
                listView.Items.Add($"Kino: {projections[i].cinema.name} Film: {projections[i].name}");
            }

            listView.MouseDoubleClick += new MouseButtonEventHandler(listView_DoubleClick);
            return listView;
        }

        // listview double click on item
        private void listView_DoubleClick(object sender, MouseEventArgs e)
        {
            ListView x = (ListView)sender;
            var i = x.SelectedIndex;

            rows = projections[i].cinema.rows;
            columns = projections[i].cinema.columns;
            Title = projections[i].cinema.name;
            cinemaNameValue = projections[i].cinema.name;
            movieNameValue = projections[i].name;
            uuidValue = projections[i].uuid;
            Content = CreateGrid();            
        }

        // create new cinema grid
        Grid CreateGrid()
        {
            // new grid 
            Grid myGrid = new Grid();

            // add rows and columns into grid
            for (int i = 0; i < columns + 2; i++)
            {
                myGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < rows + 1; i++)
            {
                myGrid.RowDefinitions.Add(new RowDefinition());
            }
            buttons = new List<List<(Button, seat)>>();
            for (int i = 0; i < columns + 2; i++)
            {
                List<(Button, seat)> column = new List<(Button, seat)>();
                for (int j = 0; j < rows + 1; j++)
                {
                    if (j == 0)
                    {
                        // platno
                        if (i == 0)
                        {
                            Label lbl = new Label();
                            lbl.Name = "platno";
                            lbl.Content = "platno";
                            lbl.HorizontalContentAlignment = HorizontalAlignment.Center;
                            lbl.Background = Brushes.DarkGray;
                            lbl.Foreground = Brushes.White;
                            Grid.SetColumnSpan(lbl, columns + 1);
                            myGrid.Children.Add(lbl);
                        }
                        // exit
                        else if (i == columns + 1)
                        {
                            Button btn = new Button();
                            btn.Content = "X";
                            btn.HorizontalAlignment = HorizontalAlignment.Stretch;
                            btn.VerticalAlignment = VerticalAlignment.Stretch;
                            btn.Click += crossClick;
                            btn.BorderThickness = new Thickness(0);
                            btn.Background = Brushes.Red;
                            Grid.SetRow(btn, j);
                            Grid.SetColumn(btn, i);
                            myGrid.Children.Add(btn);
                        }
                    }
                    // rows label
                    else if (i == 0 || i == columns + 1)
                    {
                        Label lbl = new Label();
                        lbl.Content = (j).ToString();
                        lbl.Background = Brushes.Orange;
                        lbl.Foreground = Brushes.White;
                        Grid.SetRow(lbl, j);
                        Grid.SetColumn(lbl, i);
                        myGrid.Children.Add(lbl);
                    }
                    // seats buttons
                    else
                    {
                        Button btn = new Button();
                        btn.Content = ($"{j} - {i}").ToString();
                        btn.HorizontalAlignment = HorizontalAlignment.Stretch;
                        btn.VerticalAlignment = VerticalAlignment.Stretch;
                        btn.Click += seatClick;
                        Grid.SetRow(btn, j);
                        Grid.SetColumn(btn, i);
                        myGrid.Children.Add(btn);

                        column.Add((btn, null));
                    }
                }
                if (i != 0 && i != columns + 1) { buttons.Add(column); }                
            }
            subButtons = buttons;
            reservations = GetRecords();
            udelej();
            
            return myGrid;

        }

        void udelej()
        {
            for (int i = 0; i < reservations.Count; i++)
            {
                if (reservations[i].uuid == uuidValue)
                {
                    Button button = buttons[reservations[i].column - 1][reservations[i].row - 1].Item1;
                    buttons[reservations[i].column - 1][reservations[i].row - 1] = (button, reservations[i]);
                    switch (reservations[i].state)
                    {
                        case "Reserved":
                            button.Background = reserved;
                            break;
                        case "Sold":
                            button.Background = sold;
                            break;
                        case "Unavailable":
                            button.Background = unavailable;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        // save json data into List of Projections "projections"
        void ReadFile()
        {
            if (File.Exists(jsonpath))
            {
                string real = File.ReadAllText(jsonpath);
                List<Projection> jsonData = JsonConvert.DeserializeObject<List<Projection>>(real);
                projections = jsonData;                
            }
        }

        // seat onclick
        void seatClick(object sender, RoutedEventArgs e)
        {
            Button seatButton = (Button)sender;
            //seatButton.Background = selected;
            sub = seatButton;

            List<string> rowcolumn = seatButton.Content.ToString().Split('-').ToList();

            rowValue = int.Parse(rowcolumn[0]);
            columnValue = int.Parse(rowcolumn[1]);

            // create grid in new window
            Window popup = new Window();
            Grid seatOptions = new Grid();
            
            for (int i = 0; i < 4; i++)
            {
                seatOptions.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < 3; i++)
            {
                seatOptions.RowDefinitions.Add(new RowDefinition());
            }

            // reserve button
            Button Reserved = new Button();
            Reserved.Content = "Reserved";
            Reserved.Click += state;
            Grid.SetRow(Reserved, 2);
            Grid.SetColumn(Reserved, 0);

            // sold button
            Button Sold = new Button();
            Sold.Content = "Sold";
            Sold.Click += state;
            Grid.SetRow(Sold, 2);
            Grid.SetColumn(Sold, 1);

            // unavailable button
            Button Unavailable = new Button();
            Unavailable.Content = "Unavailable";
            Unavailable.Click += state;
            Grid.SetRow(Unavailable, 2);
            Grid.SetColumn(Unavailable, 2);

            // unavailable button
            Button Vacant = new Button();
            Vacant.Content = "Vacant";
            Vacant.Click += state;
            Grid.SetRow(Vacant, 2);
            Grid.SetColumn(Vacant, 3);

            // reservation form textboxes
            // name textbox
            TextBox nameBox = new TextBox();
            nameBox.Text = "jméno";
            nameBox.Name = "NameBox";
            nameBox.FontStretch = new FontStretch();
            nameBox.LayoutTransform = new ScaleTransform();
            Grid.SetRow(nameBox, 0);
            Grid.SetColumn(nameBox, 0);
            Grid.SetColumnSpan(nameBox, 3);
            nameBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            nameBox.VerticalAlignment = VerticalAlignment.Stretch;
            // email textbox 
            TextBox emailBox = new TextBox();
            emailBox.Text = "email";
            emailBox.Name = "EmailBox";
            Grid.SetRow(emailBox, 1);
            Grid.SetColumn(emailBox, 0);
            Grid.SetColumnSpan(emailBox, 3);
            emailBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            emailBox.VerticalAlignment = VerticalAlignment.Stretch;

            // add textboxes to grid
            seatOptions.Children.Add(nameBox);
            seatOptions.Children.Add(emailBox);
            nameBox.Visibility = Visibility.Hidden;
            emailBox.Visibility = Visibility.Collapsed;
            subNameBox = nameBox;
            subEmailBox = emailBox;

            // add buttons to grid
            seatOptions.Children.Add(Reserved);
            seatOptions.Children.Add(Sold);
            seatOptions.Children.Add(Unavailable);
            seatOptions.Children.Add(Vacant);
            subGrid = seatOptions;

            // set new window
            popup.Content = seatOptions;
            popup.Width = 400;
            popup.Height = 400;
            popup.ResizeMode = ResizeMode.NoResize;

            // show new window
            popup.ShowDialog();
        }
        
        // cinema exit
        void crossClick(object sender, RoutedEventArgs e)
        {
            Content = listvju();
            Title = "Seznam Projekcí";
        }

        // state button onclick
        void state(object sender, RoutedEventArgs e)
        {
            Button seatOptionButton = (Button)sender;
            string btnContent = seatOptionButton.Content.ToString();
            stateValue = btnContent;
            // reserve button click
            if (btnContent == "Reserved")
            {
                subNameBox.Visibility = Visibility.Visible;
                subEmailBox.Visibility = Visibility.Visible;
                reservations = GetRecords();
                if (subNameBox.Text != "jméno" & subEmailBox.Text != "email")
                {
                    if (reservations.Count > 0)
                    {
                        for (int i = 0; i < reservations.Count; i++)
                        {
                            (Button, seat) tvojeMama = buttons[reservations[i].column - 1][reservations[i].row - 1];
                            if (tvojeMama.Item1.Content == sub.Content)
                            {
                                stateValue = btnContent;
                                UpdateRecord(tvojeMama.Item2.id);
                            }
                            else { InsertRecord(); break; }
                        }
                    }
                    else { InsertRecord(); }
                    sub.Background = reserved;
                }
            }
            // sold button click
            else if (btnContent == "Sold")
            {
                subNameBox.Visibility = Visibility.Hidden;
                subEmailBox.Visibility = Visibility.Hidden;
                reservations = GetRecords(); udelej();
                if (reservations.Count > 0)
                {
                    for (int i = 0; i < reservations.Count; i++)
                    {
                        (Button, seat) tvojeMama = buttons[reservations[i].column - 1][reservations[i].row - 1];
                        if (tvojeMama.Item1.Content == sub.Content)
                        {
                            stateValue = btnContent;
                            UpdateRecord(tvojeMama.Item2.id);
                        }
                        else { InsertRecord(); break; }
                    }
                } else { InsertRecord(); }
                               
                sub.Background = sold;
            }
            // unavaible button click
            else if (btnContent == "Unavailable")
            {
                subNameBox.Visibility = Visibility.Hidden;
                subEmailBox.Visibility = Visibility.Hidden;
                reservations = GetRecords(); udelej();
                /*if (reservations.Count > 0)
                {
                    for (int i = 0; i < reservations.Count; i++)
                    {
                        (Button, seat) tvojeMama = buttons[reservations[i].column - 1][reservations[i].row - 1];
                        if (tvojeMama.Item1.Content == sub.Content)
                        {
                            stateValue = btnContent;
                            UpdateRecord(tvojeMama.Item2.id);
                        }
                        else { InsertRecord(); break; }
                    }
                }
                else { InsertRecord(); }*/

                sub.Background = unavailable;
            } 
            else if (btnContent == "Vacant")
            {
                subNameBox.Visibility = Visibility.Hidden;
                subEmailBox.Visibility = Visibility.Hidden;
                reservations = GetRecords(); udelej();
                for (int i = 0; i < reservations.Count; i++)
                {
                    (Button, seat) tvojeMama = buttons[reservations[i].column - 1][reservations[i].row - 1];
                    if (tvojeMama.Item1.Content == sub.Content)
                    {
                        stateValue = btnContent;
                        DeleteRecord(tvojeMama.Item2.id.ToString());
                    }
                }
                sub.Background = unoccupied;
            }
        }

        // define new table seats
        [Table("seats")]
        public class seat
        {
            [PrimaryKey]
            [Column("id")]
            public Guid id { get; set; }
            
            [Column("row")]
            public int row { get; set; }
            [Column("column")]
            public int column { get; set; }

            [Column("uuid")]
            public string uuid { get; set; }

            [Column("cinemaName")]
            public string cinemaName { get; set; }
            [Column("movieName")]
            public string movieName { get; set; }

            [Column("state")]
            public string state { get; set; }
        }

    }
}
