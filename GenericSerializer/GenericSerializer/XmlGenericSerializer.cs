using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;
using GenericSerializer.XmlUtils;

namespace GenericSerializer
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

        internal void Serialize(object instance)
        {
            string instanceTypeString = instance.GetType().FullName;

            if (instance != null)
            {
                this.m_depth++;

                this.m_writer.WriteStartElement(instanceTypeString);
                this.SerializeProperties(instance);
                this.m_writer.WriteEndElement();
            }
            else
            {
                this.m_writer.WriteElementString(instanceTypeString, Utils.kNullString);
            }
        }

        protected void SerializeProperties(object instance)
        {
            Type instanceType = instance.GetType();
            PropertyInfo[] properties = instanceType.GetProperties();

            foreach (var property in properties)
            {
                if (!this.IsPropertySerializable(property))
                    continue;

                if (this.IsPrimitiveType(property))
                {
                    this.SerializePrimitiveType(instance, property);
                }
                else
                {
                    object propertyValue = property.GetValue(instance, null);
                    this.Serialize(propertyValue);
                }
            }
        }

        private bool IsPrimitiveType(PropertyInfo property)
        {
            return property.PropertyType.IsValueType || property.PropertyType == typeof(string);
        }

        /// <summary>
        /// TODO: Check whether the property was 
        /// marked with the GenericSerializableAttribute
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        protected bool IsPropertySerializable(PropertyInfo property)
        {
            return true;
        }

        protected void SerializePrimitiveType(object instance, PropertyInfo property)
        {
            bool isStruct = property.PropertyType.IsValueType && !property.PropertyType.IsPrimitive;

            if (!isStruct)
            {
                object value = property.GetValue(instance, null);
                string valueString = (value != null) ? value.ToString() : Utils.kNullString;

                this.m_writer.WriteElementString(property.Name, valueString);
            }
            else
            {
                object structValue = property.GetValue(instance, null);
                this.Serialize(structValue);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (this.m_writer != null)
                this.m_writer.Dispose();
        }

        #endregion

        internal static void Serialize(object instance, IXmlWriter writer)
        {
            using (XmlGenericSerializer xmlSerializer = new XmlGenericSerializer(writer))
                xmlSerializer.Serialize(instance);
        }
    }
}
