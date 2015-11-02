
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericSerializer.XmlUtils
{
    public enum eXmlItem
    {
        kNode,
        kNodeAttribute,
        kNodeText,
    }

    public class XmlParsedEventArgs : EventArgs
    {
        private eXmlItem m_xmlItem;
        private string[] m_values;

        public eXmlItem XmlItem { get { return m_xmlItem; } }
        public string[] Values { get { return m_values; } }

        public XmlParsedEventArgs(eXmlItem xmlItem, string[] values)
        {
            this.m_xmlItem = xmlItem;
            this.m_values = values;
        }
    }
}
