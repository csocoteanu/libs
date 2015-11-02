using GenericSerializer.XmlUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericSerializer.Deserializer
{
    internal class XmlReaderWrapper : IXmlReader
    {
        private string m_inputPath;

        public XmlReaderWrapper(string inputPath)
        {
            this.m_inputPath = inputPath;
        }

        #region IDisposable
        public void Dispose()
        {

        } 
        #endregion
    }
}
