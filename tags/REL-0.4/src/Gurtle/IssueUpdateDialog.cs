namespace Gurtle
{
    #region Imports

    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows.Forms;

    #endregion

    public partial class IssueUpdateDialog : Form
    {
        private string _project;


        public IssueUpdateDialog()
        {
            InitializeComponent();
        }

        public string Project
        {
            get { return _project ?? string.Empty; }
            set { _project = value; }
        }
    
        internal IList<Issue> Issues { get; set; }
        public int Revision { get; set; }

        protected override void OnLoad(System.EventArgs e)
        {
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

            base.OnLoad(e);
        }

        private Control CreateIssuePage(Issue issue) 
        {
            Debug.Assert(issue != null);

            var page = new IssueUpdatePage
            {
                Dock = DockStyle.Fill,
                Summary = issue.Summary,
                Comment = string.Format("Fixed in r{0}.", Revision),
                Url = GoogleCodeProject.FormatUrl(Project, "issues/detail?id={0}", issue.Id)
            };

            page.RevisionClicked += (sender, args) => Process.Start(GoogleCodeProject.FormatUrl(Project, "source/detail?r={0}", args.Revision).ToString());
            
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
