using Beursfuif.BL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
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
        public UCStats()
        {
            InitializeComponent();
            this.DataContext = this;
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
                    showOrder.Orders[j-1] = new ClientDrinkOrder()
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
            set { 
                SetValue(DrinksProperty, value);

                titleGrid.Children.Clear();
                int length = value.Length;
                for (int i = 0; i < length; i++)
                {
                    titleGrid.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width  =  new GridLength(1, GridUnitType.Star)
                    });

                    TextBlock text = new TextBlock() { Text = value[i].Name, TextAlignment = TextAlignment.Center };
                    text.SetValue(Grid.ColumnProperty, i);
                    titleGrid.Children.Add(text);
                }
                CreateGraph();
            }
        }

        private void CreateGraph()
        {
            int total = AllOrders.Sum(x => x.Orders.Sum(y => y.Count));
            int length = Drinks.Length;

            double[] percentages = new double[length];

            for (int i = 0; i < length; i++)
			{
                int drinkId = Drinks[i].Id;
                var ordersFromInterval = AllOrders.Where(x => x.IntervalId == IntervalId);
                var query = (from order in ordersFromInterval
                             where order.Orders.Any(x => x.DrinkId == i)
                             select order.Orders.FirstOrDefault(x => x.DrinkId == i)).ToArray();

                int sum = 0;
                int sCount = query.Length;
                for (int s = 0; s < sCount; s++)
			    {
			        sum +=    query[s].Count;
			    }

                percentages[i] = (double) sum / (double)total;
               
			}

            Console.WriteLine(percentages);
     
        }

        // Using a DependencyProperty as the backing store for Drinks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrinksProperty =
            DependencyProperty.Register("Drinks", typeof(ReducedDrink[]), typeof(UCStats), new PropertyMetadata(null));




    }
}
