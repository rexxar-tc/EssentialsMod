using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Common.ObjectBuilders;
using VRageMath;

namespace DedicatedEssentials
{
	public class CommandPosition : CommandHandlerBase
	{
		private Random m_random = new Random();

		public override String GetCommandText()
		{
			return "position";
		}

		public override void HandleCommand(String[] words)
		{
			EssentialsCore.ShowPosition = !EssentialsCore.ShowPosition;
			Communication.Message(string.Format("Show Position setting: {0}", EssentialsCore.ShowPosition));
            if (!EssentialsCore.ShowPosition)
            {
                MyAPIGateway.Utilities.GetObjectiveLine().Objectives.Clear();
                MyAPIGateway.Utilities.GetObjectiveLine().Hide();
            }
            else
            {
                ProcessPosition posInit = new ProcessPosition();
                posInit.m_init = true;
                posInit.Init();
                MyAPIGateway.Utilities.GetObjectiveLine().Show();
            }
        }
	}
}
