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
        public int Id { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Milestone { get; set; }
        public string Priority { get; set; }
        public string Stars { get; set; }
        public string Owner { get; set; }
        public string Summary { get; set; }

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