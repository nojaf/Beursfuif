using System;
using Beursfuif.Server.Entity;

namespace Beursfuif.BL
{
    /// <summary>
    /// DTO class that wraps the current interval data with ClientDrink DTOs
    /// </summary>
    public class ClientInterval
    {
        #region properties

        public int Id { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public ClientDrink[] ClientDrinks { get; set; }

        public DateTime CurrentTime { get; set; }
        #endregion

        public ClientInterval()
        {

        }

        public override string ToString()
        {
            return $"[ClientInterval]:[{Id},{Start}-{End},{CurrentTime}]";
        }

        public override bool Equals(object obj)
        {
            if (obj is ClientInterval)
            {
                ClientInterval other = obj as ClientInterval;
                if (other.Id == this.Id) return true;
            }
            return false;
        }
    }
}
