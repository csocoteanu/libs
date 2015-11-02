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
        private XmlReader m_reader;

        public delegate void OnXmlNodeParsed(object sender, XmlParsedEventArgs args);
        public OnXmlNodeParsed OnNodeParsed;

        public XmlReaderWrapper(string inputPath)
        {
            this.m_inputPath = inputPath;
            m_reader = XmlReader.Create(inputPath);
        }

        public void ReadFully()
        {
            while (m_reader.Read())
            {
                switch (m_reader.NodeType)
                {
                    case XmlNodeType.Attribute:
                        break;
                    case XmlNodeType.Element:
                        break;
                    case XmlNodeType.EndElement:
                        if (this.OnNodeParsed != null)
                            this.OnNodeParsed(this, new XmlParsedEventArgs(eXmlItem.kNode, null));
                        break;
                    case XmlNodeType.XmlDeclaration:
                    case XmlNodeType.ProcessingInstruction:
                        break;
                    case XmlNodeType.Text:
                        break;
                    default:
                        break;
                }
            }
        }

        #region IDisposable
        public void Dispose()
        {
            if (this.m_reader != null)
            {
                this.m_reader.Close();
            }
        }
        #endregion
    }
}
