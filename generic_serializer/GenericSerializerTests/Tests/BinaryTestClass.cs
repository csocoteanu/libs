using GenericSerializer.Factories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GenericSerializerTests.Tests
{
    class BinaryTreeNode
    {
        public string Value { get; set; }
        public BinaryTreeNode LeftNode { get; set; }
        public BinaryTreeNode RightNode { get; set; }

        public BinaryTreeNode() { }
        public BinaryTreeNode(string value, BinaryTreeNode left, BinaryTreeNode right)
        {
            this.Value = value;
            this.LeftNode = left;
            this.RightNode = right;
        }

        public static BinaryTreeNode BuildCustomTree()
        {
            BinaryTreeNode root = new BinaryTreeNode("5",
                    new BinaryTreeNode("4",
                            new BinaryTreeNode("2", null,null),
                            new BinaryTreeNode("3", null,null)),
                    new BinaryTreeNode("6",
                            new BinaryTreeNode("7", null, null),
                            new BinaryTreeNode("8", null, null)));
            return root;
        }
    }

    [TestFixture]
    public class BinaryTreeTest
    {
        private string m_desktopPath;
        private string m_outputPath;
        private BinaryTreeNode m_root;

        [TestFixtureSetUp]
        public void TestInit()
        {
            m_desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            m_outputPath = Path.Combine(m_desktopPath, typeof(BinaryTreeNode).Name + ".xml");
            m_root = BinaryTreeNode.BuildCustomTree();
        }

        [TestFixtureTearDown]
        public void TestEnd()
        {

        }

        [Test]
        public void Serialize()
        {
            Factory.Serialize(this.m_root, Factory.eSerializationType.kXML, this.m_outputPath);
            var @object = Factory.Deserialize(this.m_outputPath, Factory.eSerializationType.kXML);
        }
    }
}
