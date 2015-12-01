using System;
using System.Collections.Generic;
using Sandbox.ModAPI;
using Sandbox.Common.ObjectBuilders;
using VRageMath;
using VRage.ObjectBuilders;
using VRage;

namespace DedicatedEssentials
{
    static class Communication
    {
		private static Random m_random = new Random();
        static public void Message(String text)
        {
            MyAPIGateway.Utilities.ShowMessage("[Essentials]", text);
        }

		static public void Message(string from, string text, bool brackets = true)
		{
			MyAPIGateway.Utilities.ShowMessage(string.Format("{0}", from), text);
		}

        static public void Notification(String text, int disappearTimeMS = 2000, Sandbox.Common.MyFontEnum fontEnum = Sandbox.Common.MyFontEnum.White)
        {
            MyAPIGateway.Utilities.ShowNotification(text, disappearTimeMS, fontEnum);
        }

		static public void Dialog(string title, string prefix, string current, string description, string buttonText)
		{
			MyAPIGateway.Utilities.ShowMissionScreen(title, prefix, current, description, null, buttonText);
		}

		static public void SendMessageToServer(string text)
		{
			string commRelay = @"<?xml version=""1.0""?>
<MyObjectBuilder_CubeGrid xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <EntityId>0</EntityId>
  <PersistentFlags>CastShadows</PersistentFlags>
  <PositionAndOrientation>
    <Position x=""60"" y=""-5"" z=""-22.5"" />
    <Forward x=""0"" y=""-0"" z=""-1"" />
    <Up x=""0"" y=""1"" z=""0"" />
  </PositionAndOrientation>
  <GridSizeEnum>Large</GridSizeEnum>
  <CubeBlocks>
    <MyObjectBuilder_CubeBlock xsi:type=""MyObjectBuilder_Beacon"">
      <SubtypeName>LargeBlockBeacon</SubtypeName>
      <EntityId>0</EntityId>
      <Min x=""0"" y=""0"" z=""1"" />
      <IntegrityPercent>0.001</IntegrityPercent>
      <BuildPercent>0.001</BuildPercent>
      <BlockOrientation Forward=""Forward"" Up=""Up"" />
      <ColorMaskHSV x=""0"" y=""0"" z=""0"" />
      <ShareMode>All</ShareMode>
      <DeformationRatio>0</DeformationRatio>
      <ShowOnHUD>false</ShowOnHUD>
      <Enabled>false</Enabled>
      <BroadcastRadius>1</BroadcastRadius>
      <CustomName>Testing</CustomName>
    </MyObjectBuilder_CubeBlock>
  </CubeBlocks>
  <IsStatic>true</IsStatic>
  <Skeleton />
  <LinearVelocity x=""0"" y=""0"" z=""0"" />
  <AngularVelocity x=""0"" y=""0"" z=""0"" />
  <XMirroxPlane />
  <YMirroxPlane />
  <ZMirroxPlane />
  <BlockGroups />
  <Handbrake>false</Handbrake>
  <DisplayName>CommRelay</DisplayName>
</MyObjectBuilder_CubeGrid>";

			MyObjectBuilder_CubeGrid cubeGrid = MyAPIGateway.Utilities.SerializeFromXML<MyObjectBuilder_CubeGrid>(commRelay);
			cubeGrid.DisplayName = string.Format("CommRelay{0}", MyAPIGateway.Session.Player.PlayerID);
			foreach (MyObjectBuilder_CubeBlock block in cubeGrid.CubeBlocks)
			{
				if (block is MyObjectBuilder_Beacon)
				{
					MyObjectBuilder_Beacon beacon = (MyObjectBuilder_Beacon)block;
					beacon.CustomName = text;
				}
			}

			/*
			float halfExtent = MyAPIGateway.Entities.WorldSafeHalfExtent();
			if (halfExtent == 0f)
				halfExtent = 900000f;
			*/
			cubeGrid.PositionAndOrientation = new MyPositionAndOrientation(GenerateRandomEdgeVector(), Vector3.Forward, Vector3.Up);
			//List<MyObjectBuilder_EntityBase> addList = new List<MyObjectBuilder_EntityBase>();
			//addList.Add(cubeGrid);
            //MyAPIGateway.Multiplayer.SendEntitiesCreated(addList);		
            MyAPIGateway.Entities.CreateFromObjectBuilderAndAdd(cubeGrid);
		}

        private static Vector3D GenerateRandomEdgeVector()
        {
            float halfExtent = MyAPIGateway.Entities.WorldSafeHalfExtent();
            halfExtent += (halfExtent == 0 ? 900000 : -1000);
            
            int testRadius = 0;

            Vector3D vectorPosition = new Vector3D(GenerateRandomCoord(halfExtent), GenerateRandomCoord(halfExtent), GenerateRandomCoord(halfExtent));
            BoundingSphereD positionSphere = new BoundingSphereD(vectorPosition, 5000);


            
            for (int i = 0; i < 20; ++i)
            {
                if (MyAPIGateway.Entities.GetIntersectionWithSphere(ref positionSphere) != null)
                {
                    vectorPosition = new Vector3D(GenerateRandomCoord(halfExtent), GenerateRandomCoord(halfExtent), GenerateRandomCoord(halfExtent));
                    positionSphere = new BoundingSphereD(vectorPosition, testRadius);
                }
                else
                    return vectorPosition;
            }

            return new Vector3D(halfExtent, 0, 0);

            //if we can't find an acceptable place, put it out on the very edge and hope there's nothing there
        }

        private static float GenerateRandomCoord(float halfExtent)
        {
            float result = (m_random.Next((int)halfExtent)) * (m_random.Next(2) == 0 ? -1 : 1);
            //return a random distance between origin and +/- halfExtent
            return result;

        }

        /// <summary>
        /// This is kind of shitty.  I should just bitshift and copy the lengths, but whatever.  We're not sending
        /// this enough to really care
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="text"></param>
		public static void SendDataToServer(long dataId, string text)
		{
			string msgIdString = dataId.ToString();
            string steamIdString = MyAPIGateway.Session.Player.SteamUserId.ToString();

			byte[] data = System.Text.Encoding.ASCII.GetBytes(text);
			byte[] newData = new byte[text.Length + 1 + msgIdString.Length + steamIdString.Length + 1];

            int pos = 0;
			newData[pos] = (byte)msgIdString.Length;
            for (int r = 0; r < msgIdString.Length; r++)
            {
                pos++;
                newData[pos] = (byte)msgIdString[r];
            }
            pos++;

            newData[pos] = (byte)steamIdString.Length;
            for (int r = 0; r < steamIdString.Length; r++ )
            {
                pos++;
                newData[pos] = (byte)steamIdString[r];
            }
            pos++;

            Array.Copy(data, 0, newData, pos, data.Length);
			MyAPIGateway.Multiplayer.SendMessageToServer(9001, newData);
		}
    }
}
