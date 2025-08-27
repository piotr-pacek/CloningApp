namespace CloningApp
{
    public class Logger(string logFilePath)
    {
        private readonly string logFilePath = logFilePath;

        public void Log(string message)
        {
            string timestampedMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";

            Console.WriteLine(timestampedMessage);

            File.AppendAllText(logFilePath, timestampedMessage + Environment.NewLine);
        }
    }
}
