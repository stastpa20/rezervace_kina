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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace rezervace_kina
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string jsonpath = "filmy.json";

        private Button sub;

        private List<Projection> projections;

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
            ReadFile();
            Content = listvju();//CreateGrid();

        }

        ListView listvju()
        {


            ListView listView = new ListView();
            for (int i = 0; i < projections.Count; i++)
            {
                listView.Items.Add($"Kino: {projections[i].cinema.name} Film: {projections[i].name}");
            }
            return listView;
        }
        Grid CreateGrid()
        {
            Grid myGrid = new Grid();

            
            //myGrid.HorizontalAlignment = HorizontalAlignment.Left;
            //myGrid.VerticalAlignment = VerticalAlignment.Top;
            
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
                        Label lbl = new Label();
                        lbl.Name = "platno";
                        lbl.Content = "platno";
                        lbl.HorizontalContentAlignment = HorizontalAlignment.Center;
                        lbl.Background = Brushes.DarkGray;
                        lbl.Foreground = Brushes.White;
                        Grid.SetColumnSpan(lbl, columns + 2);
                        myGrid.Children.Add(lbl);
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
                rows = jsonData[0].cinema.rows;
                columns = jsonData[0].cinema.columns;
                this.Title = jsonData[0].cinema.name;
            }
            else
            {
            }
        }

        void seatClick(object sender, RoutedEventArgs e)
        {
            Button seatButton = (Button)sender;
            sub = seatButton;

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

            seatOptions.Children.Add(Reserved);
            seatOptions.Children.Add(Sold);
            seatOptions.Children.Add(Unavailable);

            popup.Content = seatOptions;

            popup.Width = 400;
            popup.Height = 400;

            popup.ShowDialog();
        }

        void state(object sender, RoutedEventArgs e)
        {
            Button seatOptionButton = (Button)sender;
            string btnContent = seatOptionButton.Content.ToString();
            if (btnContent == "Reserved")
            {
                sub.Background = reserved;
            } else if (btnContent == "Sold")
            {
                sub.Background = sold;
            } else if (btnContent == "Unavailable")
            {
                sub.Background = unavailable;
            }
        }
        
    }
}
