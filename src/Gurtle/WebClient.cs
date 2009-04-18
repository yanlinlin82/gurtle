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
    using System.IO;
    using System.Net;
    using System.Net.Mime;
    using System.Text;

    #endregion

    internal sealed class WebClient : System.Net.WebClient
    {
        private static readonly string _defaultUserAgentString;

        public new event EventHandler<DownloadStringCompletedEventArgs> DownloadStringCompleted;

        private bool _downloadingString;

        static WebClient()
        {
            var assemblyName = typeof(WebClient).Assembly.GetName();
            _defaultUserAgentString = assemblyName.Name.ToLowerInvariant()
                                    + "/" + assemblyName.Version.ToString(2);
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            if (address == null) throw new ArgumentNullException("address");

            var request = base.GetWebRequest(address);

            var httpRequest = request as HttpWebRequest;
            if (httpRequest != null)
            {
                //
                // If this is an HTTP request and the user agent string
                // has not been set then use a reasonable default that
                // identifies this client.
                //

                if (string.IsNullOrEmpty(httpRequest.UserAgent))
                    httpRequest.UserAgent = DefaultUserAgentString;
            }

            return request;
        }

        public static string DefaultUserAgentString
        {
            get { return _defaultUserAgentString; }
        }

        public new void DownloadStringAsync(Uri address)
        {
            Debug.Assert(address.IsAbsoluteUri);

            try
            {
                _downloadingString = true;
                DownloadDataAsync(address);
            }
            catch (Exception)
            {
                _downloadingString = false;
                throw;
            }
        }

        protected override void OnDownloadDataCompleted(DownloadDataCompletedEventArgs e)
        {
            if (_downloadingString)
            {
                _downloadingString = false;
                
                var result = !e.Cancelled && e.Error == null 
                           ? GetDownloadEncoding().GetString(e.Result) 
                           : null;
                
                var handler = DownloadStringCompleted;
                if (handler != null)
                    DownloadStringCompleted(this, new DownloadStringCompletedEventArgs(result, e.Error, e.Cancelled, e.UserState));
            }
            else
            {
                base.OnDownloadDataCompleted(e);
            }
        }

        private Encoding GetDownloadEncoding() 
        {
            var header = ResponseHeaders[HttpResponseHeader.ContentType];
            if (string.IsNullOrEmpty(header))
                return Encoding;

            ContentType contentType;
            try
            {
                contentType = new ContentType(header);
            }
            catch (FormatException)
            {
                return Encoding;
            }

            try
            {
                return Encoding.GetEncoding(contentType.CharSet);
            }
            catch (ArgumentException)
            {
                return Encoding;
            }
        }
    }
}
