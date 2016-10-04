using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Beursfuif.Server
{
	/// <summary>
	/// Interaction logic for Toast.xaml
	/// </summary>
	public partial class Toast : UserControl
	{
		public Toast()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
            //var workingArea = System.Windows.SystemParameters.WorkArea.TopRight;
            //var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
            this.SetValue(Grid.ColumnProperty, 2);
            this.SetValue(Grid.RowProperty, 0);
            this.SetValue(Grid.RowSpanProperty, 2);
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                //Grid.Column="2" Grid.Row="1"
		}

        public void SetValues(string title, string message)
        {
            this.title.Text = title;
            this.message.Text = message;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as Grid).Children.Remove(this);
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Grid parent = this.Parent as Grid;
                if(parent != null  && parent.Children.Contains(this))
                {
                    parent.Children.Remove(this);
                }
            }));
        }

        private void LayoutRoot_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (this.Parent as Grid).Children.Remove(this);
        }
	}
}