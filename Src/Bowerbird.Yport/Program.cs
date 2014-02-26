using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using NLog;

namespace Bowerbird.Yport
{
    public class Program
    {
        static void Main(string[] args)
        {
            Logger _logger = LogManager.GetLogger("Bowerbird.Import");

            try
            {
                var fileFetcher = new FileFetcher();
                fileFetcher.FetchTheData();
            }
            catch (Exception exception)
            {
                _logger.ErrorException("Error importing data", exception);
            }
        }
    }

    /// <summary>
    /// Copy file from a web resource locally
    /// </summary>
    internal class FileFetcher
    {
        private string _pathToWebResource;
        private string _pathToLocalResource;
        private WebClient _wc;
        private ConfigSettings _configSettings;

        public void FetchTheData()
        {
            Logger _logger = LogManager.GetLogger("Bowerbird.Import");

            _configSettings = ConfigSettings.Singleton();

            _pathToWebResource = string.Format(
                "{0}/{1}",
                _configSettings.GetUriToSite(),
                _configSettings.GetFileName()
                );

            _pathToLocalResource = string.Format(
                "{0}\\{1}",
                _configSettings.GetEnvironmentRootPath(),
                _configSettings.GetFileName()
                );

            if (File.Exists(_pathToLocalResource))
            {
                File.Delete(_pathToLocalResource);
            }

            using (_wc = new WebClient())
            {
                _wc.DownloadFile(_pathToWebResource, _pathToLocalResource);
            }
            
        }
    }

    /// <summary>
    /// App.Config property reader
    /// </summary>
    internal class ConfigSettings
    {
        private string _dumpFolder;
        private string _url;
        private string _fileName;
        private static ConfigSettings _singleton;

        public static ConfigSettings Singleton()
        {
            return _singleton ?? (_singleton = new ConfigSettings());
        }

        public string GetEnvironmentRootPath()
        {
            if (string.IsNullOrEmpty(_dumpFolder)) _dumpFolder = ConfigurationManager.AppSettings["dumpFolder"];

            return _dumpFolder;
        }

        public string GetUriToSite()
        {
            if (string.IsNullOrEmpty(_url)) _url = ConfigurationManager.AppSettings["url"];

            return _url;
        }

        public string GetFileName()
        {
            if (string.IsNullOrEmpty(_fileName)) _fileName = ConfigurationManager.AppSettings["fileName"];

            return _fileName;
        }
    }
}