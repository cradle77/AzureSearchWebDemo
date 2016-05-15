using Common;
using System.Configuration;

namespace SearchSyncJob
{
    internal class JobSettings : ISearchSettings
    {
        public string ApiKey
        {
            get
            {
                return ConfigurationManager.AppSettings["Search.Key"];
            }
        }

        public string SearchServiceName
        {
            get
            {
                return ConfigurationManager.AppSettings["Search.Name"];
            }
        }

        public string SourceConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["AdventureWorks"]?.ConnectionString;
            }
        }

        public string BusConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["sb"]?.ConnectionString;
            }
        }
    }
}
