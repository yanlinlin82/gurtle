#region License, Terms and Author(s)
//
// BackLINQ
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

namespace BackLinq
{
    #region Imports

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

    internal sealed class OrderedEnumerable<T, K> : IOrderedEnumerable<T>
    {
        private readonly IEnumerable<T> _source;
        private readonly Comparison<T> _comparison;

        public OrderedEnumerable(IEnumerable<T> source, Comparison<T> parent,
            Func<T, K> keySelector, IComparer<K> comparer, bool descending)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");

            _source = source;
            
            comparer = comparer ?? Comparer<K>.Default;
            Comparison<T> comparison = (x, y) => comparer.Compare(keySelector(x), keySelector(y)) * (descending ? -1 : 1);            
            _comparison = parent == null ? comparison : (x, y) =>
            {
                var result = parent(x, y);
                return result != 0 ? result : comparison(x, y);
            };
        }

        public IOrderedEnumerable<T> CreateOrderedEnumerable<KK>(
            Func<T, KK> keySelector, IComparer<KK> comparer, bool descending)
        {
            return new OrderedEnumerable<T, KK>(_source, _comparison, keySelector, comparer, descending);
        }

        public IEnumerator<T> GetEnumerator()
        {
            var list = _source.ToList();
            list.Sort(_comparison);
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}