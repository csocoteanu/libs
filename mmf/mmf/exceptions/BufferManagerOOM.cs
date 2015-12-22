using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mmf.exceptions
{
    public class BufferManagerOOM : OutOfMemoryException
    {
        public BufferManagerOOM(string exceptionMessage) : base(exceptionMessage) { }
    }
}
