using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GenericSerializer.XmlUtils.Extensions
{
    public static class Xml
    {
        // //---------------------------XmlNode-------------------------------------------------
        // -------------------------------------------------------------------------------------
        /// <summary>
        /// TODO:
        /// </summary>
        /// <param name="node"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        internal static IXmlNode<XmlNode> ToXmlNodeInfo(this XmlNode node, string text=null)
        {
            XmlNodeInfo<XmlNode> nodeInfo = new XmlNodeInfo<XmlNode>(node.Name, text, node);

            // populate attributes
            foreach (XmlAttribute attribute in node.Attributes)
                nodeInfo[attribute.Name] = attribute.Value;

            return nodeInfo;
        }

        /// <summary>
        /// TODO:
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        internal static bool IsCompositeNode<T>(this IXmlNode<T> node)
        {
            return !string.IsNullOrEmpty(node[Constants.kComposyteType]);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal static string GetImmediateNodeText(this XmlNode node)
        {
            string nodeText = null;
            if (node.ChildNodes.Count == 1 && node.ChildNodes[1].ChildNodes.Count == 0)
                nodeText = node.ChildNodes[1].Value;

            return nodeText;
        }
    }
}
