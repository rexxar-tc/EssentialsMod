using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EssentialsTest;
using Sandbox.ModAPI;
using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Definitions;

using VRageMath;

namespace DedicatedEssentials
{
	public class ProcessLogin : SimulationProcessorBase
	{
        private bool m_ran;
		public override void Handle()
		{
			if (MyAPIGateway.Session == null || MyAPIGateway.Session.Player == null)
				return;

            if(!m_ran)
            {
                m_ran = true;
                Communication.SendDataToServer( 5011, "Login" );
                /*
                if ( !MyAPIGateway.Session.LocalHumanPlayer.IsAdmin || AlertLog.Instance.SurveyAlert )
                    return;

                Logging.Instance.WriteLine( AlertLog.Instance.SurveyAlert.ToString() );

                string longMessage =
@"Hello space engineer!

I'd greatly appreciate it if you took a few minutes to fill out this survey.
It asks whether you'll be using the dev or stable branch, as well as some general questions about how you use SESE.

Things are changing, and I have to make some hard decisions about the development of SESE.
This survey will help me get a better understanding of how people use SESE and what you need from it.

The survey is here: http://bit.do/sesurvey and there is a discussion on the KSH multiplayer forum.

This message is only shown to admins and will be shown only once.

<3 rexxar
";

                Communication.Dialog( "SESE Survey", "", "", longMessage, "close" );
                AlertLog.Instance.SurveyAlert = true;
                AlertLog.Instance.Save();*/
            }
		}
	}
}
