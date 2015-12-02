﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    public interface IRWLock<T>
    {
        T ReadNextTask();
        void WriteTask(T task);
    }
}
