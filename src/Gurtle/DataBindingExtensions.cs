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
    using System.Linq;
    using System.Web.UI;

    #endregion

    /// <summary>
    /// Provides data expression evaluation facilites similar to 
    /// <see cref="System.Web.UI.DataBinder"/> in ASP.NET.
    /// </summary>
    
    internal static class DataBindingExtensions
    {
        public static T DataBind<T>(this object obj, string expression)
        {
            var value = obj.DataBind(expression);
            return (T) (Convert.IsDBNull(value) ? null : value);
        }

        /// <summary>
        /// Evaluates data-binding expressions at run time using an expression
        /// syntax that is similar to C# and Visual Basic for accessing
        /// properties or indexing into collections.
        /// </summary>
        /// <remarks>
        /// This method performs late-bound evaluation, using run-time reflection
        /// and therefore can cause performance less than optimal.
        /// </remarks>

        public static object DataBind(this object obj, string expression)
        {
            //
            // The ASP.NET DataBinder.Eval method does not like an empty or null
            // expression. Rather than making it an unnecessary exception, we
            // turn a nil-expression to mean, "evaluate to obj."
            //

            return string.IsNullOrEmpty(expression) ? obj : DataBinder.Eval(obj, expression);
        }

        public static string DataBind(this object obj, string format, params string[] expressions)
        {
            if (expressions == null) throw new ArgumentNullException("expressions");

            return string.Format(format, expressions.Select(e => obj.DataBind(e)).ToArray());
        }
    }
}