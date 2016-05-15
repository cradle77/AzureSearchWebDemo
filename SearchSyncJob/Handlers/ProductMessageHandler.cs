using Common;
using Common.Events;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Rebus.Handlers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SearchSyncJob.Handlers
{
    internal class ProductMessageHandler :
        IHandleMessages<ProductUpdated>
    {
        private ISearchSettings _settings;

        public ProductMessageHandler(ISearchSettings settings)
        {
            _settings = settings;
        }

        public async Task Handle(ProductUpdated message)
        {
            Console.WriteLine($"Message ProductUpdated received for product {message.ProductID}");

            using (var searchClient = new SearchServiceClient(_settings.SearchServiceName, new SearchCredentials(_settings.ApiKey)))
            using (var index = searchClient.Indexes.GetClient("products"))
            {
                var actions = new List<IndexAction<ProductUpdated>>()
                {
                    IndexAction.Merge(message)
                };

                var batch = new IndexBatch<ProductUpdated>(actions);

                var result = await index.Documents.IndexAsync(batch);

                Console.WriteLine("Reindexing completed");
            }
        }
    }
}
