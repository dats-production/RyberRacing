

using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using Utils;

namespace AddressableHandling
{
    public class AddressablesDownloadingProvider
    {
        private const int ReconnectionDelayMs = 2000;

        public async UniTask DownloadAssetWithLabelIfRequiredAsync(AssetLabelReference label)
        {
            if (!await AddressableDownloader.IsRequiredDownloadAsync(label))
                return;

            //todo UI: show loading screen
            var downloadProgress = new DownloadProgressReporter();
            do
            {
                if (await WebServerConnectionTracker.IsAddressableServerReachable())
                {
                    var result = await AddressableDownloader.DownloadAsync(label, downloadProgress);

                    if (result.IsComplete)
                        break;
                }

                //todo UI: show no connection message
                await UniTask.Delay(ReconnectionDelayMs);
            } while (true);

            //todo UI: hide loading screen
        }
    }
}
