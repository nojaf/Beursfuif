using Beursfuif.BL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Beursfuif.Server.Entity;

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

        #region DPs
        public int IntervalId
        {
            get { return (int)GetValue(IntervalIdProperty); }
            set
            {
                SetValue(IntervalIdProperty, value);
            }
        }
        // Using a DependencyProperty as the backing store for IntervalId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IntervalIdProperty =
            DependencyProperty.Register("IntervalId", typeof(int), typeof(UCStats), new PropertyMetadata(0, IntervalChanged));

        private static void IntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UCStats source = d as UCStats;
            if (source != null){
            
                source.CheckForCreation();
            }
        }


        public ObservableCollection<ShowOrder> AllOrders
        {
            get { return (ObservableCollection<ShowOrder>)GetValue(AllOrdersProperty); }
            set
            {
                SetValue(AllOrdersProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for AllOrders.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllOrdersProperty =
            DependencyProperty.Register("AllOrders", typeof(ObservableCollection<ShowOrder>), typeof(UCStats), new PropertyMetadata(null, AllOrdersChanged));

        private static void AllOrdersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UCStats source = d as UCStats;
            if (source != null)
            {
                source.CheckForCreation();

                var old = e.OldValue as ObservableCollection<ShowOrder>;

                if (old != null)
                    old.CollectionChanged -= source.OnAllOrdersCollectionChanged;

                var newValue = e.NewValue as ObservableCollection<ShowOrder>;

                if (newValue != null)
                    newValue.CollectionChanged += source.OnAllOrdersCollectionChanged;
            }
        }




        // Using a DependencyProperty as the backing store for Drinks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrinksProperty =
            DependencyProperty.Register("Drinks", typeof(ReducedDrink[]), typeof(UCStats), new PropertyMetadata(null, DrinksChanged));

        private static void DrinksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UCStats source = d as UCStats;
            if (source != null)
            {
                source.CheckForCreation();
            }
        }

        public ReducedDrink[] Drinks
        {
            get { return (ReducedDrink[])GetValue(DrinksProperty); }
            set
            {
                SetValue(DrinksProperty, value);
            }
        }

        #endregion

        public void CreateGraph(object state = null)
        {
            List<ClientDrinkOrder> allOrderItems = new List<ClientDrinkOrder>();

            if (AllOrders == null || Drinks == null) return;

            var query = from showOrder in AllOrders
                        select showOrder.Orders;

            foreach (var item in query)
            {
                allOrderItems.AddRange(item);
            }

            int total = (IntervalId != int.MaxValue ?
                allOrderItems.Where(x => x.IntervalId == IntervalId).Sum(x => x.Count) :
                  allOrderItems.Sum(x => x.Count));

            double[] percentages = new double[Drinks.Length];
            if (total != 0)
            {
                int length = Drinks.Length;
                for (int i = 0; i < length; i++)
                {
                    int sum;
                    if (IntervalId != int.MaxValue)
                    {
                        sum = allOrderItems.Where(x => x.IntervalId == IntervalId && x.DrinkId == Drinks[i].Id).Sum(x => x.Count);
                    }
                    else
                    {
                        sum = allOrderItems.Where(x => x.DrinkId == Drinks[i].Id).Sum(x => x.Count);
                    }

                    percentages[i] = (double)sum / (double)total;
                }
            }

            ShowGraph(percentages);
        }

        private void ShowGraph(double[] percentages)
        {
            int length = Drinks.Length;
        
            CreateTitles();

          
            CreateGraphUI(percentages, length);


            double maxHeight = graphGrid.ActualHeight;
            for (int j = 0; j < length; j++)
            {
                if (!double.IsNaN(percentages[j]))
                {
                    Grid target = graphGrid.Children[j] as Grid;

                    //update text
                    TextBlock text = target.Children[0] as TextBlock;
                    if (text != null)
                    {
                        text.Text = Math.Round(percentages[j] * 100, 2) + "%";
                    }



                    if (target == null) continue;
                    DoubleAnimation da = new DoubleAnimation()
                    {
                        From = target.ActualHeight,
                        To = percentages[j] * maxHeight,
                        Duration = new Duration(new TimeSpan(0, 0, 0, 1, 0)),
                        EasingFunction = new BounceEase()
                    };
                    target.BeginAnimation(Grid.HeightProperty, da);
                }
            }
        }

        private void CreateGraphUI(double[] percentages, int length)
        {
            graphGrid.Children.Clear();
            graphGrid.ColumnDefinitions.Clear();
            for (int i = 0; i < length; i++)
            {
                graphGrid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
                Grid grid = new Grid()
                {
                    Margin = new Thickness(5, 0, 5, 0),
                    Background = new SolidColorBrush(_colors[i % _colors.Length]),
                    Height = 0,
                    Name = "rect" + i,
                    VerticalAlignment = System.Windows.VerticalAlignment.Bottom,
                    Tag = Drinks[i].Id
                };

                TextBlock text = new TextBlock()
                {
                    Text = Math.Round(percentages[i] * 100, 2) + "%",
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    FontFamily = new FontFamily(new Uri("pack://application:/Styles/"), "Coolvetica Rg"),
                    Foreground = new SolidColorBrush(Colors.White),

                };

                grid.Children.Add(text);

                grid.SetValue(Grid.ColumnProperty, i);
                graphGrid.Children.Add(grid);

            }
        }


        private void OnAllOrdersCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CreateGraph();
        }

        public void CheckForCreation()
        {
            if (Drinks != null && AllOrders != null && IntervalId != 0 && !Drinks.Any(x => x == null))
            {
               CreateGraph();
            }
        }

        private void CreateTitles()
        {

            titleGrid.Children.Clear();
            titleGrid.ColumnDefinitions.Clear();

            int length = Drinks.Length;
            for (int i = 0; i < length; i++)
            {
                titleGrid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });

                Viewbox view = new Viewbox();
                TextBlock text = new TextBlock()
                {
                    Text = Drinks[i].Name,
                    TextAlignment = TextAlignment.Center,
                    FontFamily = new FontFamily(new Uri("pack://application:/Styles/"), "Coolvetica Rg"),
                    Margin = new Thickness(5)

                };
                view.SetValue(Grid.ColumnProperty, i);
                view.Child = text;
                titleGrid.Children.Add(view);
            }
        }
    }
}
