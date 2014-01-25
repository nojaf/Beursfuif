using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beursfuif.BL.Event;
using Beursfuif.BL;

namespace Beursfuif.Server.Services
{
    [HubName("beursfuif")]
    public class BeursfuifHub:Hub
    {
        public static event EventHandler<Package> NewPackageReceived;

        protected void OnPackageReceived(object sender,Package e)
        {
            if(NewPackageReceived != null)
            {
                NewPackageReceived(sender, e);
            }
        }

        public void LogOn(string name)
        {
            OnPackageReceived(this, new Package()
            {
                Kind = ProtocolKind.NEW_CLIENT_CONNECTS,
                Name = name,
                ClientContext = Context.ConnectionId,
                Ip = GetIpAddress()
            });
        }

        public void AckTimeUpdate(string auth)
        {
            OnPackageReceived(this, new Package()
            {
                Kind = ProtocolKind.ACK_TIME_UPDATE,
                AuthenticationCode = auth,
                ClientContext = base.Context.ConnectionId
            });
        }

        public void NewOrder(ClientDrinkOrder[] order, string auth)
        {
            OnPackageReceived(this, new Package()
            {
                Kind = ProtocolKind.NEW_ORDER,
                AuthenticationCode = auth,
                NewOrder = order,
                ClientContext = base.Context.ConnectionId
            });
        }


        #region Helper
        protected string GetIpAddress()
        {
            string ipAddress;
            object tempObject;

            Context.Request.Environment.TryGetValue("server.RemoteIpAddress", out tempObject);

            if (tempObject != null)
            {
                ipAddress = (string)tempObject;
            }
            else
            {
                ipAddress = "";
            }

            return ipAddress;
        }
        #endregion

        public override Task OnDisconnected()
        {
            OnPackageReceived(this, new Package()
            {
                ClientContext = Context.ConnectionId,
                Kind = ProtocolKind.CLIENT_LEAVES_SERVER
            });
            return base.OnDisconnected();
        }
    }
}
