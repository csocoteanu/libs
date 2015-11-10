using GenericSerializer.XmlUtils;
using GenericSerializer.XmlUtils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GenericSerializer.Deserializer
{
    internal class XmlReaderWrapper : IXmlReader<XmlNode>
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

        public IXmlNode<XmlNode> RootObject
        {
            get
            {
                if (this.m_xmlDocument.ChildNodes == null ||
                    this.m_xmlDocument.ChildNodes.Count < 1)
                {
                    return null;
                }

                return this.m_xmlDocument.ChildNodes[1].ToXmlNodeInfo();
            }
        }
        #endregion


        public IEnumerable<IXmlNode<XmlNode>> GetChildNodes(IXmlNode<XmlNode> node)
        {
            foreach (XmlNode domChild in node.DomOffset.ChildNodes)
            {
                string textNode = domChild.GetImmediateNodeText();
                yield return domChild.ToXmlNodeInfo(textNode);
            }
        }
    }
}
