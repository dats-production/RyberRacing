using Configs;
using System;
using Configs.Entries;
using Configs.Tabs;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[Serializable]
public class ConfigKeyValueTab : ConfigTabBase
{
    public override void AddEntry<T>(T entry, string id)
    {
        var entryType = typeof(T);
        if (entryType != typeof(string))
            throw new ArgumentException($"{entryType} is not supported by Key-Value table! Only strings to parse!");
        
        var stringEntry = (string)Convert.ChangeType(entry, typeof(string));
        var entryScriptable = CreateInstance<ConfigEntryKeyValue>();
        entryScriptable.id = id;
        entryScriptable.value = stringEntry;
        
        var path = $"Assets/Resources/ConfigData/{tabName}_{id}.asset";
        AssetDatabase.CreateAsset(entryScriptable, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        var list = data?.ToList() ?? new List<ConfigEntryBase>();
        list.Add(entryScriptable);
        data = list.ToArray();
    }

    public override bool TryGetEntry<T>(string id, out T value)
    {
        var entry = (ConfigEntryKeyValue)data.FirstOrDefault(entry => entry.id == id);
        if (entry == default)
        {
            value = default;
            return false;
        }
        var entryType = typeof(T);
        
        if (entryType == typeof(bool))
        {
            value = (T)(object)Boolean.Parse(entry.value);
            return true;
        }
        
        if (entryType == typeof(float))
        {
            value = (T)(object)float.Parse(entry.value);
            return true;
        }
        
        if (entryType == typeof(string))
        {
            value = (T)(object)entry.value;
            return true;
        }

        value = default;
        return false;
    }
}
