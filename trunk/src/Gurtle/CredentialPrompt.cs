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
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows.Forms;

    #endregion

    internal static class CredentialPrompt
    {
        public static NetworkCredential Prompt(IWin32Window owner, string realm, string fileName)
        {
            var path = AppPaths.GetLocal(fileName);            
            var credential = TryLoadFromFile(path);
            
            using (var dialog = new CredentialsDialog { Realm = realm })
            {
                if (credential != null)
                {
                    dialog.UserName = credential.UserName;
                    dialog.Password = credential.Password;
                    dialog.SavePassword = credential.Password.Length > 0;
                }

                if (dialog.ShowDialog(owner) != DialogResult.OK)
                    return null;

                if (dialog.UserName.Length == 0 || dialog.Password.Length == 0)
                    return null;

                credential = new NetworkCredential(dialog.UserName, dialog.Password);
                    
                if (dialog.SavePassword)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    SaveToFile(path, credential);
                }
                else
                {
                    if (File.Exists(path))
                        File.Delete(path);
                }
            }

            return credential;
        }

        private static void SaveToFile(string path, NetworkCredential credential)
        {
            Debug.Assert(credential != null);
            SaveToFile(path, credential.UserName, credential.Password);
        }

        private static void SaveToFile(string path, string userName, string password)
        {
            Debug.Assert(userName != null);
            Debug.Assert(userName.Length > 0);
            Debug.Assert(password != null);
            Debug.Assert(password.Length > 0);

            var entropy = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(entropy);
            var secret = ProtectedData.Protect(Encoding.Unicode.GetBytes(password), entropy, DataProtectionScope.CurrentUser);
            File.WriteAllLines(path, new[]
            {
                userName, 
                Convert.ToBase64String(entropy), 
                Convert.ToBase64String(secret)
            });
        }

        private static NetworkCredential TryLoadFromFile(string path)
        {
            return File.Exists(path) ? TryLoad(File.ReadAllLines(path)) : null;
        }

        private static NetworkCredential TryLoad(IEnumerable<string> lines)
        {
            if (lines == null)
                return null;

            return TryLoadImpl(lines.Select(line => line.Trim())
                                    .Where(line => line.Length > 0 && line[0] != '#')
                                    .Take(3)
                                    .ToArray());

        }
        
        private static NetworkCredential TryLoadImpl(string[] lines)
        {
            if (lines.Length < 3)
                return null;

            var userName = lines[0];
            var entropy = Convert.FromBase64String(lines[1]);
            var secret = Convert.FromBase64String(lines[2]);

            byte[] data;
            try
            {
                data = ProtectedData.Unprotect(secret, entropy, DataProtectionScope.CurrentUser);
            }
            catch (CryptographicException e)
            {
                Trace.TraceWarning(e.Message);
                return null;
            }

            var password = Encoding.Unicode.GetString(data);
            return new NetworkCredential(userName, password);
        }
    }
}
