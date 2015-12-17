using GenericSerializer.XmlUtils;
using GenericSerializer.XmlUtils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GenericSerializer.Deserializer
{
    public class GenericDeserializer<T> : IDisposable
    {
        protected IReader<T> m_reader;

        protected GenericDeserializer(IReader<T> reader)
        {
            this.m_reader = reader;
        }

        protected object Deserialize(INode<T> root)
        {
            object instance = root.CreateDefaultInstance();

            if (instance != null)
            {
                foreach (INode<T> node in this.m_reader.GetChildNodes(root))
                {
                    object memberValue = (node.IsCompositeNode()) ? this.Deserialize(node) : node.NodeText;
                    instance.SetMemberValue(node.Tag, memberValue);
                } 
            }

            return instance;
        }

        internal object Deserialize()
        {
            INode<T> rootNodeInfo = this.m_reader.RootObject;
            return this.Deserialize(rootNodeInfo);
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

        public static object Deserialize(IReader<T> reader)
        {
            object result = null;
            using (GenericDeserializer<T> xmlDeserializer = new GenericDeserializer<T>(reader))
                result = xmlDeserializer.Deserialize();

            return result;
        }
    }
}
