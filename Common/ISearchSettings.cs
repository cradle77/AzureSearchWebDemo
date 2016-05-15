namespace Common
{
    public interface ISearchSettings
    {
        string SearchServiceName { get; }
        string ApiKey { get; }
        string BusConnectionString { get; }
    }
}
