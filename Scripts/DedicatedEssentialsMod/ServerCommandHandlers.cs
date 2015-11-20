using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedEssentials
{
	public class ServerCommandHandlers
	{
		private static List<ServerCommandHandlerBase> m_serverCommands = new List<ServerCommandHandlerBase>();
		public static List<ServerCommandHandlerBase> ServerCommands
		{
			get { return ServerCommandHandlers.m_serverCommands; }
		}

		private static ServerCommandHandlers m_instance;
		public static ServerCommandHandlers Instance
		{
			get 
			{
				if (m_instance == null)
					m_instance = new ServerCommandHandlers();
										
				return ServerCommandHandlers.m_instance; 
			}
		}

		public static void ProcessCommands(List<string> cmdList)
		{
			try
			{
				foreach (string command in cmdList)
				{
					if (command[0] != '/')
						return;

					string[] commandParts = command.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
					int paramCount = commandParts.Length - 1;

					foreach (ServerCommandHandlerBase chatHandler in ServerCommands)
					{
						int commandCount = 0;
						if (chatHandler.CanHandle(commandParts.ToArray(), ref commandCount))
						{
							chatHandler.HandleCommand(commandParts.Skip(1).ToArray());
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Instance.WriteLine(string.Format("ProcessCommands(): {0}", ex.ToString()));
			}
		}
	}
}
