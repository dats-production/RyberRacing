using System;
using System.Collections.Generic;
using AddressableHandling.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AddressableHandling
{
    public class AddressableDownloader : MonoBehaviour
    {
        public static async UniTask<bool> IsRequiredDownloadAsync(AssetLabelReference label)
        {
            var isValidKey = label.RuntimeKeyIsValid();
            var sizeRequest = Addressables.GetDownloadSizeAsync(label);
            await sizeRequest.Task;
            return isValidKey && sizeRequest.Status == AsyncOperationStatus.Succeeded && sizeRequest.Result > 0;
        }

        public static async UniTask<DownloadOperationResult> DownloadAsync(
            IReadOnlyList<AssetLabelReference> assetLabel,
            DownloadType downloadType,
            IDownloadProgressReporter progressReporter = null)
        {
            if (downloadType == DownloadType.Sequential)
                return await InternalDownloadSequentialAsync(assetLabel, progressReporter);

            if (downloadType == DownloadType.Parallel)
                return await InternalDownloadParallelAsync(assetLabel, progressReporter);

            throw new Exception($"Not supported download type : {downloadType.ToString()}");
        }

        public static async UniTask<DownloadOperationResult> DownloadAsync(
            AssetLabelReference assetLabel,
            IDownloadProgressReporter progressReporter = null)
        {
            var stringLabel = assetLabel.labelString;

            if (!await IsRequiredDownloadAsync(assetLabel))
            {
                Debug.Log($"[Addressable Downloader]<color=green>Assets with label {stringLabel} already downloaded!</color>");
                return new DownloadOperationResult(stringLabel, true);
            }

            var downloadingOperation = Addressables.DownloadDependenciesAsync(assetLabel);
            var downloadTime = 0f;

            while (!downloadingOperation.IsDone)
            {
                var operation = downloadingOperation.GetDownloadStatus();
                progressReporter?.Report(operation.DownloadedBytes, operation.TotalBytes);
                downloadTime += Time.deltaTime;
                await UniTask.Yield();
            }

            var isComplete = IsOperationValidAndComplete(downloadingOperation);
            var downloadStatus = downloadingOperation.GetDownloadStatus();
            var downloadedBytes = downloadStatus.DownloadedBytes;
            var totalBytes = downloadStatus.TotalBytes;
            var exception = downloadingOperation.OperationException;

       
            Debug.Log(isComplete
                ? $"[Addressable Downloader] <color=green>{stringLabel} is downloaded!</color>"
                : $"[Addressable Downloader] <color=red>{stringLabel} downloading failed!</color>");

            return new DownloadOperationResult(stringLabel, isComplete, downloadedBytes, totalBytes, downloadTime,
                exception);
        }

        private static async UniTask<DownloadOperationResult> InternalDownloadParallelAsync(
            IReadOnlyList<AssetLabelReference> assetLabel,
            IDownloadProgressReporter progressReporter)
        {
            var isComplete = true;
            var downloadTime = 0f;
            var downloadedBytes = 0L;
            var totalBytes = 0L;
            Exception exception = null;

            var taskList = new UniTask<DownloadOperationResult>[assetLabel.Count];
            var parallelProgress = new DownloadProgressReporter[assetLabel.Count];

            for (var i = 0; i < assetLabel.Count; i++)
            {
                var labelReference = assetLabel[i];
                parallelProgress[i] = new DownloadProgressReporter();
                taskList[i] = DownloadAsync(labelReference, parallelProgress[i]);
            }

            var whenAll = UniTask.WhenAll(taskList);

            if (!ReferenceEquals(progressReporter, null))
            {
                do
                {
                    var downloaded = 0L;
                    var total = 0L;

                    foreach (var downloadProgress in parallelProgress)
                    {
                        downloaded += downloadProgress.DownloadedBytes;
                        total += downloadProgress.TotalBytes;
                    }

                    progressReporter.Report(downloaded, total);
                    await UniTask.Yield();
                } while (whenAll.Status == UniTaskStatus.Pending);
            }

            var results = await whenAll;

            foreach (var result in results)
            {
                if (!result.IsComplete)
                    exception = result.Error;

                isComplete &= result.IsComplete;
                downloadTime = Mathf.Max(downloadTime, result.Time);
                downloadedBytes += result.DownloadedBytes;
                totalBytes += result.TotalBytes;
            }

            return new DownloadOperationResult(assetLabel.ToString(), isComplete, downloadedBytes, totalBytes, downloadTime,
                exception);
        }

        private static async UniTask<DownloadOperationResult> InternalDownloadSequentialAsync(
            IReadOnlyList<AssetLabelReference> assetLabel,
            IDownloadProgressReporter progressReporter)
        {
            var isComplete = true;
            var downloadTime = 0f;
            var downloadedBytes = 0L;
            var totalBytes = 0L;
            Exception exception = null;

            foreach (var labelReference in assetLabel)
            {
                var result = await DownloadAsync(labelReference, progressReporter);

                if (!result.IsComplete)
                    exception = result.Error;

                isComplete &= result.IsComplete;
                downloadTime += result.Time;
                downloadedBytes += result.DownloadedBytes;
                totalBytes += result.TotalBytes;
            }

            return new DownloadOperationResult(assetLabel.ToString(), isComplete, downloadedBytes, totalBytes, downloadTime,
                exception);
        }

        private static bool IsOperationValidAndComplete(AsyncOperationHandle downloadingOperation) =>
            downloadingOperation.IsDone &&
            downloadingOperation.IsValid() &&
            downloadingOperation.Status == AsyncOperationStatus.Succeeded;
    }
}
