using AddressableHandling.Interfaces;

namespace AddressableHandling
{
    public class DownloadProgressReporter : IDownloadProgressReporter
    {
        public long DownloadedBytes { get; private set; }
        public long TotalBytes { get; private set; }

        public void Report(long downloadedBytes, long totalBytes)
        {
            DownloadedBytes = downloadedBytes;
            TotalBytes = totalBytes;
        }
    }
}
