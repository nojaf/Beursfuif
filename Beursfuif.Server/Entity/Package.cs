using Newtonsoft.Json;
using System;
using Beursfuif.Server.Entity;

namespace Beursfuif.BL
{
    //package between client and server
    public class Package
    {
        public ProtocolKind Kind { get; set; }

        public ClientInterval CurrentInterval { get; set; }

        public DateTime CurrentBeursfuifTime { get; set; }

        public ClientDrinkOrder[] NewOrder { get; set; }

        public string AuthenticationCode { get; set; }

        public string ClientContext { get; set; }

        public string Name { get; set; }

        public string Ip { get; set; }

        public int DrinkId { get; set; }
        public ClientDrink Drink { get; set; }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }

    
    }
}
