using GenericSerializer.XmlUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GenericSerializer.Serializer
{
    internal class XmlWriterWrapper : IXmlWriter
    {
        private string m_outputPath;
        private XmlWriter m_xmlWriter;
        private XmlWriterSettings m_settings;

        public XmlWriterWrapper(string outputPath)
        {
            this.m_outputPath = outputPath;

            this.InitXmlWriter();
            this.WriteHeader();
        }

        private void InitXmlWriter()
        {
            this.m_settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };

            this.m_xmlWriter = XmlWriter.Create(this.m_outputPath, this.m_settings);
        }

        private void WriteHeader()
        {
            this.m_xmlWriter.WriteStartDocument();
        }

        private void WriteAttributes(params string[] attributes)
        {
            for (int i = 0; i + 1 < attributes.Length; i += 2)
            {
                string attributeName = attributes[i];
                string attributeValue = attributes[i + 1];
                if (!string.IsNullOrEmpty(attributeName) && !string.IsNullOrEmpty(attributeValue))
                {
                    this.m_xmlWriter.WriteAttributeString(attributeName, attributeValue);
                }
            }
        }

        #region IXmlWriter Members

        public void WriteEndElement()
        {
            this.m_xmlWriter.WriteEndElement();
        }

        public void WriteStartElement(string element, params string[] attributes)
        {
            this.m_xmlWriter.WriteStartElement(element);
            this.WriteAttributes(attributes);
        }

        public void WriteElementString(string element, string elementValue, params string[] attributes)
        {
            this.WriteStartElement(element, attributes);
            char[] arr = elementValue.ToCharArray();
            this.m_xmlWriter.WriteChars(arr, 0, arr.Length);

            this.WriteEndElement();
        }

        public void CloseDocument()
        {
            this.m_xmlWriter.WriteEndDocument();
            this.m_xmlWriter.Close();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.m_xmlWriter.Close();
        }

        #endregion
    }
}
