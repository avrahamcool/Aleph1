using System;
using System.Configuration;
using System.Linq;

namespace $rootnamespace$.Classes
{
    /// <summary>Handle settings from config</summary>
    internal static class SettingsManager
    {
        private static string[] _modulesPath;

        /// <summary>Gets the modules to load as an array of paths.</summary>
        /// <value>array of module paths</value>
        public static string[] ModulesPath
        {
            get
            {
                if (_modulesPath == default(string[]))
                {
                    try
                    {
                        _modulesPath = ConfigurationManager.AppSettings["ModulesPath"].Split(';').Select(p => p.Trim()).Where(p => !String.IsNullOrWhiteSpace(p)).ToArray();
                    }
                    catch
                    {
                        _modulesPath = new string[0];
                    }
                }
                return _modulesPath;
            }
        }


		private static string _documentationDirPath;

        /// <summary>Gets the documentation dir path.</summary>
        /// <value>The documentation dir path.</value>
        public static string DocumentationDirPath
        {
            get
            {
                if (_documentationDirPath == default(string))
                {
                    _documentationDirPath = new Uri(new Uri(AppDomain.CurrentDomain.BaseDirectory), ConfigurationManager.AppSettings["DocumentationDirPath"]).LocalPath;
                }
                return _documentationDirPath;
            }
        }
    }
}