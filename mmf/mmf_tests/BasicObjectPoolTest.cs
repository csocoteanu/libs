using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mmf.tests.types;
using mmf.pool;
using System.Diagnostics;
using System.Runtime;

namespace mmf.tests
{
    [TestClass]
    public class BasicObjectPoolTest
    {
        [TestMethod]
        public void TestSingleObject_SingleAllocation()
        {
            using (var studentPool = new ObjectPool<Student>(mmf.context.MMFContext.Instance))
            {
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
        }

        [TestMethod]
        public void TestSingleObject_MultipleAllocation()
        {
            int allocationCount = 1000000;
            long totalMemoryBeforeAllocation = 0, totalMemoryAfterAllocation = 0;
            long memDefaultAllocator = 0, memNewAllocator = 0;
            var context = mmf.context.MMFContext.Instance;

            Debug.WriteLine("Allocating {0} objects of type {1}...", allocationCount, typeof(Student).Name);

            // allocate an ammount of 1000k students
            // using the old allocation mechanism
            context.UseDefaultAllocation = true;
            totalMemoryBeforeAllocation = GC.GetTotalMemory(false);
            using (var studentPool = new ObjectPool<Student>(context))
            {
                for (int i = 0; i < allocationCount; i++)
                {
                    Student gigel = studentPool.New();
                    gigel.Name = "Gigi";
                    gigel.Age = 18;
                    gigel.Grade = 10;
                    studentPool.Free(gigel);
                }
            }
            totalMemoryAfterAllocation = GC.GetTotalMemory(false);
            memDefaultAllocator = totalMemoryAfterAllocation - totalMemoryBeforeAllocation;
            Debug.WriteLine("Total memory using default allocator: " + memDefaultAllocator);

            // allocate an ammount of 1000k students
            // using the new allocation mechanism
            context.UseDefaultAllocation = false;
            totalMemoryBeforeAllocation = GC.GetTotalMemory(false);
            using (var studentPool = new ObjectPool<Student>(context))
            {
                for (int i = 0; i < allocationCount; i++)
                {
                    Student gigel = studentPool.New();
                    gigel.Name = "Gigi";
                    gigel.Age = 18;
                    gigel.Grade = 10;
                    studentPool.Free(gigel);
                }
            }
            totalMemoryAfterAllocation = GC.GetTotalMemory(false);
            memNewAllocator = totalMemoryAfterAllocation - totalMemoryBeforeAllocation;
            Debug.WriteLine("Total memory using new allocator: " + memNewAllocator);

            Assert.IsTrue(memNewAllocator <= memDefaultAllocator, "Default allocator is more efficient than the new one!");
            Assert.IsTrue(memNewAllocator == 0, "Memory should be freed after using the new allocator mechanism!");
        }
    }
}
