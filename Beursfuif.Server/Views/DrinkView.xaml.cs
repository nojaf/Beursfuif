using Beursfuif.Server.ViewModel;
using System.Windows;
using System.Windows.Controls;

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
