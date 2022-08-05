
namespace Analytics
{
    public class AnalyticsOptions
    {
        public string EnviromentName;
        public bool duplicateToLog;

        public AnalyticsOptions(string environmentName)
        {
            this.EnviromentName = environmentName;
        }
    }
}
