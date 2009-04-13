namespace Gurtle
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    public partial class IssueUpdateDialog : Form
    {
        public IssueUpdateDialog()
        {
            InitializeComponent();
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
                    var page = new TabPage("Issue #" + issue.Id);
                    page.Controls.Add(new IssueUpdatePage
                    {
                        Dock = DockStyle.Fill,
                        Issue = issue
                    });
                    tabs.TabPages.Add(page);
                }
            }

            base.OnLoad(e);
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
