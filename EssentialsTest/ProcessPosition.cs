using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox.ModAPI;
using Sandbox.Common.ObjectBuilders;
using VRage.ModAPI;
using DedicatedEssentials;
using VRage.Game;
using VRageMath;

namespace DedicatedEssentials
{
	public class ProcessPosition : SimulationProcessorBase
	{
		public bool m_init = false;
        private bool m_clear = false;
		private DateTime m_lastRun = DateTime.Now;
        private DateTime m_lastUpdate = DateTime.Now;

		public override void Handle()
		{
			if (MyAPIGateway.Multiplayer.IsServer)
				return;

			if (!m_init)
			{
				m_init = true;
				Init();
			}
            
            if(!m_clear && !Core.ShowPosition)
            {
                m_clear = true;
                MyAPIGateway.Utilities.GetObjectiveLine().Objectives.Clear();
                MyAPIGateway.Utilities.GetObjectiveLine().Hide();
            }

            if (Core.SetMaxSpeed)
                ProcessMaxSpeed();

			if (DateTime.Now - m_lastRun < TimeSpan.FromMilliseconds(300))
				return;

			m_lastRun = DateTime.Now;
			if (MyAPIGateway.Utilities.GetObjectiveLine().Visible && Core.ShowPosition)
			{
				if(MyAPIGateway.Session.Player.Controller == null || MyAPIGateway.Session.Player.Controller.ControlledEntity == null || MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity == null)
					return;

				Vector3D position = MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity.GetPosition();
				if (MyAPIGateway.Utilities.GetObjectiveLine().Title != Core.ServerName)
					MyAPIGateway.Utilities.GetObjectiveLine().Title = Core.ServerName;

                //if (MyAPIGateway.Session != null && MyAPIGateway.Session.Name.Contains("Transcend"))
                //{
                    string serverSpeed = "";
                    if (Core.ServerSpeed != "")
                        serverSpeed = " - SRT: " + Core.ServerSpeed;

                    MyAPIGateway.Utilities.GetObjectiveLine().Objectives[0] = string.Format("[X: {0:F0} Y: {1:F0} Z: {2:F0}]{3}", position.X, position.Y, position.Z, serverSpeed);
                //}
                //else
                //{
                //      MyAPIGateway.Utilities.GetObjectiveLine().Objectives[0] = string.Format("Position: X: {0:F0} Y: {1:F0} Z: {2:F0}", position.X, position.Y, position.Z);
                //}

                if (Core.ServerBorder >= 20000f)
                {
                    if (Math.Abs(position.X) >= Core.ServerBorder - 1500 ||
                        Math.Abs(position.Y) >= Core.ServerBorder - 1500 ||
                        Math.Abs(position.Z) >= Core.ServerBorder - 1500)
                    {
                        double distance = Math.Min(Math.Min(Core.ServerBorder - Math.Abs(position.X), Core.ServerBorder - Math.Abs(position.Y)), Core.ServerBorder - Math.Abs(position.Z));
                        if (distance >= 0d && distance < 1500d)
                            Communication.Notification(string.Format("You are {0:F0}m from the border.  If you cross the border your ship may be removed.", distance), 280, MyFontEnum.Red);
                        else if(distance < 0d)
                            Communication.Notification(string.Format("You have LEFT the game area.  If you do not return your ship may be removed! ({0:F0}m)", distance), 280, MyFontEnum.Red);
                    }
                }                
			}
		}

        private void ProcessMaxSpeed()
        {
            if (MyAPIGateway.Session.Player == null)
            {
                return;
            }

            if (MyAPIGateway.Session.Player.Controller == null)
            {
                return;
            }

            if (MyAPIGateway.Session.Player.Controller.ControlledEntity == null)
            {
                return;
            }

            if (MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity == null)
            {
                return;
            }

            IMyEntity entity = MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity;
            entity = entity.GetTopMostParent();
            if (entity.Physics == null)
            {
                return;
            }

            if (entity.Physics.LinearVelocity.Length() > Core.MaxSpeed)
            {
                entity.Physics.LinearVelocity = entity.Physics.LinearVelocity * (Core.MaxSpeed / entity.Physics.LinearVelocity.Length());
            }
        }

		public void Init()
		{
            
                MyAPIGateway.Utilities.GetObjectiveLine().Title = "";
                MyAPIGateway.Utilities.GetObjectiveLine().Objectives.Clear();
                MyAPIGateway.Utilities.GetObjectiveLine().Objectives.Add("");
                MyAPIGateway.Utilities.GetObjectiveLine().Show();
                      
		}
	}
}
