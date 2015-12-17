using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mmf.pool;

namespace mmf.tests.types
{
    public class Student : IPoolableObject
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public double Grade { get; set; }

        #region IPoolableObject Members
        public void Init()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.Name = null;
            this.Age = 0;
            this.Grade = 0.0;
        }
        #endregion

        #region IDisposable Members
        public void Dispose() { }
        #endregion

        public override string ToString()
        {
            return string.Format("{0} => Age:{1} Grade:{2}", Name, Age, Grade);
        }
    }
}
