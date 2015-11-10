using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GenericSerializer.XmlUtils
{

    internal class XmlNodeInfo<T> : IXmlNode<T>
    {
        private string m_tag;
        private string m_text;
        private T m_domNode;
        private XmlAttributes m_attributes;

        internal XmlNodeInfo(string tag, string text, T domNode)
        {
            this.m_tag = tag;
            this.m_text = text;
            this.m_domNode = domNode;
            this.m_attributes = new XmlAttributes();
        }

        #region IXmlNode
        public string Tag { get { return m_tag; } }
        public string NodeText { get { return m_text; } }
        public T DomOffset { get { return this.m_domNode; } }

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
