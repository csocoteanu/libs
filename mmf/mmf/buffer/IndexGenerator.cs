using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mmf.common;

namespace mmf.buffer
{
    /// <summary>
    /// Free index generator class.
    /// This class generates an index available for use for the next buffer allocations.
    /// It keeps track of the allocated and freed indexes, so that when requesting a new
    /// buffer allocation, it takes into account any released index and allocates a new 
    /// index if there isn`t any one available.
    /// </summary>
    internal class IndexGenerator : ItemGenerator<int?>
    {
        public IndexGenerator(int maxIndexValue) : base(maxIndexValue) { }

        /// <summary>
        /// Gets the next available index for the call.
        /// </summary>
        /// <returns></returns>
        public int? GetNextFreeIndex()
        {
            return this.GenerateNextItem();
        }

        /// <summary>
        /// Release an index and make it available for the next call.
        /// </summary>
        /// <param name="index"></param>
        public void ReleaseIndex(ref int? index)
        {
            this.FreeItem(ref index);
        }

        #region ItemGenerator
        protected override int? CreateItemCB()
        {
            return AllItemsCount;
        }
        protected override void ResetItemCB(int? item) { }
        protected override void DisposeItemCB(int? item) { }
        #endregion
    }
}
