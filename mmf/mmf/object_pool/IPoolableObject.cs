using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace server.memory.pool
{
    public interface IPoolableObject : IDisposable
    {
        void Init();
        void Reset();
    }
}
