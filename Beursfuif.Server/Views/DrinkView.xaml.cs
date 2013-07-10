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

namespace Beursfuif.Server.Views
{
    /// <summary>
    /// Interaction logic for DrinkView.xaml
    /// </summary>
    public partial class DrinkView : UserControl, IStateChange
    {
        public DrinkView()
        {
            InitializeComponent();
            (this.DataContext as DrinkViewModel).SetStateChanger(this);
        }

        public void GoToState(string name, bool transition = true)
        {
            VisualStateManager.GoToState(this, name, transition);
        }
    }
}
