using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Beursfuif.Server.UserControls
{
    /// <summary>
    /// Interaction logic for UCMenuItem.xaml
    /// </summary>
    public partial class UCMenuItem : UserControl
    {

        private string _menuText;

        public string MenuText
        {
            get { return _menuText; }
            set { _menuText = value;
            textBlock.Text = value;
            }
        }


        private bool _isActive = false;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                if (value) VisualStateManager.GoToState(this, "Active", false);
            }
        }


        public UCMenuItem()
        {
            InitializeComponent();
            //this.DataContext = this;
        }

        private void UC_MouseDown(object sender, MouseButtonEventArgs e)
        {        
            VisualStateManager.GoToState(this, "Active", true);
            IsActive = true;
        }

        private void UC_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!IsActive)
            {
                VisualStateManager.GoToState(this, "MouseIn", true);
            }
        }

        private void UC_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsActive)
            {
                VisualStateManager.GoToState(this, "MouseOut", true);
            }
        }

        public void GoToNonActive()
        {
            VisualStateManager.GoToState(this, "NonActive", true);
            IsActive = false;
        }
    }
}
