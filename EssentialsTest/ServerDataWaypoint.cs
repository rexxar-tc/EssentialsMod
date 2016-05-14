using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox.ModAPI;
using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI.Interfaces;

using VRageMath;
using VRage;
using VRage.Game;
using VRage.ModAPI;
using System.Timers;
using VRage.Game.Entity;
using VRage.Game.ModAPI;

namespace DedicatedEssentials
{    
	public class ServerDataWaypoint : ServerDataHandlerBase
	{
        private static List<long> clientWaypoints = new List<long>( );
        public static List<long> ClientWaypoints
        {
            get
            {
                return clientWaypoints;
            }
            private set
            {
                clientWaypoints = value;
            }
        }

        public override long GetDataId( )
        {
            return 5025;
        }    

    public enum WaypointTypes
		{
			Neutral,
			Allied,
			Enemy
		}

		// /waypoint add "name" "text" X Y Z
		// /waypoint remove "name"
		public override void HandleCommand(byte[] data)
		{
            string text;

                text = Encoding.UTF8.GetString( data );

            //string[ ] words = text.Split( new string[ ] { " " }, StringSplitOptions.RemoveEmptyEntries );

            Logging.Instance.WriteLine( string.Format( "Waypoint: {0}", text ) );

            string[ ] splits = Utility.SplitString( text );

            if (splits[0] == "add")
			{
				long distance = 1000001L;
				Vector3D vector = new Vector3D(double.Parse(splits[4]), double.Parse(splits[5]), double.Parse(splits[6]));
				WaypointTypes type = WaypointTypes.Neutral;
				Enum.TryParse<WaypointTypes>(splits[3], true, out type);
				if (splits.Length > 7)
					long.TryParse(splits[7], out distance);

				HandleWaypointAdd(splits[1], splits[2], type, vector, distance);
			}
			else if (splits[0] == "remove")
			{
				HandleWaypointRemove(splits[1]);
			}
			else if (splits[0] == "clear")
			{
				HandleWaypointClear();
			}
		}

