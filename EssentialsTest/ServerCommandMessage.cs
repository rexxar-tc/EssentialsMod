using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedEssentials
{
	public class ServerCommandMessage : ServerCommandHandlerBase
	{
		public override string GetCommandText()
		{
			return "/message";
		}

		public override void HandleCommand(string[] words)
		{
			string[] split = Utility.SplitString(string.Join(" ", words));
			Communication.Message(split[0], string.Join(" ", split.Skip(1)));
			base.HandleCommand(words);
		}
	}
}
