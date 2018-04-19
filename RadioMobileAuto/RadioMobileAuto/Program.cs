using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace RadioMobileAuto
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Radio Mobile Auto";
            ShowHeader();
            CheckExistence();
        }

        private static void CheckExistence()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                // Specify a name for your top-level folder.
                string folderName = drive.Name + @"\Radio_Mobile";
                string launcherPath = folderName + @"\rmweng.exe";
                string pathString = Path.Combine(folderName, "Geodata");

                // Check if launcher exists
                if (File.Exists(launcherPath))
                {
                    FileExists(drive);
                }
            }

            PromptInstallation(drives);
        }

        private static void PromptInstallation(DriveInfo[] drives)
        {
            ListDrives(drives);

            AskLocation(drives);
        }

        private static void AskLocation(DriveInfo[] drives)
        {
            ConsoleKey response;
            Console.Write("Choose install folder (1, 2, or 3): ");
            response = Console.ReadKey(false).Key;

            if (response == ConsoleKey.NumPad1 || response == ConsoleKey.D1)
            {
                BeginInstall(drives[0]);
            }
            else if (response == ConsoleKey.NumPad2 || response == ConsoleKey.D2)
            {
                BeginInstall(drives[1]);
            }
            else if (response == ConsoleKey.NumPad3 || response == ConsoleKey.D3)
            {
                BeginInstall(drives[2]);
            }
            else
            {
                Console.WriteLine("You chose something wrong. Try again...");
                AskLocation(drives);
            }
        }

        private static void BeginInstall(DriveInfo drive)
        {
            Console.WriteLine("");
            CreateFolders(drive);

            ExtractFiles(drive);
        }

        private static void ExtractFiles(DriveInfo drive)
        {
            Console.WriteLine("Extracting files to drive " + drive.Name + "...");
            string[] subFolders = new string[] { "srtm3", "srtm1", "srtmthird", "Landcover", "OpenStreetMap", "Terraserver", "Toporama" };
                      
            if (drive != null)
            {
                // Specify a name for your top-level folder.
                string folderName = drive.Name + @"\Radio_Mobile";
                string launcherPath = folderName + @"\rmweng.exe";
                string pathString = Path.Combine(folderName, "Geodata");

                // Check if launcher exists
                if (File.Exists(launcherPath))
                {
                    FileExists(drive);
                }
                else
                {
                    Directory.CreateDirectory(pathString);

                    // Now create subfolders for Geodata
                    foreach (var sf in subFolders)
                    {
                        pathString = System.IO.Path.Combine(pathString, sf);
                        Directory.CreateDirectory(pathString);
                        // Reset
                        pathString = System.IO.Path.Combine(folderName, "Geodata");
                    }

                    // Extract first zip - core
                    var zipFileName = Environment.CurrentDirectory + @"\zips\rmwcore.zip";
                    var targetDir = folderName;
                    FastZip fastZip = new FastZip();
                    string fileFilter = null;

                    // Will always overwrite if target filenames already exist
                    fastZip.ExtractZip(zipFileName, targetDir, fileFilter);

                    // Extract second zip
                    zipFileName = Environment.CurrentDirectory + @"\zips\rmw1166eng.zip";
                    targetDir = folderName;

                    // Will always overwrite if target filenames already exist
                    fastZip.ExtractZip(zipFileName, targetDir, fileFilter);

                    // Check if launcher exists
                    if (File.Exists(launcherPath))
                    {
                        FileExists(drive);
                    }
                }
            }
        }

        private static void CreateFolders(DriveInfo drive)
        {
            Console.WriteLine("Creating folders...");
            string[] subFolders = new string[] { "srtm3", "srtm1", "srtmthird", "Landcover", "OpenStreetMap", "Terraserver", "Toporama" };
                       
            if (drive != null)
            {
                // Specify a name for your top-level folder.
                string folderName = drive.Name + @"\Radio_Mobile";
                string launcherPath = folderName + @"\rmweng.exe";
                string pathString = Path.Combine(folderName, "Geodata");
                Directory.CreateDirectory(pathString);

                // Now create subfolders for Geodata
                foreach (var sf in subFolders)
                {
                    pathString = Path.Combine(pathString, sf);
                    Directory.CreateDirectory(pathString);
                    // Reset
                    pathString = Path.Combine(folderName, "Geodata");
                }
            }
        }

        private static void ListDrives(DriveInfo[] drives)
        {
            Console.WriteLine("Local Disk drives available:");
            for (int x = 0; x < drives.Count(); x++)
            {
                Console.WriteLine("[" + (x + 1) + "] {0} - {1}GB free of {2}GB ({3:0.##}%)",
                    drives[x].Name, ConvertToGigs(drives[x].TotalFreeSpace),
                    ConvertToGigs(drives[x].TotalSize),
                    GetPercentage(ConvertToGigs(drives[x].TotalFreeSpace),
                    ConvertToGigs(drives[x].TotalSize)));
            }
            Console.WriteLine("");
        }

        private static void FileExists(DriveInfo drive)
        {
            bool confirmed = false;
            string folderName = drive.Name + @"\Radio_Mobile";

            ConsoleKey response;
            do
            {
                Console.Write("Radio Mobile already exists in " + drive.Name + ". Run the program now? [y/n] ");
                response = Console.ReadKey(false).Key;   // true is intercept key (dont show), false is show
                if (response != ConsoleKey.Enter)
                    Console.WriteLine();

                confirmed = response == ConsoleKey.Y;
            } while (response != ConsoleKey.Y && response != ConsoleKey.N);

            if (confirmed)
            {
                Process.Start(folderName + @"\rmweng.exe");
                Environment.Exit(0);
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private static double GetPercentage(double totalFreeSpace, double totalSize)
        {
            return (totalFreeSpace / totalSize) * 100;
        }

        private static long ConvertToGigs(long size)
        {
            return (size / (1024 * 1024 * 1024));
        }

        private static void ShowHeader()
        {
            string header = @"
  _____           _ _         __  __       _     _ _                      _        
 |  __ \         | (_)       |  \/  |     | |   (_) |          /\        | |       
 | |__) |__ _  __| |_  ___   | \  / | ___ | |__  _| | ___     /  \  _   _| |_ ___  
 |  _  // _` |/ _` | |/ _ \  | |\/| |/ _ \| '_ \| | |/ _ \   / /\ \| | | | __/ _ \ 
 | | \ \ (_| | (_| | | (_) | | |  | | (_) | |_) | | |  __/  / ____ \ |_| | || (_) | by Red David
 |_|  \_\__,_|\__,_|_|\___/  |_|  |_|\___/|_.__/|_|_|\___| /_/    \_\__,_|\__\___/ 
   Easier way to install and create folders needed for Roger Coude's Radio Mobile

";
            Console.WriteLine(header);
            Console.WriteLine("");
        }
    }
}
