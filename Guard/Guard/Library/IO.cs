using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Guard.Library
{
    public static class IO
    {
        /// <summary>
        /// Path to Folder save guard files
        /// </summary>
        public static string PathGuardFile { get; private set; } =
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/acc/";

        /// <summary>
        /// Guard files Extension
        /// </summary>
        public static string ExtensionGuardFile { get; private set; } =
            ".guard";

        private static List<string> _files = null;

        /// <summary>
        /// Return Guard Files
        /// </summary>
        public static List<string> Files
        {
            get
            {
                if (_files == null)
                    _files = GetFiles();
                return _files;
            }
            set
            {
                _files = value;
            }
        }

        private static List<string> GetFiles()
        {
            List<string> files = new List<string>();

            if (!Directory.Exists(PathGuardFile))
                Directory.CreateDirectory(PathGuardFile);

            //Return files only specifd extension
            files = Directory.GetFiles(PathGuardFile, $"*{ExtensionGuardFile}").ToList();

            return files;
        }

        public static bool AddAndWrite(string name, string content)
        {
            if (!Directory.Exists(PathGuardFile))
                Directory.CreateDirectory(PathGuardFile);

            string file = Path.Combine(PathGuardFile, name + ExtensionGuardFile);
            try
            {
                File.WriteAllText(file, content);

                if (_files == null)
                    _files = GetFiles();

                _files.Add(file);
                return true;
            }
            catch (Exception ex) { return false; }
        }

        public static string GetFileByName(string name)
        {
            string fileName = Path.Combine(PathGuardFile, name + ExtensionGuardFile);

            return fileName;
        }

        public static bool RemoveFileByName(string name)
        {
            string fileName = Path.Combine(PathGuardFile, name + ExtensionGuardFile);
            if (!File.Exists(fileName))
                return false;

            try { File.Delete(fileName); return true; } catch (Exception ex) { return false; }
        }
    }
}

