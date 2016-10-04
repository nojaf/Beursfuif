using Beursfuif.Server.Messages;
using System;
using System.Windows;
using System.Windows.Controls;

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
                (this.DataContext as DialogMessage).Answer = Convert.ToBoolean(btn.Tag);
                VisualStateManager.GoToState(this, "FadeOut", true);
            }
        }
	}
}