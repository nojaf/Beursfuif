using Beursfuif.Server.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Beursfuif.Server.Views
{
    /// <summary>
    /// Interaction logic for OrdersView.xaml
    /// </summary>
    public partial class OrdersView : UserControl,IStateChange
    {
        public OrdersView()
        {
            InitializeComponent();
            (this.DataContext as BeursfuifViewModelBase).SetStateChanger(this);
        }

        public void GoToState(string name, bool transition = true, string who = null)
        {
            VisualStateManager.GoToState(this, name, transition);
            if (name == "FadeOut")
            {
                this.IsHitTestVisible = false;
            }
            else this.IsHitTestVisible = true;
        }
    }
}
