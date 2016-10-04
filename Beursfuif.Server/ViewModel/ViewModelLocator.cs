/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Beursfuif.Server"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using Beursfuif.Server.DataAccess;
using Beursfuif.Server.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace Beursfuif.Server.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            //Blend might try to instantiate the ViewModels more then once
            if (ViewModelBase.IsInDesignModeStatic)
            {
                if (!SimpleIoc.Default.IsRegistered<IIOManager>())
                {
                    SimpleIoc.Default.Register<IIOManager>(() => new IOManager());
                }
                if (!SimpleIoc.Default.IsRegistered<IBeursfuifData>())
                {
                    SimpleIoc.Default.Register<IBeursfuifData>(() => new BeursfuifData(IOManager));
                }
                if (!SimpleIoc.Default.IsRegistered<IBeursfuifServer>())
                {
                    SimpleIoc.Default.Register<IBeursfuifServer>(() => new BeursfuifServer(BeursfuifData));
                }

                BasicDesignTimeRegistration<MainViewModel>();
                BasicDesignTimeRegistration<DrinkViewModel>();
                BasicDesignTimeRegistration<IntervalViewModel>();
                BasicDesignTimeRegistration<SettingsViewModel>();
                BasicDesignTimeRegistration<ClientsViewModel>();
                BasicDesignTimeRegistration<OrdersViewModel>();
                BasicDesignTimeRegistration<LogViewModel>();
                BasicDesignTimeRegistration<PredictionViewModel>();
            }
            else
            {
                SimpleIoc.Default.Register<IIOManager>(() => new IOManager());
                SimpleIoc.Default.Register<IBeursfuifData>(() => new BeursfuifData(IOManager),true);
                SimpleIoc.Default.Register<IBeursfuifServer>(() => new BeursfuifServer(BeursfuifData));
                SimpleIoc.Default.Register<MainViewModel>();
                SimpleIoc.Default.Register<DrinkViewModel>();
                SimpleIoc.Default.Register<IntervalViewModel>();
                SimpleIoc.Default.Register<SettingsViewModel>();
                SimpleIoc.Default.Register<ClientsViewModel>();
                SimpleIoc.Default.Register<OrdersViewModel>();
                SimpleIoc.Default.Register<LogViewModel>();
                SimpleIoc.Default.Register<PredictionViewModel>();
            }
        }

        private void BasicDesignTimeRegistration<T>() where T: class
        {
            if (!SimpleIoc.Default.IsRegistered<T>())
            {
                SimpleIoc.Default.Register<T>();
            }
        }

        public IIOManager IOManager => ServiceLocator.Current.GetInstance<IIOManager>();

        public IBeursfuifData BeursfuifData => ServiceLocator.Current.GetInstance<IBeursfuifData>();

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public DrinkViewModel Drink => ServiceLocator.Current.GetInstance<DrinkViewModel>();

        public IntervalViewModel Interval => ServiceLocator.Current.GetInstance<IntervalViewModel>();

        public SettingsViewModel Settings => ServiceLocator.Current.GetInstance<SettingsViewModel>();

        public ClientsViewModel Clients => ServiceLocator.Current.GetInstance<ClientsViewModel>();

        public OrdersViewModel Orders => ServiceLocator.Current.GetInstance<OrdersViewModel>();

        public LogViewModel Log => ServiceLocator.Current.GetInstance<LogViewModel>();

        public PredictionViewModel Prediction => ServiceLocator.Current.GetInstance<PredictionViewModel>();

        public static void Cleanup()
        {
            
        }
    }
}