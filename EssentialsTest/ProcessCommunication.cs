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
using VRage.Game;
using VRage.Game.ModAPI;

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
			EssentialsCore.ServerCommandList.Clear();
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
                                EssentialsCore.ServerCommandList.Add(str);

                            else
                                ParseGlobal(str);

							//Communication.Message(string.Format("Server Command Found: {0}", str));
						}

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
				EssentialsCore.ServerName = split[1];
			}

            if(data.ToLower().StartsWith("border:"))
            {
                string[] split = data.Split(new char[] { ':' });
                EssentialsCore.ServerBorder = 0f;
                float val = 0f;
                float.TryParse(split[1], out val);
                EssentialsCore.ServerBorder = val * 1000f;

                Logging.Instance.WriteLine(string.Format("Border: {0}", EssentialsCore.ServerBorder));
            }
		}
    }
}
