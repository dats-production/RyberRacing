using UnityEngine;

namespace Configs.Provider
{
    [CreateAssetMenu(menuName = "Game/Config/Config settings")]
    public class ConfigSettings : ScriptableObject
    {
        public string ApplicationName;
        public string ApiKey;
        public string SpreadsheetId;
        public TextAsset credentialsJson;
    }
}
