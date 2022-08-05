using UnityEngine;

namespace Configs.Tabs
{
    public abstract class ConfigTabBase : ScriptableObject
    {
        public string tabName;
        
        public ConfigEntryBase[] data;
        
        public abstract void AddEntry<K>(K entry, string id);
        public abstract bool TryGetEntry<K>(string id, out K value);
    }
}
