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
    using System.Text;
    using System.Runtime.InteropServices;

    #endregion

    /// <summary>
    /// Provides formatting for a numeric value into a string that 
    /// represents the number expressed as a size value in bytes, 
    /// kilobytes, megabytes, or gigabytes, depending on the size.
    /// </summary>
    /// <remarks>
    /// This formatter relies on base operating system services
    /// for formatting and therefore does not consider the 
    /// current UI culture for any text returned in the formatted
    /// string.
    /// </remarks>

    internal sealed class ByteSizeFormatter : CustomFormatter
    {
        public static readonly ByteSizeFormatter Default = new ByteSizeFormatter();

        protected override string FormatImpl(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg == null) throw new ArgumentNullException("arg");

            return !string.IsNullOrEmpty(format) 
                   ? BaseFormat(format, arg, formatProvider) 
                   : StrFormatByteSize(Convert.ToInt64(arg));
        }

        public static string StrFormatByteSize(long size)
        {
            var buffer = new StringBuilder(20);
            StrFormatByteSize(size, buffer, 20);
            return buffer.ToString();
        }

        [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
        private static extern long StrFormatByteSize(long size, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer, int bufferSize);
    }
}
