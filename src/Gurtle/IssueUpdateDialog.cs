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
    using System.Windows.Forms;

    #endregion

    public sealed partial class IssueUpdateDialog : Form
    {
        private GoogleCodeProject _project;
        private readonly string _titleFormat;
        private int _revision;

        public IssueUpdateDialog()
        {
            InitializeComponent();

            _titleFormat = Text;
        }

        public string ProjectName
        {
            get { return Project != null ? Project.Name : string.Empty; }
            set { Project = new GoogleCodeProject(value); }
        }

        internal GoogleCodeProject Project
        {
            get { return _project; }
            set { _project = value; UpdateTitle(); }
        }

        private void UpdateTitle()
        {
            Text = string.Format(_titleFormat, Revision, ProjectName);
        }

        internal IList<IssueUpdate> Issues { get; set; }

        public int Revision
        {
            get { return _revision; }
            set { _revision = value; UpdateTitle(); }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (Project == null)
                throw new InvalidOperationException();

            var issues = Issues;

            if (issues != null && issues.Count > 0)
            {
                foreach (var issue in issues)
                {
                    var tab = new TabPage("Issue #" + issue.Issue.Id)
                    {
                        ToolTipText = issue.Issue.Summary,
                        Tag = issue
                    };
                    tab.Controls.Add(CreateIssuePage(issue));
                    tabs.TabPages.Add(tab);
                }
            }

            if (!Project.IsLoaded)
            {
                Project.Loaded += OnProjectLoaded;
                Project.Load();
            }
            else
            {
                OnProjectLoadedImpl();
            }

            base.OnLoad(e);
        }

        private void OnProjectLoaded(object sender, EventArgs e)
        {
            Project.Loaded -= OnProjectLoaded;
            OnProjectLoadedImpl();
        }

        private void OnProjectLoadedImpl()
        {
            foreach (var page in GetPages())
                page.LoadStatusOptions(Project.ClosedStatuses);
        }

        private IEnumerable<IssueUpdatePage> GetPages()
        {
            return from TabPage tab in tabs.TabPages
                   select (IssueUpdatePage) tab.Controls[0];
        }

        private Control CreateIssuePage(IssueUpdate issue) 
        {
            Debug.Assert(issue != null);

            var page = new IssueUpdatePage
            {
                Dock = DockStyle.Fill,
                Summary = issue.Issue.Summary,
                Comment = issue.Comment,
                Status = issue.Status,
                Url = _project.IssueDetailUrl(issue.Issue.Id)
            };

            page.RevisionClicked += (sender, args) => Process.Start(_project.RevisionDetailUrl(args.Revision).ToString());
            
            return page;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (Project != null)
                Project.CancelLoad();

            if (DialogResult == DialogResult.OK)
                OnOK();

            base.OnClosed(e);
        }

        private void OnOK()
        {
            var pages = GetPages().ToArray();
            for (var i = 0; i < pages.Length; i++)
            {
                var page = pages[i];
                var issue = Issues[i];
                issue.Status = page.Status;
                issue.Comment = page.Comment;
            }
        }
    }
}
