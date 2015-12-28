using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mmf.common;

namespace mmf.pool
{
    /// <summary>
    /// Free item generator class.
    /// This class generates an item available for use for the next object allocations.
    /// It keeps track of the allocated and freed items, so that when requesting a new
    /// object allocation, it takes into account any released item and allocates a new 
    /// item if there isn`t any one available.
    /// </summary>
    internal class PoolItemGenerator<T> : ItemGenerator<T>
        where T : class, IPoolableObject, new()
    {
        public PoolItemGenerator(int itemPoolSize) : base(itemPoolSize) { }

        /// <summary>
        /// Gets the next available item for the call.
        /// </summary>
        /// <returns></returns>
        public T GenerateNewItem()
        {
            return this.GenerateNextItem();
        }

        /// <summary>
        /// Release an item and make it available for the next call.
        /// </summary>
        /// <param name="index"></param>
        public void ReleaseItem(ref T item)
        {
            this.FreeItem(ref item);
        }

        #region ItemGenerator
        protected override T CreateItemCB()
        {
            T newItem = new T();
            newItem.Init();
            return newItem;
        }
        protected override void DisposeItemCB(T item)
        {
            item.Reset();
            item.Dispose();
        }
        protected override void ResetItemCB(T item)
        {
            item.Reset();
        }
        #endregion
    }
}
