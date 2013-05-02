using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.BL
{
    public class ClientOrder
    {
        #region properties
        public ClientDrinkOrder[] Items { get; set; }

        public int ClientId { get; set; }

        public string  AuthenticationCode { get; set; }
        #endregion

        public ClientOrder()
        {

        }

        public override bool Equals(object obj)
        {
            if (obj is ClientOrder)
            {
                ClientOrder other = obj as ClientOrder;
                if (other.Items == this.Items && other.AuthenticationCode == this.AuthenticationCode && this.ClientId == this.ClientId) return true;
            }
            return false;
        }

        public int TotalSumPrice(ClientDrink[] drinks)
        {
            return (from drink in drinks
                        join item in this.Items on drink.DrinkId equals item.DrinkId
                        where item.DrinkId == drink.DrinkId
                        select drink.Price * item.Count).Sum();
        }

    }
}
