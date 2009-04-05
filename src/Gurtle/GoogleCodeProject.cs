namespace Gurtle
{
    #region Imports

    using System;
    using System.Text.RegularExpressions;

    #endregion

    internal sealed class GoogleCodeProject
    {
        public static bool IsValidProjectName(string value)
        {
            if (value == null) 
                throw new ArgumentNullException("value");
            
            //
            // From http://code.google.com/hosting/createProject:
            //
            //   "...project's name must consist of a lowercase letter, 
            //    followed by lowercase letters, digits, and dashes, 
            //    with no spaces."
            //

            return value.Length > 0 && Regex.IsMatch(value, @"^[a-z][a-z0-9-]*$");
        }
    }
}