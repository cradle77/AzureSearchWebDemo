using Common;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AdventureWorksWeb.Models.ViewModels
{
    public class ProductSuggestionViewModel
    {
        public string SearchTerm { get; set; }

        public List<ProductInfoViewModel> Products { get; set; }

        public ProductSuggestionViewModel()
        {
            this.Products = new List<ProductInfoViewModel>();
        }

        public static async Task<ProductSuggestionViewModel> LoadAsync(IKernel kernel, string term)
        {
            var settings = kernel.Get<ISearchSettings>();

            using (var client = new SearchServiceClient(settings.SearchServiceName, new SearchCredentials(settings.ApiKey)))
            using (var index = client.Indexes.GetClient("products"))
            {
                SuggestParameters parameters = new SuggestParameters()
                {
                    UseFuzzyMatching = true
                };

                var suggestions = await index.Documents
                    .SuggestAsync<ProductInfoViewModel>(term, "name_number", parameters);

                return new ProductSuggestionViewModel()
                {
                    SearchTerm = term,
                    Products = suggestions.Results.Select(x => x.Document).ToList()
                };
            }
        }
    }
}