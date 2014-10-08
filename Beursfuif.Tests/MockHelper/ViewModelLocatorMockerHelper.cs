using Beursfuif.Server.DataAccess;
using Beursfuif.Server.ViewModel;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beursfuif.Tests.MockHelper
{
    public static class ViewModelLocatorMockerHelper
    {
        public static Mock<ViewModelLocator> GetMock()
        {
            Mock<ViewModelLocator> locatorMock = new Mock<ViewModelLocator>();
            return locatorMock;
        }
    }
}
