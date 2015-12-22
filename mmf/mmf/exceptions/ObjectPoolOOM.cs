using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mmf.exceptions
{
    public class ObjectPoolOOM : OutOfMemoryException
    {
        public ObjectPoolOOM(string exceptionMessage) : base(exceptionMessage) { }
    }
}
