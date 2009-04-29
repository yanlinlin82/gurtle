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

    using System.Diagnostics;

    #endregion

    internal static class StringExtensions
    {
        /// <summary>
        /// Masks an empty string with a given mask such that the result
        /// is never an empty string. If the input string is null or
        /// empty then it is masked, otherwise the original is returned.
        /// </summary>
        /// <remarks>
        /// Use this method to guarantee that you never get an empty
        /// string. Note that if the mask itself is an empty string then
        /// this method could yield an empty string!
        /// </remarks>

        [DebuggerStepThrough]
        public static string MaskEmpty(this string str, string mask)
        {
            return !string.IsNullOrEmpty(str) ? str : mask;
        }

        [DebuggerStepThrough]
        public static string MaskEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str) ? str : string.Empty;
        }

        /// <summary>
        /// Masks the null value. If the given string is null then the 
        /// result is an empty string otherwise it is the original string.
        /// </summary>
        /// <remarks>
        /// Use this method to guarantee that you never get a null string
        /// and where the distinction between a null and an empty string
        /// is irrelevant.
        /// </remarks>

        [DebuggerStepThrough]
        public static string MaskNull(this string str)
        {
            return str ?? string.Empty;
        }

        [DebuggerStepThrough]
        public static string MaskNull(this string str, string mask)
        {
            return str ?? mask.MaskNull();
        }

        /// <summary>
        /// Returns a section of a string.
        /// </summary>
        /// <remarks>
        /// The slice method copies up to, but not including, the element
        /// indicated by end. If start is negative, it is treated as length +
        /// start where length is the length of the string. If end is negative,
        /// it is treated as length + end where length is the length of the
        /// string. If end occurs before start, no characters are copied to the
        /// new string.
        /// </remarks>

        public static string Slice(this string str, int? start, int? end)
        {
            return Slice(str, start ?? 0, end ?? str.MaskNull().Length);
        }

        public static string Slice(this string str, int start)
        {
            return Slice(str, start, null);
        }

        public static string Slice(this string str, int start, int end)
        {
            var length = str.MaskNull().Length;

            if (start < 0)
            {
                start = length + start;
                if (start < 0)
                    start = 0;
            }
            else
            {
                if (start > length)
                    start = length;
            }

            if (end < 0)
            {
                end = length + end;
                if (end < 0)
                    end = 0;
            }
            else
            {
                if (end > length)
                    end = length;
            }

            var sliceLength = end - start;

            return sliceLength > 0 ?
                str.Substring(start, sliceLength) : string.Empty;
        }
    }
}
