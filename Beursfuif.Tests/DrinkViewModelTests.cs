using Beursfuif.BL;
using Beursfuif.Server.ViewModel;
using Beursfuif.Tests.MockFactory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq.Protected;
using Beursfuif.Tests.MockHelper;

namespace Beursfuif.Tests
{
    [TestClass]
    public class DrinkViewModelTests
    {
        private BeursfuifMockFactory _mockFactory = new BeursfuifMockFactory();

        [TestMethod]
        public void ChangeDrinkAvailable()
        {
            //Arrange
            var ioManagerMock = ViewModelMockHelper.IOMangerMock();

          
            //Act


            //Assert
            
        }
    }
}
