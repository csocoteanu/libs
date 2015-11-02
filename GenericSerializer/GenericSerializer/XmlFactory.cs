using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenericSerializer.XmlUtils;
using GenericSerializer.Serializer;
using GenericSerializer.Deserializer;

namespace GenericSerializer.Factory
{
    public static class XmlFactory
    {
        public static void Serialize(object instance, string outputPath)
        {
            IXmlWriter writer = new XmlWriterWrapper(outputPath);
            XmlGenericSerializer.Serialize(instance, writer);
        }

        public static object Deserialize(string inputPath)
        {
            IXmlReader reader = new XmlReaderWrapper(inputPath);
            return XmlGenericDeserializer.Deserialize(reader);
        }
    }
}
