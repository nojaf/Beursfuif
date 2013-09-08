using Beursfuif.BL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Beursfuif.Server.UserControls
{
    /// <summary>
    /// Interaction logic for UCStats.xaml
    /// </summary>
    public partial class UCStats : UserControl
    {
        private Color[] _colors = new Color[]{
            (Color)ColorConverter.ConvertFromString("#B33C36"),
            (Color)ColorConverter.ConvertFromString("#FF8A31"),
            (Color)ColorConverter.ConvertFromString("#FFD681"),
            (Color)ColorConverter.ConvertFromString("#93CC5E"),
            (Color)ColorConverter.ConvertFromString("#4EB360"),

            (Color)ColorConverter.ConvertFromString("#2A2B2A"),
            (Color)ColorConverter.ConvertFromString("#4B4B4D"),
            (Color)ColorConverter.ConvertFromString("#D63431"),
            (Color)ColorConverter.ConvertFromString("#FDBE2B"),
            (Color)ColorConverter.ConvertFromString("#FDBE2B"),

            (Color)ColorConverter.ConvertFromString("#FDBE2B"),
            (Color)ColorConverter.ConvertFromString("#FEE169"),
            (Color)ColorConverter.ConvertFromString("#CDD452"),
            (Color)ColorConverter.ConvertFromString("#F9722E"),
            (Color)ColorConverter.ConvertFromString("#C9313D")
        };

        public UCStats()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void PopulateOrders()
        {
            Random rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                int orderLength = 5;
                ShowOrder showOrder = new ShowOrder()
                {
                    IntervalId = rand.Next(0, 5),
                    Orders = new ClientDrinkOrder[orderLength]
                };

                for (int j = 1; j < orderLength + 1; j++)
                {
                    showOrder.Orders[j - 1] = new ClientDrinkOrder()
                    {
                        Count = (byte)rand.Next(0, 10),
                        DrinkId = j,
                        IntervalId = showOrder.IntervalId
                    };
                }

                AllOrders.Add(showOrder);
            }
        }

        public int IntervalId
        {
            get { return (int)GetValue(IntervalIdProperty); }
            set { SetValue(IntervalIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IntervalId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IntervalIdProperty =
            DependencyProperty.Register("IntervalId", typeof(int), typeof(UCStats), new PropertyMetadata(0));


        public ObservableCollection<ShowOrder> AllOrders
        {
            get { return (ObservableCollection<ShowOrder>)GetValue(AllOrdersProperty); }
            set { SetValue(AllOrdersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllOrders.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllOrdersProperty =
            DependencyProperty.Register("AllOrders", typeof(ObservableCollection<ShowOrder>), typeof(UCStats), new PropertyMetadata(null));

        public ReducedDrink[] Drinks
        {
            get { return (ReducedDrink[])GetValue(DrinksProperty); }
            set
            {
                SetValue(DrinksProperty, value);

                titleGrid.Children.Clear();
                int length = value.Length;
                for (int i = 0; i < length; i++)
                {
                    titleGrid.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width = new GridLength(1, GridUnitType.Star)
                    });

                    TextBlock text = new TextBlock() { Text = value[i].Name, TextAlignment = TextAlignment.Center };
                    text.SetValue(Grid.ColumnProperty, i);
                    titleGrid.Children.Add(text);
                }
                CreateGraph();
            }
        }

        private void CreateGraph(object state = null)
        {
            List<ClientDrinkOrder> allOrderItems = new List<ClientDrinkOrder>();

            var query = from showOrder in AllOrders
                        select showOrder.Orders;

            foreach (var item in query)
            {
                allOrderItems.AddRange(item);
            }

            int total = allOrderItems.Where(x => x.IntervalId == IntervalId).Sum(x => x.Count);

            double[] percentages = new double[Drinks.Length];

            int length = Drinks.Length;
            for (int i = 0; i < length; i++)
            {
                percentages[i] = (double)allOrderItems.Where(x => x.IntervalId == IntervalId && x.DrinkId == i + 1).Sum(x => x.Count) / (double)total;
            }

            ShowGraph(percentages);
        }

        private void ShowGraph(double[] percentages)
        {
            int length = Drinks.Length;
            if (graphGrid.Children.Count == 0)
            {
                for (int i = 0; i < length; i++)
                {
                    graphGrid.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width  =  new GridLength(1, GridUnitType.Star)
                    });
                    Rectangle rect = new Rectangle()
                    {
                        Margin = new Thickness(5,0,5,0),
                        Fill = new SolidColorBrush(_colors[i % _colors.Length]),
                        Height = 0,
                        Name = "rect"+i,
                        VerticalAlignment = System.Windows.VerticalAlignment.Bottom,
                        Tag = Drinks[i].Id
                    };

                    rect.MouseUp += GraphColumn_MouseUp;

                    rect.SetValue(Grid.ColumnProperty, i);
                    graphGrid.Children.Add(rect);
                    
                }
            }

            double maxHeight = graphGrid.ActualHeight;
            for (int j = 0; j < length; j++)
            {
                Rectangle target = graphGrid.Children[j] as Rectangle;

               
                if(target == null) continue;
                DoubleAnimation da = new DoubleAnimation()
                {
                    From = target.ActualHeight,
                    To = percentages[j] * maxHeight,
                    Duration = new Duration(new TimeSpan(0,0,0,1,0)),
                    EasingFunction = new BounceEase()
                };
                target.BeginAnimation(Rectangle.HeightProperty, da);
            }
        }

        void GraphColumn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            if (rect == null) return;
            int id = (int)rect.Tag;

            ShowOrder show = new ShowOrder()
            {
                IntervalId = 1,
                Orders = new ClientDrinkOrder[]{
                    new ClientDrinkOrder(){
                       Count = 100,
                       IntervalId = 1,
                       DrinkId = id
                    }
                }
            };
            AllOrders.Add(show);
            CreateGraph();
        }

        // Using a DependencyProperty as the backing store for Drinks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrinksProperty =
            DependencyProperty.Register("Drinks", typeof(ReducedDrink[]), typeof(UCStats), new PropertyMetadata(null));

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            IntervalId = 1;
            AllOrders = new ObservableCollection<ShowOrder>();
            PopulateOrders();
            Drinks = new ReducedDrink[]{
                new ReducedDrink(){
                    Id = 1,
                    Name = "Cola",
            
                },
                new ReducedDrink(){
                    Id = 2,
                    Name = "Bier",
            
                },
                new ReducedDrink(){
                    Id = 3,
                    Name = "Ice Tea",
            
                },new ReducedDrink(){
                    Id = 4,
                    Name = "Hommel",
            
                },new ReducedDrink(){
                    Id = 5,
                    Name = "Kriek",
            
                }
            };
        }




    }
}
