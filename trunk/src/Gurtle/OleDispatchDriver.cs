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
    using System.Reflection;
    using System.Runtime.InteropServices;

    #endregion

    internal sealed class OleDispatchDriver : IDisposable
    {
        private object _obj;

        public OleDispatchDriver(string progId) :
            this(Type.GetTypeFromProgID(progId)) {}

        public OleDispatchDriver(Type type) :
            this(Activator.CreateInstance(type)) { }

        public OleDispatchDriver(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            _obj = obj;
        }

        ~OleDispatchDriver()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_obj == null) return;
            GC.SuppressFinalize(this);
            Marshal.ReleaseComObject(_obj);
            _obj = null;
        }

        public object Object
        {
            get
            {
                if (_obj == null) throw new ObjectDisposedException(ToString());
                return _obj;
            }
        }

        public object Get(string name)
        {
            return Object.GetType().InvokeMember(name, BindingFlags.GetProperty, null, Object, null);
        }

        public T Get<T>(string name)
        {
            return (T) Get(name);
        }

        public void Put(string name, params object[] args)
        {
            Object.GetType().InvokeMember(name, BindingFlags.SetProperty, null, Object, args);
        }
        
        public object Invoke(string name, params object[] args)
        {
            return Object.GetType().InvokeMember(name, BindingFlags.InvokeMethod, null, Object, args);
        }

        public T Invoke<T>(string name, params object[] args)
        {
            return (T) Invoke(name, args);
        }
    }
}
