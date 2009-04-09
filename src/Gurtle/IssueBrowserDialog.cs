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
    using System.Collections;
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
    using IssueListViewItem = ListViewItem<Issue>;

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
        private readonly ObservableCollection<IssueListViewItem> _issues;
        private readonly ListViewSorter<IssueListViewItem, Issue> _sorter;
        private WebClient _issueOptionsClient;
        private readonly Font _deadFont;
        private ICollection<string> _closedStatuses;

        public IssueBrowserDialog()
        {
            InitializeComponent();

            _titleFormat = Text;
            _foundFormat = foundLabel.Text;

            var issues = _issues = new ObservableCollection<IssueListViewItem>();
            issues.ItemAdded += (sender, args) => ListIssues(Enumerable.Repeat(args.Item, 1));
            issues.ItemsAdded += (sender, args) => ListIssues(args.Items);
            issues.ItemRemoved += (sender, args) => issueListView.Items.Remove(args.Item);
            issues.Cleared += delegate { issueListView.Items.Clear(); };

            _selectedIssueObjects = new List<Issue>();

            _deadFont = new Font(issueListView.Font, FontStyle.Strikeout);
            _closedStatuses = new string[0];

            _sorter = new ListViewSorter<IssueListViewItem, Issue>(issueListView, 
                          item => item.Tag, 
                          new Func<Issue, IComparable>[] {
                              issue => (IComparable) issue.Id,
                              issue => (IComparable) issue.Type,
                              issue => (IComparable) issue.Status,
                              issue => (IComparable) issue.Priority,
                              issue => (IComparable) issue.Owner,
                              issue => (IComparable) issue.Summary
                          }
                      );
            _sorter.AutoHandle();
            _sorter.SortByColumn(0);

            var searchSourceItems = searchFieldBox.Items;
            searchSourceItems.Add(new MultiFieldIssueSearchSource("All fields", MetaIssue.Properties));

            foreach (IssueField field in Enum.GetValues(typeof(IssueField)))
            {
                searchSourceItems.Add(new SingleFieldIssueSearchSource(field.ToString(), MetaIssue.GetPropertyByField(field),
                    field == IssueField.Summary
                    || field == IssueField.Id
                    || field == IssueField.Stars
                    ? SearchableStringSourceCharacteristics.None
                    : SearchableStringSourceCharacteristics.Predefined));
            }

            searchFieldBox.SelectedIndex = 0;

            searchBox.EnableShortcutToSelectAllText();

            _updateClient = new WebClient();

            includeClosedCheckBox.DataBindings.Add("Enabled", refreshButton, "Enabled");

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
            {
                DownloadIssues();
                DownloadIssueOptions();
            }

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
                href = new Uri(GoogleCodeProject.ProjectUrlFromName("gurtle"), "downloads/list").ToString();

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

        private void DownloadIssueOptions()
        {
            var client = new WebClient();

            client.DownloadStringCompleted += (sender, args) =>
            {
                _issueOptionsClient = null;

                if (_closed || args.Cancelled || args.Error != null)
                    return;

                var contentType = client.ResponseHeaders[HttpResponseHeader.ContentType]
                                        .MaskNull().Split(new[] { ';' }, 2)[0];

                var jsonContentTypes = new[] {
                    "application/json", 
                    "application/x-javascript", 
                    "text/javascript",
                };

                if (!jsonContentTypes.Any(s => s.Equals(contentType, StringComparison.OrdinalIgnoreCase)))
                    return;

                using (var sc = new ScriptControl { Language = "JavaScript" })
                {
                    var data = sc.Eval("(" + args.Result + ")"); // TODO: JSON sanitization

                    _closedStatuses = new OleDispatchDriver(data)
                       .Get<IEnumerable>("closed")
                       .Cast<object>()
                       .Select(o => new OleDispatchDriver(o).Get<string>("name"))
                       .ToArray();
                }
            };

            client.DownloadStringAsync(new Uri(GoogleCodeProject.ProjectUrlFromName(Project), "feeds/issueOptions"));
            _issueOptionsClient = client;
        }

        private void DownloadIssues()
        {
            Debug.Assert(_aborter == null);

            refreshButton.Enabled = false;
            workStatus.Visible = true;
            statusLabel.Text = "Downloading\x2026";

            _aborter = DownloadIssues(Project, 0, includeClosedCheckBox.Checked,
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

            statusLabel.Text = string.Format("Downloading\x2026{0} transferred",
                ByteSizeFormatter.StrFormatByteSize(_totalBytesDownloaded));
            
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            Text = string.Format(_titleFormat, Project, _issues.Count.ToString("N0"));
        }

        private IEnumerable<Issue> GetSelectedIssuesFromListView()
        {
            return GetSelectedIssuesFromListView(null);
        }

        private IEnumerable<Issue> GetSelectedIssuesFromListView(Func<ListView, IEnumerable> itemsSelector)
        {
            return from IssueListViewItem item 
                   in (itemsSelector != null ? itemsSelector(issueListView) : issueListView.CheckedItems)
                   select item.Tag;
        }

        private void IssueListView_DoubleClick(object sender, EventArgs e)
        {
            AcceptButton.PerformClick();
        }

        private void DetailButton_Click(object sender, EventArgs e)
        {
            var issue = GetSelectedIssuesFromListView(lv => lv.SelectedItems).FirstOrDefault();
            if (issue != null)
                ShowIssueDetails(issue);
        }

        private void ShowIssueDetails(Issue issue)
        {
            Debug.Assert(issue != null);

            Process.Start(new Uri(GoogleCodeProject.ProjectUrlFromName(Project), 
                "issues/detail?id=" + issue.Id.ToString(CultureInfo.InvariantCulture)).ToString());
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

        private void SearchFieldBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var provider = (ISearchSourceStringProvider<Issue>)searchFieldBox.SelectedItem;
            if (provider == null)
                return;

            var definitions = searchBox.Items;
            definitions.Clear();

            var isPredefined = SearchableStringSourceCharacteristics.Predefined == (
                provider.SourceCharacteristics & SearchableStringSourceCharacteristics.Predefined);

            if (!isPredefined)
                return;

            // TODO: Update definitions if issues are still being downloaded

            definitions.AddRange(_issues
                .Select(lvi => provider.ToSearchableString(lvi.Tag))
                .Distinct(StringComparer.CurrentCultureIgnoreCase).ToArray());
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
            Release(ref _updateClient, wc => wc.CancelAsync());
            Release(ref _issueOptionsClient, wc => wc.CancelAsync());

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
        }
        
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            if (DialogResult != DialogResult.OK || e.Cancel)
                return;

            var selectedIssues = _issues.Where(lvi => lvi.Checked)
                                        .Select(lvi => lvi.Tag)
                                        .ToArray();

            //
            // If the user has selected unowned or closed issues then
            // provide a warning and confirm the intention.
            //

            var warnIssues = selectedIssues.Where(issue => !issue.HasOwner 
                                                           || _closedStatuses.Any(s => issue.Status.Equals(s, StringComparison.InvariantCultureIgnoreCase)));
            if (warnIssues.Any())
            {
                var message = string.Format(
                    "You selected one or more unowned or closed issues ({0}). "
                    + "Normally you should only select open issues with owners assigned. "
                    + "Proceed anyway?",
                    string.Join(", ", warnIssues.OrderBy(issue => issue.Id)
                                                .Select(issue => "#" + issue.Id).ToArray()));
                
                var reply = MessageBox.Show(this, message, "Unowned/Closed Issues Warning",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, 
                    /* no */ MessageBoxDefaultButton.Button2);

                if (reply == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }

            }

            _selectedIssueObjects.AddRange(selectedIssues);
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            _issues.Clear();            
            OnIssuesDownloaded(Enumerable.Empty<Issue>());
            UpdateTitle();
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

                    var item = new IssueListViewItem(id)
                    {
                        Tag = issue,
                        UseItemStyleForSubItems = true
                    };

                    Debug.Assert(_closedStatuses != null);
                    if (_closedStatuses.Any(s => issue.Status.Equals(s, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        item.ForeColor = SystemColors.GrayText;
                        item.Font = _deadFont;
                    }
                    else if (!issue.HasOwner)
                    {
                        item.ForeColor = SystemColors.GrayText;
                    }

                    var subItems = item.SubItems;
                    subItems.Add(issue.Type);
                    subItems.Add(issue.Status);
                    subItems.Add(issue.Priority);
                    subItems.Add(issue.Owner);
                    subItems.Add(issue.Summary);

                    return item;
                })
                .ToArray();

            _issues.AddRange(items);

            return items.Length > 0;
        }

        private void ListIssues(IEnumerable<IssueListViewItem> items)
        {
            Debug.Assert(items != null);

            var searchWords = searchBox.Text.Split().Where(s => s.Length > 0);
            if (searchWords.Any())
            {
                var provider = (ISearchSourceStringProvider<Issue>) searchFieldBox.SelectedItem;
                items = from item in items
                        let issue = item.Tag
                        where searchWords.All(word => provider.ToSearchableString(issue).IndexOf(word, StringComparison.CurrentCultureIgnoreCase) >= 0)
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

        private static Action DownloadIssues(string project, int start, bool includeClosedIssues,
            Func<IEnumerable<Issue>, bool> onData, 
            Action<DownloadProgressChangedEventArgs> onProgress,
            Action<bool, Exception> onCompleted)
        {
            Debug.Assert(project != null);
            Debug.Assert(onData != null);

            var client = new WebClient();

            Action<int> pager = next => client.DownloadStringAsync(
                new Uri(GoogleCodeProject.ProjectUrlFromName(project), string.Format("issues/csv?start={0}&colspec={1}{2}",
                    next.ToString(CultureInfo.InvariantCulture),
                    string.Join("%20", Enum.GetNames(typeof(IssueField))),
                    includeClosedIssues ? "&can=1" : string.Empty)));

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

        [ Serializable, Flags ]
        private enum SearchableStringSourceCharacteristics
        {
            None,
            Predefined
        }

        /// <summary>
        /// Represents a provider that yields the string for an object that 
        /// can be used in text-based searches and indexing.
        /// </summary>

        private interface ISearchSourceStringProvider<T>
        {
            SearchableStringSourceCharacteristics SourceCharacteristics { get; }
            string ToSearchableString(T item);
        }

        /// <summary>
        /// Base class for transforming an <see cref="Issue"/> into a 
        /// searchable string.
        /// </summary>

        private abstract class IssueSearchSource : ISearchSourceStringProvider<Issue>
        {
            private readonly string _label;

            protected IssueSearchSource(string label, SearchableStringSourceCharacteristics sourceCharacteristics)
            {
                Debug.Assert(label != null);
                Debug.Assert(label.Length > 0);

                _label = label;
                SourceCharacteristics = sourceCharacteristics;
            }

            public SearchableStringSourceCharacteristics SourceCharacteristics { get; private set; }
            public abstract string ToSearchableString(Issue issue);

            public override string ToString()
            {
                return _label;
            }
        }

        /// <summary>
        /// An <see cref="IssueSearchSource"/> implementation that uses a 
        /// property of an <see cref="Issue"/> as the searchable string.
        /// </summary>

        private sealed class SingleFieldIssueSearchSource : IssueSearchSource
        {
            private readonly IProperty<Issue> _property;

            public SingleFieldIssueSearchSource(string label, IProperty<Issue> property) : 
                this(label, property, SearchableStringSourceCharacteristics.None) {}

            public SingleFieldIssueSearchSource(string label, IProperty<Issue> property, 
                SearchableStringSourceCharacteristics sourceCharacteristics) :
                base(label, sourceCharacteristics)
            {
                Debug.Assert(property != null);
                _property = property;
            }

            public override string ToSearchableString(Issue issue)
            {
                Debug.Assert(issue != null);
                return _property.GetValue(issue).ToString();
            }
        }

        /// <summary>
        /// An <see cref="IssueSearchSource"/> implementation that uses 
        /// concatenates multiple properties of an <see cref="Issue"/> as 
        /// the searchable string.
        /// </summary>

        private sealed class MultiFieldIssueSearchSource : IssueSearchSource
        {
            private readonly IProperty<Issue>[] _properties;

            public MultiFieldIssueSearchSource(string label, IEnumerable<IProperty<Issue>> properties) :
                base(label, SearchableStringSourceCharacteristics.None)
            {
                Debug.Assert(properties != null);
                _properties = properties.Where(p => p != null).ToArray();
            }

            public override string ToSearchableString(Issue issue)
            {
                Debug.Assert(issue != null);

                return _properties.Aggregate(new StringBuilder(),
                    (sb, p) => sb.Append(p.GetValue(issue)).Append(' ')).ToString();
            }
        }
    }
}
