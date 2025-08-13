using System.Diagnostics;
using System.IO;

namespace FileAnalyser
{
    internal class FileManager
    {
        /// <summary>
        /// Value for current Analyse State
        /// </summary>
        public Action<double> NotifyPercentValue;

        /// <summary>
        /// String to be show
        /// </summary>
        public Action<string> NotifyProgress;

        private long usedSpace = 0;
        private long checkedSpace = 0;


        public async Task<List<FileInfo>> GetAllFiles()
        {

            DriveInfo[] allDrives = DriveInfo.GetDrives();
            Stopwatch stopwatch = Stopwatch.StartNew();
            List<FileInfo> allFiles = new();

            List<Task> tasks = new();
            this.usedSpace = 0;
            this.checkedSpace = 0;

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        this.usedSpace += drive.TotalSize - drive.TotalFreeSpace;
                        allFiles.AddRange(await this.SearchDriveAsync(drive.RootDirectory.FullName));
                    }));
                }
            }

            await Task.WhenAll(tasks);

            stopwatch.Stop();

            this.NotifyProgress($"Done - [{allFiles.Count}] ([{this.checkedSpace}] / [{this.usedSpace}]) - [{stopwatch.Elapsed.TotalSeconds}] seconds");

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
                    var fileInfo = new FileInfo(file);
                    this.checkedSpace += fileInfo.Length;
                    double percentage = this.GetPercentage();

                    this.NotifyProgress($"Checked {this.ConvertByteIntoMb(this.checkedSpace)} MB / {this.ConvertByteIntoMb(this.usedSpace)} MB - {percentage:F2}%");
                    this.NotifyPercentValue(percentage);

                    files.Add(fileInfo);
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
            await Task.Yield(); //[TS] Start asynchron

            foreach (var file in Directory.EnumerateFiles(path, "*.*", options))
            {
                yield return file;
            }
        }

        #region Helper auslagern

        private double ConvertByteIntoMb(long byteNumber)
        {
            return byteNumber / (1024 * 1024); //[TS] Convert bytes to MB
        }

        private double GetPercentage()
        {
            if (this.usedSpace == 0) 
                return 0;
            return (double)this.checkedSpace / this.usedSpace * 100;
        }

        #endregion
    }
}
