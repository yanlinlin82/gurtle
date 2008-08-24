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
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.Runtime.InteropServices;
    using Interop.BugTraqProvider;

    #endregion

    [ ComVisible(true) ]
    [ Guid("91974081-2DC7-4FB1-B3BE-0DE1C8D6CE4E") ]
    [ ClassInterface(ClassInterfaceType.None) ]
    public sealed class Plugin : IBugTraqProvider
    {
        public string GetCommitMessage(
            IntPtr hParentWnd, 
            string parameters, string commonRoot, string[] pathList,
            string originalMessage)
        {
            return GetCommitMessage(WindowHandleWrapper.TryCreate(hParentWnd), parameters, originalMessage);
        }

        [ ComVisible(false) ]
        public static string GetCommitMessage(
            IWin32Window parentWindow, 
            string parameters, string originalMessage)
        {
            try
            {
                var settings = ParseParameters(parameters)
                    .ToDictionary(p => p.Key, p => p.Value, StringComparer.OrdinalIgnoreCase);

                string project;
                if (!settings.TryGetValue("project", out project) || project.Length == 0)
                    throw new ApplicationException("Missing Google Code project specification.");

                string userName, status;
                settings.TryGetValue("user", out userName);
                settings.TryGetValue("status", out status);

                IList<Issue> issues;

                using (var dialog = new IssueBrowserDialog
                {
                    Project = project,
                    UserNamePattern = userName,
                    StatusPattern = status,
                    UpdateCheckEnabled = true,
                })
                {
                    var reply = dialog.ShowDialog(parentWindow);
                    issues = dialog.SelectedIssueObjects;

                    if (reply != DialogResult.OK || issues.Count == 0)
                        return originalMessage;
                }

                var message = new StringBuilder(originalMessage);

                if (originalMessage.Length > 0 && !originalMessage.EndsWith("\n"))
                    message.AppendLine();

                foreach (var issue in issues)
                {
                    message.Append("Fixed issue #")
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

        private static IEnumerable<KeyValuePair<string, string>> ParseParameters(string parameters)
        {
            return ParseParameters(parameters.Split(';'));
        }
        
        private static IEnumerable<KeyValuePair<string, string>> ParseParameters(IEnumerable<string> parameters)
        {
            foreach (var parameter in parameters)
            {
                var pair = parameter.Split(new[] {'='}, 2);
                var key = pair[0].Trim();
                var value = pair.Length > 1 ? pair[1].Trim() : string.Empty;
                yield return new KeyValuePair<string, string>(key, value);
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
