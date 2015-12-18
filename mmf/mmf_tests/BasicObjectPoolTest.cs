using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mmf.tests.types;
using mmf.pool;
using System.Diagnostics;

namespace mmf.tests
{
    [TestClass]
    public class BasicObjectPoolTest
    {
        [TestMethod]
        public void TestSingleObject_SingleAllocation()
        {
            var studentPool = ObjectPool<Student>.Instance;
            Debug.WriteLine("Total memory before allocations: " + GC.GetTotalMemory(false));

            Student gigel = studentPool.New();
            gigel.Name = "Gigi";
            gigel.Age = 18;
            gigel.Grade = 10;
            Debug.WriteLine(gigel);
            Debug.WriteLine("Total memory after first allocation: " + GC.GetTotalMemory(false));
            studentPool.Free(gigel);

            Student ionel = studentPool.New();
            ionel.Name = "Ionel";
            ionel.Age = 18;
            ionel.Grade = 5;
            Debug.WriteLine(ionel);
            Debug.WriteLine("Total memory after second allocation: " + GC.GetTotalMemory(false));
        }

        [TestMethod]
        public void TestSingleObject_MultipleAllocation()
        {
            // mmf.context.MMFContext.Instance.UseDefaultAllocation = true;
            var studentPool = ObjectPool<Student>.Instance;

            for (int i = 0; i < int.MaxValue; i++)
            {
                Student gigel = studentPool.New();
                gigel.Name = "Gigi";
                gigel.Age = 18;
                gigel.Grade = 10;
                studentPool.Free(gigel); 
            }
        }
    }
}
