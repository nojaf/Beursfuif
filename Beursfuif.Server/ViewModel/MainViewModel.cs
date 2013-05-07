using Beursfuif.Server.UserControls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;

namespace Beursfuif.Server.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public RelayCommand<string> MenuClicked
        {
            get;
            set;
        }
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            MenuClicked = new RelayCommand<string>(MenuClickAction);
        }

        private void MenuClickAction(string menuItem)
        {
            Console.WriteLine(menuItem);
        }
    }
}