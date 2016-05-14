using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using VRage.Game;
using VRage.ModAPI;
using VRageMath;

namespace DedicatedEssentials
{
    public class ServerDataToolbar : ServerDataHandlerBase
    {
        public override long GetDataId( )
        {
            return 5026;
        }

        public override void HandleCommand( byte[ ] data )
        {
            string text = Encoding.UTF8.GetString( data );
            ServerToolbarItem item = MyAPIGateway.Utilities.SerializeFromXML<ServerToolbarItem>( text );
            IMyEntity controllerEntity;
            if ( !MyAPIGateway.Entities.TryGetEntityById( item.EntityID, out controllerEntity ) )
            {
                Logging.Instance.WriteLine( "Failed to get cockpit for toolbar update." );
                return;
            }
            var controller = controllerEntity as MyShipController;
            if ( controller == null )
            {
                Logging.Instance.WriteLine( "Failed to get cockpit for toolbar update. 1" );
                return;
            }
            var oldName = ((IMyTerminalBlock)controller).CustomName;
            var cockpitBuilder = (MyObjectBuilder_ShipController)controller.GetObjectBuilderCubeBlock();
            cockpitBuilder.Toolbar = item.Toolbar;
            controller.Init(cockpitBuilder, controller.CubeGrid);
            ((IMyTerminalBlock)controller).SetCustomName( oldName );
        }

        public class ServerToolbarItem
        {
            public long EntityID;
            public MyObjectBuilder_Toolbar Toolbar;
        }
    }
}
