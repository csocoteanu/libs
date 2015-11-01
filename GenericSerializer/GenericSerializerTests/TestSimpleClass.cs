using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericSerializerTests
{
    public struct TestSimpleStruct
    {
        public int IntValue { get; set; }
        public double DoubleValue { get { return 2.3; } }
        public string StringValue { get; set; }
    }

    public class TestSimpleClass
    {
        public int IntValue { get; set; }
        public double DoubleValue { get { return 2.3; } }
        public string StringValue { get; set; }

        public TestSimpleStruct simpleStruct { get; set; }
        public TestSimpleClass NextClass { get; set; }
    }
}
