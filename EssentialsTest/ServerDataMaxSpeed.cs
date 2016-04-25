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
    public class ServerDataMaxSpeed : ServerDataHandlerBase
    {
        public override long GetDataId( )
        {
            return 5023;
        }

        public override void HandleCommand( byte[ ] data )
        {
            string text;

                text = Encoding.UTF8.GetString( data );

            float speed = MyAPIGateway.Utilities.SerializeFromXML<float>( text );
            if ( speed != 0f )
            {
                EssentialsCore.MaxSpeed = speed;
                EssentialsCore.SetMaxSpeed = true;
            }
            else
            {
                EssentialsCore.SetMaxSpeed = false;
            }
        }
    }
}
