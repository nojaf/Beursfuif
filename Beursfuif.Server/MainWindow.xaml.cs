using Beursfuif.Server.UserControls;
using Beursfuif.Server.ViewModel;
using System;
using System.Collections.Generic;
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

namespace Beursfuif.Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window , IStateChange
    {
        public MainWindow()
        {
            InitializeComponent();
            ((MainViewModel)this.DataContext).SetStateChanger(this);
        }

        private void MenuItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UCMenuItem newItem = sender as UCMenuItem;
            if (newItem != null)
            {
                foreach (UCMenuItem item in menuContainer.Children.OfType<UCMenuItem>().Where(x => x.IsActive))
                {
                    if (item != newItem) item.GoToNonActive();
                }
            }
        }

        public void GoToState(string name, bool transition = true, string who = null)
        {
            FrameworkElement item = gdMain.FindName(who) as FrameworkElement;
            VisualStateManager.GoToState(item, name, transition);
        }
    }
}
