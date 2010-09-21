using System;
using System.Configuration;
using System.Xml.Serialization;
using MemBus.Support;

namespace Membus.Tests.WpfClient.Frame
{
    public interface IConfigReader
    {
        T GetSection<T>(string name);
        T GetSection<T>();
    }

    public class AppConfigReader : IConfigReader
    {
        
        public T GetSection<T>(string name)
        {
            return (T)ConfigurationManager.GetSection("{0}".Fmt(name));
        }

        public T GetSection<T>()
        {
            string sectionName = null;
            if (typeof (T).HasAttribute<XmlRootAttribute>())
            {
                var t = typeof (T).GetAttribute<XmlRootAttribute>().ElementName;
                if (!string.IsNullOrEmpty(t))
                    sectionName = t;
            }

            sectionName = sectionName ?? typeof (T).Name; 

            return GetSection<T>(sectionName);
        }
    }
}