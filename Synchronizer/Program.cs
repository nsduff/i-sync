using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using static InSync.Constants;
using static InSync.Validator;

namespace InSync;
class Synchronizer
{
    private static string SourcePath;
    private static string DestinationPath;
    private static string LogFilePath;
    private static int SynchInterval;
    private static SyncTimeUnit SelectedTimeUnit;
    private static DateTime SyncStart;

    static void Main(string[] args)
    {
        ShowWelcomeMessage();

        GetSourcePath();
    }

    static void ShowWelcomeMessage()
    {
        Console.WriteLine(Constants.WELCOME_MESSAGE_SPACER);
        Console.WriteLine();
        Console.WriteLine(Constants.WELCOME_MESSAGE);
        Console.WriteLine(Constants.WELCOME_MESSAGE_TITLE);
        Console.WriteLine(Constants.WELCOME_MESSAGE_SUBTITLE);
        Console.WriteLine();
        Console.WriteLine(Constants.WELCOME_MESSAGE_SPACER);
    }

    static void GetSourcePath()
    {
        Console.WriteLine();
        Console.WriteLine(Constants.CONSOLE_ENTER_SOURCE_PATH);

        try
        {
            string path = Console.ReadLine();
            if (!ValidateDirectoryPath(path))
            {
                GetSourcePath();
            } else
            {
                path = path.Trim('"');
                Synchronizer.SourcePath = path;
                GetDestinationPath();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            GetSourcePath();
        }
    }

    static void GetDestinationPath()
    {
        Console.WriteLine();
        Console.WriteLine(Constants.CONSOLE_ENTER_DESTINATION_PATH);

        try
        {
            string path = Console.ReadLine();
            if (!ValidateDirectoryPath(path))
            {
                GetDestinationPath();
                return;
            }

            path = path.Trim('"');
            if(path == Synchronizer.SourcePath || IsDirectoryChildOfDirectory(Synchronizer.SourcePath, path))
            {
                Console.WriteLine(Constants.ERROR_SOURCE_DESTINATION_PATH);
                Console.WriteLine();
                GetDestinationPath();
            } else {
                Synchronizer.DestinationPath = path;
                GetLogFilePath();
            }
        } catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            GetDestinationPath();
        }
    }

