﻿using Beursfuif.Server.UserControls;
using Beursfuif.Server.ViewModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Beursfuif.Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window , IStateChange, IToastManager
    {
        public MainWindow()
        {
            InitializeComponent();
            ((MainViewModel)this.DataContext).SetStateChanger(this);
            ((MainViewModel)this.DataContext).SetToastManager(this);
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
            Action changeState = delegate(){
                FrameworkElement item = gdMain.FindName(who) as FrameworkElement;
                VisualStateManager.GoToState(item, name, transition);
            };

            Dispatcher.BeginInvoke(changeState);
        }

        public void RemoveToast(Toast toast)
        {
            gdMain.Children.Remove(toast);
        }

        public void ShowToast(string title, string message = "")
        {
            Dispatcher.BeginInvoke(new Action(() => {
                Toast toast = new Toast();
                toast.SetValues(title, message);
                gdMain.Children.Add(toast);
            }));
        }
    }
}
