﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.rwlock
{
    public interface IRWFactory<T>
    {
        IRWLock<T> CreateRWLock(int maxThreadCount);
        IRWFactory<U> CloneTo<U>();
    }
}
