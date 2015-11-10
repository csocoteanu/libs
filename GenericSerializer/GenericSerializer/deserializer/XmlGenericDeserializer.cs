using GenericSerializer.XmlUtils;
using GenericSerializer.XmlUtils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GenericSerializer.Deserializer
{
    internal class XmlGenericDeserializer<T> : IDisposable
    {
        protected IXmlReader<T> m_reader;

        protected XmlGenericDeserializer(IXmlReader<T> reader)
        {
            this.m_reader = reader;
        }

        protected object Deserialize(IXmlNode<T> root)
        {
            object instance = root.CreateDefaultInstance();

            if (instance != null)
            {
                foreach (IXmlNode<T> node in this.m_reader.GetChildNodes(root))
                {
                    object memberValue = (node.IsCompositeNode()) ? this.Deserialize(node) : node.NodeText;
                    instance.SetMemberValue(node.Tag, memberValue);
                } 
            }

            return instance;
        }

        internal object Deserialize()
        {
            IXmlNode<T> rootNodeInfo = this.m_reader.RootObject;
            return this.Deserialize(rootNodeInfo);
        }

        internal static object Deserialize(IXmlReader<T> reader)
        {
            object result = null;
            using (XmlGenericDeserializer<T> xmlDeserializer = new XmlGenericDeserializer<T>(reader))
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
