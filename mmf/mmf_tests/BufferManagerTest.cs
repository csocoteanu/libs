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
            var context = new mmf.context.MMFContext();
            context.BufferCount = 2;
            context.BufferSize = 4;
            using (var bufferManager = new BufferManager(context))
            {
                var buffer1 = bufferManager.GetBuffer();
                var buffer2 = bufferManager.GetBuffer();
                var buffer3 = bufferManager.GetBuffer();

                //  check if buffer3 is null as expected
                Assert.IsNull(buffer3, "After {0} allocations the returned buffer should be null" , context.BufferCount + 1);
                // release buffer2 and check for null
                bufferManager.FreeBuffer(ref buffer2);
                Assert.IsNull(buffer2, "Buffer should be null after being released!");
                // reassign buffer3 and chek it again if null
                buffer3 = bufferManager.GetBuffer();
                Assert.IsNotNull(buffer3, "Buffer should not be null!");
            }
        }
    }
}
