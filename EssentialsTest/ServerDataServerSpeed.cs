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
	public class ServerDataServerSpeed : ServerDataHandlerBase
    {
		public override long GetDataId()
		{
			return 5006;
		}

		public override void HandleCommand(byte[] data)
		{
            try
            {
                Logging.Instance.WriteLine(string.Format("Received ServerSpeed: {0}", data.Length));

                string serverSpeed = Encoding.UTF8.GetString( data );

                Core.ServerSpeed = serverSpeed;
            }
            catch (Exception ex)
            {
                Logging.Instance.WriteLine(string.Format("HandleServerSpeed(): {0}", ex.ToString()));
            }
		}
	}
}
