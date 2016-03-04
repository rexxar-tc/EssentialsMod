using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox.Common;
using Sandbox.ModAPI;
using Sandbox.Common.ObjectBuilders;

using VRageMath;
using VRage;
using VRage.Game;
using VRage.ModAPI;

namespace DedicatedEssentials
{
	public class ServerCommandConceal : ServerCommandHandlerBase
	{
		private Random m_random = new Random();
		public override string GetCommandText()
		{
			return "/conceal";
		}

		public override void HandleCommand(string[] words)
		{
			try
			{
				Logging.Instance.WriteLine(string.Format("Conceal: {0}", string.Join(" ", words)));
				if (words.Length > 0)
				{
					string[] grids = string.Join(" ", words).Split(new char[] { ',' });
					List<long> entities = new List<long>();

					foreach (string grid in grids)
					{
						string[] items = grid.Split(new char[] { ':' }, 3);

						long entityId = 0;
						if (long.TryParse(items[0], out entityId))
						{
							//entities.Add(entityId);
							ConcealGrid(entityId, items[1], items[2]);
						}
					}

					//if (entities.Count > 0)
					//	CheckGrid(entities);
				}
			}
			catch (Exception ex)
			{
				Logging.Instance.WriteLine(string.Format("HandleNotification(): {0}", ex.ToString()));
			}

			base.HandleCommand(words);
		}

