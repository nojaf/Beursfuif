using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.BL
{
    //package between client and server
    public class Package
    {
        public int MessageId { get; set; }

        public ClientInterval CurrentInterval { get; set; }

        public DateTime CurrentBeursfuifTime { get; set; }

        public ClientOrder NewOrder { get; set; }

        public string AuthenticationCode { get; set; }

        public int ClientId { get; set; }

        public string ClientName { get; set; }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
