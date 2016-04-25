using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRageMath;

namespace DedicatedEssentials
{
    public class ServerDataMove : ServerDataHandlerBase
    {
        public override long GetDataId( )
        {
            return 5021;
        }

        public override void HandleCommand( byte[ ] data )
        {
            string text;

                text = Encoding.UTF8.GetString( data );

            ServerMoveItem item = MyAPIGateway.Utilities.SerializeFromXML<ServerMoveItem>( text );
            if ( item != null )
            {
                Vector3D position = new Vector3D( item.x, item.y, item.z );

                if ( item.entityId != 0 )
                {
                    IMyEntity entityToMove = MyAPIGateway.Entities.GetEntityById( item.entityId );
                    entityToMove.GetTopMostParent( ).SetPosition( position );
                }

                if ( item.moveType == "normal" )
                {
                    MoveControlledEntity( position );
                }
                else if ( item.moveType == "spawn" )
                {
                    MoveSpawnEntity( position );
                }
                else
                {
                    MovePlayer( position );
                }
            }
        }


        private void MoveControlledEntity( Vector3D position )
        {
            if ( MyAPIGateway.Session.Player.Controller == null || MyAPIGateway.Session.Player.Controller.ControlledEntity == null || MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity == null )
                return;

            Logging.Instance.WriteLine( string.Format( "Controlling: {0}", MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity.GetTopMostParent( ).DisplayName ) );
            Logging.Instance.WriteLine( string.Format( "Moving Controller" ) );
            MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity.GetTopMostParent( ).SetPosition( position );

        }

        private void MoveSpawnEntity( Vector3D position )
        {
            if ( MyAPIGateway.Session.Player.Controller == null || MyAPIGateway.Session.Player.Controller.ControlledEntity == null || MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity == null )
                return;

            if ( MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity is IMyCharacter )
            {
                MoveControlledEntity( position );
                return;
            }

            IMyEntity parent = MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity.GetTopMostParent( );
            MyAPIGateway.Session.Player.Controller.ControlledEntity.Use( );

            Timer timer = new Timer( );
            timer.Interval = 500;
            timer.AutoReset = false;
            timer.Elapsed += ( object sender, ElapsedEventArgs e ) =>
            {
                MyAPIGateway.Utilities.InvokeOnGameThread( ( ) =>
                {
                    Logging.Instance.WriteLine( string.Format( "Moving Player" ) );
                    MoveControlledEntity( position );
                } );
            };
            timer.Enabled = true;
        }

        private void MovePlayer( Vector3D position )
        {
            if ( MyAPIGateway.Session.Player.Controller == null || MyAPIGateway.Session.Player.Controller.ControlledEntity == null || MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity == null )
                return;

            Logging.Instance.WriteLine( string.Format( "Moving Normal" ) );
            IMyEntity entity = MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity;
            if ( entity is IMyCharacter )
            {
                MoveControlledEntity( position );
            }
            else
            {
                Logging.Instance.WriteLine( string.Format( "Ejecting player" ) );
                MyAPIGateway.Session.Player.Controller.ControlledEntity.Use( );
                entity.Physics.LinearVelocity = Vector3D.Zero;
                entity.Physics.AngularVelocity = Vector3D.Zero;
                //stop the ship so the player doesn't smash into a wall or something

                Timer timer = new Timer( );
                timer.Interval = 500;
                timer.AutoReset = false;
                timer.Elapsed += ( object sender, ElapsedEventArgs e ) =>
                {
                    MyAPIGateway.Utilities.InvokeOnGameThread( ( ) =>
                    {
                        MoveControlledEntity( position );
                    } );
                };

                timer.Enabled = true;
            }
        }

        public class ServerMoveItem
        {
            public string moveType { get; set; }
            public double x { get; set; }
            public double y { get; set; }
            public double z { get; set; }
            public long entityId { get; set; }
        } 
    }
}
