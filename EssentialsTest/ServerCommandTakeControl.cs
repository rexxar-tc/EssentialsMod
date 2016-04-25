using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox.Common;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using VRage.Game.ModAPI;
using VRage.ModAPI;

namespace DedicatedEssentials
{
	public class ServerCommandTakeControl : ServerCommandHandlerBase
	{
		public override string GetCommandText()
		{
			return "/takecontrol";
		}

		public override void HandleCommand(string[] words)
		{
			try
			{
				if (words.Length < 1)
					return;

				long entityId = 0;
				if(!long.TryParse(words[0], out entityId))
					return;

				IMyEntity entity = null;
				if (MyAPIGateway.Entities.TryGetEntityById(entityId, out entity))
				{
					//MyAPIGateway.Session.Player.Controller.TakeControl((IMyControllableEntity)entity);
					
					//IMyControllableEntity controllable = (IMyControllableEntity)entity;
					//controllable.Use();
					//IMyControllerInfo info = (IMyControllerInfo)entity;
					
				}
			}
			catch (Exception ex)
			{
				Logging.Instance.WriteLine(string.Format("HandleNotification(): {0}", ex.ToString()));
			}

			base.HandleCommand(words);
		}
	}
}