		private void HandleWaypointAdd(string name, string text, WaypointTypes type, Vector3D position, long distance)
		{
			string waypointBuilder = @"<?xml version=""1.0""?>
<MyObjectBuilder_CubeGrid xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
    <EntityId>0</EntityId>
    <PersistentFlags>CastShadows InScene</PersistentFlags>
    <PositionAndOrientation>
		<Position x=""0"" y=""0"" z=""0"" />
		<Forward x=""0"" y=""-0"" z=""-1"" />
		<Up x=""0"" y=""1"" z=""0"" />
    </PositionAndOrientation>
    <GridSizeEnum>Large</GridSizeEnum>
    <CubeBlocks>
    <MyObjectBuilder_CubeBlock xsi:type=""MyObjectBuilder_Beacon"">
        <SubtypeName>LargeBlockBeacon</SubtypeName>
        <EntityId>0</EntityId>
        <Min x=""0"" y=""1"" z=""0"" />
        <BlockOrientation Forward=""Forward"" Up=""Up"" />
        <ColorMaskHSV x=""0"" y=""-1"" z=""0"" />
        <ShareMode>None</ShareMode>
        <DeformationRatio>0</DeformationRatio>
        <ShowOnHUD>false</ShowOnHUD>
        <Enabled>true</Enabled>
        <BroadcastRadius>{0}</BroadcastRadius>
    </MyObjectBuilder_CubeBlock>
    <MyObjectBuilder_CubeBlock xsi:type=""MyObjectBuilder_Reactor"">
        <SubtypeName>Waypoint</SubtypeName>
        <EntityId>0</EntityId>
        <Min x=""0"" y=""0"" z=""0"" />
        <BlockOrientation Forward=""Forward"" Up=""Up"" />
        <ColorMaskHSV x=""0"" y=""-1"" z=""0"" />
        <ShareMode>None</ShareMode>
        <DeformationRatio>0</DeformationRatio>
        <ShowOnHUD>false</ShowOnHUD>
        <Enabled>true</Enabled>
        <Inventory>
        <Items>
            <MyObjectBuilder_InventoryItem>
            <Amount>1</Amount>
            <PhysicalContent xsi:type=""MyObjectBuilder_Ingot"">
                <SubtypeName>Uranium</SubtypeName>
            </PhysicalContent>
            <ItemId>0</ItemId>
            <AmountDecimal>10000</AmountDecimal>
            </MyObjectBuilder_InventoryItem>
        </Items>
        <nextItemId>1</nextItemId>
        </Inventory>
    </MyObjectBuilder_CubeBlock>
    </CubeBlocks>
    <IsStatic>true</IsStatic>
    <Skeleton />
    <LinearVelocity x=""0"" y=""0"" z=""0"" />
    <AngularVelocity x=""0"" y=""0"" z=""0"" />
    <XMirroxPlane xsi:nil=""true"" />
    <YMirroxPlane xsi:nil=""true"" />
    <ZMirroxPlane xsi:nil=""true"" />
    <BlockGroups />
    <Handbrake>false</Handbrake>
    <DisplayName>Waypoint</DisplayName>
</MyObjectBuilder_CubeGrid>";

			try
			{
				waypointBuilder = string.Format(waypointBuilder, distance);
				Logging.Instance.WriteLine(string.Format("Adding Waypoint: {0} - {1} - {2}", name, type, position));

				MyObjectBuilder_CubeGrid cubeGrid = MyAPIGateway.Utilities.SerializeFromXML<MyObjectBuilder_CubeGrid>(waypointBuilder);
				cubeGrid.DisplayName = string.Format("Waypoint_{0}", name);

				foreach (MyObjectBuilder_CubeBlock block in cubeGrid.CubeBlocks)
				{
					if (block is MyObjectBuilder_Beacon)
					{
						MyObjectBuilder_Beacon beacon = (MyObjectBuilder_Beacon)block;
						beacon.CustomName = text;

						if (type == WaypointTypes.Enemy)
							beacon.Owner = FindEnemyPlayer(MyAPIGateway.Session.Player.PlayerID);
						else if (type == WaypointTypes.Allied)
							beacon.Owner = MyAPIGateway.Session.Player.PlayerID;
					}
				}

                MyAPIGateway.Entities.RemapObjectBuilder(cubeGrid);
                cubeGrid.PositionAndOrientation = new MyPositionAndOrientation( position, Vector3.Forward, Vector3.Up );
                IMyEntity entity = MyAPIGateway.Entities.CreateFromObjectBuilder( cubeGrid );
                if ( entity != null )
                    Logging.Instance.WriteLine( string.Format( "Waypoint added" ) );
                
                if ( entity.Physics == null )
                    Logging.Instance.WriteLine( string.Format( "Physics is null" ) );
                else
                    entity.Physics.Enabled = false;

                entity.Flags &= ~EntityFlags.Visible;
                entity.Flags &= ~EntityFlags.Sync;
                entity.Flags &= ~EntityFlags.Save;

                //TODO: Make sure this works
			    //((MyEntity)entity).IsPreview = true;

                ClientWaypoints.Add( entity.EntityId );
                MyAPIGateway.Entities.AddEntity( entity, true );
                //HACK: workaround because the Sync flag is not being respected
                //remove this whenever the devs fix it
                //spawn the waypoint in disabled because they show up for one game tick
                //on all remote clients. wait a bit for them to cleanup then turn it on
                /*
                Timer timer = new Timer( );
                timer.Interval = 100;
                timer.Elapsed += ( object source, ElapsedEventArgs e ) =>
                {
                    List<IMySlimBlock> blocks = new List<IMySlimBlock>( );
                    ((IMyCubeGrid)entity).GetBlocks( blocks );
                    foreach ( IMySlimBlock block in blocks )
                    {
                        IMyCubeBlock cubeBlock = block.FatBlock;

                        if ( cubeBlock.BlockDefinition.TypeId == typeof( MyObjectBuilder_Beacon ) )
                        {
                            Sandbox.ModAPI.Ingame.IMyBeacon beacon = (Sandbox.ModAPI.Ingame.IMyBeacon)cubeBlock;
                            beacon.RequestEnable( true );
                        }
                    }
                };*/

            }
			catch (Exception ex)
			{
				Logging.Instance.WriteLine(string.Format("HandleWaypointAdd(): {0}", ex.ToString()));					
			}
		}

