using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

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
}