using Beursfuif.Server.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Beursfuif.Server.Views
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView : UserControl, IStateChange
    {
        public LogView()
        {
            InitializeComponent();
            var beursfuifViewModelBase = this.DataContext as BeursfuifViewModelBase;
            beursfuifViewModelBase?.SetStateChanger(this);
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
