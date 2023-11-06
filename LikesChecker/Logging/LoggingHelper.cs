namespace LikesChecker.Logging;

public class LoggingHelper
{
    public static void LogException(string message, Exception exception, string logFilePath)
    {
        var logMessage = $"[{DateTime.Now}] {message}: {exception}";
        File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
    }
}