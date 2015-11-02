using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericSerializer.XmlUtils
{
    internal interface IXmlWriter : IDisposable
    {
        void WriteEndElement();
        void WriteStartElement(string element, string attributeName, string attributeValue);
        void WriteElementString(string element, string elementValue);
        void CloseDocument();
    }
}
