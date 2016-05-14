using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox.ModAPI;
using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Definitions;
using Sandbox.Game.Entities;
using Sandbox.Game.Gui;
using Sandbox.ModAPI.Ingame;
using VRage.Game;
using VRageMath;

namespace DedicatedEssentials
{
	public class ProcessToolbar : SimulationProcessorBase
	{
	    private bool wasInMenu;
	    private MyObjectBuilder_Toolbar oldToolbar;

		public override void Handle()
		{
			if (MyAPIGateway.Session == null || MyAPIGateway.Session.Player == null)
				return;
		    MyShipController cockpit;
		    if ( !wasInMenu )
		    {
		        if ( MyGuiScreenGamePlay.ActiveGameplayScreen != null )
		        {
                    wasInMenu = true;
                    cockpit = MyAPIGateway.Session.LocalHumanPlayer.Controller.ControlledEntity as MyShipController;
                    if (cockpit == null)
                        return;

                    oldToolbar = ((MyObjectBuilder_ShipController)cockpit.GetObjectBuilderCubeBlock()).Toolbar;
                }
                return;
		    }

		    if ( wasInMenu && MyGuiScreenGamePlay.ActiveGameplayScreen != null )
		        return;
            
		    cockpit = MyAPIGateway.Session.LocalHumanPlayer.Controller.ControlledEntity as MyShipController;
            if (cockpit == null)
                return;

            var newToolbar = ((MyObjectBuilder_ShipController)cockpit.GetObjectBuilderCubeBlock()).Toolbar;

		    //string oldToolbarString = MyAPIGateway.Utilities.SerializeToXML(oldToolbar);
		    //string newToolbarString = MyAPIGateway.Utilities.SerializeToXML(newToolbar);
            

            //if ( oldToolbarString == newToolbarString )
		    //    return;

		    var item = new ServerDataToolbar.ServerToolbarItem
		    {
		        Toolbar = newToolbar,
		        EntityID = cockpit.EntityId
		    };


		    var message = MyAPIGateway.Utilities.SerializeToXML( item );
		    var data = Encoding.UTF8.GetBytes( message );
		    //MyAPIGateway.Multiplayer.SendMessageToServer( 9003, data );
            Communication.SendMessageParts( 5027, data );

		    wasInMenu = false;
		    //oldToolbar = newToolbar;
		}
	}
}
