#region License, Terms and Author(s)
//
// Gurtle - IBugTraqProvider for Google Code
// Copyright (c) 2008 Atif Aziz. All rights reserved.
//
//  Author(s):
//
//      Atif Aziz, http://www.raboof.com
//      Don Kirkby, http://code.google.com/p/donkirkby/
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
// This class is a derivative work of WindowSettings from project donkirkby:
// http://code.google.com/p/donkirkby/source/browse/trunk/WindowSettings/WindowSettings.cs?spec=svn107&r=107
// It was originally licensed under the MIT License, the terms 
// and conditions of which can be found on-line at:
// http://www.opensource.org/licenses/mit-license.php.
//
#endregion

namespace Gurtle
{
    #region Imports

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    #endregion

    internal sealed partial class WorkProgressForm : Form
    {
        private string _successStatusText = "Finished";
        private string _errorStatusText = "Error: {Message}";
        private string _cancelledStatusText = "User-Cancelled";

        public event EventHandler WorkFailed;

        public bool StartWorkOnShow { get; set; }
        public bool CloseOnCompletion { get; set; }

        public string SuccessStatusText
        {
            get { return _successStatusText ?? string.Empty; }
            set { _successStatusText = value; }
        }

        public string ErrorStatusText
        {
            get { return _errorStatusText ?? string.Empty; }
            set { _errorStatusText = value; }
        }

        public string CancelledStatusText
        {
            get { return _cancelledStatusText ?? string.Empty; }
            set { _cancelledStatusText = value; }
        }

        public bool Cancelled { get; private set; }
        public Exception Error { get; private set; }
        public object Result { get; private set; }
        
        public BackgroundWorker Worker { get { return _worker; } }

        public WorkProgressForm()
        {
            InitializeComponent();
        }

        public void ReportProgress(int percentage)
        {
            ReportProgressImpl(percentage, null);
        }

        public void ReportProgress(string status)
        {
            ReportProgress(-1, status);
        }

        public void ReportProgress(int percentage, string status)
        {
            ReportProgressImpl(percentage, () => _status.Text = status);
        }

        public void ReportDetailLine(string line)
        {
            ReportProgressImpl(-1, () => _detailsBox.AppendText(line + Environment.NewLine));
        }

        private void ReportProgressImpl(int percentage, Action update)
        {
            if (InvokeRequired)
                Worker.ReportProgress(percentage, update);
            else
                OnProgressChanged(percentage, update);
        }

        protected override void OnShown(EventArgs e)
        {
            if (StartWorkOnShow)
                Worker.RunWorkerAsync();
            base.OnShown(e);
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            OnProgressChanged(e.ProgressPercentage, (Action) e.UserState);
        }

        private void OnProgressChanged(int percentage, Action update)
        {
            if (percentage >= 0)
                _bar.Value = percentage;
            if (update != null)
                update();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _bar.Value = _bar.Maximum;
            var cancelled = Cancelled = e.Cancelled;

            string status = null;

            if ((Error = e.Error) == null)
            {
                if (cancelled)
                {
                    if (CancelledStatusText.Length > 0)
                        status = CancelledStatusText;
                }
                else
                {
                    Result = e.Result;
                    if (SuccessStatusText.Length > 0)
                        status = SuccessStatusText;
                }
            }
            else
            {
                if (ErrorStatusText.Length > 0)
                    status = ErrorStatusText.FormatWith(e.Error);

                var handler = WorkFailed;
                if (handler != null)
                    handler(this, EventArgs.Empty);
            }

            if (status != null)
                _status.Text = status;

            if (CloseOnCompletion)
            {
                Close();
            }
            else
            {
                _cancelButton.Hide();

                var button = new Button
                {
                    Text = "&Close", 
                    Location = _cancelButton.Location,
                    Size = _cancelButton.Size,
                    TabIndex = _cancelButton.TabIndex,
                    Anchor = _cancelButton.Anchor,
                };
                button.Click += delegate { Close(); };
                CancelButton = button;
                Controls.Add(button);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            _cancelButton.Enabled = false;
            _worker.CancelAsync();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = _worker.IsBusy;
            base.OnClosing(e);
        }

        private void DetailsButton_Click(object sender, EventArgs e)
        {
            var swap = (string) _detailsButton.Tag;
            _detailsButton.Tag = _detailsButton.Text;
            _detailsButton.Text = swap;
            var delta = (_detailsBox.Visible ? -1 : 1) * _detailsBox.Size.Height;
            ClientSize = new Size(ClientSize.Width, ClientSize.Height + delta);
            _detailsBox.Visible = !_detailsBox.Visible;
        }
    }
}
