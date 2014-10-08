using Beursfuif.BL;
using Beursfuif.Server.ViewModel;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq.Protected;
using Beursfuif.Server.DataAccess;

namespace Beursfuif.Tests.MockHelper
{
    public static class ViewModelMockHelper
    {
        public static void MockLogAndToast<T>(Mock<T> vmMock) where T:BeursfuifViewModelBase
        {
            vmMock.Setup(vm => vm.SendLogMessage(It.IsAny<string>(), It.IsAny<LogType>()))
                            .Callback<string,LogType>((message, type) => {
                                Console.Write("Mocking Log {0} | {1}", message, type);
                            });

            //SendToastMessage(string title, string message = null)
            vmMock.Protected()
                .Setup("SendToastMessage", Moq.Protected.ItExpr.IsAny<string>(), Moq.Protected.ItExpr.IsAny<string>())
                .Callback<string, string>((title, message) =>
                {
                    Console.WriteLine("Mocking Toastr {0} | {1}", title, message);
                });
        }

        //public static void MockLogAndToast(Mock<DrinkViewModel> drinkViewModelMock)
        //{
        //    MockLogAndToast(drinkViewModelMock);
        //}

        public static void MockViewModelLocator(Mock<BeursfuifViewModelBase> viewModelMock, ViewModelLocator locatorMockInstance)
        {
            viewModelMock.Protected()
                .Setup<ViewModelLocator>("GetLocator")
                .Returns(locatorMockInstance)
                .Verifiable();
        }

        public static Mock<IIOManager> IOMangerMock()
        {
            return new Mock<IIOManager>();
        }
    }
}
