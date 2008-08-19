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

namespace System
{
    #region Access modifier
    #if BACKLINQ_LIB
        public 
    #else
        internal
    #endif
    #endregion
    
    delegate TResult Func<TResult>();

    #region Access modifier
    #if BACKLINQ_LIB
        public 
    #else
        internal
    #endif
    #endregion
    
    delegate TResult Func<T, TResult>(T a);
    #region Access modifier
    #if BACKLINQ_LIB
        public 
    #else
        internal
    #endif
    #endregion
    
    delegate TResult Func<T1, T2, TResult>(T1 arg1, T2 arg2);
    #region Access modifier
    #if BACKLINQ_LIB
        public 
    #else
        internal
    #endif
    #endregion
 
    delegate TResult Func<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3);
    #region Access modifier
    #if BACKLINQ_LIB
        public 
    #else
        internal
    #endif
    #endregion
    
    delegate TResult Func<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
}