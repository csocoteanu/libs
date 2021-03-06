﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using server.Properties;

namespace server.memory.pool
{
    public class ObjectPool<T> : IDisposable
        where T : class, IPoolableObject, new()
    {
        protected struct Reference<U>
        {
            public U Root;
            public bool IsMapped;

            public Reference(U root, bool isMapped)
            {
                this.Root = root;
                this.IsMapped = isMapped;
            }
        }

        protected ushort m_maxPoolSize = 0;
        protected Dictionary<int, Reference<T>> m_allReferences = null;

        protected ObjectPool() { Init(); }
        public static readonly ObjectPool<T> Instance = new ObjectPool<T>();

        private void Init()
        {
            m_maxPoolSize = Settings.Default.kObjectPoolSize;
            m_allReferences = new Dictionary<int, Reference<T>>();
        }

        public T New()
        {
            lock (this)
            {
                bool foundFreeItem = false;
                T newItem = null;
                Reference<T> newItemRef = default(Reference<T>);

                foreach (var refEntry in m_allReferences)
                {
                    var reference = refEntry.Value;
                    if (!reference.IsMapped)
                    {
                        foundFreeItem = true;
                        newItemRef = reference;
                        newItem = reference.Root;

                        newItem.Reset();
                        break;
                    }
                }

                if (!foundFreeItem)
                {
                    if (m_allReferences.Count >= m_maxPoolSize)
                        return null;

                    newItem = new T();
                    newItem.Init();

                    newItemRef = new Reference<T>(newItem, true);
                }

                m_allReferences[newItem.GetHashCode()] = newItemRef;

                return newItem;
            }
        }

        public void Free(T item)
        {
            lock (this)
            {
                int itemHash = item.GetHashCode();
                var itemReference = m_allReferences[itemHash];

                itemReference.IsMapped = false; 
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            var iterator = m_allReferences.GetEnumerator();
            while (iterator.MoveNext())
            {
                var currentItem = iterator.Current;
                var refItem = currentItem.Value;

                refItem.Root.Dispose();
            }
        }

        #endregion
    }
}
