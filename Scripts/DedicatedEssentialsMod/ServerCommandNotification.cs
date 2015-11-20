using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox.Common;

namespace DedicatedEssentials
{
	public class ServerCommandNotification : ServerCommandHandlerBase
	{
		public override string GetCommandText()
		{
			return "/notification";
		}

		public override void HandleCommand(string[] words)
		{
			try
			{
				if (words.Length > 2)
				{
					string colour = words[0];
					string time = words[1];

					MyFontEnum font = MyFontEnum.White;
					Enum.TryParse<MyFontEnum>(colour, out font);

					int timeInSeconds = 2;
					int.TryParse(time, out timeInSeconds);

					string message = string.Join(" ", words.Skip(2).ToArray());

					Communication.Notification(message, timeInSeconds * 1000, font);
				}
			}
			catch (Exception ex)
			{
				Logging.Instance.WriteLine(string.Format("HandleNotification(): {0}", ex.ToString()));
			}

			base.HandleCommand(words);
		}
	}
}
