using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericSerializer.XmlUtils
{
    internal interface IXmlWriter : IDisposable
    {
        void WriteEndElement();
        void WriteStartElement(string element, params string[] attributes);
        void WriteElementString(string element, string elementValue, params string[] atttributes);
        void CloseDocument();
    }
}
