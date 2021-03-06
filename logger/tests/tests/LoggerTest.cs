﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using logger;
using logger.config;
using logger.common;

namespace tests
{
    [TestClass]
    public class LoggerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            Configurator.Instance.LogLevel = eLogLevel.kAll;
            Configurator.Instance.LogSizeKB = 20;
            using (var logger = LogManager.GetLogger())
            {
                logger.Debug("debug1");
                logger.Info("info1");
            }
            
        }
    }
}
