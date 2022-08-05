using System;
using System.Text;

namespace AddressableHandling
{
    public readonly struct DownloadOperationResult
    {
        public readonly object Label;
        public readonly bool IsComplete;
        public readonly long DownloadedBytes;
        public readonly long TotalBytes;
        public readonly float Time;
        public readonly Exception Error;

        public DownloadOperationResult(
            string label,
            bool isComplete,
            long bytes = 0,
            long total = 0,
            float time = 0,
            Exception exception = null)
        {
            Label = label;
            IsComplete = isComplete;
            DownloadedBytes = bytes;
            TotalBytes = total;
            Error = exception;
            Time = time;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder
                .AppendLine($"Is Complete : {IsComplete}")
                .AppendLine($"Downloaded Bytes : {DownloadedBytes}")
                .AppendLine($"Total Bytes : {TotalBytes}")
                .AppendLine($"Download Time : {Time}")
                .AppendLine($"Error : {Error?.Message}");

            return builder.ToString();
        }
    }
}
