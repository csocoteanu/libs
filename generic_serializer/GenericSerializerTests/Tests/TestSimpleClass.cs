﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GenericSerializer.Factories;

namespace GenericSerializerTests
{
    public struct TestSimpleStruct
    {
        public int IntValue { get; set; }
        public double DoubleValue { get { return 2.3; } set { } }
        public string StringValue { get; set; }
    }

    public class TestSimpleClass
    {
        public int IntValue { get; set; }
        public double DoubleValue { get { return 2.3; } set { } }
        public string StringValue { get; set; }

        public TestSimpleStruct simpleStruct { get; set; }

        public TestSimpleClass simpleClass { get; set; }
    }

    [TestFixture]
    public class SimpleTest
    {
        private string m_desktopPath;
        private string m_outputPath;
        private TestSimpleClass m_simpleClass;

        [TestFixtureSetUp]
        public void TestInit()
        {
            m_desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            m_outputPath = Path.Combine(m_desktopPath, typeof(TestSimpleClass).Name + ".xml");
            m_simpleClass = new TestSimpleClass();
        }

        [TestFixtureTearDown]
        public void TestEnd()
        {

        }

        [Test]
        public void Serialize()
        {
            Factory.Serialize(this.m_simpleClass, Factory.eSerializationType.kXML, this.m_outputPath);
            var @object = Factory.Deserialize(this.m_outputPath, Factory.eSerializationType.kXML);
        }
    }
}
