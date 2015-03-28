using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace NewCC2015.AzureSearch.CreateSearchIndex
{
    class Program
    {
        private static readonly string searchServiceName = ConfigurationManager.AppSettings["SearchServiceName"];
        private static readonly string searchIndex = ConfigurationManager.AppSettings["SearchIndexName"];
        private static readonly string searchApiKey = ConfigurationManager.AppSettings["SearchApiKey"];
        private static readonly SearchCredentials searchCredentials = new SearchCredentials(searchApiKey);
        private static readonly SearchServiceClient searchClient = new SearchServiceClient(searchServiceName, searchCredentials);

        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                await DropAndCreateIndex();
            }).Wait();
        }

        static async Task DropAndCreateIndex()
        {
            if (await searchClient.Indexes.ExistsAsync(searchIndex))
            {
                //await searchClient.Indexes.DeleteAsync(searchIndex);
            }

            var index = new Index()
            {
                Name = searchIndex,
                Fields = new List<Field>()
                {
                    new Field("tweetId", DataType.String){ IsKey = true, IsSearchable = false, IsFilterable = false, IsSortable = false, IsRetrievable = true, IsFacetable = false},
                    new Field("source", DataType.String){ IsKey = false, IsSearchable = true, IsFilterable = true, IsSortable = true, IsRetrievable = true, IsFacetable = true},
                    new Field("text", DataType.String){ IsKey = false, IsSearchable = true, IsFilterable = true, IsSortable = true, IsRetrievable = true, IsFacetable = false},
                    new Field("screenName", DataType.String){ IsKey = false, IsSearchable = true, IsFilterable = true, IsSortable = true, IsRetrievable = true, IsFacetable = false},
                    new Field("profileBackgroundColor", DataType.String){ IsKey = false, IsSearchable = false, IsFilterable = true, IsSortable = false, IsRetrievable = true, IsFacetable = true},
                    new Field("profileImageUrl", DataType.String){ IsKey = false, IsSearchable = false, IsFilterable = true, IsSortable = true, IsRetrievable = true, IsFacetable = false},
                    new Field("timeZone", DataType.String){ IsKey = false, IsSearchable = false, IsFilterable = true, IsSortable = true, IsRetrievable = true, IsFacetable = true},
                    new Field("followers", DataType.Int64){ IsKey = false, IsSearchable = false, IsFilterable = true, IsSortable = true, IsRetrievable = true, IsFacetable = true},
                    new Field("following", DataType.Int64){ IsKey = false, IsSearchable = false, IsFilterable = true, IsSortable = true, IsRetrievable = true, IsFacetable = true},
                    new Field("retweets", DataType.Int64){ IsKey = false, IsSearchable = false, IsFilterable = true, IsSortable = true, IsRetrievable = true, IsFacetable = true},
                    new Field("statuses", DataType.Int64){ IsKey = false, IsSearchable = false, IsFilterable = true, IsSortable = true, IsRetrievable = true, IsFacetable = true},
                    new Field("createdAt", DataType.DateTimeOffset){ IsKey = false, IsSearchable = false, IsFilterable = true, IsSortable = true, IsRetrievable = true, IsFacetable = true},
                },
                Suggesters = new List<Suggester>()
                {
                    new Suggester()
                    {
                        Name = "textSuggester",
                        SearchMode = SuggesterSearchMode.AnalyzingInfixMatching,
                        SourceFields = new [] {"text"}
                    }
                }
            };
            
            await searchClient.Indexes.CreateOrUpdateAsync(index);
        }
    }
}
