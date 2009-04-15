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
    using System.Diagnostics;
    using System.Text;
    using System.Windows.Forms;
    using System.Runtime.InteropServices;
    using Interop.BugTraqProvider;

    #endregion

    [ ComVisible(true) ]
    [ Guid("91974081-2DC7-4FB1-B3BE-0DE1C8D6CE4E") ]
    [ ClassInterface(ClassInterfaceType.None) ]
    public sealed class Plugin : IBugTraqProvider2
    {
        private IList<Issue> _issues;

        public string GetCommitMessage(
            IntPtr hParentWnd, 
            string parameters, string commonRoot, string[] pathList,
            string originalMessage)
        {
            return GetCommitMessage(WindowHandleWrapper.TryCreate(hParentWnd), Parameters.Parse(parameters), originalMessage);
        }

        [ ComVisible(false) ]
        public string GetCommitMessage(
            IWin32Window parentWindow, 
            Parameters parameters, string originalMessage)
        {
            if (parameters == null) throw new ArgumentNullException("parameters");
            
            try
            {
                string project = parameters.Project;
                if (project.Length == 0)
                    throw new ApplicationException("Missing Google Code project specification.");

                IList<Issue> issues;

                using (var dialog = new IssueBrowserDialog
                {
                    Project = project,
                    UserNamePattern = parameters.User,
                    StatusPattern = parameters.Status,
                    UpdateCheckEnabled = true,
                })
                {
                    var settings = Properties.Settings.Default;
                    new WindowSettings(settings, dialog);

                    var reply = dialog.ShowDialog(parentWindow);
                    issues = dialog.SelectedIssueObjects;

                    settings.Save();

                    if (reply != DialogResult.OK || issues.Count == 0)
                        return originalMessage;

                    _issues = issues;
                }

                var message = new StringBuilder(originalMessage);

                if (originalMessage.Length > 0 && !originalMessage.EndsWith("\n"))
                    message.AppendLine();

                foreach (var issue in issues)
                {
                    message
                        .Append(GetIssueTypeAddress(issue.Type)).Append(" issue #")
                        .Append(issue.Id).Append(": ")
                        .AppendLine(issue.Summary);
                }

                return message.ToString();
            }
            catch (Exception e)
            {
                ShowErrorBox(parentWindow, e);
                throw;
            }
        }

        public bool ValidateParameters(IntPtr hParentWnd, string parameters)
        {
            return ValidateParameters(WindowHandleWrapper.TryCreate(hParentWnd), parameters);
        }

        [ ComVisible(false) ]
        public bool ValidateParameters(IWin32Window parentWindow, string parameters)
        {
            return true; // TODO validation
        }

        public string GetLinkText(IntPtr hParentWnd, string parameters)
        {
            return "Select Issue";
        }

        public string GetCommitMessage2(IntPtr hParentWnd, 
            string parameters, 
            string commonURL, string commonRoot, string[] pathList, 
            string originalMessage, string bugID, out string bugIDOut, 
            out string[] revPropNames, out string[] revPropValues)
        {
            bugIDOut = bugID;

            // If no revision properties are to be set, 
            // the plug-in MUST return empty arrays. 

            revPropNames = new string[0];
            revPropValues = new string[0];

            return GetCommitMessage(WindowHandleWrapper.TryCreate(hParentWnd), Parameters.Parse(parameters), originalMessage);
        }

        public string CheckCommit(IntPtr hParentWnd, 
            string parameters, 
            string commonURL, string commonRoot, string[] pathList, 
            string commitMessage)
        {
            return null;
        }

        public string OnCommitFinished(IntPtr hParentWnd, 
            string commonRoot, string[] pathList, 
            string logMessage, int revision)
        {
            return OnCommitFinished(WindowHandleWrapper.TryCreate(hParentWnd), commonRoot, pathList, logMessage, revision);
        }

        public string OnCommitFinished(IWin32Window parentWindow,
            string commonRoot, string[] pathList,
            string logMessage, int revision)
        {
            if (_issues == null || _issues.Count == 0)
                return null;

            using (var dialog = new IssueUpdateDialog())
            {
                dialog.Issues = _issues;
                dialog.ShowDialog(parentWindow);
            }
            
            return null;
        }

        public bool HasOptions()
        {
            return true;
        }

        public string ShowOptionsDialog(IntPtr hParentWnd, string parameters)
        {
            return ShowOptionsDialog(WindowHandleWrapper.TryCreate(hParentWnd), parameters);
        }

        [ ComVisible(false) ]
        public static string ShowOptionsDialog(IWin32Window parentWindow, string parameterString)
        {
            Parameters parameters;
            
            try
            {
                parameters = Parameters.Parse(parameterString);
            }
            catch (ParametersParseException e)
            {
                MessageBox.Show(e.Message, "Invalid Parameters", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return parameterString;
            }

            var dialog = new OptionsDialog { Parameters = parameters };           
            return dialog.ShowDialog(parentWindow) == DialogResult.OK 
                 ? dialog.Parameters.ToString() 
                 : parameterString;
        }

        private static string GetIssueTypeAddress(string issueType)
        {
            Debug.Assert(issueType != null);

            switch (issueType.ToLowerInvariant())
            {
                case "defect"     : return "Fixed";
                case "enhancement": return "Implemented";
                case "task"       : return "Finished";
                case "review"     : return "Reviewed";
                default           : return "Addressed";
            }
        }

        private static void ShowErrorBox(IWin32Window parent, Exception e)
        {
            MessageBox.Show(parent, e.Message, 
                e.Source + ": " + e.GetType().Name, 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
