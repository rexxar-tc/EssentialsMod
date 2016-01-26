using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Common;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using VRageMath;

namespace DedicatedEssentials
{
    public class ServerDataNotification : ServerDataHandlerBase
    {
        public override long GetDataId( )
        {
            return 5022;
        }

        public override void HandleCommand( byte[ ] data )
        {
            string text = Encoding.Unicode.GetString( data );

            ServerNotificationItem item = MyAPIGateway.Utilities.SerializeFromXML<ServerNotificationItem>( text );
            if ( item != null )
                    Communication.Notification( item.message, item.time, item.color );
            
        }
    }

    public class ServerNotificationItem
    {
        public MyFontEnum color { get; set; }
        public int time { get; set; }
        public string message { get; set; }
    }    
}
