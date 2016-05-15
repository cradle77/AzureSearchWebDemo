using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SearchSyncJob
{
    public static class SearchExtensions
    {
        public static async Task<Index> GetOrDefaultAsync(this IIndexesOperations indexes, string indexName)
        {
            var indexesList = await indexes.ListAsync();

            return indexesList.Indexes.SingleOrDefault(x => x.Name == indexName);
        }

        public static async Task<Indexer> GetOrDefaultAsync(this IIndexersOperations indexers, string indexerName)
        {
            // first check if the index exists
            var indexersList = await indexers.ListAsync();

            return indexersList.Indexers.SingleOrDefault(x => x.Name == indexerName);
        }

        public static async Task WaitForIndexingToComplete(this IIndexersOperations indexers, string indexerName)
        {
            while (true)
            {
                IndexerExecutionInfo status = null;

                try
                {
                    status = await indexers.GetStatusAsync(indexerName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error polling for indexer status", ex);
                }

                IndexerExecutionResult lastResult = status.LastResult;

                if (lastResult == null)
                {
                    // indexer result not yet available, let's wait and retry
                    await Task.Delay(1000);
                }
                else
                {
                    switch (lastResult.Status)
                    {
                        case IndexerExecutionStatus.InProgress:
                            Console.WriteLine($"Synchronization running on indexer {indexerName}");

                            await Task.Delay(1000);
                            break;

                        case IndexerExecutionStatus.Success:
                            Console.WriteLine($"Synchronization completed on indexer {indexerName}: {lastResult.ItemCount} items sync'd");
                            return;                            
                        default:
                            Console.WriteLine($"Synchronization failed on indexer {indexerName}: {lastResult.ErrorMessage}");
                            return;
                    }
                }
            }
        }
    }
}
