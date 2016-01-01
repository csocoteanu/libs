using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace logger
{
    internal class ConsoleLogger : AbstractLogger
    {
        public ConsoleLogger(string type) : base(type) { }

        #region AbstractLogger members
        protected override void OnInitLogger() { }
        protected override void DoCleanup() { }

        protected override void Writeline(string text)
        {
            Console.WriteLine(text);
        }
        #endregion
    }
}
