using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GenericSerializer.XmlUtils
{
    internal class XmlAttributes : Dictionary<string, string> { }

    internal interface IXmlNode<T>
    {
        string Tag { get; }
        string NodeText { get; }
        string this[string atttributeName] { get; set; }
        T DomOffset { get; }
    }

    internal interface IXmlReader<T> : IDisposable
    {
        IXmlNode<T> RootObject { get; }
        IEnumerable<IXmlNode<T>> GetChildNodes(IXmlNode<T> node);
    }
}
