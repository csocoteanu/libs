﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mmf.common
{
    /// <summary>
    /// Item generator class.
    /// This class generates an item available for use for the next use.
    /// It keeps track of the allocated and freed items, so that when requesting a new
    /// allocation, it takes into account any released item and allocates a new item
    /// if there isn`t any one available.
    /// </summary>
    internal abstract class ItemGenerator<T> : IDisposable
    {
        #region Members
        protected int AllItemsCount { get; private set; }
        protected int MaxItemCount { get; private set; }
        private Queue<T> m_freeItems = null; 
        #endregion

        #region Abstract Methods
        protected abstract T CreateItemCB(); 
        #endregion

        public ItemGenerator(int maxItems)
        {
            MaxItemCount = maxItems;
            AllItemsCount = 0;
            m_freeItems = new Queue<T>(); 
        }

        private T CreateNewItem()
        {
            T newItem = CreateItemCB();
            AllItemsCount++;

            return newItem;
        }

        protected T GenerateNextItem()
        {
            lock (this)
            {
                if (m_freeItems.Count > 0)
                {
                    return m_freeItems.Dequeue();
                }
                else
                {
                    if (AllItemsCount == MaxItemCount)
                        return default(T);

                    return CreateNewItem();
                }
            }
        }

        protected void FreeItem(ref T item)
        {
            lock (this)
            {
                m_freeItems.Enqueue(item);
                item = default(T); 
            }
        }

        #region IDisposable Members
        public void Dispose()
        {
            AllItemsCount = MaxItemCount = 0;
            m_freeItems.Clear();
        }
        #endregion
    }
}