		private void ConcealGrid(long entityId, string blockCountStr, string entityDisplayName)
		{
			HashSet<IMyEntity> entities = new HashSet<IMyEntity>();
			MyAPIGateway.Entities.GetEntities(entities);

			foreach (IMyEntity entity in entities)
			{
				if(!(entity is IMyCubeGrid))
					continue;

				if(entity.EntityId == entityId)
				{
					/*
					Logging.Instance.WriteLine(string.Format("Found grid: {0} - Removing", entityId));

					if(entity.Physics != null)
					{
						entity.Physics.AngularVelocity = Vector3.Zero;
						entity.Physics.LinearVelocity = Vector3.Zero;
					}

					MyAPIGateway.Entities.RemoveEntity(entity);
					 */
 
					return;
				}
			}

			Logging.Instance.WriteLine(string.Format("Unable to locate ship: {0}", entityId));

			foreach (IMyEntity entity in entities)
			{
				if (!(entity is IMyCubeGrid))
					continue;

				IMyCubeGrid grid = (IMyCubeGrid)entity;
				long playerId = MyAPIGateway.Session.Player.PlayerID;
				if (grid.BigOwners.Contains(playerId) || grid.SmallOwners.Contains(playerId))
				{
					//Logging.Instance.WriteLine(string.Format("Found: {0} - {1}", grid.EntityId, grid.DisplayName));
					int blockCount = 0;
					if (int.TryParse(blockCountStr, out blockCount))
					{
						MyObjectBuilder_CubeGrid gridBuilder = (MyObjectBuilder_CubeGrid)entity.GetObjectBuilder();
						if (gridBuilder.CubeBlocks.Count == blockCount && entity.DisplayName == entityDisplayName)
						{
							Logging.Instance.WriteLine(string.Format("Remove: {0}:{1}:{2} - {3}:{4}:{5}", entityId, blockCountStr, entityDisplayName, grid.EntityId, gridBuilder.CubeBlocks.Count, gridBuilder.DisplayName));
							Communication.Message(string.Format("Found entity desync, refreshing: {0}", entity.DisplayName));

							if (entity.Physics != null)
							{
								entity.Physics.LinearVelocity = Vector3.Zero;
								entity.Physics.AngularVelocity = Vector3.Zero;
								entity.Physics.Enabled = false;
							}
							else
							{
								Logging.Instance.WriteLine("Physics is null");
							}

							MyAPIGateway.Entities.RemoveEntity(entity);
							return;
						}
					}
				}
			}

			Logging.Instance.WriteLine(string.Format("Unable to locate ship: {0} - {1} - {2}", entityId, entityDisplayName, blockCountStr));

				/*
			IMyEntity entity;
			if (MyAPIGateway.Entities.TryGetEntityById(entityId, out entity))
			{
				//				if (entity.InScene)
				//				{
				//MyObjectBuilder_CubeGrid grid = (MyObjectBuilder_CubeGrid)entity.GetObjectBuilder();
				//Communication.Message(string.Format("Removing: {0} - {1}", entity.EntityId, entity.DisplayName));
				Logging.Instance.WriteLine(string.Format("Removing: {0} - {1}", entity.EntityId, entity.DisplayName));

				if (entity.Physics != null)
				{
					double linear = Math.Round(((Vector3)entity.Physics.LinearVelocity).LengthSquared(), 1);
					double angular = Math.Round(((Vector3)entity.Physics.AngularVelocity).LengthSquared(), 1);
					if (linear > 0 || angular > 0)
					{
						entity.Physics.LinearVelocity = Vector3.Zero;
						entity.Physics.AngularVelocity = Vector3.Zero;
					}
				}
				
				entity.SetPosition(new Vector3D(50000000, 50000000, 50000000));
				

				Logging.Instance.WriteLine(string.Format("Moving: {0} - {1}", entity.EntityId, entity.DisplayName));
				if (entity.Physics != null)
				{
					entity.Physics.Enabled = false;
					entity.PositionComp.SetPosition(new Vector3D(100000000 + m_random.Next(1000000), m_random.Next(1000000), m_random.Next(1000000)));
				}
				else
				{
					Logging.Instance.WriteLine("Physics null here too");
				}
				
				//MyAPIGateway.Entities.RemoveFromClosedEntities(entity);
				//grid.PersistentFlags = MyPersistentEntityFlags2.None;
				//MyAPIGateway.Entities.CreateFromObjectBuilderAndAdd(grid);
				//Logging.Instance.WriteLine(string.Format("Concealing Grid: {0}", entity.EntityId));
				//				}

				//entity.InScene = false;
				//entity.CastShadows = false;
				//entity.Visible = false;
			}
			else
			{
				Communication.Message(string.Format("Could not find: {0}", entityId));

				HashSet<IMyEntity> entities = new HashSet<IMyEntity>();
				MyAPIGateway.Entities.GetEntities(entities);
				foreach (IMyEntity testEntity in entities)
				{
					if (!(testEntity is IMyCubeGrid))
						continue;

					IMyCubeGrid grid = (IMyCubeGrid)testEntity;
					long playerId = MyAPIGateway.Session.Player.PlayerID;
					if (grid.BigOwners.Contains(playerId) || grid.SmallOwners.Contains(playerId))
					{
						//Logging.Instance.WriteLine(string.Format("Found: {0} - {1}", grid.EntityId, grid.DisplayName));
						int blockCount = 0;
						if(int.TryParse(blockCountStr, out blockCount))
						{
							MyObjectBuilder_CubeGrid gridBuilder = (MyObjectBuilder_CubeGrid)testEntity.GetObjectBuilder();
							if (gridBuilder.CubeBlocks.Count == blockCount && testEntity.DisplayName == entityDisplayName && entityId != grid.EntityId)
							{
								Logging.Instance.WriteLine(string.Format("Remove: {0}:{1}:{2} - {3}:{4}:{5}", entityId, blockCountStr, entityDisplayName, grid.EntityId, gridBuilder.CubeBlocks.Count, gridBuilder.DisplayName));
								Communication.Message(string.Format("Found entity desync, refreshing: {0}", testEntity.DisplayName));

								if (testEntity.Physics != null)
								{
									double linear = Math.Round(((Vector3)testEntity.Physics.LinearVelocity).LengthSquared(), 1);
									double angular = Math.Round(((Vector3)testEntity.Physics.AngularVelocity).LengthSquared(), 1);

									if (linear > 0 || angular > 0)
									{
										testEntity.Physics.LinearVelocity = Vector3.Zero;
										testEntity.Physics.AngularVelocity = Vector3.Zero;
									}

									testEntity.Physics.Enabled = false;
									Logging.Instance.WriteLine("Moving ship");
									testEntity.PositionComp.SetPosition(new Vector3D(100000000 + m_random.Next(1000000), m_random.Next(1000000), m_random.Next(1000000)));
								}
								else
								{
									Logging.Instance.WriteLine("Physics is null");
								}

								/*
								// So removing is crashing.  I dunno, move the stupid thing out of world borders and hope that works.
								try
								{
									testEntity.SetPosition(new Vector3D(50000000, 50000000, 50000000));
								}
								catch
								{
									try
									{
										testEntity.PositionComp.SetPosition(new Vector3D(50000000, 50000000, 50000000));
									}
									catch
									{
										try
										{
											testEntity.PositionComp.Scale = 0;
											testEntity.Physics.Enabled = false;
										}
										catch
										{
											try
											{
												testEntity.Physics.Enabled = false;
											}
											catch
											{
												Communication.Message("Nothing Works!");
											}
										}
									}
								}

								//MyAPIGateway.Entities.RemoveEntity(testEntity);
								//MyAPIGateway.Entities.RemoveFromClosedEntities(entity);
							}
							else
							{
								Communication.Message(string.Format("Count not find but found anyway????: {0}", entityId));

								if (testEntity.Physics != null)
								{
									double linear = Math.Round(((Vector3)testEntity.Physics.LinearVelocity).LengthSquared(), 1);
									double angular = Math.Round(((Vector3)testEntity.Physics.AngularVelocity).LengthSquared(), 1);

									if (linear > 0 || angular > 0)
									{
										testEntity.Physics.LinearVelocity = Vector3.Zero;
										testEntity.Physics.AngularVelocity = Vector3.Zero;
									}

									testEntity.Physics.Enabled = false;
									Logging.Instance.WriteLine("Moving a ship");
									testEntity.PositionComp.SetPosition(new Vector3D(100000000 + m_random.Next(1000000), m_random.Next(1000000), m_random.Next(1000000)));
								}
							}
						}
					}
				}
			}
				 * */
		}

		private void CheckGrid(List<long> entities)
		{
			HashSet<IMyEntity> testEntities = new HashSet<IMyEntity>();
			List<long> compareList = new List<long>();
			MyAPIGateway.Entities.GetEntities(testEntities);

			foreach (IMyEntity entity in testEntities)
			{
				if (!(entity is IMyCubeGrid))
					continue;

				if (!entity.InScene)
					continue;

				compareList.Add(entity.EntityId);
			}

			Communication.Message(string.Format("Compare: {0} - {1}", compareList.Count(), entities.Count()));
			List<long> difference = compareList.Except(entities).ToList();

			foreach (IMyEntity entity in testEntities)
			{
				if (difference.Contains(entity.EntityId))
				{
					Communication.Message(string.Format("{0} - {1}", entity.EntityId, entity.DisplayName));
				}
			}
		}
	}
}
