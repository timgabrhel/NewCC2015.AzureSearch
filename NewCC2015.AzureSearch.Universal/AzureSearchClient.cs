using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Documents;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using NewCC2015.AzureSearch.Core;

namespace NewCC2015.AzureSearch.Universal
{
    public class AzureSearchClient
    {
        private string searchServiceName;
        private string searchIndex;
        private string searchApiKey;
        private SearchCredentials searchCredentials;
        private SearchServiceClient searchClient;

        public AzureSearchClient()
        {
            searchServiceName = Utility.GetResource<string>("SearchServiceName");
            searchIndex = Utility.GetResource<string>("SearchIndexName");
            searchApiKey = Utility.GetResource<string>("SearchApiKey");

            searchCredentials = new SearchCredentials(searchApiKey);
            searchClient = new SearchServiceClient(searchServiceName, searchCredentials);
        }

        public async Task<DocumentSearchResponse<T>> Search<T>(string query) where T : class 
        {
            var indexClient = searchClient.Indexes.GetClient(searchIndex);

            var sp = new SearchParameters();
            sp.Facets = new[] { "source", "retweets", "following", "followers,values:1000|5000|10000|50000|100000" };

            return await indexClient.Documents.SearchAsync<T>(query, sp);
        }
    }
}
