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
<<<<<<< HEAD
        private string _menuText;

        public string MenuText
        {
            get { return _menuText; }
            set { _menuText = value;
            textBlock.Text = value;
            }
        }

        //public string MenuText { get; set; }
=======
        public string MenuText { get; set; }
>>>>>>> b467f20ffbb3200f18d4975e746f9fdbd327459f

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
<<<<<<< HEAD
=======
            this.DataContext = this;
>>>>>>> b467f20ffbb3200f18d4975e746f9fdbd327459f
        }

        private void UC_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsActive = !IsActive;
<<<<<<< HEAD
            if (!IsActive)
            {
                VisualStateManager.GoToState(this, "Active", true);
            }
=======
            if (IsActive)
            {
                VisualStateManager.GoToState(this, "Active", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "NonActive", true);
            }
>>>>>>> b467f20ffbb3200f18d4975e746f9fdbd327459f
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
<<<<<<< HEAD

        public void GoToNonActive()
        {
            VisualStateManager.GoToState(this, "NonActive", true);
            IsActive = false;
        }
=======
>>>>>>> b467f20ffbb3200f18d4975e746f9fdbd327459f
    }
}
