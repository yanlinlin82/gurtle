namespace Gurtle
{
    #region Imports

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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

        internal IList<Issue> Issues { get; set; }

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
                    var tab = new TabPage("Issue #" + issue.Id)
                    {
                        ToolTipText = issue.Summary
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
            foreach (TabPage tab in tabs.TabPages)
                ((IssueUpdatePage) tab.Controls[0]).LoadStatusOptions(Project.ClosedStatuses);
        }

        private Control CreateIssuePage(Issue issue) 
        {
            Debug.Assert(issue != null);

            var page = new IssueUpdatePage
            {
                Dock = DockStyle.Fill,
                Summary = issue.Summary,
                Comment = string.Format("Fixed in r{0}.", Revision),
                Url = _project.IssueDetailUrl(issue.Id)
            };

            page.RevisionClicked += (sender, args) => Process.Start(_project.RevisionDetailUrl(args.Revision).ToString());
            
            return page;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                new CredentialsDialog { Realm = "Google Code" }.ShowDialog(this);
            }
            base.OnClosing(e);
        }
    }
}
