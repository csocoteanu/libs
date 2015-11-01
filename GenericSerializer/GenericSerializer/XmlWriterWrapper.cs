using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GenericSerializer.XmlUtils
{
    internal static class Utils
    {
        internal const string kNullString = "null";
    }

    internal interface IXmlWriter : IDisposable
    {
        void WriteEndElement();
        void WriteStartElement(string element);
        void WriteElementString(string element, string elementValue);
        void CloseDocument();
    }

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

        #region IXmlWriter Members

        public void WriteEndElement()
        {
            this.m_xmlWriter.WriteEndElement();
        }

        public void WriteStartElement(string element)
        {
            this.m_xmlWriter.WriteStartElement(element);
        }

        public void WriteElementString(string element, string elementValue)
        {
            this.m_xmlWriter.WriteElementString(element, elementValue);
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
