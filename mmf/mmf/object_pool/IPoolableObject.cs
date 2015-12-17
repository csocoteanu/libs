using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mmf.pool
{
    public interface IPoolableObject : IDisposable
    {
        void Init();
        void Reset();
    }
}
