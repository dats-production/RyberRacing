
using UnityEngine.Localization.Settings;

namespace Localization
{
    public class Localization : ILocalization
    {
        public string Get(string key)
        {
            return LocalizationSettings.StringDatabase.GetLocalizedStringAsync(key).WaitForCompletion(); 
        }
    }
}
