using System;
using System.IO;
using System.Xml.Serialization;

namespace LoungeChair.Config
{
    [Serializable]
    public class Configuration
    {
        // Internals
        private static XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
        public static string CONFIGURATION_FILE_NAME = "LoungeChair-Configuration.xml";
        public static Configuration currentConfig;

        // XML properties
        public string session_token;

        public static void Load()
        {
            if (!File.Exists(CONFIGURATION_FILE_NAME))
            {
                currentConfig = new Configuration();
                currentConfig.session_token = "not-logged-in";

                Save();
            }
            else
            {
                using (FileStream stream = File.OpenRead(CONFIGURATION_FILE_NAME))
                {
                    currentConfig = (Configuration)serializer.Deserialize(stream);
                }
            }
        }

        public static void Save()
        {
            File.Delete(CONFIGURATION_FILE_NAME);
            using (FileStream writer = File.OpenWrite(CONFIGURATION_FILE_NAME))
            {
                serializer.Serialize(writer, currentConfig);
            }
        }
    }
}
