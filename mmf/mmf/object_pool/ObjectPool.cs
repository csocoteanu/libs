using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mmf.context;
using mmf.common;

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
        #region Members
        private bool m_useOldAllocator = false;
        private int m_itemPoolSize = 0;
        private MMFContext m_context = null;
        private PoolItemGenerator<T> m_itemGenerator = null;
        #endregion

        public ObjectPool(MMFContext context) { Init(context); }
        private void Init(MMFContext context)
        {
            m_context = context;
            m_useOldAllocator = m_context.UseDefaultAllocation;
            m_itemPoolSize = m_context.ObjectPoolCount;
            m_itemGenerator = new PoolItemGenerator<T>(m_itemPoolSize);
        }

        /// <summary>
        /// Allocates or returns a pre-exsiting non-used object reference.
        /// </summary>
        /// <returns></returns>
        public T New()
        {
            if (m_useOldAllocator)
                return new T();

            return m_itemGenerator.GenerateNewItem();
        }

        /// <summary>
        /// Mark the respective object as free in the reference map.
        /// </summary>
        /// <param name="item"></param>
        public void Free(T item)
        {
            if (!m_useOldAllocator)
                m_itemGenerator.ReleaseItem(ref item);            
        }

        #region IDisposable Members
        /// <summary>
        /// Cleanup for all stored references.
        /// </summary>
        public void Dispose()
        {
            m_itemGenerator.Dispose();
        }
        #endregion
    }
}
