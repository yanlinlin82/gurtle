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
    using System.Collections.ObjectModel;
 
    #endregion

    [ Serializable ]
    internal static class MetaIssue
    {
        public static readonly IProperty<Issue, int> Id = new Property<Issue, int>(issue => issue.Id, (issue, value) => issue.Id = value);
        public static readonly IProperty<Issue, string> Type = new Property<Issue, string>(issue => issue.Type, (issue, value) => issue.Type = value);
        public static readonly IProperty<Issue, string> Status = new Property<Issue, string>(issue => issue.Status, (issue, value) => issue.Status = value);
        public static readonly IProperty<Issue, string> Milestone = new Property<Issue, string>(issue => issue.Milestone, (issue, value) => issue.Milestone = value);
        public static readonly IProperty<Issue, string> Priority = new Property<Issue, string>(issue => issue.Priority, (issue, value) => issue.Priority = value);
        public static readonly IProperty<Issue, string> Stars = new Property<Issue, string>(issue => issue.Stars, (issue, value) => issue.Stars = value);
        public static readonly IProperty<Issue, string> Owner = new Property<Issue, string>(issue => issue.Owner, (issue, value) => issue.Owner = value);
        public static readonly IProperty<Issue, string> Summary = new Property<Issue, string>(issue => issue.Summary, (issue, value) => issue.Summary = value);

        public static ReadOnlyCollection<IProperty<Issue>> Properties { get; private set; }

        static MetaIssue()
        {
            Properties = new ReadOnlyCollection<IProperty<Issue>>(new IProperty<Issue>[]
            {
                Id,
                Type,
                Status,
                Milestone,
                Priority,
                Stars,
                Owner,
                Summary
            });
        }

        public static IProperty<Issue> GetPropertyByField(IssueField field)
        {
            var index = (int) field;

            if (index < 0 || index >= Properties.Count)
                throw new ArgumentOutOfRangeException("field", field, null);
            
            return Properties[index];
        }
    }
}