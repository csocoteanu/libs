using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mmf.buffer;

namespace mmf_tests
{
    [TestClass]
    public class BufferManagerTest
    {
        [TestMethod]
        public void BasicAllocations()
        {
            var context = mmf.context.MMFContext.Instance;
            context.BufferCount = 2;
            context.BufferSize = 4;
            using (var bufferManager = new BufferManager(context))
            {
                var buffer1 = bufferManager.GetBuffer();
                var buffer2 = bufferManager.GetBuffer();
                var buffer3 = bufferManager.GetBuffer();

                Assert.IsNull(buffer3, "After {0} allocations the returned buffer should be null" , context.BufferCount + 1);
            }
        }
    }
}
