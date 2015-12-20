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
    public class ObjectPoolTest
    {
        [TestMethod]
        public void SingleObject_ReferenceCheck()
        {
            using (var studentPool = new ObjectPool<Student>(mmf.context.MMFContext.Instance))
            {
                // create first object and release the memory
                Student gigel = studentPool.New();
                gigel.Name = "Gigi";
                gigel.Age = 18;
                gigel.Grade = 10;
                studentPool.Free(gigel);

                // create the second object
                Student ionel = studentPool.New();
                ionel.Name = "Ionel";
                ionel.Age = 18;
                ionel.Grade = 5;
                studentPool.Free(ionel);

                // The references should point to the same location
                Assert.IsTrue(gigel == ionel, "Object references should point to the same location!");

                // create first object without releasing the memory
                gigel = studentPool.New();
                gigel.Name = "Gigi";
                gigel.Age = 18;
                gigel.Grade = 10;

                // create the second object
                ionel = studentPool.New();
                ionel.Name = "Ionel";
                ionel.Age = 18;
                ionel.Grade = 5;

                // The references should point at different locations
                Assert.IsTrue(gigel != ionel, "Object references should point at different locations!");
            }
        }

        [TestMethod]
        public void TestSingleObject_SingleAllocation()
        {
            long totalMemory = 0;
            Debug.WriteLine("Total memory before allocations: " + GC.GetTotalMemory(true));
            using (var studentPool = new ObjectPool<Student>(mmf.context.MMFContext.Instance))
            {
                Student gigel = studentPool.New();
                gigel.Name = "Gigi";
                gigel.Age = 18;
                gigel.Grade = 10;
                studentPool.Free(gigel);
                totalMemory = GC.GetTotalMemory(false);
                Debug.WriteLine("Total memory after first allocation: " + totalMemory);

                Student ionel = studentPool.New();
                ionel.Name = "Ionel";
                ionel.Age = 18;
                ionel.Grade = 5;
                var memoryAfterSecondAllocation = GC.GetTotalMemory(false);
                Debug.WriteLine("Total memory after second allocation: " + memoryAfterSecondAllocation);

                Assert.IsTrue(totalMemory == memoryAfterSecondAllocation, "There should be no memory increase!");
            }
            Debug.WriteLine("Total memory after all allocations: " + GC.GetTotalMemory(true));
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
