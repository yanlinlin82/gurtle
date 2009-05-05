namespace Gurtle
{
    #region Imports

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
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
                        ToolTipText = issue.Summary,
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
            foreach (TabPage tab in tabs.TabPages)
                yield return (IssueUpdatePage) tab.Controls[0];
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

        protected override void OnClosing(CancelEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
                e.Cancel = !OnOK();

            base.OnClosing(e);
        }

        private bool OnOK()
        {
            var credential = CredentialPrompt.Prompt(this, "Google Code", "ggcred.txt");
            if (credential == null)
                return false;

            var updates = GetPages().Select((p, i) => new
            {
                Issue = Issues[i], 
                p.Comment, 
                p.Status
            });

            foreach (var update in updates)
                UpdateIssue(update.Issue, update.Comment, update.Status, credential, this);

            return true;
        }

        private bool UpdateIssue(Issue issue, string comment, string status, NetworkCredential credential, ISynchronizeInvoke sync)
        {
            string commentPath = null;
            
            if (comment.IndexOfAny(new[] { '\r', '\n', '\f' }) >= 0)
            {
                commentPath = Path.GetTempFileName();
                File.WriteAllText(commentPath, comment, Encoding.UTF8);
                comment = "@" + commentPath;
            }
            else
            {
                comment = "\"" + comment.Replace("\"", "\"\"") + "\"";
            }

            var args = new[]
            {
                credential.UserName, 
                credential.Password, 
                issue.Id.ToString(CultureInfo.InvariantCulture), 
                status, 
                comment
            };

            var script = Process.Start(new ProcessStartInfo
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                FileName = Environment.GetEnvironmentVariable("GURTLE_ISSUE_UPDATE_SCRIPT"),
                Arguments = string.Join(" ", args),
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            });

            var stdout = new StringWriter();
            var stderr = new StringWriter();
            script.SynchronizingObject = sync;
            script.OutputDataReceived += (sender, e) => stdout.WriteLine(e.Data);
            script.ErrorDataReceived += (sender, e) => stderr.WriteLine(e.Data);
            script.BeginOutputReadLine();
            script.BeginErrorReadLine();

            script.WaitForExit();

            Trace.TraceInformation(stdout.ToString());

            var success = 0 == script.ExitCode;

            if (success)
                Trace.TraceWarning(stderr.ToString());
            else
                Trace.TraceError(stderr.ToString());

            if (!string.IsNullOrEmpty(commentPath) && File.Exists(commentPath))
                File.Delete(commentPath);

            return success;
        }
    }
}
