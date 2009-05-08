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
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
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
        private GoogleCodeProject _project;

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
                var project = parameters.Project;
                if (project.Length == 0)
                    throw new ApplicationException("Missing Google Code project specification.");

                IList<Issue> issues;

                using (var dialog = new IssueBrowserDialog
                {
                    ProjectName = project,
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
                    _project = dialog.Project;
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

        [ ComVisible(false) ]
        public string OnCommitFinished(IWin32Window parentWindow,
            string commonRoot, string[] pathList,
            string logMessage, int revision)
        {
            if (_project == null)
                throw new InvalidOperationException();

            if (_issues == null || _issues.Count == 0)
                return null;

            var settings = Properties.Settings.Default;

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GURTLE_ISSUE_UPDATE_CMD")))
            {
                if (!settings.HideIssueUpdateTip)
                {
                    using (var dialog = new IssueUpdateInfoDialog())
                    {
                        if (parentWindow == null)
                            dialog.StartPosition = FormStartPosition.CenterScreen;
                        dialog.ShowDialog(parentWindow);
                    }
                }
            }
            else
            {
                var issues = _issues.Select(e => new IssueUpdate(e) 
                {
                    Comment = string.Format("Fixed in r{0}.", revision)
                })
                .ToList();

                while (issues.Count > 0)
                {
                    using (var dialog = new IssueUpdateDialog
                    {
                        Project = _project,
                        Issues = issues,
                        Revision = revision
                    })
                    {
                        new WindowSettings(settings, dialog);
                        if (DialogResult.OK != dialog.ShowDialog(parentWindow))
                            return null;
                    }

                    var credential = CredentialPrompt.Prompt(parentWindow, "Google Code", "ggcred.txt");
                    if (credential == null)
                        continue;

                    credential = new NetworkCredential(credential.UserName,
                        Convert.ToBase64String(Encoding.UTF8.GetBytes(credential.Password)));

                    while (issues.Count > 0)
                    {
                        var issue = issues[0];

                        try
                        {
                            if (!UpdateIssue(_project.Name, issue, credential))
                                return null;

                            issues.RemoveAt(0);
                        }
                        catch (ApplicationException e)
                        {
                            ShowErrorBox(parentWindow, e);
                            break;
                        }
                    }
                }
            }

            settings.Save();
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

        private static bool UpdateIssue(string project, IssueUpdate issue, NetworkCredential credential)
        {
            string commentPath = null;

            var comment = issue.Comment;
            if (comment.Length > 0)
            {
                if (comment.IndexOfAny(new[] { '\r', '\n', '\f' }) >= 0)
                {
                    commentPath = Path.GetTempFileName();
                    File.WriteAllText(commentPath, comment, Encoding.UTF8);
                    comment = "@" + commentPath;
                }
                else if (comment[0] == '@')
                {
                    comment = "@" + comment;
                }
            }

            var commandLine = Environment.GetEnvironmentVariable("GURTLE_ISSUE_UPDATE_CMD")
                              ?? string.Empty;

            var args = CommandLineToArgs(commandLine);
            var command = args.First();

            try
            {
                args = args.Skip(1)
                           .Select(arg => arg.FormatWith(CultureInfo.InvariantCulture, new
                           {
                               credential.UserName,
                               credential.Password,
                               Project = project,
                               Issue = issue,//.Issue,
                               Status = issue.Status,
                               Comment = comment,
                           }))
                           .Select(arg => EncodeCommandLineArg(arg))
                           .ToArray();
            }
            catch (FormatException e)
            {
                throw new ApplicationException(e.Message, e);
            }

            Process process;
            try
            {
                process = Process.Start(new ProcessStartInfo
                {

                    UseShellExecute = false,
                    CreateNoWindow = true,
                    FileName = command,
                    Arguments = string.Join(" ", args),
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                });
            }
            catch (Win32Exception e)
            {
                throw new ApplicationException(e.Message, e);
            }

            var stdout = new StringWriter();
            var stderr = new StringWriter();
            // TODO script.SynchronizingObject = sync;
            process.OutputDataReceived += (sender, e) => stdout.WriteLine(e.Data);
            process.ErrorDataReceived += (sender, e) => stderr.WriteLine(e.Data);
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            Trace.TraceInformation(stdout.ToString());

            var success = 0 == process.ExitCode;

            if (success)
                Trace.TraceWarning(stderr.ToString());
            else
                Trace.TraceError(stderr.ToString());

            if (!string.IsNullOrEmpty(commentPath) && File.Exists(commentPath))
                File.Delete(commentPath);

            return success;
        }

        private static string EncodeCommandLineArg(string str)
        {
            return string.IsNullOrEmpty(str)
                 ? "\"\""
                 : str.IndexOfAny(new[] { ' ', '\"' }) >= 0
                 ? "\"" + (str).Replace("\"", "\\\"") + "\""
                 : str;
        }

        [DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr CommandLineToArgvW(
            [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        internal static string[] CommandLineToArgs(string commandLine)
        {
            int argc;
            var argv = CommandLineToArgvW(commandLine, out argc);

            if (argv == IntPtr.Zero)
                throw new Win32Exception();

            try
            {
                var args = new string[argc];
                for (var i = 0; i < args.Length; i++)
                {
                    var p = Marshal.ReadIntPtr(argv, i * IntPtr.Size);
                    args[i] = Marshal.PtrToStringUni(p);
                }

                return args;
            }
            finally
            {
                Marshal.FreeHGlobal(argv);
            }
        }
    }
}
