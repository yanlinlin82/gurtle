#region License, Terms and Author(s)
//
// Gurtle - IBugTraqProvider for Google Code
// Copyright (c) 2008 Atif Aziz. All rights reserved.
//
//  Author(s):
//
//      Atif Aziz, http://www.raboof.com
//
// This library is free software; you can redistribute it and/or modify it 
// under the terms of the New BSD License, a copy of which should have 
// been delivered along with this distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT 
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT 
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY 
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
#endregion

namespace Gurtle
{
    #region Imports

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Forms;

    #endregion

    internal sealed class ListViewSorter<LVI, T> where LVI : ListViewItem
    {
        private readonly Func<LVI, T> _itemSelector;
        private readonly Func<T, IComparable>[] _subSelectors;
        private SortOrder _order;
        private int _index;

        public ListViewSorter(ListView listView,
            Func<LVI, T> itemSelector,
            IEnumerable<Func<T, IComparable>> subSelectors)
        {
            Debug.Assert(listView != null);
            Debug.Assert(itemSelector != null);
            Debug.Assert(subSelectors != null);

            ListView = listView;
            _itemSelector = itemSelector;
            _subSelectors = subSelectors.ToArray();

            Debug.Assert(_subSelectors.Length == listView.Columns.Count);
        }

        public ListView ListView { get; private set; }

        public void AutoHandle()
        {
            ListView.ColumnClick += (sender, e) => SortByColumn(e.Column);
        }

        public void SortByColumn(int index)
        {
            _order = _index != index 
                   ? SortOrder.Ascending 
                   : _order == SortOrder.Ascending 
                     ? SortOrder.Descending 
                     : SortOrder.Ascending;

            _index = index;

            var subSelector = _subSelectors[index];
            Comparison<LVI> comparison = (x, y) => subSelector(_itemSelector(x)).CompareTo(subSelector(_itemSelector(y)));
                
            ListView.ListViewItemSorter = new DelegatingComparer<LVI>(
                _order == SortOrder.Descending ? (x, y) => comparison(x, y) * -1 : comparison);
        }
    }
}