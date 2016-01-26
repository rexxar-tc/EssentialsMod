﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using VRageMath;

namespace DedicatedEssentials
{
	public class ServerDataTest : ServerDataHandlerBase
	{
		public override long GetDataId()
		{
			return 5000;
		}

		public override void HandleCommand(byte[] data)
		{
			string text = Encoding.Unicode.GetString( data );

            Communication.Message("Server", string.Format("Message: {0}", text));
		}
	}
}
