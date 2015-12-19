using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mmf.context;

namespace mmf.pool
{
    /// <summary>
    /// Object pool allocation cache.
    /// 1. Mantain a dictionary of all instances of the type, based on the object hash.
    /// 2. When an object is freed, mark it appropriately in the dictionary reference.
    /// 3. When an object is created, lookup the dictionary for any available references,
    /// and when one reference "freed" reference is found, return that one, instead of
    /// creating a new object.
    /// 4. Limit the size of the reference count to the one specified in the MMFContext.
    /// 
    /// Note: this class is to be used only when we have a large number of objects
    /// that will be allocated and freed, several times, causing managed memory fragmentation.
    /// </summary>
    /// <typeparam name="T">Type of the objects to be allocated</typeparam>
    public class ObjectPool<T> : IDisposable
        where T : class, IPoolableObject, new()
    {
        /// <summary>
        /// Structure used for holding the reference of the allocated object
        /// and a bool flag indicating whether the object has been "freed" or not.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        protected class Reference<U>
        {
            public U Root;
            public bool IsMapped;

            public Reference(U root, bool isMapped)
            {
                this.Root = root;
                this.IsMapped = isMapped;
            }
        }

        #region Members
        protected ushort m_maxPoolSize = 0;
        protected bool m_useDefaultAllocation = false;
        protected MMFContext m_context = null;
        protected Dictionary<int, Reference<T>> m_allReferences = null; 
        #endregion

        public ObjectPool(MMFContext context) { Init(context); }
        private void Init(MMFContext context)
        {
            m_context = context;
            m_useDefaultAllocation = m_context.UseDefaultAllocation;
            m_maxPoolSize = m_context.ObjectPoolCount;
            m_allReferences = new Dictionary<int, Reference<T>>();
        }

        /// <summary>
        /// Allocates or returns a pre-exsiting non-used object reference.
        /// </summary>
        /// <returns></returns>
        public T New()
        {
            bool foundFreeItem = false;
            T newItem = null;
            Reference<T> newItemRef = null;

            // if we don`t use this mechanism, simply create a new instance
            if (m_useDefaultAllocation)
                return new T();

            // secure concurent thread access to allocation mechanism
            lock (this)
            {
                // lookup all existing references
                // and search for free ones
                foreach (var refEntry in m_allReferences)
                {
                    var reference = refEntry.Value;
                    if (!reference.IsMapped)
                    {
                        // if we find a free reference
                        // than reset it`s default values
                        // and return it to the caller
                        foundFreeItem = true;
                        newItemRef = reference;
                        newItem = reference.Root;

                        newItem.Reset();
                        break;
                    }
                }

                if (!foundFreeItem)
                {
                    // if case we haven`t found any available reference,
                    // check if there is any space available left
                    if (m_allReferences.Count >= m_maxPoolSize)
                        return null;

                    // allocate a new object of the pool object type
                    // and initialize it properly
                    newItem = new T();
                    newItem.Init();
                    newItemRef = new Reference<T>(newItem, true);
                }

                // store or update the newly created reference
                if (newItem != null && newItemRef != null)
                    m_allReferences[newItem.GetHashCode()] = newItemRef;

                return newItem;
            }
        }

        /// <summary>
        /// Mark the respective object as free in the reference map.
        /// </summary>
        /// <param name="item"></param>
        public void Free(T item)
        {
            // if we don`t use this mechanism, simply return
            // there is no 'free'. GC will be responsible :)
            if (m_useDefaultAllocation)
                return;

            // secure concurent thread access to allocation mechanism
            lock (this)
            {
                int itemHash = item.GetHashCode();
                var itemReference = m_allReferences[itemHash];

                // update the flag that indicates the object is free
                // set the mapping based on the hash of the object
                itemReference.IsMapped = false;
                m_allReferences[itemHash] = itemReference;
            }
        }

        #region IDisposable Members
        /// <summary>
        /// Cleanup for all stored references.
        /// </summary>
        public void Dispose()
        {
            var iterator = m_allReferences.GetEnumerator();
            while (iterator.MoveNext())
            {
                // iterate through the entire object list
                // and cleanup all of the stored references.
                var currentItem = iterator.Current;
                var refItem = currentItem.Value;
                var refItemHash = refItem.GetHashCode();

                refItem.Root.Dispose();
                m_allReferences.Remove(refItemHash);
            }
        }
        #endregion
    }
}
