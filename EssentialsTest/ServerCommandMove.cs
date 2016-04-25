using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using Sandbox.Common;
using Sandbox.ModAPI;
using Sandbox.Common.ObjectBuilders;
using VRage.Game.ModAPI;
using VRageMath;
using VRage.ModAPI;

namespace DedicatedEssentials
{
	public class ServerCommandMove : ServerCommandHandlerBase
	{
		public override string GetCommandText()
		{
			return "/move";
		}

		public override void HandleCommand(string[] words)
		{
			if (words.Length != 4)
			{
				return;
			}

			Logging.Instance.WriteLine(string.Format("Move Command: {0}", string.Join(" ", words)));

			double x, y, z = 0;

			if(!double.TryParse(words[1], out x))
				return;

			if(!double.TryParse(words[2], out y))
				return;

			if(!double.TryParse(words[3], out z))
				return;

			Vector3D position = new Vector3D(x, y, z);

			if (words[0] == "normal")
			{
				MoveControlledEntity(position);
			}
			else if (words[0] == "spawn")
			{
				MoveSpawnEntity(position);
			}
			else
			{
				MovePlayer(position);
			}

			base.HandleCommand(words);
		}

		private void MoveControlledEntity(Vector3D position)
		{
			if (MyAPIGateway.Session.Player.Controller == null || MyAPIGateway.Session.Player.Controller.ControlledEntity == null || MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity == null)
				return;

			Logging.Instance.WriteLine(string.Format("Controlling: {0}", MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity.GetTopMostParent().DisplayName));
			Logging.Instance.WriteLine(string.Format("Moving Controller"));
			MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity.GetTopMostParent().SetPosition(position);

		}

		private void MoveSpawnEntity(Vector3D position)
		{
			if (MyAPIGateway.Session.Player.Controller == null || MyAPIGateway.Session.Player.Controller.ControlledEntity == null || MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity == null)
				return;

			if (MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity is IMyCharacter)
			{
				MoveControlledEntity(position);
				return;
			}

			IMyEntity parent = MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity.GetTopMostParent();
			MyAPIGateway.Session.Player.Controller.ControlledEntity.Use();

			Timer timer = new Timer();
			timer.Interval = 500;
			timer.AutoReset = false;
			timer.Elapsed += (object sender, ElapsedEventArgs e) =>
			{
				MyAPIGateway.Utilities.InvokeOnGameThread(() =>
				{
					Logging.Instance.WriteLine(string.Format("Moving Player"));
					MoveControlledEntity(position);
				});
			};
			timer.Enabled = true;
		}

		private void MovePlayer(Vector3D position)
		{
			if (MyAPIGateway.Session.Player.Controller == null || MyAPIGateway.Session.Player.Controller.ControlledEntity == null || MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity == null)
				return;

			Logging.Instance.WriteLine(string.Format("Moving Normal"));
			IMyEntity entity = MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity;
			if (entity is IMyCharacter)
			{
				MoveControlledEntity(position);
			}
			else
			{
				Logging.Instance.WriteLine(string.Format("Ejecting player"));
				MyAPIGateway.Session.Player.Controller.ControlledEntity.Use();

				Timer timer = new Timer();
				timer.Interval = 500;
				timer.AutoReset = false;
				timer.Elapsed += (object sender, ElapsedEventArgs e) => 
				{
					MyAPIGateway.Utilities.InvokeOnGameThread(() =>
					{
						MoveControlledEntity(position);
					});
				};

				timer.Enabled = true;
			}
		}
	}
}
