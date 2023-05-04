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
using SQLite;
using System.Windows.Media.Media3D;


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

        private string jsonpath = "filmy.json";

        private Button sub;
        private Grid subGrid;

        SQLiteConnection conn;

        private List<Projection> projections;

        TextBox subNameBox;
        TextBox subEmailBox;

        private static List<string> jsonlist = new List<string>();
        //private string jsonText; 

        private int rows = 7;
        private int columns = 22;

        private Brush unavailable = Brushes.DarkGray;
        private Brush sold = Brushes.Crimson;
        private Brush selected = Brushes.LightBlue;
        private Brush reserved = Brushes.DarkOrange;
        Brush unoccupied = Brushes.LightGray;


        public MainWindow()
        {

            InitializeComponent();
            Title = "Seznam Projekcí";
            ReadFile();
            Content = listvju();//CreateGrid();

        }

        SQLiteConnection _connection;
        string DataSource;

        public void InitialiseConnection()
        {
            DataSource = "seats.db";
            
            SQLiteConnectionString options = new SQLiteConnectionString(DataSource, false);

            _connection = new SQLiteConnection(options);
        }
        public void GetRecords()
        {
            var options = new SQLiteConnectionString(DataSource, false);
            var conn = new SQLiteConnection(options);

            string query = $"SELECT * FROM seats";

            var results = conn.Query<seat>(query);

            conn.Close();
        }
        public void InsertRecord()
        {
            InitialiseConnection();
            SQLiteConnectionString options = new SQLiteConnectionString(DataSource, false);
            SQLiteConnection conn = new SQLiteConnection(options);

            string query = $"INSERT INTO seats VALUES ('{new Guid()}', '{rowValue}', '{columnValue}', '{uuidValue}', '{cinemaNameValue}', '{movieNameValue}', '{stateValue}')";

            var results = conn.Query<seat>(query);

            conn.Close();
        }
        public void UpdateRecord(Guid id)
        {
            var options = new SQLiteConnectionString(DataSource, false);
            var conn = new SQLiteConnection(options);

            var record = new seat { id = id, row = rowValue, column = columnValue, uuid = uuidValue, cinemaName = cinemaNameValue, movieName = movieNameValue, state = stateValue };

            var results = conn.Update(record);

            conn.Close();
        }

        public void DeleteRecord(string id)
        {
            var options = new SQLiteConnectionString(DataSource, false);
            var conn = new SQLiteConnection(options);

            conn.Delete<seat>(id);
            conn.Close();
        }

        /*private void dbInsert(string stateValue)
        {

            string databasePath = "seats.db";
            conn = new SQLiteConnection(databasePath);
            SQLiteCommand sqlite_cmd = conn.CreateCommand("SELECT * FROM seats");


            string database_connection = "Data Source=seats.db;Version=3;";
            SQLiteConnection connection = new SQLiteConnection(database_connection);
            

            conn.Query<seat>($"INSERT INTO seats VALUES ('{new Guid()}', '{rowValue}', '{columnValue}', '{uuidValue}', '{cinemaNameValue}', '{movieNameValue}', '{stateValue}')");
            var record = new Record { id = new Guid(), row = rowValue, column = columnValue, uuid = uuidValue, cinemaName = cinemaNameValue, movieName = movieNameValue, state = stateValue };
            var results = conn.Insert(record);

        }*/


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
        private void listView_DoubleClick(object sender, MouseEventArgs e)
        {
            ListView x = (ListView)sender;
            var i = x.SelectedIndex;

            rows = projections[i].cinema.rows;
            columns = projections[i].cinema.columns;
            Title = projections[i].cinema.name;
            Content = CreateGrid();
            cinemaNameValue = projections[i].cinema.name;
            movieNameValue = projections[i].name;
            uuidValue = projections[i].uuid;
        }
        Grid CreateGrid()
        {
            Grid myGrid = new Grid();
            
            for (int i = 0; i < columns +2; i++)
            {
                myGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < rows + 1; i++)
            {
                myGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < columns + 2; i++)
            {
                for (int j = 0; j < rows + 1; j++)
                {
                    if (j == 0)
                    {
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
                        } else if (i == columns + 1)
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
                    else if (i == 0 || i == columns + 1)
                    {
                        Label lbl = new Label();
                        lbl.Content = (j).ToString();
                        lbl.Background = Brushes.Orange;
                        lbl.Foreground = Brushes.White;
                        Grid.SetRow(lbl,j);
                        Grid.SetColumn(lbl,i);
                        myGrid.Children.Add(lbl);
                    }
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
                    }                    
                }
            }
            return myGrid;
        }

        void ReadFile()
        {
            if (File.Exists(jsonpath))
            {
                string real = File.ReadAllText(jsonpath);
                List<Projection> jsonData = JsonConvert.DeserializeObject<List<Projection>>(real);
                projections = jsonData;                
            }
        }

        void seatClick(object sender, RoutedEventArgs e)
        {
            Button seatButton = (Button)sender;
            sub = seatButton;

            List<string> rowcolumn = seatButton.Content.ToString().Split('-').ToList();

            rowValue = int.Parse(rowcolumn[0]);
            columnValue = int.Parse(rowcolumn[1]);

            Window popup = new Window();
            Grid seatOptions = new Grid();
            
            for (int i = 0; i < 3; i++)
            {
                seatOptions.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < 3; i++)
            {
                seatOptions.RowDefinitions.Add(new RowDefinition());
            }
            Button Reserved = new Button();
            Reserved.Content = "Reserved";
            Reserved.Click += state;
            Grid.SetRow(Reserved, 2);
            Grid.SetColumn(Reserved, 0);

            Button Sold = new Button();
            Sold.Content = "Sold";
            Sold.Click += state;
            Grid.SetRow(Sold, 2);
            Grid.SetColumn(Sold, 1);

            Button Unavailable = new Button();
            Unavailable.Content = "Unavailable";
            Unavailable.Click += state;
            Grid.SetRow(Unavailable, 2);
            Grid.SetColumn(Unavailable, 2);

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
            TextBox emailBox = new TextBox();
            emailBox.Text = "email";
            emailBox.Name = "EmailBox";
            Grid.SetRow(emailBox, 1);
            Grid.SetColumn(emailBox, 0);
            Grid.SetColumnSpan(emailBox, 3);
            emailBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            emailBox.VerticalAlignment = VerticalAlignment.Stretch;

            seatOptions.Children.Add(nameBox);
            seatOptions.Children.Add(emailBox);
            nameBox.Visibility = Visibility.Hidden;
            emailBox.Visibility = Visibility.Collapsed;
            subNameBox = nameBox;
            subEmailBox = emailBox;

            seatOptions.Children.Add(Reserved);
            seatOptions.Children.Add(Sold);
            seatOptions.Children.Add(Unavailable);
            subGrid = seatOptions;

            popup.Content = seatOptions;

            popup.Width = 400;
            popup.Height = 400;

            popup.ShowDialog();
        }

        void crossClick(object sender, RoutedEventArgs e)
        {
            Content = listvju();
            Title = "Seznam Projekcí";
        }

        void state(object sender, RoutedEventArgs e)
        {

            Button seatOptionButton = (Button)sender;
            string btnContent = seatOptionButton.Content.ToString();
            stateValue = btnContent;
            if (btnContent == "Reserved")
            {
                subNameBox.Visibility = Visibility.Visible;
                subEmailBox.Visibility = Visibility.Visible;
                if (subNameBox.Text != "jméno" & subEmailBox.Text != "email")
                {
                    InsertRecord();
                    sub.Background = reserved;
                }
            } else if (btnContent == "Sold")
            {
                subNameBox.Visibility = Visibility.Hidden;
                subEmailBox.Visibility = Visibility.Hidden;
                InsertRecord();
                sub.Background = sold;
            } else if (btnContent == "Unavailable")
            {
                subNameBox.Visibility = Visibility.Hidden;
                subEmailBox.Visibility = Visibility.Hidden;
                InsertRecord();
                sub.Background = unavailable;
            }
        }

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
