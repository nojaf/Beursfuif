using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.BL
{
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
            return string.Format("[ClientInterval]:[{0},{1}-{2},{3}]", Id, Start, End, CurrentTime);
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
