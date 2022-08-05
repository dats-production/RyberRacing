using Configs.Tabs;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Configs.Provider
{
    public static class GoogleConfigsProvider
    {
        [MenuItem("Tools/Config/Open in browser")]
        public static void OpenSpreadsheetInBrowser()
        {
            var configSettings = Resources.Load<ConfigSettings>("ConfigSettings");
            UnityEditor.Localization.Plugins.Google.GoogleSheets.OpenSheetInBrowser(configSettings.SpreadsheetId);
        }

        [MenuItem("Tools/Config/Update remote spreadsheet")]
        public static void UpdateSpreadsheet()
        {
            var configSettings = Resources.Load<ConfigSettings>("ConfigSettings");
            var localConfig = Resources.Load<Config>("GameConfig");
            
            foreach (var tab in localConfig.tabs)
            {
                const int firstRowIndex = 2;
                
                var tabName = tab.tabName;
                var lastRowIndex = tab.data.Length + firstRowIndex - 1;
                var range = $"{tabName}!A{firstRowIndex}:C{lastRowIndex}";
                var rows = new ValueRange
                {
                    Values = new List<IList<object>>()
                };

                foreach (var entry in tab.data)
                {
                    var row = entry.ToSpreadsheetRow();
                    rows.Values.Add(row);
                }

                var credentials = GoogleCredential.FromJson(configSettings.credentialsJson.text);
                var sheetsService = new SheetsService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credentials,
                    ApplicationName = configSettings.ApplicationName
                });
                var request = sheetsService.Spreadsheets.Values
                    .Update(rows, configSettings.SpreadsheetId, range);
                request.ValueInputOption =
                    SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                request.Execute();
                Debug.Log("Remote spreadsheet config successfully updated!");
            }
        }

        [MenuItem("Tools/Config/Update local config")]
        public static void UpdateLocalConfig()
        {
            var configSettings = Resources.Load<ConfigSettings>("ConfigSettings");
            var localConfig = Resources.Load<Config>("GameConfig");
            localConfig.tabs = Array.Empty<ConfigTabBase>();

            var credentials = GoogleCredential.FromJson(configSettings.credentialsJson.text);
            var sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credentials,
                ApplicationName = configSettings.ApplicationName
            });

            var request = sheetsService.Spreadsheets.Get(configSettings.SpreadsheetId);
            var response = request.Execute();
            foreach (var sheet in response.Sheets)
            {
                localConfig.AddTab(ParseTab(sheet, sheetsService, configSettings.SpreadsheetId));
            }

            AssetDatabase.ForceReserializeAssets(new[] { "Assets/Resources/GameConfig.asset" });
            Debug.Log("Local config successfully updated!");
        }

        private static ConfigTabBase ParseTab(Sheet sheet, SheetsService sheetsService, string spreadsheetId)
        {
            const int firstLayerRowIndex = 1;

            var tabName = sheet.Properties.Title;
            var maxRowsCount = sheet.Properties.GridProperties.RowCount;
            var lastLayerRowIndex = maxRowsCount + 2;

            var request = sheetsService.Spreadsheets.Values.Get(spreadsheetId,
                $"{tabName}!A{firstLayerRowIndex}:C{lastLayerRowIndex}");
            var response = request.Execute();

            var tab = CreateTab(tabName);
            var rows = response.Values;
            for (var i = 1; i < rows.Count; i++)
            {
                var row = rows[i];
                var key = row[0].ToString();
                tab.AddEntry(row[1].ToString(), key);
                AssetDatabase.ForceReserializeAssets(new[] { $"Assets/Resources/ConfigData/table_{tabName}.asset" });
            }

            return tab;
        }

        private static ConfigTabBase CreateTab(string tabName)
        {
            switch (tabName)
            {
                case nameof(ConfigGeneralTab):
                    var generalTab = ScriptableObject.CreateInstance<ConfigGeneralTab>();
                    generalTab.tabName = tabName;
                    
                    var path = $"Assets/Resources/ConfigData/table_{tabName}.asset";
                    AssetDatabase.CreateAsset(generalTab, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    return generalTab;
                default:
                    throw new ArgumentException($"Wrong spreadsheet tab name: {tabName}");
            }
        }
    }
}