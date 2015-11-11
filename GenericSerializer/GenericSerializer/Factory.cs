using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenericSerializer.XmlUtils;
using GenericSerializer.Serializer;
using GenericSerializer.Deserializer;
using System.Xml;

namespace GenericSerializer.Factories
{
    public static class Factory
    {
        public enum eSerializationType
        {
            kXML,
            kJSON,
            kSOAP,
            kBINARY
        }

        public static void Serialize(object instance, eSerializationType serializationType, string outputPath)
        {
            IWriter writer = null; 

            switch (serializationType)
            {
                case eSerializationType.kXML:
                    writer = new XmlWriterWrapper(outputPath);
                    break;
                case eSerializationType.kJSON:
                    break;
                case eSerializationType.kSOAP:
                    break;
                case eSerializationType.kBINARY:
                    break;
                default:
                    break;
            }

            GenericSerializer.Serializer.GenericSerializer.Serialize(instance, writer);
        }

        public static object Deserialize(string inputPath, eSerializationType serializationType)
        {
            object result = null;

            switch (serializationType)
            {
                case eSerializationType.kXML:
                    IReader<XmlNode> reader = new XmlReaderWrapper(inputPath);
                    result = GenericDeserializer<XmlNode>.Deserialize(reader);
                    break;
                case eSerializationType.kJSON:
                    break;
                case eSerializationType.kSOAP:
                    break;
                case eSerializationType.kBINARY:
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}