		private void HandleWaypointRemove(string name)
		{
			try
			{
				HashSet<IMyEntity> entities = new HashSet<IMyEntity>();
				MyAPIGateway.Entities.GetEntities(entities, x => x is IMyCubeGrid && x.DisplayName.ToLower() == string.Format("Waypoint_{0}", name).ToLower());
				foreach (IMyEntity entity in entities)
				{
					if (!(entity is IMyCubeGrid))
						continue;

					Logging.Instance.WriteLine(string.Format("Removing waypoint: {0}", name));
				    entity.Close();
				    /*
					IMyCubeGrid grid = (IMyCubeGrid)entity;

					if (grid.IsStatic)
						grid.ConvertToDynamic();

					if (grid.Physics != null)
						grid.Physics.Enabled = false;

					grid.SetPosition(new Vector3D(10000000, 0, 0));

					Timer timer = new Timer();
					timer.Interval = 2000;
					timer.Elapsed += (object source, ElapsedEventArgs e) =>
					{
						MyAPIGateway.Utilities.InvokeOnGameThread(() =>
						{
							MyAPIGateway.Entities.RemoveEntity(entity);
						});
					    ClientWaypoints.Remove(entity.EntityId);
					};
					timer.Enabled = true;*/
				}

				/*
				HashSet<IMyEntity> entities = new HashSet<IMyEntity>();
				MyAPIGateway.Entities.GetEntities(entities, x => x.DisplayName == string.Format("Waypoint_{0}", name));
				foreach (IMyEntity entity in entities)
				{
					if (!(entity is IMyCubeGrid))
						continue;

					Logging.Instance.WriteLine(string.Format("Removing waypoint: {0}", name));

					IMyCubeGrid grid = (IMyCubeGrid)entity;
					/*
					List<IMySlimBlock> blocks = new List<IMySlimBlock>();
					grid.GetBlocks(blocks, x => x.FatBlock != null);
					foreach (IMySlimBlock block in blocks)
					{
						if(block.FatBlock.BlockDefinition.TypeId == typeof(MyObjectBuilder_Beacon))
						{
							// Figure out what about this hides the beacon
							Sandbox.ModAPI.Ingame.IMyTerminalBlock terminal = (Sandbox.ModAPI.Ingame.IMyTerminalBlock)block.FatBlock;
							//entity.Physics.Enabled = true;
							//terminal.SetCustomName("");							
							//entity.Render.Visible = false;
							//terminal.Visible = false;
							//grid.SetPosition(new Vector3D(100000000, 0, 0));
						}
					}

					grid.ConvertToDynamic();
					grid.SetPosition(new Vector3D(10000000, 0, 0));
					MyAPIGateway.Entities.RemoveEntity(entity);

					// This removes the model and makes the beacon stop broadcasting, go figure.
					if (entity.PositionComp == null)
						Logging.Instance.WriteLine(string.Format("PositionComp is null"));
					else
						entity.PositionComp.Scale = 0f;
					*/

					//MyAPIGateway.Entities.RemoveFromClosedEntities(entity);

					/*
					 * testing theory
					grid.Visible = false;
					grid.CastShadows = false;
					grid.Render.Transparency = 1f;
					grid.Render.UpdateRenderObject(false);
					grid.Render.ShadowBoxLod = false;					
					grid.Physics.Enabled = false;
					grid.NeedsUpdate = Sandbox.Common.MyEntityUpdateEnum.BEFORE_NEXT_FRAME;

					Timer test = new Timer();
					test.Interval = 2000;
					test.AutoReset = false;
					test.Elapsed += new ElapsedEventHandler((x, y) =>
					{
						MyAPIGateway.Utilities.InvokeOnGameThread(() =>
						{
							grid.InScene = false;
							Communication.Message("Done");
						});
					});
					test.Start();
				}
			 */
			}
			catch (Exception ex)
			{
				Logging.Instance.WriteLine(string.Format("HandleWaypointRemove(): {0}", ex.ToString()));
			}
		}

		private void HandleWaypointClear()
		{
			Logging.Instance.WriteLine("Clearing");
			try
			{
				HashSet<IMyEntity> entities = new HashSet<IMyEntity>();
				MyAPIGateway.Entities.GetEntities(entities, x => x is IMyCubeGrid && x.DisplayName.StartsWith("Waypoint_"));
				Logging.Instance.WriteLine(string.Format("Count: {0}", entities.Count));
				foreach (IMyEntity entity in entities)
				{
					if (!(entity is IMyCubeGrid))
						continue;

					Logging.Instance.WriteLine(string.Format("Removing waypoint"));

					IMyCubeGrid grid = (IMyCubeGrid)entity;

				    entity.Close();
				    /*
					if (grid.IsStatic)
						grid.ConvertToDynamic();

					if (grid.Physics != null)
						grid.Physics.Enabled = false;

					grid.SetPosition(new Vector3D(10000000, 0, 0));

					Timer timer = new Timer();
					timer.Interval = 2000;
					timer.Elapsed += (object source, ElapsedEventArgs e) =>
					{
						MyAPIGateway.Utilities.InvokeOnGameThread(() =>
						{
							MyAPIGateway.Entities.RemoveEntity(entity);
						});
				    ClientWaypoints.Remove(entity.EntityId);
					};
					timer.Enabled = true;
                    */
				}
			}
			catch (Exception ex)
			{
				Logging.Instance.WriteLine(string.Format("Clear(): {0}", ex.ToString()));
			}
		}

		private long FindEnemyPlayer(long playerId)
		{			
			List<IMyIdentity> identities = new List<IMyIdentity>();
			MyAPIGateway.Players.GetAllIdentites(identities);
			foreach(IMyIdentity identity in identities)
			{
				if(identity.PlayerId == playerId)
					continue;

			    if (identity.DisplayName.Contains("Space Pirate"))
			        return identity.PlayerId;
			    /*
				if((MyAPIGateway.Session.Player.GetRelationTo(identity.PlayerId) == MyRelationsBetweenPlayerAndBlock.Enemies || 
				    MyAPIGateway.Session.Player.GetRelationTo(identity.PlayerId) == MyRelationsBetweenPlayerAndBlock.Neutral) &&
				    MyAPIGateway.Session.Factions.TryGetPlayerFaction(identity.PlayerId) == null)
				{
					return identity.PlayerId;
				}	*/
			}
            /*
			foreach (IMyIdentity identity in identities)
			{
				if (identity.PlayerId == playerId)
					continue;

				if ((MyAPIGateway.Session.Player.GetRelationTo(identity.PlayerId) == MyRelationsBetweenPlayerAndBlock.Enemies ||
					MyAPIGateway.Session.Player.GetRelationTo(identity.PlayerId) == MyRelationsBetweenPlayerAndBlock.Neutral))
				{
					return identity.PlayerId;
				}
			}
            */
			return 0;
		}

		
	}
}
