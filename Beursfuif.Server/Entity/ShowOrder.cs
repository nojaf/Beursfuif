using System;
using Beursfuif.Server.Entity;

namespace Beursfuif.BL
{
    [Serializable]
    public class ShowOrder
    {
        public DateTime Time { get; set; }
        public int IntervalId { get; set; }
        public string OrderContent { get; set; }
        public string ClientName { get; set; }
        public int TotalPrice { get; set; }
        public ClientDrinkOrder[] Orders { get; set; }
    }
}
