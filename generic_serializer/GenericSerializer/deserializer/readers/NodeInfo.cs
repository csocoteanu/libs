using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GenericSerializer.XmlUtils
{

    internal class NodeInfo<T> : INode<T>
    {
        private string m_tag;
        private string m_text;
        private T m_node;
        private NodeAttributes m_attributes;

        internal NodeInfo(string tag, string text, T node)
        {
            this.m_tag = tag;
            this.m_text = text;
            this.m_node = node;
            this.m_attributes = new NodeAttributes();
        }

        #region IXmlNode
        public string Tag { get { return m_tag; } }
        public string NodeText { get { return m_text; } }
        public T DocumentOffset { get { return this.m_node; } }

        public string this[string attributeName]
        {
            get
            {
                string attributeValue = null;
                this.m_attributes.TryGetValue(attributeName, out attributeValue);

                return attributeValue;
            }
            set
            {
                this.m_attributes[attributeName] = value;
            }
        }
        #endregion
    }
}
