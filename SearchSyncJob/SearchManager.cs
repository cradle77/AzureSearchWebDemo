using Common;
using Microsoft.Azure.Search;
using System;
using System.Threading.Tasks;

namespace SearchSyncJob
{
    public class SearchManager
    {
        private ISearchSettings _settings;

        public SearchManager(ISearchSettings settings)
        {
            _settings = settings;
        }

        public async Task InitializeAsync()
        {
            using (var client = new SearchServiceClient(_settings.SearchServiceName, new SearchCredentials(_settings.ApiKey)))
            {
                var definition = new ProductsIndexDefinition();

                Console.WriteLine("Startup: Ensuring indexes exist");

                Console.WriteLine($"Checking {definition.Name}");

                var index = await client.Indexes.GetOrDefaultAsync(definition.Name);

                if (index == null)
                {
                    Console.WriteLine($"Index {definition.Name} doesn't exist. Creating.");

                    await client.Indexes.CreateAsync(definition.GetIndex());

                    Console.WriteLine($"Index {definition.Name} successfully created.");
                }
            }
        }

        public async Task RebuildIndexAsync()
        {
            var definition = new ProductsIndexDefinition();

            using (var searchClient = new SearchServiceClient(_settings.SearchServiceName, new SearchCredentials(_settings.ApiKey)))
            {
                var indexer = await searchClient.Indexers.GetOrDefaultAsync(definition.IndexerName);

                if (indexer == null)
                {
                    // double check the index exists
                    var index = await searchClient.Indexes.GetOrDefaultAsync(definition.Name);
                    if (index == null)
                    {
                        throw new InvalidOperationException($"Index {definition.Name} has not been created yet, so we can't rebuild it");
                    }

                    await searchClient.DataSources.CreateOrUpdateAsync(definition.GetDataSource());

                    Console.WriteLine($"RebuildDataSource for {definition.Name} successfully created");

                    await searchClient.Indexers.CreateOrUpdateAsync(definition.GetIndexer());

                    Console.WriteLine($"RebuildIndexer for {definition.Name} successfully created.");
                }
                else
                {
                    await searchClient.Indexers.RunAsync(indexer.Name);

                    Console.WriteLine($"RebuildIndexer for {definition.Name} successfully started.");
                }

                await searchClient.Indexers.WaitForIndexingToComplete(definition.IndexerName);
            }
        }
    }
}
