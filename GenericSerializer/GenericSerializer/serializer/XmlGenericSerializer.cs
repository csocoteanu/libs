using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;
using GenericSerializer.XmlUtils;

namespace GenericSerializer.Serializer
{
    internal class XmlGenericSerializer : IDisposable
    {
        protected int m_depth;
        protected IXmlWriter m_writer;

        protected XmlGenericSerializer(IXmlWriter writer)
        {
            this.m_depth = 0;
            this.m_writer = writer;
        }

        /// <summary>
        /// Serializes a root object
        /// </summary>
        /// <param name="instance"></param>
        internal void Serialize(object instance, Type instanceType=null)
        {
            instanceType = instanceType ?? instance.GetType();
            string instanceTypeString = instanceType.FullName;
            string compositeType = instanceType.GetCompositeType(); // composityType is used for differentiating compund types
                                                                    // as struct(s) and class(es)

            if (instance != null)
            {
                this.m_depth++;

                this.m_writer.WriteStartElement(instanceTypeString, Constants.kComposyteType, compositeType);
                this.SerializeProperties(instance, compositeType);
                this.m_writer.WriteEndElement();
            }
            else
            {
                this.m_writer.WriteElementString(instanceTypeString, instanceType.GetCompoundEmptyValueAsString());
            }
        }

        /// <summary>
        /// Serializes the properties of a compund type
        /// struct or class
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="compositeType"></param>
        protected void SerializeProperties(object instance, string compositeType)
        {
            Type instanceType = instance.GetType();
            PropertyInfo[] properties = instanceType.GetProperties();

            foreach (var property in properties)
            {
                if (!property.IsPropertySerializable())
                    continue;

                if (property.IsPrimitiveType())
                {
                    // this is a primitive data type
                    // e.g.: int, double and string
                    this.SerializePrimitiveType(instance, property);
                }
                else
                {
                    // this is either a class or a struct
                    object propertyValue = property.GetValue(instance, null);
                    this.Serialize(propertyValue, property.PropertyType);
                }
            }
        }

        /// <summary>
        /// Serializes a primitive data type
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="property"></param>
        protected void SerializePrimitiveType(object instance, PropertyInfo property)
        {
            object value = property.GetValue(instance, null);
            string valueString = (value != null) ? value.ToString() : Constants.kNullString;

            this.m_writer.WriteElementString(property.Name, valueString);
        }

        internal static void Serialize(object instance, IXmlWriter writer)
        {
            using (XmlGenericSerializer xmlSerializer = new XmlGenericSerializer(writer))
                xmlSerializer.Serialize(instance);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (this.m_writer != null)
                this.m_writer.Dispose();
        }

        #endregion
    }
}
