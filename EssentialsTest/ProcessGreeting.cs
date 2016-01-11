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
	public class ProcessLogin : SimulationProcessorBase
	{
        private bool m_ran = false;
		public override void Handle()
		{
			if (MyAPIGateway.Session == null || MyAPIGateway.Session.Player == null)
				return;

            if(!m_ran)
            {
                m_ran = true;
                //Communication.SendDataToServer(5011, "Login");
            }
		}
	}
}
