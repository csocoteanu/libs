using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GenericSerializer.XmlUtils
{
    internal class XmlAttributes : Dictionary<string, string> { }

    internal class XmlNodeInfo
    {
        private string m_xmlTag;
        private string m_xmlText;
        private XmlAttributes m_attributes;
        private XmlNodeList m_nodeList;

        internal XmlNodeInfo(string tag, string text, XmlNodeList nodeList)
        {
            this.m_xmlTag = tag;
            this.m_xmlText = text;
            this.m_attributes = new XmlAttributes();
            this.m_nodeList = nodeList;
        }

        internal string XmlTag { get { return m_xmlTag; } }
        internal string XmlText { get { return m_xmlText; } }

        public IEnumerable<XmlNodeInfo> Children
        {
            get
            {
                foreach (XmlNode child in this.m_nodeList)
                {
                    yield return child.ToXmlNodeInfo();
                }
            }
        }

        internal string this[string attributeName]
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

        internal object CreateDefaultInstance()
        {
            throw new NotImplementedException();
        }

        internal void SetMemberValue(object instance, object memberValue)
        {
            throw new NotImplementedException();
        }


    }
}
