using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;
using SteamAuth;
using Xamarin.Essentials;
using Xamarin.Forms.Internals;

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

            try { File.Delete(fileName); UpdateFiles(); return true; } catch (Exception ex) { return false; }
            
        }

        public static void UpdateFiles() => _files = GetFiles();
    }

    public static class TestIO
    {
        /// <summary>
        /// Path to Folder save guard files
        /// </summary>
        public static string PathGuardFile { get; private set; } =
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/acc/";

        public static string PathAccountFile { get; private set; } =
            PathGuardFile + "accounts.json";

        public static string ExtensionGuardFile { get; private set; } =
            ".guard";

        private static List<Account> _accounts = null;

        public static List<Account> Accounts
        {
            get
            {
                if (_accounts == null)
                    _accounts = GetAccounts();
                return _accounts;
            }
            set
            {
                _accounts = value;
            }
        }

        private static List<Account> GetAccounts()
        {
            List<Account> accounts = new List<Account>();
            if (File.Exists(PathAccountFile))
                accounts = JsonConvert.DeserializeObject<List<Account>>(File.ReadAllText(PathAccountFile));

            if (!Directory.Exists(PathGuardFile))
                Directory.CreateDirectory(PathGuardFile);

            Directory.GetFiles(PathGuardFile, $"*{ExtensionGuardFile}").ForEach<string>((x) =>
            {
                if(!accounts.Exists( e => e.Path == x))
                    accounts.Add(AddToList(x));
            });

            return accounts;
        }

        private static Account AddToList(string file)
        {
            SteamGuardAccount guardAccount = JsonConvert.DeserializeObject<SteamGuardAccount>(File.ReadAllText(file));
            return new Account { Path = file, Name = guardAccount.AccountName };
        }
    }

    public class Account
    {
        public string Path { get; set; }
        public string Name { get; set; }
    }
}

