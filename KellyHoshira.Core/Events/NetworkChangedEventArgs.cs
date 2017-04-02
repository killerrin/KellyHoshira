using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KellyHoshira.Core.Events
{

    public delegate void NetworkChangedEventHandler(object sender, NetworkChangedEventArgs args);
    public class NetworkChangedEventArgs : EventArgs
    {
        OnlineStatus Status { get; }

        public NetworkChangedEventArgs(OnlineStatus status)
            : base()
        {
            Status = status;
        }
    }
}
