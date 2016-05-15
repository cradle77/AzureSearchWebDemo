using Microsoft.Azure.Search.Models;
using System.Collections.Generic;

namespace SearchSyncJob
{
    internal class ProductsIndexDefinition
    {
        public string Name
        {
            get { return "products"; }
        }

        public Index GetIndex()
        {
            var definition = new Index()
            {
                Name = this.Name,
                Fields = new[]
                {
                    new Field("ProductID",      DataType.String) { IsKey = true,  IsSearchable = false, IsFilterable = false, IsSortable = false, IsFacetable = false, IsRetrievable = true},
                    new Field("Name",           DataType.String) { IsKey = false, IsSearchable = true, IsFilterable = false, IsSortable = true, IsFacetable = false, IsRetrievable = true, Analyzer = AnalyzerName.EnMicrosoft},
                    new Field("ProductNumber",  DataType.String) { IsKey = false, IsSearchable = true,  IsFilterable = true, IsSortable = false, IsFacetable = false,  IsRetrievable = true },
                    new Field("Color",          DataType.String) { IsKey = false, IsSearchable = true, IsFilterable = true, IsSortable = false, IsFacetable = false, IsRetrievable = true },
                    new Field("StandardCost",   DataType.Double) { IsKey = false, IsSearchable = false, IsFilterable = true,  IsSortable = false, IsFacetable = true,  IsRetrievable = true},
                    new Field("ListPrice",      DataType.Double) { IsKey = false, IsSearchable = false,  IsFilterable = true, IsSortable = true,  IsFacetable = true, IsRetrievable = true },
                    new Field("Size",           DataType.String) { IsKey = false, IsSearchable = false,  IsFilterable = true, IsSortable = false,  IsFacetable = true,  IsRetrievable = true },
                    new Field("Weight",         DataType.Double) { IsKey = false, IsSearchable = false,  IsFilterable = true, IsSortable = false, IsFacetable = true, IsRetrievable = true },
                    new Field("CategoryName",   DataType.String) { IsKey = false, IsSearchable = true,  IsFilterable = true, IsSortable = false, IsFacetable = true, IsRetrievable = true, Analyzer = AnalyzerName.EnMicrosoft },
                    new Field("ModelName",      DataType.String) { IsKey = false, IsSearchable = true,  IsFilterable = true, IsSortable = false, IsFacetable = true, IsRetrievable = true, Analyzer = AnalyzerName.EnMicrosoft },
                    new Field("Description",    DataType.String) { IsKey = false, IsSearchable = true,  IsFilterable = false, IsSortable = false, IsFacetable = false, IsRetrievable = true, Analyzer = AnalyzerName.EnMicrosoft},
                    new Field("ThumbnailPhoto", DataType.String) { IsKey = false, IsSearchable = false,  IsFilterable = false, IsSortable = false, IsFacetable = false, IsRetrievable = true }
                },
                Suggesters = new List<Suggester>()
                {
                    new Suggester("name_number", SuggesterSearchMode.AnalyzingInfixMatching, "Name", "ProductNumber")
                }
            };

            return definition;
        }

        private string DataSourceName
        {
            get { return "products-datasource"; }
        }

        public DataSource GetDataSource()
        {
            var settings = new JobSettings();

            return new DataSource()
            {
                Name = this.DataSourceName,
                Description = $"{this.Name} datasource",
                Type = DataSourceType.AzureSql,
                Credentials = new DataSourceCredentials(settings.SourceConnectionString),
                Container = new DataContainer("ProductsSearchView"),
                DataDeletionDetectionPolicy = new SoftDeleteColumnDeletionDetectionPolicy()
                {
                    SoftDeleteColumnName = "IsDeleted",
                    SoftDeleteMarkerValue = "1"
                }
            };
        }

        public string IndexerName
        {
            get { return "products-indexer"; }
        }

        public Indexer GetIndexer()
        {
            return new Indexer()
            {
                Name = this.IndexerName,
                Description = $"{this.Name} rebuild indexer",
                DataSourceName = this.DataSourceName,
                TargetIndexName = this.Name,
            };
        }
    }
}
