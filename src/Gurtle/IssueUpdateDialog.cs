namespace Gurtle
{
    using System.Windows.Forms;

    public partial class IssueUpdateDialog : Form
    {
        public IssueUpdateDialog()
        {
            InitializeComponent();

            var page = new TabPage("Issue #1");
            page.Controls.Add(new IssueUpdatePage() { Dock = DockStyle.Fill });
            tabs.TabPages.Add(page);

            page = new TabPage("Issue #1");
            page.Controls.Add(new IssueUpdatePage() { Dock = DockStyle.Fill });
            tabs.TabPages.Add(page);
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
