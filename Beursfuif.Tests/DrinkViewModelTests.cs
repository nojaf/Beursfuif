using Beursfuif.BL;
using Beursfuif.Server.Entity;
using Beursfuif.Server.Services;
using Beursfuif.Server.ViewModel;
using Beursfuif.Tests.MockFactory;
using Moq;
using Xunit;

namespace Beursfuif.Tests
{
    public class DrinkViewModelTests
    {
        private BeursfuifMockFactory _mockFactory = new BeursfuifMockFactory();
        private const double BigDecrease = 0.5;
        private const double BigIncrease = 1.5;
        private const double SmallIncrease = 1.25;
        private const double SmallDecrease = 0.75;

        private Drink GenerateDefaultDrink()
        {
            return new Drink
            {
                Available = true,
                BigDecrease = BigDecrease,
                BigRise = BigIncrease,
                SmallDecrease = SmallDecrease,
                SmallRise = SmallIncrease,
                CurrentPrice = 15,
                InitialPrice = 15
            };
        }


        [Fact]
        public void ChangeDrinkAvailable()
        {
            //Arrange
            var data = new Mock<IBeursfuifData>();
            var viewmodel = new DrinkViewModel(data.Object);

          
            //Act
            viewmodel.NewEditDrink = GenerateDefaultDrink();
            viewmodel.NewEditDrink.Name = "New Drink";

            //viewmodel.SaveDrink();


            //Assert

        }
    }
}
