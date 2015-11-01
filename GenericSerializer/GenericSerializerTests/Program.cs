using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenericSerializer;
using System.Xml;

namespace GenericSerializerTests
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlSerializerFactory.Serialize(new TestSimpleClass(), @"C:\Users\in10se\Desktop\out.xml");
        }
    }
}
