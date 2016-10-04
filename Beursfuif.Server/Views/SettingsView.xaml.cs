using System.Diagnostics;
using Beursfuif.Server.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Beursfuif.Server.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl,IStateChange
    {
        public SettingsView()
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

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
