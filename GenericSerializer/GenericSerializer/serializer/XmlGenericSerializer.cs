using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;
using GenericSerializer.XmlUtils;
using GenericSerializer.XmlUtils.Extensions;

namespace GenericSerializer.Serializer
{
    internal class XmlGenericSerializer : IDisposable
    {
        protected IXmlWriter m_writer;

        protected XmlGenericSerializer(IXmlWriter writer)
        {
            this.m_writer = writer;
        }

        /// <summary>
        /// Serializes a "instance" object under the "nodeElement"
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="nodeElement"></param>
        internal void Serialize(object instance, string nodeElement, Type instanceType=null)
        {
            if (instance == null)
            {
                // serialize null object
                this.SerializeNullInstance(nodeElement, instanceType);
                return;
            }

            // serialize non-null object
            instanceType = instanceType ?? instance.GetType();
            string assemblyName = instanceType.Assembly.GetName().Name;
            string instanceTypeString = instanceType.FullName;
            string compositeType = instanceType.GetCompositeType(); // composityType is used for differentiating compund types
                                                                    // as struct(s) and class(es)

            this.m_writer.WriteStartElement(nodeElement, Constants.kCompositeType, compositeType,
                                                         Constants.kAssembly, assemblyName,
                                                         Constants.kMemberType, instanceTypeString);
            this.SerializeProperties(instance);
            this.m_writer.WriteEndElement();
        }

        protected void SerializeNullInstance(string nodeElement, Type instanceType=null)
        {
            string elementValue = (instanceType != null) ? instanceType.GetCompoundEmptyValueAsString() :  Constants.kNullString;
            string assemblyName = (instanceType != null) ? instanceType.Assembly.GetName().Name : null;
            string instanceTypeString = (instanceType != null) ? instanceType.FullName : null;

            this.m_writer.WriteElementString(nodeElement,
                                             elementValue,
                                             Constants.kAssembly, assemblyName,
                                             Constants.kMemberType, instanceTypeString);
        }

        /// <summary>
        /// Serializes the properties of a compund type
        /// struct or class
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="compositeType"></param>
        protected void SerializeProperties(object instance)
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
                    this.Serialize(propertyValue, property.Name, property.PropertyType);
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

            this.m_writer.WriteElementString(property.Name,
                                             valueString,
                                             Constants.kMemberType, property.PropertyType.FullName);
        }

        internal static void Serialize(object instance, IXmlWriter writer)
        {
            using (XmlGenericSerializer xmlSerializer = new XmlGenericSerializer(writer))
                xmlSerializer.Serialize(instance, Constants.kRootElement);
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
