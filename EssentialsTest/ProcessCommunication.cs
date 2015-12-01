using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox.ModAPI;
using Sandbox.Common.ObjectBuilders;
using VRage.ModAPI;
using System.IO;
using Sandbox;

namespace DedicatedEssentials
{
    public class ProcessCommunication : SimulationProcessorBase
    {
		private bool m_init = false;
		private DateTime m_lastRun = DateTime.Now;

        public override void Handle()
        {
			if (MyAPIGateway.Multiplayer.IsServer)
				return;

			if (!m_init)
			{
				m_init = true;
				Init();
			}

			if (DateTime.Now - m_lastRun < TimeSpan.FromMilliseconds(100))
				return;

			m_lastRun = DateTime.Now;
			HashSet<IMyEntity> entities = new HashSet<IMyEntity>();
			MyAPIGateway.Entities.GetEntities(entities, x => x is IMyCubeGrid);
			foreach (IMyEntity entity in entities)
			{
                if (entity.DisplayName.StartsWith("JunkRelay"))
                {
                    MyAPIGateway.Entities.RemoveEntity(entity);
                    continue;
                //sometimes the server doesn't sync deleted grids, so it renames them so client can delete
                }

				if(!(entity.DisplayName.StartsWith(string.Format("CommRelayOutput{0}", MyAPIGateway.Session.Player.PlayerID)) || entity.DisplayName.StartsWith("CommRelayBroadcast")))
					continue;

				IMyCubeGrid grid = (IMyCubeGrid)entity;
				MyObjectBuilder_CubeGrid gridBuilder = (MyObjectBuilder_CubeGrid)grid.GetObjectBuilder();

				foreach (MyObjectBuilder_CubeBlock block in gridBuilder.CubeBlocks)
				{
					if (block is MyObjectBuilder_Beacon)
					{
						MyObjectBuilder_Beacon beacon = (MyObjectBuilder_Beacon)block;
						IMySlimBlock slim = grid.GetCubeBlock(beacon.Min);
						Sandbox.ModAPI.Ingame.IMyTerminalBlock terminal = (Sandbox.ModAPI.Ingame.IMyTerminalBlock)slim.FatBlock;
						//terminal.SetCustomName("");

						ServerCommandHandlers.ProcessCommands(beacon.CustomName.Replace("\r", "").Split(new char[] {'\n'}).ToList());
						break;
					}
				}

				MyAPIGateway.Entities.RemoveEntity(entity);
			}			
        }

		private void Init()
		{
			HashSet<IMyEntity> entities = new HashSet<IMyEntity>();
			Core.ServerCommandList.Clear();
			MyAPIGateway.Entities.GetEntities(entities, x => x is IMyCubeGrid && x.DisplayName.StartsWith(string.Format("CommRelayGlobal")));
			foreach (IMyEntity entity in entities)
			{
				IMyCubeGrid grid = (IMyCubeGrid)entity;
				MyObjectBuilder_CubeGrid gridBuilder = (MyObjectBuilder_CubeGrid)grid.GetObjectBuilder();

				foreach (MyObjectBuilder_CubeBlock block in gridBuilder.CubeBlocks)
				{
					if (block is MyObjectBuilder_Beacon)
					{
						MyObjectBuilder_Beacon beacon = (MyObjectBuilder_Beacon)block;
						foreach (string str in beacon.CustomName.Split(new char[] { '\n' }))
						{
                            if (str[0] == '/')
                                Core.ServerCommandList.Add(str);

                            else
                                ParseGlobal(str);

							//Communication.Message(string.Format("Server Command Found: {0}", str));
						}
                        //if (!beacon.CustomName.ToLower().Contains("v_essential:"))
                        //    ParseGlobal("v_essential:0.0.0.0");

						break;
					}
				}

//				MyAPIGateway.Entities.RemoveEntity(entity);
			}			
		}

		private void ParseGlobal(string data)
		{
			if(data.ToLower().StartsWith("servername:"))
			{
				string[] split = data.Split(new char[] { ':' });
				Core.ServerName = split[1];
			}

            if(data.ToLower().StartsWith("border:"))
            {
                string[] split = data.Split(new char[] { ':' });
                Core.ServerBorder = 0f;
                float val = 0f;
                float.TryParse(split[1], out val);
                Core.ServerBorder = val * 1000f;

                Logging.Instance.WriteLine(string.Format("Border: {0}", Core.ServerBorder));
            }

            /*
            if(data.ToLower().StartsWith("v_essential:"))
            {
                string[] split = data.Split(new char[] { ':' });
                if (!split[1].Contains("0.0.0.0"))
                {
                    try
                    {
                        using (TextReader reader = MyAPIGateway.Utilities.ReadFileInLocalStorage("Version.txt", typeof(ProcessCommunication)))
                        using (TextWriter writer = MyAPIGateway.Utilities.WriteFileInLocalStorage("Version.txt", typeof(ProcessCommunication)))
                        {
                            if (MySandboxGame.ConfigDedicated.Administrators.Any(userId => MyAPIGateway.Session.Player.PlayerID.ToString().Equals(userId)))
                            {
                                string versionLine = reader.ReadLine();

                                if (versionLine ==null )
                                {
                                    writer.Write("essentials_notified&0.0.0.0&");
                                    //notify admin of version update
                                    string message =
                                        "There is an important for SEServerExtender and the Essentials plugin!|" +
                                        "This update fixes a lot of critical issues and crashes. There are a few new features as well!|" +
                                        "Please go to the SE forum at forums.keensoftwarehouse.com then go to the dedicated server subforum in the multiplayer section. " +
                                        "Check the the 'SE Server Extender and Essentials help thread' thread for links to the latest updates.||" +
                                        "This message will only appear once. Happy engineering!";
                                    MyAPIGateway.Utilities.ShowMissionScreen("SESE Essentials plugin", "Important update available!", null, message, null, "close");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Instance.WriteLine(String.Format("Load(): {0}", ex.ToString()));
                    }
                }

            }
            */

		}
    }
}
