using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using logger;

namespace tests
{
    [TestClass]
    public class LoggerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var logger = LogManager.GetLogger())
            {
                logger.Debug("debug1");
                logger.Info("info1");
            }
            
        }
    }
}
