using Beursfuif.BL;
using Beursfuif.Server.Entity;

namespace Beursfuif.Server.Messages
{
    public class DrinkModifiedMessage
    {
        public Drink Drink { get; set; }
        public bool Added { get; set; }
        public bool Changed { get; set; }
    }
}
