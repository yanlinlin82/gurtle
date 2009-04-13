namespace Gurtle
{
    using System.Windows.Forms;

    internal partial class IssueUpdatePage : UserControl
    {
        public Issue Issue { get; set; }

        public IssueUpdatePage()
        {
            InitializeComponent();
        }

        protected override void OnLoad(System.EventArgs e)
        {
            var issue = Issue;

            if (issue != null)
            {
                summaryLabel.Text = issue.Summary;
                commentBox.Text = string.Format("Fixed issue #{0}.", issue.Id);
            }

            base.OnLoad(e);
        }
    }
}
