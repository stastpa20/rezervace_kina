using System;
using System.Collections.Generic;
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

namespace rezervace_kina
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int rows = 7;
        private int columns = 22;

        Grid CreateGrid()
        {
            Grid myGrid = new Grid();
            myGrid.Width = 700;
            myGrid.Height = 300;

            
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
                        btn.Content = i.ToString();
                        Grid.SetRow(btn, j);
                        Grid.SetColumn(btn, i);
                        myGrid.Children.Add(btn);
                    }
                    
                }
            }
            

            return myGrid;
        }
        public MainWindow()
        {
            
            InitializeComponent();
            Title = "Kino";
            Content = CreateGrid();

        }
    }
}
