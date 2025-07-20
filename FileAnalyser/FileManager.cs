using System.Diagnostics;
using System.IO;

namespace FileAnalyser
{
    internal class FileManager
    {
        #region Funktioniert

        //public async Task GetAllFiles(Action<string> notifyProgressReference)
        //{

        //    DriveInfo[] allDrives = DriveInfo.GetDrives();
        //    Stopwatch stopwatch = Stopwatch.StartNew();

        //    foreach (DriveInfo drive in allDrives)
        //    {
        //        notifyProgressReference($"Checking drive: [{drive.Name}]");

        //        if (!drive.IsReady) continue;

        //        var options = new EnumerationOptions
        //        {
        //            IgnoreInaccessible = true,
        //            RecurseSubdirectories = true
        //        };

        //        try
        //        {
        //            foreach (string file in Directory.EnumerateFiles(drive.RootDirectory.FullName, "*.*", options))
        //            {
        //                Debug.WriteLine(file);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"❌ Fehler bei {drive.Name}: {ex.Message}");
        //        }
        //    }

        //    stopwatch.Stop();

        //    notifyProgressReference($"Done - [{stopwatch.Elapsed.TotalMilliseconds}] MS");
        //}

        //public string GetMBSpace(long space)
        //{
        //    long freeMB = space / (1024 * 1024);
        //    return $"{freeMB} MB";
        //}

        #endregion

        public async Task<List<FileInfo>> GetAllFiles(Action<string> notifyProgressReference)
        {

            DriveInfo[] allDrives = DriveInfo.GetDrives();
            Stopwatch stopwatch = Stopwatch.StartNew();
            List<FileInfo> allFiles = new();

            List<Task> tasks = new();

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        allFiles.AddRange(await this.SearchDriveAsync(drive.RootDirectory.FullName));
                    }));
                }
            }

            await Task.WhenAll(tasks);

            stopwatch.Stop();

            notifyProgressReference($"Done - [{allFiles.Count}] - [{stopwatch.Elapsed.TotalSeconds}] seconds");

            return allFiles;
        }

        private async Task<List<FileInfo>> SearchDriveAsync(string rootPath)
        {
            List<FileInfo> files = new();
            var options = new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = true
            };

            try
            {
                await foreach (var file in GetFilesAsync(rootPath, options))
                {
                    files.Add(new FileInfo(file));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception on: [{rootPath}],[{ex.Message}]");
            }
            return files;
        }

        private async IAsyncEnumerable<string> GetFilesAsync(string path, EnumerationOptions options)
        {
            await Task.Yield(); // Start asynchron

            foreach (var file in Directory.EnumerateFiles(path, "*.*", options))
            {
                yield return file;
            }
        }
    }
}
