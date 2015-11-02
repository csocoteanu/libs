using GenericSerializer.XmlUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericSerializer.Deserializer
{
    internal class XmlGenericDeserializer : IDisposable
    {
        protected IXmlReader m_reader;

        protected XmlGenericDeserializer(IXmlReader reader)
        {
            this.m_reader = reader;
        }

        internal object Deserialize()
        {
            return null;
        }

        internal static object Deserialize(IXmlReader reader)
        {
            object result = null;
            using (XmlGenericDeserializer xmlDeserializer = new XmlGenericDeserializer(reader))
                result = xmlDeserializer.Deserialize();

            return result;
        }

        #region IDisposable
        public void Dispose()
        {
            if (this.m_reader != null)
            {
                this.m_reader.Dispose();
            }
        } 
        #endregion
    }
}
