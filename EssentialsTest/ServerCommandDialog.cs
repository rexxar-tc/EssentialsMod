using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedEssentials
{
	public class ServerCommandDialog : ServerCommandHandlerBase
	{
		public override string GetCommandText()
		{
			return "/dialog";
		}

		// /dialog "title" "prefix" "current" "descrption - long" "button"
		public override void HandleCommand(string[] words)
		{
			string[] splits = Utility.SplitString(string.Join(" ", words));

			if (splits.Length != 5)
			{
				Communication.Message(string.Format("Invalid dialog message from server.  Inform the admin.  Expected {0} got {1}", 5, splits.Length));
				Logging.Instance.WriteLine("Dialog Problem: " + string.Join(" ", words));
				return;
			}

			Logging.Instance.WriteLine("Displaying Dialog: " + string.Join(" ", words));
			Communication.Dialog(splits[0], splits[1], splits[2], splits[3].Replace("|", "\r\n"), splits[4]);
			base.HandleCommand(words);
		}
	}
}
