namespace Gurtle
{
    #region Imports

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    #endregion

    internal sealed partial class IssueUpdatePage : UserControl
    {
        public event EventHandler<RevisionEventArgs> RevisionClicked;

        public IssueUpdatePage()
        {
            InitializeComponent();
        }

        [DefaultValue("")]
        public string Summary
        {
            get { return summaryLabel.Text; }
            set { summaryLabel.Text = value; }
        }

        [Browsable(false)]
        public Uri Url { get; set; }

        [DefaultValue("")]
        public string Comment
        {
            get { return commentBox.Text; }
            set { commentBox.Text = value; }
        }

        public void LoadStatusOptions(IEnumerable<string> options)
        {
            var items = statusBox.Items;
            items.Clear();
            items.AddRange(options.ToArray());
        }

        private void Summary_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Url == null)
                return;

            Process.Start(Url.ToString());
        }

        private void RevisionsLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OnRevisionClicked(new RevisionEventArgs((int) e.Link.LinkData));
        }

        private void OnRevisionClicked(RevisionEventArgs args)
        {
            Debug.Assert(args != null);
            var handler = RevisionClicked;
            if (handler != null) handler(this, args);
        }

        private void CommentBox_TextChanged(object sender, EventArgs e)
        {
            var label = revisionsLabel;
            var re = new Regex(@"\br([0-9]{1,6})\b", RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace);
            var matches = re.Matches(commentBox.Text);
            label.Visible = matches.Count > 0;
            label.Text = "Revisions: " + string.Join(", ", matches.Cast<Match>().Select(m => m.Value).Distinct().ToArray());
            var links = label.Links;
            links.Clear();
            foreach (Match match in re.Matches(label.Text))
                links.Add(match.Index, match.Length, int.Parse(match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture));
        }

        private void StatusBox_TextChanged(object sender, EventArgs e)
        {
            var status = statusBox.Text.Trim();
            if (status.Length == 0)
                return;

            var items = statusBox.Items;
            if (items.Count == 0)
                return;

            var found = items.Cast<string>().Any(s => s.Equals(status, StringComparison.OrdinalIgnoreCase));

            statusBox.ForeColor = found 
                                ? SystemColors.WindowText 
                                : Color.FromKnownColor(KnownColor.Red);
        }
    }    
}
