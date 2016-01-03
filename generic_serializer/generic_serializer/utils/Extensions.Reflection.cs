using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace GenericSerializer.XmlUtils.Extensions
{
    public static partial class Reflection
    {
        // //---------------------------Type----------------------------------------------------
        // -------------------------------------------------------------------------------------
        /// <summary>
        /// Checks if the type is a struct or not
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsStructType(this Type type)
        {
            return type.IsValueType && !type.IsPrimitive;
        }

        /// <summary>
        /// Returns an appropriate string indicating, 
        /// whether this type represents a class or a struct
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetCompositeType(this Type type)
        {
            return type.IsStructType() ? Constants.kStructString : Constants.kClassString;
        }

        // //---------------------------PropertyInfo--------------------------------------------
        // -------------------------------------------------------------------------------------
        /// <summary>
        /// Check whether the property was 
        /// marked with the GenericSerializableAttribute
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool IsPropertySerializable(this PropertyInfo property)
        {
            return true;
        }

        /// <summary>
        /// Check whether or not this property is a primitive
        /// defined value, including the string ref type
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool IsPrimitiveType(this PropertyInfo property)
        {
            Type propertyType = property.PropertyType;
            return (propertyType.IsValueType || propertyType == typeof(string)) &&
                   (!propertyType.IsStructType());
        }

        /// <summary>
        /// For a given type (struct or class)
        /// return the string representation of the empty compound
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetCompoundEmptyValueAsString(this Type type)
        {
            return type.IsStructType() ? XmlUtils.Constants.kEmptyStruct : XmlUtils.Constants.kNullString;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        internal static object CreateDefaultInstance<T>(this INode<T> node)
        {
            string assemblyName = node[Constants.kAssembly];
            string instanceTypeString = node[Constants.kMemberType];
            Assembly typeAssembly = Assembly.Load(assemblyName);
            Type instanceType = typeAssembly.GetType(instanceTypeString);

            return Activator.CreateInstance(instanceType);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <param name="memberValue"></param>
        internal static void SetMemberValue(this object instance, string propertyName, object memberValue)
        {
            try
            {
                Type instanceType = instance.GetType();
                PropertyInfo propertyNameInfo = instanceType.GetProperty(propertyName);

                propertyNameInfo.SetValue(instance,
                                          Convert.ChangeType(memberValue, propertyNameInfo.PropertyType),
                                          null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
