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
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    #endregion

    internal static class IssueTableParser
    {
        public static readonly Regex _csvex = new Regex(
            "(?:^|,)\"((?:\"{2}|[^\"])+)\"",
            RegexOptions.CultureInvariant
            | RegexOptions.IgnorePatternWhitespace
            | RegexOptions.Compiled);

        public static IEnumerable<Issue> Parse(TextReader reader)
        {
            Debug.Assert(reader != null);

            var lines = reader.ReadLines().ToArray();

            if (lines.Length == 0)
                return Enumerable.Empty<Issue>();

            var headers = ParseValues(lines[0]).ToArray();

            var bindings = Enum.GetNames(typeof(IssueField))
                .Select(n => Array.FindIndex(headers, h => n.Equals(h, StringComparison.OrdinalIgnoreCase)))
                .Select(i => (Func<IEnumerable<string>, string>)(values => values.ElementAtOrDefault(i) ?? string.Empty))
                .ToArray();

            return //...
                from line in lines.Skip(1)
                let values = ParseValues(line).ToArray()
                let id = ParseInteger(bindings[(int)IssueField.Id](values), CultureInfo.InvariantCulture)
                where id != null && id.Value > 0
                let issue = new Issue 
                {
                    Id = id.Value, 
                    Type = bindings[(int)IssueField.Type](values), 
                    Status = bindings[(int)IssueField.Status](values), 
                    Milestone = bindings[(int)IssueField.Milestone](values), 
                    Priority = bindings[(int)IssueField.Priority](values), 
                    Stars = bindings[(int)IssueField.Stars](values), 
                    Owner = bindings[(int)IssueField.Owner](values), 
                    Summary = bindings[(int)IssueField.Summary](values)
                }
                select issue;
        }

        private static IEnumerable<string> ParseValues(string line)
        {
            Debug.Assert(line != null);

            return from Match m in _csvex.Matches(line)
                   select m.Groups[1].Value.Replace("\"\"", "\"");
        }

        private static int? ParseInteger(string s, IFormatProvider provider)
        {
            int result;
            return int.TryParse(s, NumberStyles.Integer, provider, out result) ? result : (int?) null;
        }
    }
}
