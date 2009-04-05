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

    #endregion

    [ Serializable ]
    public sealed class Parameters
    {
        private string _project;
        private string _status;
        private string _user;

        public static Parameters Parse(string str)
        {
            var dict = ParsePairs(str.Split(';')).ToDictionary(
                           p => p.Key, p => p.Value, StringComparer.OrdinalIgnoreCase);

            var result = new Parameters
            {
                Project = dict.TryPop("project"),
                User = dict.TryPop("user"),
                Status = dict.TryPop("status"),
            };

            if (dict.Any())
            {
                throw new ParametersParseException(string.Format(
                    "Parameter '{0}' is unknown.", dict.Keys.First()));
            }

            return result;
        }

        public string Project
        {
            get { return _project ?? string.Empty; }
            
            set
            {
                if (!string.IsNullOrEmpty(value) && !GoogleCodeProject.IsValidProjectName(value))
                    throw new ArgumentException("Invalid project name.", "value");

                _project = value;
            }
        }

        public string User
        {
            get { return _user ?? string.Empty; }
            set { _user = value; }
        }

        public string Status
        {
            get { return _status ?? string.Empty; }
            set { _status = value; }
        }

        public override string ToString()
        {
            var list = new List<KeyValuePair<string, string>>();

            if (Project.Length > 0)
                list.Add(Pair("project", Project));
            
            if (User.Length > 0)
                list.Add(Pair("user", User));
            
            if (Status.Length > 0)
                list.Add(Pair("status", Status));

            return string.Join(";", 
                Pairs(
                    Pair("project", Project), 
                    Pair("user", User), 
                    Pair("status", Status))
                .Where(e => e.Value.Length > 0)
                .Select(e => e.Key + "=" + e.Value)
                .ToArray());
        }

        private static IEnumerable<KeyValuePair<string, string>> Pairs(params KeyValuePair<string, string>[] pairs)
        {
            return pairs;
        }

        private static KeyValuePair<string, string> Pair(string key, string value)
        {
            return new KeyValuePair<string, string>(key, value);
        }

        private static IEnumerable<KeyValuePair<string, string>> ParsePairs(IEnumerable<string> parameters)
        {
            Debug.Assert(parameters != null);

            foreach (var parameter in parameters)
            {
                var pair = parameter.Split(new[] { '=' }, 2);
                var key = pair[0].Trim();
                var value = pair.Length > 1 ? pair[1].Trim() : string.Empty;
                yield return new KeyValuePair<string, string>(key, value);
            }
        }
    }
}
