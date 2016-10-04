using Beursfuif.Server.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Beursfuif.Server.Views
{
    /// <summary>
    /// Interaction logic for PredictionView.xaml
    /// </summary>
    public partial class PredictionView : UserControl, IStateChange
    {
        public PredictionView()
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

        private void DoubleUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            (this.DataContext as PredictionViewModel).IsDirty = true;
        }
    }
}
