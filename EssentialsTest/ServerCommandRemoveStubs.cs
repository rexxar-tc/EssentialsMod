using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox.Common;
using Sandbox.ModAPI;
using Sandbox.Common.ObjectBuilders;

using VRageMath;
using VRage;
using VRage.Game.ModAPI;
using VRage.ModAPI;

namespace DedicatedEssentials
{
	public class ServerCommandRemoveStubs : ServerCommandHandlerBase
	{
		public override string GetCommandText()
		{
			return "/removestubs";
		}

		public override void HandleCommand(string[] words)
		{
			try
			{
                HashSet<IMyEntity> entities = new HashSet<IMyEntity>();
                HashSet<IMyEntity> removeSet = new HashSet<IMyEntity>();

                MyAPIGateway.Entities.GetEntities(entities);

                foreach(IMyEntity entity in entities)
                {
                    if (!(entity is IMyCubeGrid))
                        continue;

                    if (entity.EntityId < 50000 && entity.EntityId > 0)
                        removeSet.Add(entity);
                }

                if(removeSet.Count < 1)
                    return;

                foreach (IMyEntity entity in removeSet)
                {
                    entity.Close();
                }
			}
			catch (Exception ex)
			{
				Logging.Instance.WriteLine(string.Format("Handle RemoveStubs(): {0}", ex.ToString()));
			}

			base.HandleCommand(words);
		}
	}
}
