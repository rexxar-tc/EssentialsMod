﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.Common;
using Sandbox.ModAPI;
using Sandbox.Definitions;

namespace DedicatedEssentials
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation | MyUpdateOrder.AfterSimulation | MyUpdateOrder.Simulation)]
    public class Core : MySessionComponentBase
    {
        // Declarations
        private static string version = "v0.1.0.9";
        private static bool m_debug = false;
		private static bool m_showPosition = true;
		private static string m_serverName = "";
        private static float m_serverBorder = 0f;
		private static List<string> m_serverCommandList = new List<string>();
        private static string m_join = "";
        private static System.Timers.Timer timer = new System.Timers.Timer();
        private static bool m_setMaxSpeed = false;
        private static float m_maxSpeed = 0f;

        private bool m_initialized = false;
        private List<CommandHandlerBase> m_chatHandlers = new List<CommandHandlerBase>();
        private List<SimulationProcessorBase> m_simHandlers = new List<SimulationProcessorBase>();
		private List<ServerDataHandlerBase> m_dataHandlers = new List<ServerDataHandlerBase>();
        
        // Properties
        public static bool Debug
        {
            get { return m_debug; }
            set { m_debug = value; }
        }

		public static bool ShowPosition
		{
			get { return m_showPosition; }
			set { m_showPosition = value; }
		}

		public static List<string> ServerCommandList
		{
			get { return m_serverCommandList; }
		}

		public static string ServerName
		{
			get { return Core.m_serverName; }
			set { Core.m_serverName = value; }
		}

        public static float ServerBorder
        {
            get { return Core.m_serverBorder; }
            set { Core.m_serverBorder = value; }
        }

        public static string Join
        {
            get { return m_join; }
            set { m_join = value; }
        }

        public static bool SetMaxSpeed
        {
            get { return m_setMaxSpeed; }
            set { m_setMaxSpeed = value; }
        }

        public static float MaxSpeed
        {
            get { return m_maxSpeed; }
            set { m_maxSpeed = value; }
        }

        public static string Credits { get; set; }

        public static string ServerSpeed { get; set; }

        // Initializers
        private void Initialize()
        {
			// Load Settings
			//Settings.Instance.Load();

            // Chat Line Event
            AddMessageHandler();

            // Chat Handlers
			m_chatHandlers.Add(new CommandTest());
			m_chatHandlers.Add(new CommandPosition());

            // Simulation Handlers
			m_simHandlers.Add(new ProcessCommunication());
			m_simHandlers.Add(new ProcessPosition());
			m_simHandlers.Add(new ProcessDetection());
            m_simHandlers.Add(new ProcessLogin());
            m_simHandlers.Add(new ProcessCharacter());

            // Server Command Handlers
            ServerCommandHandlers.ServerCommands.Add(new ServerCommandMessage());
			ServerCommandHandlers.ServerCommands.Add(new ServerCommandNotification());
			ServerCommandHandlers.ServerCommands.Add(new ServerCommandDialog());
			ServerCommandHandlers.ServerCommands.Add(new ServerCommandConceal());
			ServerCommandHandlers.ServerCommands.Add(new ServerCommandReveal());
			ServerCommandHandlers.ServerCommands.Add(new ServerCommandWaypoint());
			ServerCommandHandlers.ServerCommands.Add(new ServerCommandMove());
			ServerCommandHandlers.ServerCommands.Add(new ServerCommandTakeControl());
            //ServerCommandHandlers.ServerCommands.Add(new ServerCommandRemoveStubs());

			// Server Message Handlers
			m_dataHandlers.Add(new ServerDataTest());
			m_dataHandlers.Add(new ServerDataVoxelHeader());
			m_dataHandlers.Add(new ServerDataVoxelData());
			m_dataHandlers.Add(new ServerDataMessage());
            m_dataHandlers.Add(new ServerDataRemoveStubs());
            m_dataHandlers.Add(new ServerDataChangeServer());
            m_dataHandlers.Add(new ServerDataServerSpeed());
            m_dataHandlers.Add(new ServerDataCredits());

            // Setup Grid Tracker
            //CubeGridTracker.SetupGridTracking();

            Logging.Instance.WriteLine(String.Format("Script Initialized: {0}", version));

            ServerSpeed = "";
            Credits = "";
        }

        // Utility
        public void HandleMessageEntered(string messageText, ref bool sendToOthers)
        {
            try
            {
                string[] commandParts = messageText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
				if (commandParts[0].ToLower() == "/essential")
				{
					int paramCount = commandParts.Length - 1;
					if (paramCount < 1 || (paramCount == 1 && commandParts[1].ToLower() == "help"))
					{
						List<string> commands = new List<string>();
						foreach (string command in ServerCommandList)
						{
							if (!commands.Contains(command))
								commands.Add(command);
						}

						String commandList = String.Join(", ", commands.ToArray());
						String info = String.Format("Dedicated Essentials Client Side Script {0}.  Available server commands: {1} - {2}", version, commandList, MyAPIGateway.Session.Name);
						sendToOthers = false;
						Communication.Message(info);
						return;
					}

					foreach (CommandHandlerBase chatHandler in m_chatHandlers)
					{
						int commandCount = 0;
						if (chatHandler.CanHandle(commandParts.Skip(1).ToArray(), ref commandCount))
						{
							chatHandler.HandleCommand(commandParts.Skip(commandCount + 1).ToArray());
							sendToOthers = false;
							return;
						}
					}

					return;
				}

				foreach (string command in ServerCommandList)
				{
					if (commandParts[0].ToLower() == command)
					{
						Communication.SendMessageToServer(messageText);
						sendToOthers = false;
                        return;
					}
				}

                if (MyAPIGateway.Session != null && MyAPIGateway.Session.Name.Contains("Transcend"))
                {
                    // Route messages to server
                    Communication.SendDataToServer(5010, messageText);
                    sendToOthers = false;
                    Communication.Message(MyAPIGateway.Session.Player.DisplayName, messageText);
                }
            }
            catch (Exception ex)
            {
                Logging.Instance.WriteLine(String.Format("HandleMessageEntered(): {0}", ex.ToString()));
            }
        }

		public void HandleServerData(byte[] data)
		{
            if (MyAPIGateway.Multiplayer.IsServer)
                return;

			Logging.Instance.WriteLine(string.Format("Received Server Data: {0} bytes", data.Length));
			foreach (ServerDataHandlerBase handler in m_dataHandlers)
			{
				if (handler.CanHandle(data))
				{
					try
					{
						byte[] newData = handler.ProcessCommand(data);
						handler.HandleCommand(newData);
						break;
					}
					catch (Exception ex)
					{
						Logging.Instance.WriteLine(string.Format("HandleCommand(): {0}", ex.ToString()));
					}
				}
			}
		}

        public void AddMessageHandler()
        {
            MyAPIGateway.Utilities.MessageEntered += HandleMessageEntered;
			MyAPIGateway.Multiplayer.RegisterMessageHandler(9000, HandleServerData);
        }

        public void RemoveMessageHandler()
        {
            MyAPIGateway.Utilities.MessageEntered -= HandleMessageEntered;
			MyAPIGateway.Multiplayer.UnregisterMessageHandler(9000, HandleServerData);
		}

        // Overrides
        public override void UpdateBeforeSimulation()
        {
			try
			{
				if (MyAPIGateway.Session == null)
					return;

				// Run the init
				if (!m_initialized)
				{
					m_initialized = true;
					Initialize();
				}

                // Run the sim handlers
                foreach (SimulationProcessorBase simHandler in m_simHandlers)
				{
					simHandler.Handle();
				}
			}
			catch (Exception ex)
			{
				Logging.Instance.WriteLine(String.Format("UpdateBeforeSimulation(): {0}", ex.ToString()));
			}
        }

        public override void UpdateAfterSimulation()
        {
        }

        public override void Simulate()
        {
            if (Join != "")
            {
                string join = Join;
                Join = "";
                Logging.Instance.WriteLine(string.Format("Joining: {0}", join));
                MyAPIGateway.Multiplayer.JoinServer(join);
            }
        }

        protected override void UnloadData()
        {
            try
            {
                RemoveMessageHandler();

				if (MyAPIGateway.Session != null)
				{
					if (MyAPIGateway.Utilities.GetObjectiveLine().Visible)
						MyAPIGateway.Utilities.GetObjectiveLine().Hide();
				}


                if (Logging.Instance != null)
                    Logging.Instance.Close();
            }
            catch { }

            base.UnloadData();
        }
    }
}
