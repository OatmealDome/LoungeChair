using System;
using System.IO;
using System.Net;
using System.Xml.Serialization;

namespace LoungeChair
{
    public class UpdateInfo
    {
        [XmlIgnore]
        public Version _version;

        public string latestVersion
        {
            get
            {
                return _version.ToString();
            }
            set
            {
                _version = new Version(value);
            }
        }

        public string changes;
    }

    class UpdateChecker
    {
        private static XmlSerializer serializer = new XmlSerializer(typeof(UpdateInfo));

        public static UpdateInfo CheckForUpdate()
        {
            string xml;
            using (var client = new WebClient())
            {
#if DEBUG
                xml = client.DownloadString("https://oatmealdome.github.io/LoungeChair/UpdateInfo-debug.xml");
                //xml = client.DownloadString("https://oatmealdome.me/LoungeChair/update-debug.xml");
#else
                xml = client.DownloadString("https://oatmealdome.github.io/LoungeChair/UpdateInfo.xml");
#endif
            }

            UpdateInfo updateInfo;
            using (StringReader reader = new StringReader(xml))
            {
                updateInfo = (UpdateInfo)serializer.Deserialize(reader);
            }

            return updateInfo;
        }

    }
}
