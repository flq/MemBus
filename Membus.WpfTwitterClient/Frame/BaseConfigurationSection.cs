using System;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Membus.WpfTwitterClient.Frame.Twitter;

namespace Membus.WpfTwitterClient.Frame
{

    public class XmlSerializedConfigurationSection<T>
    {
        protected T create(object parent, object configContext, XmlNode section)
        {
            var sr = new StringReader(section.OuterXml);
            var s = new XmlSerializer(typeof(T));
            return (T)s.Deserialize(sr);
        }
    }

    public class TwitterKeysConfiguration : XmlSerializedConfigurationSection<TwitterKeys>, IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            return create(parent, configContext, section);
        }
    }
}