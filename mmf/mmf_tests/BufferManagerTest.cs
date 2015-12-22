using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mmf.buffer;
using mmf.exceptions;

namespace mmf_tests
{
    [TestClass]
    public class BufferManagerTest
    {
        [TestMethod]
        public void BasicAllocations()
        {
            var context = new mmf.context.MMFContext();
            context.BufferSize = 4;
            context.BufferCount = 2;

            using (var bufferManager = new BufferManager(context))
            {
                var buffer1 = bufferManager.GetBuffer();
                var buffer2 = bufferManager.GetBuffer();

                bool bufferManagerHasOOM = false;
                try
                {
                    bufferManager.GetBuffer();
                }
                catch (BufferManagerOOM)
                {
                    bufferManagerHasOOM = true;
                }

                //  check if OOM occured as expected
                Assert.IsTrue(bufferManagerHasOOM, "OOM did not occur as expected!");
                // release buffer2 and check for null
                bufferManager.FreeBuffer(ref buffer2);
                Assert.IsNull(buffer2.Array, "Buffer should be null!");
                // reassign buffer3 and chek it again if null
                var buffer3 = bufferManager.GetBuffer();
                Assert.IsNotNull(buffer3.Array, "Buffer should not be null!");

                // check buffer indexes and counts
                // check if the start index for the second array is the next one in line after the end of the first array
                Assert.AreEqual(buffer1.Offset + buffer1.Count, buffer3.Offset, "Invalid index allocation");
                Assert.AreEqual(buffer1.Count, buffer3.Count, "Size of arrays differ");
            }
        }
    }
}
