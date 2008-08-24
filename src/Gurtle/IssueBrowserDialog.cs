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
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    #endregion

    public sealed partial class IssueBrowserDialog : Form
    {
        private string _project;
        private readonly string _titleFormat;
        private readonly string _foundFormat;
        private Action _aborter;
        private long _totalBytesDownloaded;
        private ReadOnlyCollection<int> _selectedIssues;
        private ReadOnlyCollection<Issue> _roSelectedIssueObjects;
        private readonly List<Issue> _selectedIssueObjects;
        private string _userNamePattern;
        private string _statusPattern;
        private bool _closed;
        private WebClient _updateClient;
        private Func<IWin32Window, DialogResult> _upgrade;
        private readonly List<ListViewItem> _issues;

        public IssueBrowserDialog()
        {
            InitializeComponent();

            _titleFormat = Text;
            _foundFormat = foundLabel.Text;

            _issues = new List<ListViewItem>();
            _selectedIssueObjects = new List<Issue>();

            searchFieldBox.SelectedIndex = 0;

            issueListView.ListViewItemSorter = new DelegatingComparer<ListViewItem>(
                (x, y) => ((Issue) x.Tag).Id.CompareTo(((Issue) y.Tag).Id));

            _updateClient = new WebClient();

            UpdateControlStates();
        }

        public string Project
        {
            get { return _project ?? string.Empty; }
            set { _project = value; UpdateTitle(); }
        }

        public string UserNamePattern
        {
            get { return _userNamePattern ?? string.Empty; }
            set { _userNamePattern = value; }
        }

        public string StatusPattern
        {
            get { return _statusPattern ?? string.Empty; }
            set { _statusPattern = value; }
        }

        public bool UpdateCheckEnabled { get; set; }

        public IList<int> SelectedIssues
        {
            get
            {
                if (_selectedIssues == null)
                    _selectedIssues = new ReadOnlyCollection<int>(SelectedIssueObjects.Select(issue => issue.Id).ToList());
                
                return _selectedIssues;
            }
        }

        internal IList<Issue> SelectedIssueObjects
        {
            get
            {
                if (_roSelectedIssueObjects == null)
                    _roSelectedIssueObjects = new ReadOnlyCollection<Issue>(_selectedIssueObjects);

                return _roSelectedIssueObjects;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            if (Project.Length > 0)
                DownloadIssues();

            if (UpdateCheckEnabled)
            {
                var updateClient = new WebClient();

                updateClient.DownloadStringCompleted += (sender, args) =>
                {
                    _updateClient = null;

                    if (_closed || args.Cancelled || args.Error != null)
                        return;

                    var updateAction = _upgrade = OnVersionDataDownloaded(args.Result);

                    if (updateAction == null) 
                        return;
                    
                    updateNotifyIcon.Visible = true;
                    updateNotifyIcon.ShowBalloonTip(15 * 1000);
                };

                updateClient.DownloadStringAsync(new Uri("http://gurtle.googlecode.com/svn/www/update.txt"));
                _updateClient = updateClient;
            }

            base.OnShown(e);
        }

        private static Func<IWin32Window, DialogResult> OnVersionDataDownloaded(string data)
        {
            Debug.Assert(data != null);

            var separators = new[] { ':', '=' };

            var headers = (
                    from line in new StringReader(data).ReadLines()
                    where line.Length > 0 && line[0] != '#'
                    let parts = line.Split(separators, 2)
                    where parts.Length == 2
                    let key = parts[0].Trim()
                    let value = parts[1].Trim()
                    where key.Length > 0 && value.Length > 0
                    let pair = new KeyValuePair<string, string>(key, value)
                    group pair by pair.Key into g
                    select g
                )
                .ToDictionary(g => g.Key, p => p.Last().Value, StringComparer.OrdinalIgnoreCase);

            Version version;

            try
            {
                version = new Version(headers.Find("version"));
                
                //
                // Zero out build and revision if not supplied in the string
                // format, e.g. 2.0 -> 2.0.0.0.
                //

                version = new Version(version.Major, version.Minor, 
                    Math.Max(version.Build, 0), Math.Max(0, version.Revision));
            }
            catch (ArgumentException) { return null; }
            catch (FormatException) { return null; }
            catch (OverflowException) { return null; }

            var href = headers.Find("href").MaskNull();

            if (href.Length == 0 || !Uri.IsWellFormedUriString(href, UriKind.Absolute))
                href = "http://code.google.com/p/gurtle/downloads/list";

            var thisVersion = typeof(Plugin).Assembly.GetName().Version;
            if (version <= thisVersion)
                return null;

            return owner =>
            {
                var message = new StringBuilder()
                    .AppendLine("There is a new version of Gurtle available. Would you like to update now?")
                    .AppendLine()
                    .Append("Your version: ").Append(thisVersion).AppendLine()
                    .Append("New version: ").Append(version).AppendLine()
                    .ToString();

                var reply = MessageBox.Show(owner, message,
                    "Update Notice", MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                if (reply == DialogResult.Yes)
                    Process.Start(href);

                return reply;
            };
        }

        private void UpdateNotifyIcon_Click(object sender, EventArgs e)
        {
            Debug.Assert(_upgrade != null);

            var reply = _upgrade(this);

            if (reply == DialogResult.Cancel)
                return;

            updateNotifyIcon.Visible = false;

            if (reply == DialogResult.Yes)
                Close();
        }

        private void DownloadIssues()
        {
            Debug.Assert(_aborter == null);

            refreshButton.Enabled = false;
            workStatus.Visible = true;
            statusLabel.Text = "Downloading\x2026";

            _aborter = DownloadIssues(Project, 0, 
                                      OnIssuesDownloaded, 
                                      OnUpdateProgress, 
                                      OnDownloadComplete);
        }

        private void OnDownloadComplete(bool cancelled, Exception e)
        {
            if (_closed)
                return; // orphaned notification

            _aborter = null;
            refreshButton.Enabled = true;
            workStatus.Visible = false;

            if (cancelled)
            {
                statusLabel.Text = "Download aborted";
                return;
            }

            if (e != null)
            {
                statusLabel.Text = "Error downloading";
                MessageBox.Show(this, e.Message, "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            statusLabel.Text = string.Format("{0} issue(s) downloaded", _issues.Count.ToString("N0"));
            UpdateTitle();
        }

        private void OnUpdateProgress(DownloadProgressChangedEventArgs args)
        {
            if (_closed)
                return; // orphaned notification

            _totalBytesDownloaded += args.BytesReceived;

            statusLabel.Text = string.Format("Downloading\x2026{0} bytes transferred", 
                _totalBytesDownloaded.ToString("N0"));
            
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            Text = string.Format(_titleFormat, Project, _issues.Count.ToString("N0"));
        }

        private IEnumerable<Issue> GetSelectedIssuesFromListView()
        {
            return issueListView.CheckedItems
                   .Cast<ListViewItem>()
                   .Select(item => (Issue) item.Tag);
        }

        private void IssueListView_DoubleClick(object sender, EventArgs e)
        {
            AcceptButton.PerformClick();
        }

        private void DetailButton_Click(object sender, EventArgs e)
        {
            var issue = GetSelectedIssuesFromListView().FirstOrDefault();
            if (issue != null)
                ShowIssueDetails(issue);
        }

        private void ShowIssueDetails(Issue issue)
        {
            Debug.Assert(issue != null);

            Process.Start(
                string.Format("http://code.google.com/p/{0}/issues/detail?id={1}",
                Project, issue.Id.ToString(CultureInfo.InvariantCulture)));
        }

        private void IssueListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            UpdateControlStates();
        }

        private void IssueListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControlStates();
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            issueListView.Items.Clear();
            ListIssues(_issues);
        }

        private void UpdateControlStates()
        {
            detailButton.Enabled = issueListView.SelectedItems.Count == 1;
            okButton.Enabled = issueListView.CheckedItems.Count > 0;
        }

        protected override void OnClosed(EventArgs e)
        {
            Debug.Assert(!_closed);

            Release(ref _aborter, a => a());
            Release(ref _updateClient, uc => uc.CancelAsync());

            _closed = true;

            base.OnClosed(e);
        }

        private static void Release<T>(ref T member, Action<T> free) where T : class
        {
            Debug.Assert(free != null);

            var local = member;
            if (local == null)
                return;
            member = null;
            free(local);
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            _selectedIssueObjects.AddRange(GetSelectedIssuesFromListView());
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            _issues.Clear();
            issueListView.Items.Clear();
            DownloadIssues();
        }

        private bool OnIssuesDownloaded(IEnumerable<Issue> issues)
        {
            Debug.Assert(issues != null);

            if (_closed)
                return false; // orphaned notification

            if (UserNamePattern.Length > 0)
                issues = issues.Where(issue => Regex.IsMatch(issue.Owner, UserNamePattern));

            if (StatusPattern.Length > 0)
                issues = issues.Where(issue => Regex.IsMatch(issue.Status, StatusPattern));

            var items = issues.Select(issue =>
                {
                    var id = issue.Id.ToString(CultureInfo.InvariantCulture);

                    var item = new ListViewItem(id)
                    {
                        Tag = issue,
                        UseItemStyleForSubItems = true
                    };

                    if (!issue.HasOwner)
                        item.ForeColor = SystemColors.GrayText;

                    var subItems = item.SubItems;
                    subItems.Add(issue.Type);
                    subItems.Add(issue.Status);
                    subItems.Add(issue.Priority);
                    subItems.Add(issue.Owner);
                    subItems.Add(issue.Summary);

                    return item;
                })
                .ToArray();

            if (items.Length == 0)
                return false;

            _issues.AddRange(items);
            ListIssues(items);

            return true;
        }

        private void ListIssues(IEnumerable<ListViewItem> items)
        {
            Debug.Assert(items != null);

            var searchWords = searchBox.Text.Split().Where(s => s.Length > 0);
            if (searchWords.Any())
            {
                items = from item in items
                        let issue = (Issue)item.Tag
                        where searchWords.All(word => issue.Summary.IndexOf(word, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        select item;
            }

            //
            // We need to stop listening to the ItemChecked event because it 
            // is raised for each item added and this has visually noticable 
            // performance implications for the user on large lists.
            //

            ItemCheckedEventHandler onItemChecked = IssueListView_ItemChecked;
            issueListView.ItemChecked -= onItemChecked;

            issueListView.Items.AddRange(items.ToArray());

            //
            // Update control states once and start listening to the 
            // ItemChecked event once more.
            //
            
            UpdateControlStates();
            issueListView.ItemChecked += onItemChecked;

            foundLabel.Text = string.Format(_foundFormat, issueListView.Items.Count.ToString("N0"));
            foundLabel.Visible = searchWords.Any();
        }

        private static Action DownloadIssues(string project, int start, 
            Func<IEnumerable<Issue>, bool> onData, 
            Action<DownloadProgressChangedEventArgs> onProgress,
            Action<bool, Exception> onCompleted)
        {
            Debug.Assert(project != null);
            Debug.Assert(onData != null);

            var client = new WebClient();

            Action<int> pager = next => client.DownloadStringAsync(
                new Uri(string.Format("http://code.google.com/p/{0}/issues/csv?start={1}&colspec={2}",
                    project, next.ToString(CultureInfo.InvariantCulture),
                    string.Join("%20", Enum.GetNames(typeof(IssueField))))));

            client.DownloadStringCompleted += (sender, args) =>
            {
                if (args.Cancelled || args.Error != null)
                {
                    if (onCompleted != null) 
                        onCompleted(args.Cancelled, args.Error);
                    
                    return;
                }

                var issues = IssueTableParser.Parse(new StringReader(args.Result)).ToArray();
                var more = onData(issues);

                if (more)
                {
                    start += issues.Length;
                    pager(start);
                }
                else
                {
                    if (onCompleted != null) 
                        onCompleted(false, null);
                }
            };

            if (onProgress != null)
                client.DownloadProgressChanged += (sender, args) => onProgress(args);

            pager(start);

            return client.CancelAsync;
        }
    }
}
