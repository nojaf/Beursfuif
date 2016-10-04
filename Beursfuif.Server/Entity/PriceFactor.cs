using System.ComponentModel;

namespace Beursfuif.Server.Entity
{
    public enum PriceFactor
    {
        [Description("Grote daling")]
        BigDecrease,
        [Description("Kleine daling")]
        SmallDecrease,
        [Description("Grote stijging")]
        BigRise,
        [Description("Kleine stijging")]
        SmallRise,
        [Description("Overschrijven")]
        Override
    }
}