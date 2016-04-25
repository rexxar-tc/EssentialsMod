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
	public class ServerDataChangeServer : ServerDataHandlerBase
    {
		public override long GetDataId()
		{
			return 5005;
		}

		public override void HandleCommand(byte[] data)
		{
            try
            {
                Logging.Instance.WriteLine(string.Format("ChangeServer"));

                string address = "";
                for (int r = 0; r < data.Length; r++)
                    address += (char)data[r];

                EssentialsCore.Join = address;
            }
            catch (Exception ex)
            {
                Logging.Instance.WriteLine(string.Format("Handle Change Server(): {0}", ex.ToString()));
            }
		}
	}
}
