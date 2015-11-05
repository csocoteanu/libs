using GenericSerializer.XmlUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GenericSerializer.Deserializer
{
    internal class XmlReaderWrapper : IXmlReader
    {
        private string m_inputPath;
        private XmlDocument m_xmlDocument;

        public XmlReaderWrapper(string inputPath)
        {
            this.m_inputPath = inputPath;
            this.m_xmlDocument = new XmlDocument();

            this.m_xmlDocument.Load(this.m_inputPath);
        }

        #region IXmlReader
        public void Dispose() { }

        public XmlNodeInfo RootObject
        {
            get { return this.m_xmlDocument.ChildNodes[1].ToXmlNodeInfo(); }
        }
        #endregion
    }
}
