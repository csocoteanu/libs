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
        public void TestMethod1()
        {
            var context = mmf.context.MMFContext.Instance;
            using (var bufferManager = new BufferManager(context))
            {
                var buffer = bufferManager.GetBuffer();
            }
        }
    }
}
