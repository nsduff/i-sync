namespace InSync;
using static InSync.Constants;
static class Validator
{
    public static bool ValidateDirectoryPath(string path)
    {
        try
        {
            if (path == null || path.Length == 0)
            {
                return false;
            }

            path = path.Trim('"');
            if (!IsPathValid(path))
            {
                return false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return false;
        }
        return true;
    }

    static bool IsPathValid(string path)
    {
        string error = "";
        if (path == null || path.Length == 0)
        {
            error = Constants.ERROR_PATH_EMPTY;
        }
        else if (!Directory.Exists(path))
        {
            error = Constants.ERROR_FOLDER_NOT_FOUND;
        }
        else if (!IsDirectoryWritable(path))
        {
            error = Constants.ERROR_PERMISSIONS;
        }

        if (error.Length != 0)
        {
            Console.WriteLine(error);
            return false;
        }

        return true;
    }

    static bool IsDirectoryWritable(string path)
    {
        try
        {
            using FileStream fs = File.Create(
                Path.Combine(
                    path,
                    Path.GetRandomFileName()
                ),
                1,
                FileOptions.DeleteOnClose);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsDirectoryChildOfDirectory(string path1, string path2)
    {
        try
        {
            if (path1.IndexOf(path2) != -1 || path2.IndexOf(path1) != -1)
            {
                return true;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return true;
        }
        return false;
    }

    public static bool CheckSyncIntervalMaximum(SyncTimeUnit selectedTimeUnit, int interval)
    {
        TimeSpan max = TimeSpan.FromMilliseconds(int.MaxValue);

        switch (selectedTimeUnit)
        {
            case (SyncTimeUnit.Minutes):
                int maxMinutes = (int)max.TotalMinutes;
                return interval <= maxMinutes;
            case (SyncTimeUnit.Hours):
                int maxHours = (int)max.TotalHours;
                return interval <= maxHours;
            case SyncTimeUnit.Days:
                int maxDays = (int)max.TotalDays;
                return interval <= maxDays;
        }
        return false;
    }

    public static bool FileBytesEqual(string path1, string path2)
    {
        int file1byte = 0;
        int file2byte = 0;
        FileStream fileStream1;
        FileStream fileStream2;

        fileStream1 = new FileStream(path1, FileMode.Open);
        fileStream2 = new FileStream(path2, FileMode.Open);

        if (fileStream1.Length != fileStream2.Length)
        {
            fileStream1.Close();
            fileStream2.Close();
            return false;
        }

        do
        {
            file1byte = fileStream1.ReadByte();
            file2byte = fileStream2.ReadByte();
        } while ((file1byte == file2byte) && (file1byte != -1));

        fileStream1.Close();
        fileStream2.Close();

        return ((file1byte - file2byte) == 0);
    }
}