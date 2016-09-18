using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandbox.Definitions;
using Sandbox.ModAPI;
using VRage.Collections;
using VRage.Game;

namespace DedicatedEssentials
{
    public class ServerDataGMenu : ServerDataHandlerBase
    {
        public override long GetDataId()
        {
            return 5026;
        }

        public override void HandleCommand( byte[] data )
        {
            bool visible = BitConverter.ToBoolean( data, 0 );
            bool subType = BitConverter.ToBoolean( data, sizeof (bool) );
            string text = Encoding.UTF8.GetString( data, sizeof (bool) * 2, data.Length - sizeof (bool) * 2 );

            Logging.Instance.WriteLine( text );
            var allDefs = MyDefinitionManager.Static.GetAllDefinitions();
            var blockDefs = new HashSet<MyCubeBlockDefinition>();
            
            if ( subType )
            {
                foreach ( MyDefinitionBase definition in allDefs.Where( x => x is MyCubeBlockDefinition ) )
                {
                    if ( definition.Id.SubtypeId.ToString().Contains( text ) )
                    {
                        blockDefs.Add( definition as MyCubeBlockDefinition );
                        break;
                    }
                }
            }
            else
            {
                foreach ( MyDefinitionBase definition in allDefs.Where( x => x is MyCubeBlockDefinition ) )
                {
                    if ( definition.Id.TypeId.ToString().Contains( text ) )
                    {
                        blockDefs.Add( definition as MyCubeBlockDefinition );
                    }
                }
            }

            if ( blockDefs.Count == 0 )
            {
                Logging.Instance.WriteLine( "Failed to get block definition for GMenu hide" );
                return;
            }

            foreach ( var blockDef in blockDefs )
            {
                if ( blockDef == null )
                    continue;
                
                blockDef.Public = visible;
                //blockDef.Enabled = visible;
                Logging.Instance.WriteLine( string.Format( "Set block visibilty for {0} to {1}", blockDef.Id.SubtypeId, visible ) );
            }
        }
    }
}
/*public static void ToggleGMenu()
{
    // Hide gates from g-menu
    var gatesubtypes = new string[] { "Stargate S", "Stargate M", "Stargate O", "Stargate A", "Stargate U" };

    foreach (var subtype in gatesubtypes)
    {
        var def = MyDefinitionManager.Static.GetCubeBlockDefinition(new MyDefinitionId(typeof(MyObjectBuilder_Reactor), subtype));

        if (def != null)
        {
            def.Public = Configuration.Buildable;
            Logger.Instance.LogMessage("Setting " + def.BlockPairName + " to " + Configuration.Buildable);
        }
    }
}*/