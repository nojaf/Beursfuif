using Beursfuif.Server.Messages;
using System;
using System.Collections.Generic;
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
	/// Interaction logic for MessageGridControl.xaml
	/// </summary>
	public partial class MessageGridControl : UserControl
	{
		public MessageGridControl()
		{
			this.InitializeComponent();
            VisualStateManager.GoToState(this, "Default", false);
		}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                (this.DataContext as ErrorMessage).Answer = Convert.ToBoolean(btn.Tag);
                VisualStateManager.GoToState(this, "FadeOut", true);
            }
        }
	}
}