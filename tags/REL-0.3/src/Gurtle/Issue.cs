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
    using System;
    using System.Linq;
    using System.Text;

    [Serializable]
    internal sealed class Issue
    {
        private string _type;
        private string _status;
        private string _milestone;
        private string _priority;
        private string _stars;
        private string _owner;
        private string _summary;

        public int Id { get; set; }
        public string Type { get { return _type ?? string.Empty; } set { _type = value; } }
        public string Status { get { return _status ?? string.Empty; } set { _status = value; } }
        public string Milestone { get { return _milestone ?? string.Empty; } set { _milestone = value; } }
        public string Priority { get { return _priority ?? string.Empty; } set { _priority = value; } }
        public string Stars { get { return _stars ?? string.Empty; } set { _stars = value; } }
        public string Owner { get { return _owner ?? string.Empty; } set { _owner = value; } }
        public string Summary { get { return _summary ?? string.Empty; } set { _summary = value; } }

        public bool HasOwner
        {
            get
            {
                var owner = this.Owner;
                return owner.Length > 0 && !owner.All(ch => ch == '-');
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("{ Id = ").Append(Id);
            builder.Append(", Type = ").Append(Type);
            builder.Append(", Status = ").Append(Status);
            builder.Append(", Milestone = ").Append(Milestone);
            builder.Append(", Priority = ").Append(Priority);
            builder.Append(", Stars = ").Append(Stars);
            builder.Append(", Owner = ").Append(Owner);
            builder.Append(", Summary = ").Append(Summary);
            builder.Append(" }");
            return builder.ToString();
        }
    }
}