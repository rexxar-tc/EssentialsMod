using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DedicatedEssentials;
using Sandbox.ModAPI;

namespace EssentialsTest
{
    public class AlertLog
    {
        private static AlertLog _instance;
        private bool _surveyAlert;
        public static AlertLog Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = new AlertLog();
                _instance.Load();
                return _instance;
            }
        }

        public bool SurveyAlert
        {
            get
            {
                return _surveyAlert;
            }
            set
            {
                _surveyAlert = value;
            }
        }
        

        public AlertLog()
        {
            _surveyAlert = false;
        }

        public void Save()
        {
            Logging.Instance.WriteLine("Saving settings");
            var writer = MyAPIGateway.Utilities.WriteFileInLocalStorage("AlertLog.xml", typeof(AlertLog));
            writer.Write(MyAPIGateway.Utilities.SerializeToXML(_instance));
            writer.Flush();
            writer.Close();
            Logging.Instance.WriteLine("Done saving settings");
        }

        public void Load()
        {
            Logging.Instance.WriteLine("Loading settings");
            try
            {
                if (MyAPIGateway.Utilities.FileExistsInLocalStorage("AlertLog.xml", typeof(AlertLog)))
                {
                    var reader = MyAPIGateway.Utilities.ReadFileInLocalStorage("AlertLog.xml", typeof(AlertLog));
                    var xmlText = reader.ReadToEnd();
                    reader.Close();
                    _instance = MyAPIGateway.Utilities.SerializeFromXML<AlertLog>(xmlText);
                    Logging.Instance.WriteLine("Done loading settings");
                }
            }
            catch (Exception ex)
            {
                Logging.Instance.WriteLine("Error loading settings: " + ex);
            }
        }
    }
}
