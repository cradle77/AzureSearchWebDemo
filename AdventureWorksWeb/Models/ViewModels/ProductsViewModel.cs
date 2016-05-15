using AdventureWorksWeb.Models.Entitites;
using Common;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Ninject;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventureWorksWeb.Models.ViewModels
{
    public class ProductsViewModel
    {
        public string SearchTerm { get; set; }
        public string Filter { get; set; }

        public List<Tuple<ProductInfoViewModel, HitHighlights>> Products { get; set; }

        public PagingInfo Paging { get; set; }
        public int TotalCount { get; private set; }

        public List<KeyValuePair<string, IList<FacetResult>>> Facets { get; private set; }

        public int TotalPages
        {
            get
            {
                if (this.Paging.PageSize == 0)
                    return 0;

                return (int)Math.Ceiling((double)this.TotalCount / this.Paging.PageSize);
            }
        }

        public ProductsViewModel()
        {
            this.Facets = new List<KeyValuePair<string, IList<FacetResult>>>();
            this.Products = new List<Tuple<ProductInfoViewModel, HitHighlights>>();
            this.Paging = new PagingInfo();
        }

        public static async Task<ProductsViewModel> LoadAsync(IKernel kernel, string term, string filter, PagingInfo paging = null)
        {
            var context = kernel.Get<AdventureWorks>();

            paging = paging ?? new PagingInfo();

            if (string.IsNullOrWhiteSpace(term))
            {
                return new ProductsViewModel();
            }

            var settings = kernel.Get<ISearchSettings>();

            using (var client = BuildSearchClient(settings))
            using (var index = client.Indexes.GetClient("products"))
            {
                var parameters = new SearchParameters()
                {
                    IncludeTotalResultCount = true,
                    SearchMode = SearchMode.Any,
                    Skip = paging.Skip,
                    Top = paging.Top,
                    ScoringProfile = "title",
                    QueryType = QueryType.Full,
                    Facets = new List<string>()
                    {
                        "CategoryName",
                        "ModelName"
                    },
                    HighlightFields = new List<string>()
                    {
                        "Name",
                        "CategoryName",
                        "ModelName"
                    },
                    HighlightPreTag = "<mark>",
                    HighlightPostTag = "</mark>",
                    Filter = filter
                };

                var response = await index.Documents
                    .SearchAsync<ProductInfoViewModel>(EnableFuzzy(term), parameters);

                var token = response.ContinuationToken;

                while (token != null)
                {
                    response = await index.Documents.ContinueSearchAsync<ProductInfoViewModel>(token);

                    token = response.ContinuationToken;
                }

                return new ProductsViewModel()
                {
                    Paging = paging,
                    Products = response.Results.Select(x => Tuple.Create(x.Document, x.Highlights)).ToList(),
                    TotalCount = Convert.ToInt32(response.Count.GetValueOrDefault()),
                    Facets = response.Facets.ToList(),
                    SearchTerm = term,
                    Filter = filter
                };
            }
        }

        private static SearchServiceClient BuildSearchClient(ISearchSettings settings)
        {
            var result = new SearchServiceClient(settings.SearchServiceName, new SearchCredentials(settings.ApiKey));

            var propertyInfo = typeof(SearchServiceClient).GetProperty(nameof(result.ApiVersion));
            propertyInfo.SetValue(result, "2015-02-28-Preview");

            return result;
        }

        private static string EnableFuzzy(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return searchText;

            // step 1: extract words of at least 2 characters
            // and add the ~ symbol to each word longer than 3 characters
            var words = Regex.Matches(searchText, "\\w{2,}")
                .OfType<Match>()
                .Select(x => x.Value.Length > 2 ? x.Value + "~" : x.Value);

            // step 2: join the words for the new search string
            string result = string.Join(" ", words);

            return result;
        }
    }
}