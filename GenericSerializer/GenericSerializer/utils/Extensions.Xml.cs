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
        internal static INode<XmlNode> ToXmlNodeInfo(this XmlNode node, string text=null)
        {
            NodeInfo<XmlNode> nodeInfo = new NodeInfo<XmlNode>(node.Name, text, node);

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
        internal static bool IsCompositeNode<T>(this INode<T> node)
        {
            return !string.IsNullOrEmpty(node[Constants.kCompositeType]);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal static string GetImmediateNodeText(this XmlNode node)
        {
            string nodeText = null;

            if (node.ChildNodes.Count == 1 &&
                node.ChildNodes[0].ChildNodes.Count == 0 &&
                node.ChildNodes[0].Value != Constants.kNullString)
            {
                nodeText = node.ChildNodes[0].Value;
            }

            return nodeText;
        }
    }
}
