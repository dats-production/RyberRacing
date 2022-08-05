using System.Linq;
using Configs.Tabs;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(menuName = "Game/Config/Config object")]
    public class Config : ScriptableObject
    {
       public ConfigTabBase[] tabs;

        public void AddTab(ConfigTabBase tab)
        {
            var tabList = tabs.ToList();
            tabList.Add(tab);
            tabs = tabList.ToArray();
        }
        
        public ConfigGeneralTab GeneralConfig => tabs
            .Where(t => t.GetType() == typeof(ConfigGeneralTab))
            .Cast<ConfigGeneralTab>()
            .First();
    }
}
