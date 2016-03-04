using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using VRageMath;

namespace DedicatedEssentials
{
	public class ServerDataMessage : ServerDataHandlerBase
	{
		public override long GetDataId()
		{
			return 5003;
		}

		public override void HandleCommand(byte[] data)
		{
            string text = Encoding.UTF8.GetString( data );

			ServerMessageItem item = MyAPIGateway.Utilities.SerializeFromXML<ServerMessageItem>(text);
			if (item != null)
				Communication.Message(item.From, item.Message);
		}
	}

	public class ServerMessageItem
	{
		public string From { get; set; }
		public string Message { get; set; }
	}
}
