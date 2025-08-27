using System.Security.Cryptography;

namespace CloningApp
{
    internal class CloningApp
    {
        private static Logger logger;

        private static string sourcePath;
        private static string replicaPath;
        private static int interval;
        private static string logPath;

        static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Wrong number of arguments. This app should be executed with four arguments:\nCloningApp.exe [source_path] [replica_path] [interval_time] [log_path]");
                Environment.Exit(1);
            }

            sourcePath = args[0];
            replicaPath = args[1];
            interval = Convert.ToInt32(args[2]);
            logPath = args[3];

            logger = new(logPath);

            if (interval <= 0)
            {
                logger.Log("Interval must be a positive integer.");
                Environment.Exit(1);
            }

            logger.Log("Directory synchronization started.");

            while (true)
            {
                try
                {
                    Synchronize(sourcePath, replicaPath);
                }
                catch (Exception ex)
                {
                    string exceptionMessage = "Synchronization encountered an exception:\n" + ex.ToString();

                    logger.Log(exceptionMessage);
                }

                Thread.Sleep(interval * 1000);
            }
        }

        static void Synchronize(string sourcePath, string replicaPath)
        {
            CopyDirectory(sourcePath, replicaPath);
            DeleteFilesFromReplica(sourcePath, replicaPath);
            logger.Log($"Synchronized directory {replicaPath} with {sourcePath}");
        }

        static void CopyDirectory(string sourcePath, string replicaPath)
        {
            if (!Directory.Exists(sourcePath))
            {
                logger.Log("Source directory not found. Aborting synchronization.");
                Environment.Exit(1);
            }

            if (!Directory.Exists(replicaPath))
            {
                Directory.CreateDirectory(replicaPath);
                logger.Log("Created directory: " + replicaPath);
            }

            DirectoryInfo sourceDirectory = new(sourcePath);

            foreach (FileInfo file in sourceDirectory.GetFiles())
            {
                string tempPath = Path.Combine(replicaPath, file.Name);

                if (!File.Exists(tempPath))
                {
                    try
                    {
                        file.CopyTo(tempPath);
                        logger.Log($"[CopyDirectory] Copied file from {file.FullName} to {tempPath}");
                    }
                    catch (IOException ex)
                    {
                        string exceptionMessage = $"[CopyDirectory] Failed to copy {file.FullName}:\n{ex.Message}";

                        logger.Log(exceptionMessage);
                    }
                }
                else if (!ComputeMD5Hash(tempPath).SequenceEqual(ComputeMD5Hash(file.FullName)))
                {
                    try
                    {
                        file.CopyTo(tempPath, true);
                        logger.Log($"[CopyDirectory] Replaced file {tempPath} with {file.FullName}");
                    }
                    catch (IOException ex)
                    {
                        string exceptionMessage = $"[CopyDirectory] Failed to copy {file.FullName}:\n{ex.Message}";

                        logger.Log(exceptionMessage);
                    }
                }
            }

            foreach (DirectoryInfo subdirectory in sourceDirectory.GetDirectories())
            {
                string tempPath = Path.Combine(replicaPath, subdirectory.Name);
                CopyDirectory(subdirectory.FullName, tempPath);
            }
        }

        static void DeleteFilesFromReplica(string sourcePath, string replicaPath)
        {
            DirectoryInfo replicaDirectory = new(replicaPath);

            foreach (FileInfo file in replicaDirectory.GetFiles())
            {
                string tempPath = Path.Combine(sourcePath, file.Name);

                if (!File.Exists(tempPath))
                {
                    try
                    {
                        file.Delete();
                        logger.Log($"[DeleteFilesFromReplica] Deleted file {file.FullName}");
                    }
                    catch (IOException ex)
                    {
                        string exceptionMessage = $"[DeleteFilesFromReplica] Failed to delete {file.FullName}:\n{ex.Message}";

                        logger.Log(exceptionMessage);
                    }
                }
            }

            foreach (DirectoryInfo subdirectory in replicaDirectory.GetDirectories())
            {
                string tempPath = Path.Combine(sourcePath, subdirectory.Name);

                if (!Directory.Exists(tempPath))
                {
                    try
                    {
                        subdirectory.Delete(true);
                        logger.Log($"[DeleteFilesFromReplica] Deleted directory {subdirectory.FullName}");
                    }
                    catch (IOException ex)
                    {
                        string exceptionMessage = $"[DeleteFilesFromReplica] Failed to delete {subdirectory.FullName}: {ex.Message}";

                        logger.Log(exceptionMessage);
                    }
                }
                else
                {
                    DeleteFilesFromReplica(tempPath, subdirectory.FullName);
                }
            }
        }

        static byte[] ComputeMD5Hash(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            using var md5 = MD5.Create();
            return md5.ComputeHash(stream);
        }
    }
}
