using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace logger
{
    internal interface IDisposableLogger : IDisposable
    {
        event EventHandler<EventArgs> OnDispose;
    }
}