    static void GetLogFilePath()
    {
        Console.WriteLine();
        Console.WriteLine(Constants.CONSOLE_ENTER_LOG_FILE_PATH);

        try
        {
            string path = Console.ReadLine();
            if (!ValidateDirectoryPath(path))
            {
                GetLogFilePath();
                return;
            }

            path = path.Trim('"');
            if(path == Synchronizer.SourcePath || path == Synchronizer.DestinationPath 
                || IsDirectoryChildOfDirectory(Synchronizer.SourcePath, path) 
                || IsDirectoryChildOfDirectory(Synchronizer.DestinationPath, path))
            {
                Console.WriteLine(Constants.ERROR_LOG_FILE_PATH);
                GetLogFilePath();
            } else
            {
                Synchronizer.LogFilePath = path;
                GetSyncUnit();
            }
        } catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            GetLogFilePath();
        }
    }

    static void GetSyncUnit()
    {
        Console.WriteLine();
        Console.WriteLine(Constants.CONSOLE_ENTER_SYNCH_INTERVAL_UNIT);

        try
        {
            string line = Console.ReadLine();
            if(line == null|| line.Length == 0)
            {
                Console.WriteLine(Constants.ERROR_INVALID_SYNCH_UNIT);
                GetSyncUnit();
                return;
            }

            int unit = 0;
            Int32.TryParse(line, out unit);
            if(unit < 1 || unit > 3)
            {
                Console.WriteLine(Constants.ERROR_INVALID_SYNCH_UNIT);
                GetSyncUnit();
                return;
            } else
            {
                switch (unit)
                {
                    case 1:
                        Synchronizer.SelectedTimeUnit = SyncTimeUnit.Minutes;
                        break;
                    case 2:
                        Synchronizer.SelectedTimeUnit = SyncTimeUnit.Hours;
                        break;
                    case 3:
                        Synchronizer.SelectedTimeUnit = SyncTimeUnit.Days;
                        break;
                }
                GetSynchInterval();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    static void GetSynchInterval()
    {
        Console.WriteLine();
        Console.WriteLine(Constants.CONSOLE_ENTER_SYNCH_INTERVAL);

        try
        {
            string line = Console.ReadLine();
            if (line == null || line.Length == 0)
            {
                Console.WriteLine(Constants.ERROR_INVALID_SYNCH_INTERVAL);
                GetSynchInterval();
                return;
            }
            int interval = 0;
            Int32.TryParse(line, out interval);
            if(interval <= 0)
            {
                Console.WriteLine(Constants.ERROR_INVALID_SYNCH_INTERVAL);
                GetSynchInterval();
                return;
            } else if(!CheckSyncIntervalMaximum(Synchronizer.SelectedTimeUnit, interval))
            {
                Console.WriteLine(Constants.ERROR_EXCEEDS_MAXIMUM);
                GetSyncUnit();
                return;
            }

            Synchronizer.SynchInterval = interval;

            CreateLogFile();
            SaveLogLine(Constants.SUCCESS_SETUP_COMPLETE);

            RunSync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            GetSynchInterval();
        }
    }

    static void RunSync()
    {
        SaveLogLine(Constants.SYNCH_STARTED);
        SyncStart = DateTime.Now;

        CheckDeletedDirectories();
        CheckAddedDirectories();

        CheckDeletedFiles();
        CheckChangedFiles();

        SaveLogLine(Constants.SUCCESS_FOLDERS_SYNCHED);

        SetSynchTimer();
    }

    static void CheckDeletedDirectories()
    {
        string[] backupDirs = Directory.GetDirectories(Synchronizer.DestinationPath, "*", SearchOption.AllDirectories);
        foreach (string backupDir in backupDirs)
        {
            try
            {
                string sourceDir = backupDir.Replace(Synchronizer.DestinationPath, Synchronizer.SourcePath);
                if (!Directory.Exists(sourceDir))
                {
                    Directory.Delete(backupDir, true);
                    SaveFileStatus(backupDir, Constants.LOG_DIRECTORY, Constants.LOG_DELETED);
                }
            } catch (Exception e)
            {
                SaveLogLine(e.ToString());
            }
        }
    }

    static void CheckAddedDirectories()
    {
        string[] sourceDirs = Directory.GetDirectories(Synchronizer.SourcePath, "*", SearchOption.AllDirectories);
        foreach(string sourceDir in sourceDirs)
        {
            try
            {
                string backupDir = sourceDir.Replace(Synchronizer.SourcePath, Synchronizer.DestinationPath);
                if (!Directory.Exists(backupDir))
                {
                    Directory.CreateDirectory(backupDir);
                    SaveFileStatus(backupDir, Constants.LOG_DIRECTORY, Constants.LOG_CREATED);
                }
            } catch (Exception e)
            {
                SaveLogLine(e.ToString());
            }
        }
    }
    
    static void CheckDeletedFiles()
    {
        string[] backupFiles = Directory.GetFiles(Synchronizer.DestinationPath, "*", SearchOption.AllDirectories);
        foreach (string backupFile in backupFiles)
        {
            try
            {
                string sourceFile = backupFile.Replace(Synchronizer.DestinationPath, Synchronizer.SourcePath);
                if (!File.Exists(sourceFile))
                {
                    File.Delete(backupFile);
                    SaveFileStatus(backupFile, Constants.LOG_FILE, Constants.LOG_DELETED);
                }
            } catch (Exception e)
            {
                SaveLogLine(e.ToString());
            }
        } 
    }

    static void CheckChangedFiles()
    {
        string[] sourceFiles = Directory.GetFiles(Synchronizer.SourcePath, "*", SearchOption.AllDirectories);
        foreach (string sourceFile in sourceFiles)
        {
            try
            {
                string rootFile = sourceFile.Replace(Synchronizer.SourcePath, "");
                string backupFile = Synchronizer.DestinationPath + rootFile;
                if (!File.Exists(backupFile))
                {
                    File.Copy(sourceFile, backupFile);
                    SaveFileStatus(backupFile, Constants.LOG_FILE, Constants.LOG_CREATED);
                }
                else if (!FileBytesEqual(sourceFile, backupFile))
                {
                    File.Replace(sourceFile, backupFile, null);
                    SaveFileStatus(backupFile, Constants.LOG_FILE, Constants.LOG_UPDATED);
                }
            } catch (Exception e)
            {
                SaveLogLine(e.ToString());
            }
        }
    }

    static void CreateLogFile()
    {
        string logFile = Path.Combine(LogFilePath, Constants.LOG_FILE_NAME);

        if (File.Exists(logFile))
        {
            SaveLogLine(Constants.LOG_FILE_EXISTS);
            return;
        }

        using (StreamWriter sw = File.CreateText(logFile))
        {
            string createLine = GetCurrentTimestamp() + " " + Constants.LOG_FILE_CREATED;
            sw.WriteLine(createLine);
            Console.WriteLine(createLine);
        }
    }
        

    static void SaveLogLine(string message)
    {
        try
        {
            string logLine = GetCurrentTimestamp() + " " + message;
            Console.WriteLine(logLine);

            string logFile = Path.Combine(LogFilePath, Constants.LOG_FILE_NAME);
            using (StreamWriter sw = new StreamWriter(logFile, true))
            {
                sw.WriteLine(logLine);
            }
        } catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    static void SaveFileStatus(string filename, string fileType, string status)
    {
        SaveLogLine(status + " " + fileType + " " + filename);
    }

    static string GetCurrentTimestamp()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    static void SetSynchTimer()
    {
        try
        {
            TimeSpan nextSync = TimeSpan.Zero;
            switch (SelectedTimeUnit)
            {
                case SyncTimeUnit.Minutes:
                    nextSync = TimeSpan.FromMinutes(SynchInterval);
                    break;
                case SyncTimeUnit.Hours:
                    nextSync = TimeSpan.FromHours(SynchInterval);

                    break;
                case SyncTimeUnit.Days:
                    nextSync = TimeSpan.FromDays(SynchInterval);
                    break;
            }

            // subtract elapsed time during synchronize from time until next sync;
            TimeSpan duration = DateTime.Now - SyncStart;
            if(duration < nextSync)
            {
                nextSync = nextSync - duration;
            } else
            {
                // if <= 0, start immediately
                nextSync = TimeSpan.Zero;
            }

            String message = Constants.SUCCESS_NEXT_SYNCH + " " + DateTime.Now.Add(nextSync).ToString("yyyy-MM-dd HH:mm:ss");
            SaveLogLine(message);

            RunSyncTimer(nextSync);

        } catch (Exception e)
        {
            SaveLogLine(e.ToString());
        }
    }

    private static void RunSyncTimer(TimeSpan nextSync)
    {
        try {
            Thread.Sleep(nextSync);

            RunSync();
        } catch (Exception e)
        {
            SaveLogLine(e.ToString());
            SaveLogLine(Constants.ERROR_FAILED_TO_SET_TIMER);
        }
    }
}


