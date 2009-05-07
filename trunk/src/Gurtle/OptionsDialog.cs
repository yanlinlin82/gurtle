namespace Gurtle
{
    #region Imports

    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Windows.Forms;
    using Properties;

    #endregion

    public sealed partial class OptionsDialog : Form
    {
        private Parameters _parameters;

        public OptionsDialog()
        {
            InitializeComponent();
            _linkLabel.Text = GoogleCodeProject.HostingUrl.ToString();
        }

        public Parameters Parameters
        {
            get
            {
                if (_parameters == null)
                    _parameters = new Parameters();
                return _parameters;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                _parameters = value;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            _projectNameBox.Text = Parameters.Project;
            base.OnLoad(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (DialogResult != DialogResult.OK)
                return;

            Parameters.Project = _projectNameBox.Text;    
        }

        private void ProjectNameBox_TextChanged(object sender, EventArgs e)
        {
            var projectName = _projectNameBox.Text;
            var project = GoogleCodeProject.IsValidProjectName(projectName) 
                        ? new GoogleCodeProject(projectName) 
                        : null;
            
            _okButton.Enabled = _testButton.Enabled = project != null;            
            _linkLabel.Text = (project != null ? project.Url : GoogleCodeProject.HostingUrl).ToString();
        }

        private void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(_linkLabel.Text);
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            try
            {
                var projectName = _projectNameBox.Text;
                var url = new GoogleCodeProject(projectName).DnsUrl();
                new WebClient().DownloadData(url);
                var message = string.Format("The Google Code project '{0}' appears valid and reachable at {1}.", projectName, url);
                MessageBox.Show(message, "Test Passed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (WebException we)
            {
                MessageBox.Show(we.Message, "Test Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetSettings_Click(object sender, EventArgs e)
        {
            var reply = MessageBox.Show(this, 
                            "Reset all settings to their defaults?", "Reset Settings", 
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (DialogResult.Yes != reply)
                return;

            Settings.Default.Reset();
        }
    }
}
