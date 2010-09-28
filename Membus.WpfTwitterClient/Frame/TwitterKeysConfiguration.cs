using System.Configuration;
using System.Xml;
using Membus.WpfTwitterClient.Frame.Twitter;

namespace Membus.WpfTwitterClient.Frame
{
    public class TwitterKeysConfiguration : XmlSerializedConfigurationSection<TwitterKeys>, IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            return create(parent, configContext, section);
        }
    }
}