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
    using System.Diagnostics;

    #endregion

    internal interface IProperty
    {
        Type ObjectType { get; }
        Type PropertyType { get; }
        bool ReadOnly { get; }
        object GetValue(object obj);
        void SetValue(object obj, object value);
    }

    internal interface IProperty<T> : IProperty
    {
        object GetValue(T obj);
        void SetValue(T obj, object value);
    }

    internal interface IProperty<T, PT> : IProperty<T>
    {
        new PT GetValue(T obj);
        void SetValue(T obj, PT value);
    }

    [ Serializable ]
    internal sealed class Property<T, PT> : IProperty<T, PT>
    {
        private readonly Func<T, PT> _getter;
        private readonly Action<T, PT> _setter;

        public Property(Func<T, PT> getter) :
            this(getter, null) {}

        public Property(Func<T, PT> getter, Action<T, PT> setter)
        {
            Debug.Assert(getter != null);

            _getter = getter;
            _setter = setter;
        }

        public Type ObjectType { get { return typeof(T); } }
        public Type PropertyType { get { return typeof(PT); } }
        public bool ReadOnly { get { return _setter == null; } }
        
        public PT GetValue(T obj) { return _getter(obj); }

        public void SetValue(T obj, PT value)
        {
            if (_setter == null)
                throw new InvalidOperationException("Cannot write to a read-only property.");

            _setter(obj, value);
        }

        object IProperty<T>.GetValue(T obj) { return GetValue(obj); }
        void IProperty<T>.SetValue(T obj, object value) { SetValue(obj, (PT) value); }
        object IProperty.GetValue(object obj) { return GetValue((T) obj); }
        void IProperty.SetValue(object obj, object value) { SetValue((T) obj, (PT) value); }
    }
}