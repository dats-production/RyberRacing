namespace AddressableHandling.Interfaces
{
    public interface IDownloadProgressReporter
    {
        void Report(long downloadedBytes, long totalBytes);
    }
}