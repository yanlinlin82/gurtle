namespace Gurtle
{
    #region Imports

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;    

    #endregion

    [ Serializable ]
    public sealed class Parameters
    {
        private string _project;
        private string _status;
        private string _user;

        public static Parameters Parse(string str)
        {
            var dict = ParsePairs(str.Split(';')).ToDictionary(
                           p => p.Key, p => p.Value, StringComparer.OrdinalIgnoreCase);

            return new Parameters
            {
                Project = dict.TryPop("project"),
                User = dict.TryPop("user"),
                Status = dict.TryPop("status"),
            };
        }

        public string Project
        {
            get { return _project ?? string.Empty; }
            
            set
            {
                if (!string.IsNullOrEmpty(value) && !GoogleCodeProject.IsValidProjectName(value))
                    throw new ArgumentException("Invalid project name.", "value");

                _project = value;
            }
        }

        public string User
        {
            get { return _user ?? string.Empty; }
            set { _user = value; }
        }

        public string Status
        {
            get { return _status ?? string.Empty; }
            set { _status = value; }
        }

        public override string ToString()
        {
            var list = new List<KeyValuePair<string, string>>();

            if (Project.Length > 0)
                list.Add(Pair("project", Project));
            
            if (User.Length > 0)
                list.Add(Pair("user", User));
            
            if (Status.Length > 0)
                list.Add(Pair("status", Status));

            return string.Join(";", 
                Pairs(
                    Pair("project", Project), 
                    Pair("user", User), 
                    Pair("status", Status))
                .Where(e => e.Value.Length > 0)
                .Select(e => e.Key + "=" + e.Value)
                .ToArray());
        }

        private static IEnumerable<KeyValuePair<string, string>> Pairs(params KeyValuePair<string, string>[] pairs)
        {
            return pairs;
        }

        private static KeyValuePair<string, string> Pair(string key, string value)
        {
            return new KeyValuePair<string, string>(key, value);
        }

        private static IEnumerable<KeyValuePair<string, string>> ParsePairs(IEnumerable<string> parameters)
        {
            Debug.Assert(parameters != null);

            foreach (var parameter in parameters)
            {
                var pair = parameter.Split(new[] { '=' }, 2);
                var key = pair[0].Trim();
                var value = pair.Length > 1 ? pair[1].Trim() : string.Empty;
                yield return new KeyValuePair<string, string>(key, value);
            }
        }
    }
}
