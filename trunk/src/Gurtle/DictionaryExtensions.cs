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
    using System.Collections.Generic;

    internal static class DictionaryExtensions
    {
        public static V Find<K, V>(this IDictionary<K, V> dict, K key)
        {
            return Find(dict, key, default(V));
        }

        public static V Find<K, V>(this IDictionary<K, V> dict, K key, V @default)
        {
            V value;
            return dict.TryGetValue(key, out value) ? value : @default;
        }

        public static V Pop<K, V>(this IDictionary<K, V> dict, K key)
        {
            var value = dict[key];
            dict.Remove(key);
            return value;
        }

        public static V TryPop<K, V>(this IDictionary<K, V> dict, K key)
        {
            return TryPop(dict, key, default(V));
        }

        public static V TryPop<K, V>(this IDictionary<K, V> dict, K key, V @default)
        {
            var value = dict.Find(key, @default);
            dict.Remove(key);
            return value;
        }
    }
}
