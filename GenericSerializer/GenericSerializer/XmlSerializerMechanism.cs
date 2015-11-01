using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenericSerializer.XmlUtils;

namespace GenericSerializer
{
    public static class XmlSerializerFactory
    {
        public static void Serialize(object instance, string outputPath)
        {
            IXmlWriter writer = new XmlWriterWrapper(outputPath);
            XmlGenericSerializer.Serialize(instance, writer);
        }

        public static object Deserialize(string intputPath)
        {
            return null;
        }
    }
}
