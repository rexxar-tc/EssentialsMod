using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox.Common;
using Sandbox.ModAPI;
using Sandbox.Common.ObjectBuilders;

using VRage.ModAPI;
using VRage;
using VRage.Game;
using VRage.ObjectBuilders;

namespace DedicatedEssentials
{
	public class ServerCommandReveal : ServerCommandHandlerBase
	{
		public override string GetCommandText()
		{
			return "/reveal";
		}

		public override void HandleCommand(string[] words)
		{
			try
			{
				if (words.Length > 0)
				{
					string[] grids = words[0].Split(new char[] { ',' });

					foreach (string grid in grids)
					{
						long entityId = 0;
						if (long.TryParse(grid, out entityId))
						{
							RevealGrid(entityId);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Instance.WriteLine(string.Format("HandleNotification(): {0}", ex.ToString()));
			}

			base.HandleCommand(words);
		}

		private void RevealGrid(long entityId)
		{
			IMyEntity entity;
			if (MyAPIGateway.Entities.TryGetEntityById(entityId, out entity))
			{
				//entity.InScene = true;
				//entity.CastShadows = true;
				//entity.Visible = true;

				if (entity.InScene == false)
				{
					MyObjectBuilder_CubeGrid grid = (MyObjectBuilder_CubeGrid)entity.GetObjectBuilder();
					MyAPIGateway.Entities.RemoveEntity(entity);
					MyAPIGateway.Entities.RemoveFromClosedEntities(entity);
					grid.PersistentFlags = MyPersistentEntityFlags2.InScene;
					//MyAPIGateway.Entities.RemapObjectBuilder(grid);
					IMyEntity newEntity = MyAPIGateway.Entities.CreateFromObjectBuilderAndAdd(grid);
					Logging.Instance.WriteLine(string.Format("Revealing Grid: {0} - {1}", newEntity.EntityId, newEntity.GetPosition()));
				}
			}
		}
	}
}
