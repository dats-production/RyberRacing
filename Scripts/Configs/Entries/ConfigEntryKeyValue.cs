using System.Collections.Generic;

namespace Configs.Entries
{
    public class ConfigEntryKeyValue : ConfigEntryBase 
    {
        public string value;
        
        public ConfigEntryKeyValue(string value, string id)
        {
            this.value = value;
            this.id = id;
        }

        public override IList<object> ToSpreadsheetRow()
        {
            return new List<object>
            {
                id,
                value
            };
        }
    }
}
