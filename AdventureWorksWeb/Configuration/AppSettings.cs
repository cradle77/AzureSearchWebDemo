using Common;
using System.Configuration;

namespace AdventureWorksWeb.Configuration
{
    public class AppSettings : ISearchSettings
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
        public string BusConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["sb"]?.ConnectionString;
            }
        }
    }
}