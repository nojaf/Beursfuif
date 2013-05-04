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

namespace Beursfuif.Server.UserControls
{
    /// <summary>
    /// Interaction logic for UCMenuItem.xaml
    /// </summary>
    public partial class UCMenuItem : UserControl
    {
        public string MenuText { get; set; }

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
            this.DataContext = this;
        }

        private void UC_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsActive = !IsActive;
            if (IsActive)
            {
                VisualStateManager.GoToState(this, "Active", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "NonActive", true);
            }
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
    }
}
