using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;

using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using VRage.ModAPI;

namespace DedicatedEssentials
{
	public class ServerDataRemoveStubs : ServerDataHandlerBase
	{
		public override long GetDataId()
		{
			return 5004;
		}

		public override void HandleCommand(byte[] data)
		{
            try
            {
                HashSet<IMyEntity> entities = new HashSet<IMyEntity>();
                HashSet<IMyEntity> removeSet = new HashSet<IMyEntity>();

                MyAPIGateway.Entities.GetEntities(entities);

                foreach (IMyEntity entity in entities)
                {
                    if (!(entity is IMyCubeGrid))
                        continue;

                    if (entity.EntityId < 50000 && entity.EntityId > 0)
                        removeSet.Add(entity);
                }

                if (removeSet.Count < 1)
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
		}
	}
}
