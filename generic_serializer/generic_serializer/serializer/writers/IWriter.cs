using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericSerializer.XmlUtils
{
    public interface IWriter : IDisposable
    {
        void WriteEndElement();
        void WriteStartElement(string element, params string[] attributes);
        void WriteElementString(string element, string elementValue, params string[] atttributes);
        void CloseDocument();
    }
}
