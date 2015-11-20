using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox.ModAPI;
using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Definitions;

using VRageMath;

namespace DedicatedEssentials
{
	public class ProcessCharacter : SimulationProcessorBase
	{
        private DateTime m_lastRun = DateTime.Now;
        public override void Handle()
		{
			if (MyAPIGateway.Session == null || MyAPIGateway.Session.Player == null)
				return;

            if (DateTime.Now - m_lastRun < TimeSpan.FromSeconds(5))
                return;

            m_lastRun = DateTime.Now;
            if(MyAPIGateway.Session.Player.Controller != null && MyAPIGateway.Session.Player.Controller.ControlledEntity != null && MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity != null)
            {
                if(MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity is IMyCharacter)
                {
                    string entityId = MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity.EntityId.ToString();
                    //Logging.Instance.WriteLine(string.Format("ID: {0}", entityId));
                    Communication.SendDataToServer(5012, entityId);
                }
            }
		}
	}
}

