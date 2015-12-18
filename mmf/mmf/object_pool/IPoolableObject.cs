using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mmf.pool
{
    /// <summary>
    /// Interface to be implemented by all of the classes,
    /// we want to enable the object pool mechanism.
    /// </summary>
    public interface IPoolableObject : IDisposable
    {
        void Init();
        void Reset();
    }
}
