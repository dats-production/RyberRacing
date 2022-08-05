using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    public abstract class ConfigEntryBase : ScriptableObject
    {
        public string id;

        public abstract IList<object> ToSpreadsheetRow();
    }
}
