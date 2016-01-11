using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using VRageMath;

namespace DedicatedEssentials
{
    public class ServerDataInfo : ServerDataHandlerBase
    {
        public override long GetDataId( )
        {
            return 5024;
        }

        public override void HandleCommand( byte[ ] data )
        {
            //ulong steamId = (ulong)((data[0] << 24) + (data[1] << 16) + (data[2] << 8) + (data[3]));
            //if ( BitConverter.ToUInt64( data, 0 ) == MyAPIGateway.Session.Player.SteamUserId )
                Core.ExtenderDataReady = true;
        }
    }    
}
