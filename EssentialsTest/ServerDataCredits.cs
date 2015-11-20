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
	public class ServerDataCredits : ServerDataHandlerBase
    {
		public override long GetDataId()
		{
			return 5007;
		}

		public override void HandleCommand(byte[] data)
		{
            try
            {
                string credits = "";
                for (int r = 0; r < data.Length; r++)
                    credits += (char)data[r];

                Core.Credits = credits;
            }
            catch (Exception ex)
            {
                Logging.Instance.WriteLine(string.Format("HandleServerSpeed(): {0}", ex.ToString()));
            }
		}
	}
}
