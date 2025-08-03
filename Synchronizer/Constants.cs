namespace InSync;

enum SyncTimeUnit
{
    NotSelected,
    Minutes,
    Hours,
    Days
}

static class Constants
{
    public const string WELCOME_MESSAGE_SPACER = "************";
    public const string WELCOME_MESSAGE = "Welcome to";
    public const string WELCOME_MESSAGE_TITLE = "IN-SYNC";
    public const string WELCOME_MESSAGE_SUBTITLE = "~ Your Super-Lightweight Folder Synchronizer ~";

    public const string CONSOLE_ENTER_SOURCE_PATH = "(Step 1/5) Enter the file path of the folder you would like to back up:";
    public const string CONSOLE_ENTER_DESTINATION_PATH = "(Step 2/5) Enter the file path of the location you would like to save your backup folder:";
    public const string CONSOLE_ENTER_LOG_FILE_PATH = "(Step 3/5) Enter the file path of the location you would like to save the log file:";
    public const string CONSOLE_ENTER_SYNCH_INTERVAL_UNIT = "(Step 4/5) How often would you like to back up your data? Enter 1, 2, or 3\n1. minutes\n2. hours\n3. days";
    public const string CONSOLE_ENTER_SYNCH_INTERVAL = "(Step 5/5) Enter the number of your chosen unit (minutes/hours/days):";

    public const string ERROR_PATH_EMPTY = "\nERROR: File path cannot be empty.";
    public const string ERROR_FOLDER_NOT_FOUND = "\nERROR: No folder found at this address.";
    public const string ERROR_PERMISSIONS = "\nERROR: You do not have the required permissions to use this folder.";
    public const string ERROR_SOURCE_DESTINATION_PATH = "\nERROR: Source folder and backup folder cannot be nested.";
    public const string ERROR_LOG_FILE_PATH = "\nERROR: Log folder must be outside the source and backup folders.";
    public const string ERROR_INVALID_SYNCH_UNIT = "\nERROR: Invalid number. Please enter a whole number between 1 and 3.";
    public const string ERROR_INVALID_SYNCH_INTERVAL = "\nERROR: Invalid number. Please enter a positive whole number.";
    public const string ERROR_EXCEEDS_MAXIMUM = "\nERROR: Exceeds maximum backup interval. Minimum is 1 minute, maximum is 24 days";
    public const string ERROR_FAILED_TO_SET_TIMER = "\nERROR: Failed to set synchronization timer.";

    public const string LOG_FILE_NAME = "synchronizer_logger.txt";
    public const string LOG_FILE_CREATED = "log file created";
    public const string LOG_FILE_EXISTS = "log file already exists; aborting log file creation";
    public const string LOG_CREATED = "CREATED";
    public const string LOG_DELETED = "DELETED";
    public const string LOG_UPDATED = "UPDATED";
    public const string LOG_FILE = "FILE";
    public const string LOG_DIRECTORY = "DIRECTORY";

    public const string SUCCESS_SETUP_COMPLETE = "SETUP COMPLETE - synchronizing folders...";
    public const string SUCCESS_FOLDERS_SYNCHED = "SUCCESS - folders synchronized";
    public const string SUCCESS_NEXT_SYNCH = "Next sync: ";

    public const string SYNCH_STARTED = "Synchronization started. Please wait...";
}