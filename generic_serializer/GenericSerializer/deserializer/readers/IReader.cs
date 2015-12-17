using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GenericSerializer.XmlUtils
{
    internal class NodeAttributes : Dictionary<string, string> { }

    public interface INode<T>
    {
        string Tag { get; }
        string NodeText { get; }
        string this[string atttributeName] { get; set; }
        T DocumentOffset { get; }
    }

    public interface IReader<T> : IDisposable
    {
        INode<T> RootObject { get; }
        IEnumerable<INode<T>> GetChildNodes(INode<T> node);
    }
}
