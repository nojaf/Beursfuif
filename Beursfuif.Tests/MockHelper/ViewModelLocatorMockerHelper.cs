using Beursfuif.Server.ViewModel;
using Moq;

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
