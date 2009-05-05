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
            var path = Path.Combine(Application.LocalUserAppDataPath, fileName);            
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
