using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Newtonsoft.Json;
using SteamAuth;
using Xamarin.Essentials;
using Xamarin.Forms.Internals;

namespace Guard.Library
{
    public static class IO
    {
        public static string PathGuardFile { get; private set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/acc/";
        public static string ExtensionGuardFile { get; private set; } = ".guard";

        private static void _files_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => Account.Save(_files);
        private static ObservableCollection<AFile> _files = null;
        public static ObservableCollection<AFile> Files
        {
            get
            {
                if (_files == null)
                {
                    _files = GetFiles();
                    _files.CollectionChanged += _files_CollectionChanged;
                }
                return _files;
            }
            set
            {
                _files = value;
            }
        }

        private static ObservableCollection<AFile> GetFiles()
        {
            var files = new ObservableCollection<AFile>();
            if (Account.Exists())
                return Account.Load();

            if (!Directory.Exists(PathGuardFile))
                Directory.CreateDirectory(PathGuardFile);

            Directory.GetFiles(PathGuardFile, $"*{ExtensionGuardFile}").ToList().ForEach(e =>
            {
                SteamGuardAccount sga = JsonConvert.DeserializeObject<SteamGuardAccount>(File.ReadAllText(e));
                files.Add(new AFile { Name = sga.AccountName, Path = sga.AccountName + ExtensionGuardFile });
            });

            Account.Save(files);

            return files;
        }

        public static string GetFileByName(string name) => Path.Combine(PathGuardFile, name + ExtensionGuardFile);

        private static bool Access()
        {
            var res = true;


            if (!Directory.Exists(PathGuardFile))
                Directory.CreateDirectory(PathGuardFile);

            if (Files == null)
                res = false;

            return res;
        }

        public static void Update()
        {
            var files = GetFiles();

            List<string> listFiles = Directory.GetFiles(PathGuardFile, $"*{ExtensionGuardFile}").ToList();

            listFiles.ForEach(e =>
            {
                AFile aFile = files.FirstOrDefault(x => GetFileByName(x.Name) == e);
                if (aFile == null)
                    Add.Files(e);
            });

            files.ForEach(e =>
            {
                if (!File.Exists(GetFileByName(e.Name)))
                    Remove.Files(e);
            });

            _files = files;
        }

        public static class Remove
        {
            public static void Files(string path)
            {
                if (!Access() || !File.Exists(path))
                    return;

                AFile aFile = _files.FirstOrDefault(x => x.Path == path);

                if (aFile == null)
                    return;

                File.Delete(Path.Combine(PathGuardFile, path));

                _files.Remove(aFile);
            }

            public static void Files(int index)
            {
                if (!Access() || _files.Count < 0 || index < 0 || index > (_files.Count - 1))
                    return;

                File.Delete(Path.Combine(PathGuardFile, _files[index].Path));
                _files.RemoveAt(index);
            }

            public static void Files(AFile aFile)
            {
                if (!Access())
                    return;

                File.Delete(Path.Combine(PathGuardFile, aFile.Path));
                _files.Remove(aFile);
            }

            public static bool ByName(string name)
            {
                if (!Access())
                    return false;

                AFile aFile = _files.FirstOrDefault(x => x.Name == name);

                if (aFile == null)
                    return false;

                File.Delete(Path.Combine(PathGuardFile, aFile.Path));
                _files.Remove(aFile);
                return true;
            }
        }

        public static class Add
        {
            public static void Files(string path)
            {
                if (!Access() || !File.Exists(path))
                    return;

                if (_files.FirstOrDefault(x => x.Path == path) != null)
                    return;

                SteamGuardAccount sga = JsonConvert.DeserializeObject<SteamGuardAccount>(File.ReadAllText(path));

                File.WriteAllText(Path.Combine(PathGuardFile, (path = sga.AccountName + ExtensionGuardFile)), JsonConvert.SerializeObject(sga));

                _files.Add(new AFile { Name = sga.AccountName, Path = path });
            }

            public static bool Files(string name, string content)
            {
                if (!Access())
                    return false;

                if (_files.FirstOrDefault(x => x.Name == name) != null)
                    return false;

                File.WriteAllText(Path.Combine(PathGuardFile, (name + ExtensionGuardFile)), content);

                _files.Add(new AFile { Name = name, Path = (name + ExtensionGuardFile) });
                return true;
            }
        }
    }

    public static class Account
    {
        public static string AccountFile { get; private set; } = IO.PathGuardFile + "accounts.json";
        public static string Content() => File.ReadAllText(AccountFile);
        public static void Remove() => File.Delete(AccountFile);
        public static void Save(ObservableCollection<AFile> files) => File.WriteAllText(AccountFile, JsonConvert.SerializeObject(files));
        public static ObservableCollection<AFile> Load() => JsonConvert.DeserializeObject<ObservableCollection<AFile>>(Content());
        public static bool Exists() => File.Exists(AccountFile);
    }

    public class AFile
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }
}

