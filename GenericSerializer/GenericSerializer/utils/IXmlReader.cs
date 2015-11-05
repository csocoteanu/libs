using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericSerializer.XmlUtils
{
    internal interface IXmlReader : IDisposable
    {
        XmlNodeInfo RootObject { get; }
    }
}
